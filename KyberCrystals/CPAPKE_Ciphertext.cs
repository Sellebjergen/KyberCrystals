namespace KyberCrystals;

public class CpapkeCiphertext
{
    public string[] C1 { get; set; }
    public string C2 { get; set; }

    public CpapkeCiphertext(string[] c1, string c2)
    {
        C1 = c1;
        C2 = c2;
    }
}