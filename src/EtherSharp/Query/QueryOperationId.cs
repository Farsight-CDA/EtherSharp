namespace EtherSharp.Query;

internal enum QueryOperationId : byte
{
    Call = 0,
    CallAndMeasureGas = 1,
    FlashCall = 2,

    GetCode = 10,
    GetCodeHash = 11,

    GetChainId = 20,
    GetBlockNumber = 21,
    GetBlockTimestamp = 22,
    GetBlockGasLimit = 23,
    GetBlockGasPrice = 24,
    GetBlockBaseFee = 25,

    GetBalance = 30
}
