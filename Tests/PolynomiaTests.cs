using System.Numerics;
using System.Reflection.Metadata;
using KyberCrystals;
using Xunit;

namespace Tests;

// TODO: Add some more theories, to test for more polynomials.

public class PolynomiaTests
{
    [Fact]
    public void Parse()
    {
        // TODO: Create some better tests than those for the Parse function

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

    [Fact]
    public void AskingFor_NotSetCoefficient_Returns0() {
        var p = new Polynomial(new List<BigInteger> {1, 2, 3});
        var notSetCoef = p.GetCoefficient(8);

        Assert.Equal(0, notSetCoef);
    }

    [Fact]
    public void Modulo_Runs_AfterAddOperation()
    {
        var param = new Constants().Kyber512();
        var rq = new PolynomialRing(param.Q, param.N);
        var poly1 = new Polynomial(new List<BigInteger> { param.Q - 1, param.Q - 1, param.Q - 1 });
        var poly2 = new Polynomial(new List<BigInteger> { 1, 1, 1 });

        var res = rq.Add(poly1, poly2);
        var expected = new Polynomial(new List<BigInteger> { 0, 0, 0 });

        Assert.True(TestHelpers.ComparePolynomials(expected, res));
    }

    [Fact]
    public void Multiplication_OverRq_Works()
    {
        var param = new Constants().Kyber512();
        var rq = new PolynomialRing(param.Q, param.N);
        
        var poly1 = new Polynomial(new List<BigInteger> { 1, 1, 1});
        var poly2 = new Polynomial(new List<BigInteger> { 1, 1, 1 });
        var res = rq.Mult(poly1, poly2);
        
        var expPoly = new Polynomial(new List<BigInteger> { 1, 2, 3, 2, 1 }); // found with google
        Assert.True(TestHelpers.ComparePolynomials(res, expPoly));
    }

    [Fact]
    public void Modulo_Runs_AfterMultOperation()
    {
        var param = new Constants().Kyber512();
        var rq = new PolynomialRing(param.Q, param.N);
        var poly1 = new Polynomial(new List<BigInteger> { param.Q - 1, param.Q - 1, param.Q - 1 });
        var poly2 = new Polynomial(new List<BigInteger> { 2, 2, 2 });

        var res = rq.Mult(poly1, poly2);

        Assert.True(res.GetCoefficients().TrueForAll(x => x < param.Q));

}

    [Fact]
    public void RemoveTrailingZeros_ReducesTheDegree()
    {
        var p = new Polynomial(new List<BigInteger> { 1, 0, 0, 0});
        p.RemoveTrailingZeros();
        
        Assert.Equal(1, p.GetDegree());
    }

    [Fact]
    public void IsZero_Correct_ReturnsTrue()
    {
        var p = new Polynomial(new List<BigInteger> { 0, 0, 0, 0 });
        Assert.True(p.IsZeroPolynomial());
    }

    [Fact]
    public void IsZero_Incorrect_ReturnsFalse()
    {
        var p = new Polynomial(new List<BigInteger> { 1, 2, 3, 4 });
        Assert.False(p.IsZeroPolynomial());
        
    }

    [Fact]
    public void Sub_0PolyWith0Poly_Gives0Poly()
    {
        var param = new Constants().Kyber512();
        var rq = new PolynomialRing(param.Q, param.N);
        var poly1 = new Polynomial(new List<BigInteger> {0, 0, 0 });
        var poly2 = new Polynomial(new List<BigInteger> { 0, 0,0 });

        var res = rq.Sub(poly1, poly2);
        
        Assert.True(res.IsZeroPolynomial());
    }

    [Fact]
    public void Sub_1PolyWith1Poly_Gives0Poly()
    {
        var param = new Constants().Kyber512();
        var rq = new PolynomialRing(param.Q, param.N);
        var poly1 = new Polynomial(new List<BigInteger> {1,1,1,});
        var poly2 = new Polynomial(new List<BigInteger> { 1,1,1 });

        var res = rq.Sub(poly1, poly2);
        
        Assert.True(res.IsZeroPolynomial());
    }
    
    [Fact]
    public void Sub_0PolyWith1Poly_GivesParamQPoly()
    {
        var param = new Constants().Kyber512();
        var rq = new PolynomialRing(param.Q, param.N);
        var poly1 = new Polynomial(new List<BigInteger> {0,0,0,});
        var poly2 = new Polynomial(new List<BigInteger> { 1,1,1 });

        var res = rq.Sub(poly1, poly2);
        var expectedPoly = new Polynomial(new List<BigInteger> { param.Q - 1, param.Q - 1, param.Q - 1 });
        
        Assert.True(TestHelpers.ComparePolynomials(res, expectedPoly));
    }
}