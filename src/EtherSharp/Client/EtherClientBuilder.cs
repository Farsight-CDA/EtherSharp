using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Common.Extensions;
using EtherSharp.RPC;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client;
public class EtherClientBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    private Uri? _rpcUrl;

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
        _services.AddOrReplaceSingleton<IEtherSigner, IEtherSigner>(signer);
        return this;
    }

    public EtherClientBuilder WithTxScheduler<TTxScheduler>()
        where TTxScheduler : class, ITxScheduler
    {
        _services.AddOrReplaceSingleton<ITxScheduler, TTxScheduler>();
        return this;
    }

    public EtherClientBuilder WithTxPublisher<TTxPublisher>()
        where TTxPublisher : class, ITxPublisher
    {
        _services.AddOrReplaceSingleton<ITxPublisher, TTxPublisher>();
        return this;
    }

    private void AssertReadClientConfiguration()
    {
        if(_rpcUrl is null)
        {
            throw new InvalidOperationException($"No RPCUrl configured. Call the {nameof(WithRPCUrl)} method prior to {nameof(BuildReadClient)}.");
        }

        var evmRpcClient = new EvmRpcClient(
            new JsonRpcClient(
                new HttpClient(),
                _rpcUrl!
            )
        );

        _services.AddSingleton(evmRpcClient);
    }
    public IEtherClient BuildReadClient()
    {
        AssertReadClientConfiguration();

        var provider = _services.BuildServiceProvider();
        return new EtherClient(
            provider,
            false
        );
    }

    private void AssertTxClientConfiguration()
    {
        AssertReadClientConfiguration();

        if(!_services.Any(x => x.ServiceType == typeof(IEtherSigner)))
        {
            throw new InvalidOperationException($"No {nameof(IEtherSigner)} configured. Call the {nameof(WithSigner)} method prior to {nameof(BuildTxClient)}");
        }
        if(!_services.Any(x => x.ServiceType == typeof(ITxScheduler)))
        {
            throw new InvalidOperationException($"No {nameof(ITxScheduler)} configured. Call the {nameof(WithTxScheduler)} method prior to {nameof(BuildTxClient)}");
        }
        if(!_services.Any(x => x.ServiceType == typeof(ITxPublisher)))
        {
            throw new InvalidOperationException($"No {nameof(ITxPublisher)} configured. Call the {nameof(WithTxPublisher)} method prior to {nameof(BuildTxClient)}");
        }
    }
    public IEtherTxClient BuildTxClient()
    {
        AssertTxClientConfiguration();

        var provider = _services.BuildServiceProvider();
        return new EtherClient(
            provider,
            true
        );
    }
}
