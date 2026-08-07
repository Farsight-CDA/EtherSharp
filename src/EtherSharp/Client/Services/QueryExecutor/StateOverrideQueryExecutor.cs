using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.QueryExecutor;

internal sealed class StateOverrideQueryExecutor(
    IEthRpcModule ethRpcModule,
    StateOverrideQueryExecutor.Configuration configuration,
    IServiceProvider provider) : QueryExecutorBase(provider)
{
    internal sealed record Configuration(
        Address QuerierAddress,
        int MaxPayloadSize,
        int MaxResultSize)
    {
        public static Address DefaultQuerierAddress { get; }
            = Address.Parse("0x4574686572536861727051756572696572000000");
    }

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly Configuration _configuration = configuration;

    protected override Address? CallAddress => _configuration.QuerierAddress;

    protected override void PreparePlan(QueryPlan plan, QuerierByteCode querier)
        => plan.AddStateOverride(
            _configuration.QuerierAddress,
            new AccountOverride(code: querier.RuntimeCode)
        );

    protected override int GetMaxPayloadSize(QuerierByteCode querier, ulong? gasLimit, TargetHeight targetHeight)
        => _configuration.MaxPayloadSize;

    protected override int GetMaxResultSize(TargetHeight targetHeight)
        => _configuration.MaxResultSize;

    protected override Task<TxCallResult> ExecuteBatchAsync(QuerierByteCode querier, UInt256 value, ReadOnlyMemory<byte> payload,
        ulong? gasLimit, CallOptions options, CancellationToken cancellationToken)
        => _ethRpcModule.CallAsync(
            _configuration.QuerierAddress,
            gasLimit,
            null,
            value,
            payload,
            options,
            cancellationToken
        );
}
