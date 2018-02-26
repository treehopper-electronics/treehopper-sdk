#pragma once

#include <vector>
#include <SMBusDevice.h>

namespace Treehopper {
	namespace Libraries {
		class Register;

		class RegisterManager
		{
		public:
			RegisterManager(SMBusDevice& dev, bool multiRegisterAccess);
			void write(Register& reg);
			void writeRange(Register& start, Register& end);
            void read(Register& reg);
            void readRange(Register& start, Register& end);
		protected:
			std::vector<Register*> registers;
		private:
			SMBusDevice & dev;
            bool multiRegisterAccess;
		};
	}
}