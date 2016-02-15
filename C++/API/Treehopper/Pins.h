#pragma once
#include "Pin.h"
#ifdef TREEHOPPER_EXPORTS
#define EXPORT __declspec(dllexport)
#else
#define EXPORT __declspec(dllimport)
#endif

class EXPORT Pin1 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin1(TreehopperBoard* board);
};

class EXPORT Pin2 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin2(TreehopperBoard* board);
};

class EXPORT Pin3 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin3(TreehopperBoard* board);
};

class EXPORT Pin4 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin4(TreehopperBoard* board);
};

class EXPORT Pin5 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin5(TreehopperBoard* board);
};

class EXPORT Pin6 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin6(TreehopperBoard* board);
};

class EXPORT Pin7 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin7(TreehopperBoard* board);
};

class EXPORT Pin8 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin8(TreehopperBoard* board);

};

class EXPORT Pin9 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin9(TreehopperBoard* board);
};

class EXPORT Pin10 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin10(TreehopperBoard* board);
};

class EXPORT Pin11 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin11(TreehopperBoard* board);
};

class EXPORT Pin12 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin12(TreehopperBoard* board);
};

class EXPORT Pin13 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin13(TreehopperBoard* board);
};

class EXPORT Pin14 : public Pin
{
	friend class TreehopperBoard;
public:
	Pin14(TreehopperBoard* board);
};
