using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client;

public interface IInternalEtherClientBuilder
{
    public IServiceCollection Services { get; }
}
