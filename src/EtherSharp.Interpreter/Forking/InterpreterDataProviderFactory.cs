using EtherSharp.Client;
using EtherSharp.Client.Services.FlashCall;
using EtherSharp.Interpreter.Forking.Providers;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Interpreter.Forking;

/// <summary>
/// Selects and creates per-fork data providers for the client's query backend.
/// </summary>
internal static class InterpreterDataProviderFactory
{
    /// <summary>Creates an independent provider pinned to a numeric RPC height.</summary>
    public static IInterpreterDataProvider Create(
        IEtherClient client,
        TargetHeight targetHeight,
        RpcRequestOptions requestOptions = default
    )
    {
        ArgumentNullException.ThrowIfNull(client);
        if(!targetHeight.IsNumeric)
        {
            throw new ArgumentException("The provider requires a fixed numeric block height.", nameof(targetHeight));
        }
        var internalClient = client as IInternalEtherClient
            ?? throw new InvalidOperationException("The client does not expose its query backend.");

        return internalClient.Provider.GetService<IFlashRuntimeExecutor>() switch
        {
            StateOverrideFlashCallExecutor => new StateOverrideInterpreterDataProvider(
                client, targetHeight, requestOptions
            ),
            _ => throw new InvalidOperationException(
                "No interpreter data provider is available for the configured query backend. Configure WithFlashCalls(enableStateOverrides: true)."
            ),
        };
    }
}
