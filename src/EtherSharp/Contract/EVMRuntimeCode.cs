namespace EtherSharp.Contract;

/// <summary>
/// Represents runtime code, the code stored at and executed by an account.
/// </summary>
/// <remarks>
/// Code read from an account is not required to be deployable. For example, EIP-7702 delegation designators start with <c>0xEF</c>.
/// </remarks>
/// <param name="byteCode">The bytecode bytes. The backing memory must not be modified while the bytecode is in use.</param>
public sealed class EVMRuntimeCode(ReadOnlyMemory<byte> byteCode) : EVMByteCode(byteCode)
{
    /// <summary>
    /// The maximum length of runtime code deployed by contract creation.
    /// </summary>
    public const int MAX_LENGTH = 24_576;

    /// <summary>
    /// Gets empty runtime code.
    /// </summary>
    public static EVMRuntimeCode Empty { get; } = new(ReadOnlyMemory<byte>.Empty);
}
