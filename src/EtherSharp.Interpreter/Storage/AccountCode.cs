using EtherSharp.Contract;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Storage;

/// <summary>
/// Represents an account's bytecode and state code hash.
/// </summary>
/// <param name="Code">The account bytecode.</param>
/// <param name="Hash">The state code hash, or zero when the account does not exist.</param>
public readonly record struct AccountCode(
    EVMByteCode Code,
    Bytes32 Hash
);
