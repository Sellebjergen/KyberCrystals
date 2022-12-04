using System.Numerics;
using System.Text;
using KyberCrystals;
using Xunit;

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
    public void PRF_UsingRandomBytes_ReturnsCorrectLength()
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
    public void XOF_UsingRandomBytes_GivesCorrectLength()
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

    [Fact]
    public void RootOfUnity_Returns17_ForKyberRing()
    {
        var res = Utils.GetRootOfUnity(256, 3329);
        Assert.Equal(17, res); // 17 from article
    }

    [Fact]
    public void RootOfUnity_Returns4_ForBabyKyberRing()
    {
        var res = Utils.GetRootOfUnity(4, 17);
        Assert.Equal(4, res); // 4 from blog post calculation
    }

    [Fact]
    public void Br7_reversesBits_GivesExpectedResult()
    {
        Assert.Equal(8, Utils.Br7(8));
    }

    [Fact]
    public void Br7_reversed1_Gives2_32_minus_1()
    {
        // reversing 0000001 gives 1000000 2**7 = 64.
        Assert.Equal(64, Utils.Br7(1));
    }
    
    [Fact]
    public void GetBytes_ReturnsCorrect_0Byte()
    {
        var a = "00000000";
        var b = Utils.GetBytes(a);
        
        Assert.Equal(new byte[] { 0 }, b);
    }
    
    [Fact]
    public void toByte_fromByte_one()
    {
        var a = "00000001";
        var b = Utils.GetBytes(a);
        
        Assert.Equal(a, Utils.BytesToBinaryString(b));
    }
    
    [Fact]
    public void toByte_fromByte_max()
    {
        var a = "10000000";
        var b = Utils.GetBytes(a);
        
        Assert.Equal(a, Utils.BytesToBinaryString(b));
    }
    
    [Fact]
    public void GetBytes_ReturnsCorrect_0ByteWithLength32()
    {
        var a = "00000000";
        var a32 = a + a + a + a;
        var b = Utils.GetBytes(a32);
        
        Assert.Equal(new byte[] { 0,0,0,0 }, b);
    }
    
    [Fact]
    public void Compress_Decompress_GivesNumberClose()
    {
        var d = (short) 3;
        var x = Utils.Compress(2, d);
        var xB = Utils.Decompress((short) x, d);
        
        Assert.True(xB - x % 3329 < 3329 / (BigInteger) Math.Pow(2, d));
    }
    
    [Fact]
    public void Compress_Returns_ExpectedDownRoundedValue()
    {
        // This will make compress return 2^d / 3329 * x = 14.41, BigInteger will round to 14
        var d = (short) 4;
        var x = (short) 3000;
        var y = Utils.Compress(x, d);
        
        Assert.Equal(14, y);
    }
    
    [Fact]
    public void Compress_Returns_ExpectedUproundedValue()
    {
        // This will make compress return 2^d / 3329 * x = 14.41, BigInteger will round to 14
        var d = (short) 4;
        var x = (short) 3100;
        var y = Utils.Compress(x, d);
        
        Assert.Equal(15, y);
    }
    
    [Fact]
    public void Encode_Decode_GivesOriginalResult()
    {
        var pol = new Polynomial(new List<BigInteger> { 0, 0, 1 });
        var x = Utils.Encode(12, pol);
        var y = Utils.Decode(12, x);
        y.RemoveTrailingZeros();

        Assert.True(TestHelpers.ComparePolynomials(pol, y));
    }
    
    [Fact]
    public void Encode_Decode_GivesOriginalResult_2()
    {
        var pol = new Polynomial(new List<BigInteger> { 0, 0, 1 });
        var x = Utils.Encode(1, pol);
        var y = Utils.Decode(1, x);
        y.RemoveTrailingZeros();

        Assert.True(TestHelpers.ComparePolynomials(pol, y));
    }

    [Fact]
    public void Encode_Decode_GivesOriginalResult_3()
    {
        var pol = new Polynomial(new List<BigInteger> { 0, 0, 1 });
        var x = Utils.Encode(4, pol);
        var y = Utils.Decode(4, x);
        y.RemoveTrailingZeros();

        Assert.True(TestHelpers.ComparePolynomials(pol, y));
    }
    
    [Fact]
    public void Encode_Decode_GivesOriginalResult_4()
    {
        var pol = new Polynomial(new List<BigInteger> { 0, 0, 1 });
        var x = Utils.Encode(10, pol);
        var y = Utils.Decode(10, x);
        y.RemoveTrailingZeros();

        Assert.True(TestHelpers.ComparePolynomials(pol, y));
    }
    
    [Fact]
    public void Encode_Decode_MultiplePolynomials_ReturnExpected()
    {
        var u = new[]
        {
            new Polynomial(new List<BigInteger>{ 0, 0, 1}), 
            new Polynomial(new List<BigInteger>{0, 0, 2})
        };

        var uCompress = Utils.Compress(u, 10);
        var uDecompress = Utils.Decompress(uCompress, 10);

        Assert.True(TestHelpers.ComparePolynomialArrays(u, uDecompress));
    }
    
    [Fact]
    public void Encode_Decode_ForMultiplePolynomials_ReturnOriginal()
    {
        var u = new Polynomial[]
        {
            new Polynomial(new List<BigInteger>{ 0, 0, 1}), 
            new Polynomial(new List<BigInteger>{0, 0, 2})
        };
        u[0] = new Polynomial(u[0].GetPaddedCoefficients(256)); // just making sure that the length is equal
        u[1] = new Polynomial(u[0].GetPaddedCoefficients(256)); // for later assertion.
        
        var uEncoded = Utils.Encode(10, u);
        var uDecoded = Utils.Decode(10, uEncoded);

        Assert.True(TestHelpers.ComparePolynomialArrays(u, uDecoded));
    }
    
    [Fact]
    public void Debugging_Compress_Decompress()
    {
        var p = new Polynomial(new List<BigInteger> { 0, 0, 3000 });
        var pComp = Utils.Compress(p, 10);
        var pDeco = Utils.Decompress(pComp, 10);

        Assert.True(TestHelpers.ComparePolynomials(p, pDeco));
    }
    
    [Fact]
    public void Debugging_EncodeDecode_With_CompressDecompress_OnPolynomialArray_CanBeRecreated()
    {
        short du = 10;
        var u = new[]
        {
            new Polynomial(new List<BigInteger>{0, 0, 3000}), 
            new Polynomial(new List<BigInteger>{0, 0, 3000})
        };

        var uCompressed = Utils.Compress(u, du);
        var uEncoded = Utils.Encode(du, uCompressed);

        var uDecoded = Utils.Decode(du, uEncoded);
        var uDecompressed = Utils.Decompress(uDecoded, du);
        
        foreach (var (p, p2) in u.Zip(uDecompressed))
        {
            for (var i = 0; i < p.GetCoefficients().Count; i++)
            {
                var isClose = TestHelpers.ElementsAreClose((short) p.GetCoefficient(i), (short) p2.GetCoefficient(i), du, 3329);
                Assert.True(isClose);
            }
        }
    }
    
    [Fact]
    public void GetBytes_ReturnExpectedBytes()
    {
        var bitString = "000010111110000010111110";
        var bytes = Utils.GetBytes(bitString);
        
        Assert.Equal(Convert.FromHexString("d0077d"), bytes);
    }
}