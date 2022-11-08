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

    public static byte[] Prf(byte[] bytes, byte b, int length = 0)
    {
        if (bytes.Length != 32)
            throw new ArgumentException("The byte array need to be of length 32");
        if (length < 0)
            throw new ArgumentException("The length must be a positive number!"); // use -1 for when no number is set

        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.ShakeDigest(256);

        hashAlgorithm.BlockUpdate(bytes, 0, bytes.Length);
        hashAlgorithm.Update(b);
        
        if (length == 0)
        {
            var result = new byte[hashAlgorithm.GetByteLength()];
            hashAlgorithm.DoFinal(result, 0);
            return result;
        }
        else {
            var result = new byte[length];
            hashAlgorithm.DoFinal(result, 0, length);
            return result;
        }
    }

    public static byte[] Xof(byte[] bytes, byte b1, byte b2, int length = 0)
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

    public static byte[] Kdf(byte[] bytes, int length = 0)
    {
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.ShakeDigest(256);
        hashAlgorithm.BlockUpdate(bytes, 0, bytes.Length);

        byte[] result;
        if (length == 0)
        {
            result = new byte[hashAlgorithm.GetByteLength()];
            hashAlgorithm.DoFinal(result, 0);
        }
        else
        {
            result = new byte[length];
            hashAlgorithm.DoFinal(result, 0, length);
        }

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
            var bitLLength= Convert.ToString((short) coef[i], 2).PadLeft(l, '0').ToCharArray();
            Array.Reverse(bitLLength);
            res += new string(bitLLength);
        }
        
        return res;
    }
    
    public static string[] Encode(int l, Polynomial[] polys)
    {
        var res = new string[polys.Length];
        for (var i = 0; i < polys.Length; i++)
        {
            res[i] = Encode(l, polys[i]);
        }
        
        return res;
    }

    public static Polynomial Decode(int l, string bits)
    {
        var coef = new BigInteger[256];
        
        for (var i = 0; i < bits.Length / l; i++)
        {
            var tmp = bits.Substring(i * l, l).ToCharArray();
            Array.Reverse(tmp);
            var c = Convert.ToInt16(new string(tmp), 2);
            coef[i] = c;
        }

        return new Polynomial(new List<BigInteger>(coef));
    }
    
    public static Polynomial[] Decode(int l, string[] bytesArr)
    {
        var res = new Polynomial[bytesArr.Length];
        
        for (var i = 0; i < bytesArr.Length; i++)
        {
            var bytes = bytesArr[i];
            res[i] = Decode(l, bytes);
        }
        
        return res;
    }
    
    public static string EncodePolynomialList(int l, Polynomial[] polys)
    {
        var res = new string[polys.Length];
        
        for (var i = 0; i < polys.Length; i++)
        {
            res[i] = Encode(l, polys[i]);
        }

        return String.Join("", res);
    }

    public static byte[] GetBytes(string bitString)
    {
        var bytes = new byte[bitString.Length / 8];
        for (var i = 0; i < bitString.Length / 8; i++)
        {
            var byteString = bitString.Substring(i * 8, 8).ToCharArray();
            Array.Reverse(byteString);
            bytes[i] = Convert.ToByte(new string(byteString), 2);
        }

        return bytes;
    }
    
    public static BigInteger Compress(short x, short d)
    {
        var compMod = Math.Pow(2, d) / 3329; // TODO: kyber params
        var res = Convert.ToInt16(compMod * x) % Math.Pow(2, d);
        return Convert.ToInt16(res);
    }
    
    public static Polynomial Compress(Polynomial p, short d)
    {
        var coef = p.GetCoefficients();
        for (var i = 0; i < p.GetCoefficients().Count; i++)
        {
            coef[i] = Compress((short) coef[i], d);
        }
        return new Polynomial(coef);
    }
    
    public static Polynomial[] Compress(Polynomial[] polys, short d)
    {
        var res = new Polynomial[polys.Length];
        for (var i = 0; i < polys.Length; i++)
        {
            res[i] = Compress(polys[i], d);
        }

        return res;
    }
    
    public static BigInteger Decompress(short x, short d)
    {
        var decomConstant = 3329 / Math.Pow(2, d);
        return Convert.ToInt16(decomConstant * x);
    }
    
    public static Polynomial Decompress(Polynomial p, short d)
    {
        var coef = p.GetCoefficients();
        for (var i = 0; i < p.GetCoefficients().Count; i++)
        {
            coef[i] = Decompress((short) coef[i], d);
        }

        return new Polynomial(coef);
    }
    
    public static Polynomial[] Decompress(Polynomial[] polys, short d)
    {
        var res = new Polynomial[polys.Length];
        for (var i = 0; i < polys.Length; i++)
        {
            res[i] = Decompress(polys[i], d);
        }

        return res;
    }

    public static string BytesToString(byte[] p0)
    {
        var bits = new BitArray(p0);
        var res = "";
        for (var i = 0; i < bits.Length; i++)
        {
            if (bits[i]) 
                res += "1";
            else 
                res += "0";
        }

        return res;
    }
}