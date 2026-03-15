using System.Diagnostics;

namespace EtherSharp.Common;

internal sealed record InstrumentationOptions(
    string InstrumentNamePrefix,
    TagList Tags
);
