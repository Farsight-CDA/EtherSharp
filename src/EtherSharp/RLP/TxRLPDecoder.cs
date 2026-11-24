using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.RLP;

internal static class TxRLPDecoder
{
    public static bool TryReadAddress(this ref RLPDecoder decoder, out Address address)
    {
        if(!decoder.TryReadString(Address.BYTES_LENGTH, out var bytes))
        {
            address = default;
            return false;
        }

        address = Address.FromBytes(bytes);
        return true;
    }

    public static bool TryReadOptionalAddress(this ref RLPDecoder decoder, out Address? address)
    {
        var candidate = decoder;
        if(!candidate.TryReadString(out var bytes) || bytes.Length is not (0 or Address.BYTES_LENGTH))
        {
            address = null;
            return false;
        }

        // An empty string encodes a missing address, e.g. the recipient of a contract creation.
        address = null;
        if(!bytes.IsEmpty)
        {
            address = Address.FromBytes(bytes);
        }

        decoder = candidate;
        return true;
    }

    public static bool TryReadAccessList(this ref RLPDecoder decoder, out StateAccess[] accessList)
    {
        accessList = [];

        var candidate = decoder;
        if(!candidate.TryReadList(out var entries) || !TryCountItems(entries, out int count))
        {
            return false;
        }

        var result = new StateAccess[count];
        for(int i = 0; i < result.Length; i++)
        {
            if(!entries.TryReadList(out var entry)
                || !entry.TryReadAddress(out var address)
                || !entry.TryReadStorageKeys(out var storageKeys)
                || !entry.IsComplete)
            {
                return false;
            }

            result[i] = new StateAccess(in address, storageKeys);
        }

        accessList = result;
        decoder = candidate;
        return true;
    }

    private static bool TryReadStorageKeys(this ref RLPDecoder decoder, out Bytes32[] storageKeys)
    {
        storageKeys = [];

        var candidate = decoder;
        if(!candidate.TryReadList(out var keys) || !TryCountItems(keys, out int count))
        {
            return false;
        }

        var result = new Bytes32[count];
        for(int i = 0; i < result.Length; i++)
        {
            if(!keys.TryReadString(Bytes32.BYTE_LENGTH, out var key))
            {
                return false;
            }

            result[i] = Bytes32.FromBytes(key);
        }

        storageKeys = result;
        decoder = candidate;
        return true;
    }

    private static bool TryCountItems(RLPDecoder decoder, out int count)
    {
        count = 0;
        while(decoder.TryReadRaw(out _, out _))
        {
            count++;
        }

        return decoder.IsComplete;
    }

    public static bool TryReadSignature(this ref RLPDecoder decoder, out ulong v, out Bytes32 r, out Bytes32 s)
    {
        var candidate = decoder;
        if(!candidate.TryReadInt(out v)
            || !candidate.TryReadInt(out UInt256 rValue)
            || !candidate.TryReadInt(out UInt256 sValue))
        {
            v = 0;
            r = default;
            s = default;
            return false;
        }

        r = (Bytes32) rValue;
        s = (Bytes32) sValue;
        decoder = candidate;
        return true;
    }
}
