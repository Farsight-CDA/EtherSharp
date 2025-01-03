using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Wallet;

namespace EtherSharp.Client.Services.TxTypeHandler;
public abstract class BaseTxTypeHandler<TTransaction, TTxParams, TTxGasParams>(IEtherSigner signer) 
    : ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>, IInitializableService
    where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
    where TTxParams : ITxParams
    where TTxGasParams : ITxGasParams
{
    protected readonly IEtherSigner _signer = signer;

    protected bool _isInitialized;
    protected ulong _chainId;

    public ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken = default)
    {
        _chainId = chainId;
        _isInitialized = true;
        return ValueTask.CompletedTask;
    }

    Task<ITxGasParams> ITxTypeHandler.CalculateGasParamsAsync(
        ITxInput txInput, ITxParams txParams, ReadOnlySpan<byte> inputData, CancellationToken cancellationToken)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException($"TxTypeHandler {GetType().FullName} is not initialized");
        }

        var gasParamsTask = CalculateGasParamsAsync(txInput, (TTxParams) txParams, $"0x{Convert.ToHexString(inputData)}", cancellationToken);
        return WrapAsync(gasParamsTask);
    }

    protected abstract Task<TTxGasParams> CalculateGasParamsAsync(
        ITxInput txInput, TTxParams txParams,
        string inputDataHex, CancellationToken cancellationToken
    );

    private static async Task<ITxGasParams> WrapAsync(Task<TTxGasParams> task) 
        => await task;

    string ITxTypeHandler.EncodeTxToBytes(
        ITxInput txInput, ITxParams txParams, ITxGasParams gasParams,
        ReadOnlySpan<byte> inputData, uint nonce)
    {
        if(!_isInitialized)
        {
            throw new InvalidOperationException($"TxTypeHandler {GetType().FullName} is not initialized");
        }
        //
        return EncodeTxToBytes(txInput, (TTxParams) txParams, (TTxGasParams) gasParams, inputData, nonce);
    }

    protected abstract string EncodeTxToBytes(
        ITxInput txInput, TTxParams txParams, TTxGasParams gasParams,
        ReadOnlySpan<byte> inputData, uint nonce
    );
}
