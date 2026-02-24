using BenchmarkDotNet.Attributes;
using EtherSharp.Common;
using EtherSharp.RPC.Transport;
using System.Text.Json;

namespace EtherSharp.Bench.Rpc;

[MemoryDiagnoser]
public class RpcRequestSerializationBenchmarks
{
    private static readonly JsonSerializerOptions _options = ParsingUtils.EvmSerializerOptions;

    [Benchmark]
    public byte[] Serialize_Arity0()
        => JsonSerializer.SerializeToUtf8Bytes(
            new JsonRpcRequestPayload.Request0(1, "eth_test"),
            _options
        );

    [Benchmark]
    public byte[] Serialize_Arity1()
        => JsonSerializer.SerializeToUtf8Bytes(
            new JsonRpcRequestPayload.Request1<string>(1, "eth_test", "0xabc"),
            _options
        );

    [Benchmark]
    public byte[] Serialize_Arity2()
        => JsonSerializer.SerializeToUtf8Bytes(
            new JsonRpcRequestPayload.Request2<string, int>(1, "eth_test", "0xabc", 7),
            _options
        );

    [Benchmark]
    public byte[] Serialize_Arity3()
        => JsonSerializer.SerializeToUtf8Bytes(
            new JsonRpcRequestPayload.Request3<string, int, bool>(1, "eth_test", "0xabc", 7, true),
            _options
        );
}
