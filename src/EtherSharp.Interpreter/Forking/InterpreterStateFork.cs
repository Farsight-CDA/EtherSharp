using EtherSharp.Interpreter.Runtime;
using EtherSharp.Interpreter.Runtime.ExecutionSpecs;

namespace EtherSharp.Interpreter.Forking;

/// <summary>
/// Creates independent interpreters over one cached upstream state and block context.
/// </summary>
/// <param name="dataProvider">The provider used to fetch missing upstream state.</param>
/// <param name="context">The block context shared by every interpreter created from this fork.</param>
/// <param name="options">Fork configuration.</param>
public sealed partial class InterpreterStateFork(
    IInterpreterDataProvider dataProvider,
    InterpreterContext context,
    InterpreterForkOptions options = default
)
{
    private readonly IInterpreterDataProvider _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    private readonly Lock _lock = new();

    internal InterpreterStateCache Cache { get; } = new(options.InitialState);

    /// <summary>The block context shared by this fork's interpreters.</summary>
    public InterpreterContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Creates an independent interpreter over this state fork.
    /// </summary>
    /// <param name="executionSpec">The execution preset, or <see langword="null"/> to use <see cref="InterpreterExecutionSpec.Latest"/>.</param>
    /// <param name="resourceLimits">The interpreter resource limits.</param>
    /// <returns>An interpreter whose retained state can be used in structured runs and cloned.</returns>
    public IInterpreter CreateInterpreter(
        InterpreterExecutionSpec? executionSpec = null,
        InterpreterResourceLimits? resourceLimits = null
    )
    {
        executionSpec ??= InterpreterExecutionSpec.Latest;
        return new InterpreterRuntime(
            this,
            Context,
            executionSpec,
            resourceLimits?.Validate() ?? InterpreterResourceLimits.Default,
            executionSpec.ValidateAndCreatePrecompileLookup()
        );
    }

    /// <summary>Creates an independent interpreter from another interpreter's retained state.</summary>
    /// <param name="source">An idle interpreter belonging to this fork.</param>
    /// <returns>An interpreter with independent local state and the source's execution configuration and interruption count.</returns>
    public IInterpreter CloneInterpreter(IInterpreter source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if(source is not InterpreterRuntime runtime
            || runtime.Fork != this)
        {
            throw new ArgumentException("The source interpreter must belong to this fork.", nameof(source));
        }

        lock(_lock)
        {
            return runtime.Participant is not null
                ? throw new InvalidOperationException("An interpreter cannot be cloned while its structured lane is active.")
                : runtime.Clone();
        }
    }
}
