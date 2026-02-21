using EtherSharp.Contract;

namespace EtherSharp.ERC.ERC721;

/// <summary>
/// ERC-4626 vault contract interface.
/// </summary>
[AbiFile("erc4626.abi.json")]
public partial interface IERC4626 : IEVMContract;
