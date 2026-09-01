using EtherSharp.Contract;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query;

/// <summary>
/// Describes the caller override contract that delegates query execution to an <see cref="IQuerier"/> runtime.
/// </summary>
public partial interface IQuerierDelegate
{
    /// <summary>
    /// Caller override runtime bytecode and factories.
    /// </summary>
    public static class Code
    {
        private const int PRESERVING_ORIGINAL_CODE_ADDRESS_OFFSET = 3;

        /// <summary>
        /// Gets runtime code that only accepts delegated query calls.
        /// </summary>
        public static EVMByteCode Runtime { get; } = new(Convert.FromHexString(
            "63ea41597b60003560e01c14156004361017603b576000803660031901806004833781624553515af43d6000803e156036573d6000f35b3d6000fd5b600080fd"));

        private static byte[] PreservingTemplate { get; } = Convert.FromHexString(
            "600073222222222222222222222222222222222222222263ea41597b60003560e01c146003361116604e575b81600080939281933603809383375af43d6000803e156049573d6000f35b3d6000fd5b506004905062455351602b56");

        /// <summary>
        /// Creates runtime code that delegates non-query calls to the original implementation.
        /// </summary>
        public static EVMByteCode CreatePreserving(
            in Address originalCodeAddress)
        {
            byte[] code = [.. PreservingTemplate];
            originalCodeAddress.CopyTo(code.AsSpan(PRESERVING_ORIGINAL_CODE_ADDRESS_OFFSET, Address.BYTES_LENGTH));
            return new EVMByteCode(code);
        }
    }

    /// <summary>
    /// Functions implemented by the caller override contract.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Delegates a query payload to the querier runtime.
        /// </summary>
        public static class Query
        {
            /// <summary>
            /// Gets the query function selector.
            /// </summary>
            public static Bytes4 Selector { get; } = Bytes4.Parse("0xEA41597B");

            /// <summary>
            /// Gets the encoded calldata length for operations of the supplied total length.
            /// </summary>
            public static int GetCallDataLength(int operationsLength)
                => Bytes4.BYTE_LENGTH + sizeof(uint) + operationsLength;

            /// <summary>
            /// Encodes a delegated query call.
            /// </summary>
            public static void Encode(Span<byte> buffer, IReadOnlyList<IQuery> operations)
            {
                Selector.CopyTo(buffer);
                BinaryPrimitives.WriteUInt32BigEndian(buffer[Bytes4.BYTE_LENGTH..], UInt32.MaxValue);

                var operationBuffer = buffer[(Bytes4.BYTE_LENGTH + sizeof(uint))..];
                foreach(var operation in operations)
                {
                    operation.Encode(operationBuffer);
                    operationBuffer = operationBuffer[operation.CallDataLength..];
                }
            }
        }
    }
}
