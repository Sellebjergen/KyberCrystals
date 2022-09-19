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

        while (!r.IsZeroPolynomial() && r.GetLengthOfPolynomial() >= mod.GetLengthOfPolynomial())
        {
            var t = r.GetCoefficient(r.GetLengthOfPolynomial() - 1) / mod.GetCoefficient(mod.GetLengthOfPolynomial() - 1);
            var modMulT = rq.ConstMult(mod, t);
            var shift = ShiftPolynomial(modMulT, r.GetLengthOfPolynomial() - mod.GetLengthOfPolynomial());
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
