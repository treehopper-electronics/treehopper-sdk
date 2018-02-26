package io.treehopper.libraries.sensors.inertial;

import com.badlogic.gdx.math.Vector3;
import io.treehopper.libraries.sensors.IPollable;

/**
 * Accelerometer interface
 */
public interface IAccelerometer extends IPollable {
    Vector3 getAccelerometer();
}
