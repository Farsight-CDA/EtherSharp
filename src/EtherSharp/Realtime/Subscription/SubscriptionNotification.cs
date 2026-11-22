using System.Text;
using System.Text.Json;

namespace EtherSharp.Realtime.Subscription;

/// <summary>
/// The classified result of an <c>eth_subscription</c> notification.
/// </summary>
internal readonly ref struct SubscriptionNotification(
    SubscriptionNotification.Kind type, ReadOnlySpan<byte> result = default, string? error = null
)
{
    public enum Kind
    {
        Malformed,
        Result,
        Terminated,
    }

    public Kind Type { get; } = type;
    public ReadOnlySpan<byte> Result { get; } = result;
    public string? Error { get; } = error;

    public static SubscriptionNotification Parse(ReadOnlySpan<byte> payload)
    {
        var reader = new Utf8JsonReader(payload);

        if(!TryReadToParams(ref reader))
        {
            return new SubscriptionNotification(Kind.Malformed);
        }

        while(reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            bool isResult = reader.ValueTextEquals("result"u8);
            bool isError = !isResult && reader.ValueTextEquals("error"u8);
            reader.Read();

            if(isResult)
            {
                return ReadResult(ref reader, payload);
            }

            if(isError && reader.TokenType != JsonTokenType.Null)
            {
                return new SubscriptionNotification(Kind.Terminated, error: ReadErrorMessage(ref reader, payload));
            }

            reader.Skip();
        }

        return new SubscriptionNotification(Kind.Malformed);
    }

    private static SubscriptionNotification ReadResult(scoped ref Utf8JsonReader reader, ReadOnlySpan<byte> payload)
    {
        int resultStart = (int) reader.TokenStartIndex;

        switch(reader.TokenType)
        {
            case JsonTokenType.Null:
                return new SubscriptionNotification(Kind.Terminated);
            case JsonTokenType.StartObject:
                bool isEmpty = true;

                while(reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    isEmpty = false;
                    bool isError = reader.ValueTextEquals("error"u8);
                    reader.Read();

                    if(isError)
                    {
                        return new SubscriptionNotification(Kind.Terminated, error: ReadErrorMessage(ref reader, payload));
                    }

                    reader.Skip();
                }

                return isEmpty
                    ? new SubscriptionNotification(Kind.Terminated)
                    : new SubscriptionNotification(Kind.Result, payload[resultStart..(int) reader.BytesConsumed]);
            default:
                reader.Skip();
                return new SubscriptionNotification(Kind.Result, payload[resultStart..(int) reader.BytesConsumed]);
        }
    }

    private static bool TryReadToParams(ref Utf8JsonReader reader)
    {
        if(!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
        {
            return false;
        }

        while(reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            bool isParams = reader.ValueTextEquals("params"u8);
            reader.Read();

            if(isParams)
            {
                return reader.TokenType == JsonTokenType.StartObject;
            }

            reader.Skip();
        }

        return false;
    }

    private static string ReadErrorMessage(ref Utf8JsonReader reader, ReadOnlySpan<byte> payload)
    {
        if(reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString()!;
        }

        int start = (int) reader.TokenStartIndex;
        reader.Skip();
        return Encoding.UTF8.GetString(payload[start..(int) reader.BytesConsumed]);
    }
}
