# EtherSharp

<p align="center">
  <img src="resources/logo.png" alt="EtherSharp Logo" width="200" />
</p>

EtherSharp is a high-performance, type-safe .NET library for interacting with Ethereum and other EVM-based blockchains. It uses source generation to turn ABI definitions into strongly-typed contract APIs.

## Key Features

- **Source-Generated Contracts**: Generate strongly-typed methods, events, and errors directly from ABI files.
- **High Performance**: Optimized for low-latency and high-throughput workloads.
- **Query Batching**: Combine multiple reads into a single logical query to reduce RPC round trips.
- **Transaction Lifecycle Support**: Built-in nonce management, gas estimation, and confirmation handling.
- **Flash Calling**: Simulate a temporary deployment plus call in one `eth_call` flow.
- **Realtime Support**: Subscribe to contract events and new block headers over WebSocket transports.
- **Operational Tooling**: Access debug/trace modules and add custom RPC middleware when needed.

---

## Getting Started

### Creating & Configuring a Client

`EtherClientBuilder` provides a fluent API for configuring both read-only and transaction-capable clients.

```csharp
using EtherSharp.Client;
using EtherSharp.Wallet;

// Read-only client (calls, queries, logs)
var readClient = EtherClientBuilder
    .CreateForHttpRpc("https://mainnet.infura.io/v3/YOUR_PROJECT_ID")
    .BuildReadClient();

await readClient.InitializeAsync();

// Transaction client (signing + publishing)
var wallet = new EtherHdWallet("your mnemonic...");
var txClient = EtherClientBuilder
    .CreateForHttpRpc("https://mainnet.infura.io/v3/YOUR_PROJECT_ID", wallet)
    .BuildTxClient();

await txClient.InitializeAsync();
```

### Generating Contract Interfaces

Define a partial interface and decorate it with `[AbiFile]`. Add `[BytecodeFile]` if you also want generated deployment helpers and flash-call deployment payloads.

```csharp
using EtherSharp.Contract;

[AbiFile("abis/erc20.json")]
[BytecodeFile("bytecodes/erc20.bytecode")] // Needed for deployment and flash calling
public partial interface IERC20 : IEVMContract;
```

### Calling Contracts

For **view/pure** functions, use generated async methods (for example, `BalanceOfAsync`).
For write functions that you want to simulate without broadcasting, build the call and run it with `CallAsync`.

```csharp
var erc20 = readClient.Contract<IERC20>("0x123...abc");

// Direct View call
UInt256 balance = await erc20.BalanceOfAsync("0xuser...");

// Simulating a state-changing method
var transferCall = erc20.Transfer("0xreceiver...", 1000);
bool canTransfer = await readClient.CallAsync(transferCall);
```

### Sending Transactions

`PrepareTxAsync` reserves a nonce and prepares a pending transaction handler. `PublishAndConfirmAsync` publishes and waits for confirmation, with a callback for retry/cancel policy.

```csharp
// Prepare transaction (reserves a nonce)
var erc20Tx = txClient.Contract<IERC20>("0x123...abc");
var txHandler = await txClient.PrepareTxAsync(erc20Tx.Transfer("0xreceiver...", 1000));

// Publish and wait for confirmation with custom retry logic
var result = await txHandler.PublishAndConfirmAsync(
    (err, builder, submission) => 
    {
        // Strategy for handling failures (e.g., gas price too low)
        return builder.MinimalGasFeeIncrement(); 
    }
);

// Check result using pattern matching
if (result is TxConfirmationResult.Success success) 
{
    Console.WriteLine($"Transaction confirmed: {success.Receipt.TransactionHash}");
}
```

### Native ETH Transfers

You can also build native ETH transfers without creating a contract call.

```csharp
var transferInput = txClient.ETH.Transfer("0xreceiver...", 1_000_000_000_000_000_000UL);
var transferHandler = await txClient.PrepareTxAsync(transferInput);

var transferResult = await transferHandler.PublishAndConfirmAsync(
    (err, builder, submission) => builder.MinimalGasFeeIncrement()
);
```

### Query API

The Query API combines multiple operations into one typed query execution.

#### Combining Simple Queries
Combine contract calls and chain state reads into one query.

```csharp
string user = "0xuser...";

var (balance, ethBalance, blockNumber) = await readClient.QueryAsync(
    IQuery.Call(IERC20.Functions.BalanceOf.Create(erc20.Address, user)),
    IQuery.GetBalance(user),
    IQuery.GetBlockNumber()
);
```

#### Other Query Types
Built-in operations include:

| Query Type | Description |
| :--- | :--- |
| `IQuery.GetBalance(address)` | Gets the ETH balance of an account |
| `IQuery.GetBlockNumber()` | Gets the current block number |
| `IQuery.GetBlockTimestamp()` | Gets the current block timestamp |
| `IQuery.GetCode(address)` | Gets the contract code at an address |
| `IQuery.CallAndMeasureGas(call)` | Executes a call and returns both result and gas used |
| `IQuery.SafeCall(call)` | Returns a `QueryResult<T>` that gracefully handles reverts |

#### Using QueryBuilder
For custom shaping across many results, use `QueryBuilder`.

```csharp
var query = new QueryBuilder<TokenInfo>()
    .AddQuery(
        IQuery.Call(IERC20.Functions.Name.Create(erc20.Address)),
        IQuery.Call(IERC20.Functions.BalanceOf.Create(erc20.Address, user)),
        (name, balance) => new TokenInfo(name, balance)
    );

// Returns List<TokenInfo>
var results = await readClient.QueryAsync(query);
```

### Flash Calling

Flash calling executes a temporary deployment plus a follow-up call in a single simulation.

```csharp
var simulationResult = await readClient.FlashCallAsync(
    IFlashCaller.Functions.Constructor.Create(), // Deployment payload
    IFlashCaller.Functions.DeploymentHeight.Create(Address.Zero) // Target address is ignored in flash context
);
```

### Logs & Events

EtherSharp includes APIs for historical log retrieval and real-time subscriptions.

#### Fetching Logs from a Contract
Use the generated `Events` property on a contract instance to query a specific event type.

```csharp
var erc20 = readClient.Contract<IERC20>("0x123...");

// Fetch all Transfer events for this contract within a block range
var transfers = await erc20.Events.TransferEvent.GetAllAsync(
    fromBlock: TargetBlockNumber.Height(18000000),
    toBlock: TargetBlockNumber.Latest
);

foreach (var transfer in transfers)
{
    Console.WriteLine($"{transfer.From} -> {transfer.To}: {transfer.Value}");
}
```

#### Multi-Contract and Generic Queries
Query across multiple contracts or event topics using the client-level `Events()` module.

```csharp
var logs = await readClient.Events()
    .HasContracts(usdt, usdc)
    .HasTopics(0, IERC20.Logs.TransferEvent.TopicHex, IERC20.Logs.ApprovalEvent.TopicHex)
    .GetAllAsync();

// Manually match and decode raw logs using TryDecode
foreach (var log in logs)
{
    if (IERC20.Logs.TransferEvent.TryDecode(log, out var transfer))
    {
        Console.WriteLine($"Transfer on {log.Address}: {transfer.Value}");
    }
    else if (IERC20.Logs.ApprovalEvent.TryDecode(log, out var approval))
    {
        Console.WriteLine($"Approval on {log.Address}: {approval.Owner} allowed {approval.Spender}");
    }
}
```

#### Real-time Subscriptions
Subscribe to live events using `CreateSubscriptionAsync` (requires a WebSocket transport).

```csharp
var wsClient = EtherClientBuilder
    .CreateForWebsocket("wss://your-rpc-endpoint")
    .BuildReadClient();

await wsClient.InitializeAsync();
```

```csharp
var liveErc20 = wsClient.Contract<IERC20>("0x123...");
await using var subscription = await liveErc20.Events.TransferEvent.CreateSubscriptionAsync();

await foreach (var transfer in subscription.ListenAsync())
{
    Console.WriteLine($"New Transfer: {transfer.From} -> {transfer.To}");
}
```

#### Polling with Event Filters
If you prefer polling over streaming subscriptions, create a filter and fetch incremental changes.

```csharp
await using var filter = await erc20.Events.TransferEvent.CreateFilterAsync();
var changes = await filter.GetChangesAsync();
```

### Block Subscriptions

You can subscribe to new block headers from the `Blocks` module (WebSocket transport required).

```csharp
await using var blockSub = await wsClient.Blocks.SubscribeNewHeadsAsync();

await foreach (var block in blockSub.ListenAsync())
{
    Console.WriteLine($"New block: {block.Number}");
}
```

### Debug & Trace APIs

For node-level diagnostics, EtherSharp exposes trace/debug modules when the connected RPC endpoint supports them.

```csharp
var callTrace = await txClient.Trace.TraceTransactionCallsAsync("0xtransactionHash...");
var debugTrace = await txClient.Debug.TraceTransactionCallsAsync("0xtransactionHash...");
```

---

## License

EtherSharp is licensed under the MIT License.
