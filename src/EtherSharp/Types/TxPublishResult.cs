namespace EtherSharp.Types;
public record TxPublishResult
{
    /// <summary>
    /// Transaction was successfully submitted to the mempool.
    /// </summary>
    public record Success(string TxHash) : TxPublishResult;

}
