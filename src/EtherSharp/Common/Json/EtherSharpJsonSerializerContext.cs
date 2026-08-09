using EtherSharp.Realtime.Blocks;
using EtherSharp.Realtime.Subscription;
using EtherSharp.RPC.Transport.Json;
using EtherSharp.Types;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Json;

[JsonSerializable(typeof(BlockHeader))]
[JsonSerializable(typeof(Log))]
[JsonSerializable(typeof(RpcError))]
[JsonSerializable(typeof(SubscriptionEnvelope<BlockHeader>), TypeInfoPropertyName = "BlockHeaderSubscriptionEnvelope")]
[JsonSerializable(typeof(SubscriptionEnvelope<Log>), TypeInfoPropertyName = "LogSubscriptionEnvelope")]
internal sealed partial class EtherSharpJsonSerializerContext : JsonSerializerContext;
