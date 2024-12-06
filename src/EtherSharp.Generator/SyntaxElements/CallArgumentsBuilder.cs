using System.Text;

namespace EtherSharp.Generator.SyntaxElements;
public class UntypedArgument(string sourceExpression)
{
    public string SourceExpression { get; } = sourceExpression;
}
public class CallArgumentsBuilder : ISyntaxBuilder
{
    private readonly List<UntypedArgument> _arguments;

    public CallArgumentsBuilder()
    {
        _arguments = [];
    }

    public CallArgumentsBuilder AddArgument(string sourceExpression)
    {
        _arguments.Add(new UntypedArgument(sourceExpression));
        return this;
    }

    public string Build()
    {
        var sb = new StringBuilder();

        for(int i = 0; i < _arguments.Count; i++)
        {
            var argument = _arguments[i];
            _ = sb.Append(argument.SourceExpression);

            if(i + 1 < _arguments.Count)
            {
                _ = sb.Append(", ");
            }
        }

        return sb.ToString();
    }

    public SyntaxId GetSyntaxId()
    {
        int hashCode = HashCode.Combine(
            nameof(CallArgumentsBuilder),
            _arguments.Count
        );
        return new SyntaxId(hashCode);
    }
}
