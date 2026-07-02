namespace EtherSharp.RPC.Transport.Json;

internal sealed record RpcError(int Code, string Message, string? Data);
