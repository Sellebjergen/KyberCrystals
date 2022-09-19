using System.Numerics;

namespace KyberCrystals;

public static class Utils 
{
    public static byte[] GetRandomBytes(int amount)
    {
        var random = new Random();
        
        var bytes = new byte[amount];
        random.NextBytes(bytes);

        return bytes;
    }

    public static (byte[], byte[]) G(byte[] input)
    {
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);
        
        hashAlgorithm.BlockUpdate(input, 0, input.Length);

        var result = new byte[64];
        hashAlgorithm.DoFinal(result, 0);

        var res1 = new byte[32];
        var res2 = new byte[32];
        Array.Copy(result, 0, res1, 0, 32);
        Array.Copy(result, 32, res2, 0, 32);

        return (res1, res2);
    }

    public static byte[] H(byte[] input)
    {
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(256);
        
        hashAlgorithm.BlockUpdate(input, 0, input.Length);

        var result = new byte[32];
        hashAlgorithm.DoFinal(result, 0);

        return result;
    }

    public static byte[] Prf(byte[] bytes, byte b, int length)
    {
        if (bytes.Length != 32) 
            throw new ArgumentException("The byte array need to be of length 32");

        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.ShakeDigest(256);
        
        hashAlgorithm.BlockUpdate(bytes, 0, bytes.Length);
        hashAlgorithm.Update(b);
        
        var result = new byte[length];
        hashAlgorithm.DoFinal(result, 0, length);
        return result;
    }

    public static byte[] Xof(byte[] bytes, byte b1, byte b2, int length)
    {
        if (bytes.Length != 32)
            throw new ArgumentException("The byte array need to be of length 32");
        
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.ShakeDigest(128);
        hashAlgorithm.BlockUpdate(bytes, 0, bytes.Length);
        hashAlgorithm.Update(b1);
        hashAlgorithm.Update(b2);
        
        var result = new byte[length];
        hashAlgorithm.DoFinal(result, 0, length);
        
        return result;
    }

    public static byte[] Kdf(byte[] bytes, int length)
    {
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.ShakeDigest(256);
        hashAlgorithm.BlockUpdate(bytes, 0, bytes.Length);

        var result = new byte[length];
        hashAlgorithm.DoFinal(result, 0, length);
        
        return result;
    }
}