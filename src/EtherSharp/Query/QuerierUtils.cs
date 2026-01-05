using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Query;

internal class QuerierUtils
{
    public static readonly EVMByteCode QuerierCode = new EVMByteCode(
        Convert.FromHexString(
            "608080604052346015576102c9908161001a8239f35b5f80fdfe60806040523461028f575f3560e01c6004905f5f5b839136851015610285578382116101b357508091600185355f1a95019490815f146102255750806001146101b657806002146101475780600a146101205780600b1461010857806014146100f857806015146100e857806016146100d857806017146100c857806018146100bb57806019146100ae57601e14610095575f80fd5b6020906014853560601c95019431815201915b91610014565b50488152602001916100a8565b503a8152602001916100a8565b504560c01b8152600801916100a8565b504260c01b8152600801916100a8565b504360c01b8152600801916100a8565b504660c01b8152600801916100a8565b506020906014853560601c9501943f815201916100a8565b505f6003916014863560601c960195803b9283918260e81b8452858401903c0101916100a8565b505f80853560f01c600287013560e81c90602560058901359883830190818382018937010197818685f091860191614e1f195a01f16175305a108119166101a6579060049181533d60e81b60018201523d5f8383013e3d0101916100a8565b509250905b82116101b357505b5ff35b50833560e81c5f80600387013560601c928460378260178b01359a8183820185370101985a95614e1f195a01f1905a9003906175305a1081191661021b57908291601493533d60e81b600183015260801b60048201523d5f8383013e3d0101916100a8565b50509250906101ab565b945f915081903560e81c8060388801853783603882890101976004601882013591013560601c614e1f195a01f16175305a1081191661027c579060049181533d60e81b60018201523d5f8383013e3d0101916100a8565b509250906101ab565b92915092506101ab565b5f80fdfea2646970667358221220b74fd2095d3a4b20723a9e6d6d67e156efb4473171d0af8d65f07547bbf1b7ec64736f6c634300081e0033"
        )
    );

    public static byte[] EncodeCalls(IEnumerable<IQuery> queries, int maxPayloadSize, int maxResultSize,
        out int payloadSize, out int encodedCallCount, out BigInteger ethValue)
    {
        ethValue = 0;
        payloadSize = 4;
        int callCount = 0;

        foreach(var queryable in queries)
        {
            int newDataLength = payloadSize + queryable.CallDataLength;

            if(newDataLength + QuerierCode.Length + 2 > maxPayloadSize)
            {
                break;
            }

            payloadSize = newDataLength;
            callCount++;
            ethValue += queryable.EthValue;
        }

        encodedCallCount = callCount;

        byte[] arr = ArrayPool<byte>.Shared.Rent(payloadSize);
        arr.AsSpan(0, payloadSize).Clear();

        BinaryPrimitives.WriteUInt32BigEndian(arr.AsSpan(0, 4), (uint) maxResultSize);

        var buffer = arr.AsSpan(4);

        foreach(var queryable in queries)
        {
            if(callCount == 0)
            {
                break;
            }

            queryable.Encode(buffer);
            buffer = buffer[queryable.CallDataLength..];
            callCount--;
        }

        return arr;
    }
}
