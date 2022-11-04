namespace KyberCrystals;

// TODO: check the input fields in all functions. 

public class Kyber
{
    private readonly Params _params;
    private readonly PolynomialRing _rq;
    private readonly NttPolyHelper _ntt;

    public Kyber(Params p, PolynomialRing rq)
    {
        _params = p;
        _rq = rq; // TODO: Maybe this could be part of the params?
        _ntt = new NttPolyHelper();
    }

    public (CPAPKE_PublicKey, SecretKey) CCAKEM_keygen()
    {
        var (pk, skPrime) = CPAPKE_KeyGen();
        var z = Utils.GetRandomBytes(32);
    
        var sk = new SecretKey(
            skPrime, 
            pk, 
            Utils.H(Utils.GetBytes(pk.GetCombinedString())), 
            z);
   
        return (pk, sk);
    }

    public (CPAPKE_Ciphertext, string) CCAKEM_encrypt(CPAPKE_PublicKey pk)
    {
        // if (pk.Length != 12 * _params.K * _params.N + 32 * 8)
        //     throw new ArgumentException(
        //         $"Pk should be of length {12 * _params.K * _params.N + 32 * 8} but was of length {pk.Length}");

        var m = Utils.GetRandomBytes(32);
        // var m = Convert.FromHexString("ab7e585a2027b3cdd3ae31282b705f0c4d350155dda5a89b27e6e4f2ab10f871");
        m = Utils.H(m);

        // todo: append concat bytes lists function.
        var hPk = Utils.H(Utils.GetBytes(pk.GetCombinedString()));
        var mHpk = new byte[m.Length + hPk.Length];
        m.CopyTo(mHpk, 0);
        hPk.CopyTo(mHpk, m.Length);

        var (kPrime, r) = Utils.G(mHpk);
        var c = CPAPKE_encrypt(pk, m, r);
        var hC = Utils.H(Utils.GetBytes(c.GetBinaryString()));

        var kHc = new byte[kPrime.Length + hC.Length];
        kPrime.CopyTo(kHc, 0);
        hC.CopyTo(kHc, kPrime.Length);

        var k = Utils.Kdf(kHc, 32);

        return (c, Utils.BytesToString(k)); // TODO: wouldn't it be better to just return the bytes right here?
    }

    public string CCAKEM_decrypt(CPAPKE_Ciphertext c, SecretKey sk)
    {
        // if (sk.GetTotalLength() != 24 * _params.K * _params.N + 96 * 8)
        //     throw new ArgumentException(
        //         $"The length of the secret key was expected to be {24 * _params.K * _params.N + 96 * 8} " +
        //         $"but was found to be {sk.GetTotalLength()}");

        // if (c.Length != _params.Du * _params.K * _params.N + _params.Dv * _params.N)
        //     throw new ArgumentException($"The length of the ciphertext is expected to be " +
        //                                 $"{_params.Du * _params.K * _params.N + _params.Dv * _params.N}" +
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
            return Utils.BytesToString(k);
        }

        var tmp = Utils.H(Utils.GetBytes(c.GetBinaryString()));
        var buf = new byte[z.Length + tmp.Length];
        Array.Copy(z, buf, z.Length);
        Array.Copy(tmp, 0, buf, z.Length, tmp.Length); // TODO: I really need a combine bytelists function
        
        var randomValue = Utils.Kdf(buf);
        return Utils.BytesToString(randomValue);
    }

    public (CPAPKE_PublicKey, string) CPAPKE_KeyGen()
    {
        var d = Utils.GetRandomBytes(32); // TODO: remember to uncomment this!
        var (rho, sigma) = Utils.G(d);
        var n = 0;

        // Generate the A matrix
        var a = GenerateMatrix(rho, _params.K);

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
        var sNtt = new List<Polynomial>();
        foreach (var p in s)
            sNtt.Add(new Polynomial(_ntt.Ntt(p.GetPaddedCoefficients(256))));

        var eNtt = new List<Polynomial>();
        foreach (var p in e)
            eNtt.Add(new Polynomial(_ntt.Ntt(p.GetPaddedCoefficients(256))));

        // Calculate value of t
        var t = new Polynomial[2]; // TODO: make this dynamic - Generate a function for this, when we have it working.
        var x = _ntt.Multiplication(a[0, 0].GetPaddedCoefficients(256), s[0].GetPaddedCoefficients(256));
        var y = _ntt.Multiplication(a[0, 1].GetPaddedCoefficients(256), s[1].GetPaddedCoefficients(256));
        var x1 = _ntt.Multiplication(a[1, 0].GetPaddedCoefficients(256), s[0].GetPaddedCoefficients(256));
        var y2 = _ntt.Multiplication(a[1, 1].GetPaddedCoefficients(256), s[1].GetPaddedCoefficients(256));
        t[0] = _rq.Add(new Polynomial(_ntt.to_montgomery(_rq.Add(x, y))), e[0]);
        t[1] = _rq.Add(new Polynomial(_ntt.to_montgomery(_rq.Add(x1, y2))), e[1]);
        
        for (var i = 0; i < sNtt.Count; i++)
        {
            sNtt[i] = _rq.ReduceModuloQ(sNtt[i]); // TODO: could probably a function to reduce all coefs in poly
        }

        var pk = new CPAPKE_PublicKey(Utils.Encode(12, t), rho);
        var sk = Utils.EncodePolynomialList(12, sNtt);

        return (pk, sk);
    }

    public CPAPKE_Ciphertext CPAPKE_encrypt(CPAPKE_PublicKey pk, byte[] m, byte[] coins)
    {
        // if (pk.Length < 12 * _params.K * _params.N + 32 * 8)
        //     throw new ArgumentException("Expected longer public key value");
        if (m.Length < 32)
            throw new ArgumentException($"Expected a message of length {32}, got one of length {m.Length}");
        if (coins.Length < 32)
            throw new ArgumentException($"Expected coins of length {32} got one of length {coins.Length}");

        var n = 0;
        var tNtt = Utils.Decode(12, pk.Test);
        var aInv = GenerateTransposedMatrix(pk.Rho, _params.K);

        var r = new Polynomial[_params.K];
        for (var i = 0; i < _params.K; i++)
        {
            r[i] = _rq.Cbd(Utils.Prf(coins, BitConverter.GetBytes(n).First(), _params.Eta1 * 64),
                _params.Eta1);
            n += 1;
        }

        var rNtt = new Polynomial[_params.K];
        for (var i = 0; i < _params.K; i++)
        {
            rNtt[i] = new Polynomial(_ntt.Ntt(r[i].GetPaddedCoefficients(256)));
        }

        var e1 = new Polynomial[_params.K];
        for (var i = 0; i < _params.K; i++)
        {
            var tmpPrf = Utils.Prf(coins, Convert.ToByte(n), _params.Eta2 * 64);
            e1[i] = _rq.Cbd(tmpPrf, _params.Eta2);
            n += 1;
        }
        
        var e2 = _rq.Cbd(Utils.Prf(coins, Convert.ToByte(n), _params.Eta2 * 64), _params.Eta2);

        // calculate value u
        var uNtt = new Polynomial[_params.K]; // TODO: All of this can be made dynamic, it won't work for k != 2.
        var x = _ntt.Multiplication(aInv[0, 0].GetPaddedCoefficients(256), rNtt[0].GetPaddedCoefficients(256));
        var y = _ntt.Multiplication(aInv[0, 1].GetPaddedCoefficients(256), rNtt[1].GetPaddedCoefficients(256));
        var x1 = _ntt.Multiplication(aInv[1, 0].GetPaddedCoefficients(256), rNtt[0].GetPaddedCoefficients(256));
        var y1 = _ntt.Multiplication(aInv[1, 1].GetPaddedCoefficients(256), rNtt[1].GetPaddedCoefficients(256));
        var tmpNtt = new Polynomial(_rq.Add(x, y).GetPaddedCoefficients(256));
        var tmp =new Polynomial(_ntt.InvNtt(tmpNtt.GetPaddedCoefficients(256)));
        uNtt[0] = _rq.Add(tmp, e1[0]);
        var tmpNtt2 = new Polynomial(_rq.Add(x1, y1).GetPaddedCoefficients(256));
        var tmp2 = _ntt.InvNtt(tmpNtt2.GetPaddedCoefficients(256));
        uNtt[1] = _rq.Add(new Polynomial(tmp2), e1[1]);
        
        // calculate the value of v.
        var q2 = _ntt.Multiplication(tNtt[1].GetPaddedCoefficients(256), rNtt[1].GetPaddedCoefficients(256));
        var q3 = _ntt.Multiplication(tNtt[0].GetPaddedCoefficients(256), rNtt[0].GetPaddedCoefficients(256));
        var q2Plusq3 = _rq.Add(q2, q3);
        var v = _ntt.InvNtt(q2Plusq3.GetPaddedCoefficients(256));
        var vPoly = new Polynomial(v);
        vPoly = _rq.Add(vPoly, e2);
        vPoly = _rq.Add(vPoly, ConvertMessageToPolynomial(m));
        
        var c1 = Utils.Encode(_params.Du, Utils.Compress(uNtt, (short)_params.Du));
        var tmpC2 = Utils.Compress(vPoly, (short)_params.Dv);
        var c2 = Utils.Encode(_params.Dv, tmpC2);
        
        return new CPAPKE_Ciphertext(c1, c2);
    }
    
    private Polynomial ConvertMessageToPolynomial(byte[] m)
    {
        var binPoly = Utils.Decode(1, Utils.BytesToString(m));
        return _rq.ConstMult(binPoly, Convert.ToInt16(_params.Q / 2) + 1);
    }

    public string CPAPKE_decrypt(string sk, CPAPKE_Ciphertext c)
    {
        if (sk.Length != 12 * _params.K * _params.N)
            throw new ArgumentException(
                $"The secret key need to be of length {12 * _params.K * _params.N} but was of length {sk.Length}");
        // if (c.Length != _params.Du * _params.K * _params.N + _params.Dv * _params.N)
        //     throw new ArgumentException(
        //         $"The secret key need to be of length {_params.Du * _params.K * _params.N + _params.Dv * _params.N} but was of length {sk.Length}");
        

        var uTmp = Utils.Decode(_params.Du, c.C1);
        var u = Utils.Decompress(uTmp, (short)_params.Du);
        var v = Utils.Decompress(Utils.Decode(_params.Dv, c.C2), (short)_params.Dv);
        
        // TODO: this could be made more dynamic!
        var s = new Polynomial[2];
        s[0] = Utils.Decode(12, sk.Substring(0, sk.Length / 2));
        s[1] = Utils.Decode(12, sk.Substring(sk.Length / 2));
        
        
        // var uNtt = ntt.Ntt(u.GetPaddedCoefficients(256));
        var uNtt = u.Select(p => _ntt.Ntt(p.GetPaddedCoefficients(256))).ToList();
        
        // TODO: make this dynamic!
        var q = _ntt.Multiplication(s[0].GetPaddedCoefficients(256), uNtt[0]);
        var w = _ntt.Multiplication(s[1].GetPaddedCoefficients(256), uNtt[1]);
        var v1 = _ntt.InvNtt(_rq.Add(q, w).GetPaddedCoefficients(256));
        var v1Poly = new Polynomial(v1);

        var m = _rq.Sub(v, v1Poly);

        return Utils.Encode(1, Utils.Compress(m, 1));
    }

    // todo: those could be private and use a strat pattern instead.
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