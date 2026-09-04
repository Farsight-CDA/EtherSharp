namespace EtherSharp.Query;

internal enum QueryOperationId : byte
{
    Call = 0,
    CallAndMeasureGas = 1,
    FlashCall = 2,
    Isolate = 3,

    GetCode = 10,
    GetCodeHash = 11,
    HasCode = 12,

    GetChainId = 20,
    GetBlockNumber = 21,
    GetBlockTimestamp = 22,
    GetBlockGasLimit = 23,
    GetBlockGasPrice = 24,
    GetBlockBaseFee = 25,

    GetBalance = 30,
    ReadStorage = 31,

    GetRemainingGas = 40
}
