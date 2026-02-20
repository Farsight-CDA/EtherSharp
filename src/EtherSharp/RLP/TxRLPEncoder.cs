using EtherSharp.Tx;

namespace EtherSharp.RLP;

internal static class TxRLPEncoder
{
    public static int GetAccessListLength(ReadOnlySpan<StateAccess> accessList)
    {
        int totalSize = 0;

        foreach(var access in accessList)
        {
            totalSize += RLPEncoder.GetListSize(
                RLPEncoder.GetStringSize(access.Address) +
                RLPEncoder.GetListSize(GetStorageKeysLength(access))
            );
        }

        return totalSize;
    }

    private static int GetStorageKeysLength(StateAccess access)
    {
        int storageKeySize = 0;

        foreach(byte[] storageKey in access.StorageKeys)
        {
            storageKeySize += RLPEncoder.GetStringSize(storageKey);
        }

        return storageKeySize;
    }

    public static RLPEncoder EncodeAccessList(this RLPEncoder encoder, ReadOnlySpan<StateAccess> accessList)
    {
        foreach(var access in accessList)
        {
            encoder = encoder.EncodeString(access.Address)
                .EncodeList(GetStorageKeysLength(access));

            foreach(byte[] storageKey in access.StorageKeys)
            {
                encoder = encoder.EncodeString(storageKey);
            }
        }

        return encoder;
    }

    public static int MaxEncodedSignatureLength => 33 + 33 + 1;

    public static RLPEncoder EncodeSignature(this RLPEncoder encoder, ReadOnlySpan<byte> signature, out int signatureLength)
    {
        byte parityByte = signature[64] switch
        {
            0 => 0,
            1 => 1,
            27 => 0,
            28 => 1,
            _ => throw new NotSupportedException("Bad parity byte")
        };

        var r = signature[..32];
        var s = signature[32..64];

        int rLeadingZeroBytes = r.IndexOfAnyExcept((byte) 0);
        int sLeadingZeroBytes = s.IndexOfAnyExcept((byte) 0);

        if(rLeadingZeroBytes > 0)
        {
            r = r[rLeadingZeroBytes..];
        }
        if(sLeadingZeroBytes > 0)
        {
            s = s[sLeadingZeroBytes..];
        }

        signatureLength = RLPEncoder.GetStringSize(r) + RLPEncoder.GetStringSize(s) + 1;
        return encoder
            .EncodeInt(parityByte)
            .EncodeString(r)
            .EncodeString(s);
    }
}
