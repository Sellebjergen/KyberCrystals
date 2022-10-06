using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

// TODO: the tests in here expects the results to be in strings containing bits, thus a factor 8 is needed at all times.

public class KyberTests
{
    [Fact]
    public void CCAKEM_keygen_Returns_CorrectlySizedPublicKey() {
        var param = new Constants().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.CCAKEM_keygen();

        Assert.Equal(12 * param.K * param.N + (32 * 8), pk.Length);
    }
    
    [Fact]
    public void CCAKEM_keygen_Returns_CorrectlySizedSecretKey() {
        var param = new Constants().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_ , sk) = kyber.CCAKEM_keygen();

        Assert.Equal(24 * param.K * param.N + (96 * 8), sk.Length);
    }
    
    [Fact]
    public void CPAPKE_encrypt_decrypt_512()
    {
        var param = new Constants().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, sk) = kyber.CPAPKE_KeyGen();

        var m = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var coins = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var c = kyber.CPAPKE_encrypt(pk, m, coins);
        var mPrime = kyber.CPAPKE_decrypt(sk, c);

        Assert.Equal(m, mPrime);
    }

    [Fact]
    public void CPAPKE_encrypt_decrypt_768()
    {
        var param = new Constants().Kyber768();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, sk) = kyber.CPAPKE_KeyGen();

        var m = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var coins = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var c = kyber.CPAPKE_encrypt(pk, m, coins);
        var mPrime = kyber.CPAPKE_decrypt(sk, c);

        Assert.Equal(m, mPrime);
    }

    [Fact]
    public void CPAPKE_encrypt_decrypt_1024()
    {
        var param = new Constants().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, sk) = kyber.CPAPKE_KeyGen();

        var m = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var coins = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var c = kyber.CPAPKE_encrypt(pk, m, coins);
        var mPrime = kyber.CPAPKE_decrypt(sk, c);

        Assert.Equal(m, mPrime);
    }

    [Fact]
    public void CPAPKE_encrypt_returns_CorrectSizeCiphertext()
    {
        var param = new Constants().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, _) = kyber.CPAPKE_KeyGen();

        var m = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var coins = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var c = kyber.CPAPKE_encrypt(pk, m, coins);

        Assert.Equal(param.Du * param.K * param.N + param.Dv * param.N, c.Length);
    }

    [Fact]
    public void KeyGen_Returns_CorrectLengthSecretKey()
    {
        var param = new Constants().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.CPAPKE_KeyGen();

        Assert.Equal(12 * param.K * param.N, sk.Length);
    }

    [Fact]
    public void KeyGen_Returns_CorrectLengthPublicKey()
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

        Assert.NotNull(matrix[0, 0]);
        Assert.NotNull(matrix[0, 1]);
        Assert.NotNull(matrix[1, 0]);
        Assert.NotNull(matrix[1, 1]);
    }

    [Fact]
    public void TheModPoly_CanBeWrittenAs_ProductOf2DegPolynomials()
    {
        var rq = new PolynomialRing(3329, 256); // todo: replace with kyber params dynamically.

        var rootOfUnity = Utils.GetRootOfUnity(256, 3329); // todo kyber params
        var sum = new Polynomial(new List<BigInteger>
        {
            BigInteger.ModPow(-rootOfUnity, 2 * Utils.Br7(0) + 1, 3329), // todo: kyber param
            0,
            1
        });

        for (var i = 1; i < 128; i++)
        {
            var zeta = BigInteger.ModPow(rootOfUnity, 2 * Utils.Br7(i) + 1, 3329); // kyber param
            var p = new Polynomial(new List<BigInteger> { -zeta, 0, 1 });
            sum = rq.Mult(sum, p);
        }

        var expected = new Polynomial(new List<BigInteger> { 0 }); // equal to x^256 + 1 in the ring!
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
