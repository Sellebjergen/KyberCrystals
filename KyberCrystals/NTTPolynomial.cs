using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<int>>;

namespace KyberCrystals;

public class NttPolynomial
{
    private Polynomial _originalPoly;
    
    public NttPolynomial(Polynomial p)
    {
        _originalPoly = p;
    }
}