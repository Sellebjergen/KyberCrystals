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
    public void GetKyberNtt_Returns_ElementOfLength128()
    {
        var param = new Constants().Kyber512();
        var rq = new PolynomialRing(param.Q, param.N);
        var p = new Polynomial(new List<BigInteger> { 1, 0, 1 });
        var ntt = new NttPolynomial(param, rq, p);
        
        Assert.Equal(128, ntt.GetKyberNtt(p).Count);
    }

    [Fact]
    public void testing()
    {
        var p = new Polynomial(new List<BigInteger> { 1, 1, 1 });
        var p_short = new short[256];
        p_short[0] = 1;
        p_short[1] = 1;
        p_short[2] = 1;
        
        var x = NttPolyConverter.Ntt(p_short);
        var z = NttPolyConverter.InvNtt(x);
        var u = NttPolyConverter.FromMontgomery(z);
        
        Assert.True(u[0] == 1);
        Assert.True(u[1] == 1);
        Assert.True(u[2] == 1);
        for (var i = 3; i < p_short.Length; i++) {
            Assert.True(p_short[i] == 0);
        }
    }
}
