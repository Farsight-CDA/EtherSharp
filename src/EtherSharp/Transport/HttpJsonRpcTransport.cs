using EtherSharp.Client.Services.RPC;
using EtherSharp.Common;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Transport;
public class HttpJsonRpcTransport : IRPCTransport
{
    private readonly HttpClient _client;
    private int _id = 0;

    public event Action? OnConnectionEstablished;
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

    public ValueTask InitializeAsync(CancellationToken cancellationToken = default) 
        => ValueTask.CompletedTask;

    private record RpcError(int Code, string Message);
    private record JsonRpcResponse<T>([property: JsonRequired] int Id, T? Result, RpcError? Error, [property: JsonRequired] string Jsonrpc);

    private record JsonRpcRequest(int Id, string Method, object?[] Params, string Jsonrpc = "2.0");
    public async Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, object?[] parameters, CancellationToken cancellationToken = default)
    {
        int id = Interlocked.Increment(ref _id);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, (string?) null)
        {
            Content = JsonContent.Create(
                new JsonRpcRequest(id, method, parameters),
                options: ParsingUtils.EvmSerializerOptions
            )
        };

        var response = await _client.SendAsync(httpRequestMessage, cancellationToken);

        try
        {
            var jsonRpcResponse = await response.Content.ReadFromJsonAsync<JsonRpcResponse<TResult>>(ParsingUtils.EvmSerializerOptions, cancellationToken);

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
            string s = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Error: {s}", e);
        }
    }
}
