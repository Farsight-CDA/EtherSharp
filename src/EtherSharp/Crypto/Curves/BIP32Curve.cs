﻿using System.Numerics;

namespace EtherSharp.Crypto.Curves;
public abstract partial class BIP32Curve
{
    protected abstract string Name { get; }
    protected abstract BigInteger N { get; }
    protected abstract ReadOnlySpan<byte> NBytes { get; }

    protected abstract void SerializedPoint(Span<byte> point, Span<byte> destination);

    public (byte[], byte[]) DerivePath(ReadOnlySpan<byte> seed, params ReadOnlySpan<uint> path)
    {
        byte[] keyBuffer = new byte[32];
        byte[] chainCodeBuffer = new byte[32];
        DerivePath(seed, keyBuffer, chainCodeBuffer, path);
        return (keyBuffer, chainCodeBuffer);
    }

    public (byte[], byte[]) DerivePath(ReadOnlySpan<byte> seed, string path)
    {
        byte[] keyBuffer = new byte[32];
        byte[] chainCodeBuffer = new byte[32];
        DerivePath(seed, keyBuffer, chainCodeBuffer, path);
        return (keyBuffer, chainCodeBuffer);
    }

    public void DerivePath(ReadOnlySpan<byte> seed,
        Span<byte> keyDestination, Span<byte> chainCodeDestination,
        params ReadOnlySpan<uint> path)
    {
        GetMasterKeyFromSeed(seed, keyDestination, chainCodeDestination);

        foreach(uint derivStep in path)
        {
            GetChildKeyDerivation(keyDestination, chainCodeDestination, derivStep);
        }
    }

    public void DerivePath(ReadOnlySpan<byte> seed,
        Span<byte> keyDestination, Span<byte> chainCodeDestination,
        string path)
    {
        if(path.Length < 2 || path[0] != 'm' || path[1] != '/')
        {
            throw new ArgumentException("Invalid derivation path", nameof(path));
        }

        var valuePath = path[2..].AsSpan();
        int pathLength = valuePath.Count('/') + 1;

        if(pathLength == 0)
        {
            throw new ArgumentException("Invalid derivation path", nameof(path));
        }

        Span<uint> pathBuffer = stackalloc uint[pathLength];

        int pathIndex = 0;
        foreach(var range in valuePath.Split('/'))
        {
            var segment = valuePath[range];
            bool isHardened = segment[^1] == '\'' || segment[^1] == 'h';

            if(!uint.TryParse(isHardened ? segment[..^1] : segment, out uint derivStep))
            {
                throw new ArgumentException($"Invalid derivation path. Failed to parse at index {pathIndex}", nameof(path));
            }
            if(derivStep >= _hardenedOffset)
            {
                throw new ArgumentException($"Invalid derivation path. Path to large at index {pathIndex}", nameof(path));
            }

            if(isHardened)
            {
                derivStep = derivStep += _hardenedOffset;
            }

            pathBuffer[pathIndex] = derivStep;
            pathIndex++;
        }

        DerivePath(seed, keyDestination, chainCodeDestination, pathBuffer);
    }

    private bool IsValidKey(ReadOnlySpan<byte> key)
    {
        if(key.Length != NBytes.Length)
        {
            return false;
        }
        if(key.IndexOfAnyExcept((byte) 0) == -1)
        {
            return false;
        }

        for(int i = 0; i < key.Length; i++)
        {
            if(NBytes[i] > key[i])
            {
                return true;
            }
            else if(key[i] > NBytes[i])
            {
                return false;
            }
        }

        return false;
    }
}
