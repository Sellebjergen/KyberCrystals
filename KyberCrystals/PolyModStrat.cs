using System.Numerics;
using System.Xml;

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

        while (!r.IsZeroPolynomial() && r.GetDegree() >= mod.GetDegree())
        {
            var t = r.GetCoefficient(r.GetDegree() - 1) / mod.GetCoefficient(mod.GetDegree() - 1);
            var modMulT = rq.ConstMult(mod, t);
            var shift = ShiftPolynomial(modMulT, r.GetDegree() - mod.GetDegree());

            var temp = rq.Sub(r, shift);
            if (temp.IsZeroPolynomial())
                return r;

            r = temp;
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
