using EtherSharp.Contract;
using EtherSharp.Numerics;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Tx;
/// <summary>
/// Represents a contract deployment transaction payload.
/// </summary>
public interface IContractDeployment : ITxInput<byte[]>, IFlashCode
{
    /// <summary>
    /// Gets the contract creation bytecode.
    /// </summary>
    public EVMInitCode ByteCode { get; }

    bool IFlashCode.TryGetRuntimeCode([MaybeNullWhen(false)] out EVMRuntimeCode runtimeCode)
    {
        runtimeCode = default;
        return false;
    }

    int IFlashCode.GetInitCodeLength()
        => ByteCode.Length;

    EVMInitCode IFlashCode.GetInitCode()
        => ByteCode;

    /// <summary>
    /// Creates an IContractDeployment transaction payload.
    /// </summary>
    /// <param name="byteCode"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IContractDeployment Create(EVMInitCode byteCode, UInt256 value)
        => new ContractDeployment(
            byteCode,
            value
        );
}
