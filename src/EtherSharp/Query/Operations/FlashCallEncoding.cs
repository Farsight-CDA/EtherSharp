using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Types;
using System.Buffers;

namespace EtherSharp.Query.Operations;

/// <summary>
/// Derives where flash code runs, shared by query and standalone flash calls.
/// </summary>
internal static class FlashCallEncoding
{
    private const int MAX_STACKALLOC_BYTES = 1024;

    /// <summary>
    /// Gets the hash of the minimal initcode deploying <paramref name="runtimeCode"/>.
    /// </summary>
    /// <remarks>Without state overrides the Querier deploys the code with CREATE2 and salt 0, under this hash.</remarks>
    public static Bytes32 GetInitCodeHash(EVMRuntimeCode runtimeCode)
    {
        int length = EVMInitCode.GetLengthFromRuntimeCode(runtimeCode);
        byte[]? rented = null;
        var initCode = length <= MAX_STACKALLOC_BYTES
            ? stackalloc byte[length]
            : (rented = ArrayPool<byte>.Shared.Rent(length));

        try
        {
            EVMInitCode.WriteFromRuntimeCode(runtimeCode, initCode);
            return Keccak256.HashData(initCode[..length]);
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    /// <summary>
    /// Gets the account flash code is installed at through a state override, the low 20 bytes of its initcode hash.
    /// </summary>
    public static Address GetStateOverrideAddress(in Bytes32 initCodeHash)
        => Address.FromBytes(initCodeHash.DangerousGetReadOnlySpan()[^Address.BYTES_LENGTH..]);
}
