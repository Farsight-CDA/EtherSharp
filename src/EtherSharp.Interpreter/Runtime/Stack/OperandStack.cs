using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Stack;

internal sealed class OperandStack
{
    private const int MAX_DEPTH = 1024;

    private int Count { get; set; }
    private readonly Bytes32[] _values = new Bytes32[MAX_DEPTH];

    public bool TryPush<T>(in T value)
        where T : struct, IStackValue<T>
    {
        if(Count == MAX_DEPTH)
        {
            return false;
        }

        Push(in value);
        return true;
    }

    public void Push<T>(in T value)
        where T : struct, IStackValue<T>
    {
        if(Count == MAX_DEPTH)
        {
            throw new InvalidOperationException("Operand stack is full.");
        }

        _values[Count] = T.ToStackWord(in value);
        Count++;
    }

    public bool TryPop<T>(out T value)
        where T : struct, IStackValue<T>
    {
        if(Count == 0)
        {
            value = default;
            return false;
        }

        Count--;
        value = T.FromStackWord(in _values[Count]);
        return true;
    }

    public bool TryPop<TFirst, TSecond>(out TFirst first, out TSecond second)
        where TFirst : struct, IStackValue<TFirst>
        where TSecond : struct, IStackValue<TSecond>
    {
        if(Count < 2)
        {
            first = default;
            second = default;
            return false;
        }

        Count--;
        first = TFirst.FromStackWord(in _values[Count]);
        Count--;
        second = TSecond.FromStackWord(in _values[Count]);
        return true;
    }

    public bool TryPop<TFirst, TSecond, TThird>(out TFirst first, out TSecond second, out TThird third)
        where TFirst : struct, IStackValue<TFirst>
        where TSecond : struct, IStackValue<TSecond>
        where TThird : struct, IStackValue<TThird>
    {
        if(Count < 3)
        {
            first = default;
            second = default;
            third = default;
            return false;
        }

        Count--;
        first = TFirst.FromStackWord(in _values[Count]);
        Count--;
        second = TSecond.FromStackWord(in _values[Count]);
        Count--;
        third = TThird.FromStackWord(in _values[Count]);
        return true;
    }

    public bool TryPop<TFirst, TSecond, TThird, TFourth>(
        out TFirst first,
        out TSecond second,
        out TThird third,
        out TFourth fourth
    )
        where TFirst : struct, IStackValue<TFirst>
        where TSecond : struct, IStackValue<TSecond>
        where TThird : struct, IStackValue<TThird>
        where TFourth : struct, IStackValue<TFourth>
    {
        if(Count < 4)
        {
            first = default;
            second = default;
            third = default;
            fourth = default;
            return false;
        }

        Count--;
        first = TFirst.FromStackWord(in _values[Count]);
        Count--;
        second = TSecond.FromStackWord(in _values[Count]);
        Count--;
        third = TThird.FromStackWord(in _values[Count]);
        Count--;
        fourth = TFourth.FromStackWord(in _values[Count]);
        return true;
    }

    public bool TryPop<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(
        out TFirst first,
        out TSecond second,
        out TThird third,
        out TFourth fourth,
        out TFifth fifth,
        out TSixth sixth
    )
        where TFirst : struct, IStackValue<TFirst>
        where TSecond : struct, IStackValue<TSecond>
        where TThird : struct, IStackValue<TThird>
        where TFourth : struct, IStackValue<TFourth>
        where TFifth : struct, IStackValue<TFifth>
        where TSixth : struct, IStackValue<TSixth>
    {
        if(Count < 6)
        {
            first = default;
            second = default;
            third = default;
            fourth = default;
            fifth = default;
            sixth = default;
            return false;
        }

        Count--;
        first = TFirst.FromStackWord(in _values[Count]);
        Count--;
        second = TSecond.FromStackWord(in _values[Count]);
        Count--;
        third = TThird.FromStackWord(in _values[Count]);
        Count--;
        fourth = TFourth.FromStackWord(in _values[Count]);
        Count--;
        fifth = TFifth.FromStackWord(in _values[Count]);
        Count--;
        sixth = TSixth.FromStackWord(in _values[Count]);
        return true;
    }

    public bool TryPop<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
        out TFirst first,
        out TSecond second,
        out TThird third,
        out TFourth fourth,
        out TFifth fifth,
        out TSixth sixth,
        out TSeventh seventh
    )
        where TFirst : struct, IStackValue<TFirst>
        where TSecond : struct, IStackValue<TSecond>
        where TThird : struct, IStackValue<TThird>
        where TFourth : struct, IStackValue<TFourth>
        where TFifth : struct, IStackValue<TFifth>
        where TSixth : struct, IStackValue<TSixth>
        where TSeventh : struct, IStackValue<TSeventh>
    {
        if(Count < 7)
        {
            first = default;
            second = default;
            third = default;
            fourth = default;
            fifth = default;
            sixth = default;
            seventh = default;
            return false;
        }

        Count--;
        first = TFirst.FromStackWord(in _values[Count]);
        Count--;
        second = TSecond.FromStackWord(in _values[Count]);
        Count--;
        third = TThird.FromStackWord(in _values[Count]);
        Count--;
        fourth = TFourth.FromStackWord(in _values[Count]);
        Count--;
        fifth = TFifth.FromStackWord(in _values[Count]);
        Count--;
        sixth = TSixth.FromStackWord(in _values[Count]);
        Count--;
        seventh = TSeventh.FromStackWord(in _values[Count]);
        return true;
    }

    public bool TryDup(int depth)
    {
        if(depth <= 0 || depth > Count || Count == MAX_DEPTH)
        {
            return false;
        }

        _values[Count] = _values[Count - depth];
        Count++;
        return true;
    }

    public bool TrySwap(int depth)
    {
        if(depth <= 0 || depth >= Count)
        {
            return false;
        }

        int topIndex = Count - 1;
        int swapIndex = topIndex - depth;
        (_values[topIndex], _values[swapIndex]) = (_values[swapIndex], _values[topIndex]);
        return true;
    }
}
