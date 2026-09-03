using EtherSharp.Numerics;

namespace EtherSharp.Interpreter.Memory;

internal sealed class ReturnDataBuffer
{
    private ReadOnlyMemory<byte> _data;

    public int Length => _data.Length;

    public void Set(ReadOnlyMemory<byte> data)
        => _data = data;

    public bool TryCopyTo(UInt256 offset, LinearMemory.Slice destination)
    {
        if(offset > (UInt256) Length)
        {
            return false;
        }

        int start = (int) offset;
        if(destination.Length > Length - start)
        {
            return false;
        }

        _data.Span.Slice(start, destination.Length).CopyTo(destination.Span);
        return true;
    }
}
