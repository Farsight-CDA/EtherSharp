namespace EtherSharp.Query;

/// <summary>
/// Identifies a low-level operation in the Querier protocol.
/// </summary>
public enum QueryOperationId : byte
{
    /// <summary>Calls an existing account.</summary>
    Call = 0,
    /// <summary>Calls an existing account and measures gas usage.</summary>
    CallAndMeasureGas = 1,
    /// <summary>Executes nested operations and rolls back their state changes.</summary>
    Isolate = 3,
    /// <summary>Executes a successful prefix of contract calls within the available gas budget.</summary>
    PartialRange = 4,
    /// <summary>Calls runtime code at a temporary account shared by all flash calls with the same code in a query.</summary>
    FlashCall = 5,
    /// <summary>Deploys flash call runtime code before any other operation runs, when state overrides are unavailable.</summary>
    DeployFlashCode = 6,

    /// <summary>Reads an account's bytecode.</summary>
    GetCode = 10,
    /// <summary>Reads an account's code hash.</summary>
    GetCodeHash = 11,
    /// <summary>Checks whether an account has bytecode.</summary>
    HasCode = 12,

    /// <summary>Reads the chain identifier.</summary>
    GetChainId = 20,
    /// <summary>Reads the current block number.</summary>
    GetBlockNumber = 21,
    /// <summary>Reads the current block timestamp.</summary>
    GetBlockTimestamp = 22,
    /// <summary>Reads the current block gas limit.</summary>
    GetBlockGasLimit = 23,
    /// <summary>Reads the transaction gas price.</summary>
    GetBlockGasPrice = 24,
    /// <summary>Reads the current block base fee.</summary>
    GetBlockBaseFee = 25,

    /// <summary>Reads an account's ETH balance.</summary>
    GetBalance = 30,
    /// <summary>Reads storage in the current execution context.</summary>
    ReadStorage = 31,
    /// <summary>Probes the nonce in the current execution context through contract creation.</summary>
    GetNonce = 32,

    /// <summary>Reads the remaining execution gas.</summary>
    GetRemainingGas = 40
}
