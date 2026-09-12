using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Forking;

public sealed partial class InterpreterStateFork
{
    /// <summary>Tries to read a cached upstream account balance without fetching it.</summary>
    /// <param name="address">The account address.</param>
    /// <param name="value">The cached balance when known.</param>
    /// <returns><see langword="true"/> when the balance is cached; otherwise, <see langword="false"/>.</returns>
    public bool TryGetCachedBalance(Address address, out UInt256 value)
    {
        lock(_lock)
        {
            return _cache.Balances.TryGetValue(address, out value);
        }
    }

    /// <summary>Tries to read a cached upstream account nonce without fetching it.</summary>
    /// <param name="address">The account address.</param>
    /// <param name="value">The cached nonce when known.</param>
    /// <returns><see langword="true"/> when the nonce is cached; otherwise, <see langword="false"/>.</returns>
    public bool TryGetCachedNonce(Address address, out ulong value)
    {
        lock(_lock)
        {
            return _cache.Nonces.TryGetValue(address, out value);
        }
    }

    /// <summary>Tries to read cached upstream account bytecode without fetching it.</summary>
    /// <param name="address">The account address.</param>
    /// <param name="value">The cached bytecode when known.</param>
    /// <returns><see langword="true"/> when the bytecode is cached; otherwise, <see langword="false"/>.</returns>
    /// <remarks>The returned value references the cache-owned bytecode buffer.</remarks>
    public bool TryGetCachedCode(Address address, out EVMByteCode value)
    {
        lock(_lock)
        {
            return _cache.Code.TryGetValue(address, out value);
        }
    }

    /// <summary>Tries to read a cached upstream account code hash without fetching it.</summary>
    /// <param name="address">The account address.</param>
    /// <param name="value">The cached code hash, or zero for a known empty or nonexistent account.</param>
    /// <returns><see langword="true"/> when the code hash is cached; otherwise, <see langword="false"/>.</returns>
    public bool TryGetCachedCodeHash(Address address, out Bytes32 value)
    {
        lock(_lock)
        {
            if(_cache.CodeHashes.TryGetValue(address, out var codeHash))
            {
                value = codeHash ?? Bytes32.Zero;
                return true;
            }

            value = default;
            return false;
        }
    }

    /// <summary>Tries to read a cached upstream persistent storage value without fetching it.</summary>
    /// <param name="address">The account address.</param>
    /// <param name="slot">The storage slot.</param>
    /// <param name="value">The cached storage value when known.</param>
    /// <returns><see langword="true"/> when the storage value is cached; otherwise, <see langword="false"/>.</returns>
    public bool TryGetCachedStorage(Address address, Bytes32 slot, out Bytes32 value)
    {
        lock(_lock)
        {
            return _cache.Storage.TryGetValue((address, slot), out value);
        }
    }
}
