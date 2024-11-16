using BenchmarkDotNet.Attributes;
using EtherSharp.Crypto;
using Nethereum.HdWallet;
using Nethereum.Web3.Accounts;

namespace EtherSharp.Bench;
[MemoryDiagnoser]
[ShortRunJob]
public class BIPBenchmarks
{
    [Benchmark]
    public (byte[], byte[]) BIP32_EtherSharp()
        => BIP32Curves.Secp256k1.DerivePath(
            BIP39.MnemonicToSeed("ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal"),
            "m/44'/60'/0'/0/10"
        );

    [Benchmark]
    public Account BIP32_NEthereum()
        => new Wallet("ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal", "", "m/44'/60'/0'/0/10")
            .GetAccount(10);
}
