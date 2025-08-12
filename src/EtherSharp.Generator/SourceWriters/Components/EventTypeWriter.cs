using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters.Components;
public class EventTypeWriter
{
    private readonly FunctionBuilder _isMatchingLogFunction = new FunctionBuilder("IsMatchingLog")
            .AddArgument("EtherSharp.Types.Log", "log")
            .WithReturnType<bool>()
            .WithIsStatic()
            .AddStatement($"return log.Topics[0].AsSpan().SequenceEqual(TopicBytes.Span)");

    public ClassBuilder GenerateEventType(string eventTypeName, EventAbiMember eventMember)
    {
        var classBuilder = new ClassBuilder(eventTypeName)
            .AddBaseType($"EtherSharp.Realtime.Events.ITxEvent<{eventTypeName}>", true)
            .AddProperty(new PropertyBuilder("EtherSharp.Types.Log", "Log"))
            .WithAutoConstructor();

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

        for(int i = 0; i < eventMember.Inputs.Length; i++)
        {
            var parameter = eventMember.Inputs[i];

            if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out string primitiveType, out _, out string abiFunctionName, out string decodeSuffix))
            {
                throw new NotSupportedException("Event can only contain primitive types");
            }

            string parameterName = NameUtils.ToValidPropertyName(parameter.Name);

            if(string.IsNullOrEmpty(parameterName))
            {
                parameterName = $"anonymousArgument{i + 1}";
            }

            classBuilder.AddProperty(
                new PropertyBuilder(primitiveType, parameterName)
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
