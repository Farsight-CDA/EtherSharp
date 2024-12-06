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
            .WithAutoConstructor();

        var decodeMethod = new FunctionBuilder("Decode")
            .WithReturnTypeRaw(eventTypeName)
            .WithIsStatic(true)
            .AddArgument("EtherSharp.Types.Log", "log");

        var constructorCall = new ConstructorCallBuilder(eventTypeName);

        foreach(var parameter in eventMember.Inputs)
        {
            if (!_paramDecodingWriter.TryGetPrimitiveEquivalentType(parameter.Type, out string primitiveType))
            {
                throw new NotSupportedException();
            }

            classBuilder.AddProperty(
                new PropertyBuilder(primitiveType, NameUtils.ToValidPropertyName(parameter.Name))
                    .WithVisibility(PropertyVisibility.Public)
                    .WithSetterVisibility(SetterVisibility.None)                
            );

            constructorCall.AddArgument("default");
        }

        decodeMethod.AddStatement($"return {constructorCall.ToInlineCall()}");
        classBuilder.AddFunction(decodeMethod);

        _typeWriter.RegisterTypeBuilder(classBuilder);

        return classBuilder;
    }
}
