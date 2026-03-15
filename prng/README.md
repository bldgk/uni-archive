# Pseudo-Random Number Generators

Two PRNG algorithms with chi-squared goodness-of-fit testing (2014), ported to Python 3.

## Algorithms

**Linear Congruential Generator (LCG):** `x(n+1) = (K * x(n) + R) mod 2^degree`
- Classic PRNG, tested with RANDU parameters (K=65539) and glibc parameters (K=1103515245)

**Bit-Shuffle Generator:** splits number into IEEE 754 bits, rotates left/right, adds halves
- Experimental method using bit-level manipulation of float64 representation

**Chi-Squared Test:** verifies uniform distribution across 10 bins with 9 degrees of freedom at p=0.05

## Run

```bash
python3 prng.py
```

```
  Linear Congruential Generator (K=65539, R=0, mod 2^31)
  ──────────────────────────────────────────────────
    [0.0-0.1)  1002 █████████████████████████████
    [0.1-0.2)  1035 ██████████████████████████████
    ...
  Chi-squared: 6.9100  (critical: 16.919 at p=0.05)
  Result: PASS — uniform distribution

  Bit-Shuffle Generator (x0=3.14159)
  ──────────────────────────────────────────────────
    [0.8-0.9)  5537 ██████████████████████████████
    ...
  Chi-squared: 23957.37  (critical: 16.919 at p=0.05)
  Result: FAIL — not uniform
```
