/*
* Copyright (C) 1999-2024 John K�ll�n.
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

#include "functions.h"
#include "ComBase.h"
#include "Arm64Rewriter.h"


Arm64Rewriter::Arm64Rewriter(
	const uint8_t * rawBytes,
	size_t availableBytes,
	uint64_t address,
	INativeRtlEmitter * emitter,
	INativeTypeFactory * ntf,
	INativeRewriterHost * host)
:
	rawBytes(rawBytes),
	available(availableBytes),
	address(address),
	m(*emitter),
	ntf(*ntf),
	host(host),
	cRef(1),
	instr(nullptr)
{
	//Dump(".ctor: %08x", this);
	auto ec = cs_open(CS_ARCH_ARM64, CS_MODE_ARM, &hcapstone);
	ec = cs_option(hcapstone, CS_OPT_DETAIL, CS_OPT_ON);
	this->instr = cs_malloc(hcapstone);
	++s_count;
}


STDMETHODIMP Arm64Rewriter::QueryInterface(REFIID riid, void ** ppvOut)
{
	//Dump("QI: %08x %d", this, cRef);
	*ppvOut = nullptr;
	if (riid == IID_IUnknown || riid == IID_INativeRewriter)
	{
		AddRef();
		*ppvOut = static_cast<INativeRewriter *>(this);
		return S_OK;
	}
	return E_NOINTERFACE;
}

STDMETHODIMP_(int32_t) Arm64Rewriter::Next()
{
	Dump("Next: %08x", this);
	if (available == 0)
		return S_FALSE;			// No more work to do.
	auto addrInstr = address;
	bool f = cs_disasm_iter(hcapstone, &rawBytes, &available, &address, instr);
	if (!f)
	{
		// Failed to disassemble the instruction because it was invalid.
		m.Invalid();
		m.FinishCluster(InstrClass::Invalid, addrInstr, 4);
		return S_OK;
	}
	// Most instructions are linear.
	rtlClass = InstrClass::Linear;
	
#if 0
	switch (instr->id)
	{
	default:
		NotImplementedYet();
		break;
	}
#else
	NotImplementedYet();
#endif
	m.FinishCluster(rtlClass, addrInstr, instr->size);
	return S_OK;
}

void Arm64Rewriter::NotImplementedYet()
{
	char buf[200];	//$TODO: hello buffer overflow!
	::snprintf(buf, sizeof(buf), "Rewriting ARM opcode '%s' is not supported yet.", instr->mnemonic);
	EmitUnitTest();
	host->Error(
		instr->address,
		buf);
	m.Invalid();
}

HExpr Arm64Rewriter::NZCV()
{
	return host->EnsureFlagGroup((int)ARM_REG_CPSR, 0xF, "NZCV");
}

void Arm64Rewriter::MaybeUpdateFlags(HExpr opDst)
{
	if (instr->detail->arm.update_flags)
	{
		m.Assign(NZCV(), m.Cond(opDst));
	}
}


arm_cc Arm64Rewriter::Invert(arm_cc cc)
{
	switch (cc)
	{
	case ARM_CC_EQ: return ARM_CC_NE;
	case ARM_CC_NE: return ARM_CC_EQ;
	case ARM_CC_HS: return ARM_CC_LO;
	case ARM_CC_LO: return ARM_CC_HS;
	case ARM_CC_MI: return ARM_CC_PL;
	case ARM_CC_PL: return ARM_CC_MI;
	case ARM_CC_VS: return ARM_CC_VC;
	case ARM_CC_VC: return ARM_CC_VS;
	case ARM_CC_HI: return ARM_CC_LS;
	case ARM_CC_LS: return ARM_CC_HI;
	case ARM_CC_GE: return ARM_CC_LT;
	case ARM_CC_LT: return ARM_CC_GE;
	case ARM_CC_GT: return ARM_CC_LE;
	case ARM_CC_LE: return ARM_CC_GT;
	case ARM_CC_AL: return ARM_CC_INVALID;
	}
	return ARM_CC_INVALID;
}

bool Arm64Rewriter::IsLastOperand(const cs_arm_op & op)
{
	return &op == &instr->detail->arm.operands[instr->detail->arm.op_count - 1];
}

HExpr Arm64Rewriter::Operand(const cs_arm_op & op)
{
	//$TODO
	return HExpr();
}

HExpr Arm64Rewriter::TestCond(arm_cc cond)
{
	switch (cond)
	{
	//default:
	//	throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
	case ARM_CC_HS:
		return m.Test(ConditionCode::UGE, FlagGroup(FlagM::CF, "C"));
	case ARM_CC_LO:
		return m.Test(ConditionCode::ULT, FlagGroup(FlagM::CF, "C"));
	case ARM_CC_EQ:
		return m.Test(ConditionCode::EQ, FlagGroup(FlagM::ZF, "Z"));
	case ARM_CC_GE:
		return m.Test(ConditionCode::GE, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV"));
	case ARM_CC_GT:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV"));
	case ARM_CC_HI:
		return m.Test(ConditionCode::UGT, FlagGroup(FlagM::ZF | FlagM::CF, "ZC"));
	case ARM_CC_LE:
		return m.Test(ConditionCode::LE, FlagGroup(FlagM::ZF | FlagM::CF | FlagM::VF, "NZV"));
	case ARM_CC_LS:
		return m.Test(ConditionCode::ULE, FlagGroup(FlagM::ZF | FlagM::CF, "ZC"));
	case ARM_CC_LT:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF | FlagM::VF, "NV"));
	case ARM_CC_MI:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF, "N"));
	case ARM_CC_PL:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF, "NZ"));
	case ARM_CC_NE:
		return m.Test(ConditionCode::NE, FlagGroup(FlagM::ZF, "Z"));
	case ARM_CC_VC:
		return m.Test(ConditionCode::NO, FlagGroup(FlagM::VF, "V"));
	case ARM_CC_VS:
		return m.Test(ConditionCode::OV, FlagGroup(FlagM::VF, "V"));
	}
	return HExpr();
}

HExpr Arm64Rewriter::FlagGroup(FlagM bits, const char * name)
{
	return host->EnsureFlagGroup(ARM64_REG_NZCV, (int) bits, name);
}


#if (_DEBUG || DEBUG) && !MONODEVELOP
void Arm64Rewriter::EmitUnitTest()
{
	if (opcode_seen[instr->id])
		return;
	opcode_seen[instr->id] = 1;

	//var r2 = rdr.Clone();
	//r2.Offset -= dasm.Current.Length;
	auto bytes = &instr->bytes[0];
	wchar_t buf[256];
	::OutputDebugString(L"        [Test]\r\n");
	wsprintfW(buf,      L"        public void Arm64Rw_%S()\r\n", instr->mnemonic );
	::OutputDebugString(buf);
	::OutputDebugString(L"        {\r\n");
	wsprintfW(buf,      L"            BuildTest(0x%02x%02x%02x%02x);\t// %S %S\r\n",
		bytes[3], bytes[2], bytes[1], bytes[0],
		instr->mnemonic, instr->op_str);
	::OutputDebugString(buf);
	::OutputDebugString(L"            AssertCode(");
	::OutputDebugString(L"                \"0|L--|00100000(4): 1 instructions\",\r\n");
	::OutputDebugString(L"                \"1|L--|@@@\");\r\n");
	::OutputDebugString(L"        }\r\n");
	::OutputDebugString(L"\r\n");
}

int Arm64Rewriter::opcode_seen[ARM_INS_ENDING];
#endif


const int Arm64Rewriter::type_sizes[] = 
{
	0,						// Void,

	1,						// Bool,

	1,						// Byte,
	1,						// SByte,
	1,						// Char8,

	2,						// Int16,
	2,						// UInt16,
	2,						// Ptr16,
	2,						// Word16,

	4,						// Int32,
	4,						// UInt32,
	4,						// Ptr32,
	4,						// Word32,

	8,						// Int64,
	8,						// UInt64,
	8,						// Ptr64,
	8,						// Word64,
 
	16,						// Word128,

	4,						// Real32,
	8,						// Real64,
};

int32_t Arm64Rewriter::GetCount()
{
	return s_count;
}

int Arm64Rewriter::s_count = 0;
