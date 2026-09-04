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
            "6000805b813683101561024657823560001a9260010190600284106101e2575090916004811061015d57600360091982011061010d5760066013198201106100915780601e1461007c5780601f1461006b5760281461005d57506000fd5b905a81526020905b01610003565b508035548252602090810191610065565b50803560601c31825260140190602090610065565b919091806014146100fe57806015146100ef57806016146100e057806017146100d1576018146100c657488152602090610065565b3a8152602090610065565b504560c01b8152600890610065565b504260c01b8152600890610065565b504360c01b8152600890610065565b504660c01b8152600890610065565b6014820192913560601c90600a811461013f57600b14610134573b15158153600190610065565b3f8152602090610065565b50806000913b9182918260e81b855260038501903c60030190610065565b9190916002146101ad576101a781833560e81c60238160038701359663ea41597b60e01b855281838201600487013701019438303b141560021b809201916004038301903061024b565b90610065565b6101a781833560f01c600285013560e81c9060256005870135968383019081838201873701019581846000f09184019161024b565b8291358060e81c9060481c916038826018830135928183820188370101951561023a57906000929383925a9561c34f195a01f1905a900360981b3d60d81b179060f81b1781523d6000600d83013e3d600d0190610065565b926101a793819361024b565b506000f35b9160009391849361c34f195a01f13d60e01b82528153600060043d92013e3d6004019056"
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
