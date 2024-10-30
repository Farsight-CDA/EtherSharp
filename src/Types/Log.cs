using System.Numerics;

namespace EtherSharp.Types;
public record Log(string Address, byte[] Data, string BlockHash, BigInteger BlockNumber, uint LogIndex, string TransactionHash, uint TransactionIndex, string[] Topics);
