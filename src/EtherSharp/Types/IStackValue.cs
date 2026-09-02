namespace EtherSharp.Types;

internal interface IStackValue<TSelf>
    where TSelf : struct, IStackValue<TSelf>
{
    static abstract TSelf FromStackWord(in Bytes32 value);
    static abstract Bytes32 ToStackWord(in TSelf value);
}
