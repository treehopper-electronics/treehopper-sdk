package io.treehopper.libraries.sensors.inertial;

import com.badlogic.gdx.math.Vector3;
import io.treehopper.libraries.sensors.Pollable;

/**
 * Accelerometer interface
 */
public interface IAccelerometer extends Pollable {
	Vector3 getAccelerometer();
}
