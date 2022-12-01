using KyberCrystals;
using Xunit;

namespace Tests;

public class KeyTests
{
    [Fact]
    public void PublicKey_Generation_GivesTheSameAsFromHex()
    {
        var param = new ParameterGen().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.Keygen();

        // Creates hex to be hardcoded
        var bytes = Utils.GetBytes(pk.GetCombinedString());
        var hex = Convert.ToHexString(bytes);
        
        var pkPrime = PublicKey.CreateFromHex(hex, param.K);

        Assert.Equal(pk.GetCombinedString(), pkPrime.GetCombinedString());
    }
    
    
}