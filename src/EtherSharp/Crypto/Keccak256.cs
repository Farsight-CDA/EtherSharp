using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace EtherSharp.Crypto;

/// <summary>
/// Efficient implementation of the Keccak-256 hash algorithm.
/// </summary>
public ref struct Keccak256
{
    private const int OUTPUT_LENGTH_BYTES = 32;
    private const int RATE = 1088;
    private const int RATE_BYTES = RATE / 8;
    private const int RATE_64 = RATE >> 6;
    private const int MAX_STACKALLOC_BYTES = 1024;

    private static ReadOnlySpan<ulong> KeccakRoundConstants =>
    [
        0x0000000000000001UL, 0x0000000000008082UL, 0x800000000000808aUL, 0x8000000080008000UL,
        0x000000000000808bUL, 0x0000000080000001UL, 0x8000000080008081UL, 0x8000000000008009UL,
        0x000000000000008aUL, 0x0000000000000088UL, 0x0000000080008009UL, 0x000000008000000aUL,
        0x000000008000808bUL, 0x800000000000008bUL, 0x8000000000008089UL, 0x8000000000008003UL,
        0x8000000000008002UL, 0x8000000000000080UL, 0x000000000000800aUL, 0x800000008000000aUL,
        0x8000000080008081UL, 0x8000000000008080UL, 0x0000000080000001UL, 0x8000000080008008UL
    ];

    private int _bitsInQueue;
    private readonly Span<byte> _dataQueue;
    private readonly Span<ulong> _state;

    internal Keccak256(Span<byte> dataQueue, Span<ulong> state)
    {
        _dataQueue = dataQueue;
        _state = state;
    }

    /// <summary>
    /// Hashes the input data and writes the resulting hash into the destination buffer.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public static bool TryHashData(ReadOnlySpan<byte> data, Span<byte> destination)
    {
        if(destination.Length != OUTPUT_LENGTH_BYTES)
        {
            return false;
        }

        if(data.Length < RATE_BYTES)
        {
            HashSingleBlock(data, destination);
            return true;
        }

        Span<byte> dataQueue = stackalloc byte[RATE_BYTES];
        Span<ulong> state = stackalloc ulong[25];

        var keccak = new Keccak256(dataQueue, state);

        MemoryMarshal.Cast<byte, ulong>(data[..RATE_BYTES]).CopyTo(state);
        keccak.KeccakPermutation();
        keccak.BlockUpdate(data[RATE_BYTES..]);
        keccak.DoFinal(destination);
        return true;
    }

    private static void HashSingleBlock(ReadOnlySpan<byte> data, Span<byte> destination)
    {
        Span<ulong> state = stackalloc ulong[25];
        state.Clear();

        var stateBytes = MemoryMarshal.AsBytes(state);
        data.CopyTo(stateBytes);
        stateBytes[data.Length] = 1;
        state[RATE_64 - 1] |= 1UL << 63;

        var keccak = new Keccak256([], state);
        keccak.KeccakPermutation();

        state[..4].CopyTo(MemoryMarshal.Cast<byte, ulong>(destination));
    }

    /// <summary>
    /// Hashes a 32-byte value and writes the resulting hash into the destination buffer.
    /// </summary>
    /// <param name="data">The 32-byte value to hash.</param>
    /// <param name="destination">The destination buffer for the hash.</param>
    /// <returns><see langword="true"/> when <paramref name="destination"/> is exactly 32 bytes long; otherwise, <see langword="false"/>.</returns>
    public static bool TryHashData(in Bytes32 data, Span<byte> destination)
        => TryHashData(data.DangerousGetReadOnlySpan(), destination);

    /// <summary>
    /// Hashes the input data and writes the hash into a newly allocated array.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static Bytes32 HashData(ReadOnlySpan<byte> data)
    {
        Span<byte> outputBuffer = stackalloc byte[OUTPUT_LENGTH_BYTES];
        return !TryHashData(data, outputBuffer)
            ? throw new NotSupportedException()
            : Bytes32.FromBytes(outputBuffer);
    }

    /// <summary>
    /// Hashes a 32-byte value.
    /// </summary>
    /// <param name="data">The 32-byte value to hash.</param>
    /// <returns>The resulting Keccak-256 hash.</returns>
    public static Bytes32 HashData(in Bytes32 data)
        => HashData(data.DangerousGetReadOnlySpan());

    /// <summary>
    /// Hashes the UTF-8 representation of a string.
    /// </summary>
    /// <param name="data">String to hash as UTF-8.</param>
    /// <returns>The resulting Keccak-256 hash.</returns>
    public static Bytes32 HashData(string data)
    {
        ArgumentNullException.ThrowIfNull(data);

        int byteCount = Encoding.UTF8.GetByteCount(data);
        byte[]? rented = null;
        var utf8Bytes = byteCount <= MAX_STACKALLOC_BYTES
            ? stackalloc byte[byteCount]
            : (rented = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            int bytesWritten = Encoding.UTF8.GetBytes(data, utf8Bytes);
            return HashData(utf8Bytes[..bytesWritten]);
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    private void BlockUpdate(ReadOnlySpan<byte> input)
        => Absorb(input);

    private void DoFinal(Span<byte> output)
        => Squeeze(output);

    private void Absorb(byte data)
    {
        if((_bitsInQueue & 7) != 0)
        {
            throw new InvalidOperationException("attempt to absorb with odd length queue");
        }

        _dataQueue[_bitsInQueue >> 3] = data;
        if((_bitsInQueue += 8) == RATE)
        {
            KeccakAbsorb(_dataQueue);
            _bitsInQueue = 0;
        }
    }

    private void Absorb(ReadOnlySpan<byte> data)
    {
        if((_bitsInQueue & 7) != 0)
        {
            throw new InvalidOperationException("attempt to absorb with odd length queue");
        }

        int bytesInQueue = _bitsInQueue / 8;
        int rateBytes = RATE_BYTES;

        int available = rateBytes - bytesInQueue;

        if(data.Length < available)
        {
            data.CopyTo(_dataQueue[bytesInQueue..]);
            _bitsInQueue += data.Length * 8;
            return;
        }

        int count = 0;
        if(bytesInQueue > 0)
        {
            data[..available].CopyTo(_dataQueue[bytesInQueue..]);
            count += available;
            KeccakAbsorb(_dataQueue);
        }

        int remaining;
        while((remaining = data.Length - count) >= rateBytes)
        {
            KeccakAbsorb(data[count..]);
            count += rateBytes;
        }

        data.Slice(count, remaining).CopyTo(_dataQueue);
        _bitsInQueue = remaining << 3;
    }

    private void AbsorbBits(int data, int bits)
    {
        if(bits < 1 || bits > 7)
        {
            throw new ArgumentException("must be in the range 1 to 7", nameof(bits));
        }

        if((_bitsInQueue & 7) != 0)
        {
            throw new InvalidOperationException("attempt to absorb with odd length queue");
        }

        int mask = (1 << bits) - 1;
        _dataQueue[_bitsInQueue >> 3] = (byte) (data & mask);

        // NOTE: After this, bitsInQueue is no longer a multiple of 8, so no more absorbs will work
        _bitsInQueue += bits;
    }

    private void PadAndSwitchToSqueezingPhase()
    {
        Debug.Assert(_bitsInQueue < RATE);

        _dataQueue[_bitsInQueue >> 3] |= (byte) (1U << (_bitsInQueue & 7));
        _bitsInQueue++;

        if(_bitsInQueue == RATE)
        {
            KeccakAbsorb(_dataQueue);
            _bitsInQueue = 0;
        }

        int full = _bitsInQueue >> 6;
        int partial = _bitsInQueue & 63;
        for(int i = 0; i < full; i++)
        {
            _state[i] ^= BinaryPrimitives.ReadUInt64LittleEndian(_dataQueue[(i * 8)..]);
        }

        if(partial > 0)
        {
            ulong mask = (1UL << partial) - 1UL;
            _state[full] ^= BinaryPrimitives.ReadUInt64LittleEndian(_dataQueue[(full * 8)..]) & mask;
        }

        _state[(RATE - 1) >> 6] ^= 1UL << 63;

        _bitsInQueue = 0;
    }

    private void Squeeze(Span<byte> output)
    {
        Debug.Assert(output.Length == OUTPUT_LENGTH_BYTES);
        PadAndSwitchToSqueezingPhase();
        KeccakPermutation();

        _state[..4].CopyTo(MemoryMarshal.Cast<byte, ulong>(output));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private readonly void KeccakAbsorb(ReadOnlySpan<byte> data)
    {
        for(int i = 0; i < RATE_64; ++i)
        {
            _state[i] ^= BinaryPrimitives.ReadUInt64LittleEndian(data[(i * 8)..]);
        }

        KeccakPermutation();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private readonly void KeccakPermutation()
    {
        ref ulong state = ref MemoryMarshal.GetReference(_state);
        ref ulong roundConstants = ref MemoryMarshal.GetReference(KeccakRoundConstants);

        ulong a00 = Unsafe.Add(ref state, 0);
        ulong a01 = Unsafe.Add(ref state, 1);
        ulong a02 = Unsafe.Add(ref state, 2);
        ulong a03 = Unsafe.Add(ref state, 3);
        ulong a04 = Unsafe.Add(ref state, 4);
        ulong a05 = Unsafe.Add(ref state, 5);
        ulong a06 = Unsafe.Add(ref state, 6);
        ulong a07 = Unsafe.Add(ref state, 7);
        ulong a08 = Unsafe.Add(ref state, 8);
        ulong a09 = Unsafe.Add(ref state, 9);
        ulong a10 = Unsafe.Add(ref state, 10);
        ulong a11 = Unsafe.Add(ref state, 11);
        ulong a12 = Unsafe.Add(ref state, 12);
        ulong a13 = Unsafe.Add(ref state, 13);
        ulong a14 = Unsafe.Add(ref state, 14);
        ulong a15 = Unsafe.Add(ref state, 15);
        ulong a16 = Unsafe.Add(ref state, 16);
        ulong a17 = Unsafe.Add(ref state, 17);
        ulong a18 = Unsafe.Add(ref state, 18);
        ulong a19 = Unsafe.Add(ref state, 19);
        ulong a20 = Unsafe.Add(ref state, 20);
        ulong a21 = Unsafe.Add(ref state, 21);
        ulong a22 = Unsafe.Add(ref state, 22);
        ulong a23 = Unsafe.Add(ref state, 23);
        ulong a24 = Unsafe.Add(ref state, 24);
        ulong e00, e01, e02, e03, e04;
        ulong e05, e06, e07, e08, e09;
        ulong e10, e11, e12, e13, e14;
        ulong e15, e16, e17, e18, e19;
        ulong e20, e21, e22, e23, e24;
        ulong d0, d1, d2, d3, d4, b0, b1, b2, b3, b4;

        // Column parities of the input state; each round carries the parities of its output forward.
        ulong c0 = a00 ^ a05 ^ a10 ^ a15 ^ a20;
        ulong c1 = a01 ^ a06 ^ a11 ^ a16 ^ a21;
        ulong c2 = a02 ^ a07 ^ a12 ^ a17 ^ a22;
        ulong c3 = a03 ^ a08 ^ a13 ^ a18 ^ a23;
        ulong c4 = a04 ^ a09 ^ a14 ^ a19 ^ a24;

        // Two rounds per iteration (A -> E -> A). Each output row is computed in one pass:
        // theta correction, rho rotation and pi gather on the five source lanes, then chi and iota.
        for(nint round = 0; round < 24; round += 2)
        {
            d0 = c4 ^ BitOperations.RotateLeft(c1, 1);
            d1 = c0 ^ BitOperations.RotateLeft(c2, 1);
            d2 = c1 ^ BitOperations.RotateLeft(c3, 1);
            d3 = c2 ^ BitOperations.RotateLeft(c4, 1);
            d4 = c3 ^ BitOperations.RotateLeft(c0, 1);
            b0 = a00 ^ d0;
            b1 = BitOperations.RotateLeft(a06 ^ d1, 44);
            b2 = BitOperations.RotateLeft(a12 ^ d2, 43);
            b3 = BitOperations.RotateLeft(a18 ^ d3, 21);
            b4 = BitOperations.RotateLeft(a24 ^ d4, 14);
            e00 = b0 ^ (~b1 & b2) ^ Unsafe.Add(ref roundConstants, round);
            c0 = e00;
            e01 = b1 ^ (~b2 & b3);
            c1 = e01;
            e02 = b2 ^ (~b3 & b4);
            c2 = e02;
            e03 = b3 ^ (~b4 & b0);
            c3 = e03;
            e04 = b4 ^ (~b0 & b1);
            c4 = e04;
            b0 = BitOperations.RotateLeft(a03 ^ d3, 28);
            b1 = BitOperations.RotateLeft(a09 ^ d4, 20);
            b2 = BitOperations.RotateLeft(a10 ^ d0, 3);
            b3 = BitOperations.RotateLeft(a16 ^ d1, 45);
            b4 = BitOperations.RotateLeft(a22 ^ d2, 61);
            e05 = b0 ^ (~b1 & b2);
            c0 ^= e05;
            e06 = b1 ^ (~b2 & b3);
            c1 ^= e06;
            e07 = b2 ^ (~b3 & b4);
            c2 ^= e07;
            e08 = b3 ^ (~b4 & b0);
            c3 ^= e08;
            e09 = b4 ^ (~b0 & b1);
            c4 ^= e09;
            b0 = BitOperations.RotateLeft(a01 ^ d1, 1);
            b1 = BitOperations.RotateLeft(a07 ^ d2, 6);
            b2 = BitOperations.RotateLeft(a13 ^ d3, 25);
            b3 = BitOperations.RotateLeft(a19 ^ d4, 8);
            b4 = BitOperations.RotateLeft(a20 ^ d0, 18);
            e10 = b0 ^ (~b1 & b2);
            c0 ^= e10;
            e11 = b1 ^ (~b2 & b3);
            c1 ^= e11;
            e12 = b2 ^ (~b3 & b4);
            c2 ^= e12;
            e13 = b3 ^ (~b4 & b0);
            c3 ^= e13;
            e14 = b4 ^ (~b0 & b1);
            c4 ^= e14;
            b0 = BitOperations.RotateLeft(a04 ^ d4, 27);
            b1 = BitOperations.RotateLeft(a05 ^ d0, 36);
            b2 = BitOperations.RotateLeft(a11 ^ d1, 10);
            b3 = BitOperations.RotateLeft(a17 ^ d2, 15);
            b4 = BitOperations.RotateLeft(a23 ^ d3, 56);
            e15 = b0 ^ (~b1 & b2);
            c0 ^= e15;
            e16 = b1 ^ (~b2 & b3);
            c1 ^= e16;
            e17 = b2 ^ (~b3 & b4);
            c2 ^= e17;
            e18 = b3 ^ (~b4 & b0);
            c3 ^= e18;
            e19 = b4 ^ (~b0 & b1);
            c4 ^= e19;
            b0 = BitOperations.RotateLeft(a02 ^ d2, 62);
            b1 = BitOperations.RotateLeft(a08 ^ d3, 55);
            b2 = BitOperations.RotateLeft(a14 ^ d4, 39);
            b3 = BitOperations.RotateLeft(a15 ^ d0, 41);
            b4 = BitOperations.RotateLeft(a21 ^ d1, 2);
            e20 = b0 ^ (~b1 & b2);
            c0 ^= e20;
            e21 = b1 ^ (~b2 & b3);
            c1 ^= e21;
            e22 = b2 ^ (~b3 & b4);
            c2 ^= e22;
            e23 = b3 ^ (~b4 & b0);
            c3 ^= e23;
            e24 = b4 ^ (~b0 & b1);
            c4 ^= e24;

            d0 = c4 ^ BitOperations.RotateLeft(c1, 1);
            d1 = c0 ^ BitOperations.RotateLeft(c2, 1);
            d2 = c1 ^ BitOperations.RotateLeft(c3, 1);
            d3 = c2 ^ BitOperations.RotateLeft(c4, 1);
            d4 = c3 ^ BitOperations.RotateLeft(c0, 1);
            b0 = e00 ^ d0;
            b1 = BitOperations.RotateLeft(e06 ^ d1, 44);
            b2 = BitOperations.RotateLeft(e12 ^ d2, 43);
            b3 = BitOperations.RotateLeft(e18 ^ d3, 21);
            b4 = BitOperations.RotateLeft(e24 ^ d4, 14);
            a00 = b0 ^ (~b1 & b2) ^ Unsafe.Add(ref roundConstants, round + 1);
            c0 = a00;
            a01 = b1 ^ (~b2 & b3);
            c1 = a01;
            a02 = b2 ^ (~b3 & b4);
            c2 = a02;
            a03 = b3 ^ (~b4 & b0);
            c3 = a03;
            a04 = b4 ^ (~b0 & b1);
            c4 = a04;
            b0 = BitOperations.RotateLeft(e03 ^ d3, 28);
            b1 = BitOperations.RotateLeft(e09 ^ d4, 20);
            b2 = BitOperations.RotateLeft(e10 ^ d0, 3);
            b3 = BitOperations.RotateLeft(e16 ^ d1, 45);
            b4 = BitOperations.RotateLeft(e22 ^ d2, 61);
            a05 = b0 ^ (~b1 & b2);
            c0 ^= a05;
            a06 = b1 ^ (~b2 & b3);
            c1 ^= a06;
            a07 = b2 ^ (~b3 & b4);
            c2 ^= a07;
            a08 = b3 ^ (~b4 & b0);
            c3 ^= a08;
            a09 = b4 ^ (~b0 & b1);
            c4 ^= a09;
            b0 = BitOperations.RotateLeft(e01 ^ d1, 1);
            b1 = BitOperations.RotateLeft(e07 ^ d2, 6);
            b2 = BitOperations.RotateLeft(e13 ^ d3, 25);
            b3 = BitOperations.RotateLeft(e19 ^ d4, 8);
            b4 = BitOperations.RotateLeft(e20 ^ d0, 18);
            a10 = b0 ^ (~b1 & b2);
            c0 ^= a10;
            a11 = b1 ^ (~b2 & b3);
            c1 ^= a11;
            a12 = b2 ^ (~b3 & b4);
            c2 ^= a12;
            a13 = b3 ^ (~b4 & b0);
            c3 ^= a13;
            a14 = b4 ^ (~b0 & b1);
            c4 ^= a14;
            b0 = BitOperations.RotateLeft(e04 ^ d4, 27);
            b1 = BitOperations.RotateLeft(e05 ^ d0, 36);
            b2 = BitOperations.RotateLeft(e11 ^ d1, 10);
            b3 = BitOperations.RotateLeft(e17 ^ d2, 15);
            b4 = BitOperations.RotateLeft(e23 ^ d3, 56);
            a15 = b0 ^ (~b1 & b2);
            c0 ^= a15;
            a16 = b1 ^ (~b2 & b3);
            c1 ^= a16;
            a17 = b2 ^ (~b3 & b4);
            c2 ^= a17;
            a18 = b3 ^ (~b4 & b0);
            c3 ^= a18;
            a19 = b4 ^ (~b0 & b1);
            c4 ^= a19;
            b0 = BitOperations.RotateLeft(e02 ^ d2, 62);
            b1 = BitOperations.RotateLeft(e08 ^ d3, 55);
            b2 = BitOperations.RotateLeft(e14 ^ d4, 39);
            b3 = BitOperations.RotateLeft(e15 ^ d0, 41);
            b4 = BitOperations.RotateLeft(e21 ^ d1, 2);
            a20 = b0 ^ (~b1 & b2);
            c0 ^= a20;
            a21 = b1 ^ (~b2 & b3);
            c1 ^= a21;
            a22 = b2 ^ (~b3 & b4);
            c2 ^= a22;
            a23 = b3 ^ (~b4 & b0);
            c3 ^= a23;
            a24 = b4 ^ (~b0 & b1);
            c4 ^= a24;
        }

        Unsafe.Add(ref state, 0) = a00;
        Unsafe.Add(ref state, 1) = a01;
        Unsafe.Add(ref state, 2) = a02;
        Unsafe.Add(ref state, 3) = a03;
        Unsafe.Add(ref state, 4) = a04;
        Unsafe.Add(ref state, 5) = a05;
        Unsafe.Add(ref state, 6) = a06;
        Unsafe.Add(ref state, 7) = a07;
        Unsafe.Add(ref state, 8) = a08;
        Unsafe.Add(ref state, 9) = a09;
        Unsafe.Add(ref state, 10) = a10;
        Unsafe.Add(ref state, 11) = a11;
        Unsafe.Add(ref state, 12) = a12;
        Unsafe.Add(ref state, 13) = a13;
        Unsafe.Add(ref state, 14) = a14;
        Unsafe.Add(ref state, 15) = a15;
        Unsafe.Add(ref state, 16) = a16;
        Unsafe.Add(ref state, 17) = a17;
        Unsafe.Add(ref state, 18) = a18;
        Unsafe.Add(ref state, 19) = a19;
        Unsafe.Add(ref state, 20) = a20;
        Unsafe.Add(ref state, 21) = a21;
        Unsafe.Add(ref state, 22) = a22;
        Unsafe.Add(ref state, 23) = a23;
        Unsafe.Add(ref state, 24) = a24;
    }
}
