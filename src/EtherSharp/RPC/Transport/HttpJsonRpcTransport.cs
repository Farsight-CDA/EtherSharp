using EtherSharp.Common;
using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.RPC.Transport.Json;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport;
/// <summary>
/// Transport for EVM Http RPC
/// </summary>
public sealed class HttpJsonRpcTransport : IRPCTransport, IDisposable
{
    private static readonly MediaTypeHeaderValue _jsonMediaTypeHeaderValue = new("application/json")
    {
        CharSet = Encoding.UTF8.WebName
    };

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

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, TargetHeight requiredBlockNumber, CancellationToken cancellationToken = default)
    {
        int id = Interlocked.Increment(ref _id);
        return SendRpcRequestCoreAsync<TResult>(
            id,
            method,
            CreateHttpContent(JsonRpcRequestPayload.SerializeToUtf8Bytes(id, method, _jsonSerializerOptions)),
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, TResult>(
        string method, T1 t1, TargetHeight requiredBlockNumber, CancellationToken cancellationToken = default)
    {
        int id = Interlocked.Increment(ref _id);
        return SendRpcRequestCoreAsync<TResult>(
            id,
            method,
            CreateHttpContent(JsonRpcRequestPayload.SerializeToUtf8Bytes(id, method, t1, _jsonSerializerOptions)),
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, TResult>(
        string method, T1 t1, T2 t2, TargetHeight requiredBlockNumber, CancellationToken cancellationToken = default)
    {
        int id = Interlocked.Increment(ref _id);
        return SendRpcRequestCoreAsync<TResult>(
            id,
            method,
            CreateHttpContent(JsonRpcRequestPayload.SerializeToUtf8Bytes(id, method, t1, t2, _jsonSerializerOptions)),
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, TargetHeight requiredBlockNumber, CancellationToken cancellationToken = default)
    {
        int id = Interlocked.Increment(ref _id);
        return SendRpcRequestCoreAsync<TResult>(
            id,
            method,
            CreateHttpContent(JsonRpcRequestPayload.SerializeToUtf8Bytes(id, method, t1, t2, t3, _jsonSerializerOptions)),
            cancellationToken
        );
    }

    private static ByteArrayContent CreateHttpContent(byte[] payload)
    {
        var content = new ByteArrayContent(payload);
        content.Headers.ContentType = _jsonMediaTypeHeaderValue;
        return content;
    }

    private void AddRpcRequestMetric(string method, string status)
    {
        TagList tags = [
            new KeyValuePair<string, object?>("status", status),
            new KeyValuePair<string, object?>("method", method)
        ];
        _rpcRequestsCounter?.Add(1, tags);
    }

    private async Task<RpcResult<TResult>> SendRpcRequestCoreAsync<TResult>(
        int id, string method, HttpContent requestContent, CancellationToken cancellationToken)
    {
        using var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            Content = requestContent
        };

        HttpResponseMessage? response = null;

        try
        {
            response = await _client.SendAsync(httpRequestMessage, cancellationToken);
        }
        catch(OperationCanceledException) when(cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch(Exception ex)
        {
            throw new RPCTransportException($"Exception while sending HTTP request", ex);
        }

        try
        {
            string? mediaType = response.Content.Headers.ContentType?.MediaType;

            if(mediaType is not null && !mediaType.EndsWith("json"))
            {
                AddRpcRequestMetric(method, "failure");
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RPCTransportException($"Http RPC request failed with status {(int) response.StatusCode} {response.ReasonPhrase}: {responseBody}");
            }

            var jsonRpcResponse = await response.Content.ReadFromJsonAsync<JsonRpcResponse<TResult>>(
                _jsonSerializerOptions, cancellationToken
            );

            if(jsonRpcResponse is null)
            {
                AddRpcRequestMetric(method, "failure");
                throw new RPCTransportException("RPC Error: Empty response");
            }

            if(jsonRpcResponse.Error is not null)
            {
                AddRpcRequestMetric(method, "success");
                return new RpcResult<TResult>.Error(jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message, jsonRpcResponse.Error.Data);
            }

            if(jsonRpcResponse.Id != id)
            {
                AddRpcRequestMetric(method, "failure");
                throw new RPCTransportException($"RPC Error: Response id mismatch. Expected {id}, got {jsonRpcResponse.Id?.ToString() ?? "null"}");
            }

            if(jsonRpcResponse.ResultIsNull)
            {
                AddRpcRequestMetric(method, "success");
                return RpcResult<TResult>.Null.Instance;
            }

            AddRpcRequestMetric(method, "success");
            return new RpcResult<TResult>.Success(jsonRpcResponse.Result!);
        }
        catch(RPCTransportException)
        {
            throw;
        }
        catch(JsonException)
        {
            AddRpcRequestMetric(method, "failure");
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new RPCTransportException($"Http RPC request failed with status {(int) response.StatusCode} {response.ReasonPhrase}: {responseBody}");
        }
        catch(OperationCanceledException) when(cancellationToken.IsCancellationRequested)
        {
            AddRpcRequestMetric(method, "failure");
            throw;
        }
        catch(Exception ex)
        {
            AddRpcRequestMetric(method, "failure");
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new RPCTransportException($"Http RPC request failed with status {(int) response.StatusCode} {response.ReasonPhrase}: {responseBody}", ex);
        }
        finally
        {
            response?.Dispose();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}
