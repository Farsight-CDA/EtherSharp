namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class Address : FixedType<ReadOnlyMemory<char>>
    {
        public Address(string value)
            : base(value.StartsWith("0x") ? value.AsMemory()[2..] : value.AsMemory())
        {
            if(Value.Length != 40)
            {
                throw new ArgumentException("Bad address length");
            }
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public static void EncodeInto(ReadOnlyMemory<char> value, Span<byte> buffer)
        {
            for(int i = 0; i < value.Length; i += 2)
            {
                int highNibble = GetHexValue(value.Span[i]);
                int lowNibble = GetHexValue(value.Span[i + 1]);

                if(highNibble == -1 || lowNibble == -1)
                {
                    throw new InvalidOperationException("Not a hex string");
                }

                buffer[12 + (i / 2)] = (byte) ((highNibble << 4) | lowNibble);
            }
        }

        public static string Decode(ReadOnlySpan<byte> bytes)
            => $"0x{Convert.ToHexString(bytes[12..])}";

        private static int GetHexValue(char hex)
        {
            if(hex >= '0' && hex <= '9')
            {
                return hex - '0';
            }
            else if(hex >= 'a' && hex <= 'f')
            {
                return hex - 'a' + 10;
            }
            else if(hex >= 'A' && hex <= 'F')
            {
                return hex - 'A' + 10;
            }

            return -1;
        }
    }
}