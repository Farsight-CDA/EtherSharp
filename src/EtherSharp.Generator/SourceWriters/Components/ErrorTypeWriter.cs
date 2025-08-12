using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters.Components;
public class ErrorTypeWriter
{
    public ClassBuilder GenerateErrorType(string typeName, ErrorAbiMember errorMember)
    {
        var classBuilder = new ClassBuilder(typeName)
            .WithAutoConstructor();

        var decodeMethod = new FunctionBuilder("Decode")
            .WithReturnTypeRaw(typeName)
            .WithIsStatic(true)
            .AddArgument("EtherSharp.ABI.AbiDecoder", "decoder");

        var constructorCall = new ConstructorCallBuilder(
            typeName
        );

        for(int i = 0; i < errorMember.Inputs.Length; i++)
        {
            var parameter = errorMember.Inputs[i];

            if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out string primitiveType, out _, out string abiFunctionName, out string decodeSuffix))
            {
                throw new NotSupportedException("ABI Error can only contain primitive types");
            }

            string parameterName = NameUtils.ToValidPropertyName(parameter.Name);
            if(string.IsNullOrWhiteSpace(parameterName))
            {
                throw new NotSupportedException("ABI Error member must all have a name");
            }

            classBuilder.AddProperty(
                new PropertyBuilder(primitiveType, parameterName)
                    .WithVisibility(PropertyVisibility.Public)
                    .WithSetterVisibility(SetterVisibility.None)
            );

            string tempVarName = $"param{i}";
            decodeMethod.AddStatement($"var {tempVarName} = decoder.{abiFunctionName}(){decodeSuffix}");
            constructorCall.AddArgument(tempVarName);
        }

        decodeMethod.AddStatement($"return {constructorCall.ToInlineCall()}");

        classBuilder.AddFunction(decodeMethod);

        return classBuilder;
    }
}
