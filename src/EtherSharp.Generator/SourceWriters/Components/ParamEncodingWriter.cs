using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters.Components;

public class ParamEncodingWriter(AbiParameterTypeWriter parameterTypeWriter)
{
    private readonly AbiParameterTypeWriter _parameterTypeWriter = parameterTypeWriter;

    public (string ParamName, string TypeName, Func<string, string> EncodeFunc) GetInputEncoding(AbiParameter parameter, int paramIndex)
    {
        string paramName = String.IsNullOrWhiteSpace(parameter.Name)
            ? $"param{paramIndex}"
            : NameUtils.ToValidParameterName(parameter.Name);
        var (paramType, _, encodeFunction, _) = _parameterTypeWriter.CreateParameter(parameter);

        return (paramName, paramType, encodeFunction);

    }

    public (string TypeName, string DecodeFunc) GetOutputDecoding(string fallbackName, AbiParameter[] outputParameters)
    {
        var outputParameter = outputParameters.Length == 1
            ? outputParameters[0]
            : new AbiParameter(fallbackName, "anonymous-tuple", fallbackName, outputParameters);

        var (csTypeName, _, _, decodeFunc) = _parameterTypeWriter.CreateParameter(outputParameter);

        return (csTypeName, decodeFunc);
    }
}
