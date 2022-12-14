namespace KyberCrystals;

public class PublicKey
{
    public readonly byte[] Rho;
    public readonly string[] Test;

    public PublicKey(string[] test, byte[] rho)
    {
        Test = test;
        Rho = rho;
    }

    public string GetAsBinaryOutput()
    {
        return string.Join("", Test) + Utils.BytesToBinaryString(Rho);
    }

    public string GetAsHexString()
    {
        return Convert.ToHexString(Utils.GetBytes(GetAsBinaryOutput()));
    }

    public static PublicKey CreateFromHex(string hex, int k)
    {
        var bytesPrime = Convert.FromHexString(hex);
        var bitsPrime = Utils.BytesToBinaryString(bytesPrime);
        var testBits = bitsPrime.Substring(0, bitsPrime.Length - 32 * 8);

        if (testBits.Length != k * 12 * 256)
            throw new ArgumentException("The parsed hex string, is invalid. Either being to long or to short!");

        var rho = new byte[32];
        Array.Copy(bytesPrime, bytesPrime.Length - 32, rho, 0, 32);

        var test = new string[k];
        for (var i = 0; i < k; i++) test[i] = testBits.Substring(i * 12 * 256, 12 * 256);

        return new PublicKey(test, rho);
    }
}