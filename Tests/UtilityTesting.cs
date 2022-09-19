using System.Text;
using KyberCrystals;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

public class UtilityTesting 
{
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
        var outLengthOfHash = 256;
        var hashAlgorihtm = new Org.BouncyCastle.Crypto.Digests.ShakeDigest(256);
        var bytes = Encoding.ASCII.GetBytes("hearsay");
        hashAlgorihtm.BlockUpdate(bytes, 0, bytes.Length);

        var result = new byte[outLengthOfHash];
        hashAlgorihtm.DoFinal(result, 0, outLengthOfHash);
        
        var res = BitConverter.ToString(result).Replace("-","");
        
        // found on https://emn178.github.io/online-tools/shake_256.html
        var expected = "58d33210b506d65d49baad0e4f05be8e593dcb2860f1673bf3d90822771fabd0".ToUpper();
        Assert.Contains(expected, res);
    }
}