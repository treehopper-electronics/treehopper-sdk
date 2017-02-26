#pragma once
namespace Treehopper {
	/** 
	Defines the clock phase and polarity used for SPI communication.
	
	The left number indicates the clock polarity (CPOL), while the right number indicates the clock phase (CPHA). Consult https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus#Clock_polarity_and_phase for more information.

	Note that the numeric values of this enum do not match the standard nomenclature, but instead match the value needed by Treehopper's MCU. Do not attempt to cast integers from/to this enum.
	*/
	enum class SpiMode
	{
		/// <summary>
		/// Clock is initially low; data is valid on the rising edge of the clock
		/// </summary>
		Mode00 = 0x00,

		/// <summary>
		/// Clock is initially low; data is valid on the falling edge of the clock
		/// </summary>
		Mode01 = 0x20,

		/// <summary>
		/// Clock is initially high; data is valid on the rising edge of the clock
		/// </summary>
		Mode10 = 0x10,

		/// <summary>
		/// Clock is initially high; data is valid on the falling edge of the clock
		/// </summary>
		Mode11 = 0x30
	};
}