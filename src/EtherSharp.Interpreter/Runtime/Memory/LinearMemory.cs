using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Memory;

internal sealed class LinearMemory(int maxSize)
{
    public readonly ref struct Slice(LinearMemory owner, int offset, int length)
    {
        public int Length => length;
        public ReadOnlyMemory<byte> ReadOnlyMemory => owner._buffer.AsMemory(offset, length);
        public Span<byte> Span => owner._buffer.AsSpan(offset, length);
    }

    private readonly int _maxSize = maxSize;
    private byte[] _buffer = [];

    public int Size { get; private set; }

    public Slice Access(UInt256 offset, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        return Access(offset, (UInt256) length);
    }

    public Slice Access(UInt256 offset, UInt256 length)
    {
        if(length == UInt256.Zero)
        {
            return new Slice(this, 0, 0);
        }

        Expand(offset, length);
        return new Slice(this, (int) offset, (int) length);
    }

    public void Copy(UInt256 destinationOffset, UInt256 sourceOffset, UInt256 length)
    {
        if(length == UInt256.Zero)
        {
            return;
        }

        Expand(
            destinationOffset > sourceOffset
                ? destinationOffset
                : sourceOffset,
            length
        );

        _buffer.AsSpan((int) sourceOffset, (int) length).CopyTo(
            _buffer.AsSpan((int) destinationOffset, (int) length)
        );
    }

    private void Expand(UInt256 offset, UInt256 length)
    {
        if(offset > (UInt256) _maxSize
            || length > (UInt256) _maxSize
            || (int) offset > _maxSize - (int) length)
        {
            throw new MemoryLimitExceededException(offset, length, _maxSize);
        }

        int end = (int) offset + (int) length;
        if(end <= Size)
        {
            return;
        }

        int requiredSize = (((end - 1) / Bytes32.BYTE_LENGTH) + 1) * Bytes32.BYTE_LENGTH;
        if(requiredSize > _buffer.Length)
        {
            int newCapacity = Math.Max(Bytes32.BYTE_LENGTH, _buffer.Length);
            while(newCapacity < requiredSize)
            {
                if(newCapacity > _maxSize / 2)
                {
                    newCapacity = _maxSize;
                    break;
                }

                newCapacity *= 2;
            }

            Array.Resize(ref _buffer, newCapacity);
        }

        Size = requiredSize;
    }
}
