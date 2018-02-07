package io.treehopper.libraries.sensors.inertial;

import com.badlogic.gdx.math.Vector3;
import io.treehopper.libraries.sensors.Pollable;

/**
 * Gyroscope interface
 */
public interface IGyroscope extends Pollable {
	Vector3 getGyroscope();
}
