using EtherSharp.Types;

namespace EtherSharp.Contract;

/// <summary>
/// Describes the canonical FlashCaller contract and its known deployments.
/// </summary>
public interface IFlashCaller
{
    /// <summary>
    /// Gets the canonical FlashCaller address.
    /// </summary>
    public static Address Address { get; } = Address.Parse("0x31ab65b220487210bbde88b5b8fa1f9aaa77ae76");

    /// <summary>
    /// Gets the deployment block number for the specified chain.
    /// </summary>
    /// <param name="chainId">The EVM chain ID.</param>
    /// <returns>The deployment block number, or <see langword="null"/> if the chain is not listed.</returns>
    public static ulong? GetDeploymentHeight(ulong chainId)
        => chainId switch
        {
            1 => 24_779_356, // Ethereum
            10 => 149_690_337, // Optimism
            14 => 57_907_723, // FlareA
            25 => 62_812_517, // Cronos EVM
            30 => 8_684_211, // Rootstock
            50 => 100_957_665, // XDC
            56 => 90_592_190, // BNB Smart Chain
            100 => 45_436_637, // Gnosis
            130 => 44_227_246, // Unichain
            137 => 84_929_691, // Polygon
            143 => 65_081_786, // Monad
            146 => 66_395_616, // Sonic
            196 => 56_207_777, // X Layer
            223 => 32_066_363, // B² Network
            239 => 26_119_895, // TAC
            252 => 34_085_304, // Fraxtal
            369 => 26_165_229, // PulseChain
            480 => 27_822_839, // World Chain
            747 => 60_976_652, // Flow
            988 => 22_387_270, // Stable
            999 => 31_236_991, // HyperEVM
            1_088 => 22_453_307, // Metis
            1_116 => 33_889_751, // CoreDAO
            1_329 => 200_330_531, // Sei
            1_868 => 20_921_901, // Soneium
            2_741 => 59_049_422, // Abstract
            4_114 => 6_819_449, // Citrea
            4_153 => 16_512_600, // Rise Chain
            4_217 => 22_524_562, // Tempo
            4_326 => 12_181_418, // MegaETH
            4_663 => 44_770_484, // Robinhood Chain
            5_000 => 93_424_115, // Mantle
            5_042 => 23_234_182, // Arc
            6_900 => 39_167_870, // Nibiru
            8_217 => 212_786_991, // Kaia
            8_453 => 44_093_174, // Base
            9_745 => 18_049_454, // Plasma
            33_139 => 35_465_318, // ApeChain
            42_161 => 447_590_924, // Arbitrum One (L2 block number)
            42_220 => 63_075_338, // Celo
            43_111 => 4_093_030, // Hemi
            43_114 => 81_770_982, // Avalanche C-Chain
            57_073 => 41_479_220, // Ink
            59_144 => 29_994_886, // Linea
            80_094 => 18_982_435, // Berachain
            98_866 => 58_608_075, // Plume
            167_000 => 4_968_782, // Taiko
            534_352 => 32_867_390, // Scroll
            747_474 => 28_235_729, // Katana
            _ => null,
        };
}
