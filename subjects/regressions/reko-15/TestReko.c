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
	Eq_n * ebp_n = fn00401770(ebx, esi, edi, dwLoc0C, 4202984, 0x0C);
	ebp_n->dw0000 = 0;
	Eq_n * esp_n = fp - 8;
	word32 edx_n = fs->ptr0018->dw0004;
	word32 edi_n = 0;
	do
	{
		__lock();
		word32 eax_n;
		__cmpxchg(globals->dw403368, edx_n, 0, out eax_n);
		if (eax_n == 0)
			goto l00401151;
	} while (eax_n != edx_n);
	edi_n = 1;
l00401151:
	if (globals->dw40336C == 1)
	{
		Mem185[fp - 0x0C:word32] = 31;
		_amsg_exit();
		esp_n = fp + 0xFFFFFFF4;
l0040119D:
		if (globals->dw40336C == 1)
		{
			Eq_n * esp_n = esp_n - 4;
			esp_n->dw0000 = 4202652;
			esp_n->dw0000 = 4202644;
			_initterm();
			globals->dw40336C = 2;
			esp_n = esp_n;
		}
		if (edi_n == 0)
			globals->dw403368 = 0;
		if (globals->ptr403378 != null)
		{
			Eq_n * esp_n = esp_n - 4;
			esp_n->dw0000 = 4207480;
			word32 eax_n = fn00401470(dwArg00);
			esp_n = esp_n + 4;
			if (eax_n != 0)
			{
				esp_n->dw0000 = 0;
				esp_n->t0004.dw0000 = 2;
				esp_n->t0004.dw0000 = 0;
				esp_n = esp_n - 8;
				Mem165[4207480:word32]();
			}
		}
		Mem108[__initenv:word32] = Mem14[4206636:word32];
		Eq_n * esp_n = esp_n - 4;
		esp_n->dw0000 = globals->dw40302C;
		esp_n->dw0000 = globals->ptr403028;
		esp_n->dw0000 = globals->dw403024;
		fn00401000();
		globals->dw40301C = 0;
		if (globals->dw403020 == 0)
		{
			esp_n->dw0000 = 0;
			exit(esp_n->dw0000);
			word32 * * ecx_n = ebp_n->dw0000;
			word32 eax_n = **ecx_n;
			ebp_n->dw0000 = eax_n;
			esp_n->dw0000 = ecx_n;
			esp_n->dw0000 = eax_n;
			_XcptFilter();
			return;
		}
		else
		{
			if (globals->dw403018 == 0)
				_cexit();
			ebp_n->dw0000 = 0xFFFFFFFE;
l00401275:
			fn004017B5(ebp_n, 0x0C, dwArg00, dwArg04, dwArg08, dwArg0C);
			return;
		}
	}
	else if (globals->dw40336C == 0)
	{
		globals->dw40336C = 1;
		Mem193[fp - 0x0C:word32] = 0x4020B0;
		Mem195[fp - 16:word32] = 0x4020A0;
		_initterm_e();
		esp_n = fp + 0xFFFFFFF4;
		if (eax_n != 0)
		{
			ebp_n->dw0000 = 0xFFFFFFFE;
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
	Eq_n * ecx_n = dwArg04->dw003C + dwArg04;
	up32 edx_n = 0;
	up32 ebx_n = (word32) ecx_n->w0006;
	Eq_n * eax_n = (word32) ecx_n->w0014 + 24 + ecx_n + 0x0C;
	if (ebx_n != 0)
	{
		do
		{
			up32 esi_n = eax_n->dw0000;
			if (dwArg08 >=u esi_n && dwArg08 <u eax_n->dw0008 + esi_n)
				goto l0040145E;
			edx_n = edx_n + 1;
			eax_n = eax_n + 1;
		} while (edx_n <u ebx_n);
l0040145C:
		eax_n = null;
	}
	else
		goto l0040145C;
	return eax_n;
}

word32 fn00401470(word32 dwArg04)
{
	word32 eax_n = fs->dw0000;
	fs->dw0000 = fp - 20;
	if (fn00401530(0x400000) != 0)
	{
		fp->dwFFFFFFF8 = dwArg04 - 0x400000;
		Eq_n * eax_n = fn00401420(0x400000, 0x400000);
		if (eax_n != null)
		{
			word32 eax_n = ~(eax_n->dw0024 >>u 31);
			fp->dwFFFFFFF8 = 0xFFFFFFFE;
			fs->dw0000 = eax_n;
			return eax_n & 1;
		}
		else
		{
l0040150F:
			fp->dwFFFFFFF8 = 0xFFFFFFFE;
			fs->dw0000 = eax_n;
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
		Eq_n * ecx_n = dwArg04->dw003C + dwArg04;
		word32 eax_n = 0;
		if (ecx_n->dw0000 == 0x4550)
			eax_n = (word32) (ecx_n->w0018 == 267);
		return eax_n;
	}
}

void fn00401561()
{
	word32 eax_n = globals->dw403000;
	if (eax_n != 0xBB40E64E && (eax_n & 0xFFFF0000) != 0)
		globals->dw403004 = ~eax_n;
	else
	{
		GetSystemTimeAsFileTime(fp - 16);
		word32 v14_n = dwLoc0C & 0 ^ dwLoc10 & 0 ^ GetCurrentThreadId() ^ GetCurrentProcessId();
		QueryPerformanceCounter(fp - 24);
		word32 ecx_n = dwLoc14 ^ dwLoc18 ^ v14_n ^ fp - 8;
		if (ecx_n == 0xBB40E64E)
			ecx_n = 0xBB40E64F;
		else if ((ecx_n & 0xFFFF0000) == 0)
			ecx_n = ecx_n | (ecx_n | 0x4711) << 16;
		globals->dw403000 = ecx_n;
		globals->dw403004 = ~ecx_n;
	}
	return;
}

word32 fn00401770(word32 ebx, word32 esi, word32 edi, word32 dwArg00, word32 dwArg04, word32 dwArg08)
{
	Eq_n * esp_n = fp - 8 - dwArg08;
	esp_n->dwFFFFFFFC = ebx;
	esp_n->dwFFFFFFFC = esi;
	esp_n->dwFFFFFFFC = edi;
	esp_n->dwFFFFFFFC = globals->dw403000 ^ fp + 8;
	esp_n->dwFFFFFFFC = dwArg00;
	fs->dw0000 = fp - 8;
	return fp + 8;
}

void fn004017B5(Eq_n * ebp, word32 dwArg00, word32 dwArg04, word32 dwArg08, word32 dwArg0C, word32 dwArg10)
{
	fs->dw0000 = ebp->dw0000;
	ebp->dw0000 = dwArg00;
	return;
}

