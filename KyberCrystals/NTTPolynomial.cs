using System.Numerics;

namespace KyberCrystals;

// TODO: maybe this should be ntt converter and instead ntt could be a better 

public class NttPolynomial
{
    private Polynomial _originalPoly;
    private List<Polynomial> _ntt;
    private Params _params;
    private PolynomialRing _rq;

    public NttPolynomial(Params param, PolynomialRing rq, Polynomial p)
    {
        _originalPoly = p;
        _params = param;
        _rq = rq;

        _ntt = CreateNttRepresentation(p);
    }

    private List<Polynomial> CreateNttRepresentation(Polynomial p)
    {
        var res = new List<Polynomial>();
        var rootOfUnity = Utils.GetRootOfUnity(_params.N, _params.Q);
        for (var i = 0; i < 128; i++) 
        {
            var zeta = BigInteger.ModPow(rootOfUnity, 2 * Utils.Br7(i) + 1, _params.Q);
            var modPoly = new Polynomial(new List<BigInteger> { -zeta, 0, 1 });
            res.Add(_rq.ModPoly(p, modPoly));
        }

        return res;
    }
    
    public List<Polynomial> GetKyberNtt(Polynomial p)
    {
        var coef = p.GetPaddedCoefficients(256);
        var res = new List<Polynomial>();

        for (var i = 0; i < 128; i++) {
            var sum = BigInteger.Zero;
            var sum2 = BigInteger.Zero;
            
            for (var j = 0; j < 128; j++)
            {
                var zeta = BigInteger.Pow(Utils.GetRootOfUnity(_params.N, _params.Q), (2 * Utils.Br7(i) + 1) * j);
                sum += coef[2 * j] * zeta;
                sum2 += coef[2 * j + 1] * zeta;
            }
            
            res.Add(new Polynomial(new List<BigInteger>{ sum, sum2 }));
        }

        return res;
    }
}