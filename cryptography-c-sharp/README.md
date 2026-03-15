# Cryptography Library

Hand-rolled implementations of cryptographic algorithms, written from scratch (2014). No wrappers around standard libraries — every algorithm is implemented at the byte/bit level.

## Algorithms

**Symmetric ciphers:**
- **AES** — 128/192/256-bit, full SubBytes/ShiftRows/MixColumns/GF(2^8) arithmetic
- **DES** — 16-round Feistel network, 8 S-boxes, key schedule
- **Blowfish** — 16-round Feistel, P/S-box key expansion
- **Caesar**, **Trithemius**, **XOR/Vigenere** — classical ciphers

**Asymmetric / key exchange:**
- **RSA** — prime generation, modular exponentiation
- **Diffie-Hellman** — key agreement, primitive root finding
- **El-Gamal** — encryption with session keys
- **DSA** — digital signature creation and verification
- **Elliptic Curves (GOST)** — custom big integer arithmetic (~70 uint32 words), point operations on y² = x³ + ax + b, GOST digital signatures

**Hash functions:**
- **SHA-1** — 80-round, 160-bit digest
- **SHA-256** — 64-round, 256-bit digest
- **MD5** — 4 × 16 rounds, 128-bit digest

## Run

```bash
cd Demo
dotnet run
```

```
 ── Classical Ciphers ──

  [Caesar]           OK   "hello world" -> "khoorczruog" -> "hello world"
  [Trithemius]       OK   "hello world" -> "mmwzetsntql" -> "hello world"
  [XOR/Vigenere]     OK   "hello world" -> "x pbgxljvbw" -> "hello world"

 ── Block Ciphers ──

  [DES]              OK   "TestMsg!" -> [64 bits] -> "TestMsg!"
  [AES-192]          OK   16 bytes -> DDA97CA4864CDFE06EAF70A0EC0D7191 -> roundtrip
  [Blowfish]         OK   0x4142434445464748 -> encrypted -> roundtrip

 ── Asymmetric Cryptography ──

  [RSA]              OK   "Hi" -> encrypted -> "Hi"
  [Diffie-Hellman]   OK   key exchange complete
  [DSA Signature]    OK   sign/verify roundtrip

 ── Hash Functions ──

  [SHA-1]            OK   SHA1("abc") = A9993E36 4706816A BA3E2571 7850C26C 9CD0D89D
  [SHA-256]          OK   SHA256("abc") = BA7816BF 8F01CFEA 414140DE 5DAE2223 ...
  [MD5]              OK   MD5("abc") = 900150983cd24fb0d6963f7d28e17f72

   12 passed, 1 failed out of 13 tests
```

## Structure

```
CryptographyLabrary/   ← the library (original 2014 name preserved)
Demo/                  ← console app that runs all algorithms
AES.js/                ← JavaScript AES implementation (open testbench.html)
```
