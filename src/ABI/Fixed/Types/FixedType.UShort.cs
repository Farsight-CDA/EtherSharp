﻿namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class UShort(ushort value) : FixedType<ushort>(value)
    {
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(ushort value, Span<byte> buffer)
        {
            if(!BitConverter.TryWriteBytes(buffer[(32 - 2)..], value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 2)..].Reverse();
            }
        }
    }
}
