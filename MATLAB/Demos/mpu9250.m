import Treehopper.Libraries.Sensors.Inertial.*;

imu = Mpu6050(board.I2c);

dater = zeros(1000, 3);

for i=1:1000
    accel = imu.Accelerometer;
    dater(i, 1) = accel.X;
    dater(i, 2) = accel.Y;
    dater(i, 3) = accel.Z;
end

plot(dater);