using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.Tx.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EtherSharp.Tx;
internal static class TxEncoder
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
}
