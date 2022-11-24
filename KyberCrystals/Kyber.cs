using System.Numerics;

namespace KyberCrystals;

public class Kyber
{
    private readonly KyberParams _kyberParams;
    private readonly PolynomialRing _rq;
    private readonly NttPolyHelper _ntt;
    private readonly IRng _rng;

    public Kyber(KyberParams p, PolynomialRing rq)
    {
        _kyberParams = p;
        _rq = rq;
        _ntt = new NttPolyHelper();
        _rng = new StdRandom();
    }
    
    public Kyber(KyberParams p, PolynomialRing rq, IRng rng)
    {
        _kyberParams = p;
        _rq = rq;
        _ntt = new NttPolyHelper();
        _rng = rng;
    }

    public (CpapkePublicKey, SecretKey) CCAKEM_keygen()
    {
        var (pk, skPrime) = CPAPKE_KeyGen();
        var z = _rng.GetRandomBytes(32);
        
        var sk = new SecretKey(
            skPrime, 
            pk, 
            Utils.H(Utils.GetBytes(pk.GetCombinedString())), 
            z);
   
        return (pk, sk);
    }

    public (CpapkeCiphertext, byte[]) CCAKEM_encrypt(CpapkePublicKey pk)
    {
        // if (pk.Length != 12 * _kyberParams.K * _kyberParams.N + 32 * 8)
        //     throw new ArgumentException(
        //         $"Pk should be of length {12 * _kyberParams.K * _kyberParams.N + 32 * 8} but was of length {pk.Length}");

        var m = _rng.GetRandomBytes(32);
        m = Utils.H(m);

        var hPk = Utils.H(Utils.GetBytes(pk.GetCombinedString()));
        var mHpk = Utils.CombineArrays(m, hPk);

        var (kPrime, r) = Utils.G(mHpk);
        var c = CPAPKE_encrypt(pk, m, r);
        var hC = Utils.H(Utils.GetBytes(c.GetBinaryString()));

        var kHc = Utils.CombineArrays(kPrime, hC);

        var k = Utils.Kdf(kHc, 32);

        return (c, k);
    }

    public byte[] CCAKEM_decrypt(CpapkeCiphertext c, SecretKey sk)
    {
        // if (sk.GetTotalLength() != 24 * _kyberParams.K * _kyberParams.N + 96 * 8)
        //     throw new ArgumentException(
        //         $"The length of the secret key was expected to be {24 * _kyberParams.K * _kyberParams.N + 96 * 8} " +
        //         $"but was found to be {sk.GetTotalLength()}");

        // if (c.Length != _kyberParams.Du * _kyberParams.K * _kyberParams.N + _kyberParams.Dv * _kyberParams.N)
        //     throw new ArgumentException($"The length of the ciphertext is expected to be " +
        //                                 $"{_kyberParams.Du * _kyberParams.K * _kyberParams.N + _kyberParams.Dv * _kyberParams.N}" +
        //                                 $"but was found to be {c.Length}");

        var (skPrime, pk, h, z) = sk.UnpackSecretKey();

        var mPrime = CPAPKE_decrypt(skPrime, c);
        var (kPrime, rPrime) = Utils.G(Utils.GetBytes(mPrime + Utils.BytesToString(h)));

        var cPrime = CPAPKE_encrypt(pk, Utils.GetBytes(mPrime), rPrime);
        if (c.GetBinaryString() == cPrime.GetBinaryString())
        {
            var hC = Utils.H(Utils.GetBytes(c.GetBinaryString()));
        
            var kHc = new byte[kPrime.Length + hC.Length];
            kPrime.CopyTo(kHc, 0);
            hC.CopyTo(kHc, kPrime.Length);
        
            var k = Utils.Kdf(kHc, 32);
            return k;
        }

        var tmp = Utils.H(Utils.GetBytes(c.GetBinaryString()));
        var buf = new byte[z.Length + tmp.Length];
        Array.Copy(z, buf, z.Length);
        Array.Copy(tmp, 0, buf, z.Length, tmp.Length);
        
        var randomValue = Utils.Kdf(buf);
        return randomValue;
    }

    public (CpapkePublicKey, string) CPAPKE_KeyGen()
    {
        var d = _rng.GetRandomBytes(32);
        var (rho, sigma) = Utils.G(d);
        var n = 0;

        // Generate the A matrix
        var a = GenerateMatrix(rho, _kyberParams.K);

        // Sample s
        var s = new Polynomial[_kyberParams.K];
        for (var i = 0; i < _kyberParams.K; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(n).First(), 64 * _kyberParams.Eta1);
            s[i] = _rq.Cbd(inputBytes, _kyberParams.Eta1);
            n += 1;
        }

        // Sample e
        var e = new Polynomial[_kyberParams.K];
        for (var i = 0; i < _kyberParams.K; i++)
        {
            var inputBytes = Utils.Prf(sigma, BitConverter.GetBytes(n).First(), 64 * _kyberParams.Eta1);
            e[i] = _rq.Cbd(inputBytes, _kyberParams.Eta1);
            n += 1;
        }

        foreach (var p in s) // TODO: this could return an NttPolynomial to make it explicit that it is not an algebraic polynomial.
            _ntt.Ntt(p.GetPaddedCoefficients(256));
        foreach (var p in e)
            _ntt.Ntt(p.GetPaddedCoefficients(256));

        // Calculate value of t
        var t = CalcTMatrix(a, s, e);
        
        for (var i = 0; i < s.Length; i++)
            s[i] = _rq.ReduceModuloQ(s[i]);

        var pk = new CpapkePublicKey(Utils.Encode(12, t), rho);
        var sk = Utils.EncodePolynomialList(12, s);

        return (pk, sk);
    }

    public CpapkeCiphertext CPAPKE_encrypt(CpapkePublicKey pk, byte[] m, byte[] coins)
    {
        // if (pk.Length < 12 * _kyberParams.K * _kyberParams.N + 32 * 8)
        //     throw new ArgumentException("Expected longer public key value");
        if (m.Length < 32)
            throw new ArgumentException($"Expected a message of length {32}, got one of length {m.Length}");
        if (coins.Length < 32)
            throw new ArgumentException($"Expected coins of length {32} got one of length {coins.Length}");

        var n = 0;
        var tNtt = Utils.Decode(12, pk.Test);
        var aInv = GenerateTransposedMatrix(pk.Rho, _kyberParams.K);

        var r = new Polynomial[_kyberParams.K];
        for (var i = 0; i < _kyberParams.K; i++)
        {
            r[i] = _rq.Cbd(Utils.Prf(coins, BitConverter.GetBytes(n).First(), _kyberParams.Eta1 * 64),
                _kyberParams.Eta1);
            n += 1;
        }

        var rNtt = new Polynomial[_kyberParams.K];
        for (var i = 0; i < _kyberParams.K; i++)
        {
            rNtt[i] = new Polynomial(_ntt.Ntt(r[i].GetPaddedCoefficients(256)));
        }

        var e1 = new Polynomial[_kyberParams.K];
        for (var i = 0; i < _kyberParams.K; i++)
        {
            var tmpPrf = Utils.Prf(coins, Convert.ToByte(n), _kyberParams.Eta2 * 64);
            e1[i] = _rq.Cbd(tmpPrf, _kyberParams.Eta2);
            n += 1;
        }
        
        var e2 = _rq.Cbd(Utils.Prf(coins, Convert.ToByte(n), _kyberParams.Eta2 * 64), _kyberParams.Eta2);

        // calculate value u
        var uNtt = CalcUMatrix(aInv, rNtt, e1);
        
        // calculate the value of v.
        var sum = new Polynomial(new List<BigInteger> { 0 });
        for (var i = 0; i < _kyberParams.K; i++)
        {
            var tmp = _ntt.Multiplication(
                tNtt[i].GetPaddedCoefficients(256), 
                rNtt[i].GetPaddedCoefficients(256));
            sum = _rq.Add(sum, tmp);
        }
        var v = _ntt.InvNtt(sum.GetPaddedCoefficients(256));
        
        var vPoly = new Polynomial(v);
        vPoly = _rq.Add(vPoly, e2);
        vPoly = _rq.Add(vPoly, ConvertMessageToPolynomial(m));
        
        var c1 = Utils.Encode(_kyberParams.Du, Utils.Compress(uNtt, (short)_kyberParams.Du));
        var tmpC2 = Utils.Compress(vPoly, (short)_kyberParams.Dv);
        var c2 = Utils.Encode(_kyberParams.Dv, tmpC2);
        
        return new CpapkeCiphertext(c1, c2);
    }
    
    private Polynomial ConvertMessageToPolynomial(byte[] m)
    {
        var binPoly = Utils.Decode(1, Utils.BytesToString(m));
        return _rq.ConstMult(binPoly, Convert.ToInt16(_kyberParams.Q / 2) + 1);
    }

    public string CPAPKE_decrypt(string sk, CpapkeCiphertext c)
    {
        if (sk.Length != 12 * _kyberParams.K * _kyberParams.N)
            throw new ArgumentException(
                $"The secret key need to be of length {12 * _kyberParams.K * _kyberParams.N} but was of length {sk.Length}");
        // if (c.Length != _kyberParams.Du * _kyberParams.K * _kyberParams.N + _kyberParams.Dv * _kyberParams.N)
        //     throw new ArgumentException(
        //         $"The secret key need to be of length {_kyberParams.Du * _kyberParams.K * _kyberParams.N + _kyberParams.Dv * _kyberParams.N} but was of length {sk.Length}");
        

        var uTmp = Utils.Decode(_kyberParams.Du, c.C1);
        var u = Utils.Decompress(uTmp, (short)_kyberParams.Du);
        var v = Utils.Decompress(Utils.Decode(_kyberParams.Dv, c.C2), (short)_kyberParams.Dv);
        
        var s = RetreiveSecretKey(sk);
        var uNtt = u.Select(p => _ntt.Ntt(p.GetPaddedCoefficients(256))).ToList();
        
        var sum = new Polynomial(new List<BigInteger> { 0 });
        for (var i = 0; i < _kyberParams.K; i++)
        {
            var tmp = _ntt.Multiplication(s[i].GetPaddedCoefficients(256), uNtt[i]);
            sum = _rq.Add(sum, tmp);
        }
        var vPoly = new Polynomial(_ntt.InvNtt(sum.GetPaddedCoefficients(256)));
        var m = _rq.Sub(v, vPoly);

        return Utils.Encode(1, Utils.Compress(m, 1));
    }

    public Polynomial[,] GenerateMatrix(byte[] rho, int k)
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
    
    private Polynomial[] RetreiveSecretKey(string sk)
    {
        var s = new Polynomial[_kyberParams.K];
        var subLength = sk.Length / _kyberParams.K;
        for (var i = 0; i < _kyberParams.K; i++)
        {
            s[i] = Utils.Decode(12, sk.Substring(i * subLength, subLength));
        }
        
        return s;
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
    
    private Polynomial[] CalcUMatrix(Polynomial[,] aInv, Polynomial[] rNtt, Polynomial[] e1)
    {
        var uNtt = new Polynomial[_kyberParams.K];
        
        for (var i = 0; i < _kyberParams.K; i++)
        {
            var sum = new Polynomial(new List<BigInteger> { 0 });
            for (var j = 0; j < _kyberParams.K; j++)
            {
                var x = _ntt.Multiplication(
                    aInv[i, j].GetPaddedCoefficients(256), 
                    rNtt[j].GetPaddedCoefficients(256));
                sum = _rq.Add(sum, x);
            }

            uNtt[i] = _rq.Add(new Polynomial(_ntt.InvNtt(sum.GetPaddedCoefficients(256))), e1[i]);
        }
        
        return uNtt;
    }
    
    private Polynomial[] CalcTMatrix(Polynomial[,] a, Polynomial[] s, Polynomial[] e)
    {
        var t = new Polynomial[_kyberParams.K];

        for (var i = 0; i < _kyberParams.K; i++)
        {
            var sum = new Polynomial(new List<BigInteger> { 0 });
            for (var j = 0; j < _kyberParams.K; j++)
            {
                var x = _ntt.Multiplication(
                    a[i, j].GetPaddedCoefficients(256), 
                    s[j].GetPaddedCoefficients(256));
                sum = _rq.Add(sum, x);
            }

            t[i] = _rq.Add(new Polynomial(_ntt.to_montgomery(sum)), e[i]);
        }
        
        return t;
    }
}
