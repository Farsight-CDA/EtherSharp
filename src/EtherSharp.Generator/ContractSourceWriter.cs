using EtherSharp.Generator.Abi;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Numerics;

namespace EtherSharp.Generator;
public class ContractSourceWriter
{
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

            bool isQuery = member.StateMutability == StateMutability.Payable || member.StateMutability == StateMutability.View;

            var func = isQuery switch
            {
                true => GenerateQueryFunction(member),
                false => GenerateMessageFunction(member)
            };

            contractInterface.AddFunction(func);
            contractImplementation.AddFunction(func);
        }

        return
        $$"""
        namespace {{@namespace}};

        {{contractInterface.Build()}}
        {{contractImplementation.Build(generateFieldConstructor: true)}}
        """;
    }

    private FunctionBuilder GenerateQueryFunction(FunctionAbiMember queryFunction)
    {
        string functionName = NameUtils.ToValidFunctionName($"{queryFunction.Name}Async");

        var func = new FunctionBuilder(functionName)
            .WithVisibility(FunctionVisibility.Public);

        if(queryFunction.Outputs.Length != 1)
        {
            throw new NotSupportedException("Query function must have exactly one output parameter");
        }

        func.WithReturnTypeRaw($"{typeof(Task).FullName}<{GetCSharpEquivalentType(queryFunction.Outputs[0])}>");

        func.AddStatement(
        $"""
        var encoder = new EtherSharp.ABI.AbiEncoder()
        """);

        foreach(var input in queryFunction.Inputs)
        {
            func.AddArgument(
                GetCSharpEquivalentType(input),
                input.Name
            );

            func.AddStatement($"encoder.{GetABIEncodingMethodName(input)}({input.Name})");
        }

        func.AddStatement(
        $$"""
        return _client.CallAsync(new EtherSharp.Tx.TxInput<{{GetCSharpEquivalentType(queryFunction.Outputs[0])}}>(
            {{GetFunctionSignatureFieldName(queryFunction)}},
            encoder,
            decoder =>
            {
                _ = decoder.{{GetABIEncodingMethodName(queryFunction.Outputs[0])}}(out var val);
                return val;
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

        foreach(var input in messageFunction.Inputs)
        {
            func.AddArgument(
                GetCSharpEquivalentType(input),
                input.Name
            );

            func.AddStatement($"encoder.{GetABIEncodingMethodName(input)}({input.Name})");
        }

        switch(messageFunction.Outputs.Length)
        {
            case 0:
                func.WithReturnTypeRaw("EtherSharp.Tx.TxInput");
                func.AddStatement(
                $$"""
                return EtherSharp.Tx.TxInput.ForContractCall(
                    EtherSharp.Types.Address.FromString(ContractAddress),
                    0,
                    {{GetFunctionSignatureFieldName(messageFunction)}},
                    encoder
                )
                """);
                break;
            case 1:
                func.WithReturnTypeRaw($"EtherSharp.Tx.TxInput<{GetCSharpEquivalentType(messageFunction.Outputs[0])}>");
                func.AddStatement(
                $$"""
                return new EtherSharp.Tx.TxInput<{{GetCSharpEquivalentType(messageFunction.Outputs[0])}}>(
                    {{GetFunctionSignatureFieldName(messageFunction)}},
                    encoder,
                    decoder =>
                    {
                        _ = decoder.{{GetABIEncodingMethodName(messageFunction.Outputs[0])}}(out var val);
                        return val;
                    },
                    EtherSharp.Types.Address.FromString(ContractAddress),
                    0
                )a
                """);
                break;
            default:
                throw new NotSupportedException();
        }

        return func;
    }

    private static string GetCSharpEquivalentType(AbiValue abiMember) 
        => abiMember.Type switch
        {
            "address" => typeof(string).FullName,
            "string" => typeof(string).FullName,
            "bool" => typeof(bool).FullName,
            string s when s.StartsWith("uint") && int.TryParse(s.Substring(4), out int bitSize) 
                => bitSize % 8 != 0
                    ? throw new NotSupportedException("uint bitsize must be multiple of 8")
                    : bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("uint bitsize must be between 8 and 256"),
                        8 => typeof(byte).FullName,
                        <= 16 => typeof(ushort).FullName,
                        <= 32 => typeof(uint).FullName,
                        <= 64 => typeof(ulong).FullName,
                        _ => typeof(BigInteger).FullName,
                    },
            string s when s.StartsWith("int") && int.TryParse(s.Substring(4), out int bitSize)
                => bitSize % 8 != 0
                    ? throw new NotSupportedException("uint bitsize must be multiple of 8")
                    : bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("uint bitsize must be between 8 and 256"),
                        8 => typeof(sbyte).FullName,
                        <= 16 => typeof(short).FullName,
                        <= 32 => typeof(int).FullName,
                        <= 64 => typeof(long).FullName,
                        _ => typeof(BigInteger).FullName,
                    },
            _ => throw new NotSupportedException($"Solidity type {abiMember.Type} is not supported")
        };

    private static string GetABIEncodingMethodName(AbiValue abiMember)
        => abiMember.Type switch
        {
            string s when s.StartsWith("uint") => s.Substring(0, 2).ToUpper() + s.Substring(2),
            _ => NameUtils.ToValidFunctionName(abiMember.Type),
        };

    private static string GetFunctionSignatureFieldName(FunctionAbiMember abiFunction) 
        => $"_{abiFunction.Name}Signature";
}
