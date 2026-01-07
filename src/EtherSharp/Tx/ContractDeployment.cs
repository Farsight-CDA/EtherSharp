using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Tx;

internal class ContractDeployment(EVMByteCode byteCode, UInt256 value) : IContractDeployment
{
    public EVMByteCode ByteCode { get; } = byteCode;
    public Address? To => null;
    public UInt256 Value { get; } = value;
    public ReadOnlyMemory<byte> Data => ByteCode.ByteCode;

    public byte[] ReadResultFrom(ReadOnlyMemory<byte> data)
        => data.ToArray();
}
