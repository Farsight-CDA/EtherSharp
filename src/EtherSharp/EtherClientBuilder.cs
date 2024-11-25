using EtherSharp.RPC;
using EtherSharp.Wallet;

namespace EtherSharp;
public class EtherClientBuilder
{
    private Uri? _rpcUrl;
    private IEtherSigner? _signer;

    public EtherClientBuilder WithRPCUrl(Uri rpcUrl)
    {
        _rpcUrl = rpcUrl;
        return this;
    }
    public EtherClientBuilder WithRPCUrl(string rpcURl)
    {
        _rpcUrl = new Uri(rpcURl, UriKind.Absolute);
        return this;
    }

    public EtherClientBuilder WithSigner(IEtherSigner signer)
    {
        _signer = signer;
        return this;
    }

    private void AssertReadClientConfiguration()
    {
        if(_rpcUrl is null)
        {
            throw new InvalidOperationException($"No RPCUrl configured. Call the {nameof(WithRPCUrl)} method prior to {nameof(BuildReadClient)}.");
        }
    }
    public IEtherClient BuildReadClient()
    {
        AssertReadClientConfiguration();

        return new EtherClient(
            new EvmRpcClient(
                new JsonRpcClient(
                    new HttpClient(),
                    _rpcUrl!
                )
            )
        );
    }

    private void AssertTxClientConfiguration()
    {
        AssertReadClientConfiguration();

        if(_signer is null)
        {
            throw new InvalidOperationException($"No Signer configured. Call the {nameof(WithSigner)} method prior to {nameof(BuildTxClient)}");
        }
    }
    public IEtherTxClient BuildTxClient()
    {
        AssertTxClientConfiguration();

        return new EtherClient(
            new EvmRpcClient(
                new JsonRpcClient(
                    new HttpClient(),
                    _rpcUrl!
                )
            ),
            _signer!
        );
    }
}
