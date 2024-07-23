package io.treehopper;

/**
 * Utilities and helpers
 */
public class Utility {
    /**
     * Determines whether two numbers are close to each other
     * @param a the first number
     * @param b the second number
     * @param distance the distance to use to compare
     * @return
     */
    static boolean CloseTo(double a, double b, double distance) {
        if (Math.abs(a - b) < distance)
            return true;
        return false;
    }

    /**
     * Determines whether two numbers are close to each other
     * @param a the first number
     * @param b the second number
     * @return
     */
    public static boolean CloseTo(double a, double b) {
        return CloseTo(a, b, 0.00001);
    }

    static void error(String message) {
        error(message, false);
    }

    public static void error(String message, boolean throwErrors) {
        System.err.print(message);

        if (TreehopperUsb.Settings.shouldThrowExceptions()) {
            throw new RuntimeException(message);
        }
    }
}
