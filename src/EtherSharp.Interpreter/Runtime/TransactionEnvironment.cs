using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Legacy;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

internal readonly record struct TransactionEnvironment(
    Address Sender,
    ulong Nonce,
    ulong GasLimit,
    UInt256 EffectiveGasPrice,
    ITxInput Input,
    StateAccess[] AccessList,
    Bytes32[] BlobHashes
)
{
    public static TransactionEnvironment CreateFrom(
        Address sender,
        ITransaction transaction,
        InterpreterContext context
    )
    {
        if(transaction.ChainId != context.ChainId)
        {
            throw new InvalidOperationException("Transaction chain ID does not match the execution context.");
        }

        var environment = transaction switch
        {
            LegacyTransaction legacy => CreateFrom(sender, legacy, context),
            EIP1559Transaction eip1559 => CreateFrom(sender, eip1559, context),
            _ => throw new NotSupportedException(
                $"Transaction type {transaction.GetType().FullName} is not supported."
            )
        };
        return environment;
    }

    private static TransactionEnvironment CreateFrom(
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
            [],
            []
        );
        return environment;
    }

    private static TransactionEnvironment CreateFrom(
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
            []
        );
    }
}
