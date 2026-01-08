# EtherSharp

<p align="center">
  <img src="resources/logo.png" alt="EtherSharp Logo" width="200" />
</p>

EtherSharp is a high-performance, type-safe .NET library for interacting with Ethereum and other EVM-based blockchains. It leverages source generation to provide a seamless development experience when working with smart contracts.

## Key Features

- **Source Generated Contract Interfaces**: Type-safe methods, events, and errors generated directly from ABI files.
- **High Performance**: Optimized for low-latency and high-throughput applications.
- **Advanced Query Engine**: Advanced batching that combines multiple queries into minimal JSON-RPC requests.
- **Transaction Management**: Built-in support for gas estimation, nonce management, and transaction lifecycle handling.
- **Flash Calling**: Simulate complex interactions involving contract deployments and calls in a single step.

---

## Getting Started

### Creating & Configuring a Client

The `EtherClientBuilder` provides a fluent API for configuring your Ethereum client.

```csharp
using EtherSharp.Client;
using EtherSharp.Wallet;

// For read-only operations
var readClient = EtherClientBuilder
    .CreateForHttpRpc("https://mainnet.infura.io/v3/YOUR_PROJECT_ID")
    .BuildReadClient();

// For sending transactions
var wallet = EtherHdWallet.Create("your mnemonic...");
var txClient = EtherClientBuilder
    .CreateForHttpRpc("https://mainnet.infura.io/v3/YOUR_PROJECT_ID", wallet)
    .BuildTxClient();

await txClient.InitializeAsync();
```

### Generating Contract Interfaces

Define a partial interface and decorate it with `[AbiFile]`. The source generator handles the rest.

```csharp
using EtherSharp.Contract;

[AbiFile("abis/erc20.json")]
[BytecodeFile("bytecodes/erc20.bin")] // Needed for deployment and flash calling
public partial interface IERC20 : IEVMContract;
```

### Calling Contracts

For **View** or **Pure** methods, use the generated asynchronous methods ending in `Async`. For simulating **State-Changing** methods without sending a transaction, use `CallAsync`.

```csharp
var erc20 = client.Contract<IERC20>("0x123...abc");

// Direct View call
UInt256 balance = await erc20.BalanceOfAsync("0xuser...");

// Simulating a state-changing method
var transferCall = erc20.Transfer("0xreceiver...", 1000);
bool canTransfer = await client.CallAsync(transferCall);
```

### Sending Transactions

`PrepareTxAsync` handles nonce reservation. `PublishAndConfirmAsync` manages the submission and requires a callback to handle errors or retry logic.

```csharp
// Prepare transaction (reserves a nonce)
var txHandler = await client.PrepareTxAsync(erc20.Transfer("0xreceiver...", 1000));

// Publish and wait for confirmation with error handling
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

### Query API

The Query API batches multiple requests into minimal JSON-RPC calls to reduce latency.

#### Combining Simple Queries
Combine contract calls with blockchain state queries in a single request.

```csharp
var (balance, ethBalance, blockNumber) = await client.QueryAsync(
    IQuery.Call(IERC20.Functions.BalanceOf.Create(erc20.Address, user)),
    IQuery.GetBalance(user),
    IQuery.GetBlockNumber()
);
```

#### Other Query Types
EtherSharp supports various built-in query operations:

| Query Type | Description |
| :--- | :--- |
| `IQuery.GetBalance(address)` | Gets the ETH balance of an account |
| `IQuery.GetBlockNumber()` | Gets the current block number |
| `IQuery.GetBlockTimestamp()` | Gets the current block timestamp |
| `IQuery.GetCode(address)` | Gets the contract code at an address |
| `IQuery.CallAndMeasureGas(call)` | Executes a call and returns both result and gas used |
| `IQuery.SafeCall(call)` | Returns a `QueryResult<T>` that gracefully handles reverts |

#### Using QueryBuilder
For complex mapping, use `QueryBuilder` to transform multiple call results into a single object.

```csharp
var query = new QueryBuilder<TokenInfo>()
    .AddQuery(
        IQuery.Call(IERC20.Functions.Name.Create(erc20.Address)),
        IQuery.Call(IERC20.Functions.BalanceOf.Create(erc20.Address, user)),
        (name, balance) => new TokenInfo(name, balance)
    );

// Returns List<TokenInfo>
var results = await client.QueryAsync(query);
```

### Flash Calling

Flash Calling simulates a temporary contract deployment and a subsequent method call in a single `eth_call`.

```csharp
var simulationResult = await client.FlashCallAsync(
    IFlashCaller.Functions.Constructor.Create(), // Deployment payload
    flashCaller.Multicall(mySubQueries)          // Subsequent call
);
```

### Logs & Events

EtherSharp provides a powerful API for querying historical logs and subscribing to real-time events.

#### Fetching Logs from a Contract
Use the generated `Events` property on your contract instance to fetch specific events.

```csharp
var erc20 = client.Contract<IERC20>("0x123...");

// Fetch all Transfer events for this contract within a block range
var transfers = await erc20.Events.Transfer.GetAllAsync(
    fromBlock: TargetBlockNumber.Height(18000000),
    toBlock: TargetBlockNumber.Latest
);

foreach (var transfer in transfers)
{
    Console.WriteLine($"{transfer.From} -> {transfer.To}: {transfer.Value}");
}
```

#### Multi-Contract and Generic Queries
Query logs across multiple contracts or topics using the client's `Events()` module.

```csharp
var logs = await client.Events()
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
await using var subscription = await erc20.Events.Transfer.CreateSubscriptionAsync();

await foreach (var transfer in subscription.ListenAsync())
{
    Console.WriteLine($"New Transfer: {transfer.From} -> {transfer.To}");
}
```

---

## License

EtherSharp is licensed under the MIT License.
