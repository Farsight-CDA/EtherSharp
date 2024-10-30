using System.Numerics;
namespace EVM.net.types;

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