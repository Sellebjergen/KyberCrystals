using KyberCrystals;
using Xunit;

namespace Tests;

public class KeyTests
{
    [Fact]
    public void PublicKey_Generation_GivesTheSameAsFromHex_Kyber512()
    {
        var param = new ParameterGen().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.Keygen();

        var hex = GenerateHexFromBits(pk.GetCombinedString());
        var pkPrime = PublicKey.CreateFromHex(hex, param.K);

        Assert.Equal(pk.GetCombinedString(), pkPrime.GetCombinedString());
    }
    
    [Fact]
    public void PublicKey_Generation_GivesTheSameAsFromHex_Kyber768()
    {
        var param = new ParameterGen().Kyber768();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.Keygen();

        var hex = GenerateHexFromBits(pk.GetCombinedString());
        var pkPrime = PublicKey.CreateFromHex(hex, param.K);

        Assert.Equal(pk.GetCombinedString(), pkPrime.GetCombinedString());
    }
    
    [Fact]
    public void PublicKey_Generation_GivesTheSameAsFromHex_Kyber1024()
    {
        var param = new ParameterGen().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.Keygen();

        var hex = GenerateHexFromBits(pk.GetCombinedString());
        var pkPrime = PublicKey.CreateFromHex(hex, param.K);

        Assert.Equal(pk.GetCombinedString(), pkPrime.GetCombinedString());
    }
    
    [Fact]
    public void SecretKey_Generation_GivesTheSameAsFromHex_Kyber512()
    {
        var param = new ParameterGen().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.Keygen();

        var hex = GenerateHexFromBits(sk.GetCombinedString());
        var skPrime = SecretKey.GenerateFromHex(hex, param.K);

        Assert.Equal(sk.GetCombinedString(), skPrime.GetCombinedString());
    }
    
    [Fact]
    public void SecretKey_Generation_GivesTheSameAsFromHex_Kyber768()
    {
        var param = new ParameterGen().Kyber768();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.Keygen();

        var hex = GenerateHexFromBits(sk.GetCombinedString());
        var skPrime = SecretKey.GenerateFromHex(hex, param.K);

        Assert.Equal(sk.GetCombinedString(), skPrime.GetCombinedString());
    }
    
    [Fact]
    public void SecretKey_Generation_GivesTheSameAsFromHex_Kyber1024()
    {
        var param = new ParameterGen().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.Keygen();

        var hex = GenerateHexFromBits(sk.GetCombinedString());
        var skPrime = SecretKey.GenerateFromHex(hex, param.K);

        Assert.Equal(sk.GetCombinedString(), skPrime.GetCombinedString());
    }
    
    private string GenerateHexFromBits(string bits)
    {
        var bytes = Utils.GetBytes(bits);
        var hex = Convert.ToHexString(bytes);
        return hex;
    }
}