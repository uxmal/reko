// switch.c
// Generated on 2015-10-07 10:30:08 by decompiling D:\dev\uxmal\reko\master\subjects\regressions\snowman-51\switch.dll
// using Decompiler version 0.5.4.0.

#include "switch.h"

char * get(int32 n)
{
	if (dwArg04 > ~0x01)
		return &globals->b10072000;
	else
		switch (dwArg04 + 0x01)
		{
		case 0x00:
			return &globals->b10072018;
		case 0x01:
			return &globals->b10072014;
		case 0x02:
			return &globals->b10072010;
		case 0x03:
			return &globals->b10072008;
		}
}

void fn10071080()
{
	return;
}

