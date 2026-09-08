using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Provides read-only access to accounts, balances, code, and storage slots in the current interpreter state.</summary>
public interface IInterpreterStorage
{
    /// <summary>Gets the account balance, including current execution changes and overrides.</summary>
    ValueTask<UInt256> GetBalanceAsync(Address address);

    /// <summary>Gets the account nonce, including current execution changes and overrides.</summary>
    ValueTask<ulong> GetNonceAsync(Address address);

    /// <summary>Gets borrowed account bytecode without following code delegation.</summary>
    /// <remarks>The returned memory is valid only during the awaited hook callback.</remarks>
    ValueTask<ReadOnlyMemory<byte>> GetCodeAsync(Address address);

    /// <summary>Gets the account code hash using EXTCODEHASH semantics, including current execution changes and overrides.</summary>
    ValueTask<Bytes32> GetCodeHashAsync(Address address);

    /// <summary>Gets a persistent storage word, including current execution changes and overrides. Unset slots return zero.</summary>
    ValueTask<Bytes32> GetStorageAsync(Address address, Bytes32 key);

    /// <summary>Gets a transient storage word in the current execution. Unset slots return zero.</summary>
    Bytes32 GetTransientStorage(Address address, Bytes32 key);
}
