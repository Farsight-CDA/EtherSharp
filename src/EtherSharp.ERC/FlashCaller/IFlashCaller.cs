using EtherSharp.Contract;

namespace EtherSharp.ERC.FlashCaller;

/// <summary>
/// Flash caller contract interface.
/// </summary>
[AbiFile("flash-caller.abi.json")]
[BytecodeFile("flash-caller.bytecode")]
public partial interface IFlashCaller : IEVMContract;
