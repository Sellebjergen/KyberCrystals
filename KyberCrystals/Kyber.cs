using System.Collections;
using System.Numerics;

namespace KyberCrystals;

public class Kyber
{
    private Params _params;
    private PolynomialRing _rq;
    
    public Kyber(Params p, PolynomialRing rq)
    {
        _params = p;
        _rq = rq; // TODO: Maybe this could be part of the params?
    }

    public void CPAPKE_KeyGen()
    {
        var d = Utils.GetRandomBytes(32);
        var (rho, sigma) = Utils.G(d);

        var k = 8;
        var N = 0;
        var A = new Polynomial[][] { };
        
        // Generate the A matrix
        for (var i = 0; i < k - 1; i++)
        { 
            for (var j = 0; j < k - 1; j++)
            {
                var jByte = BitConverter.GetBytes(j).First();
                var iByte = BitConverter.GetBytes(i).First();
                
                // TODO: could make a function to convert int to byte. This could throw an error if int > 255 used.
                A[i][j] = _rq.Parse(Utils.Xof(rho, jByte, iByte, (int)(3 * _rq._n)));
            }
        }
        
        // Sample s
        var s = new Polynomial[] { };
        for (var i = 0; i < k; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(N).First(), 64 * _params.Eta1);
            s[i] = _rq.Cbd(inputBytes, _params.Eta1);
            N += 1;
        }

        // Sample e
        var e = new Polynomial[] { };
        for (var i = 0; i < k; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(N).First(), 64 * _params.Eta1);
            e[i] = _rq.Cbd(inputBytes, _params.Eta1);
            N += 1;
        }
        
        // Convert s and e to NTT form.
        if (_rq._n != 256) 
            throw new NotImplementedException("Kyber only specifies for n = 256");

        foreach (var p in s)
            p.ReduceToNttForm();

        foreach (var p in e)
            p.ReduceToNttForm();

        // TODO: implement the rest of the key generation.
    }
}

public class PolynomialRing
{
    private BigInteger _q { get;  } // TODO: Is this the c# way of doing things?
    public BigInteger _n { get; }
    
    public PolynomialRing(BigInteger q, BigInteger n)
    {
        _q = q;
        _n = n;
    }

    public Polynomial Parse(byte[] bytes)
    {
        var i = 0;
        var j = 0;
        var coefficients = new List<BigInteger>();
        
        while (j < _n)
        {
            var d1 = bytes[i] + 256 * (bytes[i + 1] % 16);
            var d2 = (bytes[i + 1] / 16) + 16 * bytes[i + 2]; // should automatically use floor division as we work on ints.

            if (d1 < _q)
            {
                coefficients.Add(d1);
                j += 1;
            }

            if (d2 < _q && j < _q)
            {
                coefficients.Add(d2);
                j += 1;
            }

            i += 3;
        }

        return new Polynomial(coefficients);
    }

    public Polynomial Cbd(byte[] bytes, int eta)
    {
        var bits = new BitArray(bytes);
        var coefficients = new List<BigInteger>();
        
        for (var i = 0; i < 256; i++)
        {
            var a = 0;
            for (var j = 0; j < eta; j++)
            {
                switch (bits[2 * i * eta + j])
                {
                    case true:
                        a += 1;
                        break;
                    case false:
                        continue;
                }
            }
            
            var b = 0;
            for (var j = 0; j < eta; j++)
            {
                switch (bits[2 * i * eta + eta + j])
                {
                    case true:
                        b += 1;
                        break;
                    case false:
                        continue;
                }
            }

            coefficients.Add(a - b);
        }

        return new Polynomial(coefficients);
    }

    // Note that l is the amount of 32 bytes we need to reach n.
    // l = 8   =>   l * 32 = 256, which is used in the algorithm from the spec.
    public Polynomial Decode(byte[] bytes, int l)
    {
        var bits = new BitArray(bytes);
        var coefficients = new List<BigInteger>();
        
        for (var i = 0; i < 256; i++)
        {
            var temp = 0.0;
            for (var j = 0; j < l - 1; j++)
            {
                var a = bits[i * l + j];
                switch (a)
                {
                    case true:
                        temp += 1 * Math.Pow(2, j);
                        break;
                    default:
                        continue;
                }
            }
            coefficients.Add((int) temp);
        }

        return new Polynomial(coefficients);
    }
}

public class Polynomial
{
    private readonly List<BigInteger> _coefficients;
    
    public Polynomial(List<BigInteger> coefficients)
    {
        _coefficients = coefficients;
    }

    public BigInteger GetCoefficient(int i)
    {
        return _coefficients[i];
    }

    public int GetDegree()
    {
        return _coefficients.Count;
    }
    
    public void ReduceToNttForm()
    {
        var k = 1;
        var l = 128;
        while (l >= 2)
        {
            var start = 0;
            while (start < 256)
            {
                var zeta = Constants.NttZetas[k];
                k += 1;
                var counter = 0;
                for (var j = start; j < start + l; j += 1)
                {
                    var t = Utils.NttMult(zeta, _coefficients[j + l]);
                    _coefficients[j + l] = _coefficients[j] - t;
                    _coefficients[j] += t;
                }

                start = l + counter + 1; // The counter right here should equal 127.
            }

            l >>= 1;
        }
    }
}

public class Constants
{
    // Defined in the Kyber specification paper.
    public Params Kyber512()
    {
        return new Params
        {
            N = 512,
            K = 2,
            Q = 3329,
            Eta1 = 3,
            Eta2A = 2,
            Du = 10,
            Dv = 4
        };
    }

    public Params Kyber768()
    {
        // TODO: just update from the article
        return null;
    }
    
    public Params Kyber1024()
    {
        // TODO: just update from the article
        return null;
    }

    public static readonly int[] NttZetas = 
            {2285, 2571, 2970, 1812, 1493, 1422, 287, 202, 3158, 622, 1577, 182, 962, 2127, 1855, 1468,
            573, 2004, 264, 383, 2500, 1458, 1727, 3199, 2648, 1017, 732, 608, 1787, 411, 3124, 1758,
            1223, 652, 2777, 1015, 2036, 1491, 3047, 1785, 516, 3321, 3009, 2663, 1711, 2167, 126, 1469,
            2476, 3239, 3058, 830, 107, 1908, 3082, 2378, 2931, 961, 1821, 2604, 448, 2264, 677, 2054,
            2226, 430, 555, 843, 2078, 871, 1550, 105, 422, 587, 177, 3094, 3038, 2869, 1574, 1653, 3083,
            778, 1159, 3182, 2552, 1483, 2727, 1119, 1739, 644, 2457, 349, 418, 329, 3173, 3254, 817,
            1097, 603, 610, 1322, 2044, 1864, 384, 2114, 3193, 1218, 1994, 2455, 220, 2142, 1670, 2144,
            1799, 2051, 794, 1819, 2475, 2459, 478, 3221, 3021, 996, 991, 958, 1869, 1522, 1628};
}

public class Params
{
    public int N { get; set; }
    public int K { get; set; }
    public int Q { get; set; }
    public int Eta1 { get; set; }
    public int Eta2A { get; set; }
    public int Du { get; set; }
    public int Dv { get; set; }
}

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
        var result = new byte[length];
        
        hashAlgorithm.BlockUpdate(bytes, 0, bytes.Length);
        hashAlgorithm.Update(b);
        
        hashAlgorithm.DoFinal(result, 0);
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
        hashAlgorithm.DoFinal(result, 0);
        
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

    public static BigInteger montgomery_reduce(BigInteger a)
    {
        // TODO: Constants taken from the article, should be replaced by something more appropriate.
        var math = BigInteger.Pow(2, 16); // TODO: this seems inefficient, to save 16 bit number in bigint.
        return BigInteger.ModPow(a * math, -1, 3329);
    }

    public static BigInteger NttMult(BigInteger a, BigInteger b)
    {
        return montgomery_reduce(a * b);
    }
}