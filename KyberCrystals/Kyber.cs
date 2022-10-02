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

        var n = 0;
        
        // Generate the A matrix
        var _A = GenerateMatrix(rho, _params.K); // should be NttPolynomials
        
        // Sample s
        var s = new List<Polynomial>();
        for (var i = 0; i < _params.K; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(n).First(), 64 * _params.Eta1);
            s.Add(_rq.Cbd(inputBytes, _params.Eta1));
            n += 1;
        }

        // Sample e
        var e = new List<Polynomial>();
        for (var i = 0; i < _params.K; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(n).First(), 64 * _params.Eta1);
            e.Add(_rq.Cbd(inputBytes, _params.Eta1));
            n += 1;
        }

        // converts to Ntt representation
        var ntt = new NttPolyHelper();
        var sNtt = new List<Polynomial>();
        foreach (var p in s)
            sNtt.Add(new Polynomial(ntt.Ntt(p.GetPaddedCoefficients(256))));
        
        var eNtt = new List<Polynomial>();
        foreach (var p in e)
            eNtt.Add(new Polynomial(ntt.Ntt(p.GetPaddedCoefficients(256))));
        
        // Calculate value of t
        var t = new List<Polynomial>();
        for (var i = 0; i < _params.K; i++)
        {
            var sum = new Polynomial(new List<BigInteger> { 0 });
            for (var j = 0; j < _params.K; j++)
            {
                var tmp = ntt.Multiplication(
                    _A[i, j].GetPaddedCoefficients(256), 
                    sNtt[j].GetPaddedCoefficients(256));
                var tmp2 = _rq.Add(tmp, eNtt[j]);
                sum = _rq.Add(tmp2, sum);
            }
            t.Add(sum);
        }


        var z = 0;
        // TODO: implement the rest of the key generation.
    }

    private Polynomial[,] GenerateMatrix(byte[] rho, int k)
    {
        var a = new Polynomial[k, k];
        for (var i = 0; i < k - 1; i++)
        { 
            for (var j = 0; j < k - 1; j++)
            {
                var jByte = BitConverter.GetBytes(j).First();
                var iByte = BitConverter.GetBytes(i).First();
                
                a[i, j] = _rq.Parse(Utils.Xof(rho, jByte, iByte, (int)(3 * _rq.N)));
            }
        }

        return a;
    }
}