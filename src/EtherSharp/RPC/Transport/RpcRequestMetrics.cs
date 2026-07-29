using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using System.Diagnostics;

namespace EtherSharp.RPC.Transport;

internal sealed class RpcRequestMetrics
{
    private readonly OTELCounter<long>? _counter;

    public RpcRequestMetrics(IServiceProvider provider, TagList additionalTags = default)
    {
        _counter = provider.CreateOTELCounter<long>("evm_rpc_requests", tags: additionalTags);
        if(_counter is null)
        {
            return;
        }

        AddMeasurement(_counter, 0, RpcRequestStatus.Success);
        AddMeasurement(_counter, 0, RpcRequestStatus.Failure);
    }

    public void Add(string method, RpcRequestStatus status)
    {
        if(_counter is not { } counter)
        {
            return;
        }

        AddMeasurement(counter, 1, status, method);
    }

    private static void AddMeasurement(OTELCounter<long> counter, long value, RpcRequestStatus status, string? method = null)
    {
        TagList tags = [
            new KeyValuePair<string, object?>("status", status switch
            {
                RpcRequestStatus.Success => "success",
                RpcRequestStatus.Failure => "failure",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            })
        ];
        if(method is not null)
        {
            tags.Add("method", method);
        }
        counter.Add(value, tags);
    }
}
