using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class KyberTests
{
    [Fact]
    public void KeyGen_Returns_CorrectLenghtPublicKey()
    {
        var kyber = new Kyber(new Constants().Kyber512(), new PolynomialRing(3329, 256));
        kyber.CPAPKE_KeyGen();
        
        // TODO: Fill in some more interesting features right here.
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
        
        Assert.True(TestHelpers.ComparePolynomials(p, res));
    }
}