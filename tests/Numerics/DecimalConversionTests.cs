using EtherSharp.Numerics;

namespace EtherSharp.Tests.Numerics;

public class DecimalConversionTests
{
    [Fact]
    public void Should_Convert_Decimal_To_UInt256()
    {
        var value = (UInt256)79228162514264337593543950335m;
        Assert.Equal(UInt256.MaxValue >> 160, value);
    }

    [Fact]
    public void Should_Truncate_Decimal_When_Converting_To_UInt256()
    {
        var value = (UInt256)42.99m;
        Assert.Equal((UInt256)42UL, value);
    }

    [Fact]
    public void Should_Throw_When_Converting_Negative_Decimal_To_UInt256()
        => Assert.Throws<ArgumentException>(() => _ = (UInt256)(-1m));

    [Fact]
    public void Should_Convert_UInt256_To_Decimal()
    {
        decimal value = (decimal)(UInt256.MaxValue >> 160);
        Assert.Equal(79228162514264337593543950335m, value);
    }

    [Fact]
    public void Should_Throw_When_Converting_Large_UInt256_To_Decimal()
        => Assert.Throws<OverflowException>(() => _ = (decimal)UInt256.MaxValue);

    [Fact]
    public void Should_Convert_Decimal_To_Int256()
    {
        var value = (Int256)(-79228162514264337593543950335m);
        var expected = Int256.Negate((Int256)(UInt256.MaxValue >> 160));
        Assert.Equal(expected, value);
    }

    [Fact]
    public void Should_Truncate_Decimal_When_Converting_To_Int256()
    {
        var value = (Int256)(-42.99m);
        Assert.Equal((Int256)(-42L), value);
    }

    [Fact]
    public void Should_Convert_Int256_To_Decimal()
    {
        decimal value = (decimal)(Int256)79228162514264337593543950335m;
        Assert.Equal(79228162514264337593543950335m, value);
    }

    [Fact]
    public void Should_Throw_When_Converting_Large_Int256_To_Decimal()
    {
        Assert.Throws<OverflowException>(() => _ = (decimal)Int256.MaxValue);
        Assert.Throws<OverflowException>(() => _ = (decimal)Int256.MinValue);
    }
}
