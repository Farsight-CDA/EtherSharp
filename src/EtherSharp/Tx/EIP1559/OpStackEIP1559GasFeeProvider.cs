using EtherSharp.Client.Services;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Tx.EIP1559;
public class OpStackEIP1559GasFeeProvider(IRpcClient rpcClient, IEtherSigner signer)
    : IInitializableService, IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>
{
    private readonly static EIP1559GasParams _defaultGasParams = new EIP1559GasParams(1_000_000, BigInteger.Pow(10, 10), BigInteger.Pow(10, 8));

    private readonly IRpcClient _rpcClient = rpcClient;
    private readonly IEtherSigner _signer = signer;

    private bool _initialized;
    private ulong _chainId;

    public ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken = default)
    {
        _initialized = true;
        _chainId = chainId;
        return ValueTask.CompletedTask;
    }

    public Task<EIP1559GasParams> EstimateGasParamsAsync(
        Address to, BigInteger value, ReadOnlySpan<byte> inputData,
        EIP1559TxParams txParams, CancellationToken cancellationToken)
    {
        if(!_initialized)
        {
            throw new InvalidOperationException("Not initialized");
        }

        var mockTx = EIP1559Transaction.Create(_chainId, txParams, _defaultGasParams, to, value, 1_000);
        Span<int> listSizes = stackalloc int[EIP1559Transaction.NestedListCount];

        int txByteSize = mockTx.GetEncodedSize(inputData, listSizes);
        int simulationBufferSize = (32 * ((txByteSize - 1) / 32)) + 32 + 69;

        Span<byte> simulationPayloadBuffer = simulationBufferSize > 8192
            ? new byte[simulationBufferSize]
            : stackalloc byte[simulationBufferSize];

        simulationPayloadBuffer[0] = 0x49;
        simulationPayloadBuffer[1] = 0x94;
        simulationPayloadBuffer[2] = 0x8e;
        simulationPayloadBuffer[3] = 0x0e;

        simulationPayloadBuffer[35] = 32;

        BinaryPrimitives.WriteInt32BigEndian(simulationPayloadBuffer[64..], txByteSize);

        simulationPayloadBuffer[68] = EIP1559Transaction.PrefixByte;
        mockTx.Encode(listSizes, inputData, simulationPayloadBuffer[69..]);

        return SendEstimationRequestsAsync(
            to,
            value,
            $"0x{Convert.ToHexString(inputData)}",
            $"0x{Convert.ToHexString(simulationPayloadBuffer)}",
            cancellationToken
        );
    }

    private async Task<EIP1559GasParams> SendEstimationRequestsAsync(Address to, BigInteger value,
        string inputDataHex, string getL1FeePayloadHex, CancellationToken cancellationToken)
    {
        var gasEstimationTask = _rpcClient.EthEstimateGasAsync(
            _signer.Address.String, to.String, value, inputDataHex, cancellationToken);
        var l1FeeTask = _rpcClient.EthCallAsync(
            null, "0x420000000000000000000000000000000000000F", null, null, null, getL1FeePayloadHex, TargetBlockNumber.Pending, default, cancellationToken);

        ulong gasEstimation = await gasEstimationTask;

        var gasPriceTask = _rpcClient.EthGasPriceAsync(cancellationToken);
        var priorityFeeTask = _rpcClient.EthMaxPriorityFeePerGas(cancellationToken);

        var l1FeeCallResult = await l1FeeTask;
        var gasPrice = await gasPriceTask;
        var priorityFee = await priorityFeeTask;

        if(l1FeeCallResult is not TxCallResult.Success successResult)
        {
            throw new NotSupportedException("l1 gas estimation call reverted");
        }

        var l1Fee = new BigInteger(successResult.Data, true, true);
        var l1FeePerGas = l1Fee / gasEstimation;

        return new EIP1559GasParams(
            gasEstimation,
            (l1FeePerGas + gasPrice) * 11 / 10,
            priorityFee * 1 / 10
        );
    }
}
