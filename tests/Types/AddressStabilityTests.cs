using EtherSharp.Types;
namespace EtherSharp.Tests.Types;

public sealed class AddressStabilityTests
{
    private sealed record Row(Address User);

    [Fact]
    public void ValueSortScan_ShouldMatchByteSemantics_ForTwoRows()
    {
        for(int seed = 0; seed < 512; seed++)
        {
            var rng = new Random(seed);
            byte[] aBytes = new byte[Address.BYTES_LENGTH];
            byte[] bBytes = new byte[Address.BYTES_LENGTH];
            rng.NextBytes(aBytes);
            rng.NextBytes(bBytes);
            var rows = new[]
            {
                new Row(Address.FromBytes(aBytes)),
                new Row(Address.FromBytes(bBytes))
            };
            var users = rows.Select(x => x.User).Distinct().ToList();
            users.Sort(static (x, y) => x.CompareTo(y));
            Array.Sort(rows, static (x, y) => x.User.CompareTo(y.User));

            int attached = 0;
            int idx = 0;
            foreach(var user in users)
            {
                while(idx < rows.Length && rows[idx].User == user)
                {
                    attached++;
                    idx++;
                }
            }

            if(attached != rows.Length)
            {
                Assert.Fail($"Failed at seed={seed}: attached={attached}, rows={rows.Length}");
            }
        }
    }
}
