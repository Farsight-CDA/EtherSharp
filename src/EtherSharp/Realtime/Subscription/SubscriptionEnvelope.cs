namespace EtherSharp.Realtime.Subscription;

internal record struct SubscriptionEnvelope<TResult>(SubscriptionResult<TResult> Params);
