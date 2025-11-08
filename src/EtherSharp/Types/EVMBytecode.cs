namespace EtherSharp.Types;

public class EVMBytecode(Address address, ReadOnlyMemory<byte> bytecode)
{
    public Address Address { get; } = address;
    public ReadOnlyMemory<byte> Bytecode { get; } = bytecode;
    private readonly List<ReadOnlyMemory<byte>> _functionSelectors = FindFunctionSelectors(bytecode);

    public bool HasFunctions(params IEnumerable<ReadOnlyMemory<byte>> selectors)
    {
        foreach(var requiredSelector in selectors)
        {
            bool found = false;

            foreach(var existingSelector in _functionSelectors)
            {
                if(requiredSelector.Span.SequenceEqual(existingSelector.Span))
                {
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                return false;
            }
        }

        return true;
    }

    private static List<ReadOnlyMemory<byte>> FindFunctionSelectors(ReadOnlyMemory<byte> bytecode)
    {
        var results = new List<ReadOnlyMemory<byte>>();

        int calltableStartIndex = bytecode.Span.IndexOf((byte) 0x5B) + 1;

        bytecode = bytecode[calltableStartIndex..];

        while(true)
        {
            int push4Index = bytecode.Span.IndexOf((byte) 0x63);

            if(push4Index == -1)
            {
                break;
            }

            bytecode = bytecode[(push4Index + 1)..];

            if(push4Index > 20)
            {
                break;
            }

            results.Add(bytecode[0..4]);
        }

        return results;
    }
}