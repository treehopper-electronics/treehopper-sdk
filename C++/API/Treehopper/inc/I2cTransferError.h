#pragma once
#include "Treehopper.h"
namespace Treehopper 
{
	enum class TREEHOPPER_API I2cTransferError
	{
		ArbitrationLostError = 0,
		NackError = 1,
		UnknownError = 2,
		TxunderError = 3,
		Success = 255
	};
}
