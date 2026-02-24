using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using System.Text;

namespace EtherSharp.Generator.SourceWriters.Components;

internal class EventTypeWriter(ParamEncodingWriter paramEncodingWriter, MemberTypeWriter memberTypeWriter)
{
    private readonly ParamEncodingWriter _paramEncodingWriter = paramEncodingWriter;
    private readonly MemberTypeWriter _memberTypeWriter = memberTypeWriter;

    private readonly FunctionBuilder _isMatchingEventFunction = new FunctionBuilder("IsMatchingEvent")
            .AddArgument("EtherSharp.Types.Log", "log")
            .WithReturnType<bool>()
            .WithIsStatic()
            .AddStatement(
                $$"""
                if (log.Topics[0] != Topic) 
                {
                    return false;
                }

                return SupportedTopicCounts.Span.Contains(log.Topics.Length)
                """);

    public ClassBuilder GenerateEventType(string eventTypeName, EventAbiMember[] eventMembers, out List<int> supportedTopicCounts)
    {
        supportedTopicCounts = [];
        var classBuilder = new ClassBuilder(eventTypeName)
            .AddBaseType($"EtherSharp.Realtime.Events.ITxLog<{eventTypeName}>", true)
            .AddProperty(new PropertyBuilder("EtherSharp.Types.Log", "Event"))
            .WithAutoConstructor();

        var decodeMethod = new FunctionBuilder("Decode")
            .WithReturnTypeRaw(eventTypeName)
            .WithIsStatic(true)
            .AddArgument("EtherSharp.Types.Log", "log");

        var tryDecodeMethod = new FunctionBuilder("TryDecode")
            .AddArgument("EtherSharp.Types.Log", "log")
            .AddArgument($"out {eventTypeName}", "parsedEvent")
            .WithReturnType<bool>()
            .WithIsStatic()
            .AddStatement(
                $$"""
                if (!IsMatchingEvent(log)) 
                {
                    parsedEvent = null!;
                    return false;
                }

                parsedEvent = Decode(log);
                return true
                """
            );

        _memberTypeWriter.AddInputProperties(classBuilder, eventMembers[0].Inputs);
        bool isFirst = true;

        foreach(var eventMember in eventMembers)
        {
            int topicCount = eventMember.Inputs.Count(x => x.IsIndexed);

            if(supportedTopicCounts.Contains(topicCount + 1))
            {
                throw new NotSupportedException($"ABI contains ambiguous event decodings for {eventTypeName}");
            }

            supportedTopicCounts.Add(topicCount + 1);
            decodeMethod.AddStatement(
                $$"""
                {{(isFirst ? "" : "else ")}}if (log.Topics.Length == {{topicCount + 1}}) 
                {
                    {{GenerateRegularDecodeStatements(eventTypeName, eventMember)}}
                }
                """,
                false
            );

            isFirst = false;
        }

        decodeMethod.AddStatement(
            $$"""
            else
            {
                throw new EtherSharp.Common.Exceptions.EventParsingException(log, "Topic count mismatch");
            }
            """,
            false
        );

        classBuilder.AddFunction(decodeMethod);
        classBuilder.AddFunction(tryDecodeMethod);
        classBuilder.AddFunction(_isMatchingEventFunction);

        return classBuilder;
    }

    private string GenerateRegularDecodeStatements(string eventTypeName, EventAbiMember eventMember)
    {
        var ctorBuilder = new ConstructorCallBuilder(eventTypeName).AddArgument("log");
        var statementBuilder = new StringBuilder();

        int topicIndex = 1;

        if(eventMember.Inputs.Length > 0)
        {
            statementBuilder.AppendLine("EtherSharp.ABI.AbiDecoder decoder;");
        }
        if(eventMember.Inputs.Any(x => !x.IsIndexed))
        {
            statementBuilder.AppendLine("decoder = new EtherSharp.ABI.AbiDecoder(log.Data);");
        }

        for(int i = 0; i < eventMember.Inputs.Length; i++)
        {
            var parameter = eventMember.Inputs[i];

            if(parameter.IsIndexed)
            {
                continue;
            }

            var (outputTypeName, _, decodeFunc) = _paramEncodingWriter.GetOutputDecoding(
                $"EventParam{i + 1}",
                [parameter]
            );

            statementBuilder.AppendLine($"{outputTypeName} parameter{i} = {decodeFunc};");
        }

        for(int i = 0; i < eventMember.Inputs.Length; i++)
        {
            var parameter = eventMember.Inputs[i];

            if(!parameter.IsIndexed)
            {
                continue;
            }

            var (outputTypeName, _, decodeFunc) = _paramEncodingWriter.GetOutputDecoding(
                $"EventParam{i + 1}",
                [parameter]
            );

            statementBuilder.AppendLine($"decoder = new EtherSharp.ABI.AbiDecoder(log.Topics[{topicIndex}].ToArray());");
            statementBuilder.AppendLine($"{outputTypeName} parameter{i} = {decodeFunc};");
            topicIndex++;
        }

        for(int i = 0; i < eventMember.Inputs.Length; i++)
        {
            ctorBuilder.AddArgument($"parameter{i}");
        }

        statementBuilder.AppendLine($"return {ctorBuilder.ToInlineCall()};");
        return statementBuilder.ToString();
    }
}
