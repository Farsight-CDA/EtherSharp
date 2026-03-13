using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Buffers.Binary;
using System.IO;
using Xunit;

namespace EtherSharp.Tests.Tx;

public class EIP1559TxParamsTests
{
    [Fact]
    public void Should_RoundTrip_Binary_EIP1559_TxParams()
    {
        var txParams = new EIP1559TxParams(
        [
            new StateAccess(
                Address.Parse("0x1111111111111111111111111111111111111111"),
                [
                    Convert.FromHexString("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"),
                    [0x01, 0x02, 0x03, 0x04]
                ]
            ),
            new StateAccess(
                Address.Parse("0x2222222222222222222222222222222222222222"),
                []
            )
        ]
        );

        byte[] encoded = Encode(txParams);
        var decoded = Decode<EIP1559TxParams>(encoded);

        Assert.Equal((uint) 2, BinaryPrimitives.ReadUInt32BigEndian(encoded.AsSpan(0, sizeof(uint))));
        AssertStateAccessEqual(txParams.AccessList, decoded.AccessList);
    }

    [Fact]
    public void Should_Encode_Empty_Access_List_As_Binary_Count()
    {
        byte[] encoded = Encode(EIP1559TxParams.Default);
        var decoded = Decode<EIP1559TxParams>(encoded);

        Assert.Equal([0x00, 0x00, 0x00, 0x00], encoded);
        Assert.Empty(decoded.AccessList);
    }

    [Fact]
    public void Should_Reject_Truncated_Binary_EIP1559_TxParams()
    {
        byte[] encoded = Encode(
            new EIP1559TxParams(
            [
                new StateAccess(
                    Address.Parse("0x3333333333333333333333333333333333333333"),
                    [
                        Convert.FromHexString("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB")
                    ]
                )
            ]
            )
        );

        byte[] truncated = encoded[..^1];

        Assert.Throws<InvalidDataException>(() => Decode<EIP1559TxParams>(truncated));
    }

    private static void AssertStateAccessEqual(ReadOnlySpan<StateAccess> expected, ReadOnlySpan<StateAccess> actual)
    {
        Assert.Equal(expected.Length, actual.Length);

        for(int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Address, actual[i].Address);
            Assert.Equal(expected[i].StorageKeys.Length, actual[i].StorageKeys.Length);

            for(int j = 0; j < expected[i].StorageKeys.Length; j++)
            {
                Assert.Equal(expected[i].StorageKeys[j], actual[i].StorageKeys[j]);
            }
        }
    }

    private static byte[] Encode<TTxParams>(TTxParams value)
        where TTxParams : ITxParams<TTxParams>
        => value.Encode();

    private static TTxParams Decode<TTxParams>(ReadOnlySpan<byte> data)
        where TTxParams : ITxParams<TTxParams>
        => TTxParams.Decode(data);
}
