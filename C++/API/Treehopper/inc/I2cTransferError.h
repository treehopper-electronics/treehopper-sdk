#pragma once
#include "Treehopper.h"
namespace Treehopper 
{
	/// <summary>
	/// Describes the transfer error, if not Success, that occured
	/// </summary>
	enum class I2cTransferError
	{
		/// <summary>
		/// Bus arbitration was lost
		/// </summary>
		ArbitrationLostError,

		/// <summary>
		/// The slave board failed to Nack back.
		/// </summary>
		NackError,

		/// <summary>
		/// Unknown error
		/// </summary>
		UnknownError,

		/// <summary>
		/// Tx buffer underrun error
		/// </summary>
		TxunderError,

		/// <summary>
		/// Successful transaction
		/// </summary>
		Success = 255
	};
}
