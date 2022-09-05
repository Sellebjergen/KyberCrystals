using System.Numerics;

namespace KyberCrystals;

public class Kyber
{
    public Kyber(Params p)
    {
        // TOOD: do something more interesting.
    }
}

public class PolynomialRing
{
    private BigInteger _q;
    private BigInteger _n;
    
    public PolynomialRing(BigInteger q, BigInteger n)
    {
        _q = q;
        _n = n;
    }

    public Polynomial Parse(byte[] bytes)
    {
        var i = 0;
        var j = 0;
        var coefficients = Array.Empty<int>();
        
        while (j < _n)
        {
            var d1 = bytes[i] + 256 * (bytes[i + 1] % 16);
            var d2 = (bytes[i + 1] / 16) + 16 * bytes[i + 2]; // should automatically use floor division as we work on ints.

            if (d1 < _q)
            {
                coefficients[j] = d1;
                j += 1;
            }

            if (d2 < _q && j < _q)
            {
                coefficients[j] = d2;
                j += 1;
            }

            i += 3;
        }

        return new Polynomial(coefficients);
    }
}

public class Polynomial
{
    private int[] _coefficients;
    
    public Polynomial(int[] coefficients)
    {
        _coefficients = coefficients;
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
        // TODO
        return null;
    }
    
    public Params Kyber1024()
    {
        // TODO
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