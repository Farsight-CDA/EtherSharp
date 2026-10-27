using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI <c>bool</c> value.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class Bool : FixedType<bool>, IPackedEncodeType
    {
        /// <summary>
        /// Gets the shared ABI boolean wrapper for <see langword="true"/>.
        /// </summary>
        public static Bool True { get; } = new Bool(true);

        /// <summary>
        /// Gets the shared ABI boolean wrapper for <see langword="false"/>.
        /// </summary>
        public static Bool False { get; } = new Bool(false);

        /// <inheritdoc />
        public int PackedSize => 1;

        private Bool(bool value) : base(value) { }

        /// <inheritdoc />
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        /// <summary>
        /// Encodes a boolean value.
        /// </summary>
        public static void EncodeInto(bool value, Span<byte> buffer)
            => buffer[^1] = value ? (byte) 1 : (byte) 0;

        /// <summary>
        /// Decodes a boolean from an ABI word.
        /// </summary>
        public static bool Decode(ReadOnlySpan<byte> bytes)
            => bytes[^1] == 1;
    }
}
