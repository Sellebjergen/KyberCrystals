namespace KyberCrystals;

public class ParameterGen
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
    
    public DilithiumParams DilithiumWeak()
    {
        return new DilithiumParams
        {
            Q = 8380417,
            D = 14,
            Wc = 60,
            Gamma1 = 523776,
            Gamma2 = 261888,
            K = 3,
            L = 2,
            Theta = 7,
            Beta = 375,
            Omega = 64
        };
    }
    
    public DilithiumParams DilithiumMedium()
    {
        return new DilithiumParams
        {
            Q = 8380417,
            D = 14,
            Wc = 60,
            Gamma1 = 523776,
            Gamma2 = 261888,
            K = 4,
            L = 3,
            Theta = 6,
            Beta = 325,
            Omega = 80
        };
    }
    
    public DilithiumParams DilithiumRecommended()
    {
        return new DilithiumParams
        {
            Q = 8380417,
            D = 14,
            Wc = 60,
            Gamma1 = 523776,
            Gamma2 = 261888,
            K = 5,
            L = 4,
            Theta = 5,
            Beta = 275,
            Omega = 96
        };
    }
    
    public DilithiumParams DilithiumHigh()
    {
        return new DilithiumParams
        {
            Q = 8380417,
            D = 14,
            Wc = 60,
            Gamma1 = 523776,
            Gamma2 = 261888,
            K = 6,
            L = 5,
            Theta = 3,
            Beta = 175,
            Omega = 120
        };
    }
}
