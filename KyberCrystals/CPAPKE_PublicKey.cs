using System.Collections;

namespace KyberCrystals;

public class CPAPKE_PublicKey
{
    public string[] Test;
    public byte[] Rho;
    
    public CPAPKE_PublicKey(string[] test, byte[] rho)
    {
        Test = test;
        Rho = rho;
    }
    
    public string GetCombinedString()
    {
        return String.Join("", Test) + Utils.BytesToString(Rho);
    }
}