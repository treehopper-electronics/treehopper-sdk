#pragma once

#include "I2c.h"
#include "Libraries/Treehopper.Libraries.h"
#include "SMBusDevice.h"

namespace Treehopper {
    namespace Libraries {
        namespace Motors {
            class LIBRARIES_API SeeedGroveI2cMotorDriver {
            public:
                enum class Registers {
                    MotorSpeedSet = 0x82,
                    PwmFrequencySet = 0x84,
                    DirectionSet = 0xaa,
                    MotorSetA = 0xa1,
                    MotorSetB = 0xa5,
                    Nothing = 0x01
                };

                enum class MotorSetDirection {
                    BothClockwise = 0x0a,
                    BothCounterClockwise = 0x05,
                    Motor1ClockwiseMotor2CounterClockwise = 0x06,
                    Motor1CounterClockwiseMotor2Clockwise = 0x09
                };

                /// <summary>
                /// The prescaler frequencies
                /// </summary>
                enum class PrescalerFrequency {
                    Freq_31372Hz = 0x01,
                    Freq_3921Hz = 0x02,
                    Freq_490Hz = 0x03,
                    Freq_122Hz = 0x04,
                    Freq_30Hz = 0x05
                };

                /// <summary>
                /// Construct a new Seeed Grove I2c Motor Driver with the given address DIP switch pins
                /// </summary>
                /// <param name="i2c">I2c module to use</param>
                /// <param name="a1">A1</param>
                /// <param name="a2">A2</param>
                /// <param name="a3">A3</param>
                /// <param name="a4">A4</param>
                SeeedGroveI2cMotorDriver(I2c &i2c, bool a1, bool a2, bool a3, bool a4);

                /// <summary>
                /// Construct a new Seeed Grove I2c Motor Driver with a given address
                /// </summary>
                /// <param name="i2c">I2c module to use</param>
                /// <param name="address">The address to use</param>
                SeeedGroveI2cMotorDriver(I2c &i2c, uint8_t address = 0x0f);

                /** Get the frequency **/
                PrescalerFrequency frequency();

                /** Set the frequency **/
                void frequency(PrescalerFrequency value);

                /** Get the Motor1 speed **/
                double motor1();

                /** Set the Motor1 speed **/
                void motor1(double speed);

                /** Get the Motor2 speed **/
                double motor2();

                /** Set the Motor2 speed **/
                void motor2(double speed);

            private:
                double m1 = 0;
                double m2 = 0;
                PrescalerFrequency freq;
                SMBusDevice dev;

                void update();
            };
        }
    }
}