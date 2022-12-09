using System.Numerics;

namespace KyberCrystals;

// TODO: Currently 2^16 is hardcoded. This could be fun to get out of here.

public class NttPolyHelper
{
    private const short MontR2 = 1353;

    public readonly List<BigInteger> NttZetas = new()
    // todo: calc all of these dynamically.
    {
        // Uses mont with R = 2^16 which have inverse 169 in mod 3329
        2285, 2571, 2970, 1812, 1493, 1422, 287, 202, 3158, 622, 1577, 182, 962,
        2127, 1855, 1468, 573, 2004, 264, 383, 2500, 1458, 1727, 3199, 2648, 1017,
        732, 608, 1787, 411, 3124, 1758, 1223, 652, 2777, 1015, 2036, 1491, 3047,
        1785, 516, 3321, 3009, 2663, 1711, 2167, 126, 1469, 2476, 3239, 3058, 830,
        107, 1908, 3082, 2378, 2931, 961, 1821, 2604, 448, 2264, 677, 2054, 2226,
        430, 555, 843, 2078, 871, 1550, 105, 422, 587, 177, 3094, 3038, 2869, 1574,
        1653, 3083, 778, 1159, 3182, 2552, 1483, 2727, 1119, 1739, 644, 2457, 349,
        418, 329, 3173, 3254, 817, 1097, 603, 610, 1322, 2044, 1864, 384, 2114, 3193,
        1218, 1994, 2455, 220, 2142, 1670, 2144, 1799, 2051, 794, 1819, 2475, 2459,
        478, 3221, 3021, 996, 991, 958, 1869, 1522, 1628
    };

    public readonly List<BigInteger> NttZetasInv = new()
    {
        1701, 1807, 1460, 2371, 2338, 2333, 308, 108, 2851, 870, 854, 1510, 2535,
        1278, 1530, 1185, 1659, 1187, 3109, 874, 1335, 2111, 136, 1215, 2945, 1465,
        1285, 2007, 2719, 2726, 2232, 2512, 75, 156, 3000, 2911, 2980, 872, 2685,
        1590, 2210, 602, 1846, 777, 147, 2170, 2551, 246, 1676, 1755, 460, 291, 235,
        3152, 2742, 2907, 3224, 1779, 2458, 1251, 2486, 2774, 2899, 1103, 1275, 2652,
        1065, 2881, 725, 1508, 2368, 398, 951, 247, 1421, 3222, 2499, 271, 90, 853,
        1860, 3203, 1162, 1618, 666, 320, 8, 2813, 1544, 282, 1838, 1293, 2314, 552,
        2677, 2106, 1571, 205, 2918, 1542, 2721, 2597, 2312, 681, 130, 1602, 1871,
        829, 2946, 3065, 1325, 2756, 1861, 1474, 1202, 2367, 3147, 1752, 2707, 171,
        3127, 3042, 1907, 1836, 1517, 359, 758, 1441
    };

    private BigInteger ModQMulMont(BigInteger a, BigInteger b)
    {
        return MontgomeryReduce(a * b);
    }

    public List<BigInteger> Ntt(List<BigInteger> r)
    {
        int j;
        var k = 1;
        for (var l = 128; l >= 2; l >>= 1)
        for (var start = 0; start < 256; start = j + l)
        {
            var zeta = NttZetas[k];
            k += 1;
            for (j = start; j < start + l; j++)
            {
                var t = ModQMulMont(zeta, r[j + l]);
                r[j + l] = r[j] - t;
                r[j] += t;
            }
        }

        return r;
    }

    public List<BigInteger> InvNtt(List<BigInteger> r)
    {
        int j;
        var k = 0;
        for (var l = 2; l <= 128; l <<= 1)
        for (var start = 0; start < 256; start = j + l)
        {
            var zeta = NttZetasInv[k];
            k += 1;
            for (j = start; j < start + l; j++)
            {
                var t = r[j];
                r[j] = BarrettReduce(t + r[j + l]);
                r[j + l] = t - r[j + l];
                r[j + l] = ModQMulMont(zeta, r[j + l]);
            }
        }

        for (j = 0; j < 256; j++) r[j] = ModQMulMont(r[j], NttZetasInv[127]);
        return r;
    }

    public List<BigInteger> BaseMultiplier(BigInteger a0, BigInteger a1, BigInteger b0, BigInteger b1, BigInteger zeta)
    {
        var res = new BigInteger[2];
        res[0] = ModQMulMont(a1, b1);
        res[0] = ModQMulMont(res[0], zeta);
        res[0] += ModQMulMont(a0, b0);
        res[1] = ModQMulMont(a0, b1);
        res[1] += ModQMulMont(a1, b0);
        return new List<BigInteger>(res);
    }

    private BigInteger MontgomeryReduce(BigInteger a)
    {
        var u = a * 62209; // todo 62209 -> param.qInv
        var t = u * 3329; // todo 3329 -> param.q
        t = a - t;
        t >>= 16; // TODO: is this due to montgomery factor of 2^mont
        t %= 3329; // TODO: kyber param

        if (t < 0)
            return t + 3329; //TODO: Kyber param

        return t;
    }

    private BigInteger BarrettReduce(BigInteger a)
    {
        BigInteger t;
        const long shift = (long) 1 << 26;
        var v = (BigInteger)((shift + 3329 / 2) / 3329); // todo 3329 -> param.q
        t = (v * a) >> 26;
        t *= 3329; // todo 3329 -> param.q
        return a - t;
    }

    private BigInteger FromMontgomery(BigInteger a)
    {
        return BigInteger.ModPow(a * 169, 1, 3329); // params 3329 and inverse mont 169
    }

    public List<BigInteger> FromMontgomery(List<BigInteger> p)
    {
        for (var i = 0; i < 256; i++)
        {
            p[i] = FromMontgomery(p[i]);
            if (p[i] < 0) p[i] += 3329;
        }

        return p;
    }

    public Polynomial Multiplication(List<BigInteger> p1NttCoef, List<BigInteger> p2NttCoef)
    {
        var res = new BigInteger[256];
        for (var i = 0; i < 256 / 4; i++)
        {
            var x = BaseMultiplier(
                p1NttCoef[4 * i + 0],
                p1NttCoef[4 * i + 1],
                p2NttCoef[4 * i + 0],
                p2NttCoef[4 * i + 1],
                NttZetas[64 + i]);

            var y = BaseMultiplier(
                p1NttCoef[4 * i + 2],
                p1NttCoef[4 * i + 3],
                p2NttCoef[4 * i + 2],
                p2NttCoef[4 * i + 3],
                -NttZetas[64 + i]);

            res[4 * i + 0] = x[0];
            res[4 * i + 1] = x[1];
            res[4 * i + 2] = y[0];
            res[4 * i + 3] = y[1];
        }

        return new Polynomial(new List<BigInteger>(res));
    }

    public List<BigInteger> to_montgomery(Polynomial p)
    {
        var coefs = p.GetCoefficients();
        for (var i = 0; i < coefs.Count; i++) coefs[i] = ModQMulMont(MontR2, coefs[i]);

        return coefs;
    }

    public Polynomial ReduceCoefHacks(Polynomial p) // TODO: this should probably be removed.
    {
        var coef = p.GetCoefficients();
        for (var i = 0; i < p.GetCoefficients().Count; i++)
            coef[i] = BigInteger.ModPow(coef[i] * (BigInteger)Math.Pow(2, 16), 1, 3329); // todo: kyber params
        var res = new Polynomial(coef);
        res.RemoveTrailingZeros();
        return res;
    }
}