using EtherSharp.Client.Services;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Provides gas fee estimates for OP Stack EIP-1559 transactions.
/// </summary>
public sealed class OpStackEIP1559GasFeeProvider : IInitializableService, IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>
{
    /// <summary>
    /// Configuration values used to tune OP Stack gas estimation.
    /// </summary>
    public sealed class Configuration
    {
        /// <summary>
        /// Percentage added to the estimated gas limit.
        /// </summary>
        public ulong GasWantedOffsetPercentage { get; set; } = 15;
    }

    private readonly static EIP1559GasParams _defaultGasParams = new EIP1559GasParams(1_000_000, UInt256.Pow(10, 10), UInt256.Pow(10, 8));
    private readonly static Address _opGasOracleAddress = Address.FromString("0x420000000000000000000000000000000000000F");

    private readonly IEthRpcModule _ethRpcModule;

    private readonly ulong _gasWantedOffsetPercentage;

    private bool _initialized;
    private ulong _chainId;

    /// <summary>
    /// Creates a new <see cref="OpStackEIP1559GasFeeProvider"/>.
    /// </summary>
    /// <param name="ethRpcModule">RPC module used to query fee and gas data.</param>
    /// <param name="configuration">Optional gas estimation tuning values.</param>
    public OpStackEIP1559GasFeeProvider(
        IEthRpcModule ethRpcModule,
        OpStackEIP1559GasFeeProvider.Configuration? configuration = null
    )
    {
        var resolvedConfiguration = configuration ?? new OpStackEIP1559GasFeeProvider.Configuration();

        _ethRpcModule = ethRpcModule;
        _gasWantedOffsetPercentage = resolvedConfiguration.GasWantedOffsetPercentage;
    }

    /// <inheritdoc/>
    public ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken = default)
    {
        _initialized = true;
        _chainId = chainId;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<EIP1559GasParams> EstimateGasParamsAsync(ITxInput txInput, EIP1559TxParams txParams, Address from, CancellationToken cancellationToken)
    {
        if(!_initialized)
        {
            throw new InvalidOperationException("Not initialized");
        }

        var mockTx = EIP1559Transaction.Create(_chainId, txParams, _defaultGasParams, txInput, 1_000);
        Span<int> listSizes = stackalloc int[EIP1559Transaction.NestedListCount];

        int txByteSize = mockTx.GetEncodedSize(listSizes) + 1;
        int simulationBufferSize = 68 + (32 * ((txByteSize + 31) / 32));

        byte[] rented = ArrayPool<byte>.Shared.Rent(simulationBufferSize);
        var simulationPayload = rented.AsMemory(0, simulationBufferSize);

        try
        {
            simulationPayload.Span.Clear();
            simulationPayload.Span[0] = 0x49;
            simulationPayload.Span[1] = 0x94;
            simulationPayload.Span[2] = 0x8e;
            simulationPayload.Span[3] = 0x0e;

            simulationPayload.Span[35] = 32;

            BinaryPrimitives.WriteInt32BigEndian(simulationPayload.Span[64..], txByteSize);

            simulationPayload.Span[68] = EIP1559Transaction.PrefixByte;
            mockTx.Encode(listSizes, simulationPayload.Span[69..]);

            return await SendEstimationRequestsAsync(
                from,
                txInput,
                txParams.AccessList,
                simulationPayload,
                cancellationToken
            );
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private async Task<EIP1559GasParams> SendEstimationRequestsAsync(
        Address sender, ITxInput txInput, StateAccess[] accessList,
        ReadOnlyMemory<byte> getL1FeePayload, CancellationToken cancellationToken)
    {
        var gasEstimationTask = _ethRpcModule.EstimateGasAsync(
            sender, txInput.To, txInput.Value, txInput.Data, accessList, cancellationToken);
        var l1FeeTask = _ethRpcModule.CallAsync(
            null, _opGasOracleAddress, null, null, 0, getL1FeePayload, TargetHeight.Pending, cancellationToken
        );
        var gasPriceTask = _ethRpcModule.GasPriceAsync(cancellationToken);
        var priorityFeeTask = _ethRpcModule.MaxPriorityFeePerGasAsync(cancellationToken);

        ulong gasEstimation = await gasEstimationTask;
        var l1FeeCallResult = await l1FeeTask;
        var gasPrice = await gasPriceTask;
        var priorityFee = await priorityFeeTask;

        if(!l1FeeCallResult.Success)
        {
            throw CallRevertedException.Parse(_opGasOracleAddress, l1FeeCallResult.Data.Span);
        }

        var l1Fee = BinaryPrimitives.ReadUInt256BigEndian(l1FeeCallResult.Data.Span);
        var l1FeePerGas = l1Fee / gasEstimation;

        return new EIP1559GasParams(
            gasEstimation * (100 + _gasWantedOffsetPercentage) / 100,
            (l1FeePerGas + gasPrice) * 11 / 10,
            priorityFee * 1 / 10
        );
    }
}
