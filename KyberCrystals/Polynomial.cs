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
        return _coefficients[i];
    }

    public List<BigInteger> GetCoefficients()
    {
        return _coefficients;
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