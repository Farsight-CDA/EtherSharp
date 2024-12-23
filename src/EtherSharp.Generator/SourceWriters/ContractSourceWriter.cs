using EtherSharp.Generator.Abi;
using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Numerics;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;
public class ContractSourceWriter(
    AbiTypeWriter typeWriter,
    ParamEncodingWriter paramEncodingWriter, ParamDecodingWriter paramDecodingWriter,
    EventTypeWriter eventTypeWriter
)
{
    private readonly AbiTypeWriter _typeWriter = typeWriter;
    private readonly ParamEncodingWriter _paramEncodingWriter = paramEncodingWriter;
    private readonly ParamDecodingWriter _paramDecodingWriter = paramDecodingWriter;
    private readonly EventTypeWriter _eventTypeWriter = eventTypeWriter;

    public string WriteContractSourceCode(string @namespace, string contractName, IEnumerable<AbiMember> members)
    {
        var contractInterface = new InterfaceBuilder(contractName)
            .WithIsPartial(true)
            .WithVisibility(InterfaceVisibility.Public)
            .AddRawContent($"public LogsApi Events => new LogsApi(this);");

        if(members.Any(
            x => (x is FallbackAbiMember fallbackMember && fallbackMember.StateMutability == StateMutability.Payable)
              || (x is ReceiveAbiMember receiveMember && receiveMember.StateMutability == StateMutability.Payable)))
        {
            contractInterface.AddBaseInterface("EtherSharp.Contract.IPayableContract");
        }

        string implementationName = $"{contractName}_Generated_Implementation";
        var contractImplementation = new ClassBuilder(implementationName)
            .AddBaseType(contractName, true)
            .WithVisibility(ClassVisibility.Internal)
            .AddField(new FieldBuilder("EtherSharp.Client.IEtherClient", "_client")
                .WithIsReadonly(true)
                .WithVisibility(FieldVisibility.Private)
            ).AddProperty(new PropertyBuilder("EtherSharp.Types.Address", "Address")
                .WithVisibility(PropertyVisibility.Public)
                .WithSetterVisibility(SetterVisibility.None)
            ).AddFunction(new FunctionBuilder("GetClient")
                .WithReturnTypeRaw("EtherSharp.Client.IEtherClient")
                .AddStatement("return _client")
            );

        foreach(var member in members.Where(x => x is FunctionAbiMember).Cast<FunctionAbiMember>())
        {
            byte[] signatureBytes = member.GetSignatureBytes(out string functionSignature);
            var signatureBytesField = new FieldBuilder("byte[]", GetFunctionSignatureFieldName(member))
                .WithIsStatic(true)
                .WithIsReadonly(true)
                .WithVisibility(FieldVisibility.Private)
                .WithDefaultValue($"[ {signatureBytes[0]}, {signatureBytes[1]}, {signatureBytes[2]}, {signatureBytes[3]} ]")
                .WithXmlSummaryContent(functionSignature);

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

        var eventsStructBuilder = new StringBuilder();
        eventsStructBuilder.AppendLine(
            $$"""
            public readonly ref struct LogsApi
            {
                private readonly {{@namespace}}.{{contractName}} contract;

                public LogsApi({{@namespace}}.{{contractName}} contract)
                {
                    this.contract = contract;
                }
                public LogsApi()
                {
                    throw new NotSupportedException();
                }
            """);

        foreach(var member in members.Where(x => x is EventAbiMember).Cast<EventAbiMember>())
        {
            string eventProperty = GenerateEventProperty($"{@namespace}.{contractName}", member);
            eventsStructBuilder.AppendLine(eventProperty);

            contractInterface.AddInnerType(
                _eventTypeWriter.GenerateEventType(member)
            );
        }

        eventsStructBuilder.AppendLine("}");
        contractInterface.AddRawContent(eventsStructBuilder.ToString());

        var output = new StringBuilder();

        output.AppendLine(
            $$"""
            namespace {{@namespace}};

            {{contractInterface.Build()}}
            {{contractImplementation.WithAutoConstructor().Build()}}
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

        foreach(var (input, index) in queryFunction.Inputs.Select((x, i) => (x, i)))
        {
            _paramEncodingWriter.AddParameterEncoding(func, input, index);
        }

        var (returnType, decoderFunction) = _paramDecodingWriter.SetQueryOutputDecoding(func, queryFunction.Outputs);

        func.AddStatement(
        $$"""
        return _client.CallAsync(new EtherSharp.Tx.TxInput<{{returnType}}>(
            {{GetFunctionSignatureFieldName(queryFunction)}},
            encoder,
            decoder => {
            {{decoderFunction}}
            },
            Address,
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
        foreach(var (input, index) in messageFunction.Inputs.Select((x, i) => (x, i)))
        {
            paramNames.Add(
                _paramEncodingWriter.AddParameterEncoding(func, input, index)
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
                    Address,
                    {{ethParamName}},
                    {{GetFunctionSignatureFieldName(messageFunction)}},
                    encoder
                )
                """);
                break;
            case 1:
                var (returnType, decoderFunction) = _paramDecodingWriter.SetMessageOutputDecoding(func, messageFunction.Outputs);
                func.AddStatement(
                $$"""
                return new EtherSharp.Tx.TxInput<{{returnType}}>(
                    {{GetFunctionSignatureFieldName(messageFunction)}},
                    encoder,
                    decoder =>
                    {
                    {{decoderFunction}}
                    },
                    Address,
                    {{ethParamName}}
                )
                """);
                break;
            default:
                throw new NotSupportedException();
        }

        return func;
    }

    private string GenerateEventProperty(string contractInterfaceFullName, EventAbiMember eventMember)
    {
        string propertyName = NameUtils.ToValidFunctionName($"{eventMember.Name}");
        string eventTypeName = NameUtils.ToValidClassName($"{eventMember.Name}Event");

        return
        $"""
        public readonly EtherSharp.Client.Services.LogsApi.IConfiguredLogsApi<{contractInterfaceFullName}.{eventTypeName}> {propertyName}
            => contract.GetClient()
                .Logs<{contractInterfaceFullName}.{eventTypeName}>()
                .HasContract(contract)
                .HasTopic({contractInterfaceFullName}.{eventTypeName}.Topic);
        """;
    }

    private static string GetFunctionSignatureFieldName(FunctionAbiMember abiFunction)
        => $"_{abiFunction.Name}{HexUtils.ToHexString(abiFunction.GetSignatureBytes(out _))}Signature";
}
