namespace EtherSharp.RLP;

public ref partial struct RLPDecoder
{
    /// <summary>
    /// Checks whether <paramref name="encoded"/> is exactly one canonically encoded RLP item, including all nested items.
    /// </summary>
    /// <param name="encoded">The encoded item to validate.</param>
    public static bool IsCanonical(ReadOnlySpan<byte> encoded)
    {
        if(!RLPHeader.TryRead(encoded, out var root) || root.TotalLength != encoded.Length)
        {
            return false;
        }

        // Visits every item in pre-order: strings are skipped, lists are entered.
        // Before a list is entered, its direct children are checked to exactly fill the list payload.
        // This keeps every nested item within its parent without needing a stack, so untrusted
        // input of arbitrary nesting depth is validated in constant stack space.
        int position = 0;
        while(position < encoded.Length)
        {
            if(!RLPHeader.TryRead(encoded[position..], out var header))
            {
                return false;
            }

            position += header.PrefixLength;

            if(!header.IsList)
            {
                position += header.PayloadLength;
            }
            else if(!ChildrenFillPayload(encoded.Slice(position, header.PayloadLength)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ChildrenFillPayload(ReadOnlySpan<byte> payload)
    {
        while(!payload.IsEmpty)
        {
            if(!RLPHeader.TryRead(payload, out var header))
            {
                return false;
            }

            payload = payload[header.TotalLength..];
        }

        return true;
    }
}
