using EtherSharp.Common.Converter;
using System.Text.Json.Serialization;

namespace EtherSharp.Types;

/// <summary>
/// Represents the Ethereum transaction envelope type.
/// </summary>
[JsonConverter(typeof(TransactionTypeHexConverter))]
public enum TxType : uint
{
    /// <summary>
    /// Legacy pre-EIP-2718 transaction.
    /// </summary>
    Legacy = 0,
    /// <summary>
    /// EIP-2930 access-list transaction.
    /// </summary>
    EIP2930AccessList = 1,
    /// <summary>
    /// EIP-1559 dynamic-fee transaction.
    /// </summary>
    EIP1559DynamicFee = 2,
    /// <summary>
    /// EIP-4844 blob-carrying transaction.
    /// </summary>
    EIP4844Blob = 3,
    /// <summary>
    /// EIP-7702 set-code transaction.
    /// </summary>
    EIP7702SetCode = 4,
    /// <summary>
    /// OP Stack deposit transaction.
    /// </summary>
    OPDeposit = 126
}
