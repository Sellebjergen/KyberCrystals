# Crystals-kyber implementation

This is an implementation of the CRYSTALS-Kyber scheme based on version 3.02 of the 
[Kyber specification](https://pq-crystals.org/kyber/data/kyber-specification-round3-20210804.pdf).
The project is solely intended for me to learn about the Kyber scheme and the inner workings of 
it and should under no circumstance be used as a cryptographic tool for obtaining security of any kind.

# Example

An instance of Kyber can be run as by the following example

~~~csharp
// setting up Kyber and its parameters
var param = new ParameterGen().Kyber512();
var kyber = new Kyber(param, new PolynomialRing(param.Q, param.N));

// keygen, encrypt and decrypt
var (pk, sk) = kyber.Keygen();
var (c, kEnc) = kyber.Encrypt(pk);
var kDec = kyber.Decrypt(c, sk);
~~~

Where kEnc and kDec is the same key, shared by both parties (depending on who sends the ciphertext to who).
Instead of using the keygen function keys can also be created from and exported to hex strings by using the built in functions on the keys.
Note that the function needs to now the value of k from the parameters as this tells about how many polynomials are used in the kyber scheme.

~~~csharp
var pk = PublicKey.CreateFromHex(hex, param.K);
var sk = SecretKey.CreateFromHex(hex, param.K);
~~~

This should secure that keys can be hardcoded as hex strings in programs, which makes it easier for services using the same key multiple times.

# Performance

TODO

# Testing

Every part of the system is tested through a series of unittests. Take a look in the Test folder to see exactly what unittests have been made.
One thing to highlight is the Kat values which have been taken from the original implementation.
In the test project I have implemented my very own AES-ECB pseudo random function to make sure that the test values match with what is used for
the KAT values. This is necessary as there are multiple places in the Kyber scheme where randomness are used and if we cannot fix this randomness,
then the KAT tests would not pass.

The relevant code for this can be found in the Test project or for the Kyber512 parameter set here

~~~csharp
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

        var (pk, sk) = kyber.Keygen();
        Assert.Equal(katPk, GetHexFromBitString(pk.GetAsBinaryOutput()));
        Assert.Equal(katSk, GetHexFromBitString(sk.GetAsBinaryOutput()));

        var (ct, ss) = kyber.Encrypt(pk);
        Assert.Equal(katCt, GetHexFromBitString(ct.GetBinaryString()));
        Assert.Equal(katSs, Convert.ToHexString(ss));

        var ss2 = kyber.Decrypt(ct, sk);
        Assert.Equal(Convert.ToHexString(ss2), katSs);
        Assert.Equal(ss2, ss);
    }
}
~~~

Similar tests exists for Kyber768 and Kyber1024, take a look in the KatsTests.cs file to see those.

# Future optimizations. 

Csharp is not great at working on bytes and bits. To my knowledge there does not exist any nice way to using
bits which is not directly related to the size of a byte. Therefore I have made the choice to encode strings
a series of 1 and 0's in a string and using this as my bit strings.
On the very low level this means that I spend a whole char (8bits) to encode 1 bit of information.
For my project it has no implications, but this is something that one should note about it.

[]  Custom montgomery domain in the NTTPolynomialHelper.cs file\
[]  Coding Dilithium\
[]  Make all lists to arrays\
[]  Use shorts instead of bigintegers\
[]  Make use of csharp spans\
[]  Use a better function for concatenating bytes\
[]  Consider how all coefficients can be calculated and then moved into a list instead of moving them one by one.
