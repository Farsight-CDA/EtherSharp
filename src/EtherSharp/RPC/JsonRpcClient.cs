using EtherSharp.Common;
using EtherSharp.Types;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC;

internal class JsonRpcClient(HttpClient httpClient, Uri rpcUrl)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly Uri _rpcUrl = rpcUrl;
    private int _id = 0;

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

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _rpcUrl)
        {
            Content = JsonContent.Create(
                new JsonRpcRequest(id, method, objects),
                options: ParsingUtils.EvmSerializerOptions
            )
        };

        var response = await _httpClient.SendAsync(httpRequestMessage);

        try
        {
            var jsonRpcResponse = await response.Content.ReadFromJsonAsync<JsonRpcResponse<TResult>>(ParsingUtils.EvmSerializerOptions);

            if(jsonRpcResponse == null)
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
            else if(jsonRpcResponse.Result == null)
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