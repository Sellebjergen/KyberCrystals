namespace KyberCrystals;

public class Constants
{
    // Defined in the Kyber specification paper.
    public KyberParams Kyber512()
    {
        return new KyberParams
        {
            N = 256,
            K = 2,
            Q = 3329,
            Eta1 = 3,
            Eta2 = 2,
            Du = 10,
            Dv = 4
        };
    }

    public KyberParams Kyber768()
    {
        return new KyberParams
        {
            N = 256,
            K = 3,
            Q = 3329,
            Eta1 = 2,
            Eta2 = 2,
            Du = 10,
            Dv = 4
        };
    }
    
    public KyberParams Kyber1024()
    {
        return new KyberParams
        {
            N = 256,
            K = 4,
            Q = 3329,
            Eta1 = 2,
            Eta2 = 2,
            Du = 11,
            Dv = 5
        };
    }
}
