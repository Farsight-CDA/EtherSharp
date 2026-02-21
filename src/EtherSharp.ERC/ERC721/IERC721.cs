using EtherSharp.Contract;

namespace EtherSharp.ERC.ERC721;

[AbiFile("erc721.abi.json")]
/// <summary>
/// ERC-721 token contract interface.
/// </summary>
public partial interface IERC721 : IEVMContract;
