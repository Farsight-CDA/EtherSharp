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
            "6000805b813683101561025b57823560001a9260010190600284106101f757509091806002146101c057806003146101785780600a146101505780600b1461013b5780600c146101235780601414610113578060151461010357806016146100f357806017146100e357806018146100d657806019146100c95780601e146100b45780601f146100a35760281461009557506000fd5b905a81526020905b01610003565b50803554825260209081019161009d565b50803560601c3182526014019060209061009d565b509048815260209061009d565b50903a815260209061009d565b50904560c01b815260089061009d565b50904260c01b815260089061009d565b50904360c01b815260089061009d565b50904660c01b815260089061009d565b506014810191903560601c3b1515815360019061009d565b50803560601c3f82526014019060209061009d565b509060006014833560601c930192803b9182918260e81b855260038501903c6003019061009d565b50906101ba81833560e81c60238160038701359663ea41597b60e01b855281838201600487013701019438303b141560021b8092019160040383019030610260565b9061009d565b50906101ba81833560f01c600285013560e81c9060256005870135968383019081838201873701019581846000f091840191610260565b8291358060e81c9060481c916038826018830135928183820188370101951561024f57906000929383925a9561c34f195a01f1905a900360981b3d60d81b179060f81b1781523d6000600d83013e3d600d019061009d565b926101ba938193610260565b506000f35b9160009391849361c34f195a01f13d60e01b82528153600060043d92013e3d6004019056"
        ));

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
