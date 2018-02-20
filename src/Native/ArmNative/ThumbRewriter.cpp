/*
* Copyright (C) 1999-2018 John Källén.
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
#include "ThumbRewriter.h"

#include "common/compat.h"


ThumbRewriter::ThumbRewriter(
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
	auto ec = cs_open(CS_ARCH_ARM, CS_MODE_THUMB , &hcapstone);
	ec = cs_option(hcapstone, CS_OPT_DETAIL, CS_OPT_ON);
	this->instr = cs_malloc(hcapstone);

	this->itState = 0;
	this->itStateCondition = ARM_CC_AL;

	//++s_count;
}

STDMETHODIMP ThumbRewriter::QueryInterface(REFIID riid, void ** ppvOut)
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


STDMETHODIMP ThumbRewriter::Next()
{
start:
	Dump("Next: %08x", this);
	if (available == 0)
		return S_FALSE;			// No more work to do.
	auto addrInstr = address;
	bool f = cs_disasm_iter(hcapstone, &rawBytes, &available, &address, instr);
	if (!f)
	{
		// Failed to disassemble the instruction because it was invalid.
		m.Invalid();
		m.FinishCluster(RtlClass::Invalid, addrInstr, 4);
		return S_OK;
	}

	// Most instructions are linear.
	rtlClass = RtlClass::Linear;
	switch (instr->id)
	{
	case ARM_INS_ADD: RewriteBinop([](auto & m, HExpr a, HExpr b) { return m.IAdd(a, b); }); break;
	case ARM_INS_ADDW: RewriteAddw(); break;
	case ARM_INS_ADR: RewriteAdr(); break;
	case ARM_INS_AND: RewriteAnd(); break;
	case ARM_INS_ASR: RewriteShift([](auto & m, auto a, auto b) { return m.Sar(a, b); }); break;
	case ARM_INS_B: RewriteB(); break;
	case ARM_INS_BIC: RewriteBic(); break;
	case ARM_INS_BL: RewriteBl(); break;
	case ARM_INS_BLX: RewriteBlx(); break;
	case ARM_INS_BX: RewriteBx(); break;
	case ARM_INS_CBZ: RewriteCbnz([](auto & m, auto a) { return m.Eq0(a); }); break;
	case ARM_INS_CBNZ: RewriteCbnz([](auto & m, auto a) { return m.Ne0(a); }); break;
	case ARM_INS_CMP: RewriteCmp(); break;
	case ARM_INS_DMB: RewriteDmb(); break;
	case ARM_INS_EOR: RewriteEor(); break;
	case ARM_INS_IT: RewriteIt(); goto start;  // Don't emit anything yet.
	case ARM_INS_LDR: RewriteLdr(BaseType::Word32, BaseType::Word32); break;
	case ARM_INS_LDRB: RewriteLdr(BaseType::UInt32, BaseType::Byte); break;
	case ARM_INS_LDRSB: RewriteLdr(BaseType::Int32, BaseType::SByte); break;
	case ARM_INS_LDREX: RewriteLdrex(); break;
	case ARM_INS_LDRH: RewriteLdr(BaseType::UInt32, BaseType::Word16); break;
	case ARM_INS_LSL: RewriteShift([](auto & m, auto a, auto b) { return m.Shl(a, b); }); break;
	case ARM_INS_LSR: RewriteShift([](auto & m, auto a, auto b) { return m.Shr(a, b); }); break;
	case ARM_INS_MOV: RewriteMov(); break;
	case ARM_INS_MOVT: RewriteMovt(); break;
	case ARM_INS_MOVW: RewriteMovw(); break;
	case ARM_INS_MRC: RewriteMrc(); break;
	case ARM_INS_MVN: RewriteMvn(); break;
	case ARM_INS_POP: RewritePop(); break;
	case ARM_INS_PUSH: RewritePush(); break;
	case ARM_INS_RSB: RewriteRsb(); break;
	case ARM_INS_STM: RewriteStm(0, true); break;
	case ARM_INS_STR: RewriteStr(BaseType::Word32); break;
	case ARM_INS_STRH: RewriteStr(BaseType::Word16); break;
	case ARM_INS_STRB: RewriteStr(BaseType::Byte); break;
	case ARM_INS_STREX: RewriteStrex(); break;
	case ARM_INS_SUB: RewriteBinop([](auto & m, auto a, auto b) { return m.ISub(a, b); }); break;
	case ARM_INS_SUBW: RewriteSubw(); break;
	case ARM_INS_TRAP: RewriteTrap(); break;
	case ARM_INS_TST: RewriteTst(); break;
	case ARM_INS_UDF: RewriteUdf(); break;
	case ARM_INS_UXTH: RewriteUxth(); break;
	}
	itState = (itState << 1) & 0x0F;
	if (itState == 0)
	{
		itStateCondition = ARM_CC_AL;
	}
	m.FinishCluster(rtlClass, instr->address, instr->size);
	return S_OK;
}

void ThumbRewriter::NotImplementedYet()
{
	char buf[200];	//$TODO: hello buffer overflow!
	::snprintf(buf, sizeof(buf), "Rewriting ARM opcode '%s' is not supported yet.", instr->mnemonic);
	EmitUnitTest();
	host->Error(
		instr->address,
		buf);
	m.Invalid();
}

#if (_DEBUG || DEBUG) && !MONODEVELOP
void ThumbRewriter::EmitUnitTest()
{
	if (opcode_seen[instr->id])
		return;
	opcode_seen[instr->id] = 1;

	//var r2 = rdr.Clone();
	//r2.Offset -= dasm.Current.Length;
	auto bytes = &instr->bytes[0];
	wchar_t buf[256];
	::OutputDebugString(L"        [Test]\r\n");
	wsprintfW(buf, L"        public void ThumbRw_%S()\r\n", instr->mnemonic);
	::OutputDebugString(buf);
	::OutputDebugString(L"        {\r\n");
	wsprintfW(buf, L"            BuildTest(0x%02x%02x%02x%02x);\t// %S %S\r\n",
		bytes[3], bytes[2], bytes[1], bytes[0],
		instr->mnemonic, instr->op_str);
	::OutputDebugString(buf);
	::OutputDebugString(L"            AssertCode(");
	::OutputDebugString(L"                \"0|L--|00100000(4): 1 instructions\",\r\n");
	::OutputDebugString(L"                \"1|L--|@@@\");\r\n");
	::OutputDebugString(L"        }\r\n");
	::OutputDebugString(L"\r\n");
}

int ThumbRewriter::opcode_seen[ARM_INS_ENDING];
#endif

HExpr ThumbRewriter::GetReg(int armRegister)
{
	return host->EnsureRegister(0, armRegister);
}

HExpr ThumbRewriter::RewriteOp(const cs_arm_op & op, BaseType accessSize)
{
	switch (op.type)
	{
	case ARM_OP_REG:
		return GetReg(op.reg);
	case ARM_OP_IMM:
		return m.Word32(op.imm);
	case ARM_OP_MEM:
		auto & mem = op.mem;
		auto ea = EffectiveAddress(mem);
		return m.Mem(accessSize, ea);
	}
	DebugBreak();
	return static_cast<HExpr>(0);
}

HExpr ThumbRewriter::EffectiveAddress(const arm_op_mem & mem)
{
	auto baseReg = GetReg(mem.base);
	auto ea = baseReg;
	if (mem.disp > 0)
	{
		ea = m.IAdd(ea, m.Int32(mem.disp));
	}
	else if (mem.disp < 0)
	{
		ea = m.ISub(ea, m.Int32(-mem.disp));
	}
	else if (mem.index != ARM_REG_INVALID)
	{
		ea = m.IAdd(ea, GetReg(mem.index));
	}
	return ea;
}

HExpr ThumbRewriter::TestCond(arm_cc cond)
{
	switch (cond)
	{
	default:
		Dump("ARM condition code %d not implemented.", cond);
		return static_cast<HExpr>(0);
	case ARM_CC_HS:
		return m.Test(ConditionCode::UGE, FlagGroup(FlagM::CF, "C", BaseType::Byte));
	case ARM_CC_LO:
		return m.Test(ConditionCode::ULT, FlagGroup(FlagM::CF, "C", BaseType::Byte));
	case ARM_CC_EQ:
		return m.Test(ConditionCode::EQ, FlagGroup(FlagM::ZF, "Z", BaseType::Byte));
	case ARM_CC_GE:
		return m.Test(ConditionCode::GE, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV", BaseType::Byte));
	case ARM_CC_GT:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV", BaseType::Byte));
	case ARM_CC_HI:
		return m.Test(ConditionCode::UGT, FlagGroup(FlagM::ZF | FlagM::CF, "ZC", BaseType::Byte));
	case ARM_CC_LE:
		return m.Test(ConditionCode::LE, FlagGroup(FlagM::ZF | FlagM::CF | FlagM::VF, "NZV", BaseType::Byte));
	case ARM_CC_LS:
		return m.Test(ConditionCode::ULE, FlagGroup(FlagM::ZF | FlagM::CF, "ZC", BaseType::Byte));
	case ARM_CC_LT:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF | FlagM::VF, "NV", BaseType::Byte));
	case ARM_CC_MI:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF, "N", BaseType::Byte));
	case ARM_CC_PL:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF, "NZ", BaseType::Byte));
	case ARM_CC_NE:
		return m.Test(ConditionCode::NE, FlagGroup(FlagM::ZF, "Z", BaseType::Byte));
	case ARM_CC_VS:
		return m.Test(ConditionCode::OV, FlagGroup(FlagM::VF, "V", BaseType::Byte));
	}
}

HExpr ThumbRewriter::FlagGroup(FlagM bits, const char * name, BaseType type)
{
		return host->EnsureFlagGroup(ARM_REG_CPSR, (int)bits, name, type);
}


HExpr ThumbRewriter::NZCV()
{
	return host->EnsureFlagGroup((int)ARM_REG_CPSR, 0xF, "NZCV", BaseType::Byte);
}

// If a conditional ARM instruction is encountered, generate an IL
// instruction to skip the remainder of the instruction cluster.
void ThumbRewriter::ConditionalSkip(arm_cc cc, bool force)
{
	if (!force)
	{
		if (cc == ARM_CC_AL)
			return; // never skip!
		if (instr->id == ARM_INS_B ||
			instr->id == ARM_INS_BL ||
			instr->id == ARM_INS_BLX ||
			instr->id == ARM_INS_BX)
		{
			// These instructions handle the branching themselves.
			return;
		}
	}
	m.BranchInMiddleOfInstruction(
		TestCond(Invert(cc)),
		m.Ptr32(static_cast<uint32_t>(instr->address) + instr->size),
		RtlClass::ConditionalTransfer);
}

arm_cc ThumbRewriter::Invert(arm_cc cc)
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

int32_t ThumbRewriter::GetCount()
{
	return 0;
}

//int ThumbRewriter::s_count = 0;
