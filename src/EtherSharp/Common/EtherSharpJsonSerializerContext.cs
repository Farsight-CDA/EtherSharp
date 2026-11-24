using EtherSharp.Realtime.Blocks;
using EtherSharp.RPC.Transport.Json;
using EtherSharp.Types;
using System.Text.Json.Serialization;

namespace EtherSharp.Common;

[JsonSerializable(typeof(BlockHeader))]
[JsonSerializable(typeof(Log))]
[JsonSerializable(typeof(Bytes32))]
[JsonSerializable(typeof(RpcError))]
internal sealed partial class EtherSharpJsonSerializerContext : JsonSerializerContext;
