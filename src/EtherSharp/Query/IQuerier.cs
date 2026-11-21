using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Query;

/// <summary>
/// Describes the querier's custom query protocol.
/// </summary>
public partial interface IQuerier
{
    /// <summary>
    /// Describes the fixed account <see cref="IQuerierDelegate"/> code delegates query calls to.
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
            // Deployment: opcode (1), initcode length (2), initcode.
            private const int DEPLOYMENT_HEADER_LENGTH = 3;

            /// <summary>
            /// Encodes all supplied operations into one query payload.
            /// </summary>
            /// <remarks>
            /// Without state overrides the payload starts by deploying <paramref name="flashCodes"/>, which state overrides
            /// install instead.
            /// </remarks>
            public static byte[] Encode(
                IReadOnlyList<IQueryOperation> queries,
                IReadOnlyDictionary<Address, EVMRuntimeCode>? flashCodes,
                QueryEncodingContext context,
                out int payloadSize,
                out UInt256 ethValue)
            {
                ethValue = 0;
                payloadSize = 0;

                var deployments = context.UsesStateOverrides || flashCodes is null
                    ? []
                    : flashCodes.Values;

                foreach(var runtimeCode in deployments)
                {
                    payloadSize += DEPLOYMENT_HEADER_LENGTH + EVMInitCode.GetLengthFromRuntimeCode(runtimeCode);
                }

                foreach(var query in queries)
                {
                    payloadSize += query.GetCallDataLength(context);
                    ethValue += query.EthValue;
                }

                byte[] result = ArrayPool<byte>.Shared.Rent(payloadSize);
                result.AsSpan(0, payloadSize).Clear();

                var buffer = result.AsSpan(0, payloadSize);
                foreach(var runtimeCode in deployments)
                {
                    int initCodeLength = EVMInitCode.GetLengthFromRuntimeCode(runtimeCode);
                    buffer[0] = (byte) QueryOperationId.DeployFlashCode;
                    BinaryPrimitives.WriteUInt16BigEndian(buffer[1..DEPLOYMENT_HEADER_LENGTH], checked((ushort) initCodeLength));
                    EVMInitCode.WriteFromRuntimeCode(runtimeCode, buffer[DEPLOYMENT_HEADER_LENGTH..]);
                    buffer = buffer[(DEPLOYMENT_HEADER_LENGTH + initCodeLength)..];
                }

                foreach(var query in queries)
                {
                    int length = query.GetCallDataLength(context);
                    query.Encode(buffer[..length], context);
                    buffer = buffer[length..];
                }

                return result;
            }
        }
    }
}
