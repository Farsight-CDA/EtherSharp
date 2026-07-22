using EtherSharp.Generator.ABI.Members;
using EtherSharp.Generator.ABI.Util;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.ABI.SourceWriters.Components;

internal sealed class ErrorTypeWriter
{
    private readonly FunctionBuilder _isMatchingSelectorFunction = new FunctionBuilder("IsMatchingSelector")
        .AddArgument("System.ReadOnlySpan<byte>", "errorData")
        .WithReturnType<bool>()
        .WithIsStatic()
        .AddStatement($"return errorData.Length >= 4 && Selector == EtherSharp.Types.Bytes4.FromBytes(errorData[0..4])");

    public ClassBuilder GenerateErrorType(string errorTypeName, ErrorAbiMember errorMember)
    {
        var errorTypeBuilder = new ClassBuilder(errorTypeName)
            .AddBaseType($"EtherSharp.Contract.Sections.ISolidityError<{errorTypeName}>", true)
            .WithAutoConstructor();
        var usedNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "Decode",
            "Encode",
            "EncodeData",
            "IsMatchingSelector",
            "TryDecode",
            "ErrorSignature",
            "SelectorHex",
            "Selector"
        };

        var decodeMethod = new FunctionBuilder("Decode")
            .WithReturnTypeRaw(errorTypeName)
            .WithIsStatic(true)
            .AddArgument("System.ReadOnlyMemory<byte>", "data")
            .AddStatement("var decoder = new EtherSharp.ABI.AbiDecoder(data[4..])");

        var encodeMethod = new FunctionBuilder("Encode")
            .WithReturnTypeRaw("byte[]")
            .WithVisibility(FunctionVisibility.Public);

        var encodeDataMethod = new FunctionBuilder("EncodeData")
            .WithReturnTypeRaw("byte[]")
            .WithVisibility(FunctionVisibility.Public)
            .WithIsStatic()
            .AddStatement("var encoder = new EtherSharp.ABI.AbiEncoder()");

        var errorTypeCtorCall = new ConstructorCallBuilder(errorTypeName);
        var propertyNames = new List<string>();

        for(int i = 0; i < errorMember.Inputs.Length; i++)
        {
            var parameter = errorMember.Inputs[i];

            if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out string primitiveType, out _, out string abiFunctionName, out string decodeSuffix))
            {
                throw new NotSupportedException("ABI Error can only contain primitive types");
            }

            string parameterName = NameUtils.ToValidPropertyName(parameter.Name);
            if(String.IsNullOrWhiteSpace(parameterName))
            {
                throw new NotSupportedException("ABI Error member must all have a name");
            }

            parameterName = NameUtils.MakeUniquePropertyName(parameterName, usedNames);
            propertyNames.Add(parameterName);

            errorTypeBuilder.AddProperty(
                new PropertyBuilder(primitiveType, parameterName)
                    .WithVisibility(PropertyVisibility.Public)
                    .WithSetterVisibility(SetterVisibility.None)
            );

            string tempVarName = $"param{i}";
            decodeMethod.AddStatement($"var {tempVarName} = decoder.{abiFunctionName}(){decodeSuffix}");
            errorTypeCtorCall.AddArgument(tempVarName);
            encodeDataMethod.AddArgument(primitiveType, parameterName);
            encodeDataMethod.AddStatement($"encoder.{abiFunctionName}({parameterName})");
        }

        decodeMethod.AddStatement($"return {errorTypeCtorCall.ToInlineCall()}");
        encodeMethod.AddStatement($"return EncodeData({String.Join(", ", propertyNames)})");
        encodeDataMethod.AddStatement(
            """
            byte[] data = new byte[4 + encoder.Size];
            Selector.CopyTo(data);
            encoder.TryWriteTo(data.AsSpan(4));
            return data;
            """, false
        );

        errorTypeBuilder.AddFunction(decodeMethod);
        errorTypeBuilder.AddFunction(encodeMethod);
        errorTypeBuilder.AddFunction(encodeDataMethod);
        errorTypeBuilder.AddFunction(_isMatchingSelectorFunction);

        errorTypeBuilder.AddFunction(new FunctionBuilder("TryDecode")
            .AddArgument("System.ReadOnlyMemory<byte>", "errorData")
            .WithReturnType<bool>()
            .WithIsStatic()
            .AddArgument($"[System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out {errorTypeName}", "parsedError")
            .AddStatement(
                $$"""
                if (!IsMatchingSelector(errorData.Span)) 
                {
                    parsedError = null;
                    return false;
                }

                parsedError = Decode(errorData);
                return true;
                """
            )
        );

        return errorTypeBuilder;
    }
}
