﻿using EtherSharp.Client.Services.RPC;
using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;

namespace EtherSharp.Client.Services.EtherApi;
internal class EtherApi(IRpcClient rpcClient, IServiceProvider provider) : IEtherTxApi
{
    private readonly IRpcClient _rpcClient = rpcClient;
    private readonly IServiceProvider _provider = provider;

    public TxInput Transfer(Address receiver, BigInteger amount)
        => TxInput.ForEthTransfer(receiver, amount);
    public TxInput Transfer(string receiver, BigInteger amount)
        => TxInput.ForEthTransfer(Address.FromString(receiver), amount);
    public TxInput Transfer(IPayableContract contract, BigInteger amount)
        => TxInput.ForEthTransfer(contract.Address, amount);

    public Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(address.String, blockNumber);
    public Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(address, blockNumber);
    public Task<BigInteger> GetBalanceAsync(IEVMContract contract, TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(contract.Address.String, blockNumber);

    public Task<BigInteger> GetMyBalanceAsync(TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(
            _provider.GetService<IEtherSigner>()?.Address?.String ?? throw new InvalidOperationException("Client is not a tx client"),
            blockNumber
        );
}
