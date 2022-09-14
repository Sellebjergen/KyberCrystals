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
            var t = r.GetCoefficient(r.GetDegree()) / mod.GetCoefficient(mod.GetDegree());
            var modulo_mul_t = rq.ConstMult(mod, t);
            
        }

        return null;
    }
}
