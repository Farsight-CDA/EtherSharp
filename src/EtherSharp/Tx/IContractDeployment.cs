using EtherSharp.Types;
using System.Numerics;

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
    public static IContractDeployment Create(EVMByteCode byteCode, BigInteger value)
        => new ContractDeployment(
            byteCode,
            value
        );
}
