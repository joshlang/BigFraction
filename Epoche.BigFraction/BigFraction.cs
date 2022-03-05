using System.Diagnostics;
using System.Numerics;

namespace Epoche;

public readonly struct BigFraction : IEquatable<BigFraction>, IEquatable<BigInteger>, IComparable<BigFraction>, IComparable<BigInteger>, IEquatable<long>, IComparable<long>, IEquatable<int>, IComparable<int>
{
    public const int MaxPowerOfTen = 10000;

    static readonly BigInteger Ten = 10;
    static List<BigInteger> PowersOfTen = new() { BigInteger.One };
    public static readonly BigFraction Zero;
    static readonly BigFraction[] OneToNine = new BigFraction[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    static readonly BigFraction[] NegativeOneToNine = new BigFraction[] { -1, -2, -3, -4, -5, -6, -7, -8, -9 };

    public static BigInteger GetPowerOfTen(int power)
    {
        if (power < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(power));
        }
        if (power > MaxPowerOfTen)
        {
            throw new ArgumentOutOfRangeException(nameof(power), "There is no WTFException, so throwing ArgumentOutOfRangeException");
        }
        while (true)
        {
            var powers = PowersOfTen;
            if (powers.Count <= power)
            {
                var newpowers = powers.ToList();
                while (newpowers.Count <= power)
                {
                    newpowers.Add(newpowers.Last() * Ten);
                }
                if (Interlocked.CompareExchange(ref PowersOfTen, newpowers, powers) != powers)
                {
                    continue;
                }
                powers = newpowers;
            }
            return powers[power];
        }
    }

    readonly string? decimalString; // This can be null only for "Zero" (when a constructor isn't used).
    public readonly BigInteger WholeNumber, Numerator;
    public string DecimalString => decimalString ?? "";
    public BigInteger Denominator => PowersOfTen[decimalString?.Length ?? 0]; // requires us to guarantee size, so make sure it's initialized in constructor

    BigFraction(string decimalString, in BigInteger wholeNumber, in BigInteger numerator)
    {
        this.decimalString = decimalString ?? throw new ArgumentNullException(nameof(decimalString));
        WholeNumber = wholeNumber;
        Numerator = numerator;

        if (PowersOfTen.Count <= decimalString.Length)
        {
            GetPowerOfTen(decimalString.Length);
        }

        if (decimalString.Length > 0 && decimalString[^1] == '0')
        {
            throw new ArgumentOutOfRangeException(nameof(decimalString));
        }

        Debug.Assert(wholeNumber == numerator / Denominator);
    }

    BigFraction(BigInteger numerator, int denominatorPower)
    {
        if (numerator.IsZero)
        {
            decimalString = "";
            WholeNumber = Numerator = BigInteger.Zero;
        }
        else
        {
            var negative = numerator.Sign < 0;
            if (negative)
            {
                numerator = -numerator;
            }

            var numString = numerator.ToString("R");
            var wholeLength = numString.Length - denominatorPower;
            if (wholeLength < 0)
            {
                numString = numString.PadLeft(numString.Length - wholeLength, '0');
                wholeLength = 0;
            }

            var whole = wholeLength == 0 ? BigInteger.Zero : BigInteger.Parse(numString.AsSpan(0, wholeLength));
            var decString = numString.AsSpan(wholeLength);
            if (decString.Length > 0 && decString[0] == '-')
            {
                decString = decString[1..];
            }
            var trailingZeroes = 0;
            for (var x = decString.Length - 1; x >= 0; --x)
            {
                if (decString[x] == '0')
                {
                    ++trailingZeroes;
                    --denominatorPower;
                }
                else
                {
                    break;
                }
            }
            if (trailingZeroes > 0)
            {
                numerator /= GetPowerOfTen(trailingZeroes);
                decString = decString[..^trailingZeroes];
            }

            Numerator = negative ? -numerator : numerator;
            decimalString = decString.ToString();
            WholeNumber = negative ? -whole : whole;
            if (PowersOfTen.Count <= decimalString.Length)
            {
                GetPowerOfTen(decimalString.Length);
            }
        }
    }

    public static BigFraction Parse(string value) => TryParse(value ?? throw new ArgumentNullException(nameof(value)), out var bf) ? bf : throw new FormatException();

    public static bool TryParse(string? value, out BigFraction bigFraction) => TryParse(value.AsSpan(), out bigFraction);

    public static bool TryParse(ReadOnlySpan<char> value, out BigFraction bigFraction)
    {
        bigFraction = Zero;
        if (value.Length == 0)
        {
            return false;
        }
        var negative = value[0] == '-';
        if (negative)
        {
            value = value[1..]; // Trim leading '-'
        }
        while (value[0] == '0')
        {
            if (value.Length == 1)
            {
                return true; // The value is zero or all zeroes
            }
            value = value[1..]; // Trim leading '0'
        }
        char c;
        if (value.Length == 1)
        {
            c = value[0];
            if (c > '0' && c <= '9')
            {
                bigFraction = negative ? NegativeOneToNine[c - '1'] : OneToNine[c - '1'];
                return true;
            }
            return false;
        }
        var decimalIndex = -1;
        for (var i = 0; i < value.Length; ++i)
        {
            c = value[i];
            if (c < '0' || c > '9')
            {
                if (c == '.')
                {
                    if (decimalIndex != -1)
                    {
                        return false; // Cannot have more than 1 decimal or be outside 0-9
                    }
                    if (i == value.Length - 1)
                    {
                        return false; // Cannot end with a decimal
                    }
                    decimalIndex = i;
                }
                else
                {
                    return false;
                }
            }
        }
        BigInteger numerator;
        if (decimalIndex == -1)
        {
            if (BigInteger.TryParse(value, out numerator))
            {
                bigFraction = negative
                    ? new BigFraction("", -numerator, -numerator)
                    : new BigFraction("", numerator, numerator);
                return true;
            }
            return false;
        }

        // Remove all trailing zeroes
        while (value[^1] == '0')
        {
            value = value[..^1];
        }

        var wholeString = value[..decimalIndex];
        var decimalString = value[(decimalIndex + 1)..];
        var combinedLength = wholeString.Length + decimalString.Length;
        if (combinedLength == 0)
        {
            return true;
        }

        Span<char> combined = stackalloc char[combinedLength];
        wholeString.CopyTo(combined);
        decimalString.CopyTo(combined[wholeString.Length..]);
        if (BigInteger.TryParse(combined, out numerator))
        {
            bigFraction = negative
                ? new BigFraction(decimalString.ToString(), wholeString.Length > 0 ? -BigInteger.Parse(wholeString) : BigInteger.Zero, -numerator)
                : new BigFraction(decimalString.ToString(), wholeString.Length > 0 ? BigInteger.Parse(wholeString) : BigInteger.Zero, numerator);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to parse a BigFraction.  Returns the parsed value, or Zero on failure.
    /// </summary>
    public static BigFraction TryParseDefault(string? value) => value?.Length > 0 && TryParse(value, out var bf) ? bf : Zero;

    /// <summary>
    /// Tries to parse a BigFraction.  Returns the parsed value, or defaultOnFailure on failure.
    /// </summary>
    public static BigFraction TryParseDefault(string? value, in BigFraction defaultOnFailure) => value?.Length > 0 && TryParse(value, out var bf) ? bf : defaultOnFailure;

    /// <summary>
    /// Formats a round-trippable string into a Span[char].  Culture settings have no effect.  The result contains only these characters: -.0123456789
    /// </summary>
    public bool TryToCharSpan(Span<char> dest, out int written)
    {
        written = 0;
        if (dest.Length == 0)
        {
            return false;
        }
        if (IsZero)
        {
            dest[0] = '0';
            written = 1;
            return true;
        }
        if (DecimalString.Length == 0)
        {
            return WholeNumber.TryFormat(dest, out written, "R");
        }
        if (WholeNumber.IsZero)
        {
            if (Numerator.Sign < 0)
            {
                if (dest.Length < 2)
                {
                    return false;
                }
                dest[0] = '-';
                dest[1] = '0';
                written = 2;
            }
            else
            {
                dest[0] = '0';
                written = 1;
            }
        }
        else
        {
            if (!WholeNumber.TryFormat(dest[written..], out int wholeWritten, "R"))
            {
                return false;
            }
            written += wholeWritten;
        }
        if (dest.Length < written + 1 + decimalString!.Length)
        {
            return false;
        }
        dest[written++] = '.';
        decimalString.AsSpan().CopyTo(dest[written..]);
        written += decimalString.Length;
        return true;
    }

    /// <summary>
    /// Returns a round-trippable string.  Culture settings have no effect.  The result contains only these characters: -.0123456789
    /// </summary>
    public override string ToString()
    {
        if (IsZero)
        {
            return "0";
        }
        if (DecimalString.Length == 0)
        {
            return WholeNumber.ToString("R");
        }
        var wholeIsZero = WholeNumber.IsZero;
        var whole = wholeIsZero ? Numerator.Sign < 0 ? "-0" : "0" : WholeNumber.ToString("R");
        var length = whole.Length + 1 + decimalString!.Length;
        var str = string.Create(length, (whole, decimalString), (chars, state) =>
        {
            state.whole.AsSpan().CopyTo(chars);
            chars[state.whole.Length] = '.';
            state.decimalString.AsSpan().CopyTo(chars[(state.whole.Length + 1)..]);
        });

#if DEBUG
        Debug.Assert(!str.StartsWith("0") || str.Contains('.') || IsZero);
        Debug.Assert(!str.EndsWith("0") || !str.Contains('.'));
#endif
        return str;
    }

    /// <summary>
    /// Returns a string truncated to a maximum number of decimal places.  Culture settings have no effect.  The result contains only these characters: -.0123456789
    /// </summary>
    public string ToString(int maxDecimals)
    {
        if (maxDecimals < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDecimals));
        }

        if (IsZero)
        {
            return "0";
        }
        var outputLength = decimalString!.Length > maxDecimals ? maxDecimals : decimalString.Length;
        while (outputLength > 0 && decimalString[outputLength - 1] == '0')
        {
            --outputLength;
        }
        if (outputLength == 0)
        {
            return WholeNumber.ToString("R");
        }
        var wholeIsZero = WholeNumber.IsZero;
        var whole = wholeIsZero ? Numerator.Sign < 0 ? "-0" : "0" : WholeNumber.ToString("R");
        var length = whole.Length + 1 + outputLength;
        var str = string.Create(length, (whole, decimalString), (chars, state) =>
        {
            state.whole.AsSpan().CopyTo(chars);
            chars[state.whole.Length] = '.';
            state.decimalString.AsSpan(0, outputLength).CopyTo(chars[(state.whole.Length + 1)..]);
        });

#if DEBUG
        Debug.Assert(!str.StartsWith("0") || str.Contains('.') || IsZero);
        Debug.Assert(!str.EndsWith("0") || !str.Contains('.'));
#endif
        return str;
    }

    public bool Equals(BigFraction other) => Equals(in other);
    public bool Equals(in BigFraction other) => Numerator == other.Numerator && DecimalString == other.DecimalString;
    public bool Equals(BigInteger other) => DecimalString.Length == 0 && WholeNumber == other;
    public bool Equals(in BigInteger other) => DecimalString.Length == 0 && WholeNumber == other;
    public bool Equals(long other) => DecimalString.Length == 0 && WholeNumber == other;
    public bool Equals(int other) => Equals((long)other);
    public override int GetHashCode() => WholeNumber.GetHashCode() + DecimalString.GetHashCode();

    public int CompareTo(BigFraction other) => CompareTo(in other);

    public int CompareTo(in BigFraction other)
    {
        var cmp = WholeNumber.CompareTo(other.WholeNumber);
        if (cmp != 0)
        {
            return cmp;
        }
        if (WholeNumber.IsZero)
        {
            cmp = Numerator.Sign.CompareTo(other.Numerator.Sign);
            if (cmp != 0)
            {
                return cmp;
            }
        }

        int x;
        for (x = 0; x < decimalString?.Length && x < other.decimalString?.Length; ++x)
        {
            cmp = decimalString[x].CompareTo(other.decimalString[x]);
            if (cmp != 0)
            {
                return Numerator.Sign >= 0 ? cmp : -cmp;
            }
        }
        if (x < decimalString?.Length)
        {
            return Numerator.Sign >= 0 ? 1 : -1;
        }

        if (x < other.decimalString?.Length)
        {
            return Numerator.Sign >= 0 ? -1 : 1;
        }

        return 0;
    }

    public int CompareTo(BigInteger other) => CompareTo(in other);

    public int CompareTo(in BigInteger other)
    {
        var cmp = WholeNumber.CompareTo(other);
        if (cmp != 0)
        {
            return cmp;
        }

        if (decimalString?.Length > 0)
        {
            return Numerator.Sign >= 0 ? 1 : -1;
        }

        return 0;
    }

    public int CompareTo(long other)
    {
        var cmp = WholeNumber.CompareTo(other);
        if (cmp != 0)
        {
            return cmp;
        }

        if (decimalString?.Length > 0)
        {
            return Numerator.Sign >= 0 ? 1 : -1;
        }

        return 0;
    }

    public int CompareTo(int other) => CompareTo((long)other);
    public bool IsZero => Numerator.IsZero;
    public int Sign => Numerator.Sign;

    public static implicit operator BigFraction(int value) => new("", value, value);
    public static implicit operator BigFraction(long value) => new("", value, value);
    public static implicit operator BigFraction(decimal value) => Parse(value.ToString());
    public static explicit operator BigFraction(double value) => Parse(value.ToString("F20"));
    public static implicit operator BigFraction(in BigInteger value) => new("", value, value);

    public override bool Equals(object? obj) => obj switch
    {
        BigFraction right => Equals(right),
        BigInteger rightInt => Equals(rightInt),
        long rightLong => Equals(rightLong),
        int rightInt32 => Equals(rightInt32),
        _ => false
    };

    public static bool operator ==(in BigFraction left, in BigFraction right) => left.Equals(right);
    public static bool operator !=(in BigFraction left, in BigFraction right) => !left.Equals(right);
    public static bool operator ==(in BigFraction left, in BigInteger right) => left.Equals(right);
    public static bool operator !=(in BigFraction left, in BigInteger right) => !left.Equals(right);
    public static bool operator ==(in BigInteger left, in BigFraction right) => right.Equals(left);
    public static bool operator !=(in BigInteger left, in BigFraction right) => !right.Equals(left);

    public static bool operator ==(in BigFraction left, long right) => left.Equals(right);
    public static bool operator !=(in BigFraction left, long right) => !left.Equals(right);
    public static bool operator ==(long left, in BigFraction right) => right.Equals(left);
    public static bool operator !=(long left, in BigFraction right) => !right.Equals(left);

    public static bool operator >(in BigFraction left, in BigFraction right) => left.CompareTo(right) > 0;
    public static bool operator >=(in BigFraction left, in BigFraction right) => left.CompareTo(right) >= 0;
    public static bool operator <(in BigFraction left, in BigFraction right) => left.CompareTo(right) < 0;
    public static bool operator <=(in BigFraction left, in BigFraction right) => left.CompareTo(right) <= 0;

    public static bool operator >(in BigFraction left, in BigInteger right) => left.CompareTo(right) > 0;
    public static bool operator >=(in BigFraction left, in BigInteger right) => left.CompareTo(right) >= 0;
    public static bool operator <(in BigFraction left, in BigInteger right) => left.CompareTo(right) < 0;
    public static bool operator <=(in BigFraction left, in BigInteger right) => left.CompareTo(right) <= 0;

    public static bool operator >(in BigInteger left, in BigFraction right) => right < left;
    public static bool operator >=(in BigInteger left, in BigFraction right) => right <= left;
    public static bool operator <(in BigInteger left, in BigFraction right) => right > left;
    public static bool operator <=(in BigInteger left, in BigFraction right) => right >= left;

    public static bool operator >(in BigFraction left, long right) => left.CompareTo(right) > 0;
    public static bool operator >=(in BigFraction left, long right) => left.CompareTo(right) >= 0;
    public static bool operator <(in BigFraction left, long right) => left.CompareTo(right) < 0;
    public static bool operator <=(in BigFraction left, long right) => left.CompareTo(right) <= 0;

    public static bool operator >(long left, in BigFraction right) => right < left;
    public static bool operator >=(long left, in BigFraction right) => right <= left;
    public static bool operator <(long left, in BigFraction right) => right > left;
    public static bool operator <=(long left, in BigFraction right) => right >= left;

    public static BigFraction operator +(in BigFraction left, in BigFraction right)
    {
        if (left.IsZero)
        {
            return right;
        }
        if (right.IsZero)
        {
            return left;
        }

        BigInteger numerator;
        var largestPower = left.decimalString!.Length;
        if (largestPower == right.decimalString!.Length)
        {
            numerator = left.Numerator + right.Numerator;
        }
        else
        {
            if (largestPower < right.decimalString.Length)
            {
                numerator = right.Numerator + left.Numerator * GetPowerOfTen(right.decimalString.Length - largestPower);
                largestPower = right.decimalString.Length;
            }
            else
            {
                numerator = left.Numerator + right.Numerator * GetPowerOfTen(largestPower - right.decimalString.Length);
            }
        }
        if (numerator.IsZero)
        {
            return Zero;
        }

        return new BigFraction(numerator, largestPower);
    }

    public static BigFraction operator +(in BigFraction left, in BigInteger right) => left + (BigFraction)right;
    public static BigFraction operator +(in BigInteger left, in BigFraction right) => right + left;
    public static BigFraction operator +(in BigFraction left, long right) => left + (BigFraction)right;
    public static BigFraction operator +(long left, in BigFraction right) => right + left;

    public static BigFraction operator -(in BigFraction left, in BigFraction right) => left + (-right);
    public static BigFraction operator -(in BigFraction left, long right) => left - (BigInteger)right;
    public static BigFraction operator -(in BigFraction left, in BigInteger right) => left + (-right);
    public static BigFraction operator -(in BigInteger left, in BigFraction right) => left + (-right);

    public static BigFraction operator -(in BigFraction right) => new(right.DecimalString, -right.WholeNumber, -right.Numerator);
    public static BigFraction operator +(in BigFraction right) => right;


    public static BigFraction operator *(in BigFraction left, in BigFraction right)
    {
        if (left.IsZero || right.IsZero)
        {
            return Zero;
        }
        return new BigFraction(left.Numerator * right.Numerator, left.DecimalString.Length + right.DecimalString.Length);
    }

    public static BigFraction operator *(in BigFraction left, in BigInteger right)
    {
        if (left.IsZero || right.IsZero)
        {
            return Zero;
        }
        return new BigFraction(left.Numerator * right, left.DecimalString.Length);
    }

    public static BigFraction operator *(in BigInteger left, in BigFraction right) => right * left;
    public static BigFraction Abs(in BigFraction value) => value.Sign >= 0 ? value : -value;

    public static BigFraction DividePow10(BigInteger value, int power)
    {
        if (power < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(power));
        }
        if (power == 0)
        {
            return value;
        }
        if (value.IsZero)
        {
            return Zero;
        }
        return new BigFraction(value, power);
    }

    public BigFraction DividePow10(int power)
    {
        if (power < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(power));
        }
        if (power == 0)
        {
            return this;
        }
        if (IsZero)
        {
            return Zero;
        }
        return new BigFraction(Numerator, DecimalString.Length + power);
    }

    public decimal ToApproximateDecimal() => decimal.Parse(ToString());
    public double ToApproximateDouble() => double.Parse(ToString());

    public BigFraction Truncate(int decimalCount)
    {
        if (decimalCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalCount));
        }
        if (decimalCount >= DecimalString.Length)
        {
            return this;
        }
        while (decimalCount > 0 && DecimalString[decimalCount - 1] == '0')
        {
            --decimalCount;
        }
        if (decimalCount == 0)
        {
            return WholeNumber;
        }
        return new BigFraction(DecimalString[..decimalCount], WholeNumber, Numerator / GetPowerOfTen(DecimalString.Length - decimalCount));
    }
}
