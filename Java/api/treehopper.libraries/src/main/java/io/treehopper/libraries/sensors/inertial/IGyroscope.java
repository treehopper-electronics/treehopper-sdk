package io.treehopper.libraries.sensors.inertial;

import com.badlogic.gdx.math.Vector3;
import io.treehopper.libraries.sensors.IPollable;

/**
 * Gyroscope interface
 */
public interface IGyroscope extends IPollable {
    Vector3 getGyroscope();
}
