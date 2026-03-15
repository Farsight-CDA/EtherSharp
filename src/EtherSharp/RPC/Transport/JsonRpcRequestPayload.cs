using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport;

internal static class JsonRpcRequestPayload
{
    [JsonConverter(typeof(Request0Converter))]
    internal readonly record struct Request0(int Id, string Method, string Jsonrpc = "2.0");

    [JsonConverter(typeof(Request1ConverterFactory))]
    internal readonly record struct Request1<T1>(int Id, string Method, T1 Param1, string Jsonrpc = "2.0");

    [JsonConverter(typeof(Request2ConverterFactory))]
    internal readonly record struct Request2<T1, T2>(int Id, string Method, T1 Param1, T2 Param2, string Jsonrpc = "2.0");

    [JsonConverter(typeof(Request3ConverterFactory))]
    internal readonly record struct Request3<T1, T2, T3>(
        int Id,
        string Method,
        T1 Param1,
        T2 Param2,
        T3 Param3,
        string Jsonrpc = "2.0"
    );

    private sealed class Request0Converter : JsonConverter<Request0>
    {
        public override Request0 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, Request0 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            JsonSerializer.Serialize(writer, value.Id, options);
            writer.WriteString("method", value.Method);
            writer.WritePropertyName("params");
            writer.WriteStartArray();
            writer.WriteEndArray();
            writer.WriteString("jsonrpc", value.Jsonrpc);
            writer.WriteEndObject();
        }
    }

    private sealed class Request1ConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Request1<>);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var typeArgs = typeToConvert.GetGenericArguments();
            var converterType = typeof(Request1Converter<>).MakeGenericType(typeArgs[0]);
            return (JsonConverter) Activator.CreateInstance(converterType)!;
        }
    }

    private sealed class Request1Converter<T1> : JsonConverter<Request1<T1>>
    {
        public override Request1<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, Request1<T1> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            JsonSerializer.Serialize(writer, value.Id, options);
            writer.WriteString("method", value.Method);
            writer.WritePropertyName("params");
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, value.Param1, options);
            writer.WriteEndArray();
            writer.WriteString("jsonrpc", value.Jsonrpc);
            writer.WriteEndObject();
        }
    }

    private sealed class Request2ConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Request2<,>);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var typeArgs = typeToConvert.GetGenericArguments();
            var converterType = typeof(Request2Converter<,>).MakeGenericType(typeArgs[0], typeArgs[1]);
            return (JsonConverter) Activator.CreateInstance(converterType)!;
        }
    }

    private sealed class Request2Converter<T1, T2> : JsonConverter<Request2<T1, T2>>
    {
        public override Request2<T1, T2> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, Request2<T1, T2> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            JsonSerializer.Serialize(writer, value.Id, options);
            writer.WriteString("method", value.Method);
            writer.WritePropertyName("params");
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, value.Param1, options);
            JsonSerializer.Serialize(writer, value.Param2, options);
            writer.WriteEndArray();
            writer.WriteString("jsonrpc", value.Jsonrpc);
            writer.WriteEndObject();
        }
    }

    private sealed class Request3ConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Request3<,,>);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var typeArgs = typeToConvert.GetGenericArguments();
            var converterType = typeof(Request3Converter<,,>).MakeGenericType(typeArgs[0], typeArgs[1], typeArgs[2]);
            return (JsonConverter) Activator.CreateInstance(converterType)!;
        }
    }

    private sealed class Request3Converter<T1, T2, T3> : JsonConverter<Request3<T1, T2, T3>>
    {
        public override Request3<T1, T2, T3> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, Request3<T1, T2, T3> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            JsonSerializer.Serialize(writer, value.Id, options);
            writer.WriteString("method", value.Method);
            writer.WritePropertyName("params");
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, value.Param1, options);
            JsonSerializer.Serialize(writer, value.Param2, options);
            JsonSerializer.Serialize(writer, value.Param3, options);
            writer.WriteEndArray();
            writer.WriteString("jsonrpc", value.Jsonrpc);
            writer.WriteEndObject();
        }
    }
}
