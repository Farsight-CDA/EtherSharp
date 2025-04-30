using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters;
public class EventTypeWriter(AbiTypeWriter typeWriter, ParamDecodingWriter paramDecodingWriter)
{
    private readonly AbiTypeWriter _typeWriter = typeWriter;
    private readonly ParamDecodingWriter _paramDecodingWriter = paramDecodingWriter;

    public ClassBuilder GenerateEventType(EventAbiMember eventMember)
    {
        string eventTypeName = NameUtils.ToValidClassName($"{eventMember.Name}Event");
        var classBuilder = new ClassBuilder(eventTypeName)
            .AddBaseType($"EtherSharp.Events.ITxEvent<{eventTypeName}>", true)
            .AddProperty(new PropertyBuilder("EtherSharp.Types.Log", "Log"))
            .WithAutoConstructor();

        byte[] topic = eventMember.GetEventTopic(out string eventSignature);
        classBuilder.AddRawContent(
            $"""
            /// <summary>
            /// Event topic based on signature: {eventSignature}
            /// </summary>
            public static System.String Topic => "0x{HexUtils.ToHexString(topic)}";
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
            if(!_paramDecodingWriter.TryGetPrimitiveEquivalentType(parameter.Type, out string primitiveType, out _))
            {
                throw new NotSupportedException();
            }
            string decodeMethodName = _paramDecodingWriter.GetPrimitiveABIDecodingMethodName(parameter.Type);

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
                decodeMethod.AddStatement($"decoder.{decodeMethodName}(out var {tempVarName})");

                topicIndex++;
            }
            else
            {
                tempVarName = $"param{totalIndex}";
                decodeMethod.AddStatement($"dataDecoder.{decodeMethodName}(out var param{totalIndex})");
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

        _typeWriter.RegisterTypeBuilder(classBuilder);

        return classBuilder;
    }
}
