namespace EtherSharp.Common.Exceptions;

internal class ImpossibleException : Exception
{
    public ImpossibleException()
        : base("An impossible code path was reached.")
    {
    }
}
