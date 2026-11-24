using System.Runtime.CompilerServices;

namespace EtherSharp.Realtime;

/// <summary>
/// Extensions for consuming subscription event streams.
/// </summary>
public static class SubscriptionEventExtensions
{
    extension<T>(IAsyncEnumerable<SubscriptionEvent<T>> events)
        where T : notnull
    {
        /// <summary>
        /// Yields only the subscription items, skipping <see cref="SubscriptionInterruption"/> markers.
        /// Use this when missing items during an interruption is acceptable.
        /// </summary>
        /// <param name="cancellationToken">Token used to stop listening.</param>
        /// <returns>The subscription items.</returns>
        public async IAsyncEnumerable<T> IgnoreInterruptions([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach(var subscriptionEvent in events.WithCancellation(cancellationToken))
            {
                if(subscriptionEvent is T item)
                {
                    yield return item;
                }
            }
        }
    }
}
