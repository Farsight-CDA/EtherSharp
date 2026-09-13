namespace EtherSharp.RPC.Transport;

/// <summary>Provides a strongly typed key for a custom RPC request option.</summary>
/// <typeparam name="TValue">The type of value associated with the key.</typeparam>
public readonly record struct RpcRequestOptionsKey<TValue>
{
    /// <summary>Gets the name that identifies the option.</summary>
    public string Name { get; }

    /// <summary>Creates a key with the specified name.</summary>
    /// <param name="name">A name that uniquely identifies the option.</param>
    public RpcRequestOptionsKey(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}
