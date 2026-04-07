namespace EtherSharp.Client;

/// <summary>
/// Mutable runtime defaults for client-side call gas handling.
/// </summary>
internal sealed class CallGasLimitSettings
{
    private readonly Lock _lock = new Lock();
    private ulong? _ethCallGasLimit;
    private ulong? _flashCallGasLimit;

    public CallGasLimitSettings(ulong? ethCallGasLimit, ulong? flashCallGasLimit)
    {
        Set(ethCallGasLimit, flashCallGasLimit);
    }

    public ulong? GetEthCallGasLimit()
    {
        lock(_lock)
        {
            return _ethCallGasLimit;
        }
    }

    public ulong? GetFlashCallGasLimit()
    {
        lock(_lock)
        {
            return _flashCallGasLimit;
        }
    }

    public void Set(ulong? ethCallGasLimit, ulong? flashCallGasLimit)
    {
        Validate(ethCallGasLimit, flashCallGasLimit);

        lock(_lock)
        {
            _ethCallGasLimit = ethCallGasLimit;
            _flashCallGasLimit = flashCallGasLimit;
        }
    }

    public static void Validate(ulong? ethCallGasLimit, ulong? flashCallGasLimit)
    {
        if(ethCallGasLimit == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ethCallGasLimit), "Configured eth_call gas limit must be greater than zero.");
        }

        if(flashCallGasLimit == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(flashCallGasLimit), "Configured flash-call gas limit must be greater than zero.");
        }

        if(
            ethCallGasLimit is not null
            && flashCallGasLimit is not null
            && flashCallGasLimit > ethCallGasLimit)
        {
            throw new ArgumentOutOfRangeException(nameof(flashCallGasLimit), "Configured flash-call gas limit cannot exceed the configured eth_call gas limit.");
        }
    }
}
