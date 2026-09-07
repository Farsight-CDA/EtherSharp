using EtherSharp.Numerics;

namespace EtherSharp.Interpreter.Runtime.Memory;

internal sealed class ReturnDataBuffer
{
    public int Length => Data.Length;
    public ReadOnlyMemory<byte> Data { get; private set; }

    public void Set(ReadOnlyMemory<byte> data)
        => Data = data;

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

        Data.Span.Slice(start, destination.Length).CopyTo(destination.Span);
        return true;
    }
}
