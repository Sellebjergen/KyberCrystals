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

        var hex = GenerateHexFromBits(pk.GetAsBinaryOutput());
        var pkPrime = PublicKey.CreateFromHex(hex, param.K);

        Assert.Equal(pk.GetAsBinaryOutput(), pkPrime.GetAsBinaryOutput());
    }

    [Fact]
    public void PublicKey_Generation_GivesTheSameAsFromHex_Kyber768()
    {
        var param = new ParameterGen().Kyber768();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.Keygen();

        var hex = GenerateHexFromBits(pk.GetAsBinaryOutput());
        var pkPrime = PublicKey.CreateFromHex(hex, param.K);

        Assert.Equal(pk.GetAsBinaryOutput(), pkPrime.GetAsBinaryOutput());
    }

    [Fact]
    public void PublicKey_Generation_GivesTheSameAsFromHex_Kyber1024()
    {
        var param = new ParameterGen().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.Keygen();

        var hex = GenerateHexFromBits(pk.GetAsBinaryOutput());
        var pkPrime = PublicKey.CreateFromHex(hex, param.K);

        Assert.Equal(pk.GetAsBinaryOutput(), pkPrime.GetAsBinaryOutput());
    }

    [Fact]
    public void SecretKey_Generation_GivesTheSameAsFromHex_Kyber512()
    {
        var param = new ParameterGen().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.Keygen();

        var hex = GenerateHexFromBits(sk.GetAsBinaryOutput());
        var skPrime = SecretKey.CreateFromHex(hex, param.K);

        Assert.Equal(sk.GetAsBinaryOutput(), skPrime.GetAsBinaryOutput());
    }

    [Fact]
    public void SecretKey_Generation_GivesTheSameAsFromHex_Kyber768()
    {
        var param = new ParameterGen().Kyber768();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.Keygen();

        var hex = GenerateHexFromBits(sk.GetAsBinaryOutput());
        var skPrime = SecretKey.CreateFromHex(hex, param.K);

        Assert.Equal(sk.GetAsBinaryOutput(), skPrime.GetAsBinaryOutput());
    }

    [Fact]
    public void SecretKey_Generation_GivesTheSameAsFromHex_Kyber1024()
    {
        var param = new ParameterGen().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.Keygen();

        var hex = GenerateHexFromBits(sk.GetAsBinaryOutput());
        var skPrime = SecretKey.CreateFromHex(hex, param.K);

        Assert.Equal(sk.GetAsBinaryOutput(), skPrime.GetAsBinaryOutput());
    }

    [Fact]
    public void ImportingAndExportingFromHexGivesSameSecretKey()
    {
        var param = new ParameterGen().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.Keygen();

        var hex = pk.GetAsHexString();
        var pkPrime = PublicKey.CreateFromHex(hex, param.K);

        Assert.Equal(hex, pkPrime.GetAsHexString());
    }

    [Fact]
    public void ImportAndExportingFromHexGivesSamePublicKey()
    {
        var param = new ParameterGen().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.Keygen();

        var hex = sk.GetAsHexString();
        var skPrime = SecretKey.CreateFromHex(hex, param.K);

        Assert.Equal(hex, skPrime.GetAsHexString());
    }

    private string GenerateHexFromBits(string bits)
    {
        var bytes = Utils.GetBytes(bits);
        var hex = Convert.ToHexString(bytes);
        return hex;
    }
}
