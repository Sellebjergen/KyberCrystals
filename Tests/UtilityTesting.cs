using System.Text;
using KyberCrystals;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

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
        var (x, y) = Utils.G(TestHelpers.GetRandomBytes(32));
        
        Assert.Equal(32, x.Length);
        Assert.Equal(32, y.Length);
    }
    
    [Fact]
    public void HashFunction_H_returns_CorrectAmountOfBytes()
    {
        var x = Utils.H(TestHelpers.GetRandomBytes(32));
        
        Assert.Equal(32, x.Length);
    }

    [Fact]
    public void sss() // TODO: better naming.
    {
        var bytes = TestHelpers.GetRandomBytes(32);
        var b = TestHelpers.GetRandomByte();

        var x = Utils.Prf(bytes, b, 100);

        Assert.Equal(100, x.Length);
    }

    [Fact]
    public void PrfUtility_ThrowsArgumentException_IfByteArrayIsBelowLength32()
    {
        var bytes = TestHelpers.GetRandomBytes(31);
        var b = TestHelpers.GetRandomByte();

        Assert.Throws<ArgumentException>(() => Utils.Prf(bytes, b, 100));
    }

    [Fact]
    public void PrfUtility_ThrowsArgumentException_IfByteArrayIsAboveLength32()
    {
        var bytes = TestHelpers.GetRandomBytes(33);
        var b = TestHelpers.GetRandomByte();

        Assert.Throws<ArgumentException>(() => Utils.Prf(bytes, b, 100));
    }
    
    [Fact]
    public void XofUtility_ThrowsArgumentException_IfByteArrayIsBelowLength32()
    {
        var bytes = TestHelpers.GetRandomBytes(31);
        var b = TestHelpers.GetRandomByte();
        var b2 = TestHelpers.GetRandomByte();

        Assert.Throws<ArgumentException>(() => Utils.Xof(bytes, b, b2, 100));
    }

    [Fact]
    public void XofUtility_ThrowsArgumentException_IfByteArrayIsAboveLength32()
    {
        var bytes = TestHelpers.GetRandomBytes(33);
        var b = TestHelpers.GetRandomByte();
        var b2 = TestHelpers.GetRandomByte();

        Assert.Throws<ArgumentException>(() => Utils.Xof(bytes, b, b2, 100));
    }
    
    [Fact]
    public void Ss1() // TODO: better naming.
    {
        var bytes = TestHelpers.GetRandomBytes(32);
        var b = TestHelpers.GetRandomByte();
        var b2 = TestHelpers.GetRandomByte();

        var x = Utils.Xof(bytes, b, b2, 100);

        Assert.Equal(100, x.Length);
    }

    [Fact]
    public void ShakeDigest_AgreeWith_OnlineService()
    {
        var outLengthOfHash = 100;
        var hashAlgorihtm = new Org.BouncyCastle.Crypto.Digests.ShakeDigest(256);
        var bytes = Encoding.ASCII.GetBytes("hearsay");
        hashAlgorihtm.BlockUpdate(bytes, 0, bytes.Length);

        var result = new byte[outLengthOfHash];
        hashAlgorihtm.DoFinal(result, 0, outLengthOfHash);
        
        var res = BitConverter.ToString(result).Replace("-","");
        
        // found on https://emn178.github.io/online-tools/shake_256.html
        var expected = "58D33210B506D65D49BAAD0E4F05BE8E593DCB2860F1673BF3D90822771FA";
        Assert.Contains(expected, res);
    }
}