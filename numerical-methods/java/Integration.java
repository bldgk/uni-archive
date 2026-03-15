/**
 * Numerical integration: rectangles, trapezoid, Simpson's rule.
 * Integrates f(x) = x / (3x+4)^2 on [0, 4].
 */
public class Integration {

    static double f(double x) {
        return x / ((3 * x + 4) * (3 * x + 4));
    }

    static double rectangles(double x0, double xk, double h) {
        double res = 0;
        double x = x0;
        while (x < xk) {
            res += f(x + h) * h;
            x += h;
        }
        return res;
    }

    static double trapezoid(double x0, double xk, double h) {
        double res = 0;
        double x = x0;
        int n = (int) ((xk - x0) / h);
        for (int i = 0; i < n; i++) {
            res += (f(x) + f(x + h)) * h / 2;
            x += h;
        }
        return res;
    }

    static double simpson(double x0, double xk, double h) {
        int n = (int) ((xk - x0) / h);
        double sum1 = 0, sum2 = 0;
        for (int i = 1; i < n; i += 2)
            sum1 += f(x0 + i * h);
        for (int i = 2; i < n; i += 2)
            sum2 += f(x0 + i * h);
        return (h / 3.0) * (f(x0) + f(xk) + 4 * sum1 + 2 * sum2);
    }

    static double runge(double r1, double r2, double p) {
        return r2 + (r2 - r1) / (Math.pow(2, p) - 1);
    }

    public static void main(String[] args) {
        double a = 0, b = 4.0, h1 = 1.0, h2 = 0.5;

        System.out.println("Integrating: x / (3x+4)^2 on [0, 4]\n");

        System.out.printf("Rectangles  h=%.1f: %.10f%n", h1, rectangles(a, b, h1));
        System.out.printf("Rectangles  h=%.1f: %.10f%n", h2, rectangles(a, b, h2));

        System.out.printf("Trapezoid   h=%.1f: %.10f%n", h1, trapezoid(a, b, h1));
        System.out.printf("Trapezoid   h=%.1f: %.10f%n", h2, trapezoid(a, b, h2));

        double s1 = simpson(a, b, h1);
        double s2 = simpson(a, b, h2);
        System.out.printf("Simpson     h=%.1f: %.10f%n", h1, s1);
        System.out.printf("Simpson     h=%.1f: %.10f%n", h2, s2);
        System.out.printf("Runge refinement:  %.10f%n", runge(s1, s2, 4));
    }
}
