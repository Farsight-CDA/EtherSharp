using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Interpreter.Forking;

/// <summary>
/// Describes one logical interpreter data request.
/// </summary>
public abstract record InterpreterDataRequest
{
    private InterpreterDataRequest()
    {
    }

    /// <summary>
    /// Requests an account's native balance.
    /// </summary>
    /// <param name="Address">The account address.</param>
    public sealed record Balance(Address Address) : InterpreterDataRequest;

    /// <summary>
    /// Requests an account's nonce.
    /// </summary>
    /// <param name="Address">The account address.</param>
    public sealed record Nonce(Address Address) : InterpreterDataRequest;

    /// <summary>
    /// Requests an account's bytecode.
    /// </summary>
    /// <param name="Address">The account address.</param>
    public sealed record Code(Address Address) : InterpreterDataRequest;

    /// <summary>
    /// Requests an account's canonical code hash.
    /// </summary>
    /// <param name="Address">The account address.</param>
    public sealed record CodeHash(Address Address) : InterpreterDataRequest;

    /// <summary>
    /// Requests one persistent storage slot.
    /// </summary>
    /// <param name="Address">The account address.</param>
    /// <param name="Key">The storage key.</param>
    public sealed record Storage(Address Address, Bytes32 Key) : InterpreterDataRequest;

    /// <summary>
    /// Requests execution of an input-only upstream precompile, not arbitrary contract code.
    /// </summary>
    /// <param name="Caller">The immediate message caller.</param>
    /// <param name="Target">The account to call.</param>
    /// <param name="Value">The native value supplied to the call.</param>
    /// <param name="Input">The borrowed call input.</param>
    /// <param name="Id">The Keccak-256 digest of caller (20 bytes), target (20 bytes), value (32-byte big-endian), and input.</param>
    /// <remarks>
    /// The input buffer must remain unchanged until the request completes. An explicitly supplied
    /// <paramref name="Id"/> must match the call fields; callers modifying those fields must also update it.
    /// </remarks>
    public sealed record PrecompileCall(
        Address Caller,
        Address Target,
        UInt256 Value,
        ReadOnlyMemory<byte> Input,
        Bytes32 Id
    ) : InterpreterDataRequest
    {
        /// <summary>Creates an upstream precompile call request and computes its identity.</summary>
        /// <param name="caller">The immediate message caller.</param>
        /// <param name="target">The account to call.</param>
        /// <param name="value">The native value supplied to the call.</param>
        /// <param name="input">The input, which must remain unchanged until completion.</param>
        public PrecompileCall(Address caller, Address target, UInt256 value, ReadOnlyMemory<byte> input)
            : this(caller, target, value, input, ComputeId(caller, target, value, input.Span))
        {
        }

        internal static Bytes32 ComputeId(Address caller, Address target, UInt256 value, ReadOnlySpan<byte> input)
        {
            // Fixed-width fields followed by the remaining calldata give an unambiguous encoding.
            int length = checked(72 + input.Length);
            byte[]? rented = null;
            var data = length <= 1024
                ? stackalloc byte[length]
                : (rented = ArrayPool<byte>.Shared.Rent(length)).AsSpan(0, length);
            try
            {
                caller.CopyTo(data[..20]);
                target.CopyTo(data[20..40]);
                BinaryPrimitives.WriteUInt256BigEndian(data[40..72], value);
                input.CopyTo(data[72..]);
                return Keccak256.HashData(data);
            }
            finally
            {
                if(rented is not null)
                {
                    ArrayPool<byte>.Shared.Return(rented);
                }
            }
        }

        /// <inheritdoc/>
        public bool Equals(PrecompileCall? other)
            => other is not null && Id == other.Id;

        /// <inheritdoc/>
        public override int GetHashCode()
            => Id.GetHashCode();
    }
}
