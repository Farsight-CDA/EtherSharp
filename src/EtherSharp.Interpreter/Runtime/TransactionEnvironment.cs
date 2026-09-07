using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Legacy;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>Describes the transaction environment used by an interpreter execution.</summary>
/// <param name="Sender">The transaction sender.</param>
/// <param name="Nonce">The sender nonce used for execution.</param>
/// <param name="GasLimit">The transaction gas limit.</param>
/// <param name="EffectiveGasPrice">The effective gas price exposed to executing code.</param>
/// <param name="Input">The destination, value, and call data or creation initcode.</param>
/// <param name="AccessList">The transaction access list.</param>
/// <param name="BlobHashes">The transaction's versioned blob hashes.</param>
public readonly record struct TransactionEnvironment(
    Address Sender,
    ulong Nonce,
    ulong GasLimit,
    UInt256 EffectiveGasPrice,
    ITxInput Input,
    ReadOnlyMemory<StateAccess> AccessList,
    ReadOnlyMemory<Bytes32> BlobHashes
)
{
    internal static TransactionEnvironment CreateForCall(
        Address sender,
        ITxInput input,
        ulong nonce,
        InterpreterContext context
    ) => new(
        sender,
        nonce,
        (ulong) context.GasLimit,
        UInt256.Zero,
        input,
        ReadOnlyMemory<StateAccess>.Empty,
        ReadOnlyMemory<Bytes32>.Empty
    );

    internal static TransactionEnvironment CreateForTransaction(
        Address sender,
        LegacyTransaction transaction,
        InterpreterContext context
    )
    {
        if(context.BaseFee is { } baseFee && transaction.GasPrice < baseFee)
        {
            throw new InvalidOperationException("The transaction gas price is below the block base fee.");
        }

        var environment = new TransactionEnvironment(
            sender,
            transaction.Nonce,
            transaction.Gas,
            transaction.GasPrice,
            transaction.Input,
            ReadOnlyMemory<StateAccess>.Empty,
            ReadOnlyMemory<Bytes32>.Empty
        );
        return environment;
    }

    internal static TransactionEnvironment CreateForTransaction(
        Address sender,
        EIP1559Transaction transaction,
        InterpreterContext context
    )
    {
        if(context.BaseFee is not { } baseFee)
        {
            throw new InvalidOperationException("EIP-1559 transactions require a block base fee.");
        }
        if(transaction.MaxPriorityFeePerGas > transaction.MaxFeePerGas)
        {
            throw new InvalidOperationException("The priority fee exceeds the maximum fee.");
        }
        if(transaction.MaxFeePerGas < baseFee)
        {
            throw new InvalidOperationException("The maximum fee is below the block base fee.");
        }

        var effectiveGasPrice = baseFee + UInt256.Min(
            transaction.MaxPriorityFeePerGas,
            transaction.MaxFeePerGas - baseFee
        );
        return new TransactionEnvironment(
            sender,
            transaction.Nonce,
            transaction.Gas,
            effectiveGasPrice,
            transaction.Input,
            transaction.AccessList,
            ReadOnlyMemory<Bytes32>.Empty
        );
    }
}
