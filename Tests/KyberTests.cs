using System.Numerics;
using KyberCrystals;
using Xunit;

namespace Tests;

public class KyberTests
{
    [Fact]
    public void CCAKEM_encrypt_decrypt_kyber512()
    {
        var param = new ParameterGen().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(param.Q, param.N));
        var (pk, sk) = kyber.Keygen();
        var (c, kEnc) = kyber.Encapsulate(pk);
        var kDec = kyber.Decapsulate(c, sk);

        Assert.Equal(kEnc, kDec);
    }

    [Fact]
    public void CCAKEM_encrypt_decrypt_kyber768()
    {
        var param = new ParameterGen().Kyber768();
        var kyber = new Kyber(param, new PolynomialRing(param.Q, param.N));
        var (pk, sk) = kyber.Keygen();
        var (c, kEnc) = kyber.Encapsulate(pk);
        var kDec = kyber.Decapsulate(c, sk);

        Assert.Equal(kEnc, kDec);
    }

    [Fact]
    public void CCAKEM_encrypt_decrypt_kyber1024()
    {
        var param = new ParameterGen().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(param.Q, param.N));
        var (pk, sk) = kyber.Keygen();
        var (c, kEnc) = kyber.Encapsulate(pk);
        var kDec = kyber.Decapsulate(c, sk);

        Assert.Equal(kEnc, kDec);
    }

    [Fact]
    public void CPAPKE_encrypt_decrypt_512()
    {
        var param = new ParameterGen().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(param.Q, param.N));
        var (pk, sk) = kyber.CPAPKE_KeyGen();

        var m = Utils.GetBytes(TestHelpers.GetRepeatedChar('0', 256));
        var coins = Utils.GetBytes(TestHelpers.GetRepeatedChar('0', 256));
        var c = kyber.CPAPKE_encrypt(pk, m, coins);
        var mPrime = kyber.CPAPKE_decrypt(sk, c);

        Assert.Equal(Utils.BytesToBinaryString(m), mPrime);
    }

    [Fact]
    public void CPAPKE_encrypt_decrypt_512_2()
    {
        var param = new ParameterGen().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, sk) = kyber.CPAPKE_KeyGen();

        var m = Utils.GetBytes(TestHelpers.GetRepeatedChar('1', 256));
        var coins = Utils.GetBytes(TestHelpers.GetRepeatedChar('0', 256));
        var c = kyber.CPAPKE_encrypt(pk, m, coins);
        var mPrime = kyber.CPAPKE_decrypt(sk, c);

        Assert.Equal(Utils.BytesToBinaryString(m), mPrime);
    }

    [Fact]
    public void CPAPKE_encrypt_decrypt_768()
    {
        var param = new ParameterGen().Kyber768();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, sk) = kyber.CPAPKE_KeyGen();

        var m = Utils.GetBytes(TestHelpers.GetRepeatedChar('0', 256));
        var coins = Utils.GetBytes(TestHelpers.GetRepeatedChar('0', 256));
        var c = kyber.CPAPKE_encrypt(pk, m, coins);
        var mPrime = kyber.CPAPKE_decrypt(sk, c);

        Assert.Equal(Utils.BytesToBinaryString(m), mPrime);
    }

    [Fact]
    public void CPAPKE_encrypt_decrypt_1024()
    {
        var param = new ParameterGen().Kyber1024();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (pk, sk) = kyber.CPAPKE_KeyGen();

        var m = Utils.GetBytes(TestHelpers.GetRepeatedChar('0', 256));
        var coins = Utils.GetBytes(TestHelpers.GetRepeatedChar('0', 256));
        var c = kyber.CPAPKE_encrypt(pk, m, coins);
        var mPrime = kyber.CPAPKE_decrypt(sk, c);

        Assert.Equal(Utils.BytesToBinaryString(m), mPrime);
    }

    [Fact]
    public void KeyGen_Returns_CorrectLengthSecretKey()
    {
        var param = new ParameterGen().Kyber512();
        var kyber = new Kyber(param, new PolynomialRing(3329, 256));
        var (_, sk) = kyber.CPAPKE_KeyGen();

        Assert.Equal(12 * param.K * param.N, sk.Length);
    }

    [Fact]
    public void MatrixGeneration_DoesNotInclude_nullValues()
    {
        var rq = new PolynomialRing(3329, 256);
        var param = new ParameterGen().Kyber512();
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
        var rq = new PolynomialRing(3329, 256);

        var rootOfUnity = Utils.GetRootOfUnity(256, 3329);
        var sum = new Polynomial(new List<BigInteger>
        {
            BigInteger.ModPow(-rootOfUnity, 2 * Utils.Br7(0) + 1, 3329),
            0,
            1
        });

        for (var i = 1; i < 128; i++)
        {
            var zeta = BigInteger.ModPow(rootOfUnity, 2 * Utils.Br7(i) + 1, 3329);
            var p = new Polynomial(new List<BigInteger> { -zeta, 0, 1 });
            sum = rq.Mult(sum, p);
        }

        var expected = new Polynomial(new List<BigInteger> { 0 }); // equal to x^256 + 1 in the ring!
        Assert.True(TestHelpers.ComparePolynomials(expected, sum));
    }

    [Fact]
    public void EncodeAndDecodePolynomial_Gives_SamePolynomial()
    {
        var p = new Polynomial(new List<BigInteger> { 1, 1, 1 });
        var res = Utils.Decode(12, Utils.Encode(12, p));
        res.RemoveTrailingZeros();

        Assert.True(TestHelpers.ComparePolynomials(p, res));
    }

    [Fact]
    public void EncodeAndDecodePolynomial_Gives_SamePolynomial2()
    {
        var p = new Polynomial(new List<BigInteger> { 2, 2, 2 });
        var res = Utils.Decode(12, Utils.Encode(12, p));
        res.RemoveTrailingZeros();

        Assert.True(TestHelpers.ComparePolynomials(p, res));
    }

    [Fact]
    public void EncodeAndDecodePolynomial_Gives_SamePolynomial3()
    {
        var p = new Polynomial(new List<BigInteger> { 3, 3, 3 });
        var res = Utils.Decode(12, Utils.Encode(12, p));
        res.RemoveTrailingZeros();

        Assert.True(TestHelpers.ComparePolynomials(p, res));
    }

    [Fact]
    public void Debugging()
    {
        var kyber = new Kyber(new ParameterGen().Kyber512(), new PolynomialRing(3329, 256));

        var (pk, sk) = kyber.Keygen();
        var (c, key) = kyber.Encapsulate(pk);
        var keyPrime = kyber.Decapsulate(c, sk);

        Assert.Equal(key, keyPrime);
    }

    [Fact]
    public void CryptographicRandomlySecureRandomness()
    {
        var kyber = new Kyber(
            new ParameterGen().Kyber512(),
            new PolynomialRing(3329, 256),
            new CryptoRandom());

        var (pk, sk) = kyber.Keygen();
        var (c, key) = kyber.Encapsulate(pk);
        var keyPrime = kyber.Decapsulate(c, sk);

        Assert.Equal(key, keyPrime);
    }
}