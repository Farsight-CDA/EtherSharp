using EtherSharp.Types;
using Xunit;

namespace EtherSharp.Tests.Types;

public sealed class TargetHeightTests
{
    [Fact]
    public void Default_Should_Be_Equal_To_Latest()
    {
        TargetHeight targetHeight = default;

        Assert.Equal(TargetHeight.Latest, targetHeight);
        Assert.True(targetHeight == TargetHeight.Latest);
        Assert.Equal(TargetHeight.Latest.GetHashCode(), targetHeight.GetHashCode());
        Assert.Equal("latest", targetHeight.ToString());
    }
}
