using EtherSharp.ABI.Fixed;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    public AbiDecoder Int24(int value)
    { 
        AddElement(new FixedType<string>.Int(value, 24));
        return this;
    }

    public AbiDecoder UInt24(uint value)
    { 
        AddElement(new FixedType<string>.UInt(value, 24));
        return this;
    }

    public AbiDecoder Int32(int value)
    { 
        AddElement(new FixedType<string>.Int(value, 32));
        return this;
    }

    public AbiDecoder UInt32(uint value)
    { 
        AddElement(new FixedType<string>.UInt(value, 32));
        return this;
    }

    public AbiDecoder Int40(long value)
    { 
        AddElement(new FixedType<string>.Long(value, 40));
        return this;
    }

    public AbiDecoder UInt40(ulong value)
    { 
        AddElement(new FixedType<string>.ULong(value, 40));
        return this;
    }

    public AbiDecoder Int48(long value)
    { 
        AddElement(new FixedType<string>.Long(value, 48));
        return this;
    }

    public AbiDecoder UInt48(ulong value)
    { 
        AddElement(new FixedType<string>.ULong(value, 48));
        return this;
    }

    public AbiDecoder Int56(long value)
    { 
        AddElement(new FixedType<string>.Long(value, 56));
        return this;
    }

    public AbiDecoder UInt56(ulong value)
    { 
        AddElement(new FixedType<string>.ULong(value, 56));
        return this;
    }

    public AbiDecoder Int64(long value)
    { 
        AddElement(new FixedType<string>.Long(value, 64));
        return this;
    }

    public AbiDecoder UInt64(ulong value)
    { 
        AddElement(new FixedType<string>.ULong(value, 64));
        return this;
    }

    public AbiDecoder Int72(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 72));
        return this;
    }

    public AbiDecoder UInt72(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 72));
        return this;
    }
    public AbiDecoder Int80(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 80));
        return this;
    }

    public AbiDecoder UInt80(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 80));
        return this;
    }
    public AbiDecoder Int88(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 88));
        return this;
    }

    public AbiDecoder UInt88(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 88));
        return this;
    }
    public AbiDecoder Int96(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 96));
        return this;
    }

    public AbiDecoder UInt96(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 96));
        return this;
    }
    public AbiDecoder Int104(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 104));
        return this;
    }

    public AbiDecoder UInt104(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 104));
        return this;
    }
    public AbiDecoder Int112(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 112));
        return this;
    }

    public AbiDecoder UInt112(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 112));
        return this;
    }
    public AbiDecoder Int120(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 120));
        return this;
    }

    public AbiDecoder UInt120(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 120));
        return this;
    }
    public AbiDecoder Int128(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 128));
        return this;
    }

    public AbiDecoder UInt128(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 128));
        return this;
    }
    public AbiDecoder Int136(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 136));
        return this;
    }

    public AbiDecoder UInt136(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 136));
        return this;
    }
    public AbiDecoder Int144(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 144));
        return this;
    }

    public AbiDecoder UInt144(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 144));
        return this;
    }
    public AbiDecoder Int152(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 152));
        return this;
    }

    public AbiDecoder UInt152(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 152));
        return this;
    }
    public AbiDecoder Int160(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 160));
        return this;
    }

    public AbiDecoder UInt160(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 160));
        return this;
    }
    public AbiDecoder Int168(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 168));
        return this;
    }

    public AbiDecoder UInt168(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 168));
        return this;
    }
    public AbiDecoder Int176(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 176));
        return this;
    }

    public AbiDecoder UInt176(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 176));
        return this;
    }
    public AbiDecoder Int184(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 184));
        return this;
    }

    public AbiDecoder UInt184(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 184));
        return this;
    }
    public AbiDecoder Int192(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 192));
        return this;
    }

    public AbiDecoder UInt192(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 192));
        return this;
    }
    public AbiDecoder Int200(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 200));
        return this;
    }

    public AbiDecoder UInt200(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 200));
        return this;
    }
    public AbiDecoder Int208(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 208));
        return this;
    }

    public AbiDecoder UInt208(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 208));
        return this;
    }
    public AbiDecoder Int216(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 216));
        return this;
    }

    public AbiDecoder UInt216(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 216));
        return this;
    }
    public AbiDecoder Int224(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 224));
        return this;
    }

    public AbiDecoder UInt224(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 224));
        return this;
    }
    public AbiDecoder Int232(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 232));
        return this;
    }

    public AbiDecoder UInt232(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 232));
        return this;
    }
    public AbiDecoder Int240(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 240));
        return this;
    }

    public AbiDecoder UInt240(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 240));
        return this;
    }
    public AbiDecoder Int248(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 248));
        return this;
    }

    public AbiDecoder UInt248(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 248));
        return this;
    }
    public AbiDecoder Int256(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, false, 256));
        return this;
    }

    public AbiDecoder UInt256(BigInteger value)
    { 
        AddElement(new FixedType<string>.BigInteger(value, true, 256));
        return this;
    }
}