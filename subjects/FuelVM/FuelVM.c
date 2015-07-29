// FuelVM.c
// Generated on 2015-07-25 18:17:37 by decompiling D:\dev\jkl\dec\bin\FuelVM\FuelVM.exe
// using Decompiler version 0.4.5.0.

#include "FuelVM.h"

void fn00401000(word32 edx, word32 dwArg04, word32 dwArg08, word32 dwArg0C)
{
	globals->t403180 = GetModuleHandleA(null);
	DialogBoxParamA(globals->t403180, (CHAR) 0x000003EC, null, &globals->ptr40102C, 0x00000000);
	ExitProcess(0x00000000);
	if (dwArg08 == 0x00000110)
	{
		Mem53[fp - 0x00000008:word32] = 0x00000053;
		Mem55[fp - 0x0000000C:word32] = 0x00000000;
		Mem57[fp - 0x00000010:word32] = 0x00000000;
		SetWindowPos(dwArg04, null, 0x00000000, 0x00000000, 0x000003EC, 0x00000000, 0x0040102C);
		Mem70[fp - 0x00000008:word32] = 0x000007D1;
		Mem72[fp - 0x0000000C:word32] = Mem70[0x00403180:word32];
		Mem76[fp - 0x00000008:word32] = LoadBitmapA(0x00000000, 0x0040102C);
		globals->t403194 = CreatePatternBrush(0x0040102C);
		Mem81[fp - 0x00000008:word32] = 0x0000000A;
		Mem83[fp - 0x0000000C:word32] = 0x000007D2;
		Mem85[fp - 0x00000010:word32] = Mem83[0x00403180:word32];
		globals->t403184 = FindResourceA(() 0x000003EC, 0x00000000, 0x0040102C);
		Mem90[fp - 0x00000008:word32] = Mem88[0x00403184:word32];
		Mem92[fp - 0x0000000C:word32] = Mem90[0x00403180:word32];
		globals->t403188 = LoadResource(0x00000000, 0x0040102C);
		Mem97[fp - 0x00000008:word32] = Mem95[0x00403184:word32];
		Mem99[fp - 0x0000000C:word32] = Mem97[0x00403180:word32];
		globals->t403190 = SizeofResource(0x00000000, 0x0040102C);
		Mem104[fp - 0x00000008:word32] = Mem102[0x00403188:word32];
		globals->t40318C = LockResource(0x0040102C);
		Mem109[fp - 0x00000008:word32] = Mem107[0x0040318C:word32];
		Mem111[fp - 0x0000000C:word32] = Mem109[0x00403190:word32];
		Mem113[fp - 0x00000010:word32] = 0x00000000;
		Eq_6 eax_114 = ExtCreateRegion(() 0x000003EC, 0x00000000, 0x0040102C);
		Mem117[fp - 0x00000008:word32] = 0x00000001;
		Mem119[fp - 0x0000000C:word32] = eax_114;
		Mem121[fp - 0x00000010:word32] = dwArg04;
		SetWindowRgn(() 0x000003EC, 0x00000000, 0x0040102C);
		Mem125[fp - 0x00000008:word32] = 0x00000001;
		Mem127[fp - 0x0000000C:word32] = dwArg04;
		ShowWindow(0x00000000, 0x0040102C);
l004011BC:
		return;
	}
	else if (dwArg08 == 0x00000136)
		return;
	else if (dwArg08 == 0x0000000F)
	{
		Mem144[fp - 0x00000008:word32] = 0x00000001;
		Mem146[fp - 0x0000000C:word32] = 0x00000000;
		Mem148[fp - 0x00000010:word32] = dwArg04;
		InvalidateRect(() 0x000003EC, 0x00000000, 0x0040102C);
		Mem152[fp - 0x00000008:word32] = dwArg04;
		UpdateWindow(0x0040102C);
		Mem156[fp - 0x00000008:word32] = 0x00000001;
		Mem158[fp - 0x0000000C:word32] = dwArg04;
		ShowWindow(0x00000000, 0x0040102C);
		return;
	}
	else if (dwArg08 != 0x00000111)
		if (dwArg08 == 0x00000010)
		{
			Mem206[fp - 0x00000008:word32] = 0x00090000;
			Mem208[fp - 0x0000000C:word32] = 0x000003E8;
			Mem210[fp - 0x00000010:word32] = dwArg04;
			AnimateWindow(() 0x000003EC, 0x00000000, 0x0040102C);
			Mem214[fp - 0x00000008:word32] = Mem210[0x00403194:word32];
			DeleteObject(0x0040102C);
			Mem218[fp - 0x00000008:word32] = 0x00000000;
			Mem220[fp - 0x0000000C:word32] = dwArg04;
			EndDialog(0x00000000, 0x0040102C);
		}
	else if (dwArg0C == 0x000003F2)
	{
		Mem170[fp - 0x00000008:word32] = 0x0000000C;
		Mem172[fp - 0x0000000C:word32] = 0x00403198;
		Mem174[fp - 0x00000010:word32] = 0x000003F8;
		GetDlgItemTextA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C);
		Mem180[fp - 0x00000008:word32] = 0x0000000C;
		Mem182[fp - 0x0000000C:word32] = 0x004031B0;
		Mem184[fp - 0x00000010:word32] = 0x000003F9;
		GetDlgItemTextA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C);
		globals->b4031CC = 0x00;
		fn00401238(edx);
	}
	else if (dwArg0C == 0x000003FA)
	{
		Mem194[fp - 0x00000008:word32] = 0x00000000;
		Mem196[fp - 0x0000000C:word32] = 0x00000000;
		Mem198[fp - 0x00000010:word32] = 0x00000010;
		SendMessageA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C);
	}
}

word32 fn004011C2(word32 dwArg04)
{
	Eq_344 * edi_16 = dwArg04;
	word32 ecx_11 = ~0x00000000;
	do
	{
		if (ecx_11 == 0x00000000)
			break;
		edi_16 = PTRADD(edi_33,1);
		ecx_11 = ecx_11 - 0x00000001;
		Eq_344 * edi_33 = edi_16;
	} while (0x00 != *edi_33);
	return ~ecx_11 - 0x00000001;
}

void fn004011D8()
{
	globals->dw4035CF = 0x00000000;
	globals->dw4035D7 = 0x00000000;
	globals->dw4035DF = 0x00000000;
	globals->dw4035E7 = (word32) *(word32) globals->b4031CC;
	globals->dw4035EF = 0x00000032;
	globals->dw4035FF = 0x00000000;
	globals->b403610 = 0x00;
	globals->b40360F = 0x00;
	globals->b4031CC = globals->b4031CC + 0x01;
	return;
}

void fn00401238(word32 edx)
{
fn00401238_entry:
l00401238:
	Eq_399 ecx_10 = fn004011C2(0x00403198)
	branch ecx_10 >=u 0x00000007 l0040124F
l0040124D:
	return
l0040124F:
	globals->t4031CD = ecx_10;
	Mem31[fp + 0xFFFFFFF4:word32] = 0x004031B0
	branch fn004011C2(0x00403198) >=u 0x00000007 l00401268
l00401266:
	return
l00401268:
	Mem43[fp + 0xFFFFFFF4:word32] = edx
	Eq_399 ecx_39 = 0x00000000
l00401271:
	Mem52[ecx_39 + 0x00403198:byte] = (byte) (Mem43[ecx_39 + 0x00403198:word32] ^ ecx_39)
	ecx_39.u0 = ecx_39.u0;
	branch ecx_39 <= globals->t4031CD l00401271
l00401293:
	globals->t4031CE = ecx_39;
	globals->dw4031C8 = globals->dw4035FF;
	fn004011D8()
	Mem73[fs:0x00000000:word32] = fp + 0xFFFFFFF0
	fn004011D8()
	int3()
l004012D4:
	goto l004012D4
fn00401238_exit:
}

