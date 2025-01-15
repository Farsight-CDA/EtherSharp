namespace EtherSharp.StateOverride;
public class TxStateOverride
{
    internal readonly Dictionary<string, OverrideAccount> _accountOverrides;

    internal TxStateOverride(Dictionary<string, OverrideAccount> accountOverrides)
    {
        _accountOverrides = accountOverrides;
    }
}
