using EtherSharp.Types;

namespace EtherSharp.StateOverride;
public sealed class StateOverrideBuilder
{
    private readonly Dictionary<Address, OverrideAccount> _accountOverrides = [];

    public StateOverrideBuilder AddContract(Address address, Action<ContractStateOverrideBuilder> configureContract)
    {
        if(_accountOverrides.ContainsKey(address))
        {
            throw new InvalidOperationException($"Contract {address} already configured");
        }

        var builder = new ContractStateOverrideBuilder();
        configureContract(builder);
        _accountOverrides.Add(address, builder.Build());

        return this;
    }

    public TxStateOverride Build()
        => new TxStateOverride(_accountOverrides);
}
