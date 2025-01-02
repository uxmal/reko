/*
* Copyright (C) 1999-2025 John Källén.
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
#include "Arm64Rewriter.h"
#include "Arm64Disassembler.h"
#include "Arm64Architecture.h"
#include "functions.h"

Arm64Architecture::Arm64Architecture()
{
	AddRef();
}

STDMETHODIMP Arm64Architecture::QueryInterface(REFIID riid, void ** ppvObject)
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


STDMETHODIMP Arm64Architecture::GetAllRegisters(int regKind, int * pcRegs, void ** ppRegs)
{
	*pcRegs = ARM64_REG_ENDING;
	*ppRegs = const_cast<NativeRegister*>(&aRegs[0]);
	return S_OK;
}

INativeDisassembler * STDMETHODCALLTYPE Arm64Architecture::CreateDisassembler(
	void * bytes, int length, int offset, uint64_t uAddr)
{
	auto dasm = new Arm64Disassembler(reinterpret_cast<uint8_t*>(bytes) + offset, length - offset, offset, uAddr);
	dasm->AddRef();
	return dasm;
}

INativeRewriter * STDAPICALLTYPE Arm64Architecture::CreateRewriter(
	void * rawBytes,
	int32_t length,
	int32_t offset,
	uint64_t address,
	INativeRtlEmitter * m,
	INativeTypeFactory * typeFactory,
	INativeRewriterHost * host)
{
	return new Arm64Rewriter(reinterpret_cast<uint8_t*>(rawBytes) + offset, length - offset, address, m, typeFactory, host);
}

const NativeRegister Arm64Architecture::aRegs[] = {
	{ nullptr,		 ARM64_REG_INVALID,    0, },

	{ "x29",	ARM64_REG_X29,	ARM64_REG_X29,	64 },
	{ "x30",	ARM64_REG_X30,	ARM64_REG_X30,	64 },
	{ "NZCV",	ARM64_REG_NZCV, ARM64_REG_NZCV,	8 },
	{ "sp",		ARM64_REG_SP,	ARM64_REG_SP,	64 },
	{ "wsp",	ARM64_REG_WSP,	ARM64_REG_SP,	32 },
	{ "wzr",	ARM64_REG_WZR,	ARM64_REG_XZR,	64},
	{ "xzr",	ARM64_REG_XZR,	ARM64_REG_XZR,	32 },
	{ "b0",		ARM64_REG_B0,	ARM64_REG_Q0,	8, 0 },
	{ "b1",		ARM64_REG_B1, 	ARM64_REG_Q1,	8, 0 },
	{ "b2",		ARM64_REG_B2, 	ARM64_REG_Q2,	8, 0 },
	{ "b3",		ARM64_REG_B3, 	ARM64_REG_Q3,	8, 0 },
	{ "b4",		ARM64_REG_B4, 	ARM64_REG_Q4,	8, 0 },
	{ "b5",		ARM64_REG_B5, 	ARM64_REG_Q5,	8, 0 },
	{ "b6",		ARM64_REG_B6, 	ARM64_REG_Q6,	8, 0 },
	{ "b7",		ARM64_REG_B7, 	ARM64_REG_Q7,	8, 0 },
	{ "b8",		ARM64_REG_B8, 	ARM64_REG_Q8,	8, 0 },
	{ "b9",		ARM64_REG_B9, 	ARM64_REG_Q9,	8, 0 },
	{ "b10",	ARM64_REG_B10,	ARM64_REG_Q10,	8, 0 },
	{ "b11",	ARM64_REG_B11,	ARM64_REG_Q11,	8, 0 },
	{ "b12",	ARM64_REG_B12,	ARM64_REG_Q12,	8, 0 },
	{ "b13",	ARM64_REG_B13,	ARM64_REG_Q13,	8, 0 },
	{ "b14",	ARM64_REG_B14,	ARM64_REG_Q14,	8, 0 },
	{ "b15",	ARM64_REG_B15,	ARM64_REG_Q15,	8, 0 },
	{ "b16",	ARM64_REG_B16,	ARM64_REG_Q16,	8, 0 },
	{ "b17",	ARM64_REG_B17,	ARM64_REG_Q17,	8, 0 },
	{ "b18",	ARM64_REG_B18,	ARM64_REG_Q18,	8, 0 },
	{ "b19",	ARM64_REG_B19,	ARM64_REG_Q19,	8, 0 },
	{ "b20",	ARM64_REG_B20,	ARM64_REG_Q20,	8, 0 },
	{ "b21",	ARM64_REG_B21,	ARM64_REG_Q21,	8, 0 },
	{ "b22",	ARM64_REG_B22,	ARM64_REG_Q22,	8, 0 },
	{ "b23",	ARM64_REG_B23,	ARM64_REG_Q23,	8, 0 },
	{ "b24",	ARM64_REG_B24,	ARM64_REG_Q24,	8, 0 },
	{ "b25",	ARM64_REG_B25,	ARM64_REG_Q25,	8, 0 },
	{ "b26",	ARM64_REG_B26,	ARM64_REG_Q26,	8, 0 },
	{ "b27",	ARM64_REG_B27,	ARM64_REG_Q27,	8, 0 },
	{ "b28",	ARM64_REG_B28,	ARM64_REG_Q28,	8, 0 },
	{ "b29",	ARM64_REG_B29,	ARM64_REG_Q29,	8, 0 },
	{ "b30",	ARM64_REG_B30,	ARM64_REG_Q30,	8, 0 },
	{ "b31",	ARM64_REG_B31,	ARM64_REG_Q31,	8, 0 },
	{ "d0",		ARM64_REG_D0,	ARM64_REG_Q0,	64, 0 },
	{ "d1",		ARM64_REG_D1,	ARM64_REG_Q1,	64, 0 },
	{ "d2",		ARM64_REG_D2,	ARM64_REG_Q2,	64, 0 },
	{ "d3",		ARM64_REG_D3,	ARM64_REG_Q3,	64, 0 },
	{ "d4",		ARM64_REG_D4,	ARM64_REG_Q4,	64, 0 },
	{ "d5",		ARM64_REG_D5,	ARM64_REG_Q5,	64, 0 },
	{ "d6",		ARM64_REG_D6,	ARM64_REG_Q6,	64, 0 },
	{ "d7",		ARM64_REG_D7,	ARM64_REG_Q7,	64, 0 },
	{ "d8",		ARM64_REG_D8,	ARM64_REG_Q8,	64, 0 },
	{ "d9",		ARM64_REG_D9,	ARM64_REG_Q9,	64, 0 },
	{ "d10",	ARM64_REG_D10,	ARM64_REG_Q10,	64, 0 },
	{ "d11",	ARM64_REG_D11,	ARM64_REG_Q11,	64, 0 },
	{ "d12",	ARM64_REG_D12,	ARM64_REG_Q12,	64, 0 },
	{ "d13",	ARM64_REG_D13,	ARM64_REG_Q13,	64, 0 },
	{ "d14",	ARM64_REG_D14,	ARM64_REG_Q14,	64, 0 },
	{ "d15",	ARM64_REG_D15,	ARM64_REG_Q15,	64, 0 },
	{ "d16",	ARM64_REG_D16,	ARM64_REG_Q16,	64, 0 },
	{ "d17",	ARM64_REG_D17,	ARM64_REG_Q17,	64, 0 },
	{ "d18",	ARM64_REG_D18,	ARM64_REG_Q18,	64, 0 },
	{ "d19",	ARM64_REG_D19,	ARM64_REG_Q19,	64, 0 },
	{ "d20",	ARM64_REG_D20,	ARM64_REG_Q20,	64, 0 },
	{ "d21",	ARM64_REG_D21,	ARM64_REG_Q21,	64, 0 },
	{ "d22",	ARM64_REG_D22,	ARM64_REG_Q22,	64, 0 },
	{ "d23",	ARM64_REG_D23,	ARM64_REG_Q23,	64, 0 },
	{ "d24",	ARM64_REG_D24,	ARM64_REG_Q24,	64, 0 },
	{ "d25",	ARM64_REG_D25,	ARM64_REG_Q25,	64, 0 },
	{ "d26",	ARM64_REG_D26,	ARM64_REG_Q26,	64, 0 },
	{ "d27",	ARM64_REG_D27,	ARM64_REG_Q27,	64, 0 },
	{ "d28",	ARM64_REG_D28,	ARM64_REG_Q28,	64, 0 },
	{ "d29",	ARM64_REG_D29,	ARM64_REG_Q29,	64, 0 },
	{ "d30",	ARM64_REG_D30,	ARM64_REG_Q30,	64, 0 },
	{ "d31",	ARM64_REG_D31,	ARM64_REG_Q31,	64, 0 },
	{ "h0",		ARM64_REG_H0,	ARM64_REG_Q0,	16, 0 },
	{ "h1",		ARM64_REG_H1,	ARM64_REG_Q1,	16, 0 },
	{ "h2",		ARM64_REG_H2,	ARM64_REG_Q2,	16, 0 },
	{ "h3",		ARM64_REG_H3,	ARM64_REG_Q3,	16, 0 },
	{ "h4",		ARM64_REG_H4,	ARM64_REG_Q4,	16, 0 },
	{ "h5",		ARM64_REG_H5,	ARM64_REG_Q5,	16, 0 },
	{ "h6",		ARM64_REG_H6,	ARM64_REG_Q6,	16, 0 },
	{ "h7",		ARM64_REG_H7,	ARM64_REG_Q7,	16, 0 },
	{ "h8",		ARM64_REG_H8,	ARM64_REG_Q8,	16, 0 },
	{ "h9",		ARM64_REG_H9,	ARM64_REG_Q9,	16, 0 },
	{ "h10",	ARM64_REG_H10,	ARM64_REG_Q10,	16, 0 },
	{ "h11",	ARM64_REG_H11,	ARM64_REG_Q11,	16, 0 },
	{ "h12",	ARM64_REG_H12,	ARM64_REG_Q12,	16, 0 },
	{ "h13",	ARM64_REG_H13,	ARM64_REG_Q13,	16, 0 },
	{ "h14",	ARM64_REG_H14,	ARM64_REG_Q14,	16, 0 },
	{ "h15",	ARM64_REG_H15,	ARM64_REG_Q15,	16, 0 },
	{ "h16",	ARM64_REG_H16,	ARM64_REG_Q16,	16, 0 },
	{ "h17",	ARM64_REG_H17,	ARM64_REG_Q17,	16, 0 },
	{ "h18",	ARM64_REG_H18,	ARM64_REG_Q18,	16, 0 },
	{ "h19",	ARM64_REG_H19,	ARM64_REG_Q19,	16, 0 },
	{ "h20",	ARM64_REG_H20,	ARM64_REG_Q20,	16, 0 },
	{ "h21",	ARM64_REG_H21,	ARM64_REG_Q21,	16, 0 },
	{ "h22",	ARM64_REG_H22,	ARM64_REG_Q22,	16, 0 },
	{ "h23",	ARM64_REG_H23,	ARM64_REG_Q23,	16, 0 },
	{ "h24",	ARM64_REG_H24,	ARM64_REG_Q24,	16, 0 },
	{ "h25",	ARM64_REG_H25,	ARM64_REG_Q25,	16, 0 },
	{ "h26",	ARM64_REG_H26,	ARM64_REG_Q26,	16, 0 },
	{ "h27",	ARM64_REG_H27,	ARM64_REG_Q27,	16, 0 },
	{ "h28",	ARM64_REG_H28,	ARM64_REG_Q28,	16, 0 },
	{ "h29",	ARM64_REG_H29,	ARM64_REG_Q29,	16, 0 },
	{ "h30",	ARM64_REG_H30,	ARM64_REG_Q30,	16, 0 },
	{ "h31",	ARM64_REG_H31,	ARM64_REG_Q31,	16, 0 },
	{ "q0",		ARM64_REG_Q0,	ARM64_REG_Q0,	128, 0 },
	{ "q1",		ARM64_REG_Q1,	ARM64_REG_Q1,	128, 0 },
	{ "q2",		ARM64_REG_Q2,	ARM64_REG_Q2,	128, 0 },
	{ "q3",		ARM64_REG_Q3,	ARM64_REG_Q3,	128, 0 },
	{ "q4",		ARM64_REG_Q4,	ARM64_REG_Q4,	128, 0 },
	{ "q5",		ARM64_REG_Q5,	ARM64_REG_Q5,	128, 0 },
	{ "q6",		ARM64_REG_Q6,	ARM64_REG_Q6,	128, 0 },
	{ "q7",		ARM64_REG_Q7,	ARM64_REG_Q7,	128, 0 },
	{ "q8",		ARM64_REG_Q8,	ARM64_REG_Q8,	128, 0 },
	{ "q9",		ARM64_REG_Q9,	ARM64_REG_Q9,	128, 0 },
	{ "q10",	ARM64_REG_Q10,	ARM64_REG_Q10,	128, 0 },
	{ "q11",	ARM64_REG_Q11,	ARM64_REG_Q11,	128, 0 },
	{ "q12",	ARM64_REG_Q12,	ARM64_REG_Q12,	128, 0 },
	{ "q13",	ARM64_REG_Q13,	ARM64_REG_Q13,	128, 0 },
	{ "q14",	ARM64_REG_Q14,	ARM64_REG_Q14,	128, 0 },
	{ "q15",	ARM64_REG_Q15,	ARM64_REG_Q15,	128, 0 },
	{ "q16",	ARM64_REG_Q16,	ARM64_REG_Q16,	128, 0 },
	{ "q17",	ARM64_REG_Q17,	ARM64_REG_Q17,	128, 0 },
	{ "q18",	ARM64_REG_Q18,	ARM64_REG_Q18,	128, 0 },
	{ "q19",	ARM64_REG_Q19,	ARM64_REG_Q19,	128, 0 },
	{ "q20",	ARM64_REG_Q20,	ARM64_REG_Q20,	128, 0 },
	{ "q21",	ARM64_REG_Q21,	ARM64_REG_Q21,	128, 0 },
	{ "q22",	ARM64_REG_Q22,	ARM64_REG_Q22,	128, 0 },
	{ "q23",	ARM64_REG_Q23,	ARM64_REG_Q23,	128, 0 },
	{ "q24",	ARM64_REG_Q24,	ARM64_REG_Q24,	128, 0 },
	{ "q25",	ARM64_REG_Q25,	ARM64_REG_Q25,	128, 0 },
	{ "q26",	ARM64_REG_Q26,	ARM64_REG_Q26,	128, 0 },
	{ "q27",	ARM64_REG_Q27,	ARM64_REG_Q27,	128, 0 },
	{ "q28",	ARM64_REG_Q28,	ARM64_REG_Q28,	128, 0 },
	{ "q29",	ARM64_REG_Q29,	ARM64_REG_Q29,	128, 0 },
	{ "q30",	ARM64_REG_Q30,	ARM64_REG_Q30,	128, 0 },
	{ "q31",	ARM64_REG_Q31,	ARM64_REG_Q31,	128, 0 },
	{ "s0",		ARM64_REG_S0,	ARM64_REG_Q0,	32, 0 },
	{ "s1",		ARM64_REG_S1, 	ARM64_REG_Q1,	32, 0 },
	{ "s2",		ARM64_REG_S2, 	ARM64_REG_Q2,	32, 0 },
	{ "s3",		ARM64_REG_S3, 	ARM64_REG_Q3,	32, 0 },
	{ "s4",		ARM64_REG_S4, 	ARM64_REG_Q4,	32, 0 },
	{ "s5",		ARM64_REG_S5, 	ARM64_REG_Q5,	32, 0 },
	{ "s6",		ARM64_REG_S6, 	ARM64_REG_Q6,	32, 0 },
	{ "s7",		ARM64_REG_S7, 	ARM64_REG_Q7,	32, 0 },
	{ "s8",		ARM64_REG_S8, 	ARM64_REG_Q8,	32, 0 },
	{ "s9",		ARM64_REG_S9, 	ARM64_REG_Q9,	32, 0 },
	{ "s10",	ARM64_REG_S10,	ARM64_REG_Q10,	32, 0 },
	{ "s11",	ARM64_REG_S11,	ARM64_REG_Q11,	32, 0 },
	{ "s12",	ARM64_REG_S12,	ARM64_REG_Q12,	32, 0 },
	{ "s13",	ARM64_REG_S13,	ARM64_REG_Q13,	32, 0 },
	{ "s14",	ARM64_REG_S14,	ARM64_REG_Q14,	32, 0 },
	{ "s15",	ARM64_REG_S15,	ARM64_REG_Q15,	32, 0 },
	{ "s16",	ARM64_REG_S16,	ARM64_REG_Q16,	32, 0 },
	{ "s17",	ARM64_REG_S17,	ARM64_REG_Q17,	32, 0 },
	{ "s18",	ARM64_REG_S18,	ARM64_REG_Q18,	32, 0 },
	{ "s19",	ARM64_REG_S19,	ARM64_REG_Q19,	32, 0 },
	{ "s20",	ARM64_REG_S20,	ARM64_REG_Q20,	32, 0 },
	{ "s21",	ARM64_REG_S21,	ARM64_REG_Q21,	32, 0 },
	{ "s22",	ARM64_REG_S22,	ARM64_REG_Q22,	32, 0 },
	{ "s23",	ARM64_REG_S23,	ARM64_REG_Q23,	32, 0 },
	{ "s24",	ARM64_REG_S24,	ARM64_REG_Q24,	32, 0 },
	{ "s25",	ARM64_REG_S25,	ARM64_REG_Q25,	32, 0 },
	{ "s26",	ARM64_REG_S26,	ARM64_REG_Q26,	32, 0 },
	{ "s27",	ARM64_REG_S27,	ARM64_REG_Q27,	32, 0 },
	{ "s28",	ARM64_REG_S28,	ARM64_REG_Q28,	32, 0 },
	{ "s29",	ARM64_REG_S29,	ARM64_REG_Q29,	32, 0 },
	{ "s30",	ARM64_REG_S30,	ARM64_REG_Q30,	32, 0 },
	{ "s31",	ARM64_REG_S31,	ARM64_REG_Q31,	32, 0 },
	{ "w0",		ARM64_REG_W0,	ARM64_REG_X0,	32 },
	{ "w1",		ARM64_REG_W1,	ARM64_REG_X1,	32 },
	{ "w2",		ARM64_REG_W2,	ARM64_REG_X2,	32 },
	{ "w3",		ARM64_REG_W3,	ARM64_REG_X3,	32 },
	{ "w4",		ARM64_REG_W4,	ARM64_REG_X4,	32 },
	{ "w5",		ARM64_REG_W5,	ARM64_REG_X5,	32 },
	{ "w6",		ARM64_REG_W6,	ARM64_REG_X6,	32 },
	{ "w7",		ARM64_REG_W7,	ARM64_REG_X7,	32 },
	{ "w8",		ARM64_REG_W8,	ARM64_REG_X8,	32 },
	{ "w9",		ARM64_REG_W9,	ARM64_REG_X9,	32 },
	{ "w10",	ARM64_REG_W10,	ARM64_REG_X10,	32 },
	{ "w11",	ARM64_REG_W11,	ARM64_REG_X11,	32 },
	{ "w12",	ARM64_REG_W12,	ARM64_REG_X12,	32 },
	{ "w13",	ARM64_REG_W13,	ARM64_REG_X13,	32 },
	{ "w14",	ARM64_REG_W14,	ARM64_REG_X14,	32 },
	{ "w15",	ARM64_REG_W15,	ARM64_REG_X15,	32 },
	{ "w16",	ARM64_REG_W16,	ARM64_REG_X16,	32 },
	{ "w17",	ARM64_REG_W17,	ARM64_REG_X17,	32 },
	{ "w18",	ARM64_REG_W18,	ARM64_REG_X18,	32 },
	{ "w19",	ARM64_REG_W19,	ARM64_REG_X19,	32 },
	{ "w20",	ARM64_REG_W20,	ARM64_REG_X20,	32 },
	{ "w21",	ARM64_REG_W21,	ARM64_REG_X21,	32 },
	{ "w22",	ARM64_REG_W22,	ARM64_REG_X22,	32 },
	{ "w23",	ARM64_REG_W23,	ARM64_REG_X23,	32 },
	{ "w24",	ARM64_REG_W24,	ARM64_REG_X24,	32 },
	{ "w25",	ARM64_REG_W25,	ARM64_REG_X25,	32 },
	{ "w26",	ARM64_REG_W26,	ARM64_REG_X26,	32 },
	{ "w27",	ARM64_REG_W27,	ARM64_REG_X27,	32 },
	{ "w28",	ARM64_REG_W28,	ARM64_REG_X28,	32 },
	{ "w29",	ARM64_REG_W29,	ARM64_REG_X29,	32 },
	{ "w30",	ARM64_REG_W30,	ARM64_REG_X30,	32 },
	{ "x0",		ARM64_REG_X0,	ARM64_REG_X0,	64 },
	{ "x1",		ARM64_REG_X1,	ARM64_REG_X1,	64 },
	{ "x2",		ARM64_REG_X2,	ARM64_REG_X2,	64 },
	{ "x3",		ARM64_REG_X3,	ARM64_REG_X3,	64 },
	{ "x4",		ARM64_REG_X4,	ARM64_REG_X4,	64 },
	{ "x5",		ARM64_REG_X5,	ARM64_REG_X5,	64 },
	{ "x6",		ARM64_REG_X6,	ARM64_REG_X6,	64 },
	{ "x7",		ARM64_REG_X7,	ARM64_REG_X7,	64 },
	{ "x8",		ARM64_REG_X8,	ARM64_REG_X8,	64 },
	{ "x9",		ARM64_REG_X9,	ARM64_REG_X9,	64 },
	{ "x10",	ARM64_REG_X10,	ARM64_REG_X10,	64 },
	{ "x11",	ARM64_REG_X11,	ARM64_REG_X11,	64 },
	{ "x12",	ARM64_REG_X12,	ARM64_REG_X12,	64 },
	{ "x13",	ARM64_REG_X13,	ARM64_REG_X13,	64 },
	{ "x14",	ARM64_REG_X14,	ARM64_REG_X14,	64 },
	{ "x15",	ARM64_REG_X15,	ARM64_REG_X15,	64 },
	{ "x16",	ARM64_REG_X16,	ARM64_REG_X16,	64 },
	{ "x17",	ARM64_REG_X17,	ARM64_REG_X17,	64 },
	{ "x18",	ARM64_REG_X18,	ARM64_REG_X18,	64 },
	{ "x19",	ARM64_REG_X19,	ARM64_REG_X19,	64 },
	{ "x20",	ARM64_REG_X20,	ARM64_REG_X20,	64 },
	{ "x21",	ARM64_REG_X21,	ARM64_REG_X21,	64 },
	{ "x22",	ARM64_REG_X22,	ARM64_REG_X22,	64 },
	{ "x23",	ARM64_REG_X23,	ARM64_REG_X23,	64 },
	{ "x24",	ARM64_REG_X24,	ARM64_REG_X24,	64 },
	{ "x25",	ARM64_REG_X25,	ARM64_REG_X25,	64 },
	{ "x26",	ARM64_REG_X26,	ARM64_REG_X26,	64 },
	{ "x27",	ARM64_REG_X27,	ARM64_REG_X27,	64 },
	{ "x28",	ARM64_REG_X28,	ARM64_REG_X28,	64 },
	{ "v0",		ARM64_REG_V0,	ARM64_REG_Q0,	128 },
	{ "v1",		ARM64_REG_V1,	ARM64_REG_Q1,	128 },
	{ "v2",		ARM64_REG_V2,	ARM64_REG_Q2,	128 },
	{ "v3",		ARM64_REG_V3,	ARM64_REG_Q3,	128 },
	{ "v4",		ARM64_REG_V4,	ARM64_REG_Q4,	128 },
	{ "v5",		ARM64_REG_V5,	ARM64_REG_Q5,	128 },
	{ "v6",		ARM64_REG_V6,	ARM64_REG_Q6,	128 },
	{ "v7",		ARM64_REG_V7,	ARM64_REG_Q7,	128 },
	{ "v8",		ARM64_REG_V8,	ARM64_REG_Q8,	128 },
	{ "v9",		ARM64_REG_V9,	ARM64_REG_Q9,	128 },
	{ "v10",	ARM64_REG_V10,	ARM64_REG_Q10,	128 },
	{ "v11",	ARM64_REG_V11,	ARM64_REG_Q11,	128 },
	{ "v12",	ARM64_REG_V12,	ARM64_REG_Q12,	128 },
	{ "v13",	ARM64_REG_V13,	ARM64_REG_Q13,	128 },
	{ "v14",	ARM64_REG_V14,	ARM64_REG_Q14,	128 },
	{ "v15",	ARM64_REG_V15,	ARM64_REG_Q15,	128 },
	{ "v16",	ARM64_REG_V16,	ARM64_REG_Q16,	128 },
	{ "v17",	ARM64_REG_V17,	ARM64_REG_Q17,	128 },
	{ "v18",	ARM64_REG_V18,	ARM64_REG_Q18,	128 },
	{ "v19",	ARM64_REG_V19,	ARM64_REG_Q19,	128 },
	{ "v20",	ARM64_REG_V20,	ARM64_REG_Q20,	128 },
	{ "v21",	ARM64_REG_V21,	ARM64_REG_Q21,	128 },
	{ "v22",	ARM64_REG_V22,	ARM64_REG_Q22,	128 },
	{ "v23",	ARM64_REG_V23,	ARM64_REG_Q23,	128 },
	{ "v24",	ARM64_REG_V24,	ARM64_REG_Q24,	128 },
	{ "v25",	ARM64_REG_V25,	ARM64_REG_Q25,	128 },
	{ "v26",	ARM64_REG_V26,	ARM64_REG_Q26,	128 },
	{ "v27",	ARM64_REG_V27,	ARM64_REG_Q27,	128 },
	{ "v28",	ARM64_REG_V28,	ARM64_REG_Q28,	128 },
	{ "v29",	ARM64_REG_V29,	ARM64_REG_Q29,	128 },
	{ "v30",	ARM64_REG_V30,	ARM64_REG_Q30,	128 },
	{ "v31",	ARM64_REG_V31,	ARM64_REG_Q31,  128 },


		//> alias registers

//		ARM64_REG_IP1 = ARM64_REG_X16,
//		ARM64_REG_IP0 = ARM64_REG_X17,
//		ARM64_REG_FP = ARM64_REG_X29,
//		ARM64_REG_LR = ARM64_REG_X30,
};