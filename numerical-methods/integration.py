"""
Numerical integration methods.
Integrates: f(x) = x / (3x+4)^2 on [0, 4]
"""
import math


def f(x):
    return x / (3 * x + 4) ** 2


def rectangles(f, a, b, h):
    """Rectangle (midpoint) method."""
    result = 0
    x = a
    while x < b:
        result += f(x + h) * h
        x += h
    return result


def trapezoid(f, a, b, h):
    """Trapezoid method."""
    n = int((b - a) / h)
    result = 0
    x = a
    for _ in range(n):
        result += (f(x) + f(x + h)) * h / 2
        x += h
    return result


def simpson(f, a, b, h):
    """Simpson's rule (parabolic method)."""
    n = int((b - a) / h)
    sum_odd = sum(f(a + i * h) for i in range(1, n, 2))
    sum_even = sum(f(a + i * h) for i in range(2, n, 2))
    return (h / 3) * (f(a) + f(b) + 4 * sum_odd + 2 * sum_even)


def runge(r1, r2, p):
    """Runge's rule for error estimation."""
    return r2 + (r2 - r1) / (2 ** p - 1)


if __name__ == "__main__":
    a, b = 0, 4.0
    h1, h2 = 1.0, 0.5

    # Analytical solution: integral of x/(3x+4)^2 dx from 0 to 4
    # = [1/9 * (ln|3x+4| + 4/(3x+4))] from 0 to 4
    exact = (1/9) * (math.log(16) + 4/16) - (1/9) * (math.log(4) + 4/4)

    print(f"Integrating: x / (3x+4)^2 on [{a}, {b}]")
    print(f"Exact value: {exact:.10f}\n")

    for name, method in [("Rectangles", rectangles), ("Trapezoid", trapezoid), ("Simpson", simpson)]:
        r1 = method(f, a, b, h1)
        r2 = method(f, a, b, h2)
        p = 2 if name != "Simpson" else 4
        refined = runge(r1, r2, p)
        print(f"  {name:12s}  h={h1}: {r1:.10f}   h={h2}: {r2:.10f}   Runge: {refined:.10f}   error: {abs(refined - exact):.2e}")
