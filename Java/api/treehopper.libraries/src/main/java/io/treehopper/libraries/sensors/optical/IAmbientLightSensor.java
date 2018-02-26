package io.treehopper.libraries.sensors.optical;

import io.treehopper.libraries.sensors.IPollable;

public interface IAmbientLightSensor extends IPollable {
    double getLux();
}
