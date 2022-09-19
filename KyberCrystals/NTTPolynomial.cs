using System.Numerics;

namespace KyberCrystals;

public class NttPolynomial
{
    private const int Zeta = 17;
    private readonly PolynomialRing _rq;
    private readonly List<Polynomial> _modPolynomials;
    private readonly List<Polynomial> _polynomials;
    
    public NttPolynomial(PolynomialRing rq, Polynomial f)
    {
        _rq = rq;
        _modPolynomials = CreateModPolynomials();
        _polynomials = CreateNttForm(f);
    }

    public List<Polynomial> GetNttMembers() // review: C# style?
    {
        return _polynomials;
    }

    public List<Polynomial> GetNttModulus() // review: This does not have to be public, but I am unsure how to test it.
    {
        return _modPolynomials;
    }

    private List<Polynomial> CreateNttForm(Polynomial f)
    {
        return _modPolynomials.Select(mod => _rq.ModPoly(f, mod)).ToList();
    }
    
    private static int Br7(int i)
    {
        if (i is < 0 or > 127) throw new ArgumentException("Bit reversal is only defined for 0...127");

        var bits = BitConverter.GetBytes(i).First();
        var reversal = BitConverter.GetBytes(255).First(); 

        return bits ^ reversal;
    }

    private List<Polynomial> CreateModPolynomials()
    {
        var res = new List<Polynomial>();
        var amountOfPolynomials = (int) Math.Pow(2, Math.Log2((int)_rq.N));
        // TODO: this conversion is not necessary, n is always 256 or less. Can always be described by an int at least.
        for (var i = 0; i < amountOfPolynomials; i++) 
        {
            var zetaMath = (int) Math.Pow(Zeta, 2 * Br7(i) + 1) % _rq.Q;
            res.Add(new Polynomial(new List<BigInteger> { -zetaMath, 0, 1 })); // zeta + x^2
        }

        return res;
    }
}