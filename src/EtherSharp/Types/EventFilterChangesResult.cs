using EtherSharp.Events;
using System.Numerics;
namespace EtherSharp.Types;

public record EventFilterChangesResult(
    bool Tag,
    uint LogIndex,
    uint TransactionIndex,
    string TransactionHash,
    string BlockHash,
    BigInteger BlockNumber,
    string Address,
    byte[][] Topics,
    byte[] Data
) : IEventData;