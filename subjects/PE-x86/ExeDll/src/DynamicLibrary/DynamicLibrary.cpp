// DynamicLibrary.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

// Global variables that are exported.

struct _RTL_CRITICAL_SECTION __declspec(dllexport) exported_critical_section;
int __declspec(dllexport) exported_int;

// Functions that are exported.

// Yes, this is not nearly thread safe, but it's just a test.
int __declspec(dllexport) slow_and_safe_increment(int delta)
{
	EnterCriticalSection(&exported_critical_section);
	exported_int += delta;
	LeaveCriticalSection(&exported_critical_section);
	return exported_int;
}
