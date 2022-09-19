using System.Numerics;

namespace KyberCrystals;

public interface IPolynomial
{
    BigInteger GetCoefficient(int i);
    List<BigInteger> GetCoefficients();
    List<BigInteger> GetPaddedCoefficients(int amount);
    int GetLengthOfPolynomial();
    void RemoveTrailingZeros();
    bool IsZeroPolynomial();
}