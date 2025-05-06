using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.SyntaxElements;

namespace EtherSharp.Generator.SourceWriters;
public class ParamDecodingWriter(AbiParameterTypeWriter parameterTypeWriter)
{
    private readonly AbiParameterTypeWriter _parameterTypeWriter = parameterTypeWriter;

    public (string, string) SetQueryOutputDecoding(string functionName, FunctionBuilder function, AbiParameter[] outputParameters)
    {
        var outputParameter = outputParameters.Length == 1
            ? outputParameters[0]
            : new AbiParameter($"{functionName}Result", "tuple", $"{functionName}Result", outputParameters);

        var (csTypeName, _, _, decodeFunc) = _parameterTypeWriter.CreateParameter(outputParameter);

        function.WithReturnTypeRaw($"{typeof(Task).FullName}<{csTypeName}>");
        return (csTypeName, decodeFunc);
    }

    public (string, string) SetMessageOutputDecoding(string functionName, FunctionBuilder function, AbiParameter[] outputParameters)
    {
        var outputParameter = outputParameters.Length == 1
            ? outputParameters[0]
            : new AbiParameter($"{functionName}Result", "tuple", $"{functionName}Result", outputParameters);

        var (csTypeName, _, _, decodeFunc) = _parameterTypeWriter.CreateParameter(outputParameter);

        function.WithReturnTypeRaw($"EtherSharp.Tx.ITxInput<{csTypeName}>");
        return (csTypeName, decodeFunc);
    }
}
