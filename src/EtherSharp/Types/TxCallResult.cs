namespace EtherSharp.Types;
public abstract record TxCallResult
{
    public record Success(byte[] Data) : TxCallResult;
    public record Reverted : TxCallResult;
}
