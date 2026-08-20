using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Query;

/// <summary>
/// Describes the querier contract bytecode and its custom query protocol.
/// </summary>
public partial interface IQuerier
{
    /// <summary>
    /// Querier runtime bytecode variants.
    /// </summary>
    public static class Code
    {
        /// <summary>
        /// London-compatible querier bytecode.
        /// </summary>
        public static class London
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
        /// Cancun querier bytecode.
        /// </summary>
        public static class Cancun
        {
            /// <summary>
            /// Gets the deployed runtime bytecode.
            /// </summary>
            public static EVMByteCode Runtime { get; } = new(Convert.FromHexString(
                "5f3560e01c60045f5b81368110156101c857600181355f1a910192600282106001146101cf57508060021461016f5780600a1461014a5780600b146101355780600c1461011f5780601414610111578060151461010357806016146100f557806017146100e757806018146100dc57806019146100d15780601e146100bc5780601f146100aa57602814610091575f80fd5b5a815260205b8101908382116100a75750610008565b5ff35b50602082359201915481526020610097565b506014823560601c9201913181526020610097565b504881526020610097565b503a81526020610097565b504560c01b81526008610097565b504260c01b81526008610097565b504360c01b81526008610097565b504660c01b81526008610097565b5060148201913560601c3b151581536001610097565b506014823560601c9201913f81526020610097565b505f6014833560601c930192803b9182918260e81b855260038501903c600301610097565b505f80833560f01c600285013560e81c90602560058701359683830190818382018937010195818685f09186019161c34f195a01f161d6d85a108115166101c8573d60e01b825281533d5f600483013e3d600401610097565b509150505ff35b8291933560e81c600482013560601c916038826018830135928183820188370101951561023c57905f929383925a9561c34f195a01f1905a900361d6d85a108215166102345760981b3d60d81b179060f81b1781523d5f600d83013e3d600d01610097565b50509150505ff35b925f93849361c34f195a01f161d6d85a108115166101c8573d60e01b825281533d5f600483013e3d60040161009756"));

            /// <summary>
            /// Gets the runtime as flash-call code.
            /// </summary>
            public static IFlashCode Flash { get; } = IFlashCode.FromRuntimeCode(Runtime);
        }
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
