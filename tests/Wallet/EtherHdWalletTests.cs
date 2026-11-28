using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Tx.Legacy;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Keysmith.Net.EC;

namespace EtherSharp.Tests.Wallet;

public sealed class EtherHdWalletTests
{
    public static TheoryData<string, string, string> KeyVectors => new()
    {
        {
            "0000000000000000000000000000000000000000000000000000000000000001",
            "0479BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8",
            "0x7E5F4552091A69125d5DfCb7b8C2659029395Bdf"
        },
        {
            "4c0883a69102937d6231471b5dbb6204fe5129617082792ae468d01a3f362318",
            "044E3B81AF9C2234CAD09D679CE6035ED1392347CE64CE405F5DCD36228A25DE6E47FD35C4215D1EDF53E6F83DE344615CE719BDB0FD878F6ED76F06DD277956DE",
            "0x2c7536E3605D9C16a7a3D7b1898e529396a65c23"
        },
        {
            "4646464646464646464646464646464646464646464646464646464646464646",
            "044BC2A31265153F07E70E0BAB08724E6B85E217F8CD628CEB62974247BB493382CE28CAB79AD7119EE1AD3EBCDB98A16805211530ECC6CFEFA1B88E6DFF99232A",
            "0x9d8A62f656a8d1615C1294fd71e9CFb3E4855A4F"
        },
        {
            "ac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80",
            "048318535B54105D4A7AAE60C08FC45F9687181B4FDFC625BD1A753FA7397FED753547F11CA8696646F2F3ACB08E31016AFAC23E630C5D11F59F61FEF57B0D2AA5",
            "0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266"
        },
    };

    private const string HARDHAT_MNEMONIC = "test test test test test test test test test test test junk";

    // web3.js accounts.sign("Some data", "0x4c0883...")
    private const string WEB3_PRIVATE_KEY = "4c0883a69102937d6231471b5dbb6204fe5129617082792ae468d01a3f362318";
    private const string WEB3_MESSAGE_HASH = "0x1da44b586eb0729ff70a73c326926f6ed5a25f5b056e7f47fbc6e58d86871655";
    private const string WEB3_R = "0xb91467e570a6466aa9e9876cbcd013baba02900b8979d43fe208a4a4f339f5fd";
    private const string WEB3_S = "0x6007e74cd82e037b800186422fc2da167c747ef045e5d18a5f5d4300f8e1a029";

    [Theory]
    [MemberData(nameof(KeyVectors))]
    public void Should_Derive_Address_From_PrivateKey(string privateKey, string _, string expectedAddress)
    {
        var wallet = new EtherHdWallet(Convert.FromHexString(privateKey));

        Assert.Equal(expectedAddress, wallet.Address.ToEIP55String());
    }

    [Theory]
    [MemberData(nameof(KeyVectors))]
    public void Should_Receive_Sec1_Uncompressed_PublicKey_From_Keysmith(string privateKey, string expectedPublicKey, string _)
    {
        byte[] publicKey = new byte[Secp256k1.Instance.UncompressedPublicKeyLength];
        Secp256k1.Instance.MakeUncompressedPublicKey(Convert.FromHexString(privateKey), publicKey);

        Assert.Equal(Convert.FromHexString(expectedPublicKey), publicKey);
    }

    [Theory]
    [InlineData(0u, "0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266")]
    [InlineData(1u, "0x70997970C51812dc3A010C7d01b50e0d17dc79C8")]
    [InlineData(2u, "0x3C44CdDdB6a900fa2b585dd299e03d12FA4293BC")]
    public void Should_Derive_Address_From_Mnemonic(uint accountIndex, string expectedAddress)
    {
        var wallet = new EtherHdWallet(HARDHAT_MNEMONIC, accountIndex);

        Assert.Equal(expectedAddress, wallet.Address.ToEIP55String());
    }

    [Fact]
    public void Should_Derive_Same_Address_From_Custom_DerivationPath()
    {
        var wallet = new EtherHdWallet(HARDHAT_MNEMONIC, "m/44'/60'/0'/0/1");

        Assert.Equal("0x70997970C51812dc3A010C7d01b50e0d17dc79C8", wallet.Address.ToEIP55String());
    }

    [Fact]
    public async Task SignAsync_Should_Produce_Expected_Signature()
    {
        IEtherSigner wallet = new EtherHdWallet(Convert.FromHexString(WEB3_PRIVATE_KEY));

        var signature = await wallet.SignAsync(Bytes32.Parse(WEB3_MESSAGE_HASH));

        Assert.Equal(Bytes32.Parse(WEB3_R), signature.R);
        Assert.Equal(Bytes32.Parse(WEB3_S), signature.S);
    }

    [Fact]
    public async Task SignRecoverableAsync_Should_Produce_Expected_Signature()
    {
        IEtherSigner wallet = new EtherHdWallet(Convert.FromHexString(WEB3_PRIVATE_KEY));

        var signature = await wallet.SignRecoverableAsync(Bytes32.Parse(WEB3_MESSAGE_HASH));

        Assert.Equal(Bytes32.Parse(WEB3_R), signature.R);
        Assert.Equal(Bytes32.Parse(WEB3_S), signature.S);
        Assert.Equal(1, signature.RecoveryId);
    }

    [Fact]
    public async Task SignPersonalMessageAsync_Should_Match_Web3_Signature()
    {
        IEtherSigner wallet = new EtherHdWallet(Convert.FromHexString(WEB3_PRIVATE_KEY));

        var signature = await wallet.SignPersonalMessageAsync("Some data");

        Assert.Equal(Bytes32.Parse(WEB3_R), signature.R);
        Assert.Equal(Bytes32.Parse(WEB3_S), signature.S);
        Assert.Equal(28, signature.RecoveryId);
    }

    [Fact]
    public async Task SignAsync_Should_Produce_Signature_Verifiable_By_PublicKey()
    {
        byte[] privateKey = Convert.FromHexString(WEB3_PRIVATE_KEY);
        IEtherSigner wallet = new EtherHdWallet(privateKey);
        var hash = Bytes32.Parse(WEB3_MESSAGE_HASH);

        var signature = await wallet.SignAsync(hash);

        byte[] publicKey = new byte[Secp256k1.Instance.UncompressedPublicKeyLength];
        Secp256k1.Instance.MakeUncompressedPublicKey(privateKey, publicKey);
        byte[] signatureBytes = new byte[64];
        signature.CopyTo(signatureBytes);

        Assert.True(Secp256k1.Instance.Verify(publicKey, hash.ToArray(), signatureBytes));
    }

    [Fact]
    public async Task SignTransactionAsync_Should_Match_EIP155_Example()
    {
        IEtherSigner wallet = new EtherHdWallet(Convert.FromHexString("4646464646464646464646464646464646464646464646464646464646464646"));
        var transaction = new LegacyTransaction(
            ChainId: 1,
            Gas: 21000,
            Nonce: 9,
            Input: ITxInput.ForEthTransfer(
                Address.FromString("0x3535353535353535353535353535353535353535"),
                new UInt256(1_000_000_000_000_000_000UL)
            ),
            GasPrice: new UInt256(20_000_000_000UL)
        );

        var signedTransaction = await wallet.SignTransactionAsync(transaction);

        Assert.Equal(
            "0xf86c098504a817c800825208943535353535353535353535353535353535353535880de0b6b3a76400008025a028ef61340bd939bc2195fe537567866003e1a15d3c71ff63e1590620aa636276a067cbe9d8997f761aecb703304b3800ccf555c9f3dc64214b297fb1966a3b6d83",
            signedTransaction.EncodedTx
        );
    }
}
