using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Interpreter.Forking.Providers;

[Bytecode(runtimeCode: "0x5f3560e01c63772ceff3146011575f80fd5b602360043554805f525f813b9283928360e81b6020523c6023015ff3")]
internal partial interface IInterpreterDataProviderContract : IEVMContract
{
    private const int STORAGE_AND_CODE_HEADER_LENGTH = Bytes32.BYTE_LENGTH + 3;

    public readonly record struct StorageAndReferencedCode(
        Bytes32 Value,
        ReadOnlyMemory<byte> Code
    );

    public partial class Functions
    {
        public static class ReadStorageAndReferencedCode
        {
            private static Bytes4 Selector { get; } = Bytes4.FromBytes(
                Keccak256.HashData("readStorageAndReferencedCode(bytes32)"u8)
                    .DangerousGetReadOnlySpan()[..Bytes4.BYTE_LENGTH]
            );

            public static IContractCall<StorageAndReferencedCode> Create(
                Address contractAddress,
                Bytes32 slot
            )
            {
                byte[] input = new byte[Bytes4.BYTE_LENGTH + Bytes32.BYTE_LENGTH];
                Selector.CopyTo(input);
                slot.CopyTo(input.AsSpan(Bytes4.BYTE_LENGTH));
                return IContractCall.ForRawContractCall(
                    contractAddress,
                    UInt256.Zero,
                    input,
                    Decode
                );
            }

            private static StorageAndReferencedCode Decode(ReadOnlyMemory<byte> result)
            {
                var value = Bytes32.FromBytes(result.Span[..Bytes32.BYTE_LENGTH]);
                int codeLength = ReadCodeLength(result.Span);
                return new StorageAndReferencedCode(
                    value,
                    result.Slice(STORAGE_AND_CODE_HEADER_LENGTH, codeLength)
                );
            }

            private static int ReadCodeLength(ReadOnlySpan<byte> result)
            {
                Span<byte> lengthBuffer = stackalloc byte[4];
                result.Slice(Bytes32.BYTE_LENGTH, 3).CopyTo(lengthBuffer[1..]);
                return (int) BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
            }
        }
    }
}
