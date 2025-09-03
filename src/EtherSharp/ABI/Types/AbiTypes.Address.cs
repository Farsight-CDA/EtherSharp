﻿using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;
public static partial class AbiTypes
{
    public class Address : FixedType<EtherSharp.Types.Address>, IPackedEncodeType
    {
        /// <inheritdoc/>
        public int PackedSize => 20;

        internal Address(EtherSharp.Types.Address value) : base(value) { }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
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