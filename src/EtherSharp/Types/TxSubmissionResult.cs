namespace EtherSharp.Types;
public abstract record TxSubmissionResult
{
    public record Success(string TxHash) : TxSubmissionResult;
    public record NonceTooLow(uint TxNonce, uint NextNonce) : TxSubmissionResult;
    public record Failure(string Message) : TxSubmissionResult;
}
