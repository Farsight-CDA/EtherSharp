using EtherSharp.Common;
using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport;
/// <summary>
/// Transport for EVM Http RPC
/// </summary>
public sealed class HttpJsonRpcTransport : IRPCTransport, IDisposable
{
    /// <inheritdoc />
    public bool SupportsFilters => true;
    /// <inheritdoc />
    public bool SupportsSubscriptions => false;

    private readonly HttpClient _client;
    private int _id;

    private readonly OTELCounter<long>? _rpcRequestsCounter;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

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

    /// <summary>
    /// Creates an HTTP JSON-RPC transport bound to the provided endpoint URI.
    /// </summary>
    /// <param name="rpcUri">HTTP RPC endpoint URI.</param>
    /// <param name="provider">Service provider used for instrumentation and serializer configuration.</param>
    /// <param name="additionalTags">Additional OpenTelemetry tags.</param>
    public HttpJsonRpcTransport(Uri rpcUri, IServiceProvider provider, TagList additionalTags = default)
    {
        _client = new HttpClient()
        {
            BaseAddress = rpcUri
        };

        _jsonSerializerOptions = provider.GetService<JsonSerializerOptions>()
            ?? ParsingUtils.EvmSerializerOptions;

        _rpcRequestsCounter = provider.CreateOTELCounter<long>("evm_rpc_requests", tags: additionalTags);
        _rpcRequestsCounter?.Add(0, new KeyValuePair<string, object?>("status", "success"));
        _rpcRequestsCounter?.Add(0, new KeyValuePair<string, object?>("status", "failure"));
    }

    /// <summary>
    /// Creates an HTTP JSON-RPC transport bound to the provided endpoint URL.
    /// </summary>
    /// <param name="rpcUrl">HTTP RPC endpoint URL.</param>
    /// <param name="provider">Service provider used for instrumentation and serializer configuration.</param>
    /// <param name="additionalTags">Additional OpenTelemetry tags.</param>
    public HttpJsonRpcTransport(string rpcUrl, IServiceProvider provider, TagList additionalTags = default)
        : this(new Uri(rpcUrl, UriKind.Absolute), provider, additionalTags)
    {
    }

    /// <inheritdoc />
    public ValueTask InitializeAsync(CancellationToken cancellationToken = default)
        => ValueTask.CompletedTask;

    private record RpcError(int Code, string Message, string? Data);
    private record JsonRpcResponse<T>([property: JsonRequired] int? Id, T? Result, RpcError? Error, [property: JsonRequired] string Jsonrpc);

    private record JsonRpcRequest(int Id, string Method, object?[] Params, string Jsonrpc = "2.0");
    /// <inheritdoc />
    public async Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, object?[] parameters, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken)
    {
        int id = Interlocked.Increment(ref _id);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, (string?) null)
        {
            Content = JsonContent.Create(
                new JsonRpcRequest(id, method, parameters),
                options: _jsonSerializerOptions
            )
        };

        HttpResponseMessage response;

        try
        {
            response = await _client.SendAsync(httpRequestMessage, cancellationToken);
        }
        catch(Exception ex)
        {
            throw new RPCTransportException($"Exception while sending HTTP request", ex);
        }

        try
        {
            var jsonRpcResponse = await response.Content.ReadFromJsonAsync<JsonRpcResponse<TResult>>(
                _jsonSerializerOptions, cancellationToken
            );

            if(jsonRpcResponse is null)
            {
                _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "failure"));
                throw new RPCTransportException("RPC Error: Empty response");
            }
            else if(jsonRpcResponse.Id != id)
            {
                _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "failure"));
                throw new RPCTransportException("RPC Error: Response id mismatch");
            }
            else if(jsonRpcResponse.Error != null)
            {
                _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "success"));
                return new RpcResult<TResult>.Error(jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message, jsonRpcResponse.Error.Data);
            }
            else if(jsonRpcResponse.Result is null)
            {
                _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "success"));
                return new RpcResult<TResult>.Null();
            }

            _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "success"));
            return new RpcResult<TResult>.Success(jsonRpcResponse.Result);
        }
        catch(JsonException ex)
        {
            _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "failure"));
            string s = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new RPCTransportException($"Error: {s}", ex);
        }
        catch(Exception ex)
        {
            _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "failure"));
            string s = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new RPCTransportException($"Error: {s}", ex);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}
