using System.Buffers;
using System.Text.Json;

namespace EtherSharp.RPC.Transport;

internal static class JsonRpcRequestPayload
{
    private const int INITIAL_BUFFER_CAPACITY = 16 * 1024;
    private const int MAX_RETAINED_BUFFER_CAPACITY = 256 * 1024;

    private static readonly JsonEncodedText _id = JsonEncodedText.Encode("id");
    private static readonly JsonEncodedText _method = JsonEncodedText.Encode("method");
    private static readonly JsonEncodedText _params = JsonEncodedText.Encode("params");
    private static readonly JsonEncodedText _jsonrpc = JsonEncodedText.Encode("jsonrpc");
    private static readonly JsonWriterOptions _writerOptions = new JsonWriterOptions()
    {
        SkipValidation = true
    };

    [ThreadStatic]
    private static ReusablePooledByteBufferWriter? _buffer;

    [ThreadStatic]
    private static Utf8JsonWriter? _writer;

    public static byte[] SerializeToUtf8Bytes(int requestId, string method, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var buffer = AcquireBuffer();
        var writer = AcquireWriter(buffer);
        try
        {
            WriteStart(writer, requestId, method, options);
            WriteEnd(writer);
            writer.Flush();

            return buffer.ToArray();
        }
        finally
        {
            ReleaseBuffer(buffer);
        }
    }

    public static byte[] SerializeToUtf8Bytes<T1>(int requestId, string method, T1 param1, JsonSerializerOptions options)
    {
        var buffer = AcquireBuffer();
        var writer = AcquireWriter(buffer);
        try
        {
            WriteStart(writer, requestId, method, options);
            JsonSerializer.Serialize(writer, param1, options);
            WriteEnd(writer);
            writer.Flush();

            return buffer.ToArray();
        }
        finally
        {
            ReleaseBuffer(buffer);
        }
    }

    public static byte[] SerializeToUtf8Bytes<T1, T2>(
        int requestId,
        string method,
        T1 param1,
        T2 param2,
        JsonSerializerOptions options
    )
    {
        var buffer = AcquireBuffer();
        var writer = AcquireWriter(buffer);
        try
        {
            WriteStart(writer, requestId, method, options);
            JsonSerializer.Serialize(writer, param1, options);
            JsonSerializer.Serialize(writer, param2, options);
            WriteEnd(writer);
            writer.Flush();

            return buffer.ToArray();
        }
        finally
        {
            ReleaseBuffer(buffer);
        }
    }

    public static byte[] SerializeToUtf8Bytes<T1, T2, T3>(
        int requestId,
        string method,
        T1 param1,
        T2 param2,
        T3 param3,
        JsonSerializerOptions options
    )
    {
        var buffer = AcquireBuffer();
        var writer = AcquireWriter(buffer);
        try
        {
            WriteStart(writer, requestId, method, options);
            JsonSerializer.Serialize(writer, param1, options);
            JsonSerializer.Serialize(writer, param2, options);
            JsonSerializer.Serialize(writer, param3, options);
            WriteEnd(writer);
            writer.Flush();

            return buffer.ToArray();
        }
        finally
        {
            ReleaseBuffer(buffer);
        }
    }

    private static ReusablePooledByteBufferWriter AcquireBuffer()
    {
        var buffer = _buffer ??= new ReusablePooledByteBufferWriter(INITIAL_BUFFER_CAPACITY);
        buffer.Reset();
        return buffer;
    }

    private static Utf8JsonWriter AcquireWriter(ReusablePooledByteBufferWriter buffer)
    {
        var writer = _writer;

        if(writer is null)
        {
            writer = new Utf8JsonWriter(buffer, _writerOptions);
            _writer = writer;
            return writer;
        }

        writer.Reset(buffer);
        return writer;
    }

    private static void ReleaseBuffer(ReusablePooledByteBufferWriter buffer)
    {
        if(buffer.Capacity > MAX_RETAINED_BUFFER_CAPACITY)
        {
            buffer.Dispose();
            _buffer = null;
        }
    }

    private static void WriteStart(Utf8JsonWriter writer, int requestId, string method, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(_id);
        JsonSerializer.Serialize(writer, requestId, options);
        writer.WriteString(_method, method);
        writer.WritePropertyName(_params);
        writer.WriteStartArray();
    }

    private static void WriteEnd(Utf8JsonWriter writer)
    {
        writer.WriteEndArray();
        writer.WriteString(_jsonrpc, "2.0");
        writer.WriteEndObject();
    }

    private sealed class ReusablePooledByteBufferWriter(int initialCapacity) : IBufferWriter<byte>, IDisposable
    {
        private byte[]? _buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        private int _writtenCount;

        public int Capacity
            => _buffer?.Length ?? 0;

        public void Advance(int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);

            if(_buffer is null || _writtenCount > _buffer.Length - count)
            {
                throw new InvalidOperationException("Cannot advance past the end of the buffer.");
            }

            _writtenCount += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return _buffer.AsMemory(_writtenCount);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return _buffer.AsSpan(_writtenCount);
        }

        public byte[] ToArray()
        {
            ObjectDisposedException.ThrowIf(_buffer is null, this);
            return _buffer.AsSpan(0, _writtenCount).ToArray();
        }

        public void Reset()
        {
            ObjectDisposedException.ThrowIf(_buffer is null, this);
            _writtenCount = 0;
        }

        public void Dispose()
        {
            if(_buffer is null)
            {
                return;
            }

            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = null;
            _writtenCount = 0;
        }

        private void EnsureCapacity(int sizeHint)
        {
            ObjectDisposedException.ThrowIf(_buffer is null, this);

            if(sizeHint <= 0)
            {
                sizeHint = 1;
            }

            if(sizeHint <= _buffer.Length - _writtenCount)
            {
                return;
            }

            int newCapacity = Math.Max(_buffer.Length * 2, _writtenCount + sizeHint);
            byte[] newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);
            _buffer.AsSpan(0, _writtenCount).CopyTo(newBuffer);
            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = newBuffer;
        }
    }
}
