/*
* Copyright (C) 1999-2017 John Källén.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; either version 2, or (at your option)
* any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; see the file COPYING.  If not, write to
* the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
*/

#include "stdafx.h"
#include "reko.h"

#include "ComBase.h"
#include "ArmRewriter.h"
#include "Arm32Disassembler.h"
#include "ArmArchitecture.h"
#include "functions.h"

// 09FFCC1F-60C8-4058-92C2-C90DAF115250
static const IID IID_INativeArchitecture =
{ 0x09FFCC1F, 0x60C8, 0x4058,{ 0x92, 0xC2, 0xc9, 0x0D, 0xAF, 0x11, 0x52, 0x50 } };

ArmArchitecture::ArmArchitecture()
{
	AddRef();
}

STDMETHODIMP ArmArchitecture::QueryInterface(REFIID riid, void ** ppvObject)
{
	if (riid == IID_INativeArchitecture || riid == IID_IUnknown)
	{
		AddRef();
		*ppvObject = static_cast<INativeArchitecture *>(this);
		return S_OK;
	}
	*ppvObject = nullptr;
	return E_NOINTERFACE;
}


void STDMETHODCALLTYPE ArmArchitecture::GetAllRegisters(int * pcRegs, const NativeRegister ** ppRegs)
{
	*pcRegs = ARM_REG_ENDING;
	*ppRegs = &aRegs[0];
}

INativeDisassembler * STDMETHODCALLTYPE ArmArchitecture::CreateDisassembler(
	const uint8_t * bytes, int length, int offset, uint64_t uAddr)
{
	return new Arm32Disassembler(bytes, length, offset, uAddr);
}

INativeRewriter * STDAPICALLTYPE ArmArchitecture::CreateRewriter(
	const uint8_t * rawBytes,
	uint32_t length,
	uint32_t offset,
	uint64_t address,
	INativeRtlEmitter * m,
	INativeTypeFactory * typeFactory,
	INativeRewriterHost * host)
{
	return new ArmRewriter(rawBytes, length-offset, address, m, typeFactory, host);
}

const NativeRegister ArmArchitecture::aRegs[] = {
	{ nullptr,		 ARM_REG_INVALID,    0, },
	{ "apsr",        ARM_REG_APSR,		 32, },
	{ "apsr_nzcv",	 ARM_REG_APSR_NZCV,	 32, },
	{ "cpsr",		 ARM_REG_CPSR,		 32, },
	{ "fpexc",		 ARM_REG_FPEXC,		 32, },
	{ "fpinst",		 ARM_REG_FPINST,	 32, },
	{ "fpscr",		 ARM_REG_FPSCR,		 32, },
	{ "fpscr_nzcv",	 ARM_REG_FPSCR_NZCV, 32, },
	{ "fpsid",		 ARM_REG_FPSID,		 32, },
	{ "itstate",	 ARM_REG_ITSTATE,	 32, },
	{ "lr",			 ARM_REG_LR,		 32, },
	{ "pc",			 ARM_REG_PC,		 32, },
	{ "sp",			 ARM_REG_SP,		 32, },
	{ "spsr",		 ARM_REG_SPSR,		 32, },
	{ "d0",			 ARM_REG_D0,		 64, },
	{ "d1",			 ARM_REG_D1,		 64, },
	{ "d2",			 ARM_REG_D2,		 64, },
	{ "d3",			 ARM_REG_D3,		 64, },
	{ "d4",			 ARM_REG_D4,		 64, },
	{ "d5",			 ARM_REG_D5,		 64, },
	{ "d6",			 ARM_REG_D6,		 64, },
	{ "d7",			 ARM_REG_D7,		 64, },
	{ "d8",			 ARM_REG_D8,		 64, },
	{ "d9",			 ARM_REG_D9,		 64, },
	{ "d10",		 ARM_REG_D10,		 64, },
	{ "d11",		 ARM_REG_D11,		 64, },
	{ "d12",		 ARM_REG_D12,		 64, },
	{ "d13",		 ARM_REG_D13,		 64, },
	{ "d14",		 ARM_REG_D14,		 64, },
	{ "d15",		 ARM_REG_D15,		 64, },
	{ "d16",		 ARM_REG_D16,		 64, },
	{ "d17",		 ARM_REG_D17,		 64, },
	{ "d18",		 ARM_REG_D18,		 64, },
	{ "d19",		 ARM_REG_D19,		 64, },
	{ "d20",		 ARM_REG_D20,		 64, },
	{ "d21",		 ARM_REG_D21,		 64, },
	{ "d22",		 ARM_REG_D22,		 64, },
	{ "d23",		 ARM_REG_D23,		 64, },
	{ "d24",		 ARM_REG_D24,		 64, },
	{ "d25",		 ARM_REG_D25,		 64, },
	{ "d26",		 ARM_REG_D26,		 64, },
	{ "d27",		 ARM_REG_D27,		 64, },
	{ "d28",		 ARM_REG_D28,		 64, },
	{ "d29",		 ARM_REG_D29,		 64, },
	{ "d30",		 ARM_REG_D30,		 64, },
	{ "d31",		 ARM_REG_D31,		 64, },
	{ "fpinst2",	 ARM_REG_FPINST2,	 32, },
	{ "mvfr0",		 ARM_REG_MVFR0,		 32, },
	{ "mvfr1",		 ARM_REG_MVFR1,		 32, },
	{ "mvfr2",		 ARM_REG_MVFR2,		 32, },
	{ "q0",			 ARM_REG_Q0,		 128, },
	{ "q1",			 ARM_REG_Q1,		 128, },
	{ "q2",			 ARM_REG_Q2,		 128, },
	{ "q3",			 ARM_REG_Q3,		 128, },
	{ "q4",			 ARM_REG_Q4,		 128, },
	{ "q5",			 ARM_REG_Q5,		 128, },
	{ "q6",			 ARM_REG_Q6,		 128, },
	{ "q7",			 ARM_REG_Q7,		 128, },
	{ "q8",			 ARM_REG_Q8,		 128, },
	{ "q9",			 ARM_REG_Q9,		 128, },
	{ "q10",		 ARM_REG_Q10,		 128, },
	{ "q11",		 ARM_REG_Q11,		 128, },
	{ "q12",		 ARM_REG_Q12,		 128, },
	{ "q13",		 ARM_REG_Q13,		 128, },
	{ "q14",		 ARM_REG_Q14,		 128, },
	{ "q15",		 ARM_REG_Q15,		 128, },
	{ "r0",			 ARM_REG_R0,		 32, },
	{ "r1",			 ARM_REG_R1,		 32, },
	{ "r2",			 ARM_REG_R2,		 32, },
	{ "r3",			 ARM_REG_R3,		 32, },
	{ "r4",			 ARM_REG_R4,		 32, },
	{ "r5",			 ARM_REG_R5,		 32, },
	{ "r6",			 ARM_REG_R6,		 32, },
	{ "r7",			 ARM_REG_R7,		 32, },
	{ "r8",			 ARM_REG_R8,		 32, },
	{ "r9",			 ARM_REG_R9,		 32, },
	{ "r10",		 ARM_REG_R10,		 32, },
	{ "r11",		 ARM_REG_R11,		 32, },
	{ "r12",		 ARM_REG_R12,		 32, },
	{ "s0",			 ARM_REG_S0,		 32, },
	{ "s1",			 ARM_REG_S1,		 32, },
	{ "s2",			 ARM_REG_S2,		 32, },
	{ "s3",			 ARM_REG_S3,		 32, },
	{ "s4",			 ARM_REG_S4,		 32, },
	{ "s5",			 ARM_REG_S5,		 32, },
	{ "s6",			 ARM_REG_S6,		 32, },
	{ "s7",			 ARM_REG_S7,		 32, },
	{ "s8",			 ARM_REG_S8,		 32, },
	{ "s9",			 ARM_REG_S9,		 32, },
	{ "s10",		 ARM_REG_S10,		 32, },
	{ "s11",		 ARM_REG_S11,		 32, },
	{ "s12",		 ARM_REG_S12,		 32, },
	{ "s13",		 ARM_REG_S13,		 32, },
	{ "s14",		 ARM_REG_S14,		 32, },
	{ "s15",		 ARM_REG_S15,		 32, },
	{ "s16",		 ARM_REG_S16,		 32, },
	{ "s17",		 ARM_REG_S17,		 32, },
	{ "s18",		 ARM_REG_S18,		 32, },
	{ "s19",		 ARM_REG_S19,		 32, },
	{ "s20",		 ARM_REG_S20,		 32, },
	{ "s21",		 ARM_REG_S21,		 32, },
	{ "s22",		 ARM_REG_S22,		 32, },
	{ "s23",		 ARM_REG_S23,		 32, },
	{ "s24",		 ARM_REG_S24,		 32, },
	{ "s25",		 ARM_REG_S25,		 32, },
	{ "s26",		 ARM_REG_S26,		 32, },
	{ "s27",		 ARM_REG_S27,		 32, },
	{ "s28",		 ARM_REG_S28,		 32, },
	{ "s29",		 ARM_REG_S29,		 32, },
	{ "s30",		 ARM_REG_S30,		 32, },
	{ "s31",		 ARM_REG_S31,		 32, },

	//	ARM_REG_ENDING,		// <-- mark the end of the list or registers

		//					//> alias registers
		//ARM_REG_R13 = ARM_REG_SP,
		//ARM_REG_R14 = ARM_REG_LR,
		//ARM_REG_R15 = ARM_REG_PC,

		//ARM_REG_SB = ARM_REG_R9,
		//ARM_REG_SL = ARM_REG_R10,
		//ARM_REG_FP = ARM_REG_R11,
		//ARM_REG_IP = ARM_REG_R12,
};
