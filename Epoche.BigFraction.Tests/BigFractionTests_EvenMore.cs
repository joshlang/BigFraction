using System;
using System.Numerics;
using Xunit;

namespace Epoche;

public class BigFractionTests_EvenMore
{
    static void AssertEqual(BigFraction f1, BigFraction f2)
    {
        Assert.Equal(f1.DecimalString, f2.DecimalString);
        Assert.Equal(f1.Denominator, f2.Denominator);
        Assert.Equal(f1.IsZero, f2.IsZero);
        Assert.Equal(f1.Numerator, f2.Numerator);
        Assert.Equal(f1.Sign, f2.Sign);
        Assert.Equal(f1.WholeNumber, f2.WholeNumber);
        Assert.Equal(f1.ToApproximateDecimal(), f2.ToApproximateDecimal());
        Assert.Equal(f1.ToApproximateDouble(), f2.ToApproximateDouble());
        Assert.True(f1 == f2);
        Assert.False(f1 != f2);
        Assert.Equal(0, f1.CompareTo(f2));
        Assert.Equal(0, f2.CompareTo(f1));
        Assert.False(f1 < f2);
        Assert.False(f2 < f1);
        Assert.False(f1 > f2);
        Assert.False(f2 > f1);
        Assert.True(f1 >= f2);
        Assert.True(f1 <= f2);
        Assert.True(f2 >= f1);
        Assert.True(f2 <= f1);
        Assert.True(f1.Equals(f2));
        Assert.True(f2.Equals(f1));
        Assert.Equal(f1.GetHashCode(), f2.GetHashCode());
        Assert.Equal(f1.ToString(), f2.ToString());
        Assert.Equal(-f1, -f2);
        Assert.Equal(f1, f2.Truncate(f1.DecimalString.Length));
        Assert.Equal(f2, f1.Truncate(f2.DecimalString.Length));
        Assert.Equal(f1, +f2);
        Assert.Equal(f2, +f1);
        Assert.Equal(f1.DividePow10(0), f2);
        Assert.Equal(f2.DividePow10(0), f1);
        Assert.Equal(BigFraction.Abs(f1), f2 < 0 ? -f2 : f2);
        Assert.Equal(BigFraction.Abs(f2), f1 < 0 ? -f1 : f1);
    }
    readonly Random Random = new();
    double RandomDouble() => (Random.NextDouble() - 0.5) * RandomHelper.GetRandomInt64();

    [Fact]
    [Trait("Type", "Unit")]
    public void AddingSubtracting_BigFraction_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = (BigFraction)RandomDouble();

            var result = f1 + f2;
            result -= f2;
            AssertEqual(f1, result);

            result = f1 + f2;
            result -= f1;
            AssertEqual(f2, result);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void AddingSubtracting_BigInteger_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = (BigInteger)RandomHelper.GetRandomInt64();

            var result = f1 + f2;
            result -= f2;
            AssertEqual(f1, result);

            result = f1 + f2;
            result -= f1;
            AssertEqual(f2, result);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void AddingSubtracting_Int64_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = RandomHelper.GetRandomInt64();

            var result = f1 + f2;
            result -= f2;
            AssertEqual(f1, result);

            result = f1 + f2;
            result -= f1;
            AssertEqual(f2, result);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Adding_BigFraction_BothWays_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = (BigFraction)RandomDouble();

            AssertEqual(f1 + f2, f2 + f1);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Adding_BigInteger_BothWays_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = (BigInteger)RandomHelper.GetRandomInt64();

            AssertEqual(f1 + f2, f2 + f1);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Adding_Int64_BothWays_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = RandomHelper.GetRandomInt64();

            AssertEqual(f1 + f2, f2 + f1);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Negate_Equals_SubtractFromZero()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();

            AssertEqual(-f1, 0 - f1);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Multiply_BigFraction_ThenSubtract_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = f1 * (BigFraction)2;
            f2 -= f1;

            AssertEqual(f1, f2);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Multiply_BigInteger_ThenSubtract_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = f1 * (BigInteger)2;
            f2 -= f1;

            AssertEqual(f1, f2);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Multiply_Int64_ThenSubtract_YieldsSame()
    {
        for (var x = 0; x < 100; ++x)
        {
            var f1 = (BigFraction)RandomDouble();
            var f2 = f1 * (long)2;
            f2 -= f1;

            AssertEqual(f1, f2);
        }
    }
}
