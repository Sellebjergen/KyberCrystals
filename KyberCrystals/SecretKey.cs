namespace KyberCrystals;

public class SecretKey
{
    private readonly byte[] _h;
    private readonly PublicKey _pk;
    private readonly string _skPrime;
    private readonly byte[] _z;

    public SecretKey(string skPrime, PublicKey pk, byte[] h, byte[] z)
    {
        _skPrime = skPrime;
        _pk = pk;
        _h = h;
        _z = z;
    }

    public (string skPrime, PublicKey pk, byte[] h, byte[] z) UnpackSecretKey()
    {
        return (_skPrime, _pk, _h, _z);
    }

    public string GetAsBinaryOutput()
    {
        return _skPrime + _pk.GetAsBinaryOutput() + Utils.BytesToBinaryString(_h) + Utils.BytesToBinaryString(_z);
    }

    public string GetAsHexString()
    {
        return Convert.ToHexString(Utils.GetBytes(GetAsBinaryOutput()));
    }

    public static SecretKey CreateFromHex(string hex, int k)
    {
        var bytes = Convert.FromHexString(hex);
        var offset = 0;

        if (k == 2) offset = 768; // kyber 512
        if (k == 3) offset = 1152; // kyber 768
        if (k == 4) offset = 1536; // kyber 1024

        var skPrime = new byte[offset];
        var pk = new byte[offset + 32];
        var hpk = new byte[32];
        var z = new byte[32];

        Array.Copy(bytes, 0, skPrime, 0, offset);
        Array.Copy(bytes, offset, pk, 0, offset + 32);
        Array.Copy(bytes, bytes.Length - 32 * 2, hpk, 0, 32);
        Array.Copy(bytes, bytes.Length - 32, z, 0, 32);

        return new SecretKey(
            Utils.BytesToBinaryString(skPrime),
            PublicKey.CreateFromHex(Convert.ToHexString(pk), k),
            hpk,
            z);
    }
}