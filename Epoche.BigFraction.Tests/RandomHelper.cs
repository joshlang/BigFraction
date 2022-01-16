using System;
using System.Security.Cryptography;
using System.Threading;

namespace Epoche;
static class RandomHelper
{
    static readonly ThreadLocal<RandomNumberGenerator> Randoms = new(RandomNumberGenerator.Create);
    static void GetRandomBytes(Span<byte> buf) => Randoms.Value!.GetBytes(buf);
    public static long GetRandomInt64()
    {
        Span<byte> buf = stackalloc byte[8];
        GetRandomBytes(buf);
        return BitConverter.ToInt64(buf);
    }

    public static int GetRandomInt32()
    {
        Span<byte> buf = stackalloc byte[4];
        GetRandomBytes(buf);
        return BitConverter.ToInt32(buf);
    }
}
