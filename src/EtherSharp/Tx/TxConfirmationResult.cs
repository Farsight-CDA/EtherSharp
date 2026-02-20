using EtherSharp.Types;

namespace EtherSharp.Tx;

public record TxConfirmationResult
{
    public record Success(TransactionReceipt Receipt) : TxConfirmationResult;
    public record NonceAlreadyUsed() : TxConfirmationResult;
    public record Cancelled() : TxConfirmationResult;
    public record UnhandledException(Exception Exception) : TxConfirmationResult;

    public TransactionReceipt GetReceipt()
        => this is Success successResult
            ? successResult.Receipt
            : throw new InvalidOperationException($"{nameof(TxConfirmationResult)} is of type {GetType().Name}");
}
