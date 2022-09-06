using System.Collections;
using System.Numerics;

namespace KyberCrystals;

public class Kyber
{
    public Kyber(Params p)
    {
        // TODO: do something more interesting.
    }
}

public class PolynomialRing
{
    private readonly BigInteger _q;
    private readonly BigInteger _n;
    
    public PolynomialRing(BigInteger q, BigInteger n)
    {
        _q = q;
        _n = n;
    }

    public Polynomial Parse(byte[] bytes)
    {
        var i = 0;
        var j = 0;
        var coefficients = new List<int>();
        
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
        var coefficients = new List<int>();
        
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
}

public class Polynomial
{
    private readonly List<int> _coefficients;
    
    public Polynomial(List<int> coefficients)
    {
        _coefficients = coefficients;
    }

    public int GetCoefficient(int i)
    {
        return _coefficients[i];
    }

    public int GetDegree()
    {
        return _coefficients.Count;
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