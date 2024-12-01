using EtherSharp.Generator.Abi;
using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Numerics;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;
public class ContractSourceWriter(AbiTypeWriter typeWriter, ParamEncodingWriter paramEncodingWriter, ParamDecodingWriter paramDecodingWriter)
{
    private readonly AbiTypeWriter _typeWriter = typeWriter;
    private readonly ParamEncodingWriter _paramEncodingWriter = paramEncodingWriter;
    private readonly ParamDecodingWriter _paramDecodingWriter = paramDecodingWriter;

    public string WriteContractSourceCode(string @namespace, string contractName, IEnumerable<AbiMember> members)
    {
        var contractInterface = new InterfaceBuilder(contractName)
            .WithIsPartial(true)
            .WithVisibility(InterfaceVisibility.Public);

        var contractImplementation = new ClassBuilder($"{contractName}_Generated_Implementation")
            .AddBaseType(contractName, true)
            .WithVisibility(ClassVisibility.Internal)
            .AddField(new FieldBuilder("EtherSharp.Client.IEtherClient", "_client")
                .WithIsReadonly(true)
                .WithVisibility(FieldVisibility.Private)
            ).AddProperty(new PropertyBuilder("System.String", "ContractAddress")
                .WithVisibility(PropertyVisibility.Public)
                .WithSetterVisibility(SetterVisibility.None)
            );

        foreach(var member in members.Where(x => x is FunctionAbiMember).Cast<FunctionAbiMember>())
        {
            byte[] signatureBytes = member.GetSignatureBytes();
            var signatureBytesField = new FieldBuilder("byte[]", GetFunctionSignatureFieldName(member))
                .WithIsStatic(true)
                .WithIsReadonly(true)
                .WithVisibility(FieldVisibility.Private)
                .WithDefaultValue($"[ {signatureBytes[0]}, {signatureBytes[1]}, {signatureBytes[2]}, {signatureBytes[3]} ]");

            contractImplementation.AddField(signatureBytesField);

            bool isQuery = member.StateMutability == StateMutability.Pure || member.StateMutability == StateMutability.View;

            var func = isQuery switch
            {
                true => GenerateQueryFunction(member),
                false => GenerateMessageFunction(member)
            };

            contractInterface.AddFunction(func);
            contractImplementation.AddFunction(func);
        }

        var output = new StringBuilder();

        output.AppendLine(
            $$"""
            namespace {{@namespace}};

            {{contractInterface.Build()}}
            {{contractImplementation.Build(generateFieldConstructor: true)}}
            """
        );

        foreach(var typeBuilder in _typeWriter.GetTypeBuilders())
        {
            output.AppendLine(typeBuilder.Build());
        }

        return output.ToString();
    }

    private FunctionBuilder GenerateQueryFunction(FunctionAbiMember queryFunction)
    {
        string functionName = NameUtils.ToValidFunctionName($"{queryFunction.Name}Async");

        var func = new FunctionBuilder(functionName)
            .WithVisibility(FunctionVisibility.Public);

        func.AddStatement(
        $"""
        var encoder = new EtherSharp.ABI.AbiEncoder()
        """);

        foreach(var input in queryFunction.Inputs)
        {
            _paramEncodingWriter.AddParameterEncoding(func, input);
        }

        var (returnType, decoderFunction) = _paramDecodingWriter.SetQueryOutputDecoding(queryFunction.Name, func, queryFunction.Outputs);

        func.AddStatement(
        $$"""
        return _client.CallAsync(new EtherSharp.Tx.TxInput<{{returnType}}>(
            {{GetFunctionSignatureFieldName(queryFunction)}},
            encoder,
            decoder => {
            {{decoderFunction}}
            },
            EtherSharp.Types.Address.FromString(ContractAddress),
            0
        ))
        """);

        return func;
    }

    private FunctionBuilder GenerateMessageFunction(FunctionAbiMember messageFunction)
    {
        string functionName = NameUtils.ToValidFunctionName($"{messageFunction.Name}");

        var func = new FunctionBuilder(functionName)
            .WithVisibility(FunctionVisibility.Public);

        func.AddStatement(
        $"""
        var encoder = new EtherSharp.ABI.AbiEncoder()
        """);

        var paramNames = new List<string>();
        foreach(var input in messageFunction.Inputs)
        {
            paramNames.Add(
                _paramEncodingWriter.AddParameterEncoding(func, input)
            );
        }

        string ethParamName = paramNames.Contains("ethValue")
            ? "_ethValue"
            : "ethValue";

        if(messageFunction.StateMutability == StateMutability.Payable)
        {
            func.AddArgument(
                typeof(BigInteger).FullName, ethParamName
            );
        }
        else
        {
            func.AddStatement($"{typeof(BigInteger).FullName} {ethParamName} = 0");
        }

        switch(messageFunction.Outputs.Length)
        {
            case 0:
                func.WithReturnTypeRaw("EtherSharp.Tx.TxInput");
                func.AddStatement(
                $$"""
                return EtherSharp.Tx.TxInput.ForContractCall(
                    EtherSharp.Types.Address.FromString(ContractAddress),
                    {{ethParamName}},
                    {{GetFunctionSignatureFieldName(messageFunction)}},
                    encoder
                )
                """);
                break;
            case 1:
                var (returnType, decoderFunction) = _paramDecodingWriter.SetMessageOutputDecoding(messageFunction.Name, func, messageFunction.Outputs);
                func.AddStatement(
                $$"""
                return new EtherSharp.Tx.TxInput<{{returnType}}>(
                    {{GetFunctionSignatureFieldName(messageFunction)}},
                    encoder,
                    decoder =>
                    {
                    {{decoderFunction}}
                    },
                    EtherSharp.Types.Address.FromString(ContractAddress),
                    {{ethParamName}}
                )
                """);
                break;
            default:
                throw new NotSupportedException();
        }

        return func;
    }

    private static string GetFunctionSignatureFieldName(FunctionAbiMember abiFunction)
        => $"_{abiFunction.Name}{HexUtils.ToHexString(abiFunction.GetSignatureBytes())}Signature";
}
