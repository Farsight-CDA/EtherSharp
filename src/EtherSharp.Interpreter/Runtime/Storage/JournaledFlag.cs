namespace EtherSharp.Interpreter.Runtime.Storage;

internal sealed class JournaledFlag
{
    private long? _setRevision;

    public bool IsSet
        => _setRevision is not null;

    public void Set(long revision)
        => _setRevision ??= revision;

    public void Reset(long revision)
    {
        if(_setRevision is long setRevision && setRevision > revision)
        {
            _setRevision = null;
        }
    }

    public void Commit()
        => _setRevision = _setRevision is null ? null : 0;

    public void Clear()
        => _setRevision = null;
}
