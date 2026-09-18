using EtherSharp.Crypto;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Storage;

internal static class BytesExtensions
{
    private static Bytes32 EmptyCodeHashValue { get; } = Keccak256.HashData([]);

    extension(Bytes32)
    {
        public static Bytes32 EmptyCodeHash
            => EmptyCodeHashValue;
    }
}
