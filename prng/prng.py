"""
Pseudo-Random Number Generators with Chi-Squared Goodness-of-Fit Test.

Two PRNG algorithms:
  1. Linear Congruential Generator (LCG): x = (K*x + R) mod 2^degree
  2. Bit-Shuffle Generator: split number into bits, shift left/right, add halves

Both are tested for uniformity using the chi-squared statistic.

Run: python3 prng.py
"""
import math
import struct


# ── Linear Congruential Generator ───────────────────────

def lcg(x0, k, r, degree, count):
    """Generate numbers using x(n+1) = (K * x(n) + R) mod 2^degree."""
    m = 2 ** degree
    x = x0
    numbers = []
    for _ in range(count):
        x = (k * x + r) % m
        numbers.append(x / m)  # normalize to [0, 1)
    return numbers


# ── Bit-Shuffle Generator ──────────────────────────────

def _rotate_left(bits, n, width=64):
    """Circular left rotation of an integer."""
    n %= width
    return ((bits << n) | (bits >> (width - n))) & ((1 << width) - 1)


def _rotate_right(bits, n, width=64):
    """Circular right rotation of an integer."""
    n %= width
    return ((bits >> n) | (bits << (width - n))) & ((1 << width) - 1)


def _double_to_bits(x):
    """Convert float64 to 64-bit integer representation."""
    return struct.unpack('Q', struct.pack('d', x))[0]


def _bits_to_double(b):
    """Convert 64-bit integer back to float64."""
    return struct.unpack('d', struct.pack('Q', b))[0]


def shuffle_method(x):
    """One step of the bit-shuffle generator."""
    bits = _double_to_bits(x)
    left = _rotate_left(bits, 2)
    right = _rotate_right(bits, 2)
    new_bits = (left + right) & 0xFFFFFFFFFFFFFFFF
    result = _bits_to_double(new_bits)
    if math.isnan(result) or math.isinf(result):
        return abs(x * 0.999 + 0.001)
    return abs(result)


def shuffle_gen(x0, count):
    """Generate numbers using the bit-shuffle method."""
    x = x0
    numbers = []
    for _ in range(count):
        x = shuffle_method(x)
        val = abs(math.cos(math.log10(x + 1e-10)))
        numbers.append(val)
    return numbers


# ── Chi-Squared Test ───────────────────────────────────

def chi_squared_test(numbers, bins=10):
    """
    Chi-squared goodness-of-fit test for uniformity on [0, 1).
    Returns (chi2 statistic, frequencies per bin, critical value at p=0.05).
    """
    n = len(numbers)
    expected = n / bins
    freq = [0] * bins
    for x in numbers:
        idx = min(int(x * bins), bins - 1)
        freq[idx] += 1

    chi2 = sum((f - expected) ** 2 / expected for f in freq)
    # Critical value for 9 degrees of freedom at p=0.05
    critical = 16.919
    return chi2, freq, critical


# ── Display ────────────────────────────────────────────

def print_result(name, numbers):
    chi2, freq, critical = chi_squared_test(numbers)
    uniform = chi2 < critical

    print(f"\n  {name}")
    print(f"  {'─' * 50}")
    print(f"  First 10 values: {', '.join(f'{x:.4f}' for x in numbers[:10])}")
    print(f"  Frequency distribution (10 bins):")
    bar_max = max(freq)
    for i, f in enumerate(freq):
        bar = '█' * int(f / bar_max * 30)
        print(f"    [{i/10:.1f}-{(i+1)/10:.1f}) {f:5d} {bar}")
    print(f"  Chi-squared: {chi2:.4f}  (critical: {critical} at p=0.05)")
    print(f"  Result: {'PASS — uniform distribution' if uniform else 'FAIL — not uniform'}")


# ── Main ───────────────────────────────────────────────

if __name__ == "__main__":
    N = 10000

    print("╔══════════════════════════════════════════════════╗")
    print("║   Pseudo-Random Number Generators + Chi² Test   ║")
    print("╚══════════════════════════════════════════════════╝")

    # LCG with common parameters
    lcg_numbers = lcg(x0=7, k=65539, r=0, degree=31, count=N)
    print_result("Linear Congruential Generator (K=65539, R=0, mod 2^31)", lcg_numbers)

    # Better LCG parameters
    lcg_numbers2 = lcg(x0=12345, k=1103515245, r=12345, degree=31, count=N)
    print_result("LCG (glibc parameters: K=1103515245, R=12345, mod 2^31)", lcg_numbers2)

    # Bit-shuffle
    shuffle_numbers = shuffle_gen(x0=3.14159, count=N)
    print_result("Bit-Shuffle Generator (x0=3.14159)", shuffle_numbers)
