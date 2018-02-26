package io.treehopper.libraries.sensors.magnetic;

import com.badlogic.gdx.math.Vector3;
import io.treehopper.libraries.sensors.IPollable;

public interface IMagnetometer extends IPollable {
    Vector3 getMagnetometer();
}
