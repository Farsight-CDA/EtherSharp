using System.Text.Json.Serialization;

namespace EtherSharp.Types;

[JsonConverter(typeof(JsonStringEnumConverter<CallType>))]
public enum CallType
{
    CALL,
    STATICCALL,
    DELEGATECALL
}
