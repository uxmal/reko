#include <stdio.h>
#include "types.h"


class Item 
{
};

class ZContainer
{
public:
	typedef Item ** iterator;
	Item * item;
	Item ** begin() { return &item; }
	Item ** end() { return &item; }
};

int main(int arch, char * argv[])
{
	SampleClass c(argv[1]);
	int n = c.atoi() + c.cdecl_method() + c.stdcall_method();
	printf("Result: {0}\n", n);
	ZContainer * q = new ZContainer();
}
