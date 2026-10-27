using System.Runtime.InteropServices;

namespace EtherSharp.Interpreter.Runtime.Storage;

internal sealed class JournaledMap<TKey, TValue>
    where TKey : notnull
{
    private readonly record struct Change(
        long Revision,
        TKey Key,
        bool HadValue,
        TValue Value
    );

    private readonly Dictionary<TKey, TValue> _values = [];
    private readonly List<Change> _changes = [];

    public void Set(long revision, in TKey key, in TValue value)
    {
        ref var currentValue = ref CollectionsMarshal.GetValueRefOrAddDefault(_values, key, out bool hadValue);
        _changes.Add(new Change(revision, key, hadValue, currentValue!));
        currentValue = value;
    }

    public bool TryGetValue(in TKey key, out TValue value)
        => _values.TryGetValue(key, out value!);

    public void Reset(long revision)
    {
        for(int i = _changes.Count - 1; i >= 0 && _changes[i].Revision > revision; i--)
        {
            var change = _changes[i];
            if(change.HadValue)
            {
                _values[change.Key] = change.Value;
            }
            else
            {
                _values.Remove(change.Key);
            }

            _changes.RemoveAt(i);
        }
    }

    public void Commit()
        => _changes.Clear();

    public void CopyTo(JournaledMap<TKey, TValue> target)
    {
        target._values.EnsureCapacity(_values.Count);
        foreach(var (key, value) in _values)
        {
            target._values.Add(key, value);
        }
    }

    public void Clear(long revision)
    {
        foreach(var (key, value) in _values)
        {
            _changes.Add(new Change(revision, key, true, value));
        }

        _values.Clear();
    }

    public void Clear()
    {
        _values.Clear();
        _changes.Clear();
    }
}
