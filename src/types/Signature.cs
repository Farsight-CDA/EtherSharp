using System.Numerics;

namespace EVM.net.types;
public record Signature(uint V, BigInteger R, BigInteger S);

