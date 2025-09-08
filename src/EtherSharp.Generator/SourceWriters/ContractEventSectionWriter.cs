using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SourceWriters.Components;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;
using System.Reflection.PortableExecutable;
using System.Text;

namespace EtherSharp.Generator.SourceWriters;
public class ContractEventSectionWriter(EventTypeWriter eventTypeWriter)
{
    private readonly EventTypeWriter _eventTypeWriter = eventTypeWriter;

    public void GenerateContractEventSection(InterfaceBuilder interfaceBuilder, ClassBuilder implementationBuilder,
        string @namespace, string contractName, IEnumerable<EventAbiMember> eventMembers)
    {
        var sectionBuilder = new ClassBuilder("Logs")
            .WithIsStatic();

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

        foreach(var eventMembersGroup in distinctEvents.GroupBy(x => NameUtils.ToValidClassName(x.Name)))
        {
            foreach(var eventMember in eventMembersGroup)
            {
                byte[] topicBytes = eventMember.GetEventTopic(out string signature);
                string eventTypeName = eventMembersGroup.Count() > 1
                    ? $"{eventMembersGroup.Key}_{HexUtils.ToHexString(topicBytes.AsSpan(0, 4))}"
                    : eventMembersGroup.Key;
                string eventsModulePropertyName = eventTypeName;

                if(!eventTypeName.EndsWith("Event"))
                {
                    eventTypeName += "Event";
                }

                var typeBuilder = _eventTypeWriter.GenerateEventType(eventTypeName, eventMember);

                typeBuilder.AddRawContent(
                    $$"""
                    /// <summary>
                    /// Event signature used to calculate the event topic.
                    /// </summary>
                    public const string Signature = "{{signature}}";
                    /// <summary>
                    /// Event topic based on signature: {{signature}}
                    /// </summary>
                    public static ReadOnlyMemory<byte> TopicBytes { get; } 
                        = new byte[] { {{String.Join(",", topicBytes)}} };
                    /// <summary>
                    /// Hex encoded event topic based on signature: {{signature}}
                    /// </summary>
                    public const string TopicHex = "0x{{HexUtils.ToHexString(topicBytes)}}";
                    """
                );

                eventsModuleBuilder.AppendLine(
                    $"""
                    public readonly EtherSharp.Client.Modules.Events.IConfiguredEventsModule<{@namespace}.{contractName}.Logs.{eventTypeName}> {eventsModulePropertyName}
                        => contract.GetClient()
                            .Events<{@namespace}.{contractName}.Logs.{eventTypeName}>()
                            .HasContract(contract)
                            .HasTopic({@namespace}.{contractName}.Logs.{eventTypeName}.TopicHex);
                    """
                );

                sectionBuilder.AddInnerType(typeBuilder);
            }
        }

        eventsModuleBuilder.AppendLine("}");
        sectionBuilder.AddRawContent(eventsModuleBuilder.ToString());
        interfaceBuilder.AddInnerType(sectionBuilder);
    }

    private IEnumerable<EventAbiMember> GetDistinctEvents(IEnumerable<EventAbiMember> eventMembers)
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
