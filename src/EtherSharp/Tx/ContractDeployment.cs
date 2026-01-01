using EtherSharp.ABI;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;

internal class ContractDeployment(EVMByteCode byteCode, BigInteger value) : IContractDeployment
{
    public EVMByteCode ByteCode { get; } = byteCode;
    public Address To => Address.Zero;
    public BigInteger Value { get; } = value;
    public ReadOnlySpan<byte> Data => ByteCode.ByteCode.Span;

    public byte[] ReadResultFrom(ReadOnlyMemory<byte> data)
    {
        var decoder = new AbiDecoder(data);
        var byteSpan = decoder.Bytes();
        return byteSpan.ToArray();
    }
}
