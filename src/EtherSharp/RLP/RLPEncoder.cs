using System.Numerics;

public static class RLPEncoder
{
    public static void EncodeSpan(ReadOnlySpan<byte> input, Span<byte> output)
    {
        // [0x00, 0x7f] "string"
        if(input.Length == 1 && input[0] <= 0x7f)
        {
            input.CopyTo(output);
            return;
        }

        // 0-55 bytes long
        if(input.Length <= 55)
        {
            output[0] = (byte) (0x80 + input.Length);
            input.CopyTo(output[1..]);
            return;
        }

        // more than 55 bytes long,
        int lengthOfLength = GetLengthOfLength((uint) input.Length);

        output[0] = (byte) (0xb7 + lengthOfLength);
        WriteLength(input.Length, output.Slice(1, lengthOfLength));
        input.CopyTo(output[(1 + lengthOfLength)..]);
    }

    public static void EncodeInt(BigInteger input, Span<byte> output)
    {
        // Special case for zero
        if(input == 0)
        {
            output[0] = 0x80;
            return;
        }

        Span<byte> intBytes = stackalloc byte[sizeof(int)];
        int actualLength = WriteIntToSpan(input, intBytes);

        ReadOnlySpan<byte> trimmedBytes = intBytes[(sizeof(int) - actualLength)..];

        EncodeSpan(trimmedBytes, output);
    }

    public static void EncodeList(ReadOnlySpan<byte> payload, Span<byte> output)
    {
        //  0-55 bytes long
        if(payload.Length <= 55)
        {
            output[0] = (byte) (0xc0 + payload.Length);
            payload.CopyTo(output[1..]);

            return;
        }

        // more than 55 bytes long
        int lengthOfLength = GetLengthOfLength((uint) payload.Length);

        output[0] = (byte) (0xf7 + lengthOfLength);

        WriteLength(payload.Length, output.Slice(1, lengthOfLength));

        payload.CopyTo(output[(1 + lengthOfLength)..]);
    }

    private static int GetLengthOfLength(uint length)
    {
        int bits = System.Numerics.BitOperations.LeadingZeroCount(length);
        return ((sizeof(int) * 8) - bits + 7) / 8;
    }

    private static void WriteLength(int length, Span<byte> output)
    {
        for(int i = output.Length - 1; i >= 0; i--)
        {
            output[i] = (byte) (length & 0xFF);
            length >>= 8;
        }
    }

    private static int WriteIntToSpan(BigInteger value, Span<byte> output)
    {
        int bytesWritten = 0;
        for(int i = sizeof(int) - 1; i >= 0; i--)
        {
            byte currentByte = (byte) ((value >> (i * 8)) & 0xFF);
            if(currentByte != 0 || bytesWritten > 0)
            {
                output[sizeof(int) - 1 - bytesWritten] = currentByte;
                bytesWritten++;
            }
        }
        return bytesWritten;
    }

    private static int GetMaxEncodeSpanSize(ReadOnlySpan<byte> input)
    {
        // [0x00, 0x7f] "string"
        if(input.Length == 1 && input[0] <= 0x7f)
        {
            return 1;
        }

        // 0-55 bytes long
        if(input.Length <= 55)
        {
            return input.Length + 1;
        }

        // Long string
        int lengthOfLength = GetLengthOfLength((uint) input.Length);
        return input.Length + 1 + lengthOfLength;
    }

    public static int GetMaxEncodeIntSize(BigInteger input)
    {
        // Special case for zero
        if(input == 0)
        {
            return 1;
        }

        // Calculate max bytes for int
        int actualLength = sizeof(int) - (System.Numerics.BitOperations.LeadingZeroCount((uint) input) / 8);
        return GetMaxEncodeSpanSize(new byte[actualLength]);
    }

    public static int GetMaxEncodeListSize(ReadOnlySpan<byte> payload)
    {
        // Short list (0-55 bytes)
        if(payload.Length <= 55)
        {
            return payload.Length + 1;
        }

        // Long list
        int lengthOfLength = GetLengthOfLength((uint) payload.Length);
        return payload.Length + 1 + lengthOfLength;
    }
}