namespace EtherSharp;
public class EtherClientBuilder
{
    private Uri? _rpcUrl;

    public EtherClientBuilder WithRPCUrl(Uri rpcUrl)
    {
        _rpcUrl = rpcUrl;
        return this;
    }
    public EtherClientBuilder WithRPCURl(string rpcURl)
    {
        _rpcUrl = new Uri(rpcURl, UriKind.Absolute);
        return this;
    }

    public EtherClient Build()
    {
        if (_rpcUrl is null)
        {
            throw new InvalidOperationException($"No RPCUrl configured. Call the {nameof(WithRPCUrl)} method prior to {nameof(Build)}.");
        }
        //
        return new EtherClient(
            new EvmRpcClient(
                new JsonRpcClient(
                    new HttpClient(),
                    _rpcUrl
                )
            )
        );
    }
}
