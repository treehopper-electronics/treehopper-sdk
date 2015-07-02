#include "mex.h"
#include <libusb.h>
#include <stdio.h>
#include <TreehopperManager.h>
#include <TreehopperBoard.h>
#include <Pin.h>
#include <string>
#include <vector>

const char *field_names[] = { "name", "serial" };

mwSize dims[] = { 1, 0 };

TreehopperBoard* Board;

int boardOpen = 0;

using namespace std;

void Exit()
{
	delete Board;
}

void mexFunction(int nlhs, mxArray *plhs[], int nrhs, const mxArray *prhs[])
{
	if (nrhs > 0)
	{
		wstring command = wstring(mxGetChars(prhs[0]));
		if (command.compare(L"scan") == 0)
		{
			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			TreehopperManager manager;
			vector<TreehopperBoard>* boards = manager.ScanForDevices();
			int size = boards->size();
			dims[1] = size;
			plhs[0] = mxCreateStructArray(2, dims, 2, field_names);
			for (vector<TreehopperBoard>::size_type i = 0; i != size; i++)
			{
				mxSetFieldByNumber(plhs[0], i, 0, mxCreateString(boards->at(i).Name.c_str()));
				mxSetFieldByNumber(plhs[0], i, 1, mxCreateString(boards->at(i).SerialNumber.c_str()));
			}
			
		}
		else if (command.compare(L"open") == 0)
		{
			// start by closing existing boards.
			if (boardOpen)
				Board->Close();

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			// User is trying to open a specific device
			if (nrhs > 1)
			{
				if (!mxIsChar(prhs[1]))
					mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a string specifying the serial number of the device you wish to connect to");
				char buff[32];
				mxGetString(prhs[1], buff, 32);
				string serial = string(buff);
				Board = new TreehopperBoard(serial);
			}
			else 
			{
				// Just find the first one and open it.
				Board = new TreehopperBoard();
			}
			//mexAtExit(Exit);
			Board->Open();
			boardOpen = 1;
		}
		else if (command.compare(L"close") == 0)
		{
			Board->Close();
			Exit();
		}
		else if (command.compare(L"digitalIn") == 0)
		{

		}
		else if (command.compare(L"digitalOut") == 0)
		{
			if (nrhs < 2)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");

			double pinNumber = *((double *)mxGetData(prhs[1]));

			Pin pin = Pin(pinNumber, Board);
			pin.MakeDigitalOutput();
		}
		else if (command.compare(L"digitalWrite") == 0)
		{
			if (nrhs < 3)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");
			if (!mxIsLogical(prhs[2]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a logical specifying the pin value");

			double* pinNumber = (double *)mxGetData(prhs[1]);
			int* pinValue = (int *)mxGetLogicals(prhs[2]);
			Pin pin = Pin(*pinNumber, Board);
			pin.Value = *pinValue;
		}
		else if (command.compare(L"digitalRead") == 0)
		{

		}
		else if (command.compare(L"analogIn") == 0)
		{

		}
		else if (command.compare(L"analogRead") == 0)
		{

		}
	}

	mwSize dims[2] = { 1, 0 };
	
	unsigned char nameBuffer[32];

	//int numTreehoppers = 0;

	//if (numTreehoppers > 0)
	//{
	//	dims[1] = numTreehoppers;
	//	plhs[0] = mxCreateStructArray(2, (const mwSize*)dims, 2, field_names);

	//	libusb_get_string_descriptor_ascii(handle, 3, (unsigned char*)string, 128);
	//	mxSetFieldByNumber(plhs[0], j, 0, mxCreateString(string));
	//	libusb_get_string_descriptor_ascii(handle, 4, (unsigned char*)string, 128);
	//	mxSetFieldByNumber(plhs[0], j, 1, mxCreateString(string));
	//
	//}

	//libusb_free_device_list(devs, 1);

	//libusb_exit(NULL);
	//	return 0;
}