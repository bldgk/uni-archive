# Numerical Methods

Classical numerical methods (2014), cleaned up. Python 3 + original Java implementations.

## Algorithms

**Root finding** — solving `ln(x+2) - x² = 0`:
- Bisection (dichotomy)
- Newton's method
- Simple iteration

**Numerical integration** — integrating `x/(3x+4)²` on [0,4]:
- Rectangle method
- Trapezoid method
- Simpson's rule
- Runge error estimation

**Interpolation** — interpolating `cos(x)`:
- Lagrange polynomial
- Newton's forward difference polynomial
- Error bound estimation

## Run

```bash
python3 root_finding.py
python3 integration.py
python3 interpolation.py
```

Java versions are in `java/` — compile with `javac *.java`, run with `java RootFinding` etc.
