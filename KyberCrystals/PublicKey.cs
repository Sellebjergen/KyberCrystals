namespace KyberCrystals;

public class PublicKey
{
    public string[] Test;
    public byte[] Rho;
    
    public PublicKey(string[] test, byte[] rho)
    {
        Test = test;
        Rho = rho;
    }
    
    public string GetCombinedString()
    {
        return String.Join("", Test) + Utils.BytesToString(Rho);
    }
}