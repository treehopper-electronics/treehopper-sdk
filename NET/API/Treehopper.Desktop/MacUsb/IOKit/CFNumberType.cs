namespace Treehopper.Desktop.MacUsb.IOKit
{

	/// <summary>
	/// CF number types
	/// </summary>
	public enum CFNumberType
	{
		/* Fixed-width types */
		kCFNumberSInt8Type = 1,
		kCFNumberSInt16Type = 2,
		kCFNumberSInt32Type = 3,
		kCFNumberSInt64Type = 4,
		kCFNumberFloat32Type = 5,
		kCFNumberFloat64Type = 6,   /* 64-bit IEEE 754 */
									/* Basic C types */
		kCFNumberCharType = 7,
		kCFNumberShortType = 8,
		kCFNumberIntType = 9,
		kCFNumberLongType = 10,
		kCFNumberLongLongType = 11,
		kCFNumberFloatType = 12,
		kCFNumberDoubleType = 13,
		/* Other */
		kCFNumberCFIndexType = 14,
		//kCFNumberNSIntegerType CF_ENUM_AVAILABLE(10_5, 2_0) = 15,
		//kCFNumberCGFloatType CF_ENUM_AVAILABLE(10_5, 2_0) = 16,
		kCFNumberMaxType = 16
	};
}
