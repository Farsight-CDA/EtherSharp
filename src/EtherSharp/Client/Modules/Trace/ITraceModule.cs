﻿using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Trace;
public interface ITraceModule
{
    public Task TraceCallTracesAsync(ITxInput call, TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default);
}
