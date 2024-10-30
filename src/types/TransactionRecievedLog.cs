using System.Numerics;

namespace EtherSharp.Types;
public record TransactionRecievedLog(
    int LogIndex,
    BigInteger? BlockNumber,
    string? BlockHash,
    string? TransactionHash,
    string? TransactionIndex,
    string? Address,
    string? Data,
    List<string>? Topics);