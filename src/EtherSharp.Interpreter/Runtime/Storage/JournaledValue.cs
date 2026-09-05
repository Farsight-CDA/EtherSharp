namespace EtherSharp.Interpreter.Runtime.Storage;

internal sealed class JournaledValue<TValue>
{
    private readonly record struct Change(
        long Revision,
        bool HadValue,
        TValue Value
    );

    private readonly List<Change> _changes = [];
    private bool _hasValue;
    private TValue _value = default!;

    public void Set(long revision, in TValue value)
    {
        _changes.Add(new Change(revision, _hasValue, _value));
        _hasValue = true;
        _value = value;
    }

    public bool TryGetValue(out TValue value)
    {
        value = _value;
        return _hasValue;
    }

    public void Reset(long revision)
    {
        for(int i = _changes.Count - 1; i >= 0 && _changes[i].Revision > revision; i--)
        {
            var change = _changes[i];
            _hasValue = change.HadValue;
            _value = change.Value;
            _changes.RemoveAt(i);
        }
    }

    public void Commit()
        => _changes.Clear();

    public void Clear()
    {
        _changes.Clear();
        _hasValue = false;
        _value = default!;
    }
}
