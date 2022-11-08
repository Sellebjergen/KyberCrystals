namespace KyberCrystals;

public class SecretKey
{
    private readonly string _skPrime;
    private readonly CPAPKE_PublicKey _pk;
    private readonly byte[] _h;
    private readonly byte[] _z;
    
    public SecretKey(string skPrime, CPAPKE_PublicKey pk, byte[] h, byte[] z)
    {
        _skPrime = skPrime;
        _pk = pk;
        _h = h;
        _z = z;
    }
    
    public (string, CPAPKE_PublicKey , byte[], byte[]) UnpackSecretKey()
    {
        return (_skPrime, _pk, _h, _z);
    }
    
    public string GetCombinedString()
    {
        return _skPrime + _pk.GetCombinedString() + Utils.BytesToString(_h) + Utils.BytesToString(_z);;
    }
}