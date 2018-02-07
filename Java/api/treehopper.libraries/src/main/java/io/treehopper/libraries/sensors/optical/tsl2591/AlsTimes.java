package io.treehopper.libraries.sensors.optical.tsl2591;

public enum AlsTimes
{
    Time_100ms (0),
    Time_200ms (1),
    Time_300ms (2),
    Time_400ms (3),
    Time_500ms (4),
    Time_600ms (5);

int val;

AlsTimes(int val) { this.val = val; }
public int getVal() { return val; }
}
