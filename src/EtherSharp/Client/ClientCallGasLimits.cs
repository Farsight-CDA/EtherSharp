namespace EtherSharp.Client;

internal sealed record ClientCallGasLimits(
    ulong? EthCallGasLimit = null,
    ulong? FlashCallGasLimit = null
);
