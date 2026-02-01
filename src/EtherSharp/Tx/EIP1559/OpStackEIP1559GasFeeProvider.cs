using EtherSharp.Client.Services;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Common;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Tx.EIP1559;

public class OpStackEIP1559GasFeeProvider(IEthRpcModule ethRpcModule, IEtherSigner signer)
    : IInitializableService, IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>
{
    private readonly static EIP1559GasParams _defaultGasParams = new EIP1559GasParams(1_000_000, UInt256.Pow(10, 10), UInt256.Pow(10, 8));
    private readonly static Address _opGasOracleAddress = Address.FromString("0x420000000000000000000000000000000000000F");

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly IEtherSigner _signer = signer;

    private bool _initialized;
    private ulong _chainId;

    public ulong GasWantedOffsetPercentage { get; set; } = 15;

    public ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken = default)
    {
        _initialized = true;
        _chainId = chainId;
        return ValueTask.CompletedTask;
    }

    public Task<EIP1559GasParams> EstimateGasParamsAsync(ITxInput txInput, EIP1559TxParams txParams, CancellationToken cancellationToken)
    {
        if(!_initialized)
        {
            throw new InvalidOperationException("Not initialized");
        }

        var mockTx = EIP1559Transaction.Create(_chainId, txParams, _defaultGasParams, txInput, 1_000);
        Span<int> listSizes = stackalloc int[EIP1559Transaction.NestedListCount];

        int txByteSize = mockTx.GetEncodedSize(listSizes);
        int simulationBufferSize = (32 * ((txByteSize - 1) / 32)) + 32 + 69;

        byte[]? rented = null;
        var simulationPayloadBuffer = simulationBufferSize <= 4096
            ? stackalloc byte[simulationBufferSize]
            : (rented = ArrayPool<byte>.Shared.Rent(simulationBufferSize)).AsSpan(0, simulationBufferSize);

        if(rented is not null)
        {
            simulationPayloadBuffer.Clear();
        }

        try
        {
            simulationPayloadBuffer[0] = 0x49;
            simulationPayloadBuffer[1] = 0x94;
            simulationPayloadBuffer[2] = 0x8e;
            simulationPayloadBuffer[3] = 0x0e;

            simulationPayloadBuffer[35] = 32;

            BinaryPrimitives.WriteInt32BigEndian(simulationPayloadBuffer[64..], txByteSize);

            simulationPayloadBuffer[68] = EIP1559Transaction.PrefixByte;
            mockTx.Encode(listSizes, simulationPayloadBuffer[69..]);

            return SendEstimationRequestsAsync(
                txInput,
                HexUtils.ToPrefixedHexString(simulationPayloadBuffer),
                cancellationToken
            );
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    private async Task<EIP1559GasParams> SendEstimationRequestsAsync(ITxInput txInput, string getL1FeePayloadHex, CancellationToken cancellationToken)
    {
        var gasEstimationTask = _ethRpcModule.EstimateGasAsync(
            _signer.Address, txInput.To, txInput.Value, HexUtils.ToPrefixedHexString(txInput.Data.Span), cancellationToken);
        var l1FeeTask = _ethRpcModule.CallAsync(
            null, _opGasOracleAddress, null, null, 0, getL1FeePayloadHex, TargetBlockNumber.Pending, cancellationToken
        );

        ulong gasEstimation = await gasEstimationTask;
        var gasPriceTask = _ethRpcModule.GasPriceAsync(cancellationToken);
        var priorityFeeTask = _ethRpcModule.MaxPriorityFeePerGasAsync(cancellationToken);

        var l1FeeCallResult = await l1FeeTask;
        var gasPrice = await gasPriceTask;
        var priorityFee = await priorityFeeTask;

        var l1Fee = BinaryPrimitives.ReadUInt256BigEndian(l1FeeCallResult.Unwrap(_opGasOracleAddress).Span);
        var l1FeePerGas = l1Fee / gasEstimation;

        return new EIP1559GasParams(
            gasEstimation * (100 + GasWantedOffsetPercentage) / 100,
            (l1FeePerGas + gasPrice) * 11 / 10,
            priorityFee * 1 / 10
        );
    }
}
