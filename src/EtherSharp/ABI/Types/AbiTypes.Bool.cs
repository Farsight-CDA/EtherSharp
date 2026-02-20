using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class Bool : FixedType<bool>, IPackedEncodeType
    {
        /// <inheritdoc/>
        public int PackedSize => 1;

        internal Bool(bool value) : base(value) { }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(bool value, Span<byte> buffer)
            => buffer[^1] = value ? (byte) 1 : (byte) 0;
        public static bool Decode(ReadOnlySpan<byte> bytes)
            => bytes[^1] == 1;
    }
}
