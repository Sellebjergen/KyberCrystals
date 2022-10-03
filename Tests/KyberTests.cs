using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class KyberTests
{
    [Fact]
    public void KeyGen_Returns_CorrectLenghtPublicKey()
    {
        var param = new Constants().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, sk) = kyber.CPAPKE_KeyGen();
        
        Assert.Equal(12 * param.K * param.N, pk.Length);
    }
    
    [Fact]
    public void KeyGen_Returns_CorrectLenghtPrivateKey()
    {
        var param = new Constants().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.CPAPKE_KeyGen();
        
        Assert.Equal(12 * param.K * param.N + 32 * 8, pk.Length);
    }
    
    [Fact]
    public void MatrixGeneration_DoesNotInclude_nullValues()
    {
        var rq = new PolynomialRing(3329, 256);
        var param = new Constants().Kyber512();
        var d = Utils.GetRandomBytes(32);
        var (rho, _) = Utils.G(d);
        
        var matrix = new Kyber(param, rq).GenerateMatrix(rho, param.K);
        
        Assert.NotNull(matrix[0,0]);
        Assert.NotNull(matrix[0,1]);
        Assert.NotNull(matrix[1,0]);
        Assert.NotNull(matrix[1,1]);
    }

    [Fact]
    public void TheModPoly_CanBeWrittenAs_ProductOf2DegPolynomials()
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
    
    [Fact]
    public void EncodeAndDecodePolynomial_Gives_SamePolynomial()
    {
        var p = new Polynomial(new List<BigInteger> { 1, 1, 1, });
        var res = Utils.Decode(12, Utils.Encode(12, p));
        res.RemoveTrailingZeros();
        
        Assert.True(TestHelpers.ComparePolynomials(p, res));
    }
    
    [Fact]
    public void EncodeAndDecodePolynomial_Gives_SamePolynomial2()
    {
        var p = new Polynomial(new List<BigInteger> { 2, 2, 2, });
        var res = Utils.Decode(12, Utils.Encode(12, p));
        res.RemoveTrailingZeros();
        
        Assert.True(TestHelpers.ComparePolynomials(p, res));
    }
    
    [Fact]
    public void EncodeAndDecodePolynomial_Gives_SamePolynomial3()
    {
        var p = new Polynomial(new List<BigInteger> { 3, 3, 3, });
        var res = Utils.Decode(12, Utils.Encode(12, p));
        res.RemoveTrailingZeros();
        
        Assert.True(TestHelpers.ComparePolynomials(p, res));
    }
}