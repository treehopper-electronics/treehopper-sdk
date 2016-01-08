#define TREEHOPPER_STATIC_LINK // must be put above the Treehopper.h include line

#include "mex.h"
#include <libusb.h>
#include <stdio.h>
#include <TreehopperManager.h>
#include <TreehopperBoard.h>
#include <Pwm.h>
#include <Pin.h>
#include <string>
#include <vector>

extern "C" { FILE __imp__iob[3] = { *stdin,*stdout,*stderr }; }

const char *field_names[] = { "name", "serial" };

mwSize dims[] = { 1, 0 };

TreehopperBoard* Board;

int boardOpen = 0;

using namespace std;

static void Exit()
{
	if (Board != NULL)
	{
		if (Board->IsOpen)
		{
			Board->Close();
			delete Board;
			Board = NULL;
		}

	}
}

void mexFunction(int nlhs, mxArray *plhs[], int nrhs, const mxArray *prhs[])
{
	mexAtExit(Exit);
	if (nrhs > 0)
	{
		//wstring command = wstring((wchar_t*)mxGetChars(prhs[0]));
		char buff[32];
		mxGetString(prhs[0], buff, 32);

		string command = string(buff);
		
		if (command.compare("scan") == 0)
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
		else if (command.compare("open") == 0)
		{
			// start by closing existing boards.
			if (Board != NULL)
			{
				if (Board->IsOpen)
					Board->Close();
			}


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
		}
		else if (command.compare("close") == 0)
		{
			Exit();
		}
		else if (command.compare("makeDigitalIn") == 0)
		{
			if (nrhs < 2)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");

			double pinNumber = *((double *)mxGetData(prhs[1]));
			int pinNumInt = (int)pinNumber;
			Board->Pins[pinNumInt - 1]->MakeDigitalInput();

		}
		else if (command.compare("makeDigitalOut") == 0)
		{
			if (nrhs < 2)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");

			double pinNumber = *((double *)mxGetData(prhs[1]));
			int pinNumInt = (int)pinNumber;
			Board->Pins[pinNumInt - 1]->MakeDigitalOutput();

		}
		else if (command.compare("digitalWrite") == 0)
		{
			if (nrhs < 3)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");
			if (!mxIsLogical(prhs[2]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a logical specifying the pin value");

			
			double pinNumber = *((double *)mxGetData(prhs[1]));
			int pinValue = *((int *)mxGetLogicals(prhs[2]));
			int pinNumInt = (int)pinNumber;
			Board->Pins[pinNumInt - 1]->DigitalValue = pinValue;
		}
		else if (command.compare("digitalRead") == 0)
		{
			if (nrhs < 2)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "You may only specify one output argument.");

			

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");

			double pinNumber = *((double *)mxGetData(prhs[1]));
			int pinNumInt = (int)pinNumber;
			plhs[0] = mxCreateDoubleScalar(Board->Pins[pinNumInt - 1]->DigitalValue);
		}
		else if (command.compare("makeAnalogIn") == 0)
		{
			if (nrhs < 2)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");

			double pinNumber = *((double *)mxGetData(prhs[1]));
			int pinNumInt = (int)pinNumber;
			Board->Pins[pinNumInt - 1]->MakeAnalogInput();
		}

		else if (command.compare("analogReadValue") == 0)
		{
			if (nrhs < 2)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "You may only specify one output argument.");



			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");

			double pinNumber = *((double *)mxGetData(prhs[1]));
			int pinNumInt = (int)pinNumber;
			plhs[0] = mxCreateDoubleScalar(Board->Pins[pinNumInt - 1]->AnalogValue);

		}

		else if (command.compare("analogReadVoltage") == 0)
		{
			if (nrhs < 2)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			//if (nlhs != 1)
			//	mexErrMsgIdAndTxt("Treehopper:WrongNumberOutputArguments", "You must specify one output argument.");
			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "You may only specify one output argument.");


			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");

			double pinNumber = *((double *)mxGetData(prhs[1]));
			int pinNumInt = (int)pinNumber;
			plhs[0] = mxCreateDoubleScalar(Board->Pins[pinNumInt - 1]->AnalogVoltage);

		}

		else if (command.compare("makePwm") == 0)
		{
			if (nrhs < 2)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");

			double pinNumber = *((double *)mxGetData(prhs[1]));

			int pinNumInt = (int)pinNumber;
			Pwm pwm = Pwm((Board->Pins[pinNumInt - 1]));
			pwm.IsEnabled = true;

		}

		else if (command.compare("pwmWrite") == 0)
		{
			if (nrhs < 3)
				mexErrMsgIdAndTxt("Treehopper:NotEnoughInputArguments", "This command requires an additional input argument.");

			if (nlhs > 1)
				mexErrMsgIdAndTxt("Treehopper:TooManyOutputArguments", "Too many output arguments. This command only returns a single output value.");

			if (!mxIsNumeric(prhs[1]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the desired pin");
			if (!mxIsNumeric(prhs[2]))
				mexErrMsgIdAndTxt("Treehopper:InvalidInputArgument", "Input should be a number specifying the pwm value");

			double pinNumber = *((double *)mxGetData(prhs[1]));
			double pwmValue = *((double *)mxGetData(prhs[2]));

			int pinNumInt = (int)pinNumber;
			Pwm pwm = Pwm((Board->Pins[pinNumInt - 1]));
			pwm.IsEnabled = true;
			pwm.DutyCycle = pwmValue;
			

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