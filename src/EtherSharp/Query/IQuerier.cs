using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;

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
        /// Gets the fixed simulation-only account address.
        /// </summary>
        public static Address Address { get; } = Address.Parse("0x0000000000000000000000000000000000455351");

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
            "6000805b813683101561027c57600160008435901a9301906002841060011461021557509091806002146101c4578060031461017c5780600a146101545780600b1461013f5780600c146101275780601414610117578060151461010757806016146100f757806017146100e757806018146100da57806019146100cd5780601e146100b85780601f146100a75760281461009957506000fd5b905a81526020905b01610003565b5080355482526020908101916100a1565b50803560601c318252601401906020906100a1565b50904881526020906100a1565b50903a81526020906100a1565b50904560c01b81526008906100a1565b50904260c01b81526008906100a1565b50904360c01b81526008906100a1565b50904660c01b81526008906100a1565b506014810191903560601c3b151581536001906100a1565b50803560601c3f8252601401906020906100a1565b509060006014833560601c930192803b9182918260e81b855260038501903c600301906100a1565b50906101be81833560e81c60238160038701359663ea41597b60e01b855281838201600487013701019438303b141560021b8092019160040383019030610281565b906100a1565b5090600080833560f01c600285013560e81c90602560058701359683830190818382018937010195818685f09186019161c34f195a01f13d60e01b825281533d6000600483013e3d600401906100a1565b82913560e81c600482013560601c916038826018830135928183820188370101951561027057906000929383925a9561c34f195a01f1905a900360981b3d60d81b179060f81b1781523d6000600d83013e3d600d01906100a1565b926101be938193610281565b506000f35b9160009391849361c34f195a01f13d60e01b82528153600060043d92013e3d6004019056"));

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
            /// Encodes all supplied operations into one query payload.
            /// </summary>
            public static byte[] Encode(
                IReadOnlyList<IQuery> queries,
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
