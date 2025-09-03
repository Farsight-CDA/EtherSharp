﻿using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
public static partial class AbiTypes
{
    public class Short : FixedType<short>, IPackedEncodeType
    {
        /// <inheritdoc/>
        public int PackedSize => 2;

        internal Short(short value) : base(value) { }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(short value, Span<byte> buffer)
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer[(buffer.Length - 2)..], value);

            if(value < 0)
            {
                buffer[..(buffer.Length - 2)].Fill(byte.MaxValue);
            }
        }

        public static short Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadInt16BigEndian(bytes[(32 - 2)..]);
    }
}
