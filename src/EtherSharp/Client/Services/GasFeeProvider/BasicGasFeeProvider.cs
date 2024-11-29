using EtherSharp.Client.Services.RPC;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Wallet;

namespace EtherSharp.Client.Services.GasFeeProvider;
public class BasicGasFeeProvider(IRpcClient rpcClient, IEtherSigner signer) : IGasFeeProvider
{
    private readonly IRpcClient _rpcClient = rpcClient;
    private readonly IEtherSigner _signer = signer;

    public Task<ulong> EstimateGasAsync(ITxInput txInput, ReadOnlySpan<byte> data) 
        => _rpcClient.EthEstimateGasAsync(_signer.Address.String, txInput.To.String, txInput.Value, $"0x{Convert.ToHexString(data)}");

    public async Task<ITxGasParams> CalculateGasParamsAsync<TTxParams>(ITxInput txInput, TTxParams txParams, ulong gas)
        where TTxParams : ITxParams
    {
        switch(txParams)
        {
            case EIP1559TxParams:
                var gasPriceTask = _rpcClient.EthGasPriceAsync();
                var priorityFeeTask = _rpcClient.EthMaxPriorityFeePerGas();

                var gasPrice = await gasPriceTask;
                var priorityFee = await priorityFeeTask;

                return new EIP1559GasParams(gasPrice, priorityFee);
            default:
                throw new NotSupportedException();
        }
    }
}
