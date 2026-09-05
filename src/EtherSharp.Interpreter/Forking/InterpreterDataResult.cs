using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Forking;

/// <summary>
/// Represents one resolved interpreter data request.
/// </summary>
public abstract record InterpreterDataResult
{
    private InterpreterDataResult()
    {
    }

    /// <summary>
    /// Contains a resolved account balance.
    /// </summary>
    /// <param name="Address">The account address.</param>
    /// <param name="Value">The account balance.</param>
    public sealed record Balance(
        Address Address,
        UInt256 Value
    ) : InterpreterDataResult;

    /// <summary>
    /// Contains a resolved account nonce.
    /// </summary>
    /// <param name="Address">The account address.</param>
    /// <param name="Value">The account nonce.</param>
    public sealed record Nonce(
        Address Address,
        ulong Value
    ) : InterpreterDataResult;

    /// <summary>
    /// Contains resolved account bytecode.
    /// </summary>
    /// <param name="Address">The account address.</param>
    /// <param name="Value">The account bytecode.</param>
    public sealed record Code(
        Address Address,
        EVMByteCode Value
    ) : InterpreterDataResult;

    /// <summary>
    /// Contains a resolved upstream external code hash.
    /// </summary>
    /// <param name="Address">The account address.</param>
    /// <param name="Value">
    /// The external code hash, or zero when the account does not exist or is empty according to EIP-161.
    /// </param>
    public sealed record CodeHash(
        Address Address,
        Bytes32 Value
    ) : InterpreterDataResult;

    /// <summary>
    /// Contains a resolved persistent storage value.
    /// </summary>
    /// <param name="Address">The account address.</param>
    /// <param name="Key">The storage key.</param>
    /// <param name="Value">The storage value.</param>
    public sealed record Storage(
        Address Address,
        Bytes32 Key,
        Bytes32 Value
    ) : InterpreterDataResult;

    /// <summary>
    /// Contains an upstream call result.
    /// </summary>
    /// <param name="Caller">The immediate message caller.</param>
    /// <param name="Target">The called account.</param>
    /// <param name="Value">The native value supplied to the call.</param>
    /// <param name="Input">The call input.</param>
    /// <param name="Result">The call result.</param>
    public sealed record Call(
        Address Caller,
        Address Target,
        UInt256 Value,
        ReadOnlyMemory<byte> Input,
        TxCallResult Result
    ) : InterpreterDataResult;
}
