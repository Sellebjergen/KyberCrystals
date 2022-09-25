using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class PolynomialRingTests {
    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialAddition_1plus1_gives_2(BigInteger q, BigInteger n)
    {
        var rq = new PolynomialRing(q, n);

        var p1 = new Polynomial(new List<BigInteger> {1});
        var p2 = new Polynomial(new List<BigInteger> {1});

        var res = rq.Add(p1, p2);
       
        Assert.Equal(2, res.GetCoefficient(0));
    }

    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialAddition_1plus0_gives_1(BigInteger q, BigInteger n) {
        var rq = new PolynomialRing(q, n);
        var p1 = new Polynomial(new List<BigInteger> {1});
        var p2 = new Polynomial(new List<BigInteger> {0});

        var res = rq.Add(p1, p2);
       
        Assert.Equal(1, res.GetCoefficient(0));
    }

    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialAddition_DifferentDegrees_ReturnExpected(BigInteger q, BigInteger n) {
        var rq = new PolynomialRing(q, n);
        var p1 = new Polynomial(new List<BigInteger> {1});
        var p2 = new Polynomial(new List<BigInteger> {1, 1});

        var res = rq.Add(p1, p2);
       
        Assert.Equal(new List<BigInteger> {2, 1}, res.GetCoefficients());
    }

    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialMult_1Times1_Gives1Polynomial(BigInteger q, BigInteger n)
    {
        var rq = new PolynomialRing(q, n);
        var p1 = new Polynomial(new List<BigInteger> { 1 });
        var p2 = new Polynomial(new List<BigInteger> { 1 });

        var res = rq.Mult(p1, p2);
        var expected = new Polynomial(new List<BigInteger> { 1 });
        
        Assert.True(TestHelpers.ComparePolynomials(expected, res));
    }
    
    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialMult_1TimesX2_GivesX2Polynomial(BigInteger q, BigInteger n)
    {
        var rq = new PolynomialRing(q, n);
        var p1 = new Polynomial(new List<BigInteger> { 1 });
        var p2 = new Polynomial(new List<BigInteger> { 0, 0, 1 });

        var res = rq.Mult(p1, p2);
        
        Assert.True(TestHelpers.ComparePolynomials(p2, res));
    }
    
    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialMult_0Times1_Gives0Polynomial(BigInteger q, BigInteger n)
    {
        var rq = new PolynomialRing(q, n);
        var p1 = new Polynomial(new List<BigInteger> { 1 });
        var p2 = new Polynomial(new List<BigInteger> { 0 });

        var res = rq.Mult(p1, p2);
        var expected = new Polynomial(new List<BigInteger> { 0 });
        
        Assert.True(TestHelpers.ComparePolynomials(expected, res));
    }

    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialMult_2DegTimes2Deg_Gives4DegPoly(BigInteger q, BigInteger n)
    {
        var rq = new PolynomialRing(q, n);
        var p1 = new Polynomial(new List<BigInteger> { 1, 1, 1 });
        var p2 = new Polynomial(new List<BigInteger> { 1, 1, 1 });

        var res = rq.Mult(p1, p2);

        Assert.Equal(4, res.GetDegree());
    }

    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialMult_1DegTimes_3Deg_GivesDeg4Poly(BigInteger q, BigInteger n)
    {
        var rq = new PolynomialRing(q, n);
        var p1 = new Polynomial(new List<BigInteger> { 1, 1, 1, 1 });
        var p2 = new Polynomial(new List<BigInteger> { 1, 1 });

        var res = rq.Mult(p1, p2);

        Assert.Equal(4, res.GetDegree());
    }

    [Theory,
    InlineData(17, 5),
    InlineData(3329, 256)]
    public void PolynomialMult_Cannot_ExceedRingDegree(BigInteger q, BigInteger n)
    {
        var rq = new PolynomialRing(q, n);

        var p1 = TestHelpers.GetStandardModPoly((int) n);
        var p2 = TestHelpers.GetStandardModPoly((int) n);

        var res = rq.Mult(p1, p2);

        Assert.True(res.GetDegree() < rq.N); 
        // todo: should this be less than or equal? is x^17 in the ring x^17 + 1?
    }
    
    [Fact]
    public void PolynomialMult_127MultiplicationOfDeg2_GivesDeg254()
    {
        var rq = new PolynomialRing(3329, 256);
        var p = new Polynomial(new List<BigInteger> { 1 });

        for (var i = 0; i < 127; i++)
        {
            p = rq.Mult(p, new Polynomial(new List<BigInteger> { 0, 0, 1 }));
        }
        
        Assert.Equal(254, p.GetDegree());
    }
    
    [Fact]
    public void Test_PolynomialLongDivision()
    {
        var param = new Constants().Kyber512();
        var degreePol = 1;
        var rq = new PolynomialRing(param.Q, degreePol);
        var p = new Polynomial(new List<BigInteger> {1, 3, 1});     // 1 + 3x + x^2

        var res = rq.ModPoly(p, TestHelpers.GetStandardModPoly(degreePol));
        var expectedPoly = new Polynomial(new List<BigInteger> { 3328 });
        
        Assert.True(TestHelpers.ComparePolynomials(expectedPoly, res));
    }
    
    [Fact]
    public void Test2_PolynomialLongDivision()
    {
        var param = new Constants().Kyber512();
        var degreePol = 1;
        var rq = new PolynomialRing(param.Q, degreePol);
        var p = new Polynomial(new List<BigInteger> {2, 3, 1});     // 2 + 3x + x^2

        var res = rq.ModPoly(p, TestHelpers.GetStandardModPoly(degreePol));
        var expectedPoly = new Polynomial(new List<BigInteger> { 0 });
        
        Assert.True(TestHelpers.ComparePolynomials(expectedPoly, res));
    }

    [Fact]
    public void Test1_DuringDebuggingOfArticleProduct()
    {
        var p1 = new Polynomial(new List<BigInteger>{ -17, 0, 1 });
        var p2 = new Polynomial(new List<BigInteger>{ -3312, 0, 1 });
        var rq = new PolynomialRing(3329, 256); // todo: var kyber params (maybe add to testhelper that I can this ring?

        var res = rq.Mult(p1, p2);

        var expected = new Polynomial(new List<BigInteger> { 3040, 0, 0, 0,1});
        Assert.True(TestHelpers.ComparePolynomials(expected, res));
    }

    [Fact]
    public void Test2_DuringDebuggingOfArticleProduct()
    {
        var p1 = new Polynomial(new List<BigInteger>{ 2298, 0, 3040, 0,568,0,1 });
        var p2 = new Polynomial(new List<BigInteger>{ -3281, 0, 1 });
        var rq = new PolynomialRing(3329, 256); // todo: var kyber params (maybe add to testhelper that I can this ring?

        var res = rq.Mult(p1, p2);

        var expected = new Polynomial(new List<BigInteger> { 447, 0, 1742, 0, 343, 0, 616, 0, 1});
        Assert.True(TestHelpers.ComparePolynomials(expected, res));
    }
}