
using EVM.net.ABI;
using System.Numerics;

public partial class AbiEncoder
{
    public AbiEncoder Int24(int value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateInts(value, 24));
        return this;
    }

    public AbiEncoder UInt24(uint value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateUInts(value, 24));
        return this;
    }

    public AbiEncoder Int32(int value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateInts(value, 32));
        return this;
    }

    public AbiEncoder UInt32(uint value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateUInts(value, 32));
        return this;
    }

    public AbiEncoder Int40(long value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateLong(value, 40));
        return this;
    }

    public AbiEncoder UInt40(ulong value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateULong(value, 40));
        return this;
    }

    public AbiEncoder Int48(long value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateLong(value, 48));
        return this;
    }

    public AbiEncoder UInt48(ulong value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateULong(value, 48));
        return this;
    }

    public AbiEncoder Int56(long value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateLong(value, 56));
        return this;
    }

    public AbiEncoder UInt56(ulong value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateULong(value, 56));
        return this;
    }

    public AbiEncoder Int64(long value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateLong(value, 64));
        return this;
    }

    public AbiEncoder UInt64(ulong value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateULong(value, 64));
        return this;
    }

    public AbiEncoder Int72(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 72));
        return this;
    }

    public AbiEncoder UInt72(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 72));
        return this;
    }
    public AbiEncoder Int80(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 80));
        return this;
    }

    public AbiEncoder UInt80(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 80));
        return this;
    }
    public AbiEncoder Int88(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 88));
        return this;
    }

    public AbiEncoder UInt88(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 88));
        return this;
    }
    public AbiEncoder Int96(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 96));
        return this;
    }

    public AbiEncoder UInt96(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 96));
        return this;
    }
    public AbiEncoder Int104(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 104));
        return this;
    }

    public AbiEncoder UInt104(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 104));
        return this;
    }
    public AbiEncoder Int112(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 112));
        return this;
    }

    public AbiEncoder UInt112(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 112));
        return this;
    }
    public AbiEncoder Int120(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 120));
        return this;
    }

    public AbiEncoder UInt120(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 120));
        return this;
    }
    public AbiEncoder Int128(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 128));
        return this;
    }

    public AbiEncoder UInt128(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 128));
        return this;
    }
    public AbiEncoder Int136(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 136));
        return this;
    }

    public AbiEncoder UInt136(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 136));
        return this;
    }
    public AbiEncoder Int144(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 144));
        return this;
    }

    public AbiEncoder UInt144(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 144));
        return this;
    }
    public AbiEncoder Int152(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 152));
        return this;
    }

    public AbiEncoder UInt152(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 152));
        return this;
    }
    public AbiEncoder Int160(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 160));
        return this;
    }

    public AbiEncoder UInt160(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 160));
        return this;
    }
    public AbiEncoder Int168(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 168));
        return this;
    }

    public AbiEncoder UInt168(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 168));
        return this;
    }
    public AbiEncoder Int176(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 176));
        return this;
    }

    public AbiEncoder UInt176(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 176));
        return this;
    }
    public AbiEncoder Int184(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 184));
        return this;
    }

    public AbiEncoder UInt184(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 184));
        return this;
    }
    public AbiEncoder Int192(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 192));
        return this;
    }

    public AbiEncoder UInt192(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 192));
        return this;
    }
    public AbiEncoder Int200(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 200));
        return this;
    }

    public AbiEncoder UInt200(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 200));
        return this;
    }
    public AbiEncoder Int208(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 208));
        return this;
    }

    public AbiEncoder UInt208(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 208));
        return this;
    }
    public AbiEncoder Int216(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 216));
        return this;
    }

    public AbiEncoder UInt216(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 216));
        return this;
    }
    public AbiEncoder Int224(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 224));
        return this;
    }

    public AbiEncoder UInt224(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 224));
        return this;
    }
    public AbiEncoder Int232(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 232));
        return this;
    }

    public AbiEncoder UInt232(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 232));
        return this;
    }
    public AbiEncoder Int240(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 240));
        return this;
    }

    public AbiEncoder UInt240(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 240));
        return this;
    }
    public AbiEncoder Int248(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 248));
        return this;
    }

    public AbiEncoder UInt248(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 248));
        return this;
    }
    public AbiEncoder Int256(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, false, 256));
        return this;
    }

    public AbiEncoder UInt256(BigInteger value)
    { 
        AddElement(new FixedEncodeType<string>.GenerateBigInteger(value, true, 256));
        return this;
    }
}