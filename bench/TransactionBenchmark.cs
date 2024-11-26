using BenchmarkDotNet.Attributes;
using EtherSharp.Client;
using EtherSharp.Contract;
using EtherSharp.Wallet;
using Nethereum.ABI;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Model;
using Nethereum.Signer;

namespace EtherSharp.Bench;

[MemoryDiagnoser, ShortRunJob]
public class TransactionBenchmark
{
    private readonly string _privateKey = "9313be12650331b6dd9df5a6da638eeb2b509631192b79204bf8a67ad72f45a1";
    private readonly string _usdtAddress = "0xc2132d05d31c914a87c6611c10748aeb04b58e8f";
    private readonly string _targetAddress = "0xa85A6945DBFE66D9531Da819DBdf240E18e812eC";
    private readonly string _mockRpc = "localhost:1111";

    private readonly IEtherTxClient _evmRpcClient;
    private readonly Transaction1559Signer _signer;
    private readonly EthECKey _ethEcKey;

    public TransactionBenchmark()
    {
        _signer = new Transaction1559Signer();
        _evmRpcClient = new EtherClientBuilder()
            .WithRPCUrl(_mockRpc)
            .WithSigner(new EtherHdWallet(_privateKey))
            .BuildTxClient();
        _ethEcKey = new EthECKey(_privateKey);
    }

    //[Benchmark]
    //public string NetheriumTX()
    //{
    //    var transferFunction = new TransferFunction()
    //    {
    //        To = _usdtAddress,
    //        AmountToSend = 1000000
    //    };

    //    byte[] data = new ABIEncode().GetABIParamsEncoded(transferFunction);
    //    var tx = new Transaction1559(137, 111, 6000000, 6000000, 5000000, _usdtAddress, 0, data.ToHex(), []);

    //    return _signer.SignTransaction(_ethEcKey, tx);
    //}

    //[Benchmark]
    //public string EtherSharpTX()
    //{
    //    var erc20 = new ERC20("0x0d8775f648430679a709e98d2b0cb6250d2887ef");
    //    return _evmRpcClient.EncodeCallData(erc20.Transfer(_targetAddress, 1000000));
    //}
}
