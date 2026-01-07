using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Tx;
/// <summary>
/// Represents a contract deployment transaction payload.
/// </summary>
public interface IContractDeployment : ITxInput<byte[]>
{
    /// <summary>
    /// The ByteCode of the contract to deploy.
    /// </summary>
    public EVMByteCode ByteCode { get; }

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
