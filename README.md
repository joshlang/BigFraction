# BigFraction

This is an arbitrary precision C# decimal type.

Addition, subtraction, and multiplication are supported.  Division is not.

# Usage

Add the `BigFraction` nuget package.

Import the `Epoche` namespace.

Examples:
```
Console.WriteLine("Hello, World!");

BigFraction a = BigFraction.Zero;
BigFraction b = 2; // 2

//BigFraction c = 123.4; // error - float/double are lossy (imprecise) types
BigFraction c = 123.4m; // decimal is okay

BigFraction d = BigFraction.Parse("-123.4");

var parsedE = BigFraction.TryParse("-.1", out var e); // e = -0.1, returned true
var parsedF = BigFraction.TryParse("aaa", out var f); // f = 0, returned false
var g = BigFraction.TryParseDefault("xyz"); // g = 0
var h = BigFraction.TryParseDefault("xyz", 2); // h = 2

Console.WriteLine($"a = {a}"); // 0
Console.WriteLine($"b = {b}"); // 2
Console.WriteLine($"c = {c}"); // 123.4
Console.WriteLine($"d = {d}"); // -123.4
Console.WriteLine($"e = {e} ({parsedE})"); // -0.1 (True)
Console.WriteLine($"f = {f} ({parsedF})"); // 0 (False)
Console.WriteLine($"g = {g}"); // 0
Console.WriteLine($"h = {h}"); // 2

Console.WriteLine("--------------");

foreach (var x in new[] { a, b, c, d, e, f, g, h })
{
    Console.WriteLine($"{x,10}: Numerator={x.Numerator,-6} Denominator={x.Denominator,-6} WholeNumber={x.WholeNumber,-6} DecimalString={x.DecimalString,-6}");
    /*
         0: Numerator=0      Denominator=1      WholeNumber=0      DecimalString=
         2: Numerator=2      Denominator=1      WholeNumber=2      DecimalString=
     123.4: Numerator=1234   Denominator=10     WholeNumber=123    DecimalString=4
    -123.4: Numerator=-1234  Denominator=10     WholeNumber=-123   DecimalString=4
      -0.1: Numerator=-1     Denominator=10     WholeNumber=0      DecimalString=1
         0: Numerator=0      Denominator=1      WholeNumber=0      DecimalString=
         0: Numerator=0      Denominator=1      WholeNumber=0      DecimalString=
         2: Numerator=2      Denominator=1      WholeNumber=2      DecimalString=
     */
}

Console.WriteLine($"b + d * 123.4m - 50 = {b + d * 123.4m - 50}"); // b + d * 123.4m - 50 = -15275.56
Console.WriteLine($"b == 1 ... {b == 1}"); // b == 1... False
Console.WriteLine($"d == -123.4m ... {d == -123.4m}"); // d == -123.4m ... True
Console.WriteLine($"c == -d ... {c == -d}"); // c == -d ... True
Console.WriteLine($"b > 1 ... {b > 1}"); // b > 1 ... True

Console.WriteLine($"b.CompareTo(d) = {b.CompareTo(d)}"); // b.CompareTo(d) = 1
Console.WriteLine($"b.Equals(d) = {b.Equals(d)}"); // b.Equals(d) = False
```

# JSON

Here's a JSON converter you can use if you need one (I didn't want to add a dependency on `System.Text.Json`)

```
public sealed class BigFractionConverter : JsonConverter<BigFraction>
{
    public static readonly BigFractionConverter Instance = new();

    public override BigFraction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to BigFraction with this converter");
        }
        if (BigFraction.TryParse(reader.GetString(), out var value))
        {
            return value;
        }
        throw new FormatException("The value could not be parsed into a BigFraction");
    }

    public override void Write(Utf8JsonWriter writer, BigFraction value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
```

# Details

All numbers are stored using the following properties:

`BigInteger Numerator` - When divided by the Denominator, yields the exact number being stored.

`BigInteger Denominator` - Always a power of ten (1, 10, 100, 1000, etc).

`BigInteger WholeNumber` - The number before the decimal (Example:  123.45 - WholeNumber = 123)

`string DecimalString` - The number after the decimal (Example:  123.45 - DecimalString = "45") (Example:  123 - DecimalString = "")

Storing `WholeNumber` and `DecimalString` allow for faster comparisons and operations, as well as faster string serialization.

