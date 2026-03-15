"""
Root finding methods for nonlinear equations.
Solves: ln(x+2) - x^2 = 0
"""
import math


def f(x):
    return math.log(x + 2) - x ** 2


def df(x):
    return 1 / (x + 2) - 2 * x


def xf(x):
    return math.sqrt(math.log(x + 2))


def bisection(a, b, eps=1e-4):
    """Bisection (dichotomy) method."""
    iterations = 0
    while abs(b - a) > eps:
        c = (a + b) / 2
        if f(b) * f(c) < 0:
            a = c
        else:
            b = c
        iterations += 1
    root = (a + b) / 2
    print(f"  Bisection:        root = {root:.6f}, f(root) = {f(root):.2e}, iterations = {iterations}")
    return root


def newton(x0, eps=1e-4):
    """Newton's method (tangent method)."""
    x = x0
    iterations = 0
    while True:
        t = f(x) / df(x)
        x -= t
        iterations += 1
        if abs(t) <= eps:
            break
    print(f"  Newton:           root = {x:.6f}, f(root) = {f(x):.2e}, iterations = {iterations}")
    return x


def simple_iteration(a, b, eps=1e-4):
    """Simple iteration method. x = sqrt(ln(x+2))."""
    x = (a + b) / 2
    iterations = 0
    while True:
        x_prev = x
        x = xf(x)
        iterations += 1
        if abs(x - x_prev) < eps:
            break
    print(f"  Simple iteration: root = {x:.6f}, f(root) = {f(x):.2e}, iterations = {iterations}")
    return x


if __name__ == "__main__":
    print("Solving: ln(x+2) - x^2 = 0\n")

    print("Interval [-1.99, 0]:")
    bisection(-1.99, -1e-8)
    newton(-1.99)
    print()

    print("Interval [0, 3]:")
    bisection(1e-7, 3)
    newton(3)
    simple_iteration(1e-7, 3)
