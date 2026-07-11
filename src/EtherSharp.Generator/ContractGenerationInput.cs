using System.Collections.Immutable;

namespace EtherSharp.Generator;

internal readonly struct ContractGenerationInput(
    ContractInfo contract,
    AdditionalFileMatch abiFile,
    AdditionalFileMatch bytecodeFile) : IEquatable<ContractGenerationInput>
{
    public ContractInfo Contract { get; } = contract;
    public AdditionalFileMatch AbiFile { get; } = abiFile;
    public AdditionalFileMatch BytecodeFile { get; } = bytecodeFile;

    public static ContractGenerationInput Create(
        ContractInfo contract, ImmutableDictionary<string, ImmutableArray<string?>> additionalFilesByName)
        => new ContractGenerationInput(
            contract,
            AdditionalFileMatch.Resolve(contract.AbiFileName, additionalFilesByName),
            AdditionalFileMatch.Resolve(contract.BytecodeFileName, additionalFilesByName)
        );

    public bool Equals(ContractGenerationInput other)
        => Contract.Equals(other.Contract)
            && AbiFile.Equals(other.AbiFile)
            && BytecodeFile.Equals(other.BytecodeFile);

    public override bool Equals(object? obj)
        => obj is ContractGenerationInput other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Contract, AbiFile, BytecodeFile);
}
