using EtherSharp.Numerics;
using System.Collections;

namespace EtherSharp.Types;

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
    public Enumerator Enumerate() => new Enumerator(this);

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
