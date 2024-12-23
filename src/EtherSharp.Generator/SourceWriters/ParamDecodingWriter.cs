using EtherSharp.Generator.Abi.Parameters;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;
public class ParamDecodingWriter
{
    public (string, string) SetQueryOutputDecoding(FunctionBuilder function, AbiOutputParameter[] outputParameters)
    {
        var (type, decoderFunction) = GenerateOutputEncoding(outputParameters);
        function.WithReturnTypeRaw($"{typeof(Task).FullName}<{type}>");
        return (type, decoderFunction);
    }

    public (string, string) SetMessageOutputDecoding(FunctionBuilder function, AbiOutputParameter[] outputParameters)
    {
        var (type, decoderFunction) = GenerateOutputEncoding(outputParameters);
        function.WithReturnTypeRaw($"EtherSharp.Tx.TxInput<{type}>");
        return (type, decoderFunction);
    }

    private (string, string) GenerateOutputEncoding(AbiOutputParameter[] outputParameters)
    {
        if(outputParameters.Length == 1)
        {
            if(!TryGetPrimitiveEquivalentType(outputParameters[0].Type, out string primitiveType, out bool _))
            {
                throw new NotSupportedException();
            }

            string decoderFunction =
                $"""
                decoder.{GetPrimitiveABIDecodingMethodName(outputParameters[0].Type)}(out var val);
                return {(outputParameters[0].Type.Contains("bytes") ? "val.ToArray()" : "val")};
                """;
            return (primitiveType, decoderFunction);
        }
        else
        {
            var tupleTypeSb = new StringBuilder().Append('(');
            var decoderFuncSb = new StringBuilder();
            var returnValSb = new StringBuilder().Append("return (");

            for(int i = 0; i < outputParameters.Length; i++)
            {
                var parameter = outputParameters[i];
                if(!TryGetPrimitiveEquivalentType(parameter.Type, out string primitiveType, out bool _))
                {
                    throw new NotSupportedException();
                }

                bool isLastParameter = i == outputParameters.Length - 1;
                
                tupleTypeSb.Append(isLastParameter ? primitiveType : $"{primitiveType}, ");
                returnValSb.Append(isLastParameter ? $"val{i}" : $"val{i}, ");

                bool isBytesType = parameter.Type.Contains("bytes");

                decoderFuncSb.AppendLine(
                    $"decoder.{GetPrimitiveABIDecodingMethodName(parameter.Type)}(out var val{(isBytesType ? "bytes" : "")}{i});"
                );

                if (isBytesType)
                {
                    decoderFuncSb.AppendLine($"var val{i} = valbytes{i}.ToArray();");
                }
            }

            tupleTypeSb.Append(')');
            returnValSb.Append(')');
            decoderFuncSb.AppendLine(returnValSb.ToString());
            decoderFuncSb.Append(';');

            return (tupleTypeSb.ToString(), decoderFuncSb.ToString());
        }
    }

    public bool TryGetPrimitiveEquivalentType(string solidityType, out string type, out bool isDynamicType)
    {
        if(solidityType.EndsWith("[]", StringComparison.OrdinalIgnoreCase))
        {
            bool isValid = TryGetPrimitiveEquivalentType(
                solidityType.Substring(0, solidityType.Length - 2), out type, out isDynamicType
            );

            if(!isValid)
            {
                return false;
            }

            isDynamicType = true;
            type = $"{type}[]";
            return isValid;
        }

        var (pt, idt) = solidityType switch
        {
            "address" => (typeof(string).FullName, false),
            "string" => (typeof(string).FullName, true),
            "bool" => (typeof(bool).FullName, false),
            "bytes" => (typeof(byte[]).FullName, true),
            string s when s.StartsWith("uint", StringComparison.Ordinal) && int.TryParse(s.Substring(4), out int bitSize)
                => bitSize % 8 != 0
                    ? throw new NotSupportedException("uint bitsize must be multiple of 8")
                    : (bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("uint bitsize must be between 8 and 256"),
                        8 => typeof(byte).FullName,
                        <= 16 => typeof(ushort).FullName,
                        <= 32 => typeof(uint).FullName,
                        <= 64 => typeof(ulong).FullName,
                        _ => typeof(BigInteger).FullName,
                    }, false),
            string s when s.StartsWith("int", StringComparison.Ordinal) && int.TryParse(s.Substring(3), out int bitSize)
                    => bitSize % 8 != 0
                    ? throw new NotSupportedException("int bitsize must be multiple of 8")
                    : (bitSize switch
                    {
                        < 8 or > 256 => throw new NotSupportedException("int bitsize must be between 8 and 256"),
                        8 => typeof(sbyte).FullName,
                        <= 16 => typeof(short).FullName,
                        <= 32 => typeof(int).FullName,
                        <= 64 => typeof(long).FullName,
                        _ => typeof(BigInteger).FullName,
                    }, false),
            string s when s.StartsWith("bytes", StringComparison.Ordinal) && int.TryParse(s.Substring(5), out int bitSize)
                => (bitSize switch
                {
                    < 1 or > 32 => throw new NotSupportedException("bytes bitsize must be between 8 and 256"),
                    1 => typeof(byte).FullName,
                    _ => typeof(byte[]).FullName,
                }, false),
            _ => (null, false)
        };

        type = pt!;
        isDynamicType = idt;

        return type is not null;
    }

    public string GetPrimitiveABIDecodingMethodName(string solidityType)
        => solidityType switch
        {
            string s when s.EndsWith("[]", StringComparison.OrdinalIgnoreCase)
                => $"{GetPrimitiveABIDecodingMethodName(s.Substring(0, s.Length - 2))}Array",
            string s when s.StartsWith("uint", StringComparison.Ordinal) => s.Substring(0, 2).ToUpper(CultureInfo.InvariantCulture) + s.Substring(2),
            _ => NameUtils.ToValidFunctionName(solidityType),
        };
}
