using EtherSharp.Contract;

namespace EtherSharp.ERC.ERC1155;

/// <summary>
/// ERC-1155 token contract interface.
/// </summary>
[AbiFile("erc1155.abi.json")]
public partial interface IERC1155 : IEVMContract;
