// Executable.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "../DynamicLibrary/DynamicLibrary.h"

int main()
{
	InitializeCriticalSection(&exported_critical_section);
	printf("%d\n", exported_int);
	printf("%d\n", slow_and_safe_increment(1));
	printf("%d\n", exported_int);
	return 0;
}

