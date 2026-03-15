/**
 * Interpolation: Lagrange and Newton polynomials.
 * Interpolates cos(x) on different node sets.
 */
public class Interpolation {

    static double f(double x) {
        return Math.cos(x);
    }

    static double lagrange(double x, double[] xi, double[] fi) {
        int n = xi.length - 1;
        double[] ft = fi.clone();
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n - i; j++) {
                ft[j] = (x - xi[j]) / (xi[i + j + 1] - xi[j]) * ft[j + 1]
                       + (x - xi[i + j + 1]) / (xi[j] - xi[i + j + 1]) * ft[j];
            }
        }
        return ft[0];
    }

    static double newton(double[] x, double[] y, double point) {
        int n = x.length;
        double h = x[1] - x[0];

        // Build finite difference table
        double[][] diff = new double[n][n];
        for (int i = 0; i < n; i++)
            diff[i][0] = y[i];
        for (int j = 1; j < n; j++)
            for (int i = 0; i < n - j; i++)
                diff[i][j] = diff[i + 1][j - 1] - diff[i][j - 1];

        // Find nearest node
        int idx = 0;
        for (int i = 0; i < n; i++)
            if (x[i] <= point) idx = i;

        double t = (point - x[idx]) / h;
        double result = diff[idx][0];
        double term = 1.0;
        for (int k = 1; k < n; k++) {
            term *= (t - (k - 1)) / k;
            result += term * diff[idx][k];
        }
        return result;
    }

    static double factorial(int n) {
        double r = 1;
        for (int i = 2; i <= n; i++) r *= i;
        return r;
    }

    static double errorBound(double[] xi, double point) {
        double max = 0;
        for (double x : xi)
            max = Math.max(max, Math.abs(f(x)));
        double err = max / factorial(xi.length);
        for (double x : xi)
            err *= Math.abs(point - x);
        return Math.abs(err);
    }

    public static void main(String[] args) {
        double pi = Math.PI;
        double check = pi / 4;
        double exact = f(check);

        System.out.println("Interpolating cos(x)");
        System.out.printf("Check point: pi/4 = %.6f, exact cos(pi/4) = %.10f%n%n", check, exact);

        double[][] nodes = {
            {0, pi/6, 2*pi/6, 3*pi/6},
            {0, pi/6, 5*pi/12, pi/2}
        };
        String[] labels = {"Uniform nodes", "Non-uniform nodes"};

        for (int k = 0; k < nodes.length; k++) {
            double[] X = nodes[k];
            double[] Y = new double[X.length];
            for (int i = 0; i < X.length; i++) Y[i] = f(X[i]);

            System.out.println("  " + labels[k] + ":");
            double lag = lagrange(check, X, Y);
            double newt = newton(X, Y, check);
            double bound = errorBound(X, check);

            System.out.printf("    Lagrange:    %.10f   error: %.2e%n", lag, Math.abs(lag - exact));
            System.out.printf("    Newton:      %.10f   error: %.2e%n", newt, Math.abs(newt - exact));
            System.out.printf("    Error bound: %.2e%n%n", bound);
        }
    }
}
