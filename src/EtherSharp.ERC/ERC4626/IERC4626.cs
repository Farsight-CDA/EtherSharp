using EtherSharp.Contract;

namespace EtherSharp.ERC.ERC721;

[AbiFile("erc4626.abi.json")]
/// <summary>
/// ERC-4626 vault contract interface.
/// </summary>
public partial interface IERC4626 : IEVMContract;
