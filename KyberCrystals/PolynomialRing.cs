using System.Collections;
using System.Numerics;

namespace KyberCrystals;

public class PolynomialRing
{
    public BigInteger Q { get; }
    public BigInteger N { get; }
    private readonly Polynomial _modPoly;
    private readonly IPolyModStrategy _polyModStrategy = new LongPolynomialDivision();

    public PolynomialRing(BigInteger q, BigInteger n)
    {
        Q = q;
        N = n;
        var coef = new List<BigInteger> { 1 };

        for (var i = 0; i < n - 1; i++)
        {
            coef.Add(BigInteger.Zero);
        }

        coef.Add(BigInteger.One);
        _modPoly = new Polynomial(coef);
    }

    public Polynomial Parse(byte[] bytes)
    {
        var i = 0;
        var j = 0;
        var coefficients = new BigInteger[256];

        while (j < N)
        {
            var d1 = bytes[i] + 256 * (bytes[i + 1] % 16);
            var d2 = bytes[i + 1] / 16 + 16 * bytes[i + 2];

            if (d1 < Q)
            {
                coefficients[j] = d1;
                j += 1;
            }

            if (d2 < Q && j < N)
                // todo: is the else above right? Without we can have polynomials of deg 256 not in rq.
            {
                coefficients[j] = d2;
                j += 1;
            }

            i += 3;
        }

        return new Polynomial(new List<BigInteger>(coefficients));
    }

    public Polynomial Cbd(byte[] bytes, int eta)
    {
        if (bytes.Length != 64 * eta)
            throw new ArgumentException($"The input bytes need to be of length {64 * eta} but it was {bytes.Length}");
        
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

        return ReduceModuloQ(new Polynomial(coefficients));
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

            coefficients.Add((int)temp);
        }

        return new Polynomial(coefficients);
    }

    public Polynomial Add(Polynomial p1, Polynomial p2)
    {
        var maxDeg = Math.Max(p1.GetLengthOfPolynomial(), p2.GetLengthOfPolynomial());
        var p1Coef = p1.GetPaddedCoefficients(maxDeg);
        var p2Coef = p2.GetPaddedCoefficients(maxDeg);

        var result = new List<BigInteger> { };
        foreach (var (x, y) in p1Coef.Zip(p2Coef))
        {
            result.Add(x + y);
        }

        var res = ReduceModuloQ(new Polynomial(result));
        return res;
    }

    public Polynomial Mult(Polynomial p1, Polynomial p2)
    {
        var res = new List<BigInteger>();
        for (var i = 0; i < Math.Pow(Math.Max(p1.GetLengthOfPolynomial(), p2.GetLengthOfPolynomial()), 2); i++)
        {
            res.Add(BigInteger.Zero);
        }

        for (var i = 0; i < p1.GetLengthOfPolynomial(); i++)
        {
            for (var j = 0; j < p2.GetLengthOfPolynomial(); j++)
            {
                res[i + j] += p1.GetCoefficient(i) * p2.GetCoefficient(j);
            }
        }

        var resPoly = ReduceModuloQ(new Polynomial(res));
        resPoly.RemoveTrailingZeros();
        return ModPoly(resPoly, _modPoly);
    }

    public Polynomial Sub(Polynomial p1, Polynomial p2)
    {
        var maxDeg = Math.Max(p1.GetLengthOfPolynomial(), p2.GetLengthOfPolynomial());
        var p1Coef = p1.GetPaddedCoefficients(maxDeg);
        var p2Coef = p2.GetPaddedCoefficients(maxDeg);

        var result = new List<BigInteger> { };
        foreach (var (x, y) in p1Coef.Zip(p2Coef))
        {
            result.Add(x - y);
        }

        var res = ReduceModuloQ(new Polynomial(result));
        res.RemoveTrailingZeros();
        return res;
    }

    public Polynomial ConstMult(Polynomial p1, BigInteger c)
    {
        var res = new List<BigInteger>();
        foreach (var pc in p1.GetCoefficients())
        {
            res.Add(pc * c);
        }

        return ReduceModuloQ(new Polynomial(res));
    }

    public Polynomial ModPoly(Polynomial p, Polynomial mod)
    {
        return _polyModStrategy.PolyMod(this, p, mod);
    }

    public Polynomial ReduceModuloQ(Polynomial p)
    {
        var res = new List<BigInteger>();

        foreach (var c in p.GetCoefficients())
        {
            var temp = BigInteger.ModPow(c, 1, Q);
            if (temp >= 0)
                res.Add(temp);
            else
                res.Add(temp + Q);
        }

        return new Polynomial(res);
    }
}