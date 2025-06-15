using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters;
public class ParamEncodingWriter(AbiParameterTypeWriter parameterTypeWriter)
{
    public string AddParameterEncoding(FunctionBuilder function, AbiParameter parameter, int paramIndex)
    {
        string paramName = string.IsNullOrWhiteSpace(parameter.Name)
            ? $"param{paramIndex}"
            : NameUtils.ToValidParameterName(parameter.Name);
        var (paramType, _, encodeFunction, _) = parameterTypeWriter.CreateParameter(parameter);

        function.AddArgument(paramType, paramName);
        function.AddStatement(encodeFunction(paramName));

        return paramName;
    }
}
