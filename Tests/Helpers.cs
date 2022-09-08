using KyberCrystals;

namespace Tests;

public static class Helpers
{
    public static byte[] GetRandomBytes(int length)
    {
        var random = new Random();

        var bytes = new byte[length];
        random.NextBytes(bytes);

        return bytes;
    }

    public static byte GetRandomByte()
    {
        return GetRandomBytes(1).First();
    }

    public static bool ComparePolynomials(Polynomial p1, Polynomial p2)
    {
        if (p1.GetDegree() != p2.GetDegree())
            return false;
        
        for (var i = 0; i < p1.GetDegree(); i++)
        {
            if (p1.GetCoefficient(i) != p2.GetCoefficient(i))
                return false;
        }

        return true;
    }
}