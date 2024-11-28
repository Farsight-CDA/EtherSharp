using EtherSharp.Types;

namespace EtherSharp.Client.Services.TxConfirmer;
public abstract record TxConfirmationResult
{
    public record Confirmed(TransactionReceipt Receipt) : TxConfirmationResult;
    public record TimedOut() : TxConfirmationResult;
}
