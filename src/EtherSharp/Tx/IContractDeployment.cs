using EtherSharp.Contract;
using EtherSharp.Numerics;

namespace EtherSharp.Tx;
/// <summary>
/// Represents a contract deployment transaction payload.
/// </summary>
public interface IContractDeployment : ITxInput<byte[]>, IFlashCode
{
    /// <summary>
    /// Gets the contract creation bytecode.
    /// </summary>
    public EVMByteCode ByteCode { get; }

    bool IFlashCode.TryGetRuntimeCode(out EVMByteCode runtimeCode)
    {
        runtimeCode = default;
        return false;
    }

    int IFlashCode.GetInitCodeLength()
        => ByteCode.Length;

    EVMByteCode IFlashCode.GetInitCode()
        => ByteCode;

    /// <summary>
    /// Creates an IContractDeployment transaction payload.
    /// </summary>
    /// <param name="byteCode"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IContractDeployment Create(EVMByteCode byteCode, UInt256 value)
        => new ContractDeployment(
            byteCode,
            value
        );
}
