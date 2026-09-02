using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Memory;

internal readonly struct ZeroPaddedData(ReadOnlyMemory<byte> data)
{
    public int Length => data.Length;
    public ReadOnlyMemory<byte> Data => data;

    public byte this[int index]
    {
        get {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            return index < Length ? data.Span[index] : (byte) 0;
        }
    }

    public Bytes32 ReadAtOffset(int offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        Span<byte> value = stackalloc byte[Bytes32.BYTE_LENGTH];
        value.Clear();

        if(offset < Length)
        {
            data.Span.Slice(offset, Math.Min(Bytes32.BYTE_LENGTH, Length - offset)).CopyTo(value);
        }

        return Bytes32.FromBytes(value);
    }

    public void CopyTo(UInt256 offset, LinearMemory.Slice destination)
    {
        destination.Span.Clear();
        if(offset >= (UInt256) Length)
        {
            return;
        }

        int start = (int) offset;
        data.Span.Slice(start, Math.Min(destination.Length, Length - start)).CopyTo(destination.Span);
    }
}
