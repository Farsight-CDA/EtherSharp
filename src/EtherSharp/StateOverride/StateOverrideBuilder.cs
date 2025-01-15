using EtherSharp.Types;

namespace EtherSharp.StateOverride;
public sealed class StateOverrideBuilder
{
    private readonly Dictionary<string, OverrideAccount> _accountOverrides = [];

    public StateOverrideBuilder AddContract(Address address, Action<ContractStateOverrideBuilder> configureContract)
    {
        if(_accountOverrides.ContainsKey(address.String))
        {
            throw new InvalidOperationException($"Contract {address.String} already configured");
        }

        var builder = new ContractStateOverrideBuilder();
        configureContract(builder);
        _accountOverrides.Add(address.String, builder.Build());

        return this;
    }

    public TxStateOverride Build() 
        => new TxStateOverride(_accountOverrides);
}
