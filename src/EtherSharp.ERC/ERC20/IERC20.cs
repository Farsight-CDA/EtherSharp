using EtherSharp.Contract;

namespace EtherSharp.ERC.ERC20;

[AbiFile("erc20.abi.json")]
/// <summary>
/// ERC-20 token contract interface.
/// </summary>
public partial interface IERC20 : IEVMContract;
