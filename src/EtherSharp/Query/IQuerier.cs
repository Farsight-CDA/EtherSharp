using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Query;

/// <summary>
/// Describes the querier contract bytecode and its custom query protocol.
/// </summary>
public partial interface IQuerier
{
    /// <summary>
    /// Describes the ephemeral account used to execute the querier through state overrides.
    /// </summary>
    public static class StateOverride
    {
        /// <summary>
        /// Gets the simulation-only account address derived from <see cref="Code.Runtime"/>.
        /// </summary>
        public static Address Address { get; } = Address.FromBytes(Keccak256.HashData(Code.Runtime.ByteCode.Span).DangerousGetReadOnlySpan()[^Address.BYTES_LENGTH..]);

        /// <summary>
        /// Gets the account override that installs the querier runtime at <see cref="Address"/>.
        /// </summary>
        public static AccountOverride Account { get; } = new(balance: UInt256.Zero, nonce: 1, code: Code.Runtime.ByteCode);
    }

    /// <summary>
    /// Querier runtime bytecode.
    /// </summary>
    public static class Code
    {
        /// <summary>
        /// Gets the deployed runtime bytecode.
        /// </summary>
        public static EVMByteCode Runtime { get; } = new(Convert.FromHexString(
            "60003560e01c600460005b81368110156101d257600060018235821a920193600283106001146101da575050806002146101775780600a146101515780600b1461013c5780600c146101265780601414610118578060151461010a57806016146100fc57806017146100ee57806018146100e357806019146100d85780601e146100c35780601f146100b15760281461009757600080fd5b5a815260205b8101908382116100ad575061000a565b6000f35b5060208235920191548152602061009d565b506014823560601c920191318152602061009d565b50488152602061009d565b503a8152602061009d565b504560c01b8152600861009d565b504260c01b8152600861009d565b504360c01b8152600861009d565b504660c01b8152600861009d565b5060148201913560601c3b15158153600161009d565b506014823560601c9201913f8152602061009d565b5060006014833560601c930192803b9182918260e81b855260038501903c60030161009d565b50600080833560f01c600285013560e81c90602560058701359683830190818382018937010195818685f09186019161c34f195a01f161d6d85a108115166101d2573d60e01b825281533d6000600483013e3d60040161009d565b509150506000f35b90919383903560e81c91600481013560601c936038846018840135938183820187370101961561024e5750916000929183925a9561c34f195a01f1905a900361d6d85a108215166102455760981b3d60d81b179060f81b1781523d6000600d83013e3d600d0161009d565b50509150506000f35b9391849391849361c34f195a01f161d6d85a10811516610245573d60e01b835282533d90600483013e3d60040161009d56"));

        /// <summary>
        /// Gets the runtime as flash-call code.
        /// </summary>
        public static IFlashCode Flash { get; } = IFlashCode.FromRuntimeCode(Runtime);
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
            /// Encodes as many operations as fit in the supplied provider limits.
            /// </summary>
            public static byte[] Encode(
                IReadOnlyList<IQuery> queries,
                int startIndex,
                int maxPayloadSize,
                int maxResultSize,
                out int payloadSize,
                out int encodedCallCount,
                out UInt256 ethValue)
            {
                ethValue = 0;
                payloadSize = sizeof(uint);
                int callCount = 0;

                for(int i = startIndex; i < queries.Count; i++)
                {
                    var query = queries[i];
                    int newDataLength = payloadSize + query.CallDataLength;

                    if(newDataLength > maxPayloadSize)
                    {
                        break;
                    }

                    payloadSize = newDataLength;
                    callCount++;
                    ethValue += query.EthValue;
                }

                encodedCallCount = callCount;

                byte[] result = ArrayPool<byte>.Shared.Rent(payloadSize);
                result.AsSpan(0, payloadSize).Clear();
                BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(0, sizeof(uint)), (uint) maxResultSize);

                var buffer = result.AsSpan(sizeof(uint));
                for(int i = startIndex; callCount > 0; i++)
                {
                    var query = queries[i];
                    query.Encode(buffer);
                    buffer = buffer[query.CallDataLength..];
                    callCount--;
                }

                return result;
            }
        }
    }
}
