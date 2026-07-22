using System.Collections.Immutable;

namespace EtherSharp.Generator.ABI;

internal readonly struct AdditionalFileMatch(int count, string? content) : IEquatable<AdditionalFileMatch>
{
    public int Count { get; } = count;
    public string? Content { get; } = content;

    public static AdditionalFileMatch Resolve(
        string? fileName, ImmutableDictionary<string, ImmutableArray<string?>> additionalFilesByName)
        => fileName is null || !additionalFilesByName.TryGetValue(fileName, out var files) || files.IsDefaultOrEmpty
            ? default
            : new AdditionalFileMatch(files.Length, files.Length == 1 ? files[0] : null);

    public bool Equals(AdditionalFileMatch other)
        => Count == other.Count && Content == other.Content;

    public override bool Equals(object? obj)
        => obj is AdditionalFileMatch other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Count, Content);
}
