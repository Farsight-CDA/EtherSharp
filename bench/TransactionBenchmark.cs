using BenchmarkDotNet.Attributes;
using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Wallet;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Accounts;

namespace EtherSharp.Bench;

[MemoryDiagnoser, ShortRunJob]
public class TransactionBenchmark
{
    private readonly string _privateKey = "9313be12650331b6dd9df5a6da638eeb2b509631192b79204bf8a67ad72f45a1";

    private readonly string adressTo = "0xa85A6945DBFE66D9531Da819DBdf240E18e812eC";

    private Account _account = null!;
    private TransactionInput? transactionInput;
    private EtherHdWallet _wallet = null!;
    private IEtherTxClient _evmRpcClient = null!;
    private TxInput<bool> call = null!;
    private readonly string msg1 = "wee test message 18/09/2017 02:55PM";

    [GlobalSetup]
    public void Setup()
    {
        _account = new Account(_privateKey);

        _wallet = new EtherHdWallet(_privateKey);
        _evmRpcClient = new EtherClientBuilder().WithRPCUrl("https://eth.llamarpc.com").WithSigner(_wallet)
            .BuildTxClient();

    }

    [Benchmark]
    public void NetheriumTX()
    {
        transactionInput = new TransactionInput(msg1, adressTo);
        _ = _account.TransactionManager.SignTransactionAsync(transactionInput);
    }

    [Benchmark]
    public void EtherSharpTX()
    {

        call = new ERC20("0x0d8775f648430679a709e98d2b0cb6250d2887ef").Transfer(adressTo, 100);
        _ = _evmRpcClient.EncodeCallData(call);
    }
}
