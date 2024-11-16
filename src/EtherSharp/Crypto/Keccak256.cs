using System;
using System.Diagnostics;

namespace EtherSharp.Crypto;
public ref struct Keccak256
{
    private static readonly ulong[] _keccakRoundConstants = [
        0x0000000000000001UL, 0x0000000000008082UL, 0x800000000000808aUL, 0x8000000080008000UL,
        0x000000000000808bUL, 0x0000000080000001UL, 0x8000000080008081UL, 0x8000000000008009UL,
        0x000000000000008aUL, 0x0000000000000088UL, 0x0000000080008009UL, 0x000000008000000aUL,
        0x000000008000808bUL, 0x800000000000008bUL, 0x8000000000008089UL, 0x8000000000008003UL,
        0x8000000000008002UL, 0x8000000000000080UL, 0x000000000000800aUL, 0x800000008000000aUL,
        0x8000000080008081UL, 0x8000000000008080UL, 0x0000000080000001UL, 0x8000000080008008UL
    ];
    private const int _outputLengthBytes = 32;
    private const int _outputLengthBits = _outputLengthBytes << 3;
    private const int _rate = 1088;

    private int _bitsInQueue;
    private readonly Span<byte> _dataQueue;
    private readonly Span<ulong> _state;

    public Keccak256(Span<byte> dataQueue, Span<ulong> state)
    {
        _dataQueue = dataQueue;
        _state = state;
    }

    public static bool TryHashData(ReadOnlySpan<byte> data, Span<byte> outputBuffer)
    {
        if(outputBuffer.Length != _outputLengthBytes)
        {
            return false;
        }

        Span<byte> dataQueue = stackalloc byte[192];
        Span<ulong> state = stackalloc ulong[25];

        var keccak = new Keccak256(dataQueue, state);

        keccak.BlockUpdate(data);
        keccak.DoFinal(outputBuffer);
        return true;
    }

    public static byte[] HashData(ReadOnlySpan<byte> data)
    {
        byte[] outputBuffer = new byte[_outputLengthBytes];
        return !TryHashData(data, outputBuffer)
            ? throw new NotSupportedException()
            : outputBuffer;
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
        if((_bitsInQueue += 8) == _rate)
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
        int rateBytes = _rate / 8;

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
        Debug.Assert(_bitsInQueue < _rate);

        _dataQueue[_bitsInQueue >> 3] |= (byte) (1U << (_bitsInQueue & 7));
        _bitsInQueue++;

        if(_bitsInQueue == _rate)
        {
            KeccakAbsorb(_dataQueue);
            _bitsInQueue = 0;
        }

        int full = _bitsInQueue >> 6;
        int partial = _bitsInQueue & 63;

        for(int i = 0; i < full; i++)
        {
            _state[i] ^= BitConverter.IsLittleEndian
                ? BitConverter.ToUInt64(_dataQueue[(i * 8)..])
                : Pack.LE_To_UInt64(_dataQueue[(i * 8)..]);
        }

        if(partial > 0)
        {
            ulong mask = (1UL << partial) - 1UL;
            _state[full] ^= BitConverter.IsLittleEndian 
                ? BitConverter.ToUInt64(_dataQueue[(full * 8)..]) & mask
                : Pack.LE_To_UInt64(_dataQueue[(full * 8)..]) & mask;
        }

        _state[(_rate - 1) >> 6] ^= 1UL << 63;

        _bitsInQueue = 0;
    }

    private void Squeeze(Span<byte> output)
    {
        PadAndSwitchToSqueezingPhase();

        int i = 0;
        while(i < output.Length)
        {
            if(_bitsInQueue == 0)
            {
                KeccakExtract();
            }

            int partialBlock = Math.Min(_bitsInQueue, _outputLengthBits - i);

            _dataQueue.Slice((_rate - _bitsInQueue) >> 3, partialBlock >> 3).CopyTo(output[(i >> 3)..]);
            _bitsInQueue -= partialBlock;
            i += partialBlock;
        }
    }

    private readonly void KeccakAbsorb(ReadOnlySpan<byte> data)
    {
        int count = _rate >> 6;
        for(int i = 0; i < count; ++i)
        {
            _state[i] ^= BitConverter.IsLittleEndian
                ? BitConverter.ToUInt64(data[(i * 8)..])
                : Pack.LE_To_UInt64(data[(i * 8)..]);
        }

        KeccakPermutation();
    }

    private void KeccakExtract()
    {
        KeccakPermutation();

        if(BitConverter.IsLittleEndian)
        {
            for(int i = 0; i < _rate >> 6; ++i)
            {
                BitConverter.TryWriteBytes(_dataQueue[(8 * i)..], _state[i]);
            }
        }
        else
        {
            for(int i = 0; i < _rate >> 6; ++i)
            {
                Pack.UInt64_To_LE(_state, _rate >> 6, _dataQueue);
            }
        }

        _bitsInQueue = _rate;
    }

    private readonly void KeccakPermutation()
    {
        ulong a00 = _state[0], a01 = _state[1], a02 = _state[2], a03 = _state[3], a04 = _state[4];
        ulong a05 = _state[5], a06 = _state[6], a07 = _state[7], a08 = _state[8], a09 = _state[9];
        ulong a10 = _state[10], a11 = _state[11], a12 = _state[12], a13 = _state[13], a14 = _state[14];
        ulong a15 = _state[15], a16 = _state[16], a17 = _state[17], a18 = _state[18], a19 = _state[19];
        ulong a20 = _state[20], a21 = _state[21], a22 = _state[22], a23 = _state[23], a24 = _state[24];

        for(int i = 0; i < 24; i++)
        {
            // theta
            ulong c0 = a00 ^ a05 ^ a10 ^ a15 ^ a20;
            ulong c1 = a01 ^ a06 ^ a11 ^ a16 ^ a21;
            ulong c2 = a02 ^ a07 ^ a12 ^ a17 ^ a22;
            ulong c3 = a03 ^ a08 ^ a13 ^ a18 ^ a23;
            ulong c4 = a04 ^ a09 ^ a14 ^ a19 ^ a24;

            ulong d1 = ((c1 << 1) | (c1 >> -1)) ^ c4;
            ulong d2 = ((c2 << 1) | (c2 >> -1)) ^ c0;
            ulong d3 = ((c3 << 1) | (c3 >> -1)) ^ c1;
            ulong d4 = ((c4 << 1) | (c4 >> -1)) ^ c2;
            ulong d0 = ((c0 << 1) | (c0 >> -1)) ^ c3;

            a00 ^= d1;
            a05 ^= d1;
            a10 ^= d1;
            a15 ^= d1;
            a20 ^= d1;
            a01 ^= d2;
            a06 ^= d2;
            a11 ^= d2;
            a16 ^= d2;
            a21 ^= d2;
            a02 ^= d3;
            a07 ^= d3;
            a12 ^= d3;
            a17 ^= d3;
            a22 ^= d3;
            a03 ^= d4;
            a08 ^= d4;
            a13 ^= d4;
            a18 ^= d4;
            a23 ^= d4;
            a04 ^= d0;
            a09 ^= d0;
            a14 ^= d0;
            a19 ^= d0;
            a24 ^= d0;

            // rho/pi
            c1 = (a01 << 1) | (a01 >> 63);
            a01 = (a06 << 44) | (a06 >> 20);
            a06 = (a09 << 20) | (a09 >> 44);
            a09 = (a22 << 61) | (a22 >> 3);
            a22 = (a14 << 39) | (a14 >> 25);
            a14 = (a20 << 18) | (a20 >> 46);
            a20 = (a02 << 62) | (a02 >> 2);
            a02 = (a12 << 43) | (a12 >> 21);
            a12 = (a13 << 25) | (a13 >> 39);
            a13 = (a19 << 8) | (a19 >> 56);
            a19 = (a23 << 56) | (a23 >> 8);
            a23 = (a15 << 41) | (a15 >> 23);
            a15 = (a04 << 27) | (a04 >> 37);
            a04 = (a24 << 14) | (a24 >> 50);
            a24 = (a21 << 2) | (a21 >> 62);
            a21 = (a08 << 55) | (a08 >> 9);
            a08 = (a16 << 45) | (a16 >> 19);
            a16 = (a05 << 36) | (a05 >> 28);
            a05 = (a03 << 28) | (a03 >> 36);
            a03 = (a18 << 21) | (a18 >> 43);
            a18 = (a17 << 15) | (a17 >> 49);
            a17 = (a11 << 10) | (a11 >> 54);
            a11 = (a07 << 6) | (a07 >> 58);
            a07 = (a10 << 3) | (a10 >> 61);
            a10 = c1;

            // chi
            c0 = a00 ^ (~a01 & a02);
            c1 = a01 ^ (~a02 & a03);
            a02 ^= ~a03 & a04;
            a03 ^= ~a04 & a00;
            a04 ^= ~a00 & a01;
            a00 = c0;
            a01 = c1;

            c0 = a05 ^ (~a06 & a07);
            c1 = a06 ^ (~a07 & a08);
            a07 ^= ~a08 & a09;
            a08 ^= ~a09 & a05;
            a09 ^= ~a05 & a06;
            a05 = c0;
            a06 = c1;

            c0 = a10 ^ (~a11 & a12);
            c1 = a11 ^ (~a12 & a13);
            a12 ^= ~a13 & a14;
            a13 ^= ~a14 & a10;
            a14 ^= ~a10 & a11;
            a10 = c0;
            a11 = c1;

            c0 = a15 ^ (~a16 & a17);
            c1 = a16 ^ (~a17 & a18);
            a17 ^= ~a18 & a19;
            a18 ^= ~a19 & a15;
            a19 ^= ~a15 & a16;
            a15 = c0;
            a16 = c1;

            c0 = a20 ^ (~a21 & a22);
            c1 = a21 ^ (~a22 & a23);
            a22 ^= ~a23 & a24;
            a23 ^= ~a24 & a20;
            a24 ^= ~a20 & a21;
            a20 = c0;
            a21 = c1;

            // iota
            a00 ^= _keccakRoundConstants[i];
        }

        _state[0] = a00;
        _state[1] = a01;
        _state[2] = a02;
        _state[3] = a03;
        _state[4] = a04;
        _state[5] = a05;
        _state[6] = a06;
        _state[7] = a07;
        _state[8] = a08;
        _state[9] = a09;
        _state[10] = a10;
        _state[11] = a11;
        _state[12] = a12;
        _state[13] = a13;
        _state[14] = a14;
        _state[15] = a15;
        _state[16] = a16;
        _state[17] = a17;
        _state[18] = a18;
        _state[19] = a19;
        _state[20] = a20;
        _state[21] = a21;
        _state[22] = a22;
        _state[23] = a23;
        _state[24] = a24;
    }
}
