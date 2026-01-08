using EtherSharp.Generator.Abi;
using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SourceWriters.Components;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters;

internal class ContractFunctionSectionWriter(ParamEncodingWriter paramEncodingWriter, MemberTypeWriter memberTypeWriter)
{
    private readonly FunctionBuilder _isMatchingSelectorFunction = new FunctionBuilder("IsMatchingSelector")
        .AddArgument("System.ReadOnlySpan<byte>", "data")
        .WithReturnType<bool>()
        .WithIsStatic()
        .AddStatement(
            $$"""
            if (data.Length < 4)
            {
                return false;
            }

            return data.Slice(0, 4).SequenceEqual(SelectorBytes.Span);
            """
        );

    private readonly ParamEncodingWriter _paramEncodingWriter = paramEncodingWriter;
    private readonly MemberTypeWriter _memberTypeWriter = memberTypeWriter;

    public void GenerateContractFunctionSection(InterfaceBuilder interfaceBuilder, ClassBuilder implementationBuilder,
        string contractName, IEnumerable<FunctionAbiMember> functionMembers,
        ConstructorAbiMember? constructorMember, byte[]? byteCode,
        FallbackAbiMember? fallbackMember)
    {
        var functionClassNames = new List<string>();
        var sectionBuilder = new ClassBuilder("Functions")
            .WithIsStatic();

        if(fallbackMember is not null)
        {
            var typeBuilder = new ClassBuilder("Fallback")
                .WithIsStatic();
            var createFunction = new FunctionBuilder("Create")
                .WithIsStatic()
                .WithReturnTypeRaw("EtherSharp.Tx.IContractCall<System.ReadOnlyMemory<byte>>")
                .AddArgument("EtherSharp.Types.Address", "contractAddress")
                .AddArgument("System.ReadOnlyMemory<byte>", "calldata");

            if(fallbackMember.StateMutability == StateMutability.NonPayable)
            {
                createFunction.AddStatement(
                    $"""
                    return EtherSharp.Tx.IContractCall.ForRawContractCall(contractAddress, 0, calldata);
                    """
                );
            }
            else
            {
                createFunction.AddArgument("EtherSharp.Numerics.UInt256", "ethValue");
                createFunction.AddStatement(
                    $"""
                    return EtherSharp.Tx.IContractCall.ForRawContractCall(contractAddress, ethValue, calldata);
                    """
                );
            }

            typeBuilder.AddFunction(createFunction);
            sectionBuilder.AddInnerType(typeBuilder);
        }

        if(byteCode is not null)
        {
            var typeBuilder = new ClassBuilder("Constructor")
                .WithIsStatic();

            typeBuilder.AddRawContent(
                $$"""
                public static EtherSharp.Types.EVMByteCode ByteCode { get; } = new EtherSharp.Types.EVMByteCode(Convert.FromHexString("{{HexUtils.ToHexString(byteCode)}}"));
                """
            );

            var createCodeFunction = new FunctionBuilder("CreateCode")
                .WithIsStatic()
                .WithReturnTypeRaw("EtherSharp.Types.EVMByteCode");
            var createFunction = new FunctionBuilder("Create")
                .WithIsStatic()
                .WithReturnTypeRaw("EtherSharp.Tx.IContractDeployment");
            var create2Function = new FunctionBuilder("Create2")
                .WithIsStatic()
                .WithReturnTypeRaw("EtherSharp.Tx.IContractCall<EtherSharp.Types.Address>");

            if(constructorMember is not null && constructorMember.Inputs.Length > 0)
            {
                var createCallBuilder = new CallArgumentsBuilder();
                bool isPayable = constructorMember.StateMutability == StateMutability.Payable;

                for(int i = 0; i < constructorMember.Inputs.Length; i++)
                {
                    var input = constructorMember.Inputs[i];
                    var (paramName, paramType, encodeFunc) = _paramEncodingWriter.GetInputEncoding(input, i);

                    createCodeFunction.AddArgument(paramType, paramName);
                    createCodeFunction.AddStatement(encodeFunc(paramName));

                    createFunction.AddArgument(paramType, paramName);
                    create2Function.AddArgument(paramType, paramName);
                    createCallBuilder.AddArgument(paramName);
                }

                if(isPayable)
                {
                    createFunction.AddArgument("EtherSharp.Numerics.UInt256", "ethValue");
                    create2Function.AddArgument("EtherSharp.Numerics.UInt256", "ethValue");
                }

                createCodeFunction.AddStatement(
                    $"""
                    var buffer = new byte[ByteCode.Length + encoder.Size];
                    ByteCode.ByteCode.Span.CopyTo(buffer);
                    encoder.TryWritoTo(buffer.AsSpan(ByteCode.Length));
                    return new EtherSharp.Types.EVMByteCode(buffer)
                    """
                );
                createFunction.AddStatement(
                    $"""
                    var contractByteCode = CreateCode({createCallBuilder.Build()});
                    return EtherSharp.Tx.IContractDeployment.Create(contractByteCode, ethValue)
                    """
                );
                create2Function.AddArgument("System.ReadOnlySpan<byte>", "salt");
                create2Function.AddStatement(
                    $"""
                    var contractByteCode = CreateCode({createCallBuilder.Build()});
                    return EtherSharp.Tx.IContractCall.ForCreate2Call(contractByteCode, salt, ethValue)
                    """
                );
            }
            else
            {
                createCodeFunction.AddStatement(
                    $"""
                    return ByteCode
                    """
                );
                createFunction.AddStatement(
                    $"""
                    return EtherSharp.Tx.IContractDeployment.Create(ByteCode, 0)
                    """
                );
                create2Function.AddArgument("System.ReadOnlySpan<byte>", "salt");
                create2Function.AddStatement(
                    $"""
                    return EtherSharp.Tx.IContractCall.ForCreate2Call(ByteCode, salt, 0)
                    """
                );
            }

            typeBuilder.AddFunction(createCodeFunction);
            typeBuilder.AddFunction(createFunction);
            typeBuilder.AddFunction(create2Function);
            sectionBuilder.AddInnerType(typeBuilder);
        }

        foreach(var functionMembersGroup in functionMembers.GroupBy(x => NameUtils.ToValidClassName(x.Name)))
        {
            foreach(var functionMember in functionMembersGroup)
            {
                byte[] selectorBytes = functionMember.GetSignatureBytes(out string signature);
                string functionTypeName = functionMembersGroup.Count() > 1
                    ? $"{functionMembersGroup.Key}_{HexUtils.ToHexString(selectorBytes)}"
                    : functionMembersGroup.Key;
                functionClassNames.Add(functionTypeName);

                var typeBuilder = new ClassBuilder(functionTypeName)
                    .WithAutoConstructor()
                    .AddFunction(_isMatchingSelectorFunction);

                var decodeMethod = new FunctionBuilder("Decode")
                    .WithReturnTypeRaw(functionTypeName)
                    .WithIsStatic(true)
                    .AddArgument("System.ReadOnlyMemory<byte>", "data")
                    .AddStatement("if (data.Length < 4) throw new System.ArgumentException(\"Data too short\", nameof(data));", false)
                    .AddStatement(_memberTypeWriter.GenerateDecodeStatements(functionTypeName, functionMember.Inputs, "data.Slice(4)"));

                var tryDecodeMethod = new FunctionBuilder("TryDecode")
                    .AddArgument("System.ReadOnlyMemory<byte>", "data")
                    .AddArgument($"out {functionTypeName}", "parsedFunction")
                    .WithReturnType<bool>()
                    .WithIsStatic()
                    .AddStatement(
                        $$"""
                        if (!IsMatchingSelector(data.Span)) 
                        {
                            parsedFunction = null!;
                            return false;
                        }

                        parsedFunction = Decode(data);
                        return true
                        """
                    );

                _memberTypeWriter.AddInputProperties(typeBuilder, functionMember.Inputs);
                typeBuilder.AddFunction(decodeMethod);
                typeBuilder.AddFunction(tryDecodeMethod);

                var createTxFunction = new FunctionBuilder("Create")
                    .WithIsStatic()
                    .AddStatement("var encoder = new EtherSharp.ABI.AbiEncoder()")
                    .AddArgument("EtherSharp.Types.Address", "contractAddress");

                bool isQuery = functionMember.Outputs.Length > 0
                    && (functionMember.StateMutability == StateMutability.Pure || functionMember.StateMutability == StateMutability.View);
                bool isPayable = functionMember.StateMutability == StateMutability.Payable;

                var interfaceFunction = new FunctionBuilder(isQuery ? $"{functionTypeName}Async" : functionTypeName);
                var inputNameList = new List<string>();

                for(int i = 0; i < functionMember.Inputs.Length; i++)
                {
                    var input = functionMember.Inputs[i];
                    var (paramName, paramType, encodeFunc) = _paramEncodingWriter.GetInputEncoding(input, i);

                    inputNameList.Add(paramName);

                    interfaceFunction.AddArgument(paramType, paramName);
                    createTxFunction.AddArgument(paramType, paramName);
                    createTxFunction.AddStatement(encodeFunc(paramName));
                }

                if(isPayable)
                {
                    createTxFunction.AddArgument("EtherSharp.Numerics.UInt256", "ethValue");
                }

                string? outputTypeName = null;

                if(functionMember.Outputs.Length > 0)
                {
                    (outputTypeName, string decodeFunc) = _paramEncodingWriter.GetOutputDecoding(
                        $"{functionTypeName}Result",
                        functionMember.Outputs
                    );

                    createTxFunction.WithReturnTypeRaw($"EtherSharp.Tx.IContractCall<{outputTypeName}>");
                    createTxFunction.AddStatement(
                        $"""
                        return EtherSharp.Tx.IContractCall<{outputTypeName}>.ForContractCall(
                            contractAddress,
                            {(isPayable ? "ethValue" : "0")},
                            SelectorBytes,
                            encoder,
                            decoder => {decodeFunc}
                        )
                        """
                    );
                }
                else
                {
                    createTxFunction.WithReturnTypeRaw("EtherSharp.Tx.IContractCall");
                    createTxFunction.AddStatement(
                        $"""
                        return EtherSharp.Tx.IContractCall.ForContractCall(
                            contractAddress,
                            {(isPayable ? "ethValue" : "0")},
                            SelectorBytes,
                            encoder
                        )
                        """
                    );
                }

                typeBuilder.AddFunction(createTxFunction);
                typeBuilder.AddRawContent(
                    $$"""
                    /// <summary>
                    /// Function signature used to calculate the signature bytes.
                    /// </summary>
                    public const string FunctionSignature = "{{signature}}";
                    /// <summary>
                    /// Function signature bytes based on function signature: {{signature}}
                    /// </summary>
                    public static ReadOnlyMemory<byte> SelectorBytes { get; } 
                        = new byte[] { {{String.Join(",", selectorBytes)}} };
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
                        .AddArgument("EtherSharp.Types.TargetBlockNumber", "targetHeight", true, "default")
                        .AddArgument("System.Threading.CancellationToken", "cancellationToken", true, "default")
                        .AddStatement(
                            $"""
                            return _client.CallAsync(
                                {contractName}.Functions.{functionTypeName}.Create(
                                Address{(inputNameList.Count > 0 ? "," : "")}
                                {String.Join(",", inputNameList)}), 
                                targetHeight,
                                cancellationToken: cancellationToken
                            )
                            """
                        );
                }
                else
                {
                    if(isPayable)
                    {
                        interfaceFunction.AddArgument("EtherSharp.Numerics.UInt256", "ethValue");
                    }

                    interfaceFunction
                        .AddStatement(
                            $"""
                            return {contractName}.Functions.{functionTypeName}.Create(
                                Address{(inputNameList.Count > 0 ? "," : "")}
                                {String.Join(",", inputNameList)}{(isPayable ? ',' : "")}
                                {(isPayable ? "ethValue" : "")}
                            )
                            """
                        );

                    if(outputTypeName is null)
                    {
                        interfaceFunction.WithReturnTypeRaw("EtherSharp.Tx.IContractCall");
                    }
                    else
                    {
                        interfaceFunction.WithReturnTypeRaw($"EtherSharp.Tx.IContractCall<{outputTypeName}>");
                    }
                }

                interfaceBuilder.AddFunction(interfaceFunction);
                implementationBuilder.AddFunction(interfaceFunction);
                sectionBuilder.AddInnerType(typeBuilder);
            }
        }

        var getAllSelectorFunnction = new FunctionBuilder("GetSelectors")
            .WithIsStatic(true)
            .WithVisibility(FunctionVisibility.Public)
            .WithReturnTypeRaw("System.ReadOnlyMemory<byte>[]");

        getAllSelectorFunnction.AddStatement(
            $"""
                return [
            {String.Join(",\n", functionClassNames.Select(x => $"       {x}.SelectorBytes"))}
                ]
            """
        );

        sectionBuilder.AddFunction(getAllSelectorFunnction);
        interfaceBuilder.AddInnerType(sectionBuilder);
    }
}
