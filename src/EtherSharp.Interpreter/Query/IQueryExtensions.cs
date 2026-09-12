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
        /// Creates a query that fetches the full <see cref="Runtime.InterpreterContext"/> at the execution block.
        /// </summary>
        /// <param name="disableBlobBaseFee">
        /// Whether to omit the <c>BLOBBASEFEE</c> probe and disable the opcode in the resulting context.
        /// Set this for RPC endpoints that crash when <c>BLOBBASEFEE</c> is executed.
        /// </param>
        /// <remarks>
        /// Base and blob base fees are returned as <see langword="null"/> when the corresponding
        /// opcodes are unsupported by the execution environment.
        /// </remarks>
        public static IQuery<InterpreterContext> InterpreterContext(bool disableBlobBaseFee = false)
            => new InterpreterContextQuery(disableBlobBaseFee);
    }
}
