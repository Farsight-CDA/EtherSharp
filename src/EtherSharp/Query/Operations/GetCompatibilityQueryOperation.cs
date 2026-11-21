using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class GetCompatibilityQueryOperation : IQuery<CompatibilityReport>
{
    public static GetCompatibilityQueryOperation Instance { get; } = new();

    private static readonly EVMRuntimeCode _push0 = new EVMRuntimeCode(Convert.FromHexString("5f"));
    private static readonly EVMRuntimeCode _mCopy = new EVMRuntimeCode(Convert.FromHexString("6001600060015e"));
    private static readonly EVMRuntimeCode _tStore = new EVMRuntimeCode(Convert.FromHexString("600060005d"));
    private static readonly EVMRuntimeCode _baseFee = new EVMRuntimeCode(Convert.FromHexString("48"));

    private static readonly IFlashCall<bool> _call = IFlashCall.ForRawFlashCall(0, Array.Empty<byte>(), x => true);

    private static readonly IQueryOperation[] _queries = [
        new SafeFlashCallQueryOperation<bool>(_push0, _call),
        new SafeFlashCallQueryOperation<bool>(_mCopy, _call),
        new SafeFlashCallQueryOperation<bool>(_tStore, _call),
        new SafeFlashCallQueryOperation<bool>(_baseFee, _call)
    ];

    private GetCompatibilityQueryOperation()
    {
    }

    int IQuery.OperationCount => _queries.Length;

    void IQuery.AddTo(IQueryPlan plan)
    {
        foreach(var query in _queries)
        {
            plan.Add(query);
        }
    }

    CompatibilityReport IQuery<CompatibilityReport>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => new CompatibilityReport(
            queryResults[0].Span[0] != 0,
            queryResults[1].Span[0] != 0,
            queryResults[2].Span[0] != 0,
            queryResults[3].Span[0] != 0
        );
}
