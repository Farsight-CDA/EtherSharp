using System.Buffers.Binary;

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
    private const int RUNTIME_OFFSET = 12;

    /// <summary>
    /// Creates minimal initcode that deploys <paramref name="runtimeCode"/>.
    /// </summary>
    public static EVMInitCode FromRuntimeCode(EVMRuntimeCode runtimeCode)
    {
        byte[] initCode = new byte[GetLengthFromRuntimeCode(runtimeCode)];
        WriteFromRuntimeCode(runtimeCode, initCode);
        return new EVMInitCode(initCode);
    }

    /// <summary>
    /// Gets the length of the initcode <see cref="FromRuntimeCode"/> creates for <paramref name="runtimeCode"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="runtimeCode"/> exceeds 65535 bytes.</exception>
    public static int GetLengthFromRuntimeCode(EVMRuntimeCode runtimeCode)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(runtimeCode.Length, UInt16.MaxValue);
        return RUNTIME_OFFSET + runtimeCode.Length;
    }

    /// <summary>
    /// Writes the initcode <see cref="FromRuntimeCode"/> creates for <paramref name="runtimeCode"/> into <paramref name="destination"/>.
    /// </summary>
    /// <param name="runtimeCode">The runtime code to deploy.</param>
    /// <param name="destination">Destination buffer of at least <see cref="GetLengthFromRuntimeCode"/> bytes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="runtimeCode"/> exceeds 65535 bytes.</exception>
    public static void WriteFromRuntimeCode(EVMRuntimeCode runtimeCode, Span<byte> destination)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(runtimeCode.Length, UInt16.MaxValue);

        var header = destination[..RUNTIME_OFFSET];

        header[0] = (byte) EvmOpcode.Push2;
        BinaryPrimitives.WriteUInt16BigEndian(header[1..], (ushort) runtimeCode.Length);
        header[3] = (byte) EvmOpcode.Push1;
        header[4] = RUNTIME_OFFSET;
        header[5] = (byte) EvmOpcode.ReturnDataSize;
        header[6] = (byte) EvmOpcode.CodeCopy;
        header[7] = (byte) EvmOpcode.Push2;
        BinaryPrimitives.WriteUInt16BigEndian(header[8..], (ushort) runtimeCode.Length);
        header[10] = (byte) EvmOpcode.ReturnDataSize;
        header[11] = (byte) EvmOpcode.Return;
        runtimeCode.ByteCode.Span.CopyTo(destination[RUNTIME_OFFSET..]);
    }
}
