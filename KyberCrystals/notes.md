# Notes for the project
These are my notes for the PQP project regarding coding of the CRYSTALS-kyber algorithm.
The specification can be found on the following link https://pq-crystals.org/kyber/data/kyber-specification-round3-20210804.pdf.

# Questions for Diego
* NTT, We need to discuss what this is exactly.
  * seems to be some kind of fourier transformation
* How do I get my functions well tested?
  * It is not very easy to test the project and its different functions.
* When using Shake256, does it make a difference if I update with a bytearray and then a byte? should I update it all just one array where the byte is concatenated?
* I seems as if I have some kind of mistake when using bouncy castle shake implementation, as i Can't control the output length of the hash.
  * https://emn178.github.io/online-tools/shake_256.html

## number theoretic transform (NTT)
* https://www.nayuki.io/page/number-theoretic-transform-integer-dft
* https://codeforces.com/blog/entry/48798

## Montgomery reduction
* https://cryptography.fandom.com/wiki/Montgomery_reduction

## Existing implementations
* https://github.com/jack4818/kyber-py/blob/20d549fee4e844a68a109a7d229c8bf09760ba38/polynomials.py#L5

## People trying to use PQP encryption
* https://blog.dashlane.com/lets-get-ready-for-post-quantum-cryptography-in-dashlane/
