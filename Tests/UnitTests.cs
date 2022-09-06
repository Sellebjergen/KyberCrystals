using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class PolynomiaTests
{
    [Fact]
    public void Parse()
    {
        // TODO: Create some better tests than those for the Parse function
        var p = new Constants().Kyber512();
        var rq = new PolynomialRing(new BigInteger(3329), new BigInteger(5));

    }

    [Fact]
    public void RunningCbd_TwiceOnSameInput_GivesSameOutput()
    {
        var p = new Constants().Kyber512();
        var bytes = Helpers.GetRandomBytes(64 * p.Eta1);

        var rq = new PolynomialRing(p.Q, p.N);
        var res1 = rq.Cbd(bytes, p.Eta1);
        var res2 = rq.Cbd(bytes, p.Eta1);

        Assert.True(Helpers.ComparePolynomials(res1, res2));
    }

    [Fact]
    public void RunningDecode_TwiceOnSameInput_GivesSameOutput()
    {
        var l = 8;
        var p = new Constants().Kyber512();
        var bytes = Helpers.GetRandomBytes(32 * l);

        var rq = new PolynomialRing(p.Q, p.N);
        var res1 = rq.Decode(bytes, l);
        var res2 = rq.Decode(bytes, l);
        
        Assert.True(Helpers.ComparePolynomials(res1, res2));
    }
}

public static class Helpers
{
    public static byte[] GetRandomBytes(int length)
    {
        var random = new Random();

        var bytes = new byte[length];
        random.NextBytes(bytes);

        return bytes;
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