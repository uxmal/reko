#include <stdlib.h>
#define DEFEXPORTS
#include "types.h"


int __declspec(dllexport) SampleClass::atoi()
{
	return ::atoi(this->data);
}

int __declspec(dllexport) __cdecl SampleClass::cdecl_method()
{
	return 1;
}

int __declspec(dllexport) __stdcall SampleClass::stdcall_method()
{
	return 10;
}