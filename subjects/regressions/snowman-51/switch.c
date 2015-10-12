// switch.c
// Generated on 2015-10-12 10:10:03 by decompiling D:\dev\uxmal\reko\master\subjects\regressions\snowman-51\switch.dll
// using Decompiler version 0.5.4.0.

#include "switch.h"

char * get(int32 n)
{
	if (dwArg04 > ~0x01)
		return "other";
	else
		switch (dwArg04 + 0x01)
		{
		case 0x00:
			return "zero";
		case 0x01:
			return "one";
		case 0x02:
			return "two";
		case 0x03:
			return "three";
		}
}

void fn10071080()
{
	return;
}

