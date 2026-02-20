using System.Diagnostics;

namespace EtherSharp.Common;

internal record InstrumentationOptions(
    string InstrumentNamePrefix,
    TagList Tags
);