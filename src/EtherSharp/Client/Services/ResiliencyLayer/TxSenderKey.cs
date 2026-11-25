using EtherSharp.Types;

namespace EtherSharp.Client.Services.ResiliencyLayer;

/// <summary>
/// Identifies the sender account whose transaction submissions share a nonce sequence.
/// </summary>
/// <param name="ChainId">Chain on which the transactions are signed.</param>
/// <param name="Sender">Address of the signer.</param>
public readonly record struct TxSenderKey(ulong ChainId, Address Sender);
