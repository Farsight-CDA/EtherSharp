namespace EtherSharp.RLP;

/// <summary>
/// Prefix bytes and thresholds of the Recursive Length Prefix (RLP) encoding.
/// </summary>
public static class RLPConstants
{
    /// <summary>
    /// Exclusive upper bound of single bytes that are encoded as themselves, without a prefix.
    /// </summary>
    public const byte SINGLE_BYTE_LIMIT = 0x80;

    /// <summary>
    /// Exclusive upper bound of payload lengths that are encoded in the short form.
    /// </summary>
    public const int SHORT_PAYLOAD_LIMIT = 56;

    /// <summary>
    /// Prefix of strings with a payload shorter than <see cref="SHORT_PAYLOAD_LIMIT"/>. The payload length is added to it.
    /// </summary>
    public const byte SHORT_STRING_PREFIX = 0x80;

    /// <summary>
    /// Prefix of strings with a payload of at least <see cref="SHORT_PAYLOAD_LIMIT"/> bytes. The byte count of the payload length is added to it.
    /// </summary>
    public const byte LONG_STRING_PREFIX = 0xb7;

    /// <summary>
    /// Prefix of lists with a payload shorter than <see cref="SHORT_PAYLOAD_LIMIT"/>. The payload length is added to it.
    /// </summary>
    public const byte SHORT_LIST_PREFIX = 0xc0;

    /// <summary>
    /// Prefix of lists with a payload of at least <see cref="SHORT_PAYLOAD_LIMIT"/> bytes. The byte count of the payload length is added to it.
    /// </summary>
    public const byte LONG_LIST_PREFIX = 0xf7;
}
