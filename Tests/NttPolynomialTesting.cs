using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class NttPolynomialTesting
{
    [Fact(Skip = "Reworking the NTTPolynomial class")]
    public void NttHoldsTheCorrectNumberOfElements()
    {
        var p = new Polynomial(new List<BigInteger>{ 1 });
        const int n = 4;
        var rq = new PolynomialRing(17, n);
        //var nttP = new NttPolynomial(rq, p);

        //var res = nttP.GetNttMembers();
        
        //Assert.Equal(Math.Pow(2, Math.Log2(n)), res.Count);
    }

    [Fact(Skip = "Reworking the NTTPolynomial class")]
    public void NttRepresentation_WithBabyKyberRing_Using1Polynomial_Gives1Polynomial()
    {
        var p = new Polynomial(new List<BigInteger>{ 1 });
        const int n = 4;
        var rq = new PolynomialRing(17, n);
        //var nttP = new NttPolynomial(rq, p);

        //var res = nttP.GetNttMembers();

        //Assert.True(res.TrueForAll(poly => TestHelpers.ComparePolynomials(poly, p)));
    }
    
    [Fact(Skip = "Reworking the NTTPolynomial class")]
    public void NttRepresentation_WithBabyKyberRing_Using0Polynomial_Gives0Polynomial()
    {
        var p = new Polynomial(new List<BigInteger>{ 0 });
        const int n = 4;
        var rq = new PolynomialRing(17, n);
        //var nttP = new NttPolynomial(rq, p);

        //var res = nttP.GetNttMembers();

        //Assert.True(res.TrueForAll(poly => TestHelpers.ComparePolynomials(poly, p)));
    }

    [Fact(Skip = "Unsure if this will work with new way of NTT")]
    public void Ntt_OnBabyKyber_GivesCorrectResults()
    {
        var p = new Polynomial(new List<BigInteger>{ 0 });
        const int n = 4;
        var rq = new PolynomialRing(17, n);
        //var nttP = new NttPolynomial(rq, p);

        //var res = nttP.GetNttModulus();
        var expected = new List<Polynomial>
        {
            new(new List<BigInteger> { -2, 0, 1 }),
            new(new List<BigInteger> { 2, 0, 1 }),
            new(new List<BigInteger> { -8, 0, 1 }),
            new(new List<BigInteger> { 8, 0, 1 })
        };
        
        //Assert.True(TestHelpers.ComparePolynomialLists(expected, res));
    }

    [Fact]
    public void testing_test()
    {
        var p = new short[500];
        p[0] = 1;
        p[1] = 2;
        p[2] = 3;
        
        var x = NttPoly.Ntt(p);
        var res = NttPoly.InvNtt(x);
        
        // todo: why does this not give me the real result??
        
        var h = 0;
    }

    [Fact(Skip = "This should work, right?")]
    public void TestingFromTheArticle()
    {
        // todo: try to calculate the first few steps by hand, then this should make sense.
        const int rootOfUnity = 17;
        var rq = new PolynomialRing(3329, 256); // the parameters from the code.

        var math = BigInteger.ModPow(-rootOfUnity, 1, 3329);
        var poly = new Polynomial(new List<BigInteger> { math, 0, 1});
        for (var i = 1; i < 127; i++)
        {
            var zeta = BigInteger.ModPow(-rootOfUnity, 2 * i + 1, 3329);
            var p = new Polynomial(new List<BigInteger> { zeta, 0, 1 });
            poly = rq.ModPoly(rq.Mult(poly, p), TestHelpers.GetStandardModPoly(256));
        }
        
        Assert.True(TestHelpers.ComparePolynomials(poly, new Polynomial(new List<BigInteger>{1,0,1})));
    }
}
