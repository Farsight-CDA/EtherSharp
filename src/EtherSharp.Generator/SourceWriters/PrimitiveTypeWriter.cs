using System.Numerics;

namespace EtherSharp.Generator.SourceWriters;
public static class PrimitiveTypeWriter
{
    public static bool TryMatchPrimitiveType(string type, out string csharpTypeName, out bool isDynamic, out string abiFunctionName, out string decodeSuffix)
    {
        string rawType = type;
        decodeSuffix = "";

        if(type.EndsWith("[]"))
        {
            rawType = type.Substring(0, type.Length - 2);
        }

        switch(rawType)
        {
            case "address":
                csharpTypeName = typeof(string).FullName;
                isDynamic = false;
                abiFunctionName = "Address";
                break;
            case "string":
                csharpTypeName = typeof(string).FullName;
                isDynamic = true;
                abiFunctionName = "String";
                break;
            case "bytes":
                csharpTypeName = typeof(byte[]).FullName;
                isDynamic = true;
                abiFunctionName = "Bytes";
                decodeSuffix = ".ToArray()";
                break;
            case "bool":
                csharpTypeName = typeof(bool).FullName;
                isDynamic = false;
                abiFunctionName = "Bool";
                break;
            case string s when s.StartsWith("uint", StringComparison.Ordinal) && int.TryParse(s.Substring(4), out int bitSize)
                && bitSize % 8 == 0 && bitSize >= 8 && bitSize <= 256:
                csharpTypeName = bitSize switch
                {
                    8 => typeof(byte).FullName,
                    <= 16 => typeof(ushort).FullName,
                    <= 32 => typeof(uint).FullName,
                    <= 64 => typeof(ulong).FullName,
                    _ => typeof(BigInteger).FullName,
                };
                isDynamic = false;
                abiFunctionName = $"UInt{bitSize}";
                break;
            case string s when s.StartsWith("int", StringComparison.Ordinal) && int.TryParse(s.Substring(3), out int bitSize)
                && bitSize % 8 == 0 && bitSize >= 8 && bitSize <= 256:
                csharpTypeName = bitSize switch
                {
                    8 => typeof(sbyte).FullName,
                    <= 16 => typeof(short).FullName,
                    <= 32 => typeof(int).FullName,
                    <= 64 => typeof(long).FullName,
                    _ => typeof(BigInteger).FullName,
                };
                isDynamic = false;
                abiFunctionName = $"Int{bitSize}";
                break;
            case string s when s.StartsWith("bytes", StringComparison.Ordinal) && int.TryParse(s.Substring(5), out int bitSize)
                && bitSize >= 1 && bitSize <= 32:
                csharpTypeName = typeof(byte[]).FullName;
                isDynamic = false;
                abiFunctionName = $"Bytes{bitSize}";
                decodeSuffix = ".ToArray()";
                break;
            default:
                csharpTypeName = null!;
                isDynamic = false;
                abiFunctionName = null!;
                return false;
        }

        if(rawType != type)
        {
            csharpTypeName = $"{csharpTypeName}[]";
            isDynamic = true;
            abiFunctionName = $"{abiFunctionName}Array";
        }

        return true;
    }
}
