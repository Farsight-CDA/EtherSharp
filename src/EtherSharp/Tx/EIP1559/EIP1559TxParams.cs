using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Defines additional transaction parameters for EIP-1559 transactions.
/// </summary>
/// <param name="AccessList">Optional access list used to predeclare state slots touched by the transaction.</param>
public sealed record EIP1559TxParams(
    StateAccess[] AccessList
) : ITxParams<EIP1559TxParams>
{
    private const int LENGTH_PREFIX_SIZE = sizeof(int);

    /// <inheritdoc/>
    public static EIP1559TxParams Default { get; } = new EIP1559TxParams([]);

    /// <inheritdoc/>
    static EIP1559TxParams ITxParams<EIP1559TxParams>.Decode(ReadOnlySpan<byte> data)
    {
        if(data.IsEmpty)
        {
            return Default;
        }

        int offset = 0;
        EnsureBytesAvailable(data, offset, LENGTH_PREFIX_SIZE);
        int accessListLength = BinaryPrimitives.ReadInt32BigEndian(data.Slice(offset, LENGTH_PREFIX_SIZE));
        offset += LENGTH_PREFIX_SIZE;

        if(accessListLength < 0)
        {
            throw new InvalidDataException("Access list length cannot be negative.");
        }

        var accessList = new StateAccess[accessListLength];

        for(int i = 0; i < accessList.Length; i++)
        {
            EnsureBytesAvailable(data, offset, Address.BYTES_LENGTH);
            var address = Address.FromBytes(data.Slice(offset, Address.BYTES_LENGTH));
            offset += Address.BYTES_LENGTH;

            EnsureBytesAvailable(data, offset, LENGTH_PREFIX_SIZE);
            int storageKeyCount = BinaryPrimitives.ReadInt32BigEndian(data.Slice(offset, LENGTH_PREFIX_SIZE));
            offset += LENGTH_PREFIX_SIZE;

            if(storageKeyCount < 0)
            {
                throw new InvalidDataException("Storage key count cannot be negative.");
            }

            byte[][] storageKeys = new byte[storageKeyCount][];

            for(int j = 0; j < storageKeys.Length; j++)
            {
                EnsureBytesAvailable(data, offset, LENGTH_PREFIX_SIZE);
                int storageKeyLength = BinaryPrimitives.ReadInt32BigEndian(data.Slice(offset, LENGTH_PREFIX_SIZE));
                offset += LENGTH_PREFIX_SIZE;

                if(storageKeyLength < 0)
                {
                    throw new InvalidDataException("Storage key length cannot be negative.");
                }

                EnsureBytesAvailable(data, offset, storageKeyLength);

                storageKeys[j] = data.Slice(offset, storageKeyLength).ToArray();
                offset += storageKeyLength;
            }

            accessList[i] = new StateAccess(address, storageKeys);
        }

        return offset == data.Length
            ? new EIP1559TxParams(accessList)
            : throw new InvalidDataException("Unexpected trailing bytes while decoding EIP-1559 tx params.");
    }

    /// <inheritdoc/>
    byte[] ITxParams<EIP1559TxParams>.Encode()
    {
        int size = LENGTH_PREFIX_SIZE;

        foreach(var access in AccessList)
        {
            size = size + Address.BYTES_LENGTH + LENGTH_PREFIX_SIZE;

            foreach(byte[] storageKey in access.StorageKeys)
            {
                size = size + LENGTH_PREFIX_SIZE + storageKey.Length;
            }
        }

        byte[] buffer = new byte[size];
        var span = buffer.AsSpan();
        int offset = 0;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset, LENGTH_PREFIX_SIZE), AccessList.Length);
        offset += LENGTH_PREFIX_SIZE;

        foreach(var access in AccessList)
        {
            access.Address.CopyTo(span[offset..]);
            offset += Address.BYTES_LENGTH;

            BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset, LENGTH_PREFIX_SIZE), access.StorageKeys.Length);
            offset += LENGTH_PREFIX_SIZE;

            foreach(byte[] storageKey in access.StorageKeys)
            {
                BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset, LENGTH_PREFIX_SIZE), storageKey.Length);
                offset += LENGTH_PREFIX_SIZE;
                storageKey.CopyTo(span[offset..]);
                offset += storageKey.Length;
            }
        }

        return buffer;
    }

    private static void EnsureBytesAvailable(ReadOnlySpan<byte> data, int offset, int requiredLength)
    {
        if(requiredLength < 0 || data.Length - offset < requiredLength)
        {
            throw new InvalidDataException("Unexpected end of data while decoding EIP-1559 tx params.");
        }
    }
}
