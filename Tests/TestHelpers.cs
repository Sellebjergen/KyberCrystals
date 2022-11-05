using System.Numerics;
using KyberCrystals;

namespace Tests;

public static class TestHelpers
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
        if (p1.GetCoefficients().Count != p2.GetCoefficients().Count)
            return false;

        for (var i = 0; i < p1.GetCoefficients().Count; i++)
        {
            if (p1.GetCoefficient(i) != p2.GetCoefficient(i))
                return false;
        }

        return true;
    }

    public static Polynomial GetStandardModPoly(int n)
    {
        var coef = new List<BigInteger> { 1 };

        for (var i = 0; i < n - 1; i++)
        {
            coef.Add(BigInteger.Zero);
        }
        coef.Add(BigInteger.One);
        return new Polynomial(coef);
    }

    public static bool ComparePolynomialLists(List<Polynomial> p, List<Polynomial> p2)
    {
        if (p.Count != p2.Count)
            return false;

        for (var i = 0; i < p.Count - 1; i++)
        {
            if (!ComparePolynomials(p[i], p2[i]))
                return false;
        }

        return true;
    }
    
    public static bool ComparePolynomialArrays(Polynomial[] p, Polynomial[] p2)
    {
        return ComparePolynomialLists(new List<Polynomial>(p), new List<Polynomial>(p2));
    }
    
    public static string GetRepeatedChar(char ch, int amount)
    {
        string res = "";
        for (var i = 0; i < amount; i++)
            res += ch;
        return res;
    }
    
    public static bool ElementsAreClose(int x, int xPrime, int d, int q)
    {
        var diff = x - xPrime;
        var exponent = q / Math.Pow(2, d + 1);
        return diff <= Convert.ToInt32(exponent);
    }
} 