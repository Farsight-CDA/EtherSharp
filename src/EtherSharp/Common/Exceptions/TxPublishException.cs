namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Represents a failure while publishing a signed transaction to the network.
/// </summary>
/// <param name="message">Description of the publish failure returned by the client or node.</param>
public class TxPublishException(string message)
    : Exception(message)
{
}
