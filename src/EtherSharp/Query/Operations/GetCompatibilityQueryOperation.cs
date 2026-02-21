using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal class GetCompatibilityQueryOperation : IQuery<CompatibilityReport>
{
    private static readonly IContractDeployment _push0 = IContractDeployment.Create(new EVMByteCode(Convert.FromHexString("600b380380600b3d393df35f")), 0);
    private static readonly IContractDeployment _mCopy = IContractDeployment.Create(new EVMByteCode(Convert.FromHexString("600b380380600b3d393df36001600060015e")), 0);
    private static readonly IContractDeployment _tStore = IContractDeployment.Create(new EVMByteCode(Convert.FromHexString("600b380380600b3d393df3600060005d")), 0);
    private static readonly IContractDeployment _baseFee = IContractDeployment.Create(new EVMByteCode(Convert.FromHexString("600b380380600b3d393df348")), 0);

    private static readonly IContractCall<bool> _call = new TxInput<bool>(null!, 0, Array.Empty<byte>(), x => true);

    private static readonly IQuery[] _queries = [
        new SafeFlashCallQueryOperation<bool>(_push0, _call),
        new SafeFlashCallQueryOperation<bool>(_mCopy, _call),
        new SafeFlashCallQueryOperation<bool>(_tStore, _call),
        new SafeFlashCallQueryOperation<bool>(_baseFee, _call)
    ];

    IReadOnlyList<IQuery> IQuery<CompatibilityReport>.Queries => _queries;

    CompatibilityReport IQuery<CompatibilityReport>.ReadResultFrom(params scoped ReadOnlySpan<byte[]> queryResults)
        => new CompatibilityReport(
            queryResults[0][0] != 0,
            queryResults[1][0] != 0,
            queryResults[2][0] != 0,
            queryResults[3][0] != 0
        );
}
