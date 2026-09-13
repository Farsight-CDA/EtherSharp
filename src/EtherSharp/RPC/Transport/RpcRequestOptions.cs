using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.RPC.Transport;

/// <summary>
/// Stores custom values passed to an RPC transport.
/// </summary>
/// <remarks>Do not modify these options while a request using them is in progress.</remarks>
public struct RpcRequestOptions
{
    private Dictionary<string, object?>? _values;

    /// <summary>Sets the value associated with a key and returns the updated options.</summary>
    public RpcRequestOptions With<TValue>(RpcRequestOptionsKey<TValue> key, TValue value)
    {
        (_values ??= [])[key.Name] = value;
        return this;
    }

    /// <summary>Adds a statistics collector for built-in and participating custom transports.</summary>
    public RpcRequestOptions WithStatistics(RpcRequestStatistics statistics)
        => With(RpcRequestOptionsKeys.Statistics, statistics);

    /// <summary>Attempts to get the value associated with a key.</summary>
    public readonly bool TryGetValue<TValue>(
        RpcRequestOptionsKey<TValue> key,
        [MaybeNullWhen(false)] out TValue value
    )
    {
        if(_values?.TryGetValue(key.Name, out object? untypedValue) is true
            && (untypedValue is TValue || (untypedValue is null && default(TValue) is null)))
        {
            value = (TValue) untypedValue!;
            return true;
        }

        value = default;
        return false;
    }
}
