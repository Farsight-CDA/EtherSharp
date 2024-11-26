using EtherSharp.ABI;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Contract;
public class ERC20(string address) : IContract
{
    public string ContractAddress { get; } = address;

    private static readonly byte[] _balanceOfSignature = [0x70, 0xa0, 0x82, 0x31];
    public TxInput<BigInteger> BalanceOf(string address)
    {
        var encoder = new AbiEncoder()
            .Address(address);

        return new TxInput<BigInteger>(
            _balanceOfSignature,
            encoder,
            decoder =>
            {
                _ = decoder.UInt256(out var balance);
                return balance;
            },
            Address.FromString(address),
            0
        );
    }

    private static readonly byte[] _transferSignature = [0xa9, 0x05, 0x9c, 0xbb];
    public TxInput<bool> Transfer(string recipient, BigInteger amount)
    {
        var encoder = new AbiEncoder()
            .Address(recipient)
            .UInt256(amount);

        return new TxInput<bool>(
            _transferSignature,
            encoder,
            decoder =>
            {
                _ = decoder.Bool(out bool success);
                return success;
            },
            Address.FromString(ContractAddress),
            0
        );
    }
}
