using EtherSharp.Contract;

namespace EtherSharp.ERC.ERC721;

/// <summary>
/// ERC-721 token contract interface.
/// </summary>
[AbiFile("erc721.abi.json")]
public partial interface IERC721 : IEVMContract;
