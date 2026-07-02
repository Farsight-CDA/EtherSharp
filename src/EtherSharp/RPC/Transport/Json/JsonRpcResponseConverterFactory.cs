using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport.Json;

internal sealed class JsonRpcResponseConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(JsonRpcResponse<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var resultType = typeToConvert.GetGenericArguments()[0];
        return (JsonConverter) Activator.CreateInstance(typeof(JsonRpcResponseConverter<>).MakeGenericType(resultType))!;
    }
}
