// ThrottleControl_seg000C1D6C.c
// Generated by decompiling ThrottleControl.mot
// using Reko decompiler version 0.12.1.0.

#include "ThrottleControl.h"

// 000C1D6C: Register word16 fn000C1D6C(Stack word16 wArg03, Stack ui16 wArg05, Stack cup16 wArg07, Stack ui16 wArg09, Stack Eq_n wArg0B)
// Called from:
//      fn000C18E6
word16 fn000C1D6C(word16 wArg03, ui16 wArg05, cup16 wArg07, ui16 wArg09, Eq_n wArg0B)
{
	Eq_n r0_n = wArg0B;
	ui32 dwLoc0A_n = SEQ(wArg09, wArg07);
	ui32 dwLoc06_n = SEQ(wArg05, wArg03);
	while (true)
	{
		word16 wLoc04_n = SLICE(dwLoc06_n, word16, 16);
		word16 wLoc06_n = (word16) dwLoc06_n;
		word16 wLoc08_n = SLICE(dwLoc0A_n, word16, 16);
		word16 wLoc0A_n = (word16) dwLoc0A_n;
		r0_n = (word32) r0_n + 0x0000FFFF;
		if (r0_n == 0x00)
			break;
		*SEQ(wLoc04_n, r0_n) = *SEQ(wLoc08_n, r0_n);
		dwLoc0A_n = SEQ(SLICE(dwLoc0A_n + 0x01, word16, 16), wLoc0A_n + 0x01);
		dwLoc06_n = SEQ(SLICE(dwLoc06_n + 0x01, word16, 16), wLoc06_n + 0x01);
	}
	return wArg03;
}

// 000C1DB0: void fn000C1DB0(Register Eq_n r2, Stack word16 wArg03, Stack ui16 wArg05, Stack Eq_n wArg07)
// Called from:
//      fn000C181E
void fn000C1DB0(Eq_n r2, word16 wArg03, ui16 wArg05, Eq_n wArg07)
{
	Eq_n r0_n = wArg07;
	ui32 dwLoc06_n = SEQ(wArg05, wArg03);
	while (true)
	{
		word16 wLoc04_n = SLICE(dwLoc06_n, word16, 16);
		word16 wLoc06_n = (word16) dwLoc06_n;
		r0_n.u1 = (word16) r0_n.u1 + 0x0000FFFF;
		if (r0_n == 0x00)
			break;
		SEQ(wLoc04_n, r0_n)->u0 = (byte) r2;
		dwLoc06_n = SEQ(SLICE(dwLoc06_n + 0x01, word16, 16), wLoc06_n + 0x01);
	}
}

// 000C1DE4: Sequence uint32 fn000C1DE4(Sequence uint32 r2r0, Register (ptr16 Eq_n) sb)
// Called from:
//      fn000C1ACC
uint32 fn000C1DE4(uint32 r2r0, struct Eq_n * sb)
{
	cup16 r2 = SLICE(r2r0, word16, 16);
	word16 r1_n = sb->w000B;
	uint16 r3_n = sb->w000D;
	if (r2 >= 0x8000)
	{
		word32 r2r0_n = ~r2r0;
		r2r0 = SEQ(SLICE(r2r0_n + 0x01, word16, 16), (word16) r2r0_n + 0x01);
	}
	word16 r0_n = (word16) r2r0;
	Eq_n r2_n = SLICE(r2r0, word16, 16);
	uint32 r3_r1_n = SEQ(r3_n, r1_n);
	if (r3_n >= 0x8000)
	{
		uint32 r3_r1_n = ~SEQ(r3_n, r1_n);
		r3_r1_n = SEQ(SLICE(r3_r1_n + 0x01, word16, 16), (word16) r3_r1_n + 0x01);
	}
	uint16 r1_n = (word16) r3_r1_n;
	Eq_n r3_n;
	r3_n.u1 = SLICE(r3_r1_n, word16, 16);
	uint32 r2r0_n;
	if (r3_n == 0x00)
	{
		if (r2_n != 0x00)
		{
			uint32 r2r0_n = (uint32) r2_n;
			r2_n = r2r0_n % r1_n;
			r3_n = r2r0_n /u r1_n;
		}
		r2r0_n = SEQ(r3_n, SEQ(r2_n, r0_n) /u r1_n);
	}
	else
	{
		Eq_n a0_n;
		a0_n.u0 = 0x00;
		ci16 a1_n = 0x00;
		uint32 r3_r1_n = r3_r1_n;
		uint32 r2r0_n;
		do
		{
			r2r0_n = r2r0;
			if (r2_n <= SLICE(r3_r1_n, word16, 16))
				break;
			r3_r1_n <<= 0x01;
			++a1_n;
			r2r0_n = r2r0;
		} while ((word16) r3_r1_n >= 0x00);
		do
		{
			uint32 r2r0_n = r2r0_n - r3_r1_n;
			cup16 r2_n = SLICE(r2r0_n, word16, 16);
			Eq_n C_n = cond(r2_n) & 0x01;
			if (r2_n < 0x00)
			{
				r2r0_n += r3_r1_n;
				C_n.u0 = 0x00;
			}
			a0_n = __rcl<word16,int16>(a0_n, 1, C_n);
			r3_r1_n >>= 0x08;
			--a1_n;
			r2r0_n = r2r0_n;
		} while (a1_n >= 0x00);
		r2r0_n = (uint32) a0_n;
	}
	word16 r2_n = SLICE(r2r0_n, word16, 16);
	word16 r0_n = (word16) r2r0_n;
	if (sb->w0000 != 0x00)
	{
		word32 r2r0_n = ~r2r0_n;
		r0_n = (word16) r2r0_n + 0x01;
		r2_n = SLICE(r2r0_n + 0x01, word16, 16);
	}
	return SEQ(r2_n, r0_n);
}

