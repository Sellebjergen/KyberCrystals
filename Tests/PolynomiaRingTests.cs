using KyberCrystals;
using Xunit;
using System.Numerics;


public class PolynomialRingTests {
    [Fact]
    public void PolynomialAddition_1plus1_gives_2()
    {
        var rq = new PolynomialRing(new BigInteger(5), new BigInteger(17));

        var p1 = new Polynomial(new List<BigInteger> {1});
        var p2 = new Polynomial(new List<BigInteger> {1});

        var res = rq.Add(p1, p2);
       
        Assert.Equal(2, res.GetCoefficient(0));
    }

    [Fact]
    public void PolynomialAddition_1plus0_gives_1() {
        var rq = new PolynomialRing(new BigInteger(2), new BigInteger(17));
        var p1 = new Polynomial(new List<BigInteger> {1});
        var p2 = new Polynomial(new List<BigInteger> {0});

        var res = rq.Add(p1, p2);
       
        Assert.Equal(1, res.GetCoefficient(0));
    }

    [Fact]
    public void PolynomialAddition_DifferentDegres_ReturnExpected() {
        var rq = new PolynomialRing(new BigInteger(5), new BigInteger(17));
        var p1 = new Polynomial(new List<BigInteger> {1});
        var p2 = new Polynomial(new List<BigInteger> {1, 1});

        var res = rq.Add(p1, p2);
       
        Assert.Equal(new List<BigInteger> {2, 1}, res.GetCoefficients());
    }
}