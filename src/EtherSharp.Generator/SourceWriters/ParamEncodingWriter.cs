using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;
public class ParamEncodingWriter(AbiTypeWriter typeWriter)
{
    private readonly AbiTypeWriter _typeWriter = typeWriter;

    public string AddParameterEncoding(FunctionBuilder function, AbiInputParameter parameter, int paramIndex)
    {
        string paramName = string.IsNullOrWhiteSpace(parameter.Name)
            ? $"param{paramIndex}"
            : NameUtils.ToValidParameterName(parameter.Name);
        var (paramType, encoderFunction, _) = GetParameterEncoding(parameter, paramName);

        function.AddArgument(paramType, paramName);
        function.AddStatement(encoderFunction);

        return paramName;
    }

    private (string ParamType, string EncoderFunction, bool IsDynamicType) GetParameterEncoding(AbiInputParameter parameter, string paramName)
    {
        if(TryGetPrimitiveEquivalentType(parameter.Type, out string primitiveType, out bool isDynamicType))
        {
            string encoderFunction =
                $"""
                encoder.{GetPrimitiveABIEncodingMethodName(parameter.Type)}({paramName});
                """;

            return (primitiveType, encoderFunction, isDynamicType);
        }

        switch(parameter.Type)
        {
            default:
                throw new NotSupportedException($"Parameters of type {parameter.Type} are not supported in this version.");
            case "tuple":
            {
                return GenerateTupleType(parameter, paramName);
            }
            case "tuple[]":
            {
                var (typeName, encodingMethod, isTupleDynamicType) = GenerateTupleType(parameter, "value");
                string encoderFunction =
                    $$"""
                    encoder.Array(encoder => 
                    {
                        foreach(var value in {{paramName}}) 
                        {
                        {{encodingMethod}}
                        }
                    });
                    """;

                return ($"{typeName}[]", encoderFunction, isTupleDynamicType);
            }
        }
    }

    private (string TupleTypeName, string EncodingMethod, bool IsDynamicType) GenerateTupleType(AbiInputParameter tupleParameter, string paramName)
    {
        if(tupleParameter.Components is null || tupleParameter.Components.Length == 0)
        {
            throw new NotSupportedException("Tuple parameters must include a non-empty components array");
        }
        if(tupleParameter.InternalType is null || string.IsNullOrEmpty(tupleParameter.InternalType))
        {
            throw new NotSupportedException("Tuple parameter must include an InternalType");
        }

        string internalName = tupleParameter.InternalType.Split('.').Last().Trim('[', ']');
        string className = NameUtils.ToValidClassName(internalName);

        var encodingFunctionBuilder = new StringBuilder();
        var classBuilder = new ClassBuilder(className)
            .WithVisibility(ClassVisibility.Public);

        bool isDynamicType = false;

        foreach(var component in tupleParameter.Components)
        {
            string propertyName = NameUtils.ToValidPropertyName(component.Name);
            var (componentType, componentEncodingFunction, isInnerDynamicType) = GetParameterEncoding(component, $"{paramName}.{propertyName}");

            if(isInnerDynamicType && !isDynamicType)
            {
                isDynamicType = true;
            }

            classBuilder.AddProperty(new PropertyBuilder(componentType, propertyName)
                .WithIsRequired()
                .WithSetterVisibility(SetterVisibility.Init)
            );
            encodingFunctionBuilder.AppendLine(componentEncodingFunction);
        }

        _typeWriter.RegisterTypeBuilder(classBuilder);

        string encodingFunction =
            $$"""
            encoder.{{(isDynamicType ? "DynamicTuple" : "FixedTuple")}}(encoder => 
            {
            {{encodingFunctionBuilder}}
            });
            """;

        return (className, encodingFunction, isDynamicType);
    }

    private bool TryGetPrimitiveEquivalentType(string solidityType, out string type, out bool isDynamicType)
    {
        var (pt, idt) = solidityType switch
        {
            "address" => (typeof(string).FullName, false),
            "address[]" => (typeof(string[]).FullName, false),
            "string" => (typeof(string).FullName, true),
            "string[]" => (typeof(string[]).FullName, true),
            "bool" => (typeof(bool).FullName, false),
            "bytes" => (typeof(byte[]).FullName, true),
            "bytes[]" => (typeof(byte[][]).FullName, true),
            string s when s.StartsWith("uint", StringComparison.Ordinal) && int.TryParse(s.Substring(4), out int bitSize)
                => bitSize % 8 != 0
                    ? throw new NotSupportedException("uint bitsize must be multiple of 8")
                    : (bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("uint bitsize must be between 8 and 256"),
                        8 => typeof(byte).FullName,
                        <= 16 => typeof(ushort).FullName,
                        <= 32 => typeof(uint).FullName,
                        <= 64 => typeof(ulong).FullName,
                        _ => typeof(BigInteger).FullName,
                    }, false),
            string s when s.StartsWith("int", StringComparison.Ordinal) && int.TryParse(s.Substring(3), out int bitSize)
                    => bitSize % 8 != 0
                    ? throw new NotSupportedException("int bitsize must be multiple of 8")
                    : (bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("int bitsize must be between 8 and 256"),
                        8 => typeof(sbyte).FullName,
                        <= 16 => typeof(short).FullName,
                        <= 32 => typeof(int).FullName,
                        <= 64 => typeof(long).FullName,
                        _ => typeof(BigInteger).FullName,
                    }, false),
            string s when s.StartsWith("bytes", StringComparison.Ordinal) && int.TryParse(s.Substring(5), out int bitSize)
                => (bitSize switch
                {
                    < 1 or > 32 => throw new NotSupportedException("bytes bitsize must be between 8 and 256"),
                    1 => typeof(byte).FullName,
                    _ => typeof(byte[]).FullName,
                }, false),
            _ => (null, false)
        };

        type = pt!;
        isDynamicType = idt;

        return type is not null;
    }

    private static string GetPrimitiveABIEncodingMethodName(string solidityType)
        => solidityType switch
        {
            string s when s.EndsWith("[]", StringComparison.OrdinalIgnoreCase)
                => $"{GetPrimitiveABIEncodingMethodName(s.Substring(0, s.Length - 2))}Array",
            string s when s.StartsWith("uint", StringComparison.Ordinal)
                => s.Substring(0, 2).ToUpper(CultureInfo.InvariantCulture) + s.Substring(2),
            _ => NameUtils.ToValidFunctionName(solidityType),
        };
}
