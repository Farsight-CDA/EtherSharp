namespace EtherSharp.Client.Services.RPC;

public abstract record RpcResult<T>
{
    public record Null() : RpcResult<T>;
    public record Success(T Result) : RpcResult<T>;
    public record Error(int Code, string Message, string? Data) : RpcResult<T>;
}

