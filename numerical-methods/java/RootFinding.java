/**
 * Root finding: bisection, Newton's method, simple iteration.
 * Solves ln(x+2) - x^2 = 0.
 */
public class RootFinding {

    static double f(double x) {
        return Math.log(x + 2) - x * x;
    }

    static double df(double x) {
        return 1.0 / (x + 2) - 2 * x;
    }

    static double xf(double x) {
        return Math.sqrt(Math.log(x + 2));
    }

    static double bisection(double a, double b, double eps) {
        double c = 0;
        int iter = 0;
        while (Math.abs(b - a) > eps) {
            c = (a + b) / 2;
            if (f(b) * f(c) < 0)
                a = c;
            else
                b = c;
            iter++;
        }
        System.out.printf("  Bisection:        root = %.6f, f(root) = %.2e, iterations = %d%n", c, f(c), iter);
        return c;
    }

    static double newton(double x0, double eps) {
        double x = x0;
        int iter = 0;
        double t = f(x) / df(x);
        while (Math.abs(t) > eps) {
            t = f(x) / df(x);
            x -= t;
            iter++;
        }
        System.out.printf("  Newton:           root = %.6f, f(root) = %.2e, iterations = %d%n", x, f(x), iter);
        return x;
    }

    static double simpleIteration(double a, double b, double eps) {
        double x = (a + b) / 2;
        int iter = 0;
        double z;
        do {
            z = x;
            x = xf(x);
            iter++;
        } while (Math.abs(x - z) >= eps);
        System.out.printf("  Simple iteration: root = %.6f, f(root) = %.2e, iterations = %d%n", x, f(x), iter);
        return x;
    }

    public static void main(String[] args) {
        double eps = 1e-4;

        System.out.println("Solving: ln(x+2) - x^2 = 0\n");

        System.out.println("Interval [-1.99, 0]:");
        bisection(-1.99, -1e-8, eps);
        newton(-1.99, eps);

        System.out.println("\nInterval [0, 3]:");
        bisection(1e-7, 3, eps);
        newton(3, eps);
        simpleIteration(1e-7, 3, eps);
    }
}
