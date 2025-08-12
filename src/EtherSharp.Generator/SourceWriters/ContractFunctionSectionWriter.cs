using EtherSharp.Generator.Abi;
using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SourceWriters.Components;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters;
public class ContractFunctionSectionWriter(ParamEncodingWriter paramEncodingWriter)
{
    private readonly FunctionBuilder _isMatchingSelectorFunction = new FunctionBuilder("IsMatchingSelector")
        .AddArgument("System.ReadOnlySpan<byte>", "selector")
        .WithReturnType<bool>()
        .WithIsStatic()
        .AddStatement($"return selector.SequenceEqual(SelectorBytes.Span)");

    private readonly ParamEncodingWriter _paramEncodingWriter = paramEncodingWriter;

    public void GenerateContractFunctionSection(InterfaceBuilder interfaceBuilder, ClassBuilder implementationBuilder,
        string contractName, IEnumerable<FunctionAbiMember> functionMembers)
    {
        var sectionBuilder = new ClassBuilder("Functions")
            .WithIsStatic();

        foreach(var functionMembersGroup in functionMembers.GroupBy(x => NameUtils.ToValidClassName(x.Name)))
        {
            foreach(var functionMember in functionMembersGroup)
            {
                byte[] selectorBytes = functionMember.GetSignatureBytes(out string signature);
                string functionTypeName = functionMembersGroup.Count() > 1
                    ? $"{functionMembersGroup.Key}_{HexUtils.ToHexString(selectorBytes)}"
                    : functionMembersGroup.Key;

                var typeBuilder = new ClassBuilder(functionTypeName)
                    .WithIsStatic()
                    .AddFunction(_isMatchingSelectorFunction);

                var createInputFunction = new FunctionBuilder("CreateTxInput")
                    .WithIsStatic()
                    .AddStatement("var encoder = new EtherSharp.ABI.AbiEncoder()")
                    .AddArgument("EtherSharp.Types.Address", "contractAddress");

                bool isQuery = functionMember.Outputs.Length > 0
                    && (functionMember.StateMutability == StateMutability.Pure || functionMember.StateMutability == StateMutability.View);

                var interfaceFunction = new FunctionBuilder(isQuery ? $"{functionTypeName}Async" : functionTypeName);
                var inputNameList = new List<string>();

                for(int i = 0; i < functionMember.Inputs.Length; i++)
                {
                    var input = functionMember.Inputs[i];
                    var (paramName, paramType, encodeFunc) = _paramEncodingWriter.GetInputEncoding(input, i);

                    inputNameList.Add(paramName);

                    interfaceFunction.AddArgument(paramType, paramName);
                    createInputFunction.AddArgument(paramType, paramName);
                    createInputFunction.AddStatement(encodeFunc(paramName));
                }

                createInputFunction.AddArgument("System.Numerics.BigInteger", "ethValue");

                string? outputTypeName = null;

                if(functionMember.Outputs.Length > 0)
                {
                    (outputTypeName, string decodeFunc) = _paramEncodingWriter.GetOutputDecoding(
                        functionTypeName,
                        functionMember.Outputs
                    );

                    createInputFunction.WithReturnTypeRaw($"EtherSharp.Tx.ITxInput<{outputTypeName}>");
                    createInputFunction.AddStatement(
                        $"""
                        return EtherSharp.Tx.ITxInput.ForContractCall<{outputTypeName}>(
                            contractAddress,
                            ethValue,
                            SelectorBytes,
                            encoder,
                            decoder => {decodeFunc}
                        )
                        """
                    );
                }
                else
                {
                    createInputFunction.WithReturnTypeRaw("EtherSharp.Tx.ITxInput");
                    createInputFunction.AddStatement(
                        $"""
                        return EtherSharp.Tx.ITxInput.ForContractCall(
                            contractAddress,
                            ethValue,
                            SelectorBytes,
                            encoder
                        )
                        """
                    );
                }

                typeBuilder.AddFunction(createInputFunction);
                typeBuilder.AddRawContent(
                    $$"""
                    /// <summary>
                    /// Function signature used to calculate the signature bytes.
                    /// </summary>
                    public const string Signature = "{{signature}}";
                    /// <summary>
                    /// Function signature bytes based on function signature: {{signature}}
                    /// </summary>
                    public static ReadOnlyMemory<byte> SelectorBytes { get; } 
                        = new byte[] { {{string.Join(",", selectorBytes)}} };
                    /// <summary>
                    /// Hex encoded function signature bytes based on function signature: {{signature}}
                    /// </summary>
                    public const string SelectorHex = "0x{{HexUtils.ToHexString(selectorBytes)}}";
                    """
                );

                if(isQuery)
                {
                    interfaceFunction
                        .WithReturnTypeRaw($"System.Threading.Tasks.Task<{outputTypeName}>")
                        .AddArgument("EtherSharp.Types.TargetBlockNumber", "targetBlockNumber", true, "default")
                        .AddArgument("EtherSharp.StateOverride.TxStateOverride", "stateOverride", true, "default")
                        .AddArgument("System.Threading.CancellationToken", "cancellationToken", true, "default")
                        .AddStatement(
                            $"""
                            return _client.CallAsync(
                                {contractName}.Functions.{functionTypeName}.CreateTxInput(Address, {string.Join(",", inputNameList)}{(inputNameList.Count > 0 ? "," : "")} 0), 
                                targetBlockNumber,
                                stateOverride: stateOverride, 
                                cancellationToken: cancellationToken
                            )
                            """
                        );
                }
                else
                {
                    bool isPayable = functionMember.StateMutability == StateMutability.Payable;
                    if(isPayable)
                    {
                        interfaceFunction.AddArgument("System.Numerics.BigInteger", "ethValue");
                    }

                    interfaceFunction
                        .AddStatement(
                            $"""
                            return {contractName}.Functions.{functionTypeName}.CreateTxInput(Address, {string.Join(",", inputNameList)}{(inputNameList.Count > 0 ? "," : "")} {(isPayable ? "ethValue" : "0")})
                            """
                        );

                    if(outputTypeName is null)
                    {
                        interfaceFunction.WithReturnTypeRaw("EtherSharp.Tx.ITxInput");
                    }
                    else
                    {
                        interfaceFunction.WithReturnTypeRaw($"EtherSharp.Tx.ITxInput<{outputTypeName}>");
                    }
                }

                interfaceBuilder.AddFunction(interfaceFunction);
                implementationBuilder.AddFunction(interfaceFunction);
                sectionBuilder.AddInnerType(typeBuilder);
            }
        }

        interfaceBuilder.AddInnerType(sectionBuilder);
    }
}
