namespace EtherSharp.Contract;

/// <summary>
/// Represents initcode, the code executed by contract creation that returns the runtime code to deploy.
/// </summary>
/// <param name="byteCode">The bytecode bytes. The backing memory must not be modified while the bytecode is in use.</param>
public sealed class EVMInitCode(ReadOnlyMemory<byte> byteCode) : EVMByteCode(byteCode)
{
    /// <summary>
    /// The maximum initcode length accepted by contract creation.
    /// </summary>
    public const int MAX_LENGTH = 49_152;
}
