using EtherSharp.Tx;

namespace EtherSharp.RLP;

internal static class TxRLPEncoder
{
    public static int GetAddressStringSize(Types.Address? address)
        => address is null
            ? RLPEncoder.GetStringSize([])
            : RLPEncoder.ADDRESS_STRING_SIZE;

    public static int GetAccessListLength(ReadOnlySpan<StateAccess> accessList)
    {
        int totalSize = 0;

        foreach(var access in accessList)
        {
            totalSize += RLPEncoder.GetListSize(
                RLPEncoder.ADDRESS_STRING_SIZE +
                RLPEncoder.GetListSize(GetStorageKeysLength(access))
            );
        }

        return totalSize;
    }

    private static int GetStorageKeysLength(StateAccess access)
        => access.StorageKeys.Length * RLPEncoder.BYTES32_STRING_SIZE;

    public static RLPEncoder EncodeAccessList(this RLPEncoder encoder, ReadOnlySpan<StateAccess> accessList)
    {
        foreach(var access in accessList)
        {
            int storageKeysLength = GetStorageKeysLength(access);

            encoder = encoder
                .EncodeList(RLPEncoder.ADDRESS_STRING_SIZE + RLPEncoder.GetListSize(storageKeysLength))
                .EncodeAddress(access.Address)
                .EncodeList(storageKeysLength);

            foreach(ref readonly var storageKey in access.StorageKeys.Span)
            {
                encoder = encoder.EncodeBytes32(in storageKey);
            }
        }

        return encoder;
    }

    private const int MAX_ENCODED_SIGNATURE_COMPONENT_LENGTH = 1 + 32;

    public static int GetMaxEncodedSignatureLength(ulong maxV)
        => RLPEncoder.GetIntSize(maxV) + (2 * MAX_ENCODED_SIGNATURE_COMPONENT_LENGTH);

    public static RLPEncoder EncodeSignature(
        this RLPEncoder encoder, ReadOnlySpan<byte> signature, ulong v, out int signatureLength)
    {
        var r = signature[..32].TrimStart((byte) 0);
        var s = signature[32..64].TrimStart((byte) 0);

        signatureLength = RLPEncoder.GetIntSize(v) + RLPEncoder.GetStringSize(r) + RLPEncoder.GetStringSize(s);
        return encoder
            .EncodeInt(v)
            .EncodeString(r)
            .EncodeString(s);
    }
}
