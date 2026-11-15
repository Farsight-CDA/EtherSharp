using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers;

namespace EtherSharp.Query;

/// <summary>
/// Describes the querier's custom query protocol.
/// </summary>
public partial interface IQuerier
{
    /// <summary>
    /// Describes the ephemeral account used to execute the querier through state overrides.
    /// </summary>
    public static class StateOverride
    {
        /// <summary>
        /// Gets the fixed simulation-only account address.
        /// </summary>
        public static Address Address { get; } = Address.Parse("0x0000000000000000000000000000000000455351");
    }

    /// <summary>
    /// Functions implemented by the querier's custom protocol.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Encodes and executes query operations.
        /// </summary>
        public static class Query
        {
            /// <summary>
            /// Encodes all supplied operations into one query payload.
            /// </summary>
            public static byte[] Encode(
                IReadOnlyList<IQueryOperation> queries,
                out int payloadSize,
                out UInt256 ethValue)
            {
                ethValue = 0;
                payloadSize = 0;

                foreach(var query in queries)
                {
                    payloadSize = checked(payloadSize + query.CallDataLength);
                    ethValue += query.EthValue;
                }

                byte[] result = ArrayPool<byte>.Shared.Rent(payloadSize);
                result.AsSpan(0, payloadSize).Clear();

                var buffer = result.AsSpan(0, payloadSize);
                foreach(var query in queries)
                {
                    query.Encode(buffer);
                    buffer = buffer[query.CallDataLength..];
                }

                return result;
            }
        }
    }
}
