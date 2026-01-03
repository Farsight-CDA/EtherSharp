using EtherSharp.Contract;

namespace EtherSharp.ERC.FlashCaller;

[AbiFile("flash-caller.abi.json")]
[BytecodeFile("flash-caller.bytecode")]
public partial interface IFlashCaller : IEVMContract;
