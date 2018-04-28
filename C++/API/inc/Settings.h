#pragma once
#include <stdint.h>
#include "Treehopper.h"
namespace Treehopper 
{
	class TREEHOPPER_API Settings
	{
	public:
		Settings(Settings const&) = delete;
		void operator=(Settings const&) = delete;
		static Settings& instance()
		{
			static Settings instance;
			return instance;
		}

		bool throwExceptions = false;
		bool printExceptions = true;
		uint16_t vid = 0x10c4;
		uint16_t pid = 0x8a7e;
		uint16_t bootloaderPid = 0xeac9;
		uint16_t bootloaderVid = 0x10c4;

	private:
		Settings() { }
	};
}

