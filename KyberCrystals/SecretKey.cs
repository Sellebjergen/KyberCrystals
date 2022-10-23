namespace KyberCrystals;

public class SecretKey
{
    private readonly string _skPrime;
    private readonly string _pk;
    private readonly string _h;
    private readonly string _z;
    
    public SecretKey(string skPrime, string pk, string h, string z)
    {
        _skPrime = skPrime;
        _pk = pk;
        _h = h;
        _z = z;
    }
    
    public int GetTotalLength()
    {
        return _skPrime.Length + _pk.Length + _h.Length + _z.Length;
    }
    
    public (string, string, string, string) UnpackSecretKey()
    {
        return (_skPrime, _pk, _h, _z);
    }
}