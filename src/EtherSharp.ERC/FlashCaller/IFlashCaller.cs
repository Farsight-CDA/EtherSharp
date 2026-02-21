using EtherSharp.Contract;

namespace EtherSharp.ERC.FlashCaller;

[AbiFile("flash-caller.abi.json")]
[BytecodeFile("flash-caller.bytecode")]
/// <summary>
/// Flash caller contract interface.
/// </summary>
public partial interface IFlashCaller : IEVMContract;
