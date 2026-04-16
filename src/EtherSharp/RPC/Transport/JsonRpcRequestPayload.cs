using System.Buffers;
using System.Text.Json;

namespace EtherSharp.RPC.Transport;

internal static class JsonRpcRequestPayload
{
    public static byte[] SerializeToUtf8Bytes(int requestId, string method, JsonSerializerOptions options)
        => SerializeRequest(requestId, method, options, static _ => { });

    public static byte[] SerializeToUtf8Bytes<T1>(int requestId, string method, T1 param1, JsonSerializerOptions options)
        => SerializeRequest(requestId, method, options, writer => JsonSerializer.Serialize(writer, param1, options));

    public static byte[] SerializeToUtf8Bytes<T1, T2>(
        int requestId,
        string method,
        T1 param1,
        T2 param2,
        JsonSerializerOptions options
    ) => SerializeRequest(
            requestId,
            method,
            options,
            writer =>
            {
                JsonSerializer.Serialize(writer, param1, options);
                JsonSerializer.Serialize(writer, param2, options);
            }
        );

    public static byte[] SerializeToUtf8Bytes<T1, T2, T3>(
        int requestId,
        string method,
        T1 param1,
        T2 param2,
        T3 param3,
        JsonSerializerOptions options
    ) => SerializeRequest(
            requestId,
            method,
            options,
            writer =>
            {
                JsonSerializer.Serialize(writer, param1, options);
                JsonSerializer.Serialize(writer, param2, options);
                JsonSerializer.Serialize(writer, param3, options);
            }
        );

    private static byte[] SerializeRequest(
        int requestId,
        string method,
        JsonSerializerOptions options,
        Action<Utf8JsonWriter> writeParams
    )
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(options);

        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);

        writer.WriteStartObject();
        writer.WriteNumber("id", requestId);
        writer.WriteString("method", method);
        writer.WritePropertyName("params");
        writer.WriteStartArray();
        writeParams(writer);
        writer.WriteEndArray();
        writer.WriteString("jsonrpc", "2.0");
        writer.WriteEndObject();
        writer.Flush();

        return buffer.WrittenSpan.ToArray();
    }
}
