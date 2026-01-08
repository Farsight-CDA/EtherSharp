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
    byte[] Output,
    CallType Type,
    CallTrace[]? Calls
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

        public readonly CallTrace Current => _current!;

        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if(!_stack.TryPop(out _current))
            {
                return false;
            }

            if(_current.Calls is not null)
            {
                for(int i = _current.Calls.Length - 1; i >= 0; i--)
                {
                    _stack.Push(_current.Calls[i]);
                }
            }

            return true;
        }

        public readonly void Reset() => throw new NotSupportedException();
        public readonly void Dispose() { }
        public readonly Enumerator GetEnumerator() => this;
        IEnumerator<CallTrace> IEnumerable<CallTrace>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}