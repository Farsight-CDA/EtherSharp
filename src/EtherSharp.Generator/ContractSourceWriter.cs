using Cosm.Net.Generators.Common.Util;
using EtherSharp.Generator.Abi;
using EtherSharp.Generator.SyntaxElements;

namespace EtherSharp.Generator;
public class ContractSourceWriter
{
    public string WriteContractSourceCode(string @namespace, string contractName, IEnumerable<AbiMember> members)
    {
        return WriteContractInterfaceCode(@namespace, contractName, members);
    }

    private string WriteContractInterfaceCode(string @namespace, string contractName, IEnumerable<AbiMember> members)
    {
        var contractInterface = new InterfaceBuilder(contractName)
            .WithIsPartial(true)
            .WithVisibility(InterfaceVisibility.Public)
            .AddBaseInterface("EtherSharp.Contract.IContract");

        foreach(var member in members.Where(x => x is FunctionAbiMember).Cast<FunctionAbiMember>())
        {
            bool isQuery = member.StateMutability == StateMutability.Payable || member.StateMutability == StateMutability.View;

            string functionName = NameUtils.ToValidFunctionName(isQuery
                ? $"{member.Name}Async"
                : member.Name);

            var function = new FunctionBuilder(functionName)
                .WithReturnType<Task>();

            contractInterface.AddFunction(function);

        }

        return 
        $$"""
        namespace {{@namespace}};

        {{contractInterface.Build()}}
        """;
    }
}
