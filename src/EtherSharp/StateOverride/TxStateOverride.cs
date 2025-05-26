using EtherSharp.Types;

namespace EtherSharp.StateOverride;
public class TxStateOverride
{
    internal readonly Dictionary<Address, OverrideAccount> _accountOverrides;

    internal TxStateOverride(Dictionary<Address, OverrideAccount> accountOverrides)
    {
        _accountOverrides = accountOverrides;
    }
}
