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

    public List<BigInteger> GetPaddedCoefficients(int amount)
    {
        if (amount < _coefficients.Count)
            throw new ArgumentException("Can't return coefficients shorter than the polynomial.");

        var res = new List<BigInteger>(_coefficients);
        var diff = amount - _coefficients.Count;
        
        for (var i = 0; i < diff; i++)
        {
            res.Add(new BigInteger(0));
        }

        return res;
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

                start = l + counter + 1;
            }

            l >>= 1;
        }
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
}