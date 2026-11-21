using EtherSharp.Contract;
using System.Buffers.Binary;
using System.Diagnostics;

namespace EtherSharp.Query;

/// <summary>
/// Constructs a querier runtime containing only explicitly requested operation handlers.
/// </summary>
internal static partial class QuerierCodeBuilder
{
    private readonly record struct CodeRegion(QueryOperationId OperationId, QuerierCodeVariant Variant, int Offset, int Length);
    private readonly record struct JumpReference(int Offset, int Target);

    /// <summary>
    /// Creates a runtime for the supplied unique, implemented operation IDs, in any order.
    /// </summary>
    /// <param name="operationIds">The operations the runtime must handle.</param>
    /// <param name="variant">The backend variant whose handlers are included alongside shared handlers.</param>
    public static EVMRuntimeCode CreateRuntime(ReadOnlySpan<QueryOperationId> operationIds, QuerierCodeVariant variant)
    {
        Span<bool> includedRegions = stackalloc bool[_regions.Length];
        int removedLength = 0;
        for(int i = 0; i < _regions.Length; i++)
        {
            includedRegions[i] = operationIds.Contains(_regions[i].OperationId)
                && (_regions[i].Variant == QuerierCodeVariant.Shared || _regions[i].Variant == variant);
            if(!includedRegions[i])
            {
                removedLength += _regions[i].Length;
            }
        }

        if(removedLength == 0)
        {
            return new EVMRuntimeCode(Template.AsMemory());
        }

        var template = Template.AsSpan();
        byte[] byteCode = new byte[template.Length - removedLength];
        int sourceOffset = 0;
        int outputOffset = 0;

        for(int i = 0; i < _regions.Length; i++)
        {
            if(includedRegions[i])
            {
                continue;
            }

            var region = _regions[i];
            int length = region.Offset - sourceOffset;
            template.Slice(sourceOffset, length).CopyTo(byteCode.AsSpan(outputOffset));
            outputOffset += length;
            sourceOffset = region.Offset + region.Length;
        }

        template[sourceOffset..].CopyTo(byteCode.AsSpan(outputOffset));

        foreach(var reference in _jumpReferences)
        {
            int offset = TranslateOffset(reference.Offset, includedRegions);
            if(offset < 0)
            {
                continue;
            }

            int target = TranslateOffset(reference.Target, includedRegions);
            Debug.Assert(target >= 0, "A retained jump cannot target an omitted operation handler.");
            BinaryPrimitives.WriteUInt16BigEndian(byteCode.AsSpan(offset + 1), checked((ushort) target));
        }

        return new EVMRuntimeCode(byteCode);
    }

    private static int TranslateOffset(int offset, ReadOnlySpan<bool> includedRegions)
    {
        int removedLength = 0;
        for(int i = 0; i < _regions.Length; i++)
        {
            var region = _regions[i];
            if(offset < region.Offset)
            {
                break;
            }
            if(includedRegions[i])
            {
                continue;
            }
            if(offset < region.Offset + region.Length)
            {
                return -1;
            }

            removedLength += region.Length;
        }

        return offset - removedLength;
    }
}
