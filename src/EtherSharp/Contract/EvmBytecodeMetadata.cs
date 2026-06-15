using System.Buffers.Binary;

namespace EtherSharp.Contract;

internal static class EvmBytecodeMetadata
{
    public static ReadOnlyMemory<byte> GetExecutableByteCode(ReadOnlyMemory<byte> byteCode)
        => TryGetMetadataStart(byteCode, out int metadataStart) ? byteCode[..metadataStart] : byteCode;

    private static bool TryGetMetadataStart(ReadOnlyMemory<byte> byteCode, out int metadataStart)
    {
        metadataStart = 0;

        if(byteCode.Length < 3)
        {
            return false;
        }

        int metadataLength = BinaryPrimitives.ReadUInt16BigEndian(byteCode.Span[^2..]);
        if(metadataLength == 0)
        {
            return false;
        }

        metadataStart = byteCode.Length - metadataLength - 2;
        return metadataStart >= 0 && LooksLikeCompilerMetadata(byteCode.Span.Slice(metadataStart, metadataLength));
    }

    private static bool LooksLikeCompilerMetadata(ReadOnlySpan<byte> metadata)
        => metadata.Length > 0
            && (metadata[0] >> 5) == 5
            && (metadata.IndexOf("solc"u8) != -1
            || metadata.IndexOf("vyper"u8) != -1
            || metadata.IndexOf("ipfs"u8) != -1
            || metadata.IndexOf("bzzr0"u8) != -1
            || metadata.IndexOf("bzzr1"u8) != -1);
}
