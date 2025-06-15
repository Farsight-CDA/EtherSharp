namespace EtherSharp.Common;
internal record InstrumentationOptions(
    string InstrumentNamePrefix,
    IReadOnlyDictionary<string, object?>? Tags
);