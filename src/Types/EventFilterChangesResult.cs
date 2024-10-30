using System.Numerics;
namespace EtherSharp.Types;

public record EventFilterChangesResult(
    bool Tag,
    uint LogIndex,
    uint TransactionIndex,
    string TransactionHash,
    string BlockHash,
    BigInteger BlockNumber,
    byte[] Address,
    byte[] Data,
    IReadOnlyList<string> Topics);