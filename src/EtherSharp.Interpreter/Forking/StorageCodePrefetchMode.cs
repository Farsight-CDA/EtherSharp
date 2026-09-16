namespace EtherSharp.Interpreter.Forking;

/// <summary>Controls speculative code prefetching from address-shaped storage values.</summary>
public enum StorageCodePrefetchMode
{
    /// <summary>Prefetches code from known proxy implementation and beacon slots. This is the default.</summary>
    Proxy = 0,

    /// <summary>Disables code prefetching from storage values.</summary>
    Disabled = 1,

    /// <summary>Prefetches code from every explicitly requested slot whose value is a canonical contract address.</summary>
    Full = 2,
}
