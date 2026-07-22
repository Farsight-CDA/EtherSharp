namespace EtherSharp.Crypto;

/// <summary>
/// Marks a partial type for EIP-712 struct hashing source generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class EIP712TypeAttribute : Attribute;
