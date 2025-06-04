using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;
public class AbiParameterTypeWriter(AbiTypeWriter typeWriter)
{
    private readonly AbiTypeWriter _typeWriter = typeWriter;

    public (string CSTypeName, bool isDynamic, Func<string, string> EncodeFunc, string DecodeFunc) CreateParameter(AbiParameter parameter)
    {
        if(TryMatchPrimitiveType(parameter, out string csTypeName, out bool isDynamic, out var encodeFunc, out string decodeFunc))
        {
            return (csTypeName, isDynamic, encodeFunc, decodeFunc);
        }
        else if(TryMatchArrayType(parameter, out csTypeName, out encodeFunc, out decodeFunc))
        {
            return (csTypeName, true, encodeFunc, decodeFunc);
        }
        else if(TryMatchStructType(parameter, out csTypeName, out isDynamic, out encodeFunc, out decodeFunc))
        {
            return (csTypeName, isDynamic, encodeFunc, decodeFunc);
        }
        else if(TryMatchAnonymousTupleType(parameter, out csTypeName, out isDynamic, out encodeFunc, out decodeFunc))
        {
            return (csTypeName, isDynamic, encodeFunc, decodeFunc);
        }

        throw new NotSupportedException($"Parameter {parameter.Name} of type {parameter.Type} not supported");
    }

    private bool TryMatchPrimitiveType(AbiParameter parameter, out string csTypeName, out bool isDynamic, out Func<string, string> encodeFunc, out string decodeFunc)
    {
        if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out csTypeName, out isDynamic, out string abiFunctionName, out string decodeSuffix))
        {
            encodeFunc = null!;
            decodeFunc = null!;
            return false;
        }

        encodeFunc = inputName => $"encoder.{abiFunctionName}({inputName});";
        decodeFunc = $"decoder.{abiFunctionName}(){decodeSuffix}";
        return true;
    }

    private bool TryMatchArrayType(AbiParameter parameter, out string csTypeName, out Func<string, string> encodeFunc, out string decodeFunc)
    {
        if(!parameter.Type.EndsWith("[]"))
        {
            csTypeName = null!;
            encodeFunc = null!;
            decodeFunc = null!;
            return false;
        }

        string? internalType = parameter.InternalType is null
            ? null
            : parameter.InternalType.EndsWith("[]")
                ? parameter.InternalType.Substring(0, parameter.InternalType.Length - 2)
                : parameter.InternalType;

        var innerParameter = new AbiParameter("value", parameter.Type.Substring(0, parameter.Type.Length - 2), internalType, parameter.Components);
        var (innerCSharpType, _, innerEncodeFunction, innerDecodeFunction) = CreateParameter(innerParameter);

        csTypeName = $"{innerCSharpType}[]";
        encodeFunc = inputName =>
            $$"""
            encoder.Array({{inputName}}, (encoder, value) => {
            {{innerEncodeFunction("value")}}
            });
            """;
        decodeFunc =
            $$"""
            decoder.Array(decoder => {{innerDecodeFunction}})
            """;

        return true;
    }

    private bool TryMatchStructType(AbiParameter tupleParameter, out string csTypeName, out bool isDynamic, out Func<string, string> encodeFunc, out string decodeFunc)
    {
        if(!tupleParameter.Type.Contains("tuple")
            || string.IsNullOrEmpty(tupleParameter.InternalType)
            || tupleParameter.Components is null
            || tupleParameter.Components.Any(x => string.IsNullOrEmpty(x.Name)))
        {
            csTypeName = null!;
            isDynamic = false;
            encodeFunc = null!;
            decodeFunc = null!;
            return false;
        }

        bool isAnonymous = tupleParameter.Type.Contains("anonymous");

        isDynamic = false;
        csTypeName = NameUtils.ToValidClassName(tupleParameter.InternalType!.Split(' ', '.').Last());
        var tupleClassBuilder = new ClassBuilder(csTypeName)
            .WithAutoConstructor();

        var encodeFunctionBuilder = new FunctionBuilder("Encode");
        var decodeFunctionBuilder = new FunctionBuilder("Decode")
            .WithIsStatic()
            .WithReturnTypeRaw(csTypeName);
        var decodeCtorBuilder = new StringBuilder();

        foreach(var parameter in tupleParameter.Components)
        {
            var (innerCsType, innerIsDynamic, innerEncodeFunc, innerDecodeFunc) = CreateParameter(parameter);

            if(innerIsDynamic && !isDynamic)
            {
                isDynamic = true;
            }

            string propertyName = NameUtils.ToValidPropertyName(parameter.Name);
            tupleClassBuilder.AddProperty(new PropertyBuilder(innerCsType, propertyName));

            encodeFunctionBuilder.AddStatement(innerEncodeFunc(propertyName), false);

            if(decodeCtorBuilder.Length > 0)
            {
                decodeCtorBuilder.Append(',');
            }

            decodeCtorBuilder.AppendLine(innerDecodeFunc);
        }

        if(isDynamic)
        {
            encodeFunctionBuilder.AddArgument("EtherSharp.ABI.Encode.Interfaces.IDynamicTupleEncoder", "encoder");
            decodeFunctionBuilder.AddArgument("EtherSharp.ABI.Decode.Interfaces.IDynamicTupleDecoder", "decoder");
        }
        else
        {
            encodeFunctionBuilder.AddArgument("EtherSharp.ABI.Encode.Interfaces.IFixedTupleEncoder", "encoder");
            decodeFunctionBuilder.AddArgument("EtherSharp.ABI.Decode.Interfaces.IFixedTupleDecoder", "decoder");
        }

        decodeFunctionBuilder.AddStatement(
            $"""
            return new {csTypeName}({decodeCtorBuilder})
            """);

        tupleClassBuilder.AddFunction(encodeFunctionBuilder);
        tupleClassBuilder.AddFunction(decodeFunctionBuilder);

        _typeWriter.RegisterTypeBuilder(tupleClassBuilder);

        bool localIsDynamic = isDynamic;
        encodeFunc = inputName => isAnonymous
            ? $"{inputName}.Encode(encoder)"
            : localIsDynamic
                ? $"encoder.DynamicTuple(encoder => {inputName}.Encode(encoder));"
                : $"encoder.FixedTuple(encoder => {inputName}.Encode(encoder));";
        decodeFunc = isAnonymous
            ? $"{csTypeName}.Decode(decoder)"
            : localIsDynamic
                ? $"decoder.DynamicTuple(decoder => {csTypeName}.Decode(decoder))"
                : $"decoder.FixedTuple(decoder => {csTypeName}.Decode(decoder))";
        return true;
    }

    private bool TryMatchAnonymousTupleType(AbiParameter tupleParameter, out string csTypeName, out bool isDynamic, out Func<string, string> encodeFunc, out string decodeFunc)
    {
        if(tupleParameter.Type != "anonymous-tuple"
            || tupleParameter.Components is null)
        {
            csTypeName = null!;
            isDynamic = false;
            encodeFunc = null!;
            decodeFunc = null!;
            return false;
        }

        isDynamic = false;
        var typeNameBuilder = new StringBuilder();
        var encodeFuncBuilder = new StringBuilder();
        var decodeFuncBuilder = new StringBuilder();

        for(int i = 0; i < tupleParameter.Components.Length; i++)
        {
            var parameter = tupleParameter.Components[i];
            var (innerCsType, innerIsDynamic, innerEncodeFunc, innerDecodeFunc) = CreateParameter(parameter);

            if(innerIsDynamic && !isDynamic)
            {
                isDynamic = true;
            }

            bool isLast = parameter == tupleParameter.Components[tupleParameter.Components.Length - 1];

            typeNameBuilder.Append(innerCsType);
            decodeFuncBuilder.Append(innerDecodeFunc);

            if(!isLast)
            {
                typeNameBuilder.Append(", ");
                encodeFuncBuilder.AppendLine(innerEncodeFunc($"###.Item{i + 1}"));
                decodeFuncBuilder.AppendLine(", ");
            }
        }

        csTypeName = $"({typeNameBuilder})";
        encodeFunc = inputName => encodeFuncBuilder.ToString().Replace("###", inputName);
        decodeFunc =
            $"""
            (
            {decodeFuncBuilder}
            )
            """;

        return true;
    }
}
