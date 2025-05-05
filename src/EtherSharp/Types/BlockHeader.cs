namespace EtherSharp.Types;
public record BlockHeader(
    ulong Number,
    DateTimeOffset Timestamp,
    string Hash,
    string ParentHash,
    ulong Nonce,
    string Miner,
    ulong GasLimit,
    ulong GasUsed
);
