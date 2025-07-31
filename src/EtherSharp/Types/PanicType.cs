namespace EtherSharp.Types;
/// <summary>
/// Represents various conditions that result in an EVM execution to panic.
/// </summary>
public enum PanicType : byte
{
    /// <summary>
    /// Used for generic compiler inserted panics.
    /// </summary>
    CompilerInserted = 0x00,
    /// <summary>
    /// If you call assert with an argument that evaluates to false.
    /// </summary>
    AssertionFailed = 0x01,
    /// <summary>
    /// If an arithmetic operation results in underflow or overflow outside of an unchecked { ... } block.
    /// </summary>
    ArithmeticOverflowOrUnderflow = 0x11,
    /// <summary>
    /// If you divide or modulo by zero (e.g. 5 / 0 or 23 % 0).
    /// </summary>
    DivideByZero = 0x12,
    /// <summary>
    /// If you convert a value that is too big or negative into an enum type.
    /// </summary>
    InvalidEnumType = 0x21,
    /// <summary>
    /// If you access a storage byte array that is incorrectly encoded.
    /// </summary>
    IncorrectByteArrayEncoding = 0x22,
    /// <summary>
    /// If you call .pop() on an empty array.
    /// </summary>
    PoppedEmptyArray = 0x31,
    /// <summary>
    /// If you access an array, bytesN or an array slice at an out-of-bounds or negative index (i.e. x[i] where i >= x.length or i &lt; 0).
    /// </summary>
    IndexOutOfRange = 0x32,
    /// <summary>
    /// If you allocate too much memory or create an array that is too large.
    /// </summary>
    OutOfMemory = 0x41,
    /// <summary>
    /// If you call a zero-initialized variable of internal function type.
    /// </summary>
    UninitializedInternalFunctionType = 0x51

}
