using System.Numerics;

namespace KyberCrystals;

public interface IPolyModStrategy
{ 
    public Polynomial PolyMod(Polynomial p, Polynomial mod);
}

public class LongPolynomialDivision : IPolyModStrategy 
{
    public Polynomial PolyMod(Polynomial p, Polynomial mod)
    {
        var nd = p.GetDegree();
        var dd = mod.GetDegree();

        return null;
    }
}

// TODO: if time permits.
public class SynteticDivision : IPolyModStrategy
{
    public Polynomial PolyMod(Polynomial p, Polynomial mod)
    {
        throw new NotImplementedException();
    }
}