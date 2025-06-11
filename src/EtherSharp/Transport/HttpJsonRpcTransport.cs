using EtherSharp.Client.Services.RPC;
using EtherSharp.Common;
using EtherSharp.Common.Exceptions;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Transport;
/// <summary>
/// Transport for EVM Http RPC
/// </summary>
public sealed class HttpJsonRpcTransport : IRPCTransport, IDisposable
{
    private readonly HttpClient _client;
    private int _id;

    /// <inheritdoc />
    public bool SupportsFilters { get; set; }
    /// <inheritdoc />
    public bool SupportsSubscriptions => false;

    /// <inheritdoc />
    public event Action? OnConnectionEstablished
    {
        add => throw new NotSupportedException();
        remove => throw new NotSupportedException();
    }
    /// <inheritdoc />
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage
    {
        add => throw new NotSupportedException();
        remove => throw new NotSupportedException();
    }

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

    /// <inheritdoc />
    public ValueTask InitializeAsync(CancellationToken cancellationToken = default)
        => ValueTask.CompletedTask;

    private record RpcError(int Code, string Message);
    private record JsonRpcResponse<T>([property: JsonRequired] int Id, T? Result, RpcError? Error, [property: JsonRequired] string Jsonrpc);

    private record JsonRpcRequest(int Id, string Method, object?[] Params, string Jsonrpc = "2.0");
    /// <inheritdoc />
    public async Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, object?[] parameters, CancellationToken cancellationToken = default)
    {
        int id = Interlocked.Increment(ref _id);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, (string?) null)
        {
            Content = JsonContent.Create(
                new JsonRpcRequest(id, method, parameters),
                options: ParsingUtils.EvmSerializerOptions
            )
        };

        var response = await _client.SendAsync(httpRequestMessage, cancellationToken);

        try
        {
            var jsonRpcResponse = await response.Content.ReadFromJsonAsync<JsonRpcResponse<TResult>>(
                ParsingUtils.EvmSerializerOptions, cancellationToken
            );

            if(jsonRpcResponse is null)
            {
                throw new RPCTransportException("RPC Error: Invalid response");
            }
            else if(jsonRpcResponse.Id != id)
            {
                throw new RPCTransportException("RPC Error: Invalid response Id");
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
            throw new RPCTransportException($"Error: {s}", e);
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}
