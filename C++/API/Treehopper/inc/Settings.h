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
		static Settings& getInstance()
		{
			static Settings instance;
			return instance;
		}

		bool throwExceptions;
		bool printExceptions;
		uint16_t vid;
		uint16_t pid;
		uint16_t bootloaderPid;
		uint16_t bootloaderVid;

	private:
		Settings()
		{
			throwExceptions = false;
			printExceptions = false;
			vid = 0x10c4;
			pid = 0x8a7e;
			bootloaderPid = 0xeac9;
			bootloaderVid = 0x10c4;
		}
	};
}

