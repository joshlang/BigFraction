using System;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Epoche;

public class BigFractionTests_MoreTestCases
{
    [Fact]
    [Trait("Type", "Unit")]
    public void Add_BigInteger_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var num2 = (long)RandomHelper.GetRandomInt32();
            var val1 = (BigFraction)num1;
            var val2 = (BigInteger)num2;
            Assert.Equal((num1 + num2).ToString(), (val1 + val2).ToString());
            Assert.Equal((num1 + num2).ToString(), (val2 + val1).ToString());
            Assert.Equal(val1.Denominator, (val1 + val2).Denominator);
            Assert.Equal(val1.DecimalString, (val1 + val2).DecimalString);
            Assert.Equal(val1.Denominator, (val2 + val1).Denominator);
            Assert.Equal(val1.DecimalString, (val2 + val1).DecimalString);
            Assert.True(num1 + num2 == val1 + val2);
            Assert.True(num1 + num2 == val2 + val1);
            Assert.True(val1 + val2 == num1 + num2);
            Assert.True(val2 + val1 == num1 + num2);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Add_Int64_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var num2 = (long)RandomHelper.GetRandomInt32();
            var val1 = (BigFraction)num1;
            Assert.True(num1 + num2 == val1 + num2);
            Assert.True(num1 + num2 == num2 + val1);
            Assert.True(val1 + num2 == num1 + num2);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Subtract_BigInteger_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var num2 = (long)RandomHelper.GetRandomInt32();
            var val1 = (BigFraction)num1;
            var val2 = (BigInteger)num2;
            Assert.Equal((num1 - num2).ToString(), (val1 - val2).ToString());
            Assert.Equal((num2 - num1).ToString(), (val2 - val1).ToString());
            Assert.Equal(val1.Denominator, (val1 - val2).Denominator);
            Assert.Equal(val1.DecimalString, (val1 - val2).DecimalString);
            Assert.Equal(val1.Denominator, (val2 - val1).Denominator);
            Assert.Equal(val1.DecimalString, (val2 - val1).DecimalString);
            Assert.True(num1 - num2 == val1 - val2);
            Assert.True(num2 - num1 == val2 - val1);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Subtract_Int64_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var num2 = (long)RandomHelper.GetRandomInt32();
            var val1 = (BigFraction)num1;
            var val2 = num2;
            Assert.Equal((num1 - num2).ToString(), (val1 - val2).ToString());
            Assert.Equal((num2 - num1).ToString(), (val2 - val1).ToString());
            Assert.Equal(val1.Denominator, (val1 - val2).Denominator);
            Assert.Equal(val1.DecimalString, (val1 - val2).DecimalString);
            Assert.Equal(val1.Denominator, (val2 - val1).Denominator);
            Assert.Equal(val1.DecimalString, (val2 - val1).DecimalString);
            Assert.True(num1 - num2 == val1 - val2);
            Assert.True(num2 - num1 == val2 - val1);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Add_WholeBigFractions_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var num2 = (long)RandomHelper.GetRandomInt32();
            var val1 = (BigFraction)num1;
            var val2 = (BigFraction)num2;
            Assert.Equal((num1 + num2).ToString(), (val2 + val1).ToString());
            Assert.True(num1 + num2 == val1 + val2);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Add_WholeAndDecimalBigFractions_ReturnsCorrectResults()
    {
        var rnd = new Random();
        for (var x = 0; x < 100; ++x)
        {
            var num1 = Math.Abs((long)RandomHelper.GetRandomInt32());
            var num2 = Math.Abs(rnd.NextDouble() * 1000000000 - 500000000);
            var val1 = (BigFraction)num1;
            var val2 = (BigFraction)num2;
            var sum = val1 + val2;
            Assert.Equal(sum.WholeNumber, val1.WholeNumber + val2.WholeNumber);
            Assert.Equal(sum.DecimalString, val2.DecimalString);
            Assert.Equal(sum.Denominator, val2.Denominator);
            Assert.Equal(sum.Denominator, val2.Denominator);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Add_BigFractions_ReturnsCorrectResults()
    {
        var rnd = new Random();
        for (var x = 0; x < 100; ++x)
        {
            var num1 = rnd.NextDouble() * 1000000000 - 500000000;
            var num2 = rnd.NextDouble() * 1000000000 - 500000000;
            var val1 = (BigFraction)num1;
            var val2 = (BigFraction)num2;
            var sum = val1 + val2;
            Assert.Equal(sum.WholeNumber, (long)Math.Truncate(num1 + num2));
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Subtract_BigFractions_ReturnsCorrectResults()
    {
        var rnd = new Random();
        for (var x = 0; x < 100; ++x)
        {
            var num1 = rnd.NextDouble() * 1000000000 - 500000000;
            var num2 = rnd.NextDouble() * 1000000000 - 500000000;
            var val1 = (BigFraction)num1;
            var val2 = (BigFraction)num2;
            var diff = val1 - val2;
            Assert.Equal(diff.WholeNumber, (long)Math.Truncate(num1 - num2));
        }
    }
    [Theory]
    [InlineData("0", "0", "0")]
    [InlineData("1", "1", "0")]
    [InlineData("0", "1", "1")]
    [InlineData("-1", "1", "2")]
    [InlineData("-1", "-1", "0")]
    [InlineData("0", "-1", "-1")]
    [InlineData("1", "-1", "-2")]
    [InlineData("-2", "-1", "1")]
    [InlineData("0.1", "0.3", "0.2")]
    [InlineData("0.24", "0.37", "0.13")]
    [InlineData("1.24", "1.37", "0.13")]
    [InlineData("1.7", "1.1", "-0.6")]
    [InlineData("-1.7", "-1.1", "0.6")]
    [InlineData("-0.55", "-1.15", "-0.6")]
    [InlineData("0", "-0.234", "-0.234")]
    [InlineData("0", "-0", "0")]
    [InlineData("0", "-0", "-0")]
    [Trait("Type", "Unit")]
    public void Subtract_TestCases_ReturnsCorrectResults(string diffstr, string val1, string val2)
    {
        var num1 = BigFraction.Parse(val1);
        var num2 = BigFraction.Parse(val2);
        var diff = num1 - num2;
        Assert.Equal(diffstr, diff.ToString());
    }
    [Theory]
    [InlineData("0", "0", "0", 0, 1)]
    [InlineData("1", "1", "0", 0, 1)]
    [InlineData("2", "1", "1", 0, 1)]
    [InlineData("3", "1", "2", 0, 1)]
    [InlineData("-1", "-1", "0", 0, 1)]
    [InlineData("-2", "-1", "-1", 0, 1)]
    [InlineData("-3", "-1", "-2", 0, 1)]
    [InlineData("0", "-1", "1", 0, 1)]
    [InlineData("0.5", "0.3", "0.2", 1, 10)]
    [InlineData("0.5", "0.37", "0.13", 1, 10)]
    [InlineData("1.5", "1.37", "0.13", 1, 10)]
    [InlineData("0.5", "1.1", "-0.6", 1, 10)]
    [InlineData("-0.5", "-1.1", "0.6", 1, 10)]
    [InlineData("-1.75", "-1.15", "-0.6", 2, 100)]
    [InlineData("0", "-0.234", "0.234", 0, 1)]
    [InlineData("0", "-0", "0", 0, 1)]
    [InlineData("0", "-0", "-0", 0, 1)]
    [Trait("Type", "Unit")]
    public void Add_TestCases_ReturnsCorrectResults(string sum, string val1, string val2, int decLength, int denominator)
    {
        var num1 = BigFraction.Parse(val1);
        var num2 = BigFraction.Parse(val2);
        var numSum = num1 + num2;
        var numSum2 = num2 + num1;
        Assert.Equal(numSum, numSum2);
        Assert.Equal(sum, numSum.ToString());
        Assert.Equal(decLength, numSum.DecimalString.Length);
        Assert.Equal(denominator, numSum.Denominator);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Negate_BigFractions_ReturnCorrectResults()
    {
        var rnd = new Random();
        for (var x = 0; x < 100; ++x)
        {
            var num = (BigFraction)RandomHelper.GetRandomInt64();
            if (num.IsZero)
            {
                continue;
            }

            Assert.Equal(num.ToString(), ("-" + (-num).ToString()).Replace("--", ""));
            num = (BigFraction)(rnd.NextDouble() * 10000 - 5000);
            if (num.IsZero)
            {
                continue;
            }

            Assert.Equal(num.ToString(), ("-" + (-num).ToString()).Replace("--", ""));
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Negate_Zero_ReturnsZero()
    {
        Assert.Equal(BigFraction.Zero, -BigFraction.Zero);
        Assert.Equal(BigFraction.Zero.ToString(), (-BigFraction.Zero).ToString());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Multiply_WholeBigFractions_ReturnCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var num2 = (long)RandomHelper.GetRandomInt32();
            var val1 = (BigFraction)num1;
            var val2 = (BigFraction)num2;
            Assert.Equal((num1 * num2).ToString(), (val2 * val1).ToString());
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Multiply_BigInteger_ReturnCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var num2 = (long)RandomHelper.GetRandomInt32();
            var val1 = (BigFraction)num1;
            var val2 = (BigInteger)num2;
            Assert.Equal((num1 * num2).ToString(), (val2 * val1).ToString());
            Assert.Equal((num1 * num2).ToString(), (val1 * val2).ToString());
            Assert.True(val1 * val2 == val2 * val1);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Multiply_TwoBigFractions_ReturnCorrectResults()
    {
        var rnd = new Random();
        for (var x = 0; x < 100; ++x)
        {
            var num1 = decimal.Round((decimal)(rnd.NextDouble() * 100000 - 50000), 6);
            var num2 = decimal.Round((decimal)(rnd.NextDouble() * 100000 - 50000), 6);
            var val1 = (BigFraction)num1;
            var val2 = (BigFraction)num2;
            var mul = val1 * val2;
            Assert.True(mul == BigFraction.Parse((num1 * num2).ToString()));
        }
    }
    [Theory]
    [InlineData("0", "0", "0", 0, 1)]
    [InlineData("0", "1", "0", 0, 1)]
    [InlineData("1", "1", "1", 0, 1)]
    [InlineData("2", "1", "2", 0, 1)]
    [InlineData("0", "-1", "0", 0, 1)]
    [InlineData("1", "-1", "-1", 0, 1)]
    [InlineData("2", "-1", "-2", 0, 1)]
    [InlineData("-1", "-1", "1", 0, 1)]
    [InlineData("0.06", "0.3", "0.2", 2, 100)]
    [InlineData("0.0481", "0.37", "0.13", 4, 10000)]
    [InlineData("0.1781", "1.37", "0.13", 4, 10000)]
    [InlineData("-0.66", "1.1", "-0.6", 2, 100)]
    [InlineData("-0.66", "-1.1", "0.6", 2, 100)]
    [InlineData("0.69", "-1.15", "-0.6", 2, 100)]
    [InlineData("-0.054756", "-0.234", "0.234", 6, 1000000)]
    [InlineData("0", "-0", "0", 0, 1)]
    [InlineData("0", "-0", "-0", 0, 1)]
    [Trait("Type", "Unit")]
    public void Multiply_TestCases_ReturnCorrectResults(string mul, string val1, string val2, int decLength, int denominator)
    {
        var num1 = BigFraction.Parse(val1);
        var num2 = BigFraction.Parse(val2);
        var numMul = num1 * num2;
        var numMul2 = num2 * num1;
        Assert.Equal(numMul, numMul2);
        Assert.Equal(mul, numMul.ToString());
        Assert.Equal(decLength, numMul.DecimalString.Length);
        Assert.Equal(denominator, numMul.Denominator);
    }

    [Theory]
    [InlineData(0, 0, "0")]
    [InlineData(123, 0, "123")]
    [InlineData(123, 1, "12.3")]
    [InlineData(123, 2, "1.23")]
    [InlineData(123, 3, "0.123")]
    [InlineData(123, 4, "0.0123")]
    [InlineData(-123, 5, "-0.00123")]
    [InlineData(100, 0, "100")]
    [InlineData(100, 1, "10")]
    [InlineData(100, 2, "1")]
    [InlineData(100, 3, "0.1")]
    [InlineData(1010, 3, "1.01")]
    [InlineData(-1010, 3, "-1.01")]
    [Trait("Type", "Unit")]
    public void DividePow10_TestCases_ReturnCorrectResults(long start, int shift, string end)
    {
        var f = BigFraction.DividePow10(start, shift);
        Assert.Equal(end, f.ToString());
        Assert.Equal(f, ((BigFraction)start).DividePow10(shift));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Comparisons_Int32_ReturnCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = RandomHelper.GetRandomInt32();
            var num2 = RandomHelper.GetRandomInt32();
            var bf1 = (BigFraction)num1;
            Assert.Equal(num1 < num2, bf1 < num2);
            Assert.Equal(num1 <= num2, bf1 <= num2);
            Assert.Equal(num1 > num2, bf1 > num2);
            Assert.Equal(num1 >= num2, bf1 >= num2);
            Assert.Equal(num1 == num2, bf1 == num2);
            Assert.Equal(num1 != num2, bf1 != num2);

            Assert.Equal(num1.CompareTo(num2), bf1.CompareTo(num2));
            Assert.True(-num1 == -bf1);
            Assert.True(!(-num1 != -bf1));

            Assert.Equal(num2 < num1, num2 < bf1);
            Assert.Equal(num2 <= num1, num2 <= bf1);
            Assert.Equal(num2 > num1, num2 > bf1);
            Assert.Equal(num2 >= num1, num2 >= bf1);
            Assert.Equal(num2 == num1, num2 == bf1);
            Assert.Equal(num2 != num1, num2 != bf1);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Comparisons_Int64_ReturnCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = RandomHelper.GetRandomInt64();
            var num2 = RandomHelper.GetRandomInt64();
            var bf1 = (BigFraction)num1;
            Assert.Equal(num1 < num2, bf1 < num2);
            Assert.Equal(num1 <= num2, bf1 <= num2);
            Assert.Equal(num1 > num2, bf1 > num2);
            Assert.Equal(num1 >= num2, bf1 >= num2);
            Assert.Equal(num1 == num2, bf1 == num2);
            Assert.Equal(num1 != num2, bf1 != num2);

            Assert.Equal(num1.CompareTo(num2), bf1.CompareTo(num2));
            Assert.True(-num1 == -bf1);
            Assert.True(!(-num1 != -bf1));

            Assert.Equal(num2 < num1, num2 < bf1);
            Assert.Equal(num2 <= num1, num2 <= bf1);
            Assert.Equal(num2 > num1, num2 > bf1);
            Assert.Equal(num2 >= num1, num2 >= bf1);
            Assert.Equal(num2 == num1, num2 == bf1);
            Assert.Equal(num2 != num1, num2 != bf1);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Comparisons_BigInteger_ReturnCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = RandomHelper.GetRandomInt64();
            var num2 = (BigInteger)RandomHelper.GetRandomInt64();
            var bf1 = (BigFraction)num1;
            Assert.Equal(num1 < num2, bf1 < num2);
            Assert.Equal(num1 <= num2, bf1 <= num2);
            Assert.Equal(num1 > num2, bf1 > num2);
            Assert.Equal(num1 >= num2, bf1 >= num2);
            Assert.Equal(num1 == num2, bf1 == num2);
            Assert.Equal(num1 != num2, bf1 != num2);

            Assert.Equal(((BigInteger)num1).CompareTo(num2), bf1.CompareTo(num2));
            Assert.True(-num1 == -bf1);
            Assert.True(!(-num1 != -bf1));

            Assert.Equal(num2 < num1, num2 < bf1);
            Assert.Equal(num2 <= num1, num2 <= bf1);
            Assert.Equal(num2 > num1, num2 > bf1);
            Assert.Equal(num2 >= num1, num2 >= bf1);
            Assert.Equal(num2 == num1, num2 == bf1);
            Assert.Equal(num2 != num1, num2 != bf1);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Comparisons_BigFractions_ReturnCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = RandomHelper.GetRandomInt64();
            var num2 = RandomHelper.GetRandomInt64();
            var bf1 = (BigFraction)num1;
            var bf2 = (BigFraction)num2;
            Assert.Equal(num1 < num2, bf1 < bf2);
            Assert.Equal(num1 <= num2, bf1 <= bf2);
            Assert.Equal(num1 > num2, bf1 > bf2);
            Assert.Equal(num1 >= num2, bf1 >= bf2);
            Assert.Equal(num1 == num2, bf1 == bf2);
            Assert.Equal(num1 != num2, bf1 != bf2);

            Assert.Equal(num1.CompareTo(num2), bf1.CompareTo(num2));
            Assert.True(-num1 == -bf1);
            Assert.True(!(-num1 != -bf1));

            Assert.Equal(num2 < num1, bf2 < bf1);
            Assert.Equal(num2 <= num1, bf2 <= bf1);
            Assert.Equal(num2 > num1, bf2 > bf1);
            Assert.Equal(num2 >= num1, bf2 >= bf1);
            Assert.Equal(num2 == num1, bf2 == bf1);
            Assert.Equal(num2 != num1, bf2 != bf1);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Abs_BigFraction_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var bf1 = (BigFraction)num1;
            Assert.True(Math.Abs(num1) == BigFraction.Abs(bf1));
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Sign_BigFraction_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var bf1 = (BigFraction)num1;
            Assert.Equal(Math.Sign(num1), bf1.Sign);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void UnaryPlus_BigFraction_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var bf1 = (BigFraction)num1;
            Assert.True(bf1.Equals(+bf1));
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void UnaryMinus_BigFraction_ReturnsCorrectResults()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (long)RandomHelper.GetRandomInt32();
            var bf1 = (BigFraction)num1;
            Assert.True(bf1.Equals(-(-bf1)));
            Assert.True(bf1.Equals(-(0 - bf1)));
        }
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    [InlineData(0)]
    [Trait("Type", "Unit")]
    public void Operations_EdgeCases_ReturnCorrectResults(long val)
    {
        BigInteger x = RandomHelper.GetRandomInt64();
        BigInteger bigval = val;
        var f = (BigFraction)x;

        Assert.True((f + val).Equals(x + bigval));
        Assert.True((f - val).Equals(x - bigval));
        Assert.True((f * val).Equals(x * bigval));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Equal_ApproximateDouble_ReturnsCorrectResults()
    {
        var rnd = new Random();
        for (var x = 0; x < 100; ++x)
        {
            var num1 = (decimal)rnd.NextDouble();
            Assert.Equal(num1, ((BigFraction)num1).ToApproximateDecimal());
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void OrderBy_BigFraction_ReturnsCorrectResults()
    {
        var nums = new BigFraction[] { -1, (BigFraction)(-0.051), (BigFraction)(-0.05), BigFraction.Zero, (BigFraction)0.02, (BigFraction)0.05, BigFraction.Parse("12.34"), 30, 99 };
        var rnd = new Random();
        for (var a = 0; a < 100; ++a)
        {
            var sorted =
                nums
                .OrderBy(x => rnd.Next())
                .OrderBy(x => x)
                .ToList();
            for (var z = 0; z < nums.Length; ++z)
            {
                Assert.True(nums[z] == sorted[z]);
            }
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void AddBigInteger_PositivePositive_ReturnsCorrectResults()
    {
        BigFraction f = 0.01m;
        var f2 = f + (BigInteger)1;
        Assert.Equal(101, f2.Numerator);
        Assert.Equal(100, f2.Denominator);
        Assert.Equal("01", f2.DecimalString);
        Assert.Equal(1, f2.WholeNumber);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void AddBigInteger_PositiveNegative_ReturnsCorrectResults()
    {
        BigFraction f = 0.01m;
        var f2 = f + (BigInteger)(-1);
        Assert.Equal(-99, f2.Numerator);
        Assert.Equal(100, f2.Denominator);
        Assert.Equal("99", f2.DecimalString);
        Assert.Equal(0, f2.WholeNumber);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void AddBigInteger_NegativePositive_ReturnsCorrectResults()
    {
        BigFraction f = -0.01m;
        var f2 = f + (BigInteger)1;
        Assert.Equal(99, f2.Numerator);
        Assert.Equal(100, f2.Denominator);
        Assert.Equal("99", f2.DecimalString);
        Assert.Equal(0, f2.WholeNumber);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void AddBigInteger_NegativeNegative_ReturnsCorrectResults()
    {
        BigFraction f = -0.01m;
        var f2 = f + (BigInteger)(-1);
        Assert.Equal(-101, f2.Numerator);
        Assert.Equal(100, f2.Denominator);
        Assert.Equal("01", f2.DecimalString);
        Assert.Equal(-1, f2.WholeNumber);
    }

    [Theory]
    [InlineData("\0")]
    [InlineData("0\0")]
    [InlineData("\00")]
    [InlineData("\0-0.1")]
    [InlineData("-\00.1")]
    [InlineData("-0\0.1")]
    [InlineData("-0.\01")]
    [InlineData("-0.1\0")]
    [Trait("Type", "Unit")]
    public void ParseFails(string str) => Assert.Throws<FormatException>(() => BigFraction.Parse(str));

    [Theory]
    [InlineData("\0")]
    [InlineData("0\0")]
    [InlineData("\00")]
    [InlineData("\0-0.1")]
    [InlineData("-\00.1")]
    [InlineData("-0\0.1")]
    [InlineData("-0.\01")]
    [InlineData("-0.1\0")]
    [Trait("Type", "Unit")]
    public void TryParseFails(string str) => Assert.False(BigFraction.TryParse(str, out _));

    [Theory]
    [InlineData("\0")]
    [InlineData("0\0")]
    [InlineData("\00")]
    [InlineData("\0-0.1")]
    [InlineData("-\00.1")]
    [InlineData("-0\0.1")]
    [InlineData("-0.\01")]
    [InlineData("-0.1\0")]
    [Trait("Type", "Unit")]
    public void TryParseDefaultReturnsDefault(string str) => Assert.True(BigFraction.TryParseDefault(str, 123) == 123);
}
