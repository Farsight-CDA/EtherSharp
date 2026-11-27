using EtherSharp.Common.Logging;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Common.Extensions;

internal static class ILoggerExtensions
{
    extension(ILogger logger)
    {
        public TxLogger Tx => new(logger);
        public TxWatcherLogger TxWatcher => new(logger);
        public SubscriptionsLogger Subscriptions => new(logger);
        public WssTransportLogger WssTransport => new(logger);
    }
}
