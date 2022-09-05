using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var kyber = new Kyber(null);
        
        // TODO: assert that we can gen keys.
    }
}

public class PolynomiaTests
{
    [Fact]
    public void Parse()
    {
        Params p = new Constants().Kyber512();
        var rq = new PolynomialRing(new BigInteger(3329), new BigInteger(5));

        byte[] bytes = new byte[] { };
        rq.Parse(bytes);
    }
}