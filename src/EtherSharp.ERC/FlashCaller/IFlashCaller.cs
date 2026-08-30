using EtherSharp.Client.Services.FlashCall;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.ERC.FlashCaller;

/// <summary>
/// Flash caller contract interface.
/// </summary>
[AbiFile("flash-caller.abi.json")]
[Bytecode(initCode: "0x6020605f8060325f394363a3b1b31d60e01b8252828260048160645afa833d1416602a575b8152015ff35b508051602456fe63217cd3e15f3560e01c146053575f80803560c01c8160083560f01c9182600a8337828280f092600a019283360380948437815a92604a575b503491f15f533d5f60013e3d6001015ff35b909150905f6038565b60208038035f3960205ff3")]
public partial interface IFlashCaller : IEVMContract
{
    public partial class Functions
    {
        /// <summary>
        /// Calls temporary initcode through the deployed flash caller's packed fallback protocol.
        /// </summary>
        public static class Flash
        {
            /// <summary>
            /// Creates a call to deploy the supplied initcode temporarily and call the resulting contract.
            /// </summary>
            /// <param name="contractAddress">The deployed flash caller address.</param>
            /// <param name="initCode">The temporary contract initcode.</param>
            /// <param name="callData">The calldata sent to the temporary contract.</param>
            /// <param name="flashCallGasLimit">The gas limit for the temporary contract call, or <see langword="null"/> to use all remaining gas.</param>
            /// <param name="ethValue">The ETH value sent to the temporary contract.</param>
            /// <returns>A contract call that decodes the temporary call's success status and return or revert data.</returns>
            public static IContractCall<TxCallResult> Create(
                Address contractAddress,
                EVMByteCode initCode,
                ReadOnlyMemory<byte> callData,
                ulong? flashCallGasLimit = null,
                UInt256 ethValue = default
            ) => IContractCall.ForRawContractCall(
                    contractAddress,
                    ethValue,
                    DeployedFlashCallEncoding.Encode(initCode, callData, flashCallGasLimit),
                    DeployedFlashCallEncoding.DecodeResult
                );
        }
    }
}
