using EtherSharp.Crypto;
using System.Numerics;

namespace EtherSharp.StateOverride;
public sealed class ContractStateOverrideBuilder
{
    private readonly Dictionary<string, string> _slots = [];

    public ContractStateOverrideBuilder AddMapping(BigInteger mappingSlot, Action<MappingStateOverrideBuilder> mappingAction)
    {
        byte[] buffer = new byte[32];
        mappingSlot.TryWriteBytes(buffer, out _, true, false);
        buffer.AsSpan().Reverse();

        var innerBuilder = new MappingStateOverrideBuilder((prefix, value) =>
        {
            Span<byte> slotHashInput = stackalloc byte[buffer.Length + prefix.Length];

            prefix.CopyTo(slotHashInput);
            buffer.CopyTo(slotHashInput[prefix.Length..]);

            string slot = $"0x{Convert.ToHexString(Keccak256.HashData(slotHashInput))}";

            if(_slots.ContainsKey(slot))
            {
                throw new InvalidOperationException($"Slot {slot} already configured");
            }

            _slots[slot] = $"0x{Convert.ToHexString(value)}";
        });

        mappingAction(innerBuilder);

        return this;
    }

    internal OverrideAccount Build()
        => new OverrideAccount(
            null,
            null,
            null,
            null,
            _slots,
            null
        );
}
