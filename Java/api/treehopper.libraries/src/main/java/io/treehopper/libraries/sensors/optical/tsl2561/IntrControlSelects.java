/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.sensors.optical.tsl2561;

public enum IntrControlSelects
{
    InterruptOutputDisabled (0),
    LevelInterrupt (1),
    SMBAlertCompliant (2),
    TestMode (3);

int val;

IntrControlSelects(int val) { this.val = val; }
public int getVal() { return val; }
}
