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
            "6000805b813681101561024557600060018235821a920193600283106001146101be5750508060021461016e5780600a146101475780600b146101315780600c1461011a578060141461010b57806015146100fc57806016146100ed57806017146100de57806018146100d257806019146100c65780601e146100b05780601f1461009d5760281461009057600080fd5b5a81526020905b01610003565b5060208235920191548152602090610097565b506014823560601c920191318152602090610097565b50488152602090610097565b503a8152602090610097565b504560c01b8152600890610097565b504260c01b8152600890610097565b504360c01b8152600890610097565b504660c01b8152600890610097565b5060148201913560601c3b15158153600190610097565b506014823560601c9201913f8152602090610097565b5060006014833560601c930192803b9182918260e81b855260038501903c60030190610097565b50600080833560f01c600285013560e81c90602560058701359683830190818382018937010195818685f09186019161c34f195a01f13d60e01b825281533d6000600483013e3d60040190610097565b90919383903560e81c91600481013560601c936038846018840135938183820187370101961561021e5750916000929183925a9561c34f195a01f1905a900360981b3d60d81b179060f81b1781523d6000600d83013e3d600d0190610097565b9391849391849361c34f195a01f13d60e01b835282533d90600483013e3d60040190610097565b506000f3"));

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
