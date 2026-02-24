using EtherSharp.Numerics;
using System.Collections;

namespace EtherSharp.Types;

/// <summary>
/// Represents a single call frame in a traced transaction, including nested internal calls.
/// </summary>
/// <param name="From">Address that initiated this call frame.</param>
/// <param name="To">Address targeted by this call frame.</param>
/// <param name="Gas">Gas allocated to this frame before execution.</param>
/// <param name="GasUsed">Gas consumed by this frame.</param>
/// <param name="Value">Native value transferred as part of this frame.</param>
/// <param name="Input">Calldata provided to this frame.</param>
/// <param name="Output">Return data produced by this frame, when available.</param>
/// <param name="Type">EVM call/create operation used for this frame.</param>
/// <param name="Calls">Direct child calls, ordered by execution path, or <see langword="null"/> when none exist.</param>
/// <param name="Error">Node-reported execution error for this frame, if any.</param>
public record CallTrace(
    Address From,
    Address To,
    ulong Gas,
    ulong GasUsed,
    UInt256 Value,
    byte[] Input,
    byte[]? Output,
    CallType Type,
    IReadOnlyList<CallTrace>? Calls,
    string? Error
)
{
    /// <summary>
    /// Returns a stack-based depth-first enumerator over this call frame and all descendants.
    /// </summary>
    public Enumerator Enumerate() => new Enumerator(this);

    /// <summary>
    /// Enumerates a <see cref="CallTrace"/> tree in pre-order (parent before children).
    /// </summary>
    public struct Enumerator : IEnumerable<CallTrace>, IEnumerator<CallTrace>
    {
        private readonly Stack<CallTrace> _stack;
        private CallTrace? _current;

        internal Enumerator(CallTrace root)
        {
            _stack = new Stack<CallTrace>();
            _stack.Push(root);
            _current = null;
        }

        /// <inheritdoc/>
        public readonly CallTrace Current => _current!;
        readonly object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if(!_stack.TryPop(out _current))
            {
                return false;
            }

            if(_current.Calls is not null)
            {
                for(int i = _current.Calls.Count - 1; i >= 0; i--)
                {
                    _stack.Push(_current.Calls[i]);
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public readonly void Reset()
            => throw new NotSupportedException();
        /// <inheritdoc/>
        public readonly void Dispose() { }
        /// <inheritdoc/>
        public readonly Enumerator GetEnumerator()
            => this;
        readonly IEnumerator<CallTrace> IEnumerable<CallTrace>.GetEnumerator()
            => this;
        readonly IEnumerator IEnumerable.GetEnumerator()
            => this;
    }
}
