using EtherSharp.ABI;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Text;

namespace EtherSharp.Crypto;

/// <summary>
/// Provides EIP-712 typed-data hashing.
/// </summary>
public static class EIP712
{
    private const byte NAME_FIELD = 1 << 0;
    private const byte VERSION_FIELD = 1 << 1;
    private const byte CHAIN_ID_FIELD = 1 << 2;
    private const byte VERIFYING_CONTRACT_FIELD = 1 << 3;
    private const byte SALT_FIELD = 1 << 4;
    private static readonly Bytes32[] _domainTypeHashes = CreateDomainTypeHashes();

    /// <summary>
    /// Calculates the final 32-byte digest for an EIP-712 message.
    /// </summary>
    /// <typeparam name="TMessage">Source-generated EIP-712 message type.</typeparam>
    /// <param name="domain">Signature domain.</param>
    /// <param name="message">Typed message to hash.</param>
    /// <returns>The digest ready to pass to an Ethereum signer.</returns>
    public static Bytes32 HashTypedData<TMessage>(in EIP712Domain domain, in TMessage message)
        where TMessage : IEIP712Type
    {
        if(message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var domainSeparator = HashDomain(domain);
        var structHash = message.HashStruct();

        Span<byte> payload = stackalloc byte[66];
        payload[0] = 0x19;
        payload[1] = 0x01;
        domainSeparator.CopyTo(payload[2..34]);
        structHash.CopyTo(payload[34..]);
        return Keccak256.HashData(payload);
    }

    private static Bytes32 HashDomain(in EIP712Domain domain)
    {
        byte fields = GetDomainFields(domain);
        if(fields == 0)
        {
            throw new ArgumentException("An EIP-712 domain must contain at least one field.", nameof(domain));
        }

        var encoder = new AbiEncoder().Bytes32(_domainTypeHashes[fields]);

        if(domain.Name is not null)
        {
            encoder.Bytes32(Keccak256.HashData(domain.Name));
        }
        if(domain.Version is not null)
        {
            encoder.Bytes32(Keccak256.HashData(domain.Version));
        }
        if(domain.ChainId is UInt256 chainId)
        {
            encoder.UInt256(chainId);
        }
        if(domain.VerifyingContract is Address verifyingContract)
        {
            encoder.Address(verifyingContract);
        }
        if(domain.Salt is Bytes32 salt)
        {
            encoder.Bytes32(salt);
        }

        return Keccak256.HashData(encoder.Build());
    }

    private static byte GetDomainFields(in EIP712Domain domain)
    {
        byte fields = 0;
        if(domain.Name is not null)
        {
            fields |= NAME_FIELD;
        }
        if(domain.Version is not null)
        {
            fields |= VERSION_FIELD;
        }
        if(domain.ChainId is not null)
        {
            fields |= CHAIN_ID_FIELD;
        }
        if(domain.VerifyingContract is not null)
        {
            fields |= VERIFYING_CONTRACT_FIELD;
        }
        if(domain.Salt is not null)
        {
            fields |= SALT_FIELD;
        }

        return fields;
    }

    private static Bytes32[] CreateDomainTypeHashes()
    {
        var hashes = new Bytes32[1 << 5];
        for(byte fields = 1; fields < hashes.Length; fields++)
        {
            var definition = new StringBuilder("EIP712Domain(");
            bool hasPreviousField = false;

            AppendDomainField(definition, fields, NAME_FIELD, "string name", ref hasPreviousField);
            AppendDomainField(definition, fields, VERSION_FIELD, "string version", ref hasPreviousField);
            AppendDomainField(definition, fields, CHAIN_ID_FIELD, "uint256 chainId", ref hasPreviousField);
            AppendDomainField(definition, fields, VERIFYING_CONTRACT_FIELD, "address verifyingContract", ref hasPreviousField);
            AppendDomainField(definition, fields, SALT_FIELD, "bytes32 salt", ref hasPreviousField);

            definition.Append(')');
            hashes[fields] = Keccak256.HashData(definition.ToString());
        }

        return hashes;
    }

    private static void AppendDomainField(
        StringBuilder definition,
        byte fields,
        byte field,
        string declaration,
        ref bool hasPreviousField)
    {
        if((fields & field) == 0)
        {
            return;
        }

        if(hasPreviousField)
        {
            definition.Append(',');
        }

        definition.Append(declaration);
        hasPreviousField = true;
    }
}
