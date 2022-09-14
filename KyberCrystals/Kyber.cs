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

        var k = 8;
        var N = 0;
        var A = new Polynomial[][] { };
        
        // Generate the A matrix
        for (var i = 0; i < k - 1; i++)
        { 
            for (var j = 0; j < k - 1; j++)
            {
                var jByte = BitConverter.GetBytes(j).First();
                var iByte = BitConverter.GetBytes(i).First();
                
                A[i][j] = _rq.Parse(Utils.Xof(rho, jByte, iByte, (int)(3 * _rq._n)));
            }
        }
        
        // Sample s
        var s = new Polynomial[] { };
        for (var i = 0; i < k; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(N).First(), 64 * _params.Eta1);
            s[i] = _rq.Cbd(inputBytes, _params.Eta1);
            N += 1;
        }

        // Sample e
        var e = new Polynomial[] { };
        for (var i = 0; i < k; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(N).First(), 64 * _params.Eta1);
            e[i] = _rq.Cbd(inputBytes, _params.Eta1);
            N += 1;
        }
        
        // Convert s and e to NTT form.
        if (_rq._n != 256) 
            // TODO: the paper does not specify for n != 256, but it must be possible?
            throw new NotImplementedException("Kyber only specifies for n = 256");

        foreach (var p in s)
            p.ReduceToNttForm();

        foreach (var p in e)
            p.ReduceToNttForm();

        // TODO: implement the rest of the key generation.
    }
}