using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class Address(EtherSharp.Types.Address value) : FixedType<EtherSharp.Types.Address>(value), IPackedEncodeType
    {
        public int PackedSize => 20;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer, true);

        public static void EncodeInto(EtherSharp.Types.Address value, Span<byte> buffer, bool isPacked)
        {
            if(!isPacked)
            {
                buffer = buffer[12..];
            }

            value.Bytes.CopyTo(buffer);
        }

        public static EtherSharp.Types.Address Decode(ReadOnlySpan<byte> bytes)
            => EtherSharp.Types.Address.FromBytes(bytes[12..]);
    }
}