using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherSharp.Realtime;
internal interface ISubscription
{
    public string Id { get; }
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload);
    public Task InstallAsync(CancellationToken cancellationToken = default);
}
