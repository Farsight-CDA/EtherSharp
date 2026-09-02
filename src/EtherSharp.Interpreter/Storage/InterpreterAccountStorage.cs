using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Storage;

internal sealed class InterpreterAccountStorage
{
    private readonly Address _address;
    private readonly InterpreterContext _context;
    private readonly IGlobalStateProvider _globalStateProvider;

    private readonly StorageJournal _journal = new();
    private readonly Dictionary<Bytes32, Bytes32> _persistentStorageCache = [];
    private UInt256? _balanceCache;
    private AccountCode? _accountCodeCache;

    public StorageJournal.Snapshot CurrentSnapshot => _journal.CurrentSnapshot;

    public InterpreterAccountStorage(
        Address address,
        InterpreterContext context,
        IGlobalStateProvider globalStateProvider
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(globalStateProvider);
        _address = address;
        _context = context;
        _globalStateProvider = globalStateProvider;
    }

    public async ValueTask<Bytes32> SLoadAsync(Bytes32 key)
    {
        if(_journal.TrySLoad(in key, out var value) || _persistentStorageCache.TryGetValue(key, out value))
        {
            return value;
        }

        value = await _globalStateProvider.GetStorageAtAsync(_context, _address, key);
        _persistentStorageCache.Add(key, value);
        return value;
    }

    public void SStore(in Bytes32 key, in Bytes32 value)
        => _journal.SStore(in key, in value);

    public Bytes32 TLoad(in Bytes32 key)
        => _journal.TryTLoad(in key, out var value)
            ? value
            : Bytes32.Zero;

    public void TStore(in Bytes32 key, in Bytes32 value)
        => _journal.TStore(in key, in value);

    public void AddLog(Bytes32[] topics, byte[] data)
        => _journal.AddLog(_address, topics, data);

    public async ValueTask<UInt256> GetBalanceAsync()
    {
        if(_journal.TryGetBalance(out var value))
        {
            return value;
        }

        if(_balanceCache.HasValue)
        {
            return _balanceCache.Value;
        }

        value = await _globalStateProvider.GetBalanceAsync(_context, _address);
        _balanceCache = value;
        return value;
    }

    public void SetBalance(in UInt256 value)
        => _journal.SetBalance(in value);

    public async ValueTask<EVMByteCode> GetCodeAsync()
        => _journal.TryGetCode(out var value)
            ? value
            : (await GetAccountCodeAsync()).Code;

    public void SetCode(in EVMByteCode value)
        => _journal.SetCode(in value);

    public async ValueTask<Bytes32> GetCodeHashAsync()
        => _journal.TryGetCode(out var code)
            ? Keccak256.HashData(code.ByteCode.Span)
            : (await GetAccountCodeAsync()).Hash;

    private async ValueTask<AccountCode> GetAccountCodeAsync()
    {
        if(_accountCodeCache.HasValue)
        {
            return _accountCodeCache.Value;
        }

        var value = await _globalStateProvider.GetAccountCodeAsync(_context, _address);
        _accountCodeCache = value;
        return value;
    }

    public void Reset(StorageJournal.Snapshot snapshot)
        => _journal.Reset(snapshot);
}
