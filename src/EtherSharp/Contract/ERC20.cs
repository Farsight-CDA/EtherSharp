using EtherSharp.ABI;
using EtherSharp.Tx;
using System.Numerics;

namespace EtherSharp.Contract;
public class ERC20(string address) : IContract
{
    public string Address { get; } = address;

    private static readonly byte[] _balanceOfSignature = [0x70, 0xa0, 0x82, 0x31];
    
    
    public TxInput<BigInteger> BalanceOf(string address)
    {
        var encoder = new AbiEncoder()
            .Address(address);

        return new TxInput<BigInteger>(
            Address,
            _balanceOfSignature,
            encoder,
            decoder =>
            {
                decoder.UInt256(out var balance);
                return balance;
            }
        );
    }
}
