using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Text;

namespace EtherSharp.Generator.SourceWriters.Components;

internal class MemberTypeWriter(ParamEncodingWriter paramEncodingWriter)
{
    private readonly ParamEncodingWriter _paramEncodingWriter = paramEncodingWriter;

    public void AddInputProperties(ClassBuilder classBuilder, AbiParameter[] inputs, IReadOnlyCollection<string>? reservedPropertyNames = null)
    {
        var usedNames = new HashSet<string>(StringComparer.Ordinal);

        if(reservedPropertyNames is not null)
        {
            foreach(string reservedPropertyName in reservedPropertyNames)
            {
                usedNames.Add(reservedPropertyName);
            }
        }

        for(int i = 0; i < inputs.Length; i++)
        {
            var (outputTypeName, _, _) = _paramEncodingWriter.GetOutputDecoding(
                $"Param{i + 1}",
                [inputs[i]]
            );

            string parameterName = NameUtils.ToValidPropertyName(inputs[i].Name);

            if(String.IsNullOrEmpty(parameterName))
            {
                parameterName = $"anonymousArgument{i + 1}";
            }

            parameterName = NameUtils.MakeUniquePropertyName(parameterName, usedNames);

            classBuilder.AddProperty(
                new PropertyBuilder(outputTypeName, parameterName)
                    .WithVisibility(PropertyVisibility.Public)
                    .WithSetterVisibility(SetterVisibility.None)
            );
        }
    }

    public string GenerateDecodeStatements(string typeName, AbiParameter[] inputs, string dataExpression, string? extraCtorArg = null)
    {
        var ctorBuilder = new ConstructorCallBuilder(typeName);
        if(extraCtorArg != null)
        {
            ctorBuilder.AddArgument(extraCtorArg);
        }

        var statementBuilder = new StringBuilder();

        if(inputs.Length != 0)
        {
            statementBuilder.AppendLine($"EtherSharp.ABI.AbiDecoder decoder = new EtherSharp.ABI.AbiDecoder({dataExpression});");
        }

        for(int i = 0; i < inputs.Length; i++)
        {
            var parameter = inputs[i];
            var (outputTypeName, _, decodeFunc) = _paramEncodingWriter.GetOutputDecoding(
                $"Param{i + 1}",
                [parameter]
            );

            statementBuilder.AppendLine($"{outputTypeName} parameter{i} = {decodeFunc};");
            ctorBuilder.AddArgument($"parameter{i}");
        }

        statementBuilder.AppendLine($"return {ctorBuilder.ToInlineCall()};");
        return statementBuilder.ToString();
    }
}
