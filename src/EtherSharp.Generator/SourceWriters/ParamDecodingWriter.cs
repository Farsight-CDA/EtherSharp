using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Numerics;

namespace EtherSharp.Generator.SourceWriters;
public class ParamDecodingWriter(AbiTypeWriter typeWriter)
{
    private readonly AbiTypeWriter _typeWriter = typeWriter;

    public (string, string) SetQueryOutputDecoding(string functionName, FunctionBuilder function, AbiOutputParameter[] outputParameters)
    {
        ValidateParameters(functionName, outputParameters);

        if(!TryGetPrimitiveEquivalentType(outputParameters[0].Type, out string primitiveType))
        {
            throw new NotSupportedException();
        }

        function.WithReturnTypeRaw($"{typeof(Task).FullName}<{primitiveType}>");

        string decoderFunction =
            $"""
            decoder.{GetPrimitiveABIDecodingMethodName(outputParameters[0].Type)}(out var val);
            return val;
            """;

        return (primitiveType, decoderFunction);
    }

    public (string, string) SetMessageOutputDecoding(string functionName, FunctionBuilder function, AbiOutputParameter[] outputParameters)
    {
        ValidateParameters(functionName, outputParameters);

        if(!TryGetPrimitiveEquivalentType(outputParameters[0].Type, out string primitiveType))
        {
            throw new NotSupportedException();
        }

        function.WithReturnTypeRaw($"EtherSharp.Tx.TxInput<{primitiveType}>");

        string decoderFunction =
            $"""
            decoder.{GetPrimitiveABIDecodingMethodName(outputParameters[0].Type)}(out var val);
            return val;
            """;

        return (primitiveType, decoderFunction);
    }

    private void ValidateParameters(string functionName, AbiOutputParameter[] outputParameters)
    {
        switch(outputParameters.Length)
        {
            case 0:
                throw new NotSupportedException($"Query function {functionName} does not have any output parameters");
            case > 1:
                throw new NotSupportedException($"Query function {functionName} has too many output parameters");
        }
    }

    private bool TryGetPrimitiveEquivalentType(string solidityType, out string type)
    {
        type = (solidityType switch
        {
            "address" => typeof(string).FullName,
            "string" => typeof(string).FullName,
            "bool" => typeof(bool).FullName,
            "bytes" => typeof(byte[]).FullName,
            string s when s.StartsWith("uint") && int.TryParse(s.Substring(4), out int bitSize)
                => bitSize % 8 != 0
                    ? throw new NotSupportedException("uint bitsize must be multiple of 8")
                    : bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("uint bitsize must be between 8 and 256"),
                        8 => typeof(byte).FullName,
                        <= 16 => typeof(ushort).FullName,
                        <= 32 => typeof(uint).FullName,
                        <= 64 => typeof(ulong).FullName,
                        _ => typeof(BigInteger).FullName,
                    },
            string s when s.StartsWith("int") && int.TryParse(s.Substring(3), out int bitSize)
                    => bitSize % 8 != 0
                    ? throw new NotSupportedException("int bitsize must be multiple of 8")
                    : bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("int bitsize must be between 8 and 256"),
                        8 => typeof(sbyte).FullName,
                        <= 16 => typeof(short).FullName,
                        <= 32 => typeof(int).FullName,
                        <= 64 => typeof(long).FullName,
                        _ => typeof(BigInteger).FullName,
                    },
            string s when s.StartsWith("bytes") && int.TryParse(s.Substring(5), out int bitSize)
                => bitSize switch
                {
                    < 1 or > 32 => throw new NotSupportedException("bytes bitsize must be between 8 and 256"),
                    1 => typeof(byte).FullName,
                    _ => typeof(byte[]).FullName,
                },
            _ => null
        })!;
        return type is not null;
    }

    private static string GetPrimitiveABIDecodingMethodName(string solidityType)
        => solidityType switch
        {
            string s when s.StartsWith("uint") => s.Substring(0, 2).ToUpper() + s.Substring(2),
            _ => NameUtils.ToValidFunctionName(solidityType),
        };
}
