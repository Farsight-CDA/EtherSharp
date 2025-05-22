using EtherSharp.ABI.Types.Base;
using System.Globalization;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class Address : FixedType<ReadOnlyMemory<char>>, IPackedEncodeType
    {
        public int PackedSize => 20;

        public Address(string value)
            : base(value.StartsWith("0x", false, CultureInfo.InvariantCulture) ? value.AsMemory()[2..] : value.AsMemory())
        {
            if(Value.Length != 40)
            {
                throw new ArgumentException("Bad address length");
            }
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer, true);

        public static void EncodeInto(ReadOnlyMemory<char> value, Span<byte> buffer, bool isPacked)
        {
            if(!isPacked)
            {
                buffer = buffer[12..];
            }

            var status = Convert.FromHexString(value.Span, buffer, out _, out _);

            if(status != System.Buffers.OperationStatus.Done)
            {
                throw new InvalidOperationException($"Failed to encode address: {status}");
            }
        }

        public static EtherSharp.Types.Address Decode(ReadOnlySpan<byte> bytes)
            => EtherSharp.Types.Address.FromBytes(bytes[12..]);
    }
}