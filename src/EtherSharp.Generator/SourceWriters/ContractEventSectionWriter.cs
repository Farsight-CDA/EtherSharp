using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SourceWriters.Components;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;

internal class ContractEventSectionWriter(EventTypeWriter eventTypeWriter)
{
    private readonly EventTypeWriter _eventTypeWriter = eventTypeWriter;

    public void GenerateContractEventSection(InterfaceBuilder interfaceBuilder, string @namespace,
        string contractName, IEnumerable<EventAbiMember> eventMembers)
    {
        var sectionBuilder = new ClassBuilder("Logs")
            .AddBaseType("EtherSharp.Contract.Sections.ILogsSection", true)
            .AddRawContent("private Logs() {}");

        var distinctEvents = GetDistinctEvents(eventMembers).ToArray();

        var eventsModuleBuilder = new StringBuilder();
        eventsModuleBuilder.AppendLine(
            $$"""    
            public readonly ref struct EventsModule
            {
                private readonly {{contractName}} contract;

                public EventsModule({{contractName}} contract)
                {
                    this.contract = contract;
                }
                public EventsModule()
                {
                    throw new NotSupportedException();
                }
            """
        );

        var eventNameTopics = new Dictionary<string, List<byte[]>>();
        foreach(var eventMember in eventMembers)
        {
            string rawEventTypeName = NameUtils.ToValidClassName(eventMember.Name);
            byte[] eventTopic = eventMember.GetEventTopic(out _);

            if(!eventNameTopics.TryGetValue(rawEventTypeName, out var topics))
            {
                eventNameTopics.Add(rawEventTypeName, [eventTopic]);
                continue;
            }
            if(topics.Any(x => x.SequenceEqual(eventTopic)))
            {
                continue;
            }

            topics.Add(eventTopic);
        }

        string GetEventName(EventAbiMember eventMember)
        {
            string rawEventTypeName = NameUtils.ToValidClassName(eventMember.Name);
            byte[] topicBytes = eventMember.GetEventTopic(out _);
            string name = eventNameTopics[rawEventTypeName].Count == 1
                ? rawEventTypeName
                : $"{rawEventTypeName}_{HexUtils.ToHexString(topicBytes.AsSpan(0, 4))}";
            return name.EndsWith("Event")
                ? name
                : $"{name}Event";
        }

        var topicClassNames = new List<string>();

        foreach(var eventMembersGroup in eventMembers.GroupBy(x => HexUtils.ToHexString(x.GetEventTopic(out _))))
        {
            string eventTypeName = GetEventName(eventMembersGroup.First());
            topicClassNames.Add(eventTypeName);
            byte[] topicBytes = eventMembersGroup.First().GetEventTopic(out string? eventSignature);

            var typeBuilder = _eventTypeWriter.GenerateEventType(eventTypeName, [.. eventMembersGroup], out var supportedTopicCounts);

            typeBuilder.AddRawContent(
                $$"""
                    /// <summary>
                    /// Event signature used to calculate the event topic.
                    /// </summary>
                    public const string Signature = "{{eventSignature}}";
                    /// <summary>
                    /// Event topic based on signature: {{eventSignature}}
                    /// </summary>
                    public static ReadOnlyMemory<byte> TopicBytes { get; } 
                        = new byte[] { {{String.Join(",", topicBytes)}} };
                    /// <summary>
                    /// Hex encoded event topic based on signature: {{eventSignature}}
                    /// </summary>
                    public const string TopicHex = "0x{{HexUtils.ToHexString(topicBytes)}}";
                    /// <summary>
                    /// Supported topic counts when decoding event.
                    /// </summary>
                    public static ReadOnlyMemory<int> SupportedTopicCounts { get; } 
                        = new int[] { {{String.Join(",", supportedTopicCounts)}} };
                    """
            );

            eventsModuleBuilder.AppendLine(
                $"""
                    public readonly EtherSharp.Client.Modules.Events.IConfiguredEventsModule<{@namespace}.{contractName}.Logs.{eventTypeName}> {eventTypeName}
                        => contract.GetClient()
                            .Events<{@namespace}.{contractName}.Logs.{eventTypeName}>()
                            .HasContract(contract)
                            .HasTopic({@namespace}.{contractName}.Logs.{eventTypeName}.TopicHex);
                    """
            );

            sectionBuilder.AddInnerType(typeBuilder);
        }

        var getAllTopicsFunction = new FunctionBuilder("GetTopics")
            .WithIsStatic(true)
            .WithVisibility(FunctionVisibility.Public)
            .WithReturnTypeRaw("ReadOnlyMemory<byte>[]");

        getAllTopicsFunction.AddStatement(
            $"""
                return [
            {String.Join(",\n", topicClassNames.Select(x => $"       {x}.TopicBytes"))}
                ]
            """
        );

        eventsModuleBuilder.AppendLine("}");
        sectionBuilder.AddRawContent(eventsModuleBuilder.ToString());
        sectionBuilder.AddFunction(getAllTopicsFunction);
        interfaceBuilder.AddInnerType(sectionBuilder);
    }

    private static IEnumerable<EventAbiMember> GetDistinctEvents(IEnumerable<EventAbiMember> eventMembers)
    {
        var mappedMembers = eventMembers
            .Select(x =>
            {
                _ = x.GetEventTopic(out string? eventSignature);
                return new
                {
                    Member = x,
                    Signature = eventSignature
                };
            })
            .ToArray();

        return mappedMembers
            .Where(x => x == mappedMembers.First(y => y.Signature == x.Signature))
            .Select(x => x.Member);
    }
}
