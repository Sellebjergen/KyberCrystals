using KyberCrystals;
using Xunit;

namespace Tests;

public class KatsTests
{
    [Fact]
    public void Kyber512()
    {
        var lines = File.ReadAllLines("kats/PQCkemKAT_1632.rsp");
        for (var i = 0; i < 100; i++)
        {
            var seed = lines[3 + i * 7].Split(" = ")[1];
            var katPk = lines[4 + i * 7].Split(" = ")[1];
            var katSk = lines[5 + i * 7].Split(" = ")[1];
            var katCt = lines[6 + i * 7].Split(" = ")[1];
            var katSs = lines[7 + i * 7].Split(" = ")[1];
            
            var kyber = new Kyber(
                new ParameterGen().Kyber512(), 
                new PolynomialRing(3329, 256), 
                new AesCtrRng(Convert.FromHexString(seed)));

            var (pk, sk) = kyber.CCAKEM_keygen();
            Assert.Equal(katPk, GetHexFromBitString(pk.GetCombinedString()));
            Assert.Equal(katSk, GetHexFromBitString(sk.GetCombinedString()));

            var (ct, ss) = kyber.CCAKEM_encrypt(pk);
            Assert.Equal(katCt, GetHexFromBitString(ct.GetBinaryString()));
            Assert.Equal(katSs, GetHexFromBitString(ss));

            var ss2 = kyber.CCAKEM_decrypt(ct, sk);
            Assert.Equal(GetHexFromBitString(ss2), katSs);
            Assert.Equal(GetHexFromBitString(ss2), GetHexFromBitString(ss));
        }
    }
    
    [Fact]
    public void Kyber768()
    {
        var lines = File.ReadAllLines("kats/PQCkemKAT_2400.rsp");
        for (var i = 0; i < 100; i++)
        {
            var seed = lines[3 + i * 7].Split(" = ")[1];
            var katPk = lines[4 + i * 7].Split(" = ")[1];
            var katSk = lines[5 + i * 7].Split(" = ")[1];
            var katCt = lines[6 + i * 7].Split(" = ")[1];
            var katSs = lines[7 + i * 7].Split(" = ")[1];
            
            var kyber = new Kyber(
                new ParameterGen().Kyber768(), 
                new PolynomialRing(3329, 256), 
                new AesCtrRng(Convert.FromHexString(seed)));

            var (pk, sk) = kyber.CCAKEM_keygen();
            Assert.Equal(katPk, GetHexFromBitString(pk.GetCombinedString()));
            Assert.Equal(katSk, GetHexFromBitString(sk.GetCombinedString()));

            var (ct, ss) = kyber.CCAKEM_encrypt(pk);
            Assert.Equal(katCt, GetHexFromBitString(ct.GetBinaryString()));
            Assert.Equal(katSs, GetHexFromBitString(ss));

            var ss2 = kyber.CCAKEM_decrypt(ct, sk);
            Assert.Equal(GetHexFromBitString(ss2), katSs);
            Assert.Equal(GetHexFromBitString(ss2), GetHexFromBitString(ss));
        }
    }
    
    [Fact]
    public void Kyber1024()
    {
        var lines = File.ReadAllLines("kats/PQCkemKAT_3168.rsp");
        for (var i = 0; i < 100; i++)
        {
            var seed = lines[3 + i * 7].Split(" = ")[1];
            var katPk = lines[4 + i * 7].Split(" = ")[1];
            var katSk = lines[5 + i * 7].Split(" = ")[1];
            var katCt = lines[6 + i * 7].Split(" = ")[1];
            var katSs = lines[7 + i * 7].Split(" = ")[1];
            
            var kyber = new Kyber(
                new ParameterGen().Kyber1024(), 
                new PolynomialRing(3329, 256), 
                new AesCtrRng(Convert.FromHexString(seed)));

            var (pk, sk) = kyber.CCAKEM_keygen();
            Assert.Equal(katPk, GetHexFromBitString(pk.GetCombinedString()));
            Assert.Equal(katSk, GetHexFromBitString(sk.GetCombinedString()));

            var (ct, ss) = kyber.CCAKEM_encrypt(pk);
            Assert.Equal(katCt, GetHexFromBitString(ct.GetBinaryString()));
            Assert.Equal(katSs, GetHexFromBitString(ss));

            var ss2 = kyber.CCAKEM_decrypt(ct, sk);
            Assert.Equal(GetHexFromBitString(ss2), katSs);
            Assert.Equal(GetHexFromBitString(ss2), GetHexFromBitString(ss));
        }
    }
    
    # region helpers
    private string GetHexFromBitString(string bits)
    {
        var bytes = Utils.GetBytes(bits);
        return Convert.ToHexString(bytes);
    }
    # endregion
}
