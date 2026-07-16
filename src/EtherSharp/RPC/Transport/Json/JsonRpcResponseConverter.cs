using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport.Json;

internal sealed class JsonRpcResponseConverter<TResult> : JsonConverter<JsonRpcResponse<TResult>>
{
    public override JsonRpcResponse<TResult> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected JSON-RPC response object.");
        }

        int? id = null;
        TResult? result = default;
        bool resultIsNull = false;
        RpcError? error = null;
        bool hasId = false;
        bool hasJsonrpc = false;
        bool hasResult = false;

        while(reader.Read())
        {
            if(reader.TokenType == JsonTokenType.EndObject)
            {
                return !hasId || !hasJsonrpc || (!hasResult && error is null)
                    ? throw new JsonException("JSON-RPC response is missing required properties.")
                    : new JsonRpcResponse<TResult>(id, result, resultIsNull, error);
            }

            if(reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected JSON-RPC response property name.");
            }

            if(reader.ValueTextEquals("id"u8))
            {
                ReadPropertyValue(ref reader);
                hasId = true;
                id = reader.TokenType == JsonTokenType.Null ? null : reader.GetInt32();
            }
            else if(reader.ValueTextEquals("result"u8))
            {
                ReadPropertyValue(ref reader);
                hasResult = true;

                if(reader.TokenType == JsonTokenType.Null)
                {
                    resultIsNull = true;
                }
                else if(error is not null)
                {
                    reader.Skip();
                }
                else
                {
                    result = JsonSerializer.Deserialize<TResult>(ref reader, options);
                    resultIsNull = result is null;
                }
            }
            else if(reader.ValueTextEquals("error"u8))
            {
                ReadPropertyValue(ref reader);
                error = reader.TokenType == JsonTokenType.Null
                    ? null
                    : JsonSerializer.Deserialize<RpcError>(ref reader, options);
            }
            else if(reader.ValueTextEquals("jsonrpc"u8))
            {
                ReadPropertyValue(ref reader);
                hasJsonrpc = true;
                reader.Skip();
            }
            else
            {
                ReadPropertyValue(ref reader);
                reader.Skip();
            }
        }

        throw new JsonException("Unexpected end of JSON-RPC response.");
    }

    private static void ReadPropertyValue(ref Utf8JsonReader reader)
    {
        if(!reader.Read())
        {
            throw new JsonException("Expected JSON-RPC response property value.");
        }
    }

    public override void Write(Utf8JsonWriter writer, JsonRpcResponse<TResult> value, JsonSerializerOptions options)
        => throw new NotSupportedException();
}
