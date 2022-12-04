using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace KyberCrystals;

public interface IRng
{
    public byte[] GetRandomBytes(int amount);
}


public class StdRandom : IRng
{
    public byte[] GetRandomBytes(int amount)
    {
        return Utils.GetRandomBytes(32);
    }
}

public class CryptoRandom : IRng
{
    public byte[] GetRandomBytes(int amount)
    {
        var res = new byte[amount];

        var crypRng = RandomNumberGenerator.Create();
        crypRng.GetBytes(res);

        return res;
    }
}

public class AesCtrRng : IRng
{
    private byte[] _key;
    private byte[] _v;

    public AesCtrRng(byte[] seed)
    {
        _key = Utils.Get0Bytes(32);
        _v = Utils.Get0Bytes(16);
        
        CtrDrgbUpdate(seed);
    }
    
    public byte[] GetRandomBytes(int amount)
    {
        var tmp = new byte[48];
        var counter = 0;
        
        while (counter < amount)
        {
            IncrementCounter();
            var enc = EncryptAesEcb(_v, _key);
            Array.Copy(enc, 0, tmp, counter, enc.Length);
            counter += enc.Length;
        }
        
        CtrDrgbUpdate(Utils.Get0Bytes(48));
        var res = new byte[amount];
        Array.Copy(tmp, res, amount);
        
        return res;
    }

    private void IncrementCounter()
    {
        // prepending 00 to make sure the value does not become negative!
        var length = BigInteger.Parse("00" + Convert.ToHexString(_v), NumberStyles.HexNumber);
        var newLength = length + 1;
        var x = newLength.ToByteArray();
        Array.Reverse(x);
        
        if (x.Length == 17)             // removes the first 0 byte that has been introduced above.
            x = x.Skip(1).ToArray(); 
        
        if (x.Length != 16)
        {
            var newX = new byte[16];
            Array.Copy(x, 0, newX, 16 - x.Length, x.Length);
            _v = newX;
        }
        else
        {
            _v = x;
        }
    }

    private void CtrDrgbUpdate(byte[] data)
    {
        var tmp = new byte[48];
        var counter = 0;
        
        while (counter < 48)
        {
            IncrementCounter();
            var enc = EncryptAesEcb(_v, _key);
            Array.Copy(enc, 0, tmp, counter, enc.Length);
            counter += enc.Length;
        }

        var res = new byte[48];
        for (var i = 0; i < tmp.Length; i++)
        {
            res[i] = (byte) (tmp[i] ^ data[i]);
        }

        var key = new byte[32];
        var v = new byte[16];
        Array.Copy(res, 0, key, 0, 32);
        Array.Copy(res, key.Length, v, 0, 16);

        _key = key;
        _v = v;
    }

    public static byte[] EncryptAesEcb(byte[] plainText, byte[] key)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = Utils.Get0Bytes(16);
        
        return aesAlg.EncryptEcb(plainText, PaddingMode.None);
    }
}
