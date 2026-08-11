using EtherSharp.Contract;
using System.Buffers.Binary;

namespace EtherSharp.Tx;

/// <summary>
/// Describes initcode or runtime code executed by a flash call.
/// </summary>
public interface IFlashCode
{
    private const int RUNTIME_OFFSET = 12;

    /// <summary>
    /// Attempts to get runtime code suitable for state override execution.
    /// </summary>
    /// <param name="runtimeCode">The runtime code, when available.</param>
    /// <returns>Whether runtime code is available.</returns>
    public bool TryGetRuntimeCode(out EVMByteCode runtimeCode);

    /// <summary>
    /// Gets the initcode length without generating wrapping initcode.
    /// </summary>
    public int GetInitCodeLength();

    /// <summary>
    /// Gets initcode suitable for constructor-backed execution.
    /// </summary>
    public EVMByteCode GetInitCode();

    /// <summary>
    /// Creates flash code from initcode.
    /// </summary>
    public static IFlashCode FromInitCode(EVMByteCode initCode)
        => new FlashCode(initCode, null);

    /// <summary>
    /// Creates flash code from runtime code.
    /// </summary>
    public static IFlashCode FromRuntimeCode(EVMByteCode runtimeCode)
        => new FlashCode(null, runtimeCode);

    /// <summary>
    /// Creates flash code with both initcode and runtime code representations.
    /// </summary>
    public static IFlashCode FromCode(EVMByteCode initCode, EVMByteCode runtimeCode)
        => new FlashCode(initCode, runtimeCode);

    /// <summary>
    /// Creates minimal initcode that deploys the supplied runtime bytecode.
    /// </summary>
    public static EVMByteCode CreateInitCode(EVMByteCode runtimeCode)
    {
        byte[] initCode = new byte[GetInitCodeLength(runtimeCode)];
        var header = initCode.AsSpan(0, RUNTIME_OFFSET);

        header[0] = 0x61; // PUSH2 runtime length
        BinaryPrimitives.WriteUInt16BigEndian(header[1..], (ushort) runtimeCode.Length);
        header[3] = 0x60; // PUSH1 runtime offset
        header[4] = RUNTIME_OFFSET;
        header[5] = 0x3D; // RETURNDATASIZE
        header[6] = 0x39; // CODECOPY
        header[7] = 0x61; // PUSH2 runtime length
        BinaryPrimitives.WriteUInt16BigEndian(header[8..], (ushort) runtimeCode.Length);
        header[10] = 0x3D; // RETURNDATASIZE
        header[11] = 0xF3; // RETURN
        runtimeCode.ByteCode.Span.CopyTo(initCode.AsSpan(RUNTIME_OFFSET));

        return new EVMByteCode(initCode);
    }

    /// <summary>
    /// Gets the length of the minimal initcode that deploys the supplied runtime bytecode.
    /// </summary>
    public static int GetInitCodeLength(EVMByteCode runtimeCode)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(runtimeCode.Length, UInt16.MaxValue);
        return RUNTIME_OFFSET + runtimeCode.Length;
    }
}
