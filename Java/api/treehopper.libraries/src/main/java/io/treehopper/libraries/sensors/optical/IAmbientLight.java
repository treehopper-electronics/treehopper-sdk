package io.treehopper.libraries.sensors.optical;

import io.treehopper.libraries.sensors.IPollable;

public interface IAmbientLight extends IPollable {
    double getLux();
}
