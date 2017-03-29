package io.treehopper;

/**
 * Created by jay on 3/29/2017.
 */

public class Utilities {
    static boolean CloseTo(double a, double b, double distance)
    {
        if(Math.abs(a - b) < distance)
            return true;
        return false;
    }

    static boolean CloseTo(double a, double b)
    {
        return CloseTo(a, b, 0.00001);
    }
}
