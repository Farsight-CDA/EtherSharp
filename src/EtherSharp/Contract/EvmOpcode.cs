namespace EtherSharp.Contract;

/// <summary>
/// Identifies an EVM instruction by its byte value.
/// </summary>
public enum EvmOpcode : byte
{
    /// <summary>Halts execution successfully.</summary>
    Stop = 0x00,
    /// <summary>Adds two values.</summary>
    Add = 0x01,
    /// <summary>Multiplies two values.</summary>
    Mul = 0x02,
    /// <summary>Subtracts two values.</summary>
    Sub = 0x03,
    /// <summary>Divides two unsigned values.</summary>
    Div = 0x04,
    /// <summary>Divides two signed values.</summary>
    SDiv = 0x05,
    /// <summary>Computes the unsigned remainder.</summary>
    Mod = 0x06,
    /// <summary>Computes the signed remainder.</summary>
    SMod = 0x07,
    /// <summary>Adds two values modulo a third value.</summary>
    AddMod = 0x08,
    /// <summary>Multiplies two values modulo a third value.</summary>
    MulMod = 0x09,
    /// <summary>Raises a value to a power.</summary>
    Exp = 0x0A,
    /// <summary>Extends the sign of a value.</summary>
    SignExtend = 0x0B,

    /// <summary>Compares two unsigned values for less-than.</summary>
    Lt = 0x10,
    /// <summary>Compares two unsigned values for greater-than.</summary>
    Gt = 0x11,
    /// <summary>Compares two signed values for less-than.</summary>
    SLt = 0x12,
    /// <summary>Compares two signed values for greater-than.</summary>
    SGt = 0x13,
    /// <summary>Compares two values for equality.</summary>
    Eq = 0x14,
    /// <summary>Tests whether a value is zero.</summary>
    IsZero = 0x15,
    /// <summary>Computes a bitwise AND.</summary>
    And = 0x16,
    /// <summary>Computes a bitwise OR.</summary>
    Or = 0x17,
    /// <summary>Computes a bitwise XOR.</summary>
    Xor = 0x18,
    /// <summary>Computes a bitwise complement.</summary>
    Not = 0x19,
    /// <summary>Extracts a byte from a word.</summary>
    Byte = 0x1A,
    /// <summary>Shifts a value left.</summary>
    Shl = 0x1B,
    /// <summary>Shifts a value right logically.</summary>
    Shr = 0x1C,
    /// <summary>Shifts a signed value right arithmetically.</summary>
    Sar = 0x1D,
    /// <summary>Counts the leading zero bits in a word.</summary>
    Clz = 0x1E,

    /// <summary>Computes the Keccak-256 hash of memory.</summary>
    Keccak256 = 0x20,

    /// <summary>Gets the current account address.</summary>
    Address = 0x30,
    /// <summary>Gets an account's native balance.</summary>
    Balance = 0x31,
    /// <summary>Gets the transaction origin.</summary>
    Origin = 0x32,
    /// <summary>Gets the immediate caller.</summary>
    Caller = 0x33,
    /// <summary>Gets the call value.</summary>
    CallValue = 0x34,
    /// <summary>Loads a word from calldata.</summary>
    CallDataLoad = 0x35,
    /// <summary>Gets the calldata size.</summary>
    CallDataSize = 0x36,
    /// <summary>Copies calldata into memory.</summary>
    CallDataCopy = 0x37,
    /// <summary>Gets the current code size.</summary>
    CodeSize = 0x38,
    /// <summary>Copies current code into memory.</summary>
    CodeCopy = 0x39,
    /// <summary>Gets the transaction gas price.</summary>
    GasPrice = 0x3A,
    /// <summary>Gets an external account's code size.</summary>
    ExtCodeSize = 0x3B,
    /// <summary>Copies external account code into memory.</summary>
    ExtCodeCopy = 0x3C,
    /// <summary>Gets the previous call's return-data size.</summary>
    ReturnDataSize = 0x3D,
    /// <summary>Copies previous call return data into memory.</summary>
    ReturnDataCopy = 0x3E,
    /// <summary>Gets an external account's code hash.</summary>
    ExtCodeHash = 0x3F,

    /// <summary>Gets a recent block hash.</summary>
    BlockHash = 0x40,
    /// <summary>Gets the block fee recipient.</summary>
    Coinbase = 0x41,
    /// <summary>Gets the block timestamp.</summary>
    Timestamp = 0x42,
    /// <summary>Gets the block number.</summary>
    Number = 0x43,
    /// <summary>Gets the previous block randomness value.</summary>
    PrevRandao = 0x44,
    /// <summary>Gets the block gas limit.</summary>
    GasLimit = 0x45,
    /// <summary>Gets the chain identifier.</summary>
    ChainId = 0x46,
    /// <summary>Gets the current account's native balance.</summary>
    SelfBalance = 0x47,
    /// <summary>Gets the block base fee.</summary>
    BaseFee = 0x48,
    /// <summary>Gets a transaction blob hash.</summary>
    BlobHash = 0x49,
    /// <summary>Gets the block blob base fee.</summary>
    BlobBaseFee = 0x4A,

    /// <summary>Discards the top stack value.</summary>
    Pop = 0x50,
    /// <summary>Loads a word from memory.</summary>
    MLoad = 0x51,
    /// <summary>Stores a word in memory.</summary>
    MStore = 0x52,
    /// <summary>Stores one byte in memory.</summary>
    MStore8 = 0x53,
    /// <summary>Loads a persistent storage value.</summary>
    SLoad = 0x54,
    /// <summary>Stores a persistent storage value.</summary>
    SStore = 0x55,
    /// <summary>Jumps to a destination.</summary>
    Jump = 0x56,
    /// <summary>Conditionally jumps to a destination.</summary>
    JumpI = 0x57,
    /// <summary>Gets the current program counter.</summary>
    Pc = 0x58,
    /// <summary>Gets the active memory size.</summary>
    MSize = 0x59,
    /// <summary>Gets the remaining gas.</summary>
    Gas = 0x5A,
    /// <summary>Marks a valid jump destination.</summary>
    JumpDest = 0x5B,
    /// <summary>Loads a transient storage value.</summary>
    TLoad = 0x5C,
    /// <summary>Stores a transient storage value.</summary>
    TStore = 0x5D,
    /// <summary>Copies a region of memory.</summary>
    MCopy = 0x5E,
    /// <summary>Pushes zero onto the stack.</summary>
    Push0 = 0x5F,

    /// <summary>Pushes 1 immediate byte onto the stack.</summary>
    Push1 = 0x60,
    /// <summary>Pushes 2 immediate bytes onto the stack.</summary>
    Push2 = 0x61,
    /// <summary>Pushes 3 immediate bytes onto the stack.</summary>
    Push3 = 0x62,
    /// <summary>Pushes 4 immediate bytes onto the stack.</summary>
    Push4 = 0x63,
    /// <summary>Pushes 5 immediate bytes onto the stack.</summary>
    Push5 = 0x64,
    /// <summary>Pushes 6 immediate bytes onto the stack.</summary>
    Push6 = 0x65,
    /// <summary>Pushes 7 immediate bytes onto the stack.</summary>
    Push7 = 0x66,
    /// <summary>Pushes 8 immediate bytes onto the stack.</summary>
    Push8 = 0x67,
    /// <summary>Pushes 9 immediate bytes onto the stack.</summary>
    Push9 = 0x68,
    /// <summary>Pushes 10 immediate bytes onto the stack.</summary>
    Push10 = 0x69,
    /// <summary>Pushes 11 immediate bytes onto the stack.</summary>
    Push11 = 0x6A,
    /// <summary>Pushes 12 immediate bytes onto the stack.</summary>
    Push12 = 0x6B,
    /// <summary>Pushes 13 immediate bytes onto the stack.</summary>
    Push13 = 0x6C,
    /// <summary>Pushes 14 immediate bytes onto the stack.</summary>
    Push14 = 0x6D,
    /// <summary>Pushes 15 immediate bytes onto the stack.</summary>
    Push15 = 0x6E,
    /// <summary>Pushes 16 immediate bytes onto the stack.</summary>
    Push16 = 0x6F,
    /// <summary>Pushes 17 immediate bytes onto the stack.</summary>
    Push17 = 0x70,
    /// <summary>Pushes 18 immediate bytes onto the stack.</summary>
    Push18 = 0x71,
    /// <summary>Pushes 19 immediate bytes onto the stack.</summary>
    Push19 = 0x72,
    /// <summary>Pushes 20 immediate bytes onto the stack.</summary>
    Push20 = 0x73,
    /// <summary>Pushes 21 immediate bytes onto the stack.</summary>
    Push21 = 0x74,
    /// <summary>Pushes 22 immediate bytes onto the stack.</summary>
    Push22 = 0x75,
    /// <summary>Pushes 23 immediate bytes onto the stack.</summary>
    Push23 = 0x76,
    /// <summary>Pushes 24 immediate bytes onto the stack.</summary>
    Push24 = 0x77,
    /// <summary>Pushes 25 immediate bytes onto the stack.</summary>
    Push25 = 0x78,
    /// <summary>Pushes 26 immediate bytes onto the stack.</summary>
    Push26 = 0x79,
    /// <summary>Pushes 27 immediate bytes onto the stack.</summary>
    Push27 = 0x7A,
    /// <summary>Pushes 28 immediate bytes onto the stack.</summary>
    Push28 = 0x7B,
    /// <summary>Pushes 29 immediate bytes onto the stack.</summary>
    Push29 = 0x7C,
    /// <summary>Pushes 30 immediate bytes onto the stack.</summary>
    Push30 = 0x7D,
    /// <summary>Pushes 31 immediate bytes onto the stack.</summary>
    Push31 = 0x7E,
    /// <summary>Pushes 32 immediate bytes onto the stack.</summary>
    Push32 = 0x7F,

    /// <summary>Duplicates stack item 1.</summary>
    Dup1 = 0x80,
    /// <summary>Duplicates stack item 2.</summary>
    Dup2 = 0x81,
    /// <summary>Duplicates stack item 3.</summary>
    Dup3 = 0x82,
    /// <summary>Duplicates stack item 4.</summary>
    Dup4 = 0x83,
    /// <summary>Duplicates stack item 5.</summary>
    Dup5 = 0x84,
    /// <summary>Duplicates stack item 6.</summary>
    Dup6 = 0x85,
    /// <summary>Duplicates stack item 7.</summary>
    Dup7 = 0x86,
    /// <summary>Duplicates stack item 8.</summary>
    Dup8 = 0x87,
    /// <summary>Duplicates stack item 9.</summary>
    Dup9 = 0x88,
    /// <summary>Duplicates stack item 10.</summary>
    Dup10 = 0x89,
    /// <summary>Duplicates stack item 11.</summary>
    Dup11 = 0x8A,
    /// <summary>Duplicates stack item 12.</summary>
    Dup12 = 0x8B,
    /// <summary>Duplicates stack item 13.</summary>
    Dup13 = 0x8C,
    /// <summary>Duplicates stack item 14.</summary>
    Dup14 = 0x8D,
    /// <summary>Duplicates stack item 15.</summary>
    Dup15 = 0x8E,
    /// <summary>Duplicates stack item 16.</summary>
    Dup16 = 0x8F,

    /// <summary>Swaps the top with stack item 2.</summary>
    Swap1 = 0x90,
    /// <summary>Swaps the top with stack item 3.</summary>
    Swap2 = 0x91,
    /// <summary>Swaps the top with stack item 4.</summary>
    Swap3 = 0x92,
    /// <summary>Swaps the top with stack item 5.</summary>
    Swap4 = 0x93,
    /// <summary>Swaps the top with stack item 6.</summary>
    Swap5 = 0x94,
    /// <summary>Swaps the top with stack item 7.</summary>
    Swap6 = 0x95,
    /// <summary>Swaps the top with stack item 8.</summary>
    Swap7 = 0x96,
    /// <summary>Swaps the top with stack item 9.</summary>
    Swap8 = 0x97,
    /// <summary>Swaps the top with stack item 10.</summary>
    Swap9 = 0x98,
    /// <summary>Swaps the top with stack item 11.</summary>
    Swap10 = 0x99,
    /// <summary>Swaps the top with stack item 12.</summary>
    Swap11 = 0x9A,
    /// <summary>Swaps the top with stack item 13.</summary>
    Swap12 = 0x9B,
    /// <summary>Swaps the top with stack item 14.</summary>
    Swap13 = 0x9C,
    /// <summary>Swaps the top with stack item 15.</summary>
    Swap14 = 0x9D,
    /// <summary>Swaps the top with stack item 16.</summary>
    Swap15 = 0x9E,
    /// <summary>Swaps the top with stack item 17.</summary>
    Swap16 = 0x9F,

    /// <summary>Emits a log with no topics.</summary>
    Log0 = 0xA0,
    /// <summary>Emits a log with 1 topic.</summary>
    Log1 = 0xA1,
    /// <summary>Emits a log with 2 topics.</summary>
    Log2 = 0xA2,
    /// <summary>Emits a log with 3 topics.</summary>
    Log3 = 0xA3,
    /// <summary>Emits a log with 4 topics.</summary>
    Log4 = 0xA4,

    /// <summary>Creates a contract.</summary>
    Create = 0xF0,
    /// <summary>Calls an account.</summary>
    Call = 0xF1,
    /// <summary>Calls code using the current account context.</summary>
    CallCode = 0xF2,
    /// <summary>Returns successfully with memory data.</summary>
    Return = 0xF3,
    /// <summary>Calls code preserving the current caller and value.</summary>
    DelegateCall = 0xF4,
    /// <summary>Creates a contract using a salt.</summary>
    Create2 = 0xF5,
    /// <summary>Calls an account without permitting state changes.</summary>
    StaticCall = 0xFA,
    /// <summary>Reverts with memory data.</summary>
    Revert = 0xFD,
    /// <summary>Causes an exceptional halt.</summary>
    Invalid = 0xFE,
    /// <summary>Destroys the current account.</summary>
    SelfDestruct = 0xFF
}
