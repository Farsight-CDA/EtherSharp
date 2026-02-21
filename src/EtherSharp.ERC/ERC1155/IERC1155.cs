using EtherSharp.Contract;

namespace EtherSharp.ERC.ERC1155;

[AbiFile("erc1155.abi.json")]
/// <summary>
/// ERC-1155 token contract interface.
/// </summary>
public partial interface IERC1155 : IEVMContract;
