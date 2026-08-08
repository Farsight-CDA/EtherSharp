namespace EtherSharp.Types;

/// <summary>
/// Represents account state changed by a traced call.
/// </summary>
/// <param name="Pre">State before the call for modified or deleted accounts.</param>
/// <param name="Post">State after the call for modified or created accounts.</param>
public sealed record PrestateDiffTrace(
    IReadOnlyDictionary<Address, PrestateAccount> Pre,
    IReadOnlyDictionary<Address, PrestateAccount> Post
);
