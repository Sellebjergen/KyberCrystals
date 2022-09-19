using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class TestHelpersTest
{
    [Fact]
    public void GetStandardModPoly_Returns_ExpectedPolynomial()
    {
        var res = TestHelpers.GetStandardModPoly(1);
        var expected = new Polynomial(new List<BigInteger> { 1, 1 });
        
        Assert.True(TestHelpers.ComparePolynomials(expected, res));
    }

    [Fact]
    public void GetStandardModPoly_Returns_ExpectedDegreePolynomial()
    {
        var n = 256;
        var res = TestHelpers.GetStandardModPoly(n);
        
        Assert.Equal(n, res.GetDegree());
    }

    [Fact]
    public void ComparePolynomials_ReturnsFalse_ForDifferentPolynomials()
    {
        var p1 = new Polynomial(new List<BigInteger> { 1, 1, 1 });
        var p2 = new Polynomial(new List<BigInteger> { 1, 1 });
        
        Assert.False(TestHelpers.ComparePolynomials(p1, p2));
    }
    
    [Fact]
    public void ComparePolynomials_ReturnsTrue_ForSamePolynomial()
    {
        var p = new Polynomial(new List<BigInteger> { 1, 1, 1 });
        Assert.True(TestHelpers.ComparePolynomials(p, p));
    }

    [Fact]
    public void ComparePolynomialLists_ReturnsTrue_ForTheSameList()
    {
        var p1 = new List<Polynomial>{new(new List<BigInteger> { 1 })};
        TestHelpers.ComparePolynomialLists(p1, p1);
    }
    
    [Fact]
    public void ComparePolynomialLists_ReturnsFalse_ForDifferentLists()
    {
        var p1 = new List<Polynomial>{new(new List<BigInteger> { 1 })};
        var p2 = new List<Polynomial>{new(new List<BigInteger> { 2 })};
        TestHelpers.ComparePolynomialLists(p1, p2);
    }
}