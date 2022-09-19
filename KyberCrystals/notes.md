# Notes for the project
These are my notes for the PQP project regarding coding of the CRYSTALS-kyber algorithm.
The specification can be found on the following link https://pq-crystals.org/kyber/data/kyber-specification-round3-20210804.pdf.

## TODO:
* euclidian gcd function implementation???
* Read this seemingly good introduction to Kyber https://cryptopedia.dev/posts/kyber/
* (maybe) A bit of theory about how AES works and why that is quantum secure?
* Recovering the private key requires to solve the module-learning-with-errors problem
* KAT tests (found in the article)

## Questions for Diego
1. first
    * NTT, We need to discuss what this is exactly.
      * seems to be some kind of fourier transformation
    * How do I get my functions well tested?
      * It is not very easy to test the project and its different functions.
    * When using Shake256, does it make a difference if I update with a bytearray and then a byte? should I update it all just one array where the byte is concatenated?
    * I seems as if I have some kind of mistake when using bouncy castle shake implementation, as i Can't control the output length of the hash.
      * https://emn178.github.io/online-tools/shake_256.html
    * How to calculate A o s + e, we agree that is just normal matrix multiplication?

2. Second
   * We agree that it is only during multiplication we get a polynomia of a higher degree?
   * Just making sure that I understand polynomial rings correct
   * Seems like python and java implementation differ in zeta's calculated.
     * How are the zetas calculated? Maybe I need montgomery spaces?
   * Check the missing test in NttPolynomialTesting
   * I cannot make the NTT(Ntt_inv(polynomial)) function work correctly.
   * Where do I find the shake128 xof test vectors? (I haven't been able to find them again)
   * is x^17 in the ring x^17 + 1?

## Questions for Søren / Alex
* Usage of Get property in C#
* How do I organise unit tests? One test for Add, Mult etc and then a lot of theories? - seems to give bad naming as to what is wrong.

## number theoretic transform (NTT)
* https://www.nayuki.io/page/number-theoretic-transform-integer-dft
* https://codeforces.com/blog/entry/48798
* https://dsprenkels.com/ntt.html

### NTT-precomputing
* https://www.springerprofessional.de/en/preprocess-then-ntt-technique-and-its-applications-to-kyber-and-/16564234

## Montgomery reduction
* https://cryptography.fandom.com/wiki/Montgomery_reduction

## Existing implementations
* https://github.com/jack4818/kyber-py/

## People trying to use PQP encryption
* https://blog.dashlane.com/lets-get-ready-for-post-quantum-cryptography-in-dashlane/

## Polynomial division strategies
* https://flexbooks.ck12.org/cbook/ck-12-algebra-i-concepts-honors/section/7.13/primary/lesson/long-division-and-synthetic-division-alg-i-hnrs/

## Current plan for project
1. Make an implementation of the CRYSTALS-Kyber
2. See how it fits into DB structure
   * Performance test
   * Mock the HSM module
3. Code Dilithium
4. Look at security guarantees.
   * Maybe speak with katarina for this one.
