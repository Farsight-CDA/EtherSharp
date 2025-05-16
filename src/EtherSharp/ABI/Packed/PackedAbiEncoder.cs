using EtherSharp.ABI.Types;
using System.Numerics;

namespace EtherSharp.ABI.Packed;
public class PackedAbiEncoder
{
    private readonly List<IPackedEncodeType> _entries = [];

    public int Size { get; private set; }

    private PackedAbiEncoder AddElement(IPackedEncodeType type)
    {
        Size += type.PackedSize;
        _entries.Add(type);
        return this;
    }

    public PackedAbiEncoder Number<TNumber>(TNumber number, bool isUnsigned, int bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }
        //
        return AddElement(bitLength switch
        {
            8 => isUnsigned
                ? new AbiTypes.Byte(
                    number is byte us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"))
                : new AbiTypes.SByte(
                    number is sbyte s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")),
            16 => isUnsigned
                ? new AbiTypes.UShort(
                    number is ushort us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"))
                : new AbiTypes.Short(
                    number is short s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")),
            > 16 and <= 32 => isUnsigned
                ? new AbiTypes.UInt(
                    number is uint us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"), bitLength / 8)
                : new AbiTypes.Int(
                    number is int s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"), bitLength / 8),
            > 32 and <= 64 => isUnsigned
                ? new AbiTypes.ULong(
                    number is ulong us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"), bitLength / 8)
                : new AbiTypes.Long(
                    number is long s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"), bitLength / 8),
            > 64 and <= 256 => new AbiTypes.BigInteger(
                number is BigInteger s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}"),
                isUnsigned, bitLength / 8),
            _ => throw new NotImplementedException()
        });
    }
    public PackedAbiEncoder Bool(bool value)
        => AddElement(new AbiTypes.Bool(value));
    public PackedAbiEncoder Address(string value)
        => AddElement(new AbiTypes.Address(value));

    public PackedAbiEncoder String(string value)
        => AddElement(new AbiTypes.String(value));
    public PackedAbiEncoder Bytes(byte[] arr)
        => AddElement(new AbiTypes.Bytes(arr));

    public bool TryWritoTo(Span<byte> outputBuffer)
    {
        if(outputBuffer.Length < Size)
        {
            throw new ArgumentException("Output buffer too small", nameof(outputBuffer));
        }

        int offset = 0;

        foreach(var entry in _entries)
        {
            entry.EncodePacked(
                outputBuffer.Slice(
                    offset,
                    entry.PackedSize
                )
            );
            offset += entry.PackedSize;
        }

        return true;
    }

    public byte[] Build()
    {
        byte[] buffer = new byte[Size];
        _ = TryWritoTo(buffer);
        return buffer;
    }
}
