﻿using BenchmarkDotNet.Attributes;
using EtherSharp.Bench.Common;
using EtherSharp.Client;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.TxConfirmer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Contract;
using EtherSharp.Transport;
using EtherSharp.Wallet;

namespace EtherSharp.Bench;
[MemoryDiagnoser]
[ShortRunJob]
public class EtherClientBenchmarks
{
    private IEtherTxClient _client;

    public EtherClientBenchmarks()
    {
        var wallet = EtherHdWallet.Create();
        _client = EtherClientBuilder
            .CreateForHttpRpc("https://optimism.llamarpc.com", wallet)
            .BuildTxClient();

        _client.InitializeAsync().GetAwaiter().GetResult();
    }

    [Benchmark]
    public IEVMContract CreateContractInstance()
        => _client.Contract<IERC20>("0x94b008aa00579c1307b0ef2c499ad98a8ce58e58");

}