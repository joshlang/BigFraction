using System;
using System.Linq;
using Xunit;

namespace Epoche;

public class IEnumerableExtensionsTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void SumBigFraction_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IEnumerableExtensions.SumBigFraction(null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void SumBigFraction_Empty_Returns0() => Assert.True(new BigFraction[0].SumBigFraction().IsZero);

    [Fact]
    [Trait("Type", "Unit")]
    public void SumBigFraction_OneNumber_ReturnsSame()
    {
        var b = (BigFraction)RandomHelper.GetRandomInt64();
        Assert.Equal(b, new[] { b }.SumBigFraction());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void SumBigFraction_RandomNumbers_ReturnsSum()
    {
        var rnd = new Random();
        var nums = Enumerable.Range(0, 5 + rnd.Next() % 100).Select(x => (BigFraction)rnd.NextDouble()).ToArray();
        BigFraction sum = 0;
        foreach (var num in nums)
        {
            sum += num;
        }

        Assert.True(sum.Equals(nums.SumBigFraction()));
    }
}
