using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class KyberTests
{
    [Fact]
    public void KeyGen_Returns_CorrectLenghtPublicKey()
    {
        // TODO: Fill in some more interesting features right here.
    }

    [Fact]
    public void TheModPoly_CanBeWrittenAs_ProductOf2DegPolynomias()
    {
        var rq = new PolynomialRing(3329, 256); // todo: replace with kyber params dynamically.
        
        var rootOfUnity = Utils.GetRootOfUnity(256, 3329); // todo kyber params
        var sum = new Polynomial(new List<BigInteger>
        {
            BigInteger.ModPow(-rootOfUnity, 2*Utils.Br7(0) + 1, 3329), // todo: kyber param
            0,
            1
        });
        for (var i = 1; i < 128; i++)
        {
            var zeta = BigInteger.ModPow(rootOfUnity, 2 * Utils.Br7(i) + 1, 3329); // kyber param
            var p = new Polynomial(new List<BigInteger> { -zeta, 0, 1 });
            sum = rq.Mult(sum, p);
        }

        var expected = new Polynomial(new List<BigInteger>{0 }); // equal to x^256 + 1 in the ring!
        Assert.True(TestHelpers.ComparePolynomials(expected, sum));
    }
}