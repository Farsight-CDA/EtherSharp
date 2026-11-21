using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Tx;

internal sealed class ContractDeployment(EVMInitCode byteCode, UInt256 value) : IContractDeployment
{
    public EVMInitCode ByteCode { get; } = byteCode;
    public Address? To => null;
    public UInt256 Value { get; } = value;
    public ReadOnlyMemory<byte> Data => ByteCode.ByteCode;

    public byte[] ReadResultFrom(ReadOnlyMemory<byte> data)
        => data.ToArray();
}
