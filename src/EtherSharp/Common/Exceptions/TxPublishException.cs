using EtherSharp.Client.Services.TxPublisher;

namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Represents a failure while publishing a signed transaction to the network.
/// </summary>
public sealed class TxPublishException : Exception
{
    /// <summary>
    /// The submission result that caused the failure, if available.
    /// </summary>
    public TxSubmissionResult? Result { get; }

    /// <summary>
    /// Creates a new <see cref="TxPublishException"/>.
    /// </summary>
    /// <param name="message">Description of the publish failure returned by the client or node.</param>
    public TxPublishException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TxPublishException"/> from a failed submission result.
    /// </summary>
    /// <param name="result">The submission result that caused the failure.</param>
    public TxPublishException(TxSubmissionResult result)
        : base(result.Describe(), (result as TxSubmissionResult.UnhandledException)?.Exception)
    {
        Result = result;
    }
}
