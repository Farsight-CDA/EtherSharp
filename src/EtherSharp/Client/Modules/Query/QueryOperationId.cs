namespace EtherSharp.Client.Modules.Query;

internal enum QueryOperationId : byte
{
    Call = 0,
    CallAndMeasureGas = 1,

    GetCode = 130,
    GetCodeHash = 131,

    GetBlockNumber = 140,
    GetBlockTimestamp = 141,
    GetBlockGasLimit = 142,
    GetBlockGasPrice = 143,
    GetBlockBaseFee = 144,

    GetBalance = 150,

    GetChainId = 160,

    FlashCall = 170
}
