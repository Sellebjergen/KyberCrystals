using System.Numerics;

namespace KyberCrystals;

public class Polynomial
{
    private readonly List<BigInteger> _coefficients;

    public Polynomial(List<BigInteger> coefficients)
    {
        _coefficients = coefficients;
    }

    public BigInteger GetCoefficient(int i)
    {
        try
        {
            var res = _coefficients[i];
            return res;
        }
        catch (Exception)
        {
            return new BigInteger(0);
        }
    }

    public List<BigInteger> GetCoefficients()
    {
        return _coefficients;
    }

    public List<BigInteger> GetPaddedCoefficients(int totalLength)
    {
        if (totalLength == _coefficients.Count)
            return _coefficients;
        if (totalLength < _coefficients.Count)
            throw new ArgumentException($"Can't return coefficients shorter than the polynomial.");

        var res = new List<BigInteger>(_coefficients);
        var diff = totalLength - _coefficients.Count;

        for (var i = 0; i < diff; i++)
        {
            res.Add(BigInteger.Zero);
        }

        return res;
    }

    public int GetDegree()
    {
        return _coefficients.Count - 1;
    }

    public void RemoveTrailingZeros()
    {
        var i = _coefficients.Count - 1;
        while (i > 0 && _coefficients[i] == BigInteger.Zero)
        {
            _coefficients.RemoveAt(i);
            i -= 1;
        }
    }

    public bool IsZeroPolynomial()
    {
        return _coefficients.All(c => c == BigInteger.Zero);
    }
}