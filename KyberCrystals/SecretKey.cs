namespace KyberCrystals;

public class SecretKey
{
    private readonly string _skPrime;
    private readonly CpapkePublicKey _pk;
    private readonly byte[] _h;
    private readonly byte[] _z;
    
    public SecretKey(string skPrime, CpapkePublicKey pk, byte[] h, byte[] z)
    {
        _skPrime = skPrime;
        _pk = pk;
        _h = h;
        _z = z;
    }
    
    public (string, CpapkePublicKey , byte[], byte[]) UnpackSecretKey()
    {
        return (_skPrime, _pk, _h, _z);
    }
    
    public string GetCombinedString()
    {
        return _skPrime + _pk.GetCombinedString() + Utils.BytesToString(_h) + Utils.BytesToString(_z);
    }
}