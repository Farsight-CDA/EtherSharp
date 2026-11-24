namespace EtherSharp.RLP;

/// <summary>
/// Describes the prefix of an encoded Recursive Length Prefix (RLP) item.
/// </summary>
public readonly struct RLPHeader
{
    /// <summary>
    /// Gets whether the item is a list.
    /// </summary>
    public bool IsList { get; }
    /// <summary>
    /// Gets the number of prefix bytes. Zero for single bytes encoded as themselves.
    /// </summary>
    public int PrefixLength { get; }
    /// <summary>
    /// Gets the number of payload bytes following the prefix.
    /// </summary>
    public int PayloadLength { get; }
    /// <summary>
    /// Gets the total encoded length of the item.
    /// </summary>
    public int TotalLength
        => PrefixLength + PayloadLength;

    private RLPHeader(bool isList, int prefixLength, int payloadLength)
    {
        IsList = isList;
        PrefixLength = prefixLength;
        PayloadLength = payloadLength;
    }

    /// <summary>
    /// Reads the header of the item at the start of <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The buffer starting with an encoded item.</param>
    /// <param name="header">The header of the item.</param>
    /// <returns>Whether the prefix is canonical and the payload fits within <paramref name="source"/>.</returns>
    public static bool TryRead(ReadOnlySpan<byte> source, out RLPHeader header)
    {
        header = default;

        if(source.IsEmpty)
        {
            return false;
        }

        byte prefix = source[0];
        if(prefix < RLPConstants.SINGLE_BYTE_LIMIT)
        {
            header = new RLPHeader(false, 0, 1);
            return true;
        }

        bool isList = prefix >= RLPConstants.SHORT_LIST_PREFIX;
        int shortPrefix = isList ? RLPConstants.SHORT_LIST_PREFIX : RLPConstants.SHORT_STRING_PREFIX;
        int longPrefix = isList ? RLPConstants.LONG_LIST_PREFIX : RLPConstants.LONG_STRING_PREFIX;

        int prefixLength;
        int payloadLength;
        if(prefix <= longPrefix)
        {
            prefixLength = 1;
            payloadLength = prefix - shortPrefix;
        }
        else
        {
            int lengthBytes = prefix - longPrefix;
            if(lengthBytes > sizeof(int) || lengthBytes >= source.Length || source[1] == 0)
            {
                return false;
            }

            uint size = 0;
            foreach(byte b in source.Slice(1, lengthBytes))
            {
                size = (size << 8) | b;
            }

            if(size < RLPConstants.SHORT_PAYLOAD_LIMIT || size > Int32.MaxValue)
            {
                return false;
            }

            prefixLength = 1 + lengthBytes;
            payloadLength = (int) size;
        }

        // A single byte below SINGLE_BYTE_LIMIT must be encoded as itself, without a string prefix.
        if(payloadLength > source.Length - prefixLength
            || (!isList && payloadLength == 1 && source[prefixLength] < RLPConstants.SINGLE_BYTE_LIMIT))
        {
            return false;
        }

        header = new RLPHeader(isList, prefixLength, payloadLength);
        return true;
    }
}
