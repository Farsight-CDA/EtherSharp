using System.Numerics;

namespace EVM.net.types;
public record Log(string Address, byte[] Data, string BlockHash, BigInteger BlockNumber, uint LogIndex, string TransactionHash, uint TransactionIndex, string[] Topics);
