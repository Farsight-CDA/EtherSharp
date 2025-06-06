﻿using EtherSharp.Contract;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.EtherApi;
public interface IEtherApi
{
    public Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber blockNumber = default);
    public Task<BigInteger> GetBalanceAsync(IEVMContract contract, TargetBlockNumber blockNumber = default);
}
