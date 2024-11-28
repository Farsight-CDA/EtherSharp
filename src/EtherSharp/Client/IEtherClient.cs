﻿using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client;
public interface IEtherClient
{
    public ulong ChainId { get; }

    public Task InitializeAsync();

    public Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber targetHeight = default);

    public Task<uint> GetTransactionCount(string address, TargetBlockNumber targetHeight = default);

    public TContract Contract<TContract>(string address)
        where TContract : IContract;

    public Task<T> CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight = default);

}
