package io.treehopper.libraries.sensors.inertial.lis3dh;

 enum SdoPuDiscs
{
    SdoPullUpDisconnected (144),
    SdoPullUpConnected (16);

int val;

SdoPuDiscs(int val) { this.val = val; }
public int getVal() { return val; }
}
