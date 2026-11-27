using EtherSharp.Client;
using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;
using System.Buffers.Binary;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Reads the block number and the first sender's nonce with one constructor <c>eth_call</c>,
/// then the remaining nonces with <c>eth_getTransactionCount</c> at that block.
/// </summary>
public sealed class ConstructorCallNonceSource(
    IEthRpcModule ethRpcModule,
    RpcRequestOptions requestOptions = default,
    ILogger<ConstructorCallNonceSource>? logger = null
) : NonceSource
{
    private const ulong NONCE_SEARCH_BELOW = 256;
    private const int NONCE_SEARCH_COUNT = 4096;

    // Initcode for eth_call(from: sender, to: null): returns [blockNumber:32][DeriveCreate(sender, nonce):32].
    // blockNumber is ArbSys(0x64).arbBlockNumber() if that returns 32 bytes (Arbitrum, where NUMBER is the L1 block),
    // otherwise NUMBER. Leading byte of blockNumber is 0x00, avoiding EIP-3541 0xEF rejection; no PUSH0 for pre-Shanghai chains.
    private static readonly ReadOnlyMemory<byte> _probeInitCode = new byte[]
    {
        0x63, 0xa3, 0xb1, 0xb3, 0x1d,   // PUSH4 arbBlockNumber()
        0x60, 0x00, 0x52,               // MSTORE(0, selector) -> selector at mem[28..32]
        0x60, 0x20, 0x60, 0x00,         // retSize = 32, retOffset = 0
        0x60, 0x04, 0x60, 0x1c,         // argsSize = 4, argsOffset = 28
        0x60, 0x64, 0x5a, 0xfa,         // STATICCALL(gas, 0x64, ...)
        0x3d, 0x60, 0x20, 0x14, 0x16,   // success && RETURNDATASIZE == 32
        0x60, 0x20, 0x57,               // JUMPI 0x20 (keep ArbSys result in mem[0..32])
        0x43, 0x60, 0x00, 0x52,         // MSTORE(0, NUMBER)
        0x5b,                           // JUMPDEST @ 0x20
        0x30, 0x60, 0x20, 0x52,         // MSTORE(32, ADDRESS)
        0x60, 0x40, 0x60, 0x00, 0xf3    // RETURN(0, 64)
    };

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule ?? throw new ArgumentNullException(nameof(ethRpcModule));
    private readonly RpcRequestOptions _requestOptions = requestOptions;
    private readonly ILogger<ConstructorCallNonceSource>? _logger = logger;

    /// <inheritdoc/>
    public override async Task<Snapshot> ReadAsync(ReadOnlyMemory<Request> requests, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfZero(requests.Length);

        var (firstSender, startNonce) = requests.Span[0];
        ulong searchStart = startNonce - Math.Min(startNonce, NONCE_SEARCH_BELOW);
        var callResult = await _ethRpcModule.CallAsync(
            null,
            null,
            UInt256.Zero,
            _probeInitCode,
            new CallOptions { From = firstSender, TargetHeight = TargetHeight.Latest },
            _requestOptions,
            cancellationToken
        );

        if(!callResult.Success)
        {
            throw CallRevertedException.Parse(null, callResult.Data.Span);
        }

        var data = callResult.Data.Span;
        if(data.Length != 64 || data[..24].IndexOfAnyExcept((byte) 0) >= 0)
        {
            throw new CallParsingException.MalformedReturnDataException(
                callResult.Data, "Expected a 64-byte block number and created address"
            );
        }

        ulong blockNumber = BinaryPrimitives.ReadUInt64BigEndian(data.Slice(24, 8));
        var createdAddress = Address.FromBytes(data.Slice(44, Address.BYTES_LENGTH));
        bool foundFirstNonce = Address.TryFindCreateNonce(
            in firstSender, in createdAddress, searchStart, NONCE_SEARCH_COUNT, out ulong firstNonce
        );

        ulong[] nonces = new ulong[requests.Length];
        for(int i = 0; i < nonces.Length; i++)
        {
            // The probe only yields its sender's nonce when that nonce lies within the search range.
            if(i == 0 && foundFirstNonce)
            {
                nonces[i] = firstNonce;
                continue;
            }

            var sender = requests.Span[i].Sender;
            nonces[i] = await _ethRpcModule.GetTransactionCountAsync(
                in sender, TargetHeight.Height(blockNumber), _requestOptions, cancellationToken
            );
        }

        if(!foundFirstNonce)
        {
            _logger?.TxWatcher.NonceOutsideSearchRange(nonces[0], firstSender, searchStart, blockNumber);
        }

        return new Snapshot(blockNumber, nonces);
    }
}
