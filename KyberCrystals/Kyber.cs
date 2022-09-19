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
        var a = GenerateMatrix(rho, _params.K);
        
        // Sample s
        var s = new Polynomial[] { };
        for (var i = 0; i < _params.K; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(n).First(), 64 * _params.Eta1);
            s[i] = _rq.Cbd(inputBytes, _params.Eta1);
            n += 1;
        }

        // Sample e
        var e = new Polynomial[] { };
        for (var i = 0; i < _params.K; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(n).First(), 64 * _params.Eta1);
            e[i] = _rq.Cbd(inputBytes, _params.Eta1);
            n += 1;
        }

        // var t = a * s + e
        // TODO: implement the rest of the key generation.
    }

    private Polynomial[][] GenerateMatrix(byte[] rho, int k)
    {
        // todo: this could be tested by checking the degree of all polynomials maybe?
        var a = new Polynomial[][] { };
        for (var i = 0; i < k - 1; i++)
        { 
            for (var j = 0; j < k - 1; j++)
            {
                var jByte = BitConverter.GetBytes(j).First();
                var iByte = BitConverter.GetBytes(i).First();
                
                a[i][j] = _rq.Parse(Utils.Xof(rho, jByte, iByte, (int)(3 * _rq.N)));
            }
        }

        return a;
    }
}