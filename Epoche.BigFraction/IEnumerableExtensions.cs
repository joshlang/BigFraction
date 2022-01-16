namespace Epoche;

public static class IEnumerableExtensions
{
    public static BigFraction SumBigFraction(this IEnumerable<BigFraction> vals)
    {
        var b = BigFraction.Zero;
        foreach (var val in vals ?? throw new ArgumentNullException(nameof(vals)))
        {
            b += val;
        }
        return b;
    }
}
