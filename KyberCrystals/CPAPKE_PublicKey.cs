namespace KyberCrystals;

public class CpapkePublicKey
{
    public string[] Test;
    public byte[] Rho;
    
    public CpapkePublicKey(string[] test, byte[] rho)
    {
        Test = test;
        Rho = rho;
    }
    
    public string GetCombinedString()
    {
        return String.Join("", Test) + Utils.BytesToString(Rho);
    }
}