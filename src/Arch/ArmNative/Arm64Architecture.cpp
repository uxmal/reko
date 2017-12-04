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


void STDMETHODCALLTYPE Arm64Architecture::GetAllRegisters(int * pcRegs, const NativeRegister ** ppRegs)
{
	*pcRegs = ARM_REG_ENDING;
	*ppRegs = &aRegs[0];
}

INativeDisassembler * STDMETHODCALLTYPE Arm64Architecture::CreateDisassembler(
	const uint8_t * bytes, int length, int offset, uint64_t uAddr)
{
	auto dasm = new Arm64Disassembler(bytes + offset, length - offset, offset, uAddr);
	dasm->AddRef();
	return dasm;
}

INativeRewriter * STDAPICALLTYPE Arm64Architecture::CreateRewriter(
	const uint8_t * rawBytes,
	uint32_t length,
	uint32_t offset,
	uint64_t address,
	INativeRtlEmitter * m,
	INativeTypeFactory * typeFactory,
	INativeRewriterHost * host)
{
	return new Arm64Rewriter(rawBytes + offset, length - offset, address, m, typeFactory, host);
}

const NativeRegister Arm64Architecture::aRegs[] = {
	{ nullptr,		 ARM_REG_INVALID,    0, },

	{ "x29", ARM64_REG_X29, 0 },
	{ "x30", ARM64_REG_X30, 0 },
	{ "NZCV", ARM64_REG_NZCV, 0 },
	{ "sp", ARM64_REG_SP, 0 },
	{ "wsp", ARM64_REG_WSP, 0 },
	{ "wzr", ARM64_REG_WZR, 0 },
	{ "xzr", ARM64_REG_XZR, 0 },
	{ "b0", ARM64_REG_B0, 0 },
	{ "b1", ARM64_REG_B1, 0 },
	{ "b2", ARM64_REG_B2, 0 },
	{ "b3", ARM64_REG_B3, 0 },
	{ "b4", ARM64_REG_B4, 0 },
	{ "b5", ARM64_REG_B5, 0 },
	{ "b6", ARM64_REG_B6, 0 },
	{ "b7", ARM64_REG_B7, 0 },
	{ "b8", ARM64_REG_B8, 0 },
	{ "b9", ARM64_REG_B9, 0 },
	{ "b10", ARM64_REG_B10, 0 },
	{ "b11", ARM64_REG_B11, 0 },
	{ "b12", ARM64_REG_B12, 0 },
	{ "b13", ARM64_REG_B13, 0 },
	{ "b14", ARM64_REG_B14, 0 },
	{ "b15", ARM64_REG_B15, 0 },
	{ "b16", ARM64_REG_B16, 0 },
	{ "b17", ARM64_REG_B17, 0 },
	{ "b18", ARM64_REG_B18, 0 },
	{ "b19", ARM64_REG_B19, 0 },
	{ "b20", ARM64_REG_B20, 0 },
	{ "b21", ARM64_REG_B21, 0 },
	{ "b22", ARM64_REG_B22, 0 },
	{ "b23", ARM64_REG_B23, 0 },
	{ "b24", ARM64_REG_B24, 0 },
	{ "b25", ARM64_REG_B25, 0 },
	{ "b26", ARM64_REG_B26, 0 },
	{ "b27", ARM64_REG_B27, 0 },
	{ "b28", ARM64_REG_B28, 0 },
	{ "b29", ARM64_REG_B29, 0 },
	{ "b30", ARM64_REG_B30, 0 },
	{ "b31", ARM64_REG_B31, 0 },
	{ "d0", ARM64_REG_D0, 0 },
	{ "d1", ARM64_REG_D1, 0 },
	{ "d2", ARM64_REG_D2, 0 },
	{ "d3", ARM64_REG_D3, 0 },
	{ "d4", ARM64_REG_D4, 0 },
	{ "d5", ARM64_REG_D5, 0 },
	{ "d6", ARM64_REG_D6, 0 },
	{ "d7", ARM64_REG_D7, 0 },
	{ "d8", ARM64_REG_D8, 0 },
	{ "d9", ARM64_REG_D9, 0 },
	{ "d10", ARM64_REG_D10, 0 },
	{ "d11", ARM64_REG_D11, 0 },
	{ "d12", ARM64_REG_D12, 0 },
	{ "d13", ARM64_REG_D13, 0 },
	{ "d14", ARM64_REG_D14, 0 },
	{ "d15", ARM64_REG_D15, 0 },
	{ "d16", ARM64_REG_D16, 0 },
	{ "d17", ARM64_REG_D17, 0 },
	{ "d18", ARM64_REG_D18, 0 },
	{ "d19", ARM64_REG_D19, 0 },
	{ "d20", ARM64_REG_D20, 0 },
	{ "d21", ARM64_REG_D21, 0 },
	{ "d22", ARM64_REG_D22, 0 },
	{ "d23", ARM64_REG_D23, 0 },
	{ "d24", ARM64_REG_D24, 0 },
	{ "d25", ARM64_REG_D25, 0 },
	{ "d26", ARM64_REG_D26, 0 },
	{ "d27", ARM64_REG_D27, 0 },
	{ "d28", ARM64_REG_D28, 0 },
	{ "d29", ARM64_REG_D29, 0 },
	{ "d30", ARM64_REG_D30, 0 },
	{ "d31", ARM64_REG_D31, 0 },
	{ "h0", ARM64_REG_H0, 0 },
	{ "h1", ARM64_REG_H1, 0 },
	{ "h2", ARM64_REG_H2, 0 },
	{ "h3", ARM64_REG_H3, 0 },
	{ "h4", ARM64_REG_H4, 0 },
	{ "h5", ARM64_REG_H5, 0 },
	{ "h6", ARM64_REG_H6, 0 },
	{ "h7", ARM64_REG_H7, 0 },
	{ "h8", ARM64_REG_H8, 0 },
	{ "h9", ARM64_REG_H9, 0 },
	{ "h10", ARM64_REG_H10, 0 },
	{ "h11", ARM64_REG_H11, 0 },
	{ "h12", ARM64_REG_H12, 0 },
	{ "h13", ARM64_REG_H13, 0 },
	{ "h14", ARM64_REG_H14, 0 },
	{ "h15", ARM64_REG_H15, 0 },
	{ "h16", ARM64_REG_H16, 0 },
	{ "h17", ARM64_REG_H17, 0 },
	{ "h18", ARM64_REG_H18, 0 },
	{ "h19", ARM64_REG_H19, 0 },
	{ "h20", ARM64_REG_H20, 0 },
	{ "h21", ARM64_REG_H21, 0 },
	{ "h22", ARM64_REG_H22, 0 },
	{ "h23", ARM64_REG_H23, 0 },
	{ "h24", ARM64_REG_H24, 0 },
	{ "h25", ARM64_REG_H25, 0 },
	{ "h26", ARM64_REG_H26, 0 },
	{ "h27", ARM64_REG_H27, 0 },
	{ "h28", ARM64_REG_H28, 0 },
	{ "h29", ARM64_REG_H29, 0 },
	{ "h30", ARM64_REG_H30, 0 },
	{ "h31", ARM64_REG_H31, 0 },
	{ "q0", ARM64_REG_Q0, 0 },
	{ "q1", ARM64_REG_Q1, 0 },
	{ "q2", ARM64_REG_Q2, 0 },
	{ "q3", ARM64_REG_Q3, 0 },
	{ "q4", ARM64_REG_Q4, 0 },
	{ "q5", ARM64_REG_Q5, 0 },
	{ "q6", ARM64_REG_Q6, 0 },
	{ "q7", ARM64_REG_Q7, 0 },
	{ "q8", ARM64_REG_Q8, 0 },
	{ "q9", ARM64_REG_Q9, 0 },
	{ "q10", ARM64_REG_Q10, 0 },
	{ "q11", ARM64_REG_Q11, 0 },
	{ "q12", ARM64_REG_Q12, 0 },
	{ "q13", ARM64_REG_Q13, 0 },
	{ "q14", ARM64_REG_Q14, 0 },
	{ "q15", ARM64_REG_Q15, 0 },
	{ "q16", ARM64_REG_Q16, 0 },
	{ "q17", ARM64_REG_Q17, 0 },
	{ "q18", ARM64_REG_Q18, 0 },
	{ "q19", ARM64_REG_Q19, 0 },
	{ "q20", ARM64_REG_Q20, 0 },
	{ "q21", ARM64_REG_Q21, 0 },
	{ "q22", ARM64_REG_Q22, 0 },
	{ "q23", ARM64_REG_Q23, 0 },
	{ "q24", ARM64_REG_Q24, 0 },
	{ "q25", ARM64_REG_Q25, 0 },
	{ "q26", ARM64_REG_Q26, 0 },
	{ "q27", ARM64_REG_Q27, 0 },
	{ "q28", ARM64_REG_Q28, 0 },
	{ "q29", ARM64_REG_Q29, 0 },
	{ "q30", ARM64_REG_Q30, 0 },
	{ "q31", ARM64_REG_Q31, 0 },
	{ "s0", ARM64_REG_S0, 0 },
	{ "s1", ARM64_REG_S1, 0 },
	{ "s2", ARM64_REG_S2, 0 },
	{ "s3", ARM64_REG_S3, 0 },
	{ "s4", ARM64_REG_S4, 0 },
	{ "s5", ARM64_REG_S5, 0 },
	{ "s6", ARM64_REG_S6, 0 },
	{ "s7", ARM64_REG_S7, 0 },
	{ "s8", ARM64_REG_S8, 0 },
	{ "s9", ARM64_REG_S9, 0 },
	{ "s10", ARM64_REG_S10, 0 },
	{ "s11", ARM64_REG_S11, 0 },
	{ "s12", ARM64_REG_S12, 0 },
	{ "s13", ARM64_REG_S13, 0 },
	{ "s14", ARM64_REG_S14, 0 },
	{ "s15", ARM64_REG_S15, 0 },
	{ "s16", ARM64_REG_S16, 0 },
	{ "s17", ARM64_REG_S17, 0 },
	{ "s18", ARM64_REG_S18, 0 },
	{ "s19", ARM64_REG_S19, 0 },
	{ "s20", ARM64_REG_S20, 0 },
	{ "s21", ARM64_REG_S21, 0 },
	{ "s22", ARM64_REG_S22, 0 },
	{ "s23", ARM64_REG_S23, 0 },
	{ "s24", ARM64_REG_S24, 0 },
	{ "s25", ARM64_REG_S25, 0 },
	{ "s26", ARM64_REG_S26, 0 },
	{ "s27", ARM64_REG_S27, 0 },
	{ "s28", ARM64_REG_S28, 0 },
	{ "s29", ARM64_REG_S29, 0 },
	{ "s30", ARM64_REG_S30, 0 },
	{ "s31", ARM64_REG_S31, 0 },
	{ "w0", ARM64_REG_W0, 0 },
	{ "w1", ARM64_REG_W1, 0 },
	{ "w2", ARM64_REG_W2, 0 },
	{ "w3", ARM64_REG_W3, 0 },
	{ "w4", ARM64_REG_W4, 0 },
	{ "w5", ARM64_REG_W5, 0 },
	{ "w6", ARM64_REG_W6, 0 },
	{ "w7", ARM64_REG_W7, 0 },
	{ "w8", ARM64_REG_W8, 0 },
	{ "w9", ARM64_REG_W9, 0 },
	{ "w10", ARM64_REG_W10, 0 },
	{ "w11", ARM64_REG_W11, 0 },
	{ "w12", ARM64_REG_W12, 0 },
	{ "w13", ARM64_REG_W13, 0 },
	{ "w14", ARM64_REG_W14, 0 },
	{ "w15", ARM64_REG_W15, 0 },
	{ "w16", ARM64_REG_W16, 0 },
	{ "w17", ARM64_REG_W17, 0 },
	{ "w18", ARM64_REG_W18, 0 },
	{ "w19", ARM64_REG_W19, 0 },
	{ "w20", ARM64_REG_W20, 0 },
	{ "w21", ARM64_REG_W21, 0 },
	{ "w22", ARM64_REG_W22, 0 },
	{ "w23", ARM64_REG_W23, 0 },
	{ "w24", ARM64_REG_W24, 0 },
	{ "w25", ARM64_REG_W25, 0 },
	{ "w26", ARM64_REG_W26, 0 },
	{ "w27", ARM64_REG_W27, 0 },
	{ "w28", ARM64_REG_W28, 0 },
	{ "w29", ARM64_REG_W29, 0 },
	{ "w30", ARM64_REG_W30, 0 },
	{ "x0", ARM64_REG_X0, 0 },
	{ "x1", ARM64_REG_X1, 0 },
	{ "x2", ARM64_REG_X2, 0 },
	{ "x3", ARM64_REG_X3, 0 },
	{ "x4", ARM64_REG_X4, 0 },
	{ "x5", ARM64_REG_X5, 0 },
	{ "x6", ARM64_REG_X6, 0 },
	{ "x7", ARM64_REG_X7, 0 },
	{ "x8", ARM64_REG_X8, 0 },
	{ "x9", ARM64_REG_X9, 0 },
	{ "x10", ARM64_REG_X10, 0 },
	{ "x11", ARM64_REG_X11, 0 },
	{ "x12", ARM64_REG_X12, 0 },
	{ "x13", ARM64_REG_X13, 0 },
	{ "x14", ARM64_REG_X14, 0 },
	{ "x15", ARM64_REG_X15, 0 },
	{ "x16", ARM64_REG_X16, 0 },
	{ "x17", ARM64_REG_X17, 0 },
	{ "x18", ARM64_REG_X18, 0 },
	{ "x19", ARM64_REG_X19, 0 },
	{ "x20", ARM64_REG_X20, 0 },
	{ "x21", ARM64_REG_X21, 0 },
	{ "x22", ARM64_REG_X22, 0 },
	{ "x23", ARM64_REG_X23, 0 },
	{ "x24", ARM64_REG_X24, 0 },
	{ "x25", ARM64_REG_X25, 0 },
	{ "x26", ARM64_REG_X26, 0 },
	{ "x27", ARM64_REG_X27, 0 },
	{ "x28", ARM64_REG_X28, 0 },

	{ "v0", ARM64_REG_V0, 0 },
	{ "v1", ARM64_REG_V1, 0 },
	{ "v2", ARM64_REG_V2, 0 },
	{ "v3", ARM64_REG_V3, 0 },
	{ "v4", ARM64_REG_V4, 0 },
	{ "v5", ARM64_REG_V5, 0 },
	{ "v6", ARM64_REG_V6, 0 },
	{ "v7", ARM64_REG_V7, 0 },
	{ "v8", ARM64_REG_V8, 0 },
	{ "v9", ARM64_REG_V9, 0 },
	{ "v10", ARM64_REG_V10, 0 },
	{ "v11", ARM64_REG_V11, 0 },
	{ "v12", ARM64_REG_V12, 0 },
	{ "v13", ARM64_REG_V13, 0 },
	{ "v14", ARM64_REG_V14, 0 },
	{ "v15", ARM64_REG_V15, 0 },
	{ "v16", ARM64_REG_V16, 0 },
	{ "v17", ARM64_REG_V17, 0 },
	{ "v18", ARM64_REG_V18, 0 },
	{ "v19", ARM64_REG_V19, 0 },
	{ "v20", ARM64_REG_V20, 0 },
	{ "v21", ARM64_REG_V21, 0 },
	{ "v22", ARM64_REG_V22, 0 },
	{ "v23", ARM64_REG_V23, 0 },
	{ "v24", ARM64_REG_V24, 0 },
	{ "v25", ARM64_REG_V25, 0 },
	{ "v26", ARM64_REG_V26, 0 },
	{ "v27", ARM64_REG_V27, 0 },
	{ "v28", ARM64_REG_V28, 0 },
	{ "v29", ARM64_REG_V29, 0 },
	{ "v30", ARM64_REG_V30, 0 },
	{ "v31", ARM64_REG_V31, 0 },


		//> alias registers

//		ARM64_REG_IP1 = ARM64_REG_X16,
//		ARM64_REG_IP0 = ARM64_REG_X17,
//		ARM64_REG_FP = ARM64_REG_X29,
//		ARM64_REG_LR = ARM64_REG_X30,
};