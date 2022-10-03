using System.Collections;
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

    public static int GetRootOfUnity(int n, int q) // n is degree of mod poly and q is the modulus of the ring.
    {
        var i = 2;
        while (true)
        {
            var math = BigInteger.ModPow(i, n, q);
            if (math == 1)
            {
                return i;
            }

            i += 1;
        }
    }

    public static int Br7(int num)
    {
        if (num is > 127 or < 0)
            throw new ArgumentException("The number cannot fit in 7 bits. Choose a number x, 0 <= x <= 127");

        var bits = Convert.ToString(num, 2).PadLeft(7, '0').ToCharArray();
        Array.Reverse(bits);
        var reversed = Convert.ToInt32(new string(bits), 2);

        return reversed;
    }

    public static string Encode(int l, Polynomial p)
    {
        var coef = p.GetPaddedCoefficients(256).ToArray();
        var res = "";
        
        for (var i = 0; i < coef.Length; i++)
        {
            var bit12Length =
                Convert.ToString((short) coef[i], 2).PadLeft(l, '0');
            
            res += bit12Length;
        }
        
        return res;
    }

    public static Polynomial Decode(int l, string bytes)
    {
        var bits = bytes;
        var coef = new BigInteger[256];
        
        for (var i = 0; i < 256; i++)
        {
            var sum = 0.0;
            for (var j = l-1; j >= 0; j--)
            {
                var bit = bits[i * l + j] == '1' ? 1 : 0;
                sum += bit * Math.Pow(2, 12-1-j); // todo: could be implemented using shift? 
            }
            coef[i] = (BigInteger) sum;
        }

        return new Polynomial(new List<BigInteger>(coef));
    }
    
    public static string EncodePolynomialList(int l, List<Polynomial> polys)
    {
        var res = new string[polys.Count];
        
        for (var i = 0; i < polys.Count; i++)
        {
            res[i] = Encode(l, polys[i]);
        }

        return String.Join("", res);
    }
}