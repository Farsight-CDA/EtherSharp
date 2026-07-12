using EtherSharp.Tx;

namespace EtherSharp.RLP;

internal static class TxRLPEncoder
{
    public static int AddressStringSize { get; } = RLPEncoder.GetEncodedStringLength(Types.Address.BYTES_LENGTH);

    public static int GetAddressStringSize(Types.Address? address)
        => address is null ? RLPEncoder.GetStringSize([]) : AddressStringSize;

    public static int GetAccessListLength(ReadOnlySpan<StateAccess> accessList)
    {
        int totalSize = 0;

        foreach(var access in accessList)
        {
            totalSize += RLPEncoder.GetListSize(
                AddressStringSize +
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
            encoder = encoder
                .EncodeList(AddressStringSize + RLPEncoder.GetListSize(GetStorageKeysLength(access)))
                .EncodeAddress(access.Address)
                .EncodeList(GetStorageKeysLength(access));

            foreach(byte[] storageKey in access.StorageKeys)
            {
                encoder = encoder.EncodeString(storageKey);
            }
        }

        return encoder;
    }

    public static RLPEncoder EncodeAddress(this RLPEncoder encoder, Types.Address address)
        => encoder.EncodeString(address.DangerousGetReadOnlySpan());

    public static int MaxEncodedSignatureLength => 33 + 33 + 1;

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
