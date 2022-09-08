using System.Collections;
using System.Numerics;

namespace KyberCrystals;

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

    public Polynomial Add(Polynomial p1, Polynomial p2)
    {
        var p1Coef = p1.GetCoefficients();
        var p2Coef = p2.GetCoefficients();

        var result = new List<BigInteger> { };
        foreach (var (x, y) in p1Coef.Zip(p2Coef))
        {
            result.Add(x + y);
        }

        return new Polynomial(result);
    }
}