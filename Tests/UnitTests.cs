using System.Numerics;
using System.Text;
using KyberCrystals;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

public class PolynomiaTests
{
    [Fact]
    public void Parse()
    {
        // TODO: Create some better tests than those for the Parse function
        var p = new Constants().Kyber512();
        var rq = new PolynomialRing(new BigInteger(3329), new BigInteger(5));

    }

    [Fact]
    public void RunningCbd_TwiceOnSameInput_GivesSameOutput()
    {
        var p = new Constants().Kyber512();
        var bytes = Helpers.GetRandomBytes(64 * p.Eta1);

        var rq = new PolynomialRing(p.Q, p.N);
        var res1 = rq.Cbd(bytes, p.Eta1);
        var res2 = rq.Cbd(bytes, p.Eta1);

        Assert.True(Helpers.ComparePolynomials(res1, res2));
    }

    [Fact]
    public void RunningDecode_TwiceOnSameInput_GivesSameOutput()
    {
        var l = 8;
        var p = new Constants().Kyber512();
        var bytes = Helpers.GetRandomBytes(32 * l);

        var rq = new PolynomialRing(p.Q, p.N);
        var res1 = rq.Decode(bytes, l);
        var res2 = rq.Decode(bytes, l);
        
        Assert.True(Helpers.ComparePolynomials(res1, res2));
    }
}

public class UtilityTesting 
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UtilityTesting(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void HashFunction_G_returns_CorrectAmountOfBytes()
    {
        var (x, y) = Utils.G(Helpers.GetRandomBytes(32));
        
        Assert.Equal(32, x.Length);
        Assert.Equal(32, y.Length);
    }
    
    [Fact]
    public void HashFunction_H_returns_CorrectAmountOfBytes()
    {
        var x = Utils.H(Helpers.GetRandomBytes(32));
        
        Assert.Equal(32, x.Length);
    }

    [Fact]
    public void sss() // TODO: better naming.
    {
        var bytes = Helpers.GetRandomBytes(32);
        var b = Helpers.GetRandomByte();

        var x = Utils.Prf(bytes, b, 100);

        Assert.Equal(100, x.Length);
    }

    [Fact]
    public void PrfUtility_ThrowsArgumentException_IfByteArrayIsBelowLength32()
    {
        var bytes = Helpers.GetRandomBytes(31);
        var b = Helpers.GetRandomByte();

        Assert.Throws<ArgumentException>(() => Utils.Prf(bytes, b, 100));
    }

    [Fact]
    public void PrfUtility_ThrowsArgumentException_IfByteArrayIsAboveLength32()
    {
        var bytes = Helpers.GetRandomBytes(33);
        var b = Helpers.GetRandomByte();

        Assert.Throws<ArgumentException>(() => Utils.Prf(bytes, b, 100));
    }
    
    [Fact]
    public void XofUtility_ThrowsArgumentException_IfByteArrayIsBelowLength32()
    {
        var bytes = Helpers.GetRandomBytes(31);
        var b = Helpers.GetRandomByte();
        var b2 = Helpers.GetRandomByte();

        Assert.Throws<ArgumentException>(() => Utils.Xof(bytes, b, b2, 100));
    }

    [Fact]
    public void XofUtility_ThrowsArgumentException_IfByteArrayIsAboveLength32()
    {
        var bytes = Helpers.GetRandomBytes(33);
        var b = Helpers.GetRandomByte();
        var b2 = Helpers.GetRandomByte();

        Assert.Throws<ArgumentException>(() => Utils.Xof(bytes, b, b2, 100));
    }
    
    [Fact]
    public void Ss1() // TODO: better naming.
    {
        var bytes = Helpers.GetRandomBytes(32);
        var b = Helpers.GetRandomByte();
        var b2 = Helpers.GetRandomByte();

        var x = Utils.Xof(bytes, b, b2, 100);

        Assert.Equal(100, x.Length);
    }

    [Fact]
    public void ShakeDigest_AgreeWith_OnlineService()
    {
        var hashAlgorihtm = new Org.BouncyCastle.Crypto.Digests.ShakeDigest(256);
        var bytes = Encoding.ASCII.GetBytes("hearsay");
        hashAlgorihtm.BlockUpdate(bytes, 0, bytes.Length);

        var result = new byte[100];
        hashAlgorihtm.DoFinal(result, 0);
        
        var res = BitConverter.ToString(result).Replace("-","");
        
        // found on https://emn178.github.io/online-tools/shake_256.html
        var expected = "58D33210B506D65D49BAAD0E4F05BE8E593DCB2860F1673BF3D90822771FA";
        Assert.Contains(expected, res);
    }
}

public static class Helpers
{
    public static byte[] GetRandomBytes(int length)
    {
        var random = new Random();

        var bytes = new byte[length];
        random.NextBytes(bytes);

        return bytes;
    }

    public static byte GetRandomByte()
    {
        return GetRandomBytes(1).First();
    }

    public static bool ComparePolynomials(Polynomial p1, Polynomial p2)
    {
        if (p1.GetDegree() != p2.GetDegree())
            return false;
        
        for (var i = 0; i < p1.GetDegree(); i++)
        {
            if (p1.GetCoefficient(i) != p2.GetCoefficient(i))
                return false;
        }

        return true;
    }
}