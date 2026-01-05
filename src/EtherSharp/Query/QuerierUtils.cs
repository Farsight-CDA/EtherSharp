using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Query;

internal class QuerierUtils
{
    public static readonly EVMByteCode QuerierCode = new EVMByteCode(
        Convert.FromHexString(
            "6102918061000c5f395ff3fe6004361061028d575f3560e01c6004905f5f5b839136851015610283578382116101b157508091600185355f1a95019490815f146102235750806001146101b457806002146101455780600a1461011e5780600b1461010657806014146100f657806015146100e657806016146100d657806017146100c657806018146100b957806019146100ac57601e14610093575f80fd5b6020906014853560601c95019431815201915b91610012565b50488152602001916100a6565b503a8152602001916100a6565b504560c01b8152600801916100a6565b504260c01b8152600801916100a6565b504360c01b8152600801916100a6565b504660c01b8152600801916100a6565b506020906014853560601c9501943f815201916100a6565b505f6003916014863560601c960195803b9283918260e81b8452858401903c0101916100a6565b505f80853560f01c600287013560e81c90602560058901359883830190818382018937010197818685f09186019161c34f195a01f161d6d85a108115166101a4579060049181533d60e81b60018201523d5f8383013e3d0101916100a6565b509250905b82116101b157505b5ff35b50833560e81c5f80600387013560601c928460378260178b01359a8183820185370101985a9561c34f195a01f1905a90039061d6d85a1081151661021957908291600d93533d60e01b600183015260c01b60058201523d5f8383013e3d0101916100a6565b50509250906101a9565b945f915081903560e81c8060388801853783603882890101976004601882013591013560601c61c34f195a01f161d6d85a1081151661027a579060049181533d60e81b60018201523d5f8383013e3d0101916100a6565b509250906101a9565b92915092506101a9565b5f80fd"
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
