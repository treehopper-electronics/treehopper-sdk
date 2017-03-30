package io.treehopper;

/**
 * Utilities and helpers
 */
public class Utilities {
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
    static boolean CloseTo(double a, double b) {
        return CloseTo(a, b, 0.00001);
    }
}
