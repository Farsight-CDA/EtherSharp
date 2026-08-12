using EtherSharp.Tx;

namespace EtherSharp.Types;

/// <summary>
/// Access list and gas estimate returned by <c>eth_createAccessList</c>.
/// </summary>
/// <param name="AccessList">State entries accessed while simulating the transaction.</param>
/// <param name="GasUsed">Gas used by the simulated transaction.</param>
public sealed record AccessListResult(StateAccess[] AccessList, ulong GasUsed);
