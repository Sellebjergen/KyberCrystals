using System.Numerics;

namespace KyberCrystals;

public interface IPolyModStrategy
{
    public Polynomial PolyMod(PolynomialRing rq, Polynomial p, Polynomial mod);
}

public class LongPolynomialDivision : IPolyModStrategy
{
    public Polynomial PolyMod(PolynomialRing rq, Polynomial p, Polynomial mod)
    {
        var r = new Polynomial(p.GetCoefficients());

        while (!r.IsZeroPolynomial() && r.GetCoefficients().Count >= mod.GetCoefficients().Count)
        {
            var t = r.GetCoefficient(r.GetCoefficients().Count - 1) /
                    mod.GetCoefficient(mod.GetCoefficients().Count - 1);
            var modMulT = rq.ConstMult(mod, t);
            var shift = ShiftPolynomial(modMulT, r.GetCoefficients().Count - mod.GetCoefficients().Count);
            r = rq.Sub(r, shift);
        }

        return r;
    }

    private Polynomial ShiftPolynomial(Polynomial p, int amount)
    {
        var coef = new List<BigInteger>();

        for (var i = 0; i < amount; i++)
            coef.Add(BigInteger.Zero);

        foreach (var c in p.GetCoefficients())
            coef.Add(c);

        return new Polynomial(coef);
    }
}