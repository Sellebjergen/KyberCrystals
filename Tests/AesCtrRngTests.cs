using KyberCrystals;
using Xunit;

namespace Tests;

public class AesCtrRngTests
{
    [Fact]
    public void TheFirstSeedCanBeObtainedFromEntropyInput()
    {
        var entropyInput = TestHelpers.GenerateEntropyInput();
        var rng = new AesCtrRng(entropyInput);

        // First seed in PQCkemKAT_1632.rsp
        var seed = "061550234D158C5EC95595FE04EF7A25767F2E24CC2BC479D09D86DC9ABCFDE7056A8C266F9EF97ED08541DBD2E1FFA1";

        Assert.Equal(Convert.FromHexString(seed), rng.GetRandomBytes(48));
    }

    [Fact]
    public void CheckingTheEcbAesEncryptionWorks()
    {
        var x = AesCtrRng.EncryptAesEcb(
            Convert.FromHexString("00000000000000000000000000000001"),
            Utils.Get0Bytes(32));

        Assert.Equal(Convert.ToHexString(x), "530f8afbc74536b9a963b4f1c4cb738b".ToUpper());
    }

    [Fact]
    public void CheckingThatAllSeedsCanBeMadeWithAesEcbMode_kyber512()
    {
        var lines = File.ReadAllLines("kats/PQCkemKAT_1632.rsp");
        var entropyInput = TestHelpers.GenerateEntropyInput();
        var rng = new AesCtrRng(entropyInput);

        var offset = 3;
        for (var i = 0; i < 100; i++)
        {
            var seed = lines[offset + 7 * i].Split(" = ")[1];
            Assert.Equal(seed, Convert.ToHexString(rng.GetRandomBytes(48)));
        }

        Assert.Equal(2, 2);
    }

    [Fact]
    public void CheckingThatAllSeedsCanBeMadeWithAesEcbMode_kyber768()
    {
        var lines = File.ReadAllLines("kats/PQCkemKAT_1632.rsp");
        var entropyInput = TestHelpers.GenerateEntropyInput();
        var rng = new AesCtrRng(entropyInput);

        var offset = 3;
        for (var i = 0; i < 100; i++)
        {
            var seed = lines[offset + 7 * i].Split(" = ")[1];
            Assert.Equal(seed, Convert.ToHexString(rng.GetRandomBytes(48)));
        }

        Assert.Equal(2, 2);
    }

    [Fact]
    public void CheckingThatAllSeedsCanBeMadeWithAesEcbMode_kyber1024()
    {
        var lines = File.ReadAllLines("kats/PQCkemKAT_1632.rsp");
        var entropyInput = TestHelpers.GenerateEntropyInput();
        var rng = new AesCtrRng(entropyInput);

        var offset = 3;
        for (var i = 0; i < 100; i++)
        {
            var seed = lines[offset + 7 * i].Split(" = ")[1];
            Assert.Equal(seed, Convert.ToHexString(rng.GetRandomBytes(48)));
        }

        Assert.Equal(2, 2);
    }
}