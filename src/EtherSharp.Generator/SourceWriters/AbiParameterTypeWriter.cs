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
        if(TryMatchPrimitiveType(parameter, out string csTypeName, out bool isDynamic, out var encodeFunc, out var decodeFunc))
        {
            return (csTypeName, isDynamic, encodeFunc, decodeFunc);
        }
        else if(TryMatchArrayType(parameter, out csTypeName, out encodeFunc, out decodeFunc))
        {
            return (csTypeName, true, encodeFunc, decodeFunc);
        }
        else if(TryMatchTupleType(parameter, out csTypeName, out isDynamic, out encodeFunc, out decodeFunc))
        {
            return (csTypeName, isDynamic, encodeFunc, decodeFunc);
        }

        throw new NotSupportedException($"Parameter {parameter.Name} of type {parameter.Type} not supported");
    }

    private bool TryMatchPrimitiveType(AbiParameter parameter, out string csTypeName, out bool isDynamic, out Func<string, string> encodeFunc, out string decodeFunc)
    {
        if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out csTypeName, out isDynamic, out var abiFunctionName))
        {
            encodeFunc = null!;
            decodeFunc = null!;
            return false;
        }

        encodeFunc = inputName => $"encoder.{abiFunctionName}({inputName});";
        decodeFunc = $"decoder.{abiFunctionName}()";
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

        var innerParameter = new AbiParameter("value", parameter.Type.Substring(0, parameter.Type.Length - 2), null, null);
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

    private bool TryMatchTupleType(AbiParameter tupleParameter, out string csTypeName, out bool isDynamic, out Func<string, string> encodeFunc, out string decodeFunc)
    {
        if(tupleParameter.Type != "tuple" || string.IsNullOrEmpty(tupleParameter.InternalType) || tupleParameter.Components is null)
        {
            csTypeName = null!;
            isDynamic = false;
            encodeFunc = null!;
            decodeFunc = null!;
            return false;
        }

        isDynamic = false;
        csTypeName = NameUtils.ToValidClassName(tupleParameter.InternalType!.Split(' ', '.').Last());
        var tupleClassBuilder = new ClassBuilder(csTypeName)
            .WithAutoConstructor();

        var encodeFunctionBuilder = new FunctionBuilder("Encode")
            .AddArgument("EtherSharp.ABI.AbiEncoder", "encoder");
        var decodeFunctionBuilder = new FunctionBuilder("Decode")
            .WithIsStatic()
            .WithReturnTypeRaw(csTypeName)
            .AddArgument("EtherSharp.ABI.AbiDecoder", "decoder");
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

        decodeFunctionBuilder.AddStatement(
            $"""
            return new {csTypeName}({decodeCtorBuilder})
            """);

        tupleClassBuilder.AddFunction(encodeFunctionBuilder);
        tupleClassBuilder.AddFunction(decodeFunctionBuilder);

        _typeWriter.RegisterTypeBuilder(tupleClassBuilder);

        encodeFunc = inputName => $"{inputName}.Encode(encoder);";
        decodeFunc = $"{csTypeName}.Decode(decoder)";
        return true;
    }
}
