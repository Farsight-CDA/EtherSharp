using EtherSharp.Contract;

namespace EtherSharp.ERC.ERC20;

/// <summary>
/// ERC-20 token contract interface.
/// </summary>
[AbiFile("erc20.abi.json")]
public partial interface IERC20 : IEVMContract;
