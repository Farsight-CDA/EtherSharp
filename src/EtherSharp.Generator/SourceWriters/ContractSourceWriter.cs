using EtherSharp.Generator.Abi;
using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SourceWriters.Components;
using EtherSharp.Generator.SyntaxElements;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;
public class ContractSourceWriter(
    AbiTypeWriter typeWriter,
    ContractErrorSectionWriter errorSectionWriter, ContractEventSectionWriter eventSectionWriter, ContractFunctionSectionWriter functionSectionWriter
)
{
    private readonly AbiTypeWriter _typeWriter = typeWriter;

    private readonly ContractErrorSectionWriter _errorSectionWriter = errorSectionWriter;
    private readonly ContractEventSectionWriter _eventSectionWriter = eventSectionWriter;
    private readonly ContractFunctionSectionWriter _functionSectionWriter = functionSectionWriter;

    public string WriteContractSourceCode(string @namespace, string contractName, IEnumerable<AbiMember> members)
    {
        var contractInterface = new InterfaceBuilder(contractName)
            .WithIsPartial(true)
            .WithVisibility(InterfaceVisibility.Public)
            .AddRawContent($"public Logs.EventsModule Events => new Logs.EventsModule(this);");

        if(members.Any(
            x => (x is FallbackAbiMember fallbackMember && fallbackMember.StateMutability == StateMutability.Payable)
              || (x is ReceiveAbiMember receiveMember && receiveMember.StateMutability == StateMutability.Payable)))
        {
            contractInterface.AddBaseInterface("EtherSharp.Contract.IPayableContract");
        }

        string implementationName = $"{contractName}_Generated_Implementation";
        var contractImplementation = new ClassBuilder(implementationName)
            .AddBaseType($"{@namespace}.{contractName}", true)
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

        _functionSectionWriter.GenerateContractFunctionSection(contractInterface, contractImplementation, contractName, members.OfType<FunctionAbiMember>());
        _errorSectionWriter.GenerateContractErrorSection(contractInterface, contractImplementation, members.OfType<ErrorAbiMember>());
        _eventSectionWriter.GenerateContractEventSection(contractInterface, contractImplementation, @namespace, contractName, members.OfType<EventAbiMember>());

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
}
