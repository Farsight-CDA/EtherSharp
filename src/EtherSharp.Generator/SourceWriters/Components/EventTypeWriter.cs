﻿using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Net.NetworkInformation;
using System.Text;

namespace EtherSharp.Generator.SourceWriters.Components;
public class EventTypeWriter
{
    private readonly FunctionBuilder _isMatchingEventFunction = new FunctionBuilder("IsMatchingEvent")
            .AddArgument("EtherSharp.Types.Log", "log")
            .WithReturnType<bool>()
            .WithIsStatic()
            .AddStatement($"return log.Topics[0].AsSpan().SequenceEqual(TopicBytes.Span)");

    public ClassBuilder GenerateEventType(string eventTypeName, EventAbiMember eventMember)
    {
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

        for(int i = 0; i < eventMember.Inputs.Length; i++)
        {
            var parameter = eventMember.Inputs[i];

            if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out string primitiveType, out _, out string abiFunctionName, out string decodeSuffix))
            {
                throw new NotSupportedException("Event can only contain primitive types");
            }

            string parameterName = NameUtils.ToValidPropertyName(parameter.Name);

            if(String.IsNullOrEmpty(parameterName))
            {
                parameterName = $"anonymousArgument{i + 1}";
            }

            classBuilder.AddProperty(
                new PropertyBuilder(primitiveType, parameterName)
                    .WithVisibility(PropertyVisibility.Public)
                    .WithSetterVisibility(SetterVisibility.None)
            );
        }

        int topicCount = eventMember.Inputs.Count(x => x.IsIndexed);

        decodeMethod.AddStatement(
            $$"""
            if (log.Topics.Length != 1 && log.Topics.Length != {{topicCount + 1}})
            {
                throw new System.InvalidOperationException("Topic count mismatch");
            }
            """,
            false
        );

        if(topicCount > 0)
        {
            decodeMethod.AddStatement(
                $$"""
                if (log.Topics.Length == 1) 
                {
                    {{GenerateDataDecodeStatements(eventTypeName, eventMember)}}
                }
                else
                {
                    {{GenerateRegularDecodeStatements(eventTypeName, eventMember)}}
                }
                """,
                false
            );
        }
        else
        {
            decodeMethod.AddStatement(GenerateDataDecodeStatements(eventTypeName, eventMember));
        }

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

        foreach(var parameter in eventMember.Inputs)
        {
            if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out string primitiveType, out _, out string abiFunctionName, out string decodeSuffix))
            {
                throw new NotSupportedException("Event can only contain primitive types");
            }

            if(parameter.IsIndexed)
            {
                ctorBuilder.AddArgument(
                    $"new EtherSharp.ABI.AbiDecoder(log.Topics[{topicIndex}]).{abiFunctionName}(){decodeSuffix}"
                );

                topicIndex++;
            }
            else
            {
                ctorBuilder.AddArgument(
                    $"dataDecoder.{abiFunctionName}(){decodeSuffix}"
                );
            }
        }

        if(eventMember.Inputs.Any(x => !x.IsIndexed))
        {
            statementBuilder.AppendLine("EtherSharp.ABI.AbiDecoder dataDecoder = new EtherSharp.ABI.AbiDecoder(log.Data);");
        }

        statementBuilder.AppendLine($"return {ctorBuilder.ToInlineCall()};");
        return statementBuilder.ToString();
    }

    private string GenerateDataDecodeStatements(string eventTypeName, EventAbiMember eventMember)
    {
        var ctorBuilder = new ConstructorCallBuilder(eventTypeName).AddArgument("log");
        var statementBuilder = new StringBuilder();

        foreach(var parameter in eventMember.Inputs)
        {
            if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out string primitiveType, out _, out string abiFunctionName, out string decodeSuffix))
            {
                throw new NotSupportedException("Event can only contain primitive types");
            }

            ctorBuilder.AddArgument(
                $"dataDecoder.{abiFunctionName}(){decodeSuffix}"
            );
        }

        statementBuilder.AppendLine("EtherSharp.ABI.AbiDecoder dataDecoder = new EtherSharp.ABI.AbiDecoder(log.Data);");
        statementBuilder.AppendLine($"return {ctorBuilder.ToInlineCall()};");
        return statementBuilder.ToString();
    }
}
