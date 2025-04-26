namespace EtherSharp.Client.Services.TxPublisher;
public abstract record TxSubmissionResult
{
    public record Success(string TxHash) : TxSubmissionResult;
    public record AlreadyExists() : TxSubmissionResult;
    public record TransactionUnderpriced() : TxSubmissionResult;
    public record UnhandledException(Exception Exception) : TxSubmissionResult;
}
