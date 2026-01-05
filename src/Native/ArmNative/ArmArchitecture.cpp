/*
* Copyright (C) 1999-2026 John Källén.
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

#include "common/compat.h"

ArmArchitecture::ArmArchitecture()
{

	AddRef();
}

STDMETHODIMP ArmArchitecture::QueryInterface(REFIID riid, void ** ppvObject)
{
	if (riid == IID_INativeArchitecture ||
		riid == IID_IAgileObject ||
		riid == IID_IUnknown)
	{
		AddRef();
		*ppvObject = static_cast<INativeArchitecture *>(this);
		return S_OK;
	}
	*ppvObject = nullptr;
	return E_NOINTERFACE;
}


STDMETHODIMP ArmArchitecture::GetAllRegisters(int regKind, int * pcRegs, void ** ppRegs)
{
	if (regKind == 0)
	{
		*pcRegs = countof(aRegs);
		*ppRegs = const_cast<NativeRegister *>(&aRegs[0]);
	}
	else if (regKind == 1)
	{
		*pcRegs = countof(aSysregs);
		*ppRegs = const_cast<NativeRegister *>(&aSysregs[0]);
	}
	return S_OK;
}

INativeDisassembler * STDMETHODCALLTYPE ArmArchitecture::CreateDisassembler(
	void* bytes, int length, int offset, uint64_t uAddr)
{
	auto dasm = new Arm32Disassembler(reinterpret_cast<uint8_t*>(bytes) + offset, length - offset, offset, uAddr);
	dasm->AddRef();
	return dasm;
}

INativeRewriter * STDAPICALLTYPE ArmArchitecture::CreateRewriter(
	void * rawBytes,
	int32_t length,
	int32_t offset,
	uint64_t address,
	INativeRtlEmitter * m,
	INativeTypeFactory * typeFactory,
	INativeRewriterHost * host)
{
	auto rw = new ArmRewriter(cs_mode::CS_MODE_ARM, reinterpret_cast<uint8_t*>(rawBytes) + offset, length-offset, address, m, typeFactory, host);
	rw->AddRef();
	return rw;
}

const NativeRegister ArmArchitecture::aRegs[110] = {
	{ "apsr",        ARM_REG_APSR,		ARM_REG_APSR,		 32, },
	{ "apsr_nzcv",	 ARM_REG_APSR_NZCV,	ARM_REG_APSR_NZCV,	 32, },
	{ "cpsr",		 ARM_REG_CPSR,		ARM_REG_CPSR,		 32, },
	{ "fpexc",		 ARM_REG_FPEXC,		ARM_REG_FPEXC,		 32, },
	{ "fpinst",		 ARM_REG_FPINST,	ARM_REG_FPINST,		 32, },
	{ "fpscr",		 ARM_REG_FPSCR,		ARM_REG_FPSCR,		 32, },
	{ "fpscr_nzcv",	 ARM_REG_FPSCR_NZCV,ARM_REG_FPSCR_NZCV,	 32, },
	{ "fpsid",		 ARM_REG_FPSID,		ARM_REG_FPSID,		 32, },
	{ "itstate",	 ARM_REG_ITSTATE,	ARM_REG_ITSTATE,	 32, },
	{ "lr",			 ARM_REG_LR,		ARM_REG_LR,			 32, },
	{ "pc",			 ARM_REG_PC,		ARM_REG_PC,			 32, },
	{ "sp",			 ARM_REG_SP,		ARM_REG_SP,			 32, },
	{ "spsr",		 ARM_REG_SPSR,		ARM_REG_SPSR,		 32, },
	{ "d0",			 ARM_REG_D0,		ARM_REG_Q0,			 64,	0 },
	{ "d1",			 ARM_REG_D1,		ARM_REG_Q0,			 64,	64},
	{ "d2",			 ARM_REG_D2,		ARM_REG_Q1,			 64,	0 },
	{ "d3",			 ARM_REG_D3,		ARM_REG_Q1,			 64,	64},
	{ "d4",			 ARM_REG_D4,		ARM_REG_Q2,			 64,	0 },
	{ "d5",			 ARM_REG_D5,		ARM_REG_Q2,			 64,	64 },
	{ "d6",			 ARM_REG_D6,		ARM_REG_Q3,			 64,	0 },
	{ "d7",			 ARM_REG_D7,		ARM_REG_Q3,			 64,	64 },
	{ "d8",			 ARM_REG_D8,		ARM_REG_Q4,			 64,	0 },
	{ "d9",			 ARM_REG_D9,		ARM_REG_Q4,			 64,	64 },
	{ "d10",		 ARM_REG_D10,		ARM_REG_Q5,			 64,	0 },
	{ "d11",		 ARM_REG_D11,		ARM_REG_Q5,			 64,	64 },
	{ "d12",		 ARM_REG_D12,		ARM_REG_Q6,			 64,	0 },
	{ "d13",		 ARM_REG_D13,		ARM_REG_Q6,			 64,	64 },
	{ "d14",		 ARM_REG_D14,		ARM_REG_Q7,			 64,	0 },
	{ "d15",		 ARM_REG_D15,		ARM_REG_Q7,			 64,	64 },
	{ "d16",		 ARM_REG_D16,		ARM_REG_Q8,			 64,	0 },
	{ "d17",		 ARM_REG_D17,		ARM_REG_Q8,			 64,	64 },
	{ "d18",		 ARM_REG_D18,		ARM_REG_Q9,			 64,	0 },
	{ "d19",		 ARM_REG_D19,		ARM_REG_Q9,			 64,	64 },
	{ "d20",		 ARM_REG_D20,		ARM_REG_Q10,		 64,	0 },
	{ "d21",		 ARM_REG_D21,		ARM_REG_Q10,		 64,	64 },
	{ "d22",		 ARM_REG_D22,		ARM_REG_Q11,		 64,	0 },
	{ "d23",		 ARM_REG_D23,		ARM_REG_Q11,		 64,	64 },
	{ "d24",		 ARM_REG_D24,		ARM_REG_Q12,		 64,	0 },
	{ "d25",		 ARM_REG_D25,		ARM_REG_Q12,		 64,	64 },
	{ "d26",		 ARM_REG_D26,		ARM_REG_Q13,		 64,	0 },
	{ "d27",		 ARM_REG_D27,		ARM_REG_Q13,		 64,	64 },
	{ "d28",		 ARM_REG_D28,		ARM_REG_Q14,		 64,	0 },
	{ "d29",		 ARM_REG_D29,		ARM_REG_Q14,		 64,	64 },
	{ "d30",		 ARM_REG_D30,		ARM_REG_Q15,		 64,	0 },
	{ "d31",		 ARM_REG_D31,		ARM_REG_Q15,		 64,	64 },
	{ "fpinst2",	 ARM_REG_FPINST2,	ARM_REG_FPINST2,	 32, },
	{ "mvfr0",		 ARM_REG_MVFR0,		ARM_REG_MVFR0,		 32, },
	{ "mvfr1",		 ARM_REG_MVFR1,		ARM_REG_MVFR1,		 32, },
	{ "mvfr2",		 ARM_REG_MVFR2,		ARM_REG_MVFR2,		 32, },
	{ "q0",			 ARM_REG_Q0,		ARM_REG_Q0,			 128, },
	{ "q1",			 ARM_REG_Q1,		ARM_REG_Q1,			 128, },
	{ "q2",			 ARM_REG_Q2,		ARM_REG_Q2,			 128, },
	{ "q3",			 ARM_REG_Q3,		ARM_REG_Q3,			 128, },
	{ "q4",			 ARM_REG_Q4,		ARM_REG_Q4,			 128, },
	{ "q5",			 ARM_REG_Q5,		ARM_REG_Q5,			 128, },
	{ "q6",			 ARM_REG_Q6,		ARM_REG_Q6,			 128, },
	{ "q7",			 ARM_REG_Q7,		ARM_REG_Q7,			 128, },
	{ "q8",			 ARM_REG_Q8,		ARM_REG_Q8,			 128, },
	{ "q9",			 ARM_REG_Q9,		ARM_REG_Q9,			 128, },
	{ "q10",		 ARM_REG_Q10,		ARM_REG_Q10,		 128, },
	{ "q11",		 ARM_REG_Q11,		ARM_REG_Q11,		 128, },
	{ "q12",		 ARM_REG_Q12,		ARM_REG_Q12,		 128, },
	{ "q13",		 ARM_REG_Q13,		ARM_REG_Q13,		 128, },
	{ "q14",		 ARM_REG_Q14,		ARM_REG_Q14,		 128, },
	{ "q15",		 ARM_REG_Q15,		ARM_REG_Q15,		 128, },
	{ "r0",			 ARM_REG_R0,		ARM_REG_R0,			 32, },
	{ "r1",			 ARM_REG_R1,		ARM_REG_R1,			 32, },
	{ "r2",			 ARM_REG_R2,		ARM_REG_R2,			 32, },
	{ "r3",			 ARM_REG_R3,		ARM_REG_R3,			 32, },
	{ "r4",			 ARM_REG_R4,		ARM_REG_R4,			 32, },
	{ "r5",			 ARM_REG_R5,		ARM_REG_R5,			 32, },
	{ "r6",			 ARM_REG_R6,		ARM_REG_R6,			 32, },
	{ "r7",			 ARM_REG_R7,		ARM_REG_R7,			 32, },
	{ "r8",			 ARM_REG_R8,		ARM_REG_R8,			 32, },
	{ "r9",			 ARM_REG_R9,		ARM_REG_R9,			 32, },
	{ "r10",		 ARM_REG_R10,		ARM_REG_R10,		 32, },
	{ "fp",			 ARM_REG_R11,		ARM_REG_R11,		 32, },
	{ "ip",			 ARM_REG_R12,		ARM_REG_R12,		 32, },
	{ "s0",			 ARM_REG_S0,		ARM_REG_Q0,			 32, 0 },
	{ "s1",			 ARM_REG_S1,		ARM_REG_Q0,			 32, 32 },
	{ "s2",			 ARM_REG_S2,		ARM_REG_Q0,			 32, 64 },
	{ "s3",			 ARM_REG_S3,		ARM_REG_Q0,			 32, 96 },
	{ "s4",			 ARM_REG_S4,		ARM_REG_Q1,			 32, 0 },
	{ "s5",			 ARM_REG_S5,		ARM_REG_Q1,			 32, 32 },
	{ "s6",			 ARM_REG_S6,		ARM_REG_Q1,			 32, 64 },
	{ "s7",			 ARM_REG_S7,		ARM_REG_Q1,			 32, 96 },
	{ "s8",			 ARM_REG_S8,		ARM_REG_Q2,			 32, 0 },
	{ "s9",			 ARM_REG_S9,		ARM_REG_Q2,			 32, 32 },
	{ "s10",		 ARM_REG_S10,		ARM_REG_Q2,			 32, 64 },
	{ "s11",		 ARM_REG_S11,		ARM_REG_Q2,			 32, 96 },
	{ "s12",		 ARM_REG_S12,		ARM_REG_Q3,			 32, 0 },
	{ "s13",		 ARM_REG_S13,		ARM_REG_Q3,			 32, 32 },
	{ "s14",		 ARM_REG_S14,		ARM_REG_Q3,			 32, 64 },
	{ "s15",		 ARM_REG_S15,		ARM_REG_Q3,			 32, 96 },
	{ "s16",		 ARM_REG_S16,		ARM_REG_Q4,			 32, 0 },
	{ "s17",		 ARM_REG_S17,		ARM_REG_Q4,			 32, 32 },
	{ "s18",		 ARM_REG_S18,		ARM_REG_Q4,			 32, 64 },
	{ "s19",		 ARM_REG_S19,		ARM_REG_Q4,			 32, 96 },
	{ "s20",		 ARM_REG_S20,		ARM_REG_Q5,			 32, 0 },
	{ "s21",		 ARM_REG_S21,		ARM_REG_Q5,			 32, 32 },
	{ "s22",		 ARM_REG_S22,		ARM_REG_Q5,			 32, 64 },
	{ "s23",		 ARM_REG_S23,		ARM_REG_Q5,			 32, 96 },
	{ "s24",		 ARM_REG_S24,		ARM_REG_Q6,			 32, 0 },
	{ "s25",		 ARM_REG_S25,		ARM_REG_Q6,			 32, 32 },
	{ "s26",		 ARM_REG_S26,		ARM_REG_Q6,			 32, 64 },
	{ "s27",		 ARM_REG_S27,		ARM_REG_Q6,			 32, 96 },
	{ "s28",		 ARM_REG_S28,		ARM_REG_Q7,			 32, 0 },
	{ "s29",		 ARM_REG_S29,		ARM_REG_Q7,			 32, 32 },
	{ "s30",		 ARM_REG_S30,		ARM_REG_Q7,			 32, 64 },
	{ "s31",		 ARM_REG_S31,		ARM_REG_Q7,			 32, 96 },

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

const NativeRegister ArmArchitecture::aSysregs[19] = {
	// independent registers
	{ "iapsr",		ARM_SYSREG_IAPSR,		ARM_SYSREG_IAPSR,		32 },
	{ nullptr,		ARM_SYSREG_IAPSR_G,} ,
	{ nullptr,		ARM_SYSREG_IAPSR_NZCVQG,},		// placeholders

	{ "eapsr",		ARM_SYSREG_EAPSR,		ARM_SYSREG_EAPSR,		32 },
	{ nullptr,		ARM_SYSREG_EAPSR_G, } ,
	{ nullptr,		ARM_SYSREG_EAPSR_NZCVQG, },		// placeholders

	{ "xpsr",		ARM_SYSREG_XPSR,		ARM_SYSREG_XPSR,		32 },
	{ nullptr,		ARM_SYSREG_XPSR_G, } ,
	{ nullptr,		ARM_SYSREG_XPSR_NZCVQG, },		// placeholders

	{ "ipsr",		ARM_SYSREG_IPSR,		ARM_SYSREG_IPSR,		32 },
	{ "epsr",		ARM_SYSREG_EPSR,		ARM_SYSREG_EPSR,		32 },
	{ "iepsr",		ARM_SYSREG_IEPSR,		ARM_SYSREG_IEPSR,		32 },
	{ "msp",		ARM_SYSREG_MSP,			ARM_SYSREG_MSP,			32 },

	{ "psp",		ARM_SYSREG_PSP,			ARM_SYSREG_PSP,			32 },
	{ "primask",	ARM_SYSREG_PRIMASK,		ARM_SYSREG_PRIMASK,		32 },
	{ "basepri",	ARM_SYSREG_BASEPRI,		ARM_SYSREG_BASEPRI,		32 },
	{ "basepri_max",	ARM_SYSREG_BASEPRI_MAX,		ARM_SYSREG_BASEPRI_MAX,		32 },
	{ "faultmask",	ARM_SYSREG_FAULTMASK,	ARM_SYSREG_FAULTMASK,	32 },

	{ "control",	ARM_SYSREG_CONTROL,		ARM_SYSREG_CONTROL,		32 },
};

/*
// SPSR* registers can be OR combined
ARM_SYSREG_SPSR_C = 1,
ARM_SYSREG_SPSR_X = 2,
ARM_SYSREG_SPSR_S = 4,
ARM_SYSREG_SPSR_F = 8,

// CPSR* registers can be OR combined
ARM_SYSREG_CPSR_C = 16,
ARM_SYSREG_CPSR_X = 32,
ARM_SYSREG_CPSR_S = 64,
ARM_SYSREG_CPSR_F = 128,
*/