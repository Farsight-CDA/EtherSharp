using EtherSharp.Generator.ABI.Members;
using EtherSharp.Generator.SyntaxElements;
using System.Text;

namespace EtherSharp.Generator.ABI.SourceWriters;

internal sealed class ContractSourceWriter(
    ContractErrorSectionWriter errorSectionWriter, ContractEventSectionWriter eventSectionWriter,
    ContractFunctionSectionWriter functionSectionWriter, ContractTypesSectionWriter typesSectionWriter
)
{
    private readonly ContractErrorSectionWriter _errorSectionWriter = errorSectionWriter;
    private readonly ContractEventSectionWriter _eventSectionWriter = eventSectionWriter;
    private readonly ContractFunctionSectionWriter _functionSectionWriter = functionSectionWriter;
    private readonly ContractTypesSectionWriter _typesSectionWriter = typesSectionWriter;

    public string WriteContractSourceCode(string @namespace, string contractName, IEnumerable<AbiMember> members, byte[]? byteCode)
    {
        string namespacePrefix = String.IsNullOrEmpty(@namespace) ? String.Empty : $"{@namespace}.";
        string namespaceDeclaration = String.IsNullOrEmpty(@namespace)
            ? String.Empty
            : $"namespace {@namespace};\n\n";
        var contractInterface = new InterfaceBuilder(contractName)
            .WithIsPartial(true)
            .AddRawContent($"public Logs.EventsModule Events => new Logs.EventsModule(this);");

        if(members.Any(
            x => (x is FallbackAbiMember fallbackMember && fallbackMember.StateMutability == StateMutability.Payable)
              || (x is ReceiveAbiMember receiveMember && receiveMember.StateMutability == StateMutability.Payable)))
        {
            contractInterface.AddBaseInterface("EtherSharp.Contract.IPayableContract");
        }

        string implementationName = $"{contractName}_Generated_Implementation";
        var contractImplementation = new ClassBuilder(implementationName)
            .AddBaseType($"{namespacePrefix}{contractName}", true)
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

        _functionSectionWriter.GenerateContractFunctionSection(
            contractInterface,
            contractImplementation,
            contractName,
            members.OfType<FunctionAbiMember>(),
            members.OfType<ConstructorAbiMember>().SingleOrDefault() ?? ConstructorAbiMember.Empty,
            byteCode,
            members.OfType<FallbackAbiMember>().SingleOrDefault()
        );
        _errorSectionWriter.GenerateContractErrorSection(contractInterface, members.OfType<ErrorAbiMember>());
        _eventSectionWriter.GenerateContractEventSection(contractInterface, @namespace, contractName, members.OfType<EventAbiMember>());
        _typesSectionWriter.GenerateContractTypesSection(contractInterface);

        var output = new StringBuilder();
        output.AppendLine(
            $$"""
            {{namespaceDeclaration}}{{contractInterface.Build()}}
            {{contractImplementation.WithAutoConstructor().Build()}}

            file static class {{implementationName}}_Registration
            {
                [global::System.Runtime.CompilerServices.ModuleInitializer]
                internal static void RegisterContract()
                {
                    global::EtherSharp.Client.Services.ContractFactory.GeneratedContractRegistry.Register<global::{{namespacePrefix}}{{contractName}}>(
                        static (client, address) => new global::{{namespacePrefix}}{{implementationName}}(client, address)
                    );
                }
            }
            """
        );

        return output.ToString();
    }
}
