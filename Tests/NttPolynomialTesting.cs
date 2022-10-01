using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class NttPolynomialTesting
{
    [Fact]
    public void Ntt_CanBeRevertedBy_InvNttAndFromMontgomery()
    {
        var p = new Polynomial(new List<BigInteger> { 1, 1, 1 });
        
        var x = NttPolyConverter.Ntt(p.GetPaddedCoefficients(256));
        var z = NttPolyConverter.InvNtt(x);
        var u = NttPolyConverter.FromMontgomery(z);
        
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
        var p = new Polynomial(new List<BigInteger> { 2,2,2,2,2,2,2,2,2,2,2,2,2,2 });
        
        var x = NttPolyConverter.Ntt(p.GetPaddedCoefficients(256));
        var z = NttPolyConverter.InvNtt(x);
        var u = NttPolyConverter.FromMontgomery(z);
        
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
        var p = new Polynomial(new List<BigInteger> { 1, 1, 1 });
    }
}
