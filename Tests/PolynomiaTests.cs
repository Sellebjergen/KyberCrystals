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
        var bytes = TestHelpers.GetRandomBytes(64 * p.Eta1);

        var rq = new PolynomialRing(p.Q, p.N);
        var res1 = rq.Cbd(bytes, p.Eta1);
        var res2 = rq.Cbd(bytes, p.Eta1);

        Assert.True(TestHelpers.ComparePolynomials(res1, res2));
    }

    [Fact]
    public void RunningDecode_TwiceOnSameInput_GivesSameOutput()
    {
        var l = 8;
        var p = new Constants().Kyber512();
        var bytes = TestHelpers.GetRandomBytes(32 * l);

        var rq = new PolynomialRing(p.Q, p.N);
        var res1 = rq.Decode(bytes, l);
        var res2 = rq.Decode(bytes, l);
        
        Assert.True(TestHelpers.ComparePolynomials(res1, res2));
    }
}