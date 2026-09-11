namespace EtherSharp.Interpreter.Forking;

/// <summary>Configures upstream data provider creation by the pre/post-block client helpers.</summary>
public readonly record struct InterpreterDataProviderOptions
{
    /// <summary>
    /// The number of subsequent storage slots to fetch alongside each requested slot, from zero to 255.
    /// Defaults to zero (disabled).
    /// </summary>
    public byte ForwardPrefetchDistance { get; init; }

    /// <summary>
    /// Gets a factory that optionally wraps the built-in data provider.
    /// </summary>
    /// <remarks>
    /// Invoked once per fork with the configured, block-pinned provider.
    /// When null, the built-in provider is used directly.
    /// The factory must return a non-null provider; otherwise, fork creation throws <see cref="InvalidOperationException"/>.
    /// </remarks>
    public Func<IInterpreterDataProvider, IInterpreterDataProvider>? DecorateProvider { get; init; }
}
