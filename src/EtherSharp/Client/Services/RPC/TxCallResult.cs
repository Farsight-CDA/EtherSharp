namespace EtherSharp.Client.Services.RPC;
public abstract record TxCallResult
{
    public record Success(byte[] Data) : TxCallResult;
    public record Reverted : TxCallResult;
}
