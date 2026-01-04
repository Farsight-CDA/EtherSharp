using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;

internal class ContractDeployment(EVMByteCode byteCode, BigInteger value) : IContractDeployment
{
    public EVMByteCode ByteCode { get; } = byteCode;
    public Address? To => null;
    public BigInteger Value { get; } = value;
    public ReadOnlyMemory<byte> Data => ByteCode.ByteCode;

    public byte[] ReadResultFrom(ReadOnlyMemory<byte> data)
        => data.ToArray();
}
