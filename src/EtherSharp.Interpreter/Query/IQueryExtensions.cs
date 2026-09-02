using EtherSharp.Interpreter.Runtime;
using EtherSharp.Query;

namespace EtherSharp.Interpreter.Query;

/// <summary>
/// Provides query factories for interpreter types.
/// </summary>
public static class IQueryExtensions
{
    extension(IQuery)
    {
        /// <summary>
        /// Creates a query that fetches the full <see cref="InterpreterContext"/> at the execution block.
        /// </summary>
        /// <remarks>
        /// Base and blob base fees are returned as <see langword="null"/> when the corresponding
        /// opcodes are unsupported by the execution environment.
        /// </remarks>
        /// <returns>A query that yields the current interpreter context.</returns>
        public static IQuery<InterpreterContext> InterpreterContext()
            => InterpreterContextQuery.Instance;
    }
}
