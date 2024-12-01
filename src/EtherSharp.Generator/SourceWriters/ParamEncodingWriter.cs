using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Data.Common;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;
public class ParamEncodingWriter(AbiTypeWriter typeWriter)
{
    private readonly AbiTypeWriter _typeWriter = typeWriter;

    public string AddParameterEncoding(FunctionBuilder function, AbiInputParameter parameter)
    {
        string paramName = NameUtils.ToValidParameterName(parameter.Name);
        var (paramType, encoderFunction) = GetParameterEncoding(parameter, paramName);

        function.AddArgument(paramType, paramName);
        function.AddStatement(encoderFunction);

        return paramName;
    }

    private (string ParamType, string EncoderFunction) GetParameterEncoding(AbiInputParameter parameter, string paramName)
    {
        if(TryGetPrimitiveEquivalentType(parameter.Type, out string primitiveType))
        {
            string encoderFunction =
                $"""
                encoder.{GetPrimitiveABIEncodingMethodName(parameter.Type)}({paramName});
                """;

            return (primitiveType, encoderFunction);
        }

        switch(parameter.Type)
        {
            default:
                throw new NotSupportedException($"Parameters of type {parameter.Type} are not supported in this version.");
            case "tuple":
            {
                var (typeName, encodingMethod) = GenerateTupleType(parameter, paramName);
                return (typeName, encodingMethod);
            }
            case "tuple[]":
            {
                var (typeName, encodingMethod) = GenerateTupleType(parameter, "value");
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

                return ($"{typeName}[]", encoderFunction);
            }
        }
    }

    private (string TupleTypeName, string EncodingMethod) GenerateTupleType(AbiInputParameter tupleParameter, string paramName)
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

        foreach(var component in tupleParameter.Components)
        {
            string propertyName = NameUtils.ToValidPropertyName(component.Name);
            var (componentType, componentEncodingFunction) = GetParameterEncoding(component, $"{paramName}.{propertyName}");

            classBuilder.AddProperty(new PropertyBuilder(componentType, propertyName));
            encodingFunctionBuilder.AppendLine(componentEncodingFunction);
        }

        _typeWriter.RegisterTypeBuilder(classBuilder);

        string encodingFunction =
            $$"""
            encoder.Struct(encoder => 
            {
            {{encodingFunctionBuilder}}
            });
            """;

        return (className, encodingFunction);
    }

    private bool TryGetPrimitiveEquivalentType(string solidityType, out string type)
    {
        type = (solidityType switch
        {
            "address" => typeof(string).FullName,
            "string" => typeof(string).FullName,
            "bool" => typeof(bool).FullName,
            "bytes" => typeof(byte[]).FullName,
            string s when s.StartsWith("uint") && int.TryParse(s.Substring(4), out int bitSize)
                => bitSize % 8 != 0
                    ? throw new NotSupportedException("uint bitsize must be multiple of 8")
                    : bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("uint bitsize must be between 8 and 256"),
                        8 => typeof(byte).FullName,
                        <= 16 => typeof(ushort).FullName,
                        <= 32 => typeof(uint).FullName,
                        <= 64 => typeof(ulong).FullName,
                        _ => typeof(BigInteger).FullName,
                    },
            string s when s.StartsWith("int") && int.TryParse(s.Substring(3), out int bitSize)
                    => bitSize % 8 != 0
                    ? throw new NotSupportedException("int bitsize must be multiple of 8")
                    : bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("int bitsize must be between 8 and 256"),
                        8 => typeof(sbyte).FullName,
                        <= 16 => typeof(short).FullName,
                        <= 32 => typeof(int).FullName,
                        <= 64 => typeof(long).FullName,
                        _ => typeof(BigInteger).FullName,
                    },
            string s when s.StartsWith("bytes") && int.TryParse(s.Substring(5), out int bitSize)
                => bitSize switch
                {
                    < 1 or > 32 => throw new NotSupportedException("bytes bitsize must be between 8 and 256"),
                    1 => typeof(byte).FullName,
                    _ => typeof(byte[]).FullName,
                },
            _ => null
        })!;
        return type is not null;
    }

    private static string GetPrimitiveABIEncodingMethodName(string solidityType)
        => solidityType switch
        {
            string s when s.StartsWith("uint") => s.Substring(0, 2).ToUpper() + s.Substring(2),
            _ => NameUtils.ToValidFunctionName(solidityType),
        };
}
