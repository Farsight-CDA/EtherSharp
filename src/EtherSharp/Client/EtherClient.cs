using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Query;
using EtherSharp.Client.Modules.Query.Executor;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Client;

internal class EtherClient : IEtherClient, IEtherTxClient, IInternalEtherClient
{
    private const string FLASHCALL_CONTRACT_HEX = "383d3d39602b5160f01c80602d3df03d3d3d84602d018038039034865af181533d8160013e3d60010181f3";
    private const int FLASHCALL_CONTRACT_LENGTH = 43;

    private readonly IServiceProvider _provider;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly bool _isTxClient;

    private IEtherTxModule _etherModule = null!;
    private ITraceModule _traceModule = null!;
    private IBlocksModule _blocksModule = null!;

    private IRpcClient _rpcClient = null!;
    private IEthRpcModule _ethRpcModule = null!;

    private IEtherSigner _signer = null!;
    private ITxScheduler _txScheduler = null!;

    private IQueryExecutor _queryExecutor = null!;
    private ISubscriptionsManager _subscriptionsManager = null!;
    private ContractFactory _contractFactory = null!;

    private bool _initialized;
    private ulong _chainId;

    IServiceProvider IInternalEtherClient.Provider => _provider;
    IRpcClient IInternalEtherClient.RPC => _rpcClient;

    ulong IEtherClient.ChainId
    {
        get
        {
            AssertReady();
            return _chainId;
        }
    }

    IEtherTxModule IEtherTxClient.ETH
    {
        get
        {
            AssertReady();
            AssertTxClient();
            return _etherModule;
        }
    }

    IEtherModule IEtherClient.ETH
    {
        get
        {
            AssertReady();
            return _etherModule;
        }
    }

    IBlocksModule IEtherClient.Blocks
    {
        get
        {
            AssertReady();
            return _blocksModule;
        }
    }

    public async Task<T1> QueryAsync<T1>(
        IQuery<T1> c1,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(c1, targetBlockNumber, cancellationToken);
    }

    public async Task<(T1, T2)> QueryAsync<T1, T2>(
        IQuery<T1> c1, IQuery<T2> c2,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(c1, c2), targetBlockNumber, cancellationToken);
    }

    public async Task<(T1, T2, T3)> QueryAsync<T1, T2, T3>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(c1, c2, c3), targetBlockNumber, cancellationToken);
    }

    public async Task<(T1, T2, T3, T4)> QueryAsync<T1, T2, T3, T4>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4), targetBlockNumber, cancellationToken);
    }

    public async Task<(T1, T2, T3, T4, T5)> QueryAsync<T1, T2, T3, T4, T5>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5), targetBlockNumber, cancellationToken);
    }

    public async Task<(T1, T2, T3, T4, T5, T6)> QueryAsync<T1, T2, T3, T4, T5, T6>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5, c6), targetBlockNumber, cancellationToken);
    }

    public async Task<(T1, T2, T3, T4, T5, T6, T7)> QueryAsync<T1, T2, T3, T4, T5, T6, T7>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5, c6, c7), targetBlockNumber, cancellationToken);
    }

    public async Task<(T1, T2, T3, T4, T5, T6, T7, T8)> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, IQuery<T8> c8,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5, c6, c7, c8), targetBlockNumber, cancellationToken);
    }

    public async Task<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, IQuery<T8> c8, IQuery<T9> c9,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5, c6, c7, c8, c9), targetBlockNumber, cancellationToken);
    }

    IEventsModule<TEvent> IEtherClient.Events<TEvent>()
    {
        AssertReady();
        return new EventsModule<TEvent>(_rpcClient, _ethRpcModule, _subscriptionsManager);
    }

    internal EtherClient(IServiceProvider provider, bool isTxClient)
    {
        _provider = provider;
        _isTxClient = isTxClient;
        _loggerFactory = provider.GetService<ILoggerFactory>();
    }

    Task IEtherClient.InitializeAsync(CancellationToken cancellationToken)
        => InitializeAsync<object>(null, cancellationToken);

    Task<T> IEtherClient.InitializeAsync<T>(IQuery<T> initQuery, CancellationToken cancellationToken)
        => InitializeAsync(initQuery, cancellationToken);

    private async Task<T> InitializeAsync<T>(IQuery<T>? initQuery, CancellationToken cancellationToken)
    {
        if(_initialized)
        {
            throw new InvalidOperationException("Client already initialized");
        }

        await _provider.GetRequiredService<IRPCTransport>()
            .InitializeAsync(cancellationToken);

        _etherModule = _provider.GetRequiredService<IEtherTxModule>();
        _traceModule = _provider.GetRequiredService<ITraceModule>();
        _blocksModule = _provider.GetRequiredService<IBlocksModule>();

        _rpcClient = _provider.GetRequiredService<IRpcClient>();
        _ethRpcModule = _provider.GetRequiredService<IEthRpcModule>();

        _queryExecutor = _provider.GetRequiredService<IQueryExecutor>();
        _subscriptionsManager = _provider.GetRequiredService<ISubscriptionsManager>();
        _contractFactory = _provider.GetRequiredService<ContractFactory>();

        if(_isTxClient)
        {
            _signer = _provider.GetRequiredService<IEtherSigner>();
            _txScheduler = _provider.GetRequiredService<ITxScheduler>();
        }

        T? initResult;

        if(initQuery is null)
        {
            _chainId = await _ethRpcModule.ChainIdAsync(cancellationToken);
            initResult = default;
        }
        else
        {
            (_chainId, initResult) = await _queryExecutor.ExecuteQueryAsync(IQuery.Combine(IQuery.GetChainId(), initQuery), TargetBlockNumber.Latest, cancellationToken);
        }

        _chainId = await _ethRpcModule.ChainIdAsync(cancellationToken);

        foreach(var initializeableService in _provider.GetServices<IInitializableService>())
        {
            await initializeableService.InitializeAsync(_chainId, cancellationToken);
        }

        _initialized = true;
        return initResult!;
    }

    private void AssertTxClient()
    {
        if(!_isTxClient)
        {
            throw new InvalidOperationException("Client is not a tx client");
        }
    }
    private void AssertReady()
    {
        if(!_initialized)
        {
            throw new InvalidOperationException("Client not initialized");
        }
    }

    IInternalEtherClient IEtherClient.AsInternal() => this;

    Task<Transaction?> IEtherClient.GetTransactionAsync(string hash, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.TransactionByHashAsync(hash, cancellationToken);
    }
    Task<TransactionReceipt?> IEtherClient.GetTransactionReceiptAsync(string hash, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetTransactionReceiptAsync(hash, cancellationToken);
    }

    Task<uint> IEtherClient.GetTransactionCount(
        Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetTransactionCountAsync(address, targetHeight, cancellationToken);
    }

    Task<byte[]> IEtherClient.GetStorageAtAsync(Address address, byte[] slot, TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default)
    {
        AssertReady();
        return _ethRpcModule.GetStorageAtAsync(address, slot, targetBlockNumber, cancellationToken);
    }
    Task<byte[]> IEtherClient.GetStorageAtAsync(IEVMContract contract, byte[] slot, TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default)
    {
        AssertReady();
        return _ethRpcModule.GetStorageAtAsync(contract.Address, slot, targetBlockNumber, cancellationToken);
    }

    private TContract Contract<TContract>(Address address)
        where TContract : IEVMContract
    {
        AssertReady();
        return _contractFactory.Create<TContract>(address);
    }

    Task<FeeHistory> IEtherClient.GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetFeeHistoryAsync(blockCount, newestBlock, rewardPercentiles, cancellationToken);
    }
    Task<BigInteger> IEtherClient.GetGasPriceAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GasPriceAsync(cancellationToken);
    }

    Task<BigInteger> IEtherClient.GetMaxPriorityFeePerGasAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.MaxPriorityFeePerGasAsync(cancellationToken);
    }

    Task<ulong> IEtherClient.EstimateGasLimitAsync(ITxInput call, Address? from, CancellationToken cancellationToken)
    {
        AssertReady();

        if(from is null && _isTxClient)
        {
            from = _signer.Address;
        }

        string data = $"0x{Convert.ToHexString(call.Data)}";
        return _ethRpcModule.EstimateGasAsync(from, call.To, call.Value, data, cancellationToken);
    }

    async Task<TTxGasParams> IEtherClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, CancellationToken cancellationToken)
        where TTxParams : class
    {
        AssertReady();
        var gasFeeProvider = _provider.GetService<IGasFeeProvider<TTxParams, TTxGasParams>>()
            ?? throw new InvalidOperationException(
                $"No GasFeeProvider found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");

        return await gasFeeProvider.EstimateGasParamsAsync(call, txParams ?? TTxParams.Default, cancellationToken);
    }

    TContract IEtherClient.Contract<TContract>(Address address)
        => Contract<TContract>(address);

    public async Task<TxCallResult> SafeCallAsync<T>(ITxInput<T> call, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        AssertReady();

        var sender = _isTxClient
            ? _signer.Address
            : null;

        var result = await _ethRpcModule.CallAsync(
            sender,
            call.To,
            null,
            null,
            call.Value,
            $"0x{Convert.ToHexString(call.Data)}",
            targetHeight,
            cancellationToken
        );

        return result;
    }

    async Task<T> IEtherClient.CallAsync<T>(ITxInput<T> call, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        var result = await SafeCallAsync(call, targetHeight, cancellationToken);
        return call.ReadResultFrom(result.Unwrap(call.To));
    }

    public async Task<TxCallResult> SafeFlashCallAsync<T>(IContractDeployment deployment, IContractCall<T> call, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        AssertReady();
        if(deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }

        var sender = _isTxClient
            ? _signer.Address
            : null;

        int argsLength = 2 + deployment.Data.Length + call.Data.Length;

        if(argsLength + FLASHCALL_CONTRACT_LENGTH > EVMByteCode.MAX_INIT_LENGTH)
        {
            throw new InvalidOperationException($"Maximum call length exceeded, {argsLength + FLASHCALL_CONTRACT_LENGTH} > {EVMByteCode.MAX_INIT_LENGTH}");
        }

        Span<byte> buffer = stackalloc byte[argsLength];

        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort) deployment.Data.Length);
        deployment.Data.CopyTo(buffer[2..]);
        call.Data.CopyTo(buffer[(deployment.Data.Length + 2)..]);

        var result = await _ethRpcModule.CallAsync(
            sender,
            null,
            null,
            null,
            call.Value,
            $"{FLASHCALL_CONTRACT_HEX}{Convert.ToHexString(buffer)}",
            targetHeight,
            cancellationToken
        );

        var data = ((TxCallResult.Success) result).Data;

        return data.Span[0] switch
        {
            0 => new TxCallResult.Reverted(data[1..]),
            1 => new TxCallResult.Success(data[1..]),
            _ => throw new ImpossibleException()
        };

    }

    async Task<T> IEtherClient.FlashCallAsync<T>(IContractDeployment deployment, IContractCall<T> call, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        var result = await SafeFlashCallAsync(deployment, call, targetHeight, cancellationToken);
        return call.ReadResultFrom(result.Unwrap(call.To));
    }

    async Task<IPendingTxHandler<TTxParams, TTxGasParams>> IEtherTxClient.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams
    )
        where TTxParams : class
        where TTxGasParams : class
        => await _txScheduler.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(call, txParams, txGasParams);

    async Task<IPendingTxHandler<TTxParams, TTxGasParams>> IEtherTxClient.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce)
        => await _txScheduler.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(nonce);
}
