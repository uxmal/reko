// TestReko.c
// Generated on 2015-08-06 21:12:11 by decompiling D:\dev\jkl\reko\master\subjects\regressions\reko-15\TestReko.exe
// using Decompiler version 0.5.1.0.

#include "TestReko.h"

void fn00401000()
{
	return;
}

void fn0040127B(word32 ebx, word32 esi, word32 edi)
{
	fn00401561();
	Eq_5 * ebp_10 = fn00401770(ebx, esi, edi, dwLoc0C, 4202984, 0x0C);
	ebp_10->dw0000 = 0;
	Eq_25 * esp_105 = fp - 8;
	word32 edx_17 = fs->ptr0018->dw0004;
	word32 edi_18 = 0;
	do
	{
		__lock();
		word32 eax_24;
		__cmpxchg(globals->dw403368, edx_17, 0, out eax_24);
		if (eax_24 == 0)
			goto l00401151;
	} while (eax_24 != edx_17);
	edi_18 = 1;
l00401151:
	if (globals->dw40336C == 1)
	{
		Mem185[fp - 0x0C:word32] = 31;
		_amsg_exit();
		esp_105 = fp + 0xFFFFFFF4;
l0040119D:
		if (globals->dw40336C == 1)
		{
			Eq_25 * esp_174 = esp_105 - 4;
			esp_174->dw0000 = 4202652;
			esp_174->dw0000 = 4202644;
			_initterm();
			globals->dw40336C = 2;
			esp_105 = esp_174;
		}
		if (edi_18 == 0)
			globals->dw403368 = 0;
		if (globals->ptr403378 != null)
		{
			Eq_167 * esp_152 = esp_105 - 4;
			esp_152->dw0000 = 4207480;
			word32 eax_154 = fn00401470(dwArg00);
			esp_105 = esp_152 + 4;
			if (eax_154 != 0)
			{
				esp_152->dw0000 = 0;
				esp_152->t0004.dw0000 = 2;
				esp_152->t0004.dw0000 = 0;
				esp_105 = esp_152 - 8;
				Mem165[4207480:word32]();
			}
		}
		Mem108[__initenv:word32] = Mem14[4206636:word32];
		Eq_139 * esp_109 = esp_105 - 4;
		esp_109->dw0000 = globals->dw40302C;
		esp_109->dw0000 = globals->ptr403028;
		esp_109->dw0000 = globals->dw403024;
		fn00401000();
		globals->dw40301C = 0;
		if (globals->dw403020 == 0)
		{
			esp_109->dw0000 = 0;
			exit(esp_109->dw0000);
			word32 * * ecx_121 = ebp_10->dw0000;
			word32 eax_123 = **ecx_121;
			ebp_10->dw0000 = eax_123;
			esp_109->dw0000 = ecx_121;
			esp_109->dw0000 = eax_123;
			_XcptFilter();
			return;
		}
		else
		{
			if (globals->dw403018 == 0)
				_cexit();
			ebp_10->dw0000 = 0xFFFFFFFE;
l00401275:
			fn004017B5(ebp_10, 0x0C, dwArg00, dwArg04, dwArg08, dwArg0C);
			return;
		}
	}
	else if (globals->dw40336C == 0)
	{
		globals->dw40336C = 1;
		Mem193[fp - 0x0C:word32] = 0x4020B0;
		Mem195[fp - 16:word32] = 0x4020A0;
		_initterm_e();
		esp_105 = fp + 0xFFFFFFF4;
		if (eax_24 != 0)
		{
			ebp_10->dw0000 = 0xFFFFFFFE;
			goto l00401275;
		}
		else
			goto l0040119D;
	}
	else
	{
		globals->dw403018 = 1;
		goto l0040119D;
	}
}

word32 fn00401420(word32 dwArg04, word32 dwArg08)
{
	Eq_240 * ecx_20 = dwArg04->dw003C + dwArg04;
	up32 edx_54 = 0;
	up32 ebx_22 = (word32) ecx_20->w0006;
	Eq_253 * eax_24 = (word32) ecx_20->w0014 + 24 + ecx_20 + 0x0C;
	if (ebx_22 != 0)
	{
		do
		{
			up32 esi_56 = eax_24->dw0000;
			if (dwArg08 >=u esi_56 && dwArg08 <u eax_24->dw0008 + esi_56)
				goto l0040145E;
			edx_54 = edx_54 + 1;
			eax_24 = eax_24 + 1;
		} while (edx_54 <u ebx_22);
l0040145C:
		eax_24 = null;
	}
	else
		goto l0040145C;
	return eax_24;
}

word32 fn00401470(word32 dwArg04)
{
	word32 eax_14 = fs->dw0000;
	fs->dw0000 = fp - 20;
	if (fn00401530(0x400000) != 0)
	{
		fp->dwFFFFFFF8 = dwArg04 - 0x400000;
		Eq_312 * eax_88 = fn00401420(0x400000, 0x400000);
		if (eax_88 != null)
		{
			word32 eax_95 = ~(eax_88->dw0024 >>u 31);
			fp->dwFFFFFFF8 = 0xFFFFFFFE;
			fs->dw0000 = eax_14;
			return eax_95 & 1;
		}
		else
		{
l0040150F:
			fp->dwFFFFFFF8 = 0xFFFFFFFE;
			fs->dw0000 = eax_14;
			return 0;
		}
	}
	else
		goto l0040150F;
}

word32 fn00401530(word32 dwArg04)
{
	if (dwArg04->w0000 != 23117)
		return 0;
	else
	{
		Eq_344 * ecx_35 = dwArg04->dw003C + dwArg04;
		word32 eax_37 = 0;
		if (ecx_35->dw0000 == 0x4550)
			eax_37 = (word32) (ecx_35->w0018 == 267);
		return eax_37;
	}
}

void fn00401561()
{
	word32 eax_16 = globals->dw403000;
	if (eax_16 != 0xBB40E64E && (eax_16 & 0xFFFF0000) != 0)
		globals->dw403004 = ~eax_16;
	else
	{
		GetSystemTimeAsFileTime(fp - 16);
		word32 v14_55 = dwLoc0C & 0 ^ dwLoc10 & 0 ^ GetCurrentThreadId() ^ GetCurrentProcessId();
		QueryPerformanceCounter(fp - 24);
		word32 ecx_69 = dwLoc14 ^ dwLoc18 ^ v14_55 ^ fp - 8;
		if (ecx_69 == 0xBB40E64E)
			ecx_69 = 0xBB40E64F;
		else if ((ecx_69 & 0xFFFF0000) == 0)
			ecx_69 = ecx_69 | (ecx_69 | 0x4711) << 16;
		globals->dw403000 = ecx_69;
		globals->dw403004 = ~ecx_69;
	}
	return;
}

word32 fn00401770(word32 ebx, word32 esi, word32 edi, word32 dwArg00, word32 dwArg04, word32 dwArg08)
{
	Eq_430 * esp_13 = fp - 8 - dwArg08;
	esp_13->dwFFFFFFFC = ebx;
	esp_13->dwFFFFFFFC = esi;
	esp_13->dwFFFFFFFC = edi;
	esp_13->dwFFFFFFFC = globals->dw403000 ^ fp + 8;
	esp_13->dwFFFFFFFC = dwArg00;
	fs->dw0000 = fp - 8;
	return fp + 8;
}

void fn004017B5(Eq_5 * ebp, word32 dwArg00, word32 dwArg04, word32 dwArg08, word32 dwArg0C, word32 dwArg10)
{
	fs->dw0000 = ebp->dw0000;
	ebp->dw0000 = dwArg00;
	return;
}

