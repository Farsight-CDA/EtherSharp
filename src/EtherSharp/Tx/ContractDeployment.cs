using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Tx;

internal sealed class ContractDeployment(EVMInitCode initCode, UInt256 value) : ITxInput<EVMRuntimeCode>
{
    public Address? To => null;
    public UInt256 Value { get; } = value;
    public ReadOnlyMemory<byte> Data { get; } = initCode.ByteCode;

    public EVMRuntimeCode ReadResultFrom(ReadOnlyMemory<byte> data)
        => new EVMRuntimeCode(data);
}
