using System.Numerics;

namespace EVM.net.types;
public record TransactionRecievedLog(
    int LogIndex,
    BigInteger? BlockNumber,
    string? BlockHash,
    string? TransactionHash,
    string? TransactionIndex,
    string? Address,
    string? Data,
    List<string>? Topics);