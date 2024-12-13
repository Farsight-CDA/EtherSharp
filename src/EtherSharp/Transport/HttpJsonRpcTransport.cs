using EtherSharp.Client.Services.RPC;
using EtherSharp.Common;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Transport;
public class HttpJsonRpcTransport : IRPCTransport
{
    private readonly HttpClient _client;
    private int _id = 0;

    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public bool SupportsFilters { get; set; }
    public bool SupportsSubscriptions => false;

    public HttpJsonRpcTransport(Uri rpcUri, bool allowFilters = true)
    {
        _client = new HttpClient()
        {
            BaseAddress = rpcUri
        };
        SupportsFilters = allowFilters;
    }

    public HttpJsonRpcTransport(string rpcUrl, bool allowFilters = true)
        : this(new Uri(rpcUrl, UriKind.Absolute), allowFilters)
    {
    }

    private record RpcError(int Code, string Message);
    private record JsonRpcResponse<T>([property: JsonRequired] int Id, T? Result, RpcError? Error, [property: JsonRequired] string Jsonrpc);

    public Task<RpcResult<TResult>> SendRpcRequest<TResult>(string method)
        => InnerSendRpcRequest<TResult>(method, []);

    public Task<RpcResult<TResult>> SendRpcRequest<T1, TResult>(string method, T1 t1)
        => InnerSendRpcRequest<TResult>(method, [t1]);

    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, TResult>(string method, T1 t1, T2 t2)
        => InnerSendRpcRequest<TResult>(method, [t1, t2]);

    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, T3, TResult>(string method, T1 t1, T2 t2, T3 t3)
        => InnerSendRpcRequest<TResult>(method, [t1, t2, t3]);

    private record JsonRpcRequest(int Id, string Method, object?[] Params, string Jsonrpc = "2.0");
    private async Task<RpcResult<TResult>> InnerSendRpcRequest<TResult>(string method, object?[] objects)
    {
        int id = Interlocked.Increment(ref _id);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, (string?) null)
        {
            Content = JsonContent.Create(
                new JsonRpcRequest(id, method, objects),
                options: ParsingUtils.EvmSerializerOptions
            )
        };

        var response = await _client.SendAsync(httpRequestMessage);

        try
        {
            var jsonRpcResponse = await response.Content.ReadFromJsonAsync<JsonRpcResponse<TResult>>(ParsingUtils.EvmSerializerOptions);

            if(jsonRpcResponse is null)
            {
                throw new Exception("RPC Error: Invalid response");
            }
            else if(jsonRpcResponse.Id != id)
            {
                throw new Exception("RPC Error: Invalid response Id");
            }
            else if(jsonRpcResponse.Error != null)
            {
                return new RpcResult<TResult>.Error(jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message);
            }
            else if(jsonRpcResponse.Result is null)
            {
                return new RpcResult<TResult>.Null();
            }
            //
            return new RpcResult<TResult>.Success(jsonRpcResponse.Result);

        }
        catch(Exception e)
        {
            string s = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error: {s}", e);
        }
    }
}
