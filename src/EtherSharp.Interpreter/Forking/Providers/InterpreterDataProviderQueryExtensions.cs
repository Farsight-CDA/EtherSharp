using EtherSharp.Contract;
using EtherSharp.Query;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Forking.Providers;

internal static class InterpreterDataProviderQueryExtensions
{
    extension(IQuery)
    {
        public static IQuery<InterpreterDataResult[]> ReadStorageAndReferencedCode(
            Address storageAddress,
            Bytes32 storageKey
        )
            => IQuery.Call(
                IInterpreterDataProviderContract.Functions.ReadStorageAndReferencedCode.Create(
                    storageAddress,
                    storageKey
                )
            ).Map<InterpreterDataResult[]>(value =>
            [
                new InterpreterDataResult.Storage(storageAddress, storageKey, value.Value),
                new InterpreterDataResult.Code(
                    Address.FromBytes(value.Value.DangerousGetReadOnlySpan()[^Address.BYTES_LENGTH..]),
                    new EVMByteCode(value.Code)
                ),
            ]);
    }
}
