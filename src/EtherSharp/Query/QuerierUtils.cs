using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Query;

internal class QuerierUtils
{
    public static readonly EVMByteCode QuerierCode = new EVMByteCode(
        Convert.FromHexString(
            "608080604052346015576102c0908161001a8239f35b5f80fdfe60806040525f3560e01c6004905f5f5b839136851015610280578382116101ae57508091600185355f1a95019490815f146102205750806001146101b157806002146101425780600a1461011b5780600b1461010357806014146100f357806015146100e357806016146100d357806017146100c357806018146100b657806019146100a957601e14610090575f80fd5b6020906014853560601c95019431815201915b9161000f565b50488152602001916100a3565b503a8152602001916100a3565b504560c01b8152600801916100a3565b504260c01b8152600801916100a3565b504360c01b8152600801916100a3565b504660c01b8152600801916100a3565b506020906014853560601c9501943f815201916100a3565b505f6003916014863560601c960195803b9283918260e81b8452858401903c0101916100a3565b505f80853560f01c600287013560e81c90602560058901359883830190818382018937010197818685f09186019161c34f195a01f161d6d85a108119166101a1579060049181533d60e81b60018201523d5f8383013e3d0101916100a3565b509250905b82116101ae57505b5ff35b50833560e81c5f80600387013560601c928460378260178b01359a8183820185370101985a9561c34f195a01f1905a90039061d6d85a1081191661021657908291600d93533d60e01b600183015260c01b60058201523d5f8383013e3d0101916100a3565b50509250906101a6565b945f915081903560e81c8060388801853783603882890101976004601882013591013560601c61c34f195a01f161d6d85a10811916610277579060049181533d60e81b60018201523d5f8383013e3d0101916100a3565b509250906101a6565b92915092506101a656fea26469706673582212206008d82f878337602da1fd2c589b0a63949a3e3d88f287200a68f865e4dd26b164736f6c634300081e0033"
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
