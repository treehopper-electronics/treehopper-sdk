// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the TREEHOPPERLIBRARIES_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// TREEHOPPERLIBRARIES_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef TREEHOPPERLIBRARIES_EXPORTS
#define LIBRARIES_API __declspec(dllexport)
#else
#define LIBRARIES_API __declspec(dllimport)
#endif