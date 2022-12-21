using KyberCrystals;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

public class SpeedTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SpeedTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Kyber512()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (var i = 0; i < 1000; i++)
        {
            var param = new ParameterGen().Kyber512();
            var kyber = new Kyber(param, new PolynomialRing(param.Q, param.N));
            var (pk, sk) = kyber.Keygen();
            var (c, kEnc) = kyber.Encapsulate(pk);
            var kDec = kyber.Decapsulate(c, sk);

            Assert.Equal(kEnc, kDec);
        }

        watch.Stop();
        _testOutputHelper.WriteLine(watch.ElapsedMilliseconds.ToString());
    }
    
    [Fact]
    public void Kyber768()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (var i = 0; i < 1000; i++)
        {
            var param = new ParameterGen().Kyber768();
            var kyber = new Kyber(param, new PolynomialRing(param.Q, param.N));
            var (pk, sk) = kyber.Keygen();
            var (c, kEnc) = kyber.Encapsulate(pk);
            var kDec = kyber.Decapsulate(c, sk);

            Assert.Equal(kEnc, kDec);
        }

        watch.Stop();
        _testOutputHelper.WriteLine(watch.ElapsedMilliseconds.ToString());
    }
    
    [Fact]
    public void Kyber1024()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (var i = 0; i < 1000; i++)
        {
            var param = new ParameterGen().Kyber1024();
            var kyber = new Kyber(param, new PolynomialRing(param.Q, param.N));
            var (pk, sk) = kyber.Keygen();
            var (c, kEnc) = kyber.Encapsulate(pk);
            var kDec = kyber.Decapsulate(c, sk);

            Assert.Equal(kEnc, kDec);
        }

        watch.Stop();
        _testOutputHelper.WriteLine(watch.ElapsedMilliseconds.ToString());
    }
}