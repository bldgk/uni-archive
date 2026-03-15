"""
Interpolation methods.
Interpolates cos(x) using Lagrange and Newton polynomials.
"""
import math


def f(x):
    return math.cos(x)


def lagrange(x, xi, fi):
    """Lagrange interpolation polynomial (Aitken's scheme)."""
    n = len(xi) - 1
    ft = list(fi)
    for i in range(n):
        for j in range(n - i):
            ft[j] = ((x - xi[j]) / (xi[i + j + 1] - xi[j]) * ft[j + 1]
                      + (x - xi[i + j + 1]) / (xi[j] - xi[i + j + 1]) * ft[j])
    return ft[0]


def newton(x, xi, yi):
    """Newton's forward difference interpolation polynomial."""
    n = len(xi)
    h = xi[1] - xi[0]

    # Build finite difference table
    diff = [[0.0] * n for _ in range(n)]
    for i in range(n):
        diff[i][0] = yi[i]
    for j in range(1, n):
        for i in range(n - j):
            diff[i][j] = diff[i + 1][j - 1] - diff[i][j - 1]

    # Find nearest point
    idx = 0
    for i in range(n):
        if xi[i] <= x:
            idx = i

    t = (x - xi[idx]) / h
    result = diff[idx][0]
    term = 1.0
    for k in range(1, n):
        term *= (t - (k - 1)) / k
        result += term * diff[idx][k]
    return result


def error_bound(xi, check_point):
    """Upper bound of interpolation error using 4th derivative."""
    # For cos(x), the 4th derivative is cos(x)
    max_deriv = max(abs(math.cos(x)) for x in xi)
    err = max_deriv / math.factorial(len(xi))
    for xj in xi:
        err *= abs(check_point - xj)
    return abs(err)


if __name__ == "__main__":
    print("Interpolating cos(x)\n")
    pi = math.pi

    datasets = [
        ("Uniform nodes",     [0, pi/6, 2*pi/6, 3*pi/6]),
        ("Non-uniform nodes", [0, pi/6, 5*pi/12, pi/2]),
    ]

    check = pi / 4
    exact = f(check)
    print(f"Check point: x = pi/4 = {check:.6f}")
    print(f"Exact cos(pi/4) = {exact:.10f}\n")

    for label, X in datasets:
        Y = [f(x) for x in X]
        nodes = ", ".join(f"{x:.4f}" for x in X)
        print(f"  {label}: [{nodes}]")

        lag = lagrange(check, X, Y)
        newt = newton(check, X, Y)
        bound = error_bound(X, check)

        print(f"    Lagrange:    {lag:.10f}   error: {abs(lag - exact):.2e}")
        print(f"    Newton:      {newt:.10f}   error: {abs(newt - exact):.2e}")
        print(f"    Error bound: {bound:.2e}")
        print()
