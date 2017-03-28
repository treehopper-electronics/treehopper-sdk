#include "stdafx.h"
#include "Treehopper.Libraries/inc/Motors/SeeedGroveI2cMotorDriver.h"
#include <cmath>

namespace Treehopper {
	namespace Libraries {
		namespace Motors {

			SeeedGroveI2cMotorDriver::SeeedGroveI2cMotorDriver(I2c& i2c, bool a1, bool a2, bool a3, bool a4) : SeeedGroveI2cMotorDriver(i2c, (uint8_t)((a1 ? 1 : 0 << 3) | (a2 ? 1 : 0 << 2) | (a3 ? 1 : 0 << 1) | (a4 ? 1 : 0 << 0)))
			{

			}

			SeeedGroveI2cMotorDriver::SeeedGroveI2cMotorDriver(I2c& i2c, uint8_t address) : dev((uint8_t)(address), i2c)
			{
				freq = PrescalerFrequency::Freq_3921Hz;
			}

			void SeeedGroveI2cMotorDriver::frequency(PrescalerFrequency value)
			{
				freq = value;

				uint8_t data[2];
				data[0] = (uint8_t)freq;
				data[1] = 0x01;
				dev.writeBufferData((uint8_t)Registers::PwmFrequencySet, data, 2);
			}

			SeeedGroveI2cMotorDriver::PrescalerFrequency SeeedGroveI2cMotorDriver::frequency()
			{
				return freq;
			}

			double SeeedGroveI2cMotorDriver::motor1()
			{
				return m1;
			}

			double SeeedGroveI2cMotorDriver::motor2()
			{
				return m2;
			}

			void SeeedGroveI2cMotorDriver::motor1(double value)
			{
				m1 = value;
				update();
			}

			void SeeedGroveI2cMotorDriver::motor2(double value)
			{
				m2 = value;
				update();
			}

			void SeeedGroveI2cMotorDriver::update()
			{
				uint8_t data[2];
				data[0] = (uint8_t)round(abs(m1) * 255);
				data[1] = (uint8_t)round(abs(m2) * 255);
				dev.writeBufferData((uint8_t)Registers::MotorSpeedSet, data, 2);

				uint8_t dir[2];
				dir[1] = 0x01;

				if (m1 >= 0 && m2 >= 0)
					dir[0] = (uint8_t)MotorSetDirection::BothClockwise;
				else if (m1 >= 0 && m2 < 0)
					dir[0] = (uint8_t)MotorSetDirection::Motor1ClockwiseMotor2CounterClockwise;
				else if (m1 < 0 && m2 < 0)
					dir[0] = (uint8_t)MotorSetDirection::BothCounterClockwise;
				else if (m1 < 0 && m2 >= 0)
					dir[0] = (uint8_t)MotorSetDirection::Motor1CounterClockwiseMotor2Clockwise;
			
				dev.writeBufferData((uint8_t)Registers::DirectionSet, dir, 2);
			}
} } }