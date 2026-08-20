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
    /// Caller override runtime bytecode factories.
    /// </summary>
    public static class Code
    {
        private const int SIMPLE_QUERIER_ADDRESS_OFFSET = 36;
        private const int PRESERVING_ORIGINAL_CODE_ADDRESS_OFFSET = 3;
        private const int PRESERVING_QUERIER_ADDRESS_OFFSET = 85;

        private static byte[] SimpleTemplate { get; } = Convert.FromHexString(
            "63ea41597b60003560e01c14156004361017604c5760008036600319018060048337817311111111111111111111111111111111111111115af43d6000803e156047573d6000f35b3d6000fd5b600080fd");

        private static byte[] PreservingTemplate { get; } = Convert.FromHexString(
            "600073222222222222222222222222222222222222222263ea41597b60003560e01c146003361116604e575b81600080939281933603809383375af43d6000803e156049573d6000f35b3d6000fd5b5060049050731111111111111111111111111111111111111111602b56");

        /// <summary>
        /// Creates runtime code that only accepts delegated query calls.
        /// </summary>
        public static EVMByteCode Create(in Address querierAddress)
        {
            byte[] code = [.. SimpleTemplate];
            querierAddress.CopyTo(code.AsSpan(SIMPLE_QUERIER_ADDRESS_OFFSET, Address.BYTES_LENGTH));
            return new EVMByteCode(code);
        }

        /// <summary>
        /// Creates runtime code that delegates non-query calls to the original implementation.
        /// </summary>
        public static EVMByteCode CreatePreserving(
            in Address querierAddress,
            in Address originalCodeAddress)
        {
            byte[] code = [.. PreservingTemplate];
            querierAddress.CopyTo(code.AsSpan(PRESERVING_QUERIER_ADDRESS_OFFSET, Address.BYTES_LENGTH));
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
