using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Epoche;

public class BigFractionTests
{
    static BigFraction BF_Zero => BigFraction.Zero;
    static BigFraction BF_ZeroPointNine => BigFraction.Parse("0.9");
    static BigFraction BF_One => 1;
    static BigFraction BF_OnePointOne => BigFraction.Parse("1.1");
    static BigFraction BF_Two => 2;
    static BigFraction BF_Three => 3;
    static BigFraction BF_MinusZeroPointNine => BigFraction.Parse("-0.9");
    static BigFraction BF_MinusOne => -1;
    static BigFraction BF_MinusOnePointOne => BigFraction.Parse("-1.1");
    static BigFraction BF_MinusTwo => -2;
    static BigFraction BF_MinusThree => -3;

    static BigInteger BI_Zero => BigInteger.Zero;
    static BigInteger BI_One => 1;
    static BigInteger BI_Two => 2;
    static BigInteger BI_Three => 3;
    static BigInteger BI_MinusOne => -1;
    static BigInteger BI_MinusTwo => -2;
    static BigInteger BI_MinusThree => -3;

    static long I64_One => 1;
    static long I64_Two => 2;
    static long I64_MinusOne => -1;
    static long I64_MinusTwo => -2;
    static long I64_MinusThree => -3;

    static int I32_One => 1;
    static int I32_Two => 2;
    static int I32_Three => 3;
    static int I32_MinusOne => -1;
    static int I32_MinusTwo => -2;

    public static IEnumerable<object[]> InvalidStrings => new[]
    {
        "",
        " ",
        ".",
        "0.",
        "1.",
        "1.2.3",
        "a",
        "0a",
        "a0",
        "0.2.",
        "0..2",
        "1..2",
        "01,33",
        " 0.2",
        " 0.2 ",
        "0.2 ",
        "0.2z",
        "z0.2",
        "0z.2",
        "0.z2",
        "0.-1",
    }.Select(x => new object[] { x });

    /// <summary>
    /// These are normalized.  In other words, 000.000 -> 0.  -010.10 -> -10.1
    /// </summary>
    public static IEnumerable<object[]> ValidMinimalStrings => new[]
    {
        "0",
        "1",
        "-1",
        "10",
        "-10",
        "109",
        "-109",
        "0.1",
        "-0.1",
        "0.01",
        "-0.01",
        "5.01",
        "-5.01",
        "50.01",
        "-50.01",
        "1234.987605",
        "-1234.987605",
        "0.0203405",
        "-0.0203405",
        "1.0203405",
        "-1.0203405"
    }.Select(x => new object[] { x });

    public static IEnumerable<object[]> ValidStringNumeratorDenominators = new[]
    {
        ("0", 0, 1),
        ("0.0", 0, 1),
        ("000000000000000000000.000000000000000000000", 0, 1),
        (".0", 0, 1),
        (".001", 1, 1000),
        ("1", 1, 1),
        ("1.0", 1, 1),
        ("2", 2, 1),
        ("10", 10, 1),
        ("01", 1, 1),
        ("001", 1, 1),
        ("001010", 1010, 1),
        ("0.1", 1, 10),
        ("1.1", 11, 10),
        ("2.1", 21, 10),
        ("0002.1000", 21, 10),
        (".1000", 1, 10),
        ("1000.1000", 10001, 10),
        ("12345678.90123456789", 1234567890123456789, 100000000000),
        ("0000000000000000000000000000000000000000000000000000000000000000000012345678.9012345678900000000000000000000000000000000000000000000000000000000000000000000", 1234567890123456789, 100000000000),
        ("-0", 0, 1),
        ("-0.0", 0, 1),
        ("-000000000000000000000.000000000000000000000", 0, 1),
        ("-.0", 0, 1),
        ("-.001", -1, 1000),
        ("-1", -1, 1),
        ("-1.0", -1, 1),
        ("-2", -2, 1),
        ("-10", -10, 1),
        ("-01", -1, 1),
        ("-001", -1, 1),
        ("-001010", -1010, 1),
        ("-0.1", -1, 10),
        ("-1.1", -11, 10),
        ("-2.1", -21, 10),
        ("-0002.1000", -21, 10),
        ("-.1000", -1, 10),
        ("-1000.1000", -10001, 10),
        ("-12345678.90123456789", -1234567890123456789, 100000000000),
        ("-0000000000000000000000000000000000000000000000000000000000000000000012345678.9012345678900000000000000000000000000000000000000000000000000000000000000000000", -1234567890123456789, 100000000000)
    }.Select(x => new object[] { x.Item1, x.Item2, x.Item3 });

    [Theory]
    [InlineData(typeof(IEquatable<BigFraction>))]
    [InlineData(typeof(IEquatable<BigInteger>))]
    [InlineData(typeof(IEquatable<long>))]
    [InlineData(typeof(IEquatable<int>))]
    [InlineData(typeof(IComparable<BigFraction>))]
    [InlineData(typeof(IComparable<BigInteger>))]
    [InlineData(typeof(IComparable<long>))]
    [InlineData(typeof(IComparable<int>))]
    [Trait("Type", "Unit")]
    public void BigFraction_ImplementsExpectedInterfaces(Type interfaceType) => Assert.Contains(interfaceType, typeof(BigFraction).GetInterfaces());

    [Fact]
    [Trait("Type", "Unit")]
    public void Zero_IsZero() => Assert.True(BigFraction.Zero.IsZero);

    [Fact]
    [Trait("Type", "Unit")]
    public void Parse_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => BigFraction.Parse(null!));

    [Theory]
    [MemberData(nameof(InvalidStrings))]
    [Trait("Type", "Unit")]
    public void Parse_Invalid_ThrowsFormatException(string s) => Assert.Throws<FormatException>(() => BigFraction.Parse(s));

    [Fact]
    [Trait("Type", "Unit")]
    public void TryParse_Null_ReturnsFalse() => Assert.False(BigFraction.TryParse(null!, out _));

    [Theory]
    [MemberData(nameof(InvalidStrings))]
    [Trait("Type", "Unit")]
    public void TryParse_Invalid_ReturnsFalse(string s) => Assert.False(BigFraction.TryParse(s, out _));


    [Theory]
    [MemberData(nameof(InvalidStrings))]
    [Trait("Type", "Unit")]
    public void TryParseDefault_Invalid_ReturnsZero(string s) => Assert.True(BigFraction.TryParseDefault(s).IsZero);

    [Theory]
    [MemberData(nameof(InvalidStrings))]
    [Trait("Type", "Unit")]
    public void TryParseDefault_Invalid_ReturnsDefault(string s) => Assert.True(BigFraction.TryParseDefault(s, 1) == 1);

    [Theory]
    [MemberData(nameof(ValidMinimalStrings))]
    [Trait("Type", "Unit")]
    public void Parse_ValidMinimalStrings_ToStringYieldsSameString(string s) => Assert.Equal(s, BigFraction.Parse(s).ToString());

    [Fact]
    [Trait("Type", "Unit")]
    public void ToString_Negative_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => BigFraction.Zero.ToString(-1));

    [Theory]
    [MemberData(nameof(ValidMinimalStrings))]
    [Trait("Type", "Unit")]
    public void Parse_ValidMinimalStrings_ToStringYieldsSameString_MaxLength(string s) => Assert.Equal(s, BigFraction.Parse(s).ToString(int.MaxValue));

    [Theory]
    [MemberData(nameof(ValidMinimalStrings))]
    [Trait("Type", "Unit")]
    public void Parse_ValidMinimalStrings_ToStringYieldsWhole_0(string s) => Assert.Equal(s.Split('.')[0].Replace("-0", "0"), BigFraction.Parse(s).ToString(0));

    [Theory]
    [MemberData(nameof(ValidMinimalStrings))]
    [Trait("Type", "Unit")]
    public void Parse_ValidMinimalStrings_ToStringMatchesDecimals(string s)
    {
        var parts = s.Split('.');
        if (parts.Length == 1)
        {
            return;
        }
        var declen = parts[1].Length;
        for (var x = 0; x <= declen; ++x)
        {
            var trunc = BigFraction.Parse(s).ToString(x);
            var test = $"{parts[0]}.{parts[1][..x]}0";
            Assert.Equal(BigFraction.Parse(test).ToString(), trunc);
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameBigFraction_ReturnsTrue()
    {
        var f = (new Random().NextDouble() * 100 - 50).ToString();
        Assert.True(BigFraction.Parse(f).Equals(BigFraction.Parse(f)));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentBigFraction_ReturnsFalse()
    {
        var f = (new Random().NextDouble() * 100 - 50).ToString();
        Assert.False(BigFraction.Parse(f).Equals(BigFraction.Parse(f) + 1));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameInBigFraction_ReturnsTrue()
    {
        var f = (new Random().NextDouble() * 100 - 50).ToString();
        var f2 = BigFraction.Parse(f);
        Assert.True(BigFraction.Parse(f).Equals(in f2));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentInBigFraction_ReturnsFalse()
    {
        var f = (new Random().NextDouble() * 100 - 50).ToString();
        var f2 = BigFraction.Parse(f) + 1;
        Assert.False(BigFraction.Parse(f).Equals(in f2));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameBigInteger_ReturnsTrue()
    {
        var f = RandomHelper.GetRandomInt64();
        Assert.True(((BigFraction)f).Equals((BigInteger)f));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentBigInteger_ReturnsFalse()
    {
        var f = RandomHelper.GetRandomInt64();
        Assert.False(((BigFraction)f).Equals((BigInteger)f + 1));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameInt64_ReturnsTrue()
    {
        var f = RandomHelper.GetRandomInt64();
        Assert.True(((BigFraction)f).Equals(f));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentInt64_ReturnsFalse()
    {
        var f = RandomHelper.GetRandomInt64();
        Assert.False(((BigFraction)f).Equals(f + 1));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameInt32_ReturnsTrue()
    {
        var f = RandomHelper.GetRandomInt32();
        Assert.True(((BigFraction)f).Equals(f));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentInt32_ReturnsFalse()
    {
        var f = RandomHelper.GetRandomInt32();
        Assert.False(((BigFraction)f).Equals(f + 1));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void GetHashCode_DifferentValues_DifferentCode() => Assert.NotEqual(BF_One.GetHashCode(), BF_MinusOne.GetHashCode());
    [Fact]
    [Trait("Type", "Unit")]
    public void GetHashCode_SameValues_SameCode() => Assert.Equal(BF_Two.GetHashCode(), BF_Two.GetHashCode());

    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_LargerBigFraction_ReturnsMinus1() => Assert.Equal(-1, BF_One.CompareTo(BF_OnePointOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_EqualBigFraction_Returns0() => Assert.Equal(0, BF_OnePointOne.CompareTo(BF_OnePointOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_SmallerBigFraction_Returns1() => Assert.Equal(1, BF_One.CompareTo(BF_ZeroPointNine));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeLargerBigFraction_ReturnsMinus1() => Assert.Equal(-1, BF_MinusOne.CompareTo(BF_MinusZeroPointNine));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeEqualBigFraction_Returns0() => Assert.Equal(0, (BF_MinusOnePointOne).CompareTo(BF_MinusOnePointOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeSmallerBigFraction_Returns1() => Assert.Equal(1, BF_MinusOne.CompareTo(BF_MinusOnePointOne));

    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_LargerBigInteger_ReturnsMinus1() => Assert.Equal(-1, BF_One.CompareTo(BI_Two));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_EqualBigInteger_Returns0() => Assert.Equal(0, BF_One.CompareTo(BI_One));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_SmallerBigInteger_Returns1() => Assert.Equal(1, BF_OnePointOne.CompareTo(BI_One));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeLargerBigInteger_ReturnsMinus1() => Assert.Equal(-1, BF_MinusTwo.CompareTo(BI_MinusOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeEqualBigInteger_Returns0() => Assert.Equal(0, BF_MinusOne.CompareTo(BI_MinusOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeSmallerBigInteger_Returns1() => Assert.Equal(1, BF_MinusOne.CompareTo(BI_MinusTwo));

    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_LargerInt64_ReturnsMinus1() => Assert.Equal(-1, BF_One.CompareTo(I64_Two));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_EqualInt64_Returns0() => Assert.Equal(0, BF_One.CompareTo(I64_One));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_SmallerInt64_Returns1() => Assert.Equal(1, BF_OnePointOne.CompareTo(I64_One));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeLargerInt64_ReturnsMinus1() => Assert.Equal(-1, BF_MinusTwo.CompareTo(I64_MinusOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeEqualInt64_Returns0() => Assert.Equal(0, BF_MinusOne.CompareTo(I64_MinusOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeSmallerInt64_Returns1() => Assert.Equal(1, BF_MinusOne.CompareTo(I64_MinusTwo));

    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_LargerInt32_ReturnsMinus1() => Assert.Equal(-1, BF_One.CompareTo(I32_Two));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_EqualInt32_Returns0() => Assert.Equal(0, BF_One.CompareTo(I32_One));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_SmallerInt32_Returns1() => Assert.Equal(1, BF_OnePointOne.CompareTo(I32_One));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeLargerInt32_ReturnsMinus1() => Assert.Equal(-1, BF_MinusTwo.CompareTo(I32_MinusOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeEqualInt32_Returns0() => Assert.Equal(0, BF_MinusOne.CompareTo(I32_MinusOne));
    [Fact]
    [Trait("Type", "Unit")]
    public void CompareTo_NegativeSmallerInt32_Returns1() => Assert.Equal(1, BF_MinusOne.CompareTo(I32_MinusTwo));

    [Fact]
    [Trait("Type", "Unit")]
    public void IsZero_0_IsTrue() => Assert.True(((BigFraction)0).IsZero);
    [Fact]
    [Trait("Type", "Unit")]
    public void IsZero_Positive_IsFalse() => Assert.False(BF_ZeroPointNine.IsZero);
    [Fact]
    [Trait("Type", "Unit")]
    public void IsZero_Negative_IsFalse() => Assert.False(BF_MinusZeroPointNine.IsZero);

    [Fact]
    [Trait("Type", "Unit")]
    public void Sign_0_Is0() => Assert.Equal(0, ((BigFraction)0).Sign);
    [Fact]
    [Trait("Type", "Unit")]
    public void Sign_Positive_Is1() => Assert.Equal(1, BF_ZeroPointNine.Sign);
    [Fact]
    [Trait("Type", "Unit")]
    public void Sign_Negative_IsMinus1() => Assert.Equal(-1, BF_MinusZeroPointNine.Sign);

    [Fact]
    [Trait("Type", "Unit")]
    public void Cast_Int32_ReturnsSameValue()
    {
        var f = RandomHelper.GetRandomInt32();
        Assert.True(((BigFraction)f).Equals(f));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Cast_Int64_ReturnsSameValue()
    {
        var f = RandomHelper.GetRandomInt64();
        Assert.True(((BigFraction)f).Equals(f));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Cast_Decimal_ReturnsSameValue()
    {
        var f = decimal.Round((decimal)(new Random().NextDouble() * 30 + 15), 6);
        Assert.True(((BigFraction)f).Equals(f));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Cast_Double_ReturnsSameValue()
    {
        var f = (double)RandomHelper.GetRandomInt32();
        Assert.Equal(f.ToString(), ((BigFraction)f).ToString());
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Cast_BigInteger_ReturnsSameValue()
    {
        var f = (BigInteger)RandomHelper.GetRandomInt64();
        Assert.True(((BigFraction)f).Equals(f));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameBigFractionObject_ReturnsTrue() => Assert.True(BF_Two.Equals((object)BF_Two));
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentBigFractionObject_ReturnsFalse() => Assert.False(BF_Two.Equals((object)(BF_Three)));
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameBigIntegerObject_ReturnsTrue() => Assert.True(BF_Two.Equals((object)BI_Two));
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentBigIntegerObject_ReturnsFalse() => Assert.False(BF_Two.Equals((object)BI_One));
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameInt64Object_ReturnsTrue() => Assert.True(BF_Two.Equals((object)I64_Two));
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentInt64Object_ReturnsFalse() => Assert.False(BF_Two.Equals((object)I64_One));
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_SameInt32Object_ReturnsTrue() => Assert.True(BF_Two.Equals((object)I32_Two));
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_DifferentInt32Object_ReturnsFalse() => Assert.False(BF_Two.Equals((object)I32_Three));
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_Object_ReturnsFalse() => Assert.False(BF_Two.Equals(new object()));


    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_SameBigFraction_ReturnsTrue() => Assert.True(BF_MinusTwo == BF_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_DifferentBigFraction_ReturnsFalse() => Assert.False(BF_MinusTwo == BF_MinusThree);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_SameBigFraction_ReturnsFalse() => Assert.False(BF_MinusTwo != BF_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_DifferentBigFraction_ReturnsTrue() => Assert.True(BF_MinusTwo != BF_MinusThree);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_SameBigInteger_ReturnsTrue() => Assert.True(BF_MinusTwo == BI_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_DifferentBigInteger_ReturnsFalse() => Assert.False(BF_MinusTwo == BI_MinusThree);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_SameBigInteger_ReturnsFalse() => Assert.False(BF_MinusTwo != BI_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_DifferentBigInteger_ReturnsTrue() => Assert.True(BF_MinusTwo != BI_MinusThree);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_FlippedSameBigInteger_ReturnsTrue() => Assert.True(BI_MinusTwo == BF_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_FlippedDifferentBigInteger_ReturnsFalse() => Assert.False(BI_MinusTwo == BF_MinusThree);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_FlippedSameBigInteger_ReturnsFalse() => Assert.False(BI_MinusTwo != BF_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_FlippedDifferentBigInteger_ReturnsTrue() => Assert.True(BI_MinusTwo != BF_MinusThree);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_SameInt64_ReturnsTrue() => Assert.True(BF_MinusTwo == I64_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_DifferentInt64_ReturnsFalse() => Assert.False(BF_MinusTwo == I64_MinusThree);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_SameInt64_ReturnsFalse() => Assert.False(BF_MinusTwo != I64_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_DifferentInt64_ReturnsTrue() => Assert.True(BF_MinusTwo != I64_MinusThree);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_FlippedSameInt64_ReturnsTrue() => Assert.True(I64_MinusTwo == BF_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpEqualsEquals_FlippedDifferentInt64_ReturnsFalse() => Assert.False(I64_MinusTwo == BF_MinusThree);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_FlippedSameInt64_ReturnsFalse() => Assert.False(I64_MinusTwo != BF_MinusTwo);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpNotEquals_FlippedDifferentInt64_ReturnsTrue() => Assert.True(I64_MinusTwo != BF_MinusThree);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_SameBigFraction_ReturnsFalse() => Assert.False(BF_Two > BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_BiggerBigFraction_ReturnsFalse() => Assert.False(BF_Two > BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_SmallerBigFraction_ReturnsTrue() => Assert.True(BF_Two > BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_SameBigFraction_ReturnsTrue() => Assert.True(BF_Two >= BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_BiggerBigFraction_ReturnsFalse() => Assert.False(BF_Two >= BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_SmallerBigFraction_ReturnsTrue() => Assert.True(BF_Two >= BF_One);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_SameBigFraction_ReturnsFalse() => Assert.False(BF_Two < BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_BiggerBigFraction_ReturnsTrue() => Assert.True(BF_Two < BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_SmallerBigFraction_ReturnsFalse() => Assert.False(BF_Two < BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_SameBigFraction_ReturnsTrue() => Assert.True(BF_Two <= BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_BiggerBigFraction_ReturnsTrue() => Assert.True(BF_Two <= BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_SmallerBigFraction_ReturnsFalse() => Assert.False(BF_Two <= BF_One);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_SameBigInteger_ReturnsFalse() => Assert.False(BF_Two > BI_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_BiggerBigInteger_ReturnsFalse() => Assert.False(BF_Two > BI_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_SmallerBigInteger_ReturnsTrue() => Assert.True(BF_Two > BI_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_SameBigInteger_ReturnsTrue() => Assert.True(BF_Two >= BI_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_BiggerBigInteger_ReturnsFalse() => Assert.False(BF_Two >= BI_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_SmallerBigInteger_ReturnsTrue() => Assert.True(BF_Two >= BI_One);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_SameBigInteger_ReturnsFalse() => Assert.False(BF_Two < BI_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_BiggerBigInteger_ReturnsTrue() => Assert.True(BF_Two < BI_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_SmallerBigInteger_ReturnsFalse() => Assert.False(BF_Two < BI_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_SameBigInteger_ReturnsTrue() => Assert.True(BF_Two <= BI_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_BiggerBigInteger_ReturnsTrue() => Assert.True(BF_Two <= BI_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_SmallerBigInteger_ReturnsFalse() => Assert.False(BF_Two <= BI_One);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_FlippedSameBigInteger_ReturnsFalse() => Assert.False(BI_Two > BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_FlippedBiggerBigInteger_ReturnsFalse() => Assert.False(BI_Two > BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_FlippedSmallerBigInteger_ReturnsTrue() => Assert.True(BI_Two > BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_FlippedSameBigInteger_ReturnsTrue() => Assert.True(BI_Two >= BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_FlippedBiggerBigInteger_ReturnsFalse() => Assert.False(BI_Two >= BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_FlippedSmallerBigInteger_ReturnsTrue() => Assert.True(BI_Two >= BF_One);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_FlippedSameBigInteger_ReturnsFalse() => Assert.False(BI_Two < BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_FlippedBiggerBigInteger_ReturnsTrue() => Assert.True(BI_Two < BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_FlippedSmallerBigInteger_ReturnsFalse() => Assert.False(BI_Two < BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_FlippedSameBigInteger_ReturnsTrue() => Assert.True(BI_Two <= BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_FlippedBiggerBigInteger_ReturnsTrue() => Assert.True(BI_Two <= BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_FlippedSmallerBigInteger_ReturnsFalse() => Assert.False(BI_Two <= BF_One);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_FlippedSameInt64_ReturnsFalse() => Assert.False(I64_Two > BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_FlippedBiggerInt64_ReturnsFalse() => Assert.False(I64_Two > BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreater_FlippedSmallerInt64_ReturnsTrue() => Assert.True(I64_Two > BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_FlippedSameInt64_ReturnsTrue() => Assert.True(I64_Two >= BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_FlippedBiggerInt64_ReturnsFalse() => Assert.False(I64_Two >= BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpGreaterEqual_FlippedSmallerInt64_ReturnsTrue() => Assert.True(I64_Two >= BF_One);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_FlippedSameInt64_ReturnsFalse() => Assert.False(I64_Two < BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_FlippedBiggerInt64_ReturnsTrue() => Assert.True(I64_Two < BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLess_FlippedSmallerInt64_ReturnsFalse() => Assert.False(I64_Two < BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_FlippedSameInt64_ReturnsTrue() => Assert.True(I64_Two <= BF_Two);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_FlippedBiggerInt64_ReturnsTrue() => Assert.True(I64_Two <= BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpLessEqual_FlippedSmallerInt64_ReturnsFalse() => Assert.False(I64_Two <= BF_One);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpAdd_BigFraction_ReturnsCorrectResult() => Assert.True((BF_One + BF_Two) == BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpAdd_PlusZero_ReturnsSame() => Assert.True((BF_One + BF_Zero) == BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpAdd_ZeroPlus_ReturnsSame() => Assert.True((BF_Zero + BF_One) == BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpAdd_BigInteger_ReturnsCorrectResult() => Assert.True((BF_One + BI_Two) == BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpAdd_FlippedBigInteger_ReturnsCorrectResult() => Assert.True((BI_One + BF_Two) == BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpAdd_Int64_ReturnsCorrectResult() => Assert.True((BF_One + I64_Two) == BF_Three);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpAdd_FlippedInt64_ReturnsCorrectResult() => Assert.True((I64_One + BF_Two) == BF_Three);


    [Fact]
    [Trait("Type", "Unit")]
    public void OpSubtract_BigFraction_ReturnsCorrectResult() => Assert.True((BF_One - BF_Two) == BF_MinusOne);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpSubtract_MinusZero_ReturnsSame() => Assert.True((BF_One - BF_Zero) == BF_One);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpSubtract_BigInteger_ReturnsCorrectResult() => Assert.True((BF_One - BI_Two) == BF_MinusOne);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpSubtract_FlippedBigInteger_ReturnsCorrectResult() => Assert.True((BI_One - BF_Two) == BF_MinusOne);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpSubtract_Int64_ReturnsCorrectResult() => Assert.True((BF_One - I64_Two) == BF_MinusOne);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpSubtract_FlippedInt64_ReturnsCorrectResult() => Assert.True((I64_One - BF_Two) == BF_MinusOne);


    [Fact]
    [Trait("Type", "Unit")]
    public void OpUnaryMinus_BigFraction_ReturnsCorrectResult() => Assert.True(-BF_ZeroPointNine == BF_MinusZeroPointNine);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpUnaryPlus_BigFraction_ReturnsCorrectResult() => Assert.True(+BF_MinusZeroPointNine == BF_MinusZeroPointNine);

    [Fact]
    [Trait("Type", "Unit")]
    public void OpMultiply_BigFraction_ReturnsCorrectResult() => Assert.True((BF_MinusOnePointOne * BF_MinusOne) == BF_OnePointOne);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpMultiply_Zero_ReturnsZero() => Assert.True((BF_MinusOnePointOne * BF_Zero).IsZero);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpMultiply_BigInteger_ReturnsCorrectResult() => Assert.True((BF_MinusOnePointOne * BI_MinusOne) == BF_OnePointOne);
    [Fact]
    [Trait("Type", "Unit")]
    public void OpMultiply_FlippedBigInteger_ReturnsCorrectResult() => Assert.True((BI_MinusOne * BF_MinusOnePointOne) == BF_OnePointOne);

    [Fact]
    [Trait("Type", "Unit")]
    public void Abs_Positive_ReturnsSame() => Assert.True(BigFraction.Abs(BF_ZeroPointNine) == BF_ZeroPointNine);
    [Fact]
    [Trait("Type", "Unit")]
    public void Abs_Negative_ReturnsPositive() => Assert.True(BigFraction.Abs(BF_MinusZeroPointNine) == BF_ZeroPointNine);
    [Fact]
    [Trait("Type", "Unit")]
    public void Abs_Zero_ReturnsZero() => Assert.True(BigFraction.Abs(BF_Zero).IsZero);

    [Fact]
    [Trait("Type", "Unit")]
    public void DividePow10_BigIntegerNegativePower_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => BigFraction.DividePow10(BI_MinusThree, -1));
    [Fact]
    [Trait("Type", "Unit")]
    public void DividePow10_BigIntegerPower0_ReturnsSame() => Assert.True(BigFraction.DividePow10(BI_MinusThree, 0) == BF_MinusThree);
    [Fact]
    [Trait("Type", "Unit")]
    public void DividePow10_ZeroPower1_ReturnsZero() => Assert.True(BigFraction.DividePow10(BI_Zero, 0).IsZero);
    [Fact]
    [Trait("Type", "Unit")]
    public void DividePow10_BigIntegerPower1_ReturnsCorrectResult() => Assert.True(BigFraction.DividePow10(BI_MinusThree, 1) == BigFraction.Parse("-0.3"));

    [Fact]
    [Trait("Type", "Unit")]
    public void DividePow10_Negative_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => BF_One.DividePow10(-1));
    [Fact]
    [Trait("Type", "Unit")]
    public void DividePow10_Power0_ReturnsSame() => Assert.True(BF_MinusZeroPointNine.DividePow10(0) == BF_MinusZeroPointNine);
    [Fact]
    [Trait("Type", "Unit")]
    public void DividePow10_Power1_ReturnsCorrectResult() => Assert.True(BF_MinusZeroPointNine.DividePow10(1) == BigFraction.Parse("-0.09"));

    [Fact]
    [Trait("Type", "Unit")]
    public void ToApproximateDecimal_ReturnsCorrectResult() => Assert.Equal(BF_MinusZeroPointNine.ToApproximateDecimal(), decimal.Parse(BF_MinusZeroPointNine.ToString()));
    [Fact]
    [Trait("Type", "Unit")]
    public void ToApproximateDouble_ReturnsCorrectResult() => Assert.Equal(BF_MinusZeroPointNine.ToApproximateDouble(), double.Parse(BF_MinusZeroPointNine.ToString()));

#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
#pragma warning disable IDE0060 // Remove unused parameter
    [Theory]
    [MemberData(nameof(ValidStringNumeratorDenominators))]
    [Trait("Type", "Unit")]
    public void CtorParse_TestCases_SetCorrectNumerator(string str, long numerator, long denominator) => Assert.Equal(numerator, BigFraction.Parse(str).Numerator);
    [Theory]
    [MemberData(nameof(ValidStringNumeratorDenominators))]
    [Trait("Type", "Unit")]
    public void CtorParse_TestCases_SetCorrectDenominator(string str, long numerator, long denominator) => Assert.Equal(denominator, BigFraction.Parse(str).Denominator);
    [Theory]
    [MemberData(nameof(ValidStringNumeratorDenominators))]
    [Trait("Type", "Unit")]
    public void CtorParse_TestCases_SetCorrectWholeNumber(string str, long numerator, long denominator) => Assert.Equal((str.StartsWith('-') ? -1 : 1) * BigInteger.Parse("0" + str.TrimStart('-').Split('.')[0]), BigFraction.Parse(str).WholeNumber);
    [Theory]
    [MemberData(nameof(ValidStringNumeratorDenominators))]
    [Trait("Type", "Unit")]
    public void CtorParse_TestCases_CorrectSign(string str, long numerator, long denominator) => Assert.Equal(Math.Sign(numerator), BigFraction.Parse(str).Sign);
    [Theory]
    [MemberData(nameof(ValidStringNumeratorDenominators))]
    [Trait("Type", "Unit")]
    public void CtorParse_TestCases_CorrectIsZero(string str, long numerator, long denominator) => Assert.Equal(numerator == 0, BigFraction.Parse(str).IsZero);
    [Theory]
    [MemberData(nameof(ValidStringNumeratorDenominators))]
    [Trait("Type", "Unit")]
    public void CtorParse_TestCases_SetCorrectDecimalString(string str, long numerator, long denominator) => Assert.Equal(str.Split('.').Skip(1).FirstOrDefault()?.TrimEnd('0') ?? "", BigFraction.Parse(str).DecimalString);
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
#pragma warning restore IDE0060 // Remove unused parameter

    [Fact]
    [Trait("Type", "Unit")]
    public void CtorParse_WholeNumbers_HaveDenominatorOne()
    {
        for (var x = 0; x < 1000; ++x)
        {
            Assert.Equal(1, BigFraction.Parse(RandomHelper.GetRandomInt64().ToString()).Denominator);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void CtorParse_WholeNumbers_HaveSameNumerator()
    {
        for (var x = 0; x < 1000; ++x)
        {
            var num = RandomHelper.GetRandomInt64();
            Assert.Equal(num, BigFraction.Parse(num.ToString()).Numerator);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void CtorParse_WholeNumbers_HaveSameWholeNumber()
    {
        for (var x = 0; x < 1000; ++x)
        {
            var num = RandomHelper.GetRandomInt64();
            Assert.Equal(num, BigFraction.Parse(num.ToString()).WholeNumber);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void CtorParse_WholeNumbers_HaveEmptyDecimalString()
    {
        for (var x = 0; x < 1000; ++x)
        {
            var num = RandomHelper.GetRandomInt64();
            Assert.Equal("", BigFraction.Parse(num.ToString()).DecimalString);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void CtorParse_Decimals_AreCompact()
    {
        var rnd = new Random();
        for (var x = 0; x < 1000; ++x)
        {
            var s = ((BigFraction)(rnd.NextDouble() * 400 - 200)).ToString();
            var beforeDecimal = s.Split('.')[0].Trim('-');
            Assert.False(beforeDecimal.Length > 1 && beforeDecimal.StartsWith('0'));
            Assert.True(beforeDecimal.Length >= 1);
            Assert.True(!s.Contains('.') || !s.EndsWith('0'));
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Comparisons_RandomWhole_Match()
    {
        for (var x = 0; x < 1000; ++x)
        {
            var num = RandomHelper.GetRandomInt64();
            var num2 = RandomHelper.GetRandomInt64();
            var bf = (BigFraction)num;
            var bf2 = (BigFraction)num2;
            Assert.Equal(bf.Equals(bf2), bf == bf2);
            Assert.Equal(num == num2, bf == bf2);
            Assert.Equal(num != num2, bf != bf2);
            Assert.Equal(num > num2, bf > bf2);
            Assert.Equal(num >= num2, bf >= bf2);
            Assert.Equal(num < num2, bf < bf2);
            Assert.Equal(num <= num2, bf <= bf2);
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Comparisons_RandomDecimals_Match()
    {
        var rnd = new Random();
        for (var x = 0; x < 1000; ++x)
        {
            var num = rnd.NextDouble() * 400 - 200;
            var num2 = rnd.NextDouble() * 400 - 200;
            var bf = (BigFraction)num;
            var bf2 = (BigFraction)num2;
            Assert.Equal(bf.Equals(bf2), bf == bf2);
            Assert.Equal(num == num2, bf == bf2);
            Assert.Equal(num != num2, bf != bf2);
            Assert.Equal(num > num2, bf > bf2);
            Assert.Equal(num >= num2, bf >= bf2);
            Assert.Equal(num < num2, bf < bf2);
            Assert.Equal(num <= num2, bf <= bf2);
        }
    }

    [Theory]
    [InlineData("0", 0, "0")]
    [InlineData("1", 0, "1")]
    [InlineData("-1", 0, "-1")]
    [InlineData("-1", 2, "-1")]
    [InlineData("1.5", 0, "1")]
    [InlineData("1.5", 1, "1.5")]
    [InlineData("1.5", 2, "1.5")]
    [InlineData("1.26", 0, "1")]
    [InlineData("1.26", 1, "1.2")]
    [InlineData("1.26", 2, "1.26")]
    [InlineData("-1.26", 1, "-1.2")]
    [InlineData("-1.26", int.MaxValue, "-1.26")]
    [InlineData("0.999", 0, "0")]
    [InlineData("0.099", 1, "0")]
    [InlineData("1.06", 1, "1")]
    [InlineData("1.06", 2, "1.06")]
    [Trait("Type", "Unit")]
    public void Truncate_TestCases_ReturnsCorrectResult(string input, int decimalCount, string expected) => Assert.Equal(expected, BigFraction.Parse(input).Truncate(decimalCount).ToString());

    [Fact]
    [Trait("Type", "Unit")]
    public void Truncate_NegativeDecimals_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => BigFraction.Zero.Truncate(-1));

    [Fact]
    [Trait("Type", "Unit")]
    public void MaxPowerOfTen_Is10000() => Assert.Equal(10000, BigFraction.MaxPowerOfTen);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetPowerOfTen_Negative_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => BigFraction.GetPowerOfTen(-1));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetPowerOfTen_TooBig_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => BigFraction.GetPowerOfTen(BigFraction.MaxPowerOfTen + 1));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetPowerOfTen_Max_ReturnsValueResult() => Assert.Equal("1" + new string('0', BigFraction.MaxPowerOfTen), BigFraction.GetPowerOfTen(BigFraction.MaxPowerOfTen).ToString());

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 10)]
    [InlineData(2, 100)]
    [InlineData(3, 1000)]
    [InlineData(4, 10000)]
    [InlineData(5, 100000)]
    [InlineData(6, 1000000)]
    [Trait("Type", "Unit")]
    public void GetPowerOfTen_TestCases(int power, long expected) => Assert.Equal(expected, BigFraction.GetPowerOfTen(power));

    [Theory]
    [InlineData("123.45", 0, false)]
    [InlineData("123.45", 1, false)]
    [InlineData("123.45", 2, false)]
    [InlineData("123.45", 3, false)]
    [InlineData("123.45", 4, false)]
    [InlineData("123.45", 5, false)]
    [InlineData("123.45", 6, true)]
    [InlineData("123.45", 7, true)]
    [InlineData("-123.45", 0, false)]
    [InlineData("-123.45", 1, false)]
    [InlineData("-123.45", 2, false)]
    [InlineData("-123.45", 3, false)]
    [InlineData("-123.45", 4, false)]
    [InlineData("-123.45", 5, false)]
    [InlineData("-123.45", 6, false)]
    [InlineData("-123.45", 7, true)]
    [InlineData("-123.45", 8, true)]
    [InlineData("0", 0, false)]
    [InlineData("0", 1, true)]
    [InlineData("-1", 0, false)]
    [InlineData("-1", 1, false)]
    [InlineData("-1", 2, true)]
    [InlineData("-1", 3, true)]
    [InlineData("-0.3", 3, false)]
    [InlineData("-0.3", 4, true)]
    [InlineData("-0.3", 5, true)]
    [InlineData("-1.3", 3, false)]
    [InlineData("-1.3", 4, true)]
    [InlineData("-1.3", 5, true)]
    [InlineData("0.3", 2, false)]
    [InlineData("0.3", 3, true)]
    [InlineData("0.3", 4, true)]
    [InlineData("1.3", 2, false)]
    [InlineData("1.3", 3, true)]
    [InlineData("1.3", 4, true)]
    public void TryToCharSpan_TestCases(string decString, int charLen, bool shouldSucceed)
    {
        BigFraction f = BigFraction.Parse(decString);
        Span<char> chars = stackalloc char[charLen];
        Assert.Equal(shouldSucceed, f.TryToCharSpan(chars, out var written));
        if (shouldSucceed)
        {
            Assert.Equal(decString.Length, written);
            Assert.Equal(decString, chars[..written].ToString());
        }
    }
}
