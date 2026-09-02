using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Runtime.InteropServices;

namespace EtherSharp.Interpreter.Storage;

internal sealed class StorageJournal
{
    public readonly record struct Snapshot(
        int PersistentStorageCount,
        int TransientStorageCount,
        int BalanceCount,
        int NonceCount,
        int CodeCount,
        int LogCount
    );

    private readonly record struct StorageWrite(Bytes32 Key, Bytes32 Value);
    private readonly record struct JournalLog(
        Address Address,
        ReadOnlyMemory<Bytes32> Topics,
        ReadOnlyMemory<byte> Data
    );

    private readonly List<StorageWrite> _persistentStorageWrites = [];
    private readonly List<StorageWrite> _transientStorageWrites = [];
    private readonly List<UInt256> _balanceWrites = [];
    private readonly List<ulong> _nonceWrites = [];
    private readonly List<EVMByteCode> _codeWrites = [];
    private readonly List<JournalLog> _logs = [];

    public Snapshot CurrentSnapshot => new(
        _persistentStorageWrites.Count,
        _transientStorageWrites.Count,
        _balanceWrites.Count,
        _nonceWrites.Count,
        _codeWrites.Count,
        _logs.Count
    );

    public void SStore(in Bytes32 key, in Bytes32 value)
        => _persistentStorageWrites.Add(new StorageWrite(key, value));

    public void TStore(in Bytes32 key, in Bytes32 value)
        => _transientStorageWrites.Add(new StorageWrite(key, value));

    public void SetBalance(in UInt256 value)
        => _balanceWrites.Add(value);

    public void SetNonce(ulong value)
        => _nonceWrites.Add(value);

    public void SetCode(in EVMByteCode value)
        => _codeWrites.Add(value);

    public void AddLog(Address address, Bytes32[] topics, byte[] data)
        => _logs.Add(new JournalLog(address, topics, data));

    public bool TrySLoad(in Bytes32 key, out Bytes32 value)
    {
        ReadOnlySpan<StorageWrite> entries = CollectionsMarshal.AsSpan(_persistentStorageWrites);
        for(int i = entries.Length - 1; i >= 0; i--)
        {
            ref readonly var entry = ref entries[i];
            if(entry.Key.Equals(in key))
            {
                value = entry.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool TryTLoad(in Bytes32 key, out Bytes32 value)
    {
        ReadOnlySpan<StorageWrite> entries = CollectionsMarshal.AsSpan(_transientStorageWrites);
        for(int i = entries.Length - 1; i >= 0; i--)
        {
            ref readonly var entry = ref entries[i];
            if(entry.Key.Equals(in key))
            {
                value = entry.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool TryGetBalance(out UInt256 value)
    {
        if(_balanceWrites.Count == 0)
        {
            value = default;
            return false;
        }

        value = _balanceWrites[^1];
        return true;
    }

    public bool TryGetNonce(out ulong value)
    {
        if(_nonceWrites.Count == 0)
        {
            value = default;
            return false;
        }

        value = _nonceWrites[^1];
        return true;
    }

    public bool TryGetCode(out EVMByteCode value)
    {
        if(_codeWrites.Count == 0)
        {
            value = default;
            return false;
        }

        value = _codeWrites[^1];
        return true;
    }

    public void Reset(Snapshot snapshot)
    {
        if(snapshot.PersistentStorageCount < 0
            || snapshot.PersistentStorageCount > _persistentStorageWrites.Count
            || snapshot.TransientStorageCount < 0
            || snapshot.TransientStorageCount > _transientStorageWrites.Count
            || snapshot.BalanceCount < 0
            || snapshot.BalanceCount > _balanceWrites.Count
            || snapshot.NonceCount < 0
            || snapshot.NonceCount > _nonceWrites.Count
            || snapshot.CodeCount < 0
            || snapshot.CodeCount > _codeWrites.Count
            || snapshot.LogCount < 0
            || snapshot.LogCount > _logs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(snapshot));
        }

        _persistentStorageWrites.RemoveRange(
            snapshot.PersistentStorageCount,
            _persistentStorageWrites.Count - snapshot.PersistentStorageCount
        );
        _transientStorageWrites.RemoveRange(
            snapshot.TransientStorageCount,
            _transientStorageWrites.Count - snapshot.TransientStorageCount
        );
        _balanceWrites.RemoveRange(snapshot.BalanceCount, _balanceWrites.Count - snapshot.BalanceCount);
        _nonceWrites.RemoveRange(snapshot.NonceCount, _nonceWrites.Count - snapshot.NonceCount);
        _codeWrites.RemoveRange(snapshot.CodeCount, _codeWrites.Count - snapshot.CodeCount);
        _logs.RemoveRange(snapshot.LogCount, _logs.Count - snapshot.LogCount);
    }
}
