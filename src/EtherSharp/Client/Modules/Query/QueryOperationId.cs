namespace EtherSharp.Client.Modules.Query;

internal enum QueryOperationId : byte
{
    //First 128 are reserved for call
    GetCode = 130,
    GetCodeHash = 131,

    GetBlockNumber = 140,
    GetBlockTimestamp = 141,
    GetBlockGasLimit = 142,
    GetBlockGasPrice = 143,

    GetBalance = 150,

    GetChainId = 160,

    FlashCall = 170
}
