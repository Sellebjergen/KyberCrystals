using System.Numerics;
using System.Runtime.CompilerServices;
using KyberCrystals;
using Xunit;

namespace Tests;

public class NttPolynomialTesting
{
    [Fact]
    public void Ntt_CanBeRevertedBy_InvNttAndFromMontgomery()
    {
        var nttPolyConverter = new NttPolyHelper();
        var p = new Polynomial(new List<BigInteger> { 1, 1, 1 });
        
        var x = nttPolyConverter.Ntt(p.GetPaddedCoefficients(256));
        var z = nttPolyConverter.InvNtt(x);
        var u = nttPolyConverter.FromMontgomery(z);
        
        Assert.True(u[0] == 1);
        Assert.True(u[1] == 1);
        Assert.True(u[2] == 1);
        for (var i = 3; i < p.GetPaddedCoefficients(256).Count; i++) {
            Assert.True(p.GetCoefficient(i) == 0);
        }
    }
    
    [Fact]
    public void Ntt_CanBeRevertedBy_InvNttAndFromMontgomery_only2Pols()
    {
        var nttPolyConverter = new NttPolyHelper();
        var p = new Polynomial(new List<BigInteger> { 2,2,2,2,2,2,2,2,2,2,2,2,2,2 });
        
        var x = nttPolyConverter.Ntt(p.GetPaddedCoefficients(256));
        var z = nttPolyConverter.InvNtt(x);
        var u = nttPolyConverter.FromMontgomery(z);
        
        for (var i = 0; i < p.GetLengthOfPolynomial(); i++ )
        {
            Assert.True(u[i] == 2);
        }
        for (var i = p.GetLengthOfPolynomial(); i < 256; i++) 
        {
            Assert.True(u[i] == 0);
        }
    }
    
    [Fact]
    public void BaseMultiplication_GivesTheOriginalPolynomials_Multiplied()
    {
        var nttPolyConverter = new NttPolyHelper();
        var p1 = new Polynomial(new List<BigInteger> { 1 });
        var p2 = new Polynomial(new List<BigInteger> { 2 });

        var p1Ntt = nttPolyConverter.Ntt(p1.GetPaddedCoefficients(256));
        var p2Ntt = nttPolyConverter.Ntt(p2.GetPaddedCoefficients(256));
        
        var res = nttPolyConverter.Multiplication(p1Ntt, p2Ntt);
        var res2 = nttPolyConverter.InvNtt(res.GetPaddedCoefficients(256));
        var res3 = nttPolyConverter.FromMontgomery(res2);
        var res4 = nttPolyConverter.ReduceCoefHacks(new Polynomial(res3)); // todo: this does not seem right???
        
        var rq = new PolynomialRing(3329, 256);
        var expected = rq.Mult(p1, p2);
        
        Assert.True(TestHelpers.ComparePolynomials(expected, res4));
    }
    
    [Fact]
    public void BaseMultiplication_GivesTheOriginalPolynomials_Multiplied2()
    {
        var nttPolyConverter = new NttPolyHelper();
        var p1 = new Polynomial(new List<BigInteger> { 2 });
        var p2 = new Polynomial(new List<BigInteger> { 2 });

        var p1Ntt = nttPolyConverter.Ntt(p1.GetPaddedCoefficients(256));
        var p2Ntt = nttPolyConverter.Ntt(p2.GetPaddedCoefficients(256));
        
        var res = nttPolyConverter.Multiplication(p1Ntt, p2Ntt);
        var res2 = nttPolyConverter.InvNtt(res.GetPaddedCoefficients(256));
        var res3 = nttPolyConverter.FromMontgomery(res2);
        var res4 = nttPolyConverter.ReduceCoefHacks(new Polynomial(res3)); // todo: this does not seem right???
        
        var rq = new PolynomialRing(3329, 256);
        var expected = rq.Mult(p1, p2);
        
        Assert.True(TestHelpers.ComparePolynomials(expected, res4));
    }
    
    [Fact]
    public void BaseMultiplication_GivesTheOriginalPolynomials_Multiplied3()
    {
        var nttPolyConverter = new NttPolyHelper();
        var p1 = new Polynomial(new List<BigInteger> { 2 });
        var p2 = new Polynomial(new List<BigInteger> { 2, 1, 1, 1});

        var p1Ntt = nttPolyConverter.Ntt(p1.GetPaddedCoefficients(256));
        var p2Ntt = nttPolyConverter.Ntt(p2.GetPaddedCoefficients(256));
        
        var res = nttPolyConverter.Multiplication(p1Ntt, p2Ntt);
        var res2 = nttPolyConverter.InvNtt(res.GetPaddedCoefficients(256));
        var res3 = nttPolyConverter.FromMontgomery(res2);
        var res4 = nttPolyConverter.ReduceCoefHacks(new Polynomial(res3)); // todo: this does not seem right???
        
        var rq = new PolynomialRing(3329, 256);
        var expected = rq.Mult(p1, p2);
        
        Assert.True(TestHelpers.ComparePolynomials(expected, res4));
    }
    
    [Fact]
    public void ZetasHasBeenCalculated_According_toFormulaInArticle_UsingMont2_16()
    {
        var nttPolyConverter = new NttPolyHelper();
        var montFactor = BigInteger.Pow(2, 16);
        var rootOfUnity = 17;
        for (var i = 0; i < nttPolyConverter.NttZetas.Count; i++)
        {
            var x = montFactor * BigInteger.ModPow(rootOfUnity, Utils.Br7(i), 3329); // kyber param
            Assert.Equal(nttPolyConverter.NttZetas[i], BigInteger.ModPow(x,1,3329)); // kyber param
        }
    }
    
    [Fact]
    public void MontgomeryInv_ForMontFactor2_16_is_169()
    {
        var x = BigInteger.ModPow(2, 16, 3329);
        var y = BigInteger.ModPow(x * 169, 1, 3329);
        
        Assert.Equal(1, y);
    }
    
    [Fact]
    public void QInverseIs_62209_ModMontgomery() {
        Assert.Equal(1, BigInteger.ModPow(62209 * 3329, 1, BigInteger.Pow(2, 16)));
    }
}

