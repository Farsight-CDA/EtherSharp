using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters;
public class EventTypeWriter(ParamDecodingWriter paramDecodingWriter)
{
    private readonly FunctionBuilder _isMatchingLogFunction = new FunctionBuilder("IsMatchingLog")
            .AddArgument("EtherSharp.Types.Log", "log")
            .WithReturnType<bool>()
            .WithIsStatic()
            .AddStatement($"return log.Topics[0].AsSpan().SequenceEqual(TopicBytes)");

    public ClassBuilder GenerateEventType(EventAbiMember eventMember)
    {
        string eventTypeName = NameUtils.ToValidClassName($"{eventMember.Name}Event");
        var classBuilder = new ClassBuilder(eventTypeName)
            .AddBaseType($"EtherSharp.Realtime.Events.ITxEvent<{eventTypeName}>", true)
            .AddProperty(new PropertyBuilder("EtherSharp.Types.Log", "Log"))
            .WithAutoConstructor();

        byte[] topic = eventMember.GetEventTopic(out string eventSignature);
        classBuilder.AddRawContent(
            $"""
            /// <summary>
            /// Event topic based on signature: {eventSignature}
            /// </summary>
            public const System.String Topic = "0x{HexUtils.ToHexString(topic)}";
            /// <summary>
            /// Event topic bytes based on signature: {eventSignature}
            /// </summary>
            public static System.ReadOnlySpan<byte> TopicBytes => [{string.Join(", ", topic)}];
            """
        );

        var decodeMethod = new FunctionBuilder("Decode")
            .WithReturnTypeRaw(eventTypeName)
            .WithIsStatic(true)
            .AddArgument("EtherSharp.Types.Log", "log");

        if(eventMember.Inputs.Any(x => !x.IsIndexed))
        {
            decodeMethod.AddStatement("EtherSharp.ABI.AbiDecoder dataDecoder = new EtherSharp.ABI.AbiDecoder(log.Data)");
        }
        if(eventMember.Inputs.Any(x => x.IsIndexed))
        {
            decodeMethod.AddStatement("EtherSharp.ABI.AbiDecoder decoder");
        }

        var constructorCall = new ConstructorCallBuilder(eventTypeName)
            .AddArgument("log");

        int topicIndex = 1;
        int totalIndex = 1;
        foreach(var parameter in eventMember.Inputs)
        {
            if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out string primitiveType, out _, out string abiFunctionName, out string decodeSuffix))
            {
                throw new NotSupportedException();
            }

            classBuilder.AddProperty(
                new PropertyBuilder(primitiveType, NameUtils.ToValidPropertyName(parameter.Name))
                    .WithVisibility(PropertyVisibility.Public)
                    .WithSetterVisibility(SetterVisibility.None)
            );

            string tempVarName;
            if(parameter.IsIndexed)
            {
                tempVarName = $"topic{topicIndex}";
                decodeMethod.AddStatement($"decoder = new EtherSharp.ABI.AbiDecoder(log.Topics[{topicIndex}])");
                decodeMethod.AddStatement($"var {tempVarName} = decoder.{abiFunctionName}(){decodeSuffix}");

                topicIndex++;
            }
            else
            {
                tempVarName = $"param{totalIndex}";
                decodeMethod.AddStatement($"var {tempVarName} = dataDecoder.{abiFunctionName}(){decodeSuffix}");
            }

            if(parameter.Type.Contains("bytes"))
            {
                constructorCall.AddArgument($"{tempVarName}.ToArray()");
            }
            else
            {
                constructorCall.AddArgument(tempVarName);
            }

            totalIndex++;
        }

        decodeMethod.AddStatement($"return {constructorCall.ToInlineCall()}");

        classBuilder.AddFunction(decodeMethod);
        classBuilder.AddFunction(_isMatchingLogFunction);

        return classBuilder;
    }
}
