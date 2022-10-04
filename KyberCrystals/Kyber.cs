using System.Collections;
using System.Numerics;

namespace KyberCrystals;

public class Kyber
{
    private readonly Params _params;
    private readonly PolynomialRing _rq;
    
    public Kyber(Params p, PolynomialRing rq)
    {
        _params = p;
        _rq = rq; // TODO: Maybe this could be part of the params?
    }

    public (string, string) CPAPKE_KeyGen()
    {
        var d = Utils.GetRandomBytes(32);
        var (rho, sigma) = Utils.G(d);

        var n = 0;
        
        // Generate the A matrix
        var a = GenerateMatrix(rho, _params.K); // should be NttPolynomials
        
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
            var sum = new Polynomial(new List<BigInteger>());
            for (var j = 0; j < _params.K; j++)
            {
                var tmp = ntt.Multiplication(
                    a[i, j].GetPaddedCoefficients(256), 
                    sNtt[j].GetPaddedCoefficients(256));
                var tmp2 = _rq.Add(tmp, eNtt[j]);
                sum = _rq.Add(tmp2, sum);
            }
            t.Add(sum);
        }

        for (var i = 0; i < sNtt.Count; i++)
        {
            sNtt[i] = _rq.ReduceModuloQ(sNtt[i]);
        }

        // converts rho to string array
        var rho_str = "";
        foreach (var v in new BitArray(rho))
        {
            rho_str += (bool) v ? '1' : '0';
        }
        
        var pk = Utils.EncodePolynomialList(12, t) + rho_str;
        var sk = Utils.EncodePolynomialList(12, sNtt);

        return (pk, sk);
    }
    
    public string CPAPKE_encrypt(string pk, string m, string coins)
    {
        if (pk.Length < 12 * _params.K * _params.N + 32 * 8)
            throw new ArgumentException("Expected longer public key value");
        if (m.Length < 32 * 8)
            throw new ArgumentException($"Expected a message of length {32 * 8}, got one of length {m.Length}");
        if (coins.Length < 32 * 8)
            throw new ArgumentException($"Expected coins of length {32 * 8} got one of length {coins.Length}");

        var n = 0;
        var tBin = pk.Substring(0, pk.Length - 32 * 8); // todo: I work in bits not in bytes here. thus we take times 8
        var tNtt = Utils.Decode(12, tBin);
        var rho = pk.Substring(pk.Length - 32 * 8, 32 * 8);
        var aInv = GenerateTransposedMatrix(Utils.GetBytes(rho), _params.K);

        var r = new Polynomial[_params.K];
        for (var i = 0; i < _params.K; i++)
        {
            r[i] = _rq.Cbd(Utils.Prf(Utils.GetBytes(coins), BitConverter.GetBytes(n).First(), _params.Eta1 * 64), _params.Eta1);
            n += 1;
        }

        var e1 = new Polynomial[_params.K];
        for (var i = 0; i < _params.K; i++)
        {
            e1[i] = _rq.Cbd(Utils.Prf(Utils.GetBytes(coins), BitConverter.GetBytes(n).First(), _params.Eta1 * 64), _params.Eta1);
        }

        var e2 = _rq.Cbd(Utils.Prf(Utils.GetBytes(coins), BitConverter.GetBytes(n).First(), _params.Eta2 * 64), _params.Eta2);

        var ntt = new NttPolyHelper();
        var coinsNtt = new Polynomial[_params.K];
        for (var i = 0; i < _params.K; i++)
            coinsNtt[i] = new Polynomial(ntt.Ntt(r[i].GetPaddedCoefficients(256))); // todo: probably this could be optimized with helper functions
        
        // calculate value u
        var uNtt = new List<Polynomial>();
        for (var i = 0; i < _params.K; i++)
        {
            var sum = new Polynomial(new List<BigInteger>());
            for (var j = 0; j < _params.K; j++)
            {
                var tmp = ntt.Multiplication(
                    aInv[i, j].GetPaddedCoefficients(256), 
                    coinsNtt[j].GetPaddedCoefficients(256));
                sum = _rq.Add(sum, tmp);
            }
            uNtt.Add(sum);
        }
        
        // get u from uNtt.
        var u = new Polynomial[uNtt.Count];
        for (var i = 0; i < uNtt.Count; i++)
        {
            var tmp = ntt.InvNtt(uNtt[i].GetPaddedCoefficients(256));
            var tmp2 = ntt.FromMontgomery(tmp);
            var tmp3 = ntt.ReduceCoefHacks(new Polynomial(tmp2)); // todo: a new type for reducecoefhacks?
            u[i] = _rq.Add(tmp3, e1[i]);
        }

        // calculate the value of v.
        var vNtt = new Polynomial(new List<BigInteger>{ 0 });
        var math = ntt.Multiplication(tNtt.GetPaddedCoefficients(256), vNtt.GetPaddedCoefficients(256));
        var math1 = ntt.FromMontgomery(math.GetPaddedCoefficients(256));
        var math2 = ntt.ReduceCoefHacks(new Polynomial(math1));
        var v = _rq.Add(math2, e2);
        var v2 = _rq.Add(v, Utils.Compress(Utils.Decode(1, m), 1));

        var c1 = Utils.Encode(_params.Du, Utils.Compress(u, (short) _params.Du));
        var c2 = Utils.Encode(_params.Dv, Utils.Compress(v2, (short) _params.Dv));

        return string.Join("", c1) +  c2;
    }
    
    public string CPAPKE_decrypt(string sk, string c)
    {
        var ntt = new NttPolyHelper();
        
        if (sk.Length != 12 * _params.K * _params.N)
            throw new ArgumentException(
                $"The secret key need to be of length {12 * _params.K * _params.N} but was of length {sk.Length}");
        if (c.Length != _params.Du * _params.K * _params.N + _params.Dv * _params.N)
            throw new ArgumentException(
                $"The secret key need to be of length {_params.Du * _params.K * _params.N + _params.Dv * _params.N} but was of length {sk.Length}");
        
        var u = Utils.Decompress(Utils.Decode(_params.Du, c), (short) _params.Du);
        var cBytes = c.Substring(0, _params.Du * _params.K * _params.N);
        var v = Utils.Decompress(Utils.Decode(_params.Dv, cBytes), (short) _params.Dv);
        var s = Utils.Decode(12, sk);

        var uNtt = ntt.Ntt(u.GetPaddedCoefficients(256));
        var uNttS = ntt.Multiplication(uNtt, s.GetPaddedCoefficients(256));
        var uS = ntt.InvNtt(uNttS.GetPaddedCoefficients(256));
        var uSPol = new Polynomial(uS);

        var m = Utils.Encode(1, Utils.Compress(_rq.Sub(v, uSPol), 1));
        
        return m;
    }

    public Polynomial[,] GenerateMatrix(byte[] rho, int k) // todo: those could be private and use a strat pattern instead.
    {
        var a = new Polynomial[k, k];
        
        for (var i = 0; i < k; i++)
        {
            for (var j = 0; j < k; j++)
            {
                var jByte = BitConverter.GetBytes(j).First();
                var iByte = BitConverter.GetBytes(i).First();
                
                a[i, j] = _rq.Parse(Utils.Xof(rho, jByte, iByte, (int)(3 * _rq.N)));
            }
        }

        return a;
    }
    
    public Polynomial[,] GenerateTransposedMatrix(byte[] rho, int k)
    {
        var a = new Polynomial[k, k];
        
        for (var i = 0; i < k; i++)
        {
            for (var j = 0; j < k; j++)
            {
                var jByte = BitConverter.GetBytes(j).First();
                var iByte = BitConverter.GetBytes(i).First();
                
                a[j, i] = _rq.Parse(Utils.Xof(rho, jByte, iByte, (int)(3 * _rq.N)));
            }
        }

        return a;
    }
}