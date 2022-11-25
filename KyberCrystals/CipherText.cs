namespace KyberCrystals;

public class CipherText
{
    public string[] C1 { get; }
    public string C2 { get; }

    public CipherText(string[] c1, string c2)
    {
        C1 = c1;
        C2 = c2;
    }
    
    public string GetBinaryString()
    {
        return string.Join("", C1) + C2;
    }
}