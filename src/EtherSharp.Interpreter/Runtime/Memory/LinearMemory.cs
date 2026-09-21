using EtherSharp.Interpreter.Runtime.Tracing;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Memory;

internal sealed class LinearMemory(int maxSize, GasBudget gas, ulong gasPerWord, ulong quadraticDivisor) : IInterpreterMemory
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

    ReadOnlyMemory<byte> IInterpreterMemory.Slice(int offset, int length)
        => _buffer.AsMemory(0, Size).Slice(offset, length);

    public bool TryAccess(UInt256 offset, int length, out Slice slice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        return TryAccess(offset, (UInt256) length, out slice);
    }

    public bool TryAccess(UInt256 offset, UInt256 length, out Slice slice)
    {
        if(length.IsZero)
        {
            slice = new Slice(this, 0, 0);
            return true;
        }

        if(!TryExpand(offset, length))
        {
            slice = default;
            return false;
        }
        slice = new Slice(this, (int) offset, (int) length);
        return true;
    }

    public bool TryCopy(UInt256 destinationOffset, UInt256 sourceOffset, UInt256 length)
    {
        if(length.IsZero)
        {
            return true;
        }

        if(!TryExpand(
            destinationOffset > sourceOffset
                ? destinationOffset
                : sourceOffset,
            length
        ))
        {
            return false;
        }

        _buffer.AsSpan((int) sourceOffset, (int) length).CopyTo(
            _buffer.AsSpan((int) destinationOffset, (int) length)
        );
        return true;
    }

    private bool TryExpand(UInt256 offset, UInt256 length)
    {
        if(offset > (UInt256) _maxSize
            || length > (UInt256) _maxSize
            || (int) offset > _maxSize - (int) length)
        {
            // Overflow at these magnitudes implies an unaffordable quadratic cost,
            // even with the largest supported divisor.
            if(UInt256.Add(offset, length, out var endOffset)
                || UInt256.Add(endOffset, (UInt256) (Bytes32.BYTE_LENGTH - 1), out var roundedEnd))
            {
                return false;
            }

            UInt256.Divide(roundedEnd, (UInt256) Bytes32.BYTE_LENGTH, out var words);
            if(UInt256.MultiplyOverflow(words, (UInt256) gasPerWord, out var linearCost)
                || UInt256.MultiplyOverflow(words, words, out var squaredWords))
            {
                return false;
            }

            UInt256.Divide(squaredWords, (UInt256) quadraticDivisor, out var quadraticCost);
            return UInt256.Add(linearCost, quadraticCost, out var cost)
                || cost - (UInt256) GetCost(Size) > (UInt256) gas.Remaining
                ? false
                : throw new MemoryLimitExceededException(offset, length, _maxSize);
        }

        int end = (int) offset + (int) length;
        if(end <= Size)
        {
            return true;
        }

        int requiredSize = (((end - 1) / Bytes32.BYTE_LENGTH) + 1) * Bytes32.BYTE_LENGTH;
        var expansionCost = GetCost(requiredSize) - GetCost(Size);
        if(expansionCost > UInt64.MaxValue || !gas.TryCharge((ulong) expansionCost))
        {
            return false;
        }

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
        return true;
    }

    private UInt128 GetCost(int size)
    {
        var words = (UInt128) (size / Bytes32.BYTE_LENGTH);
        return (words * gasPerWord) + (words * words / quadraticDivisor);
    }
}
