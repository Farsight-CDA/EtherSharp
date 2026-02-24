using EtherSharp.Types;

namespace EtherSharp.Tx;

/// <summary>
/// Represents state access metadata used for transaction access lists.
/// </summary>
public record StateAccess
{
    /// <summary>
    /// Creates a new state access descriptor.
    /// </summary>
    /// <param name="address">The contract address whose storage is accessed.</param>
    /// <param name="storageKeys">The storage slot keys accessed for <paramref name="address"/>.</param>
    public StateAccess(Address address, byte[][] storageKeys)
    {
        Address = address;
        StorageKeys = storageKeys;
    }

    /// <summary>
    /// The contract address whose storage is accessed.
    /// </summary>
    public Address Address { get; init; }

    /// <summary>
    /// The storage slot keys accessed for <see cref="Address"/>.
    /// </summary>
    public byte[][] StorageKeys { get; init; }
}
