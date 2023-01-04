/*
* Copyright (C) 1999-2023 John Källén.
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
#include "ArmRewriter.h"
#include "ThumbRewriter.h"

#include "common/compat.h"


ThumbRewriter::ThumbRewriter(
	const uint8_t * rawBytes,
	size_t availableBytes,
	uint64_t address,
	INativeRtlEmitter * emitter,
	INativeTypeFactory * ntf,
	INativeRewriterHost * host)
	: ArmRewriter(CS_MODE_THUMB, rawBytes, availableBytes, address, emitter, ntf, host)
{
	this->itState = 0;
	this->itStateCondition = ARM_CC_AL;
}

void ThumbRewriter::PostRewrite()
{
	itState = (itState << 1);
	if ((itState & 0x0F) == 0)
	{
		itStateCondition = ARM_CC_AL;
	}
}

//void ThumbRewriter::NotImplementedYet()
//{
//	char buf[200];	//$TODO: hello buffer overflow!
//	::snprintf(buf, sizeof(buf), "Rewriting ARM opcode '%s' is not supported yet.", instr->mnemonic);
//	EmitUnitTest();
//	host->Error(
//		instr->address,
//		buf);
//	m.Invalid();
//}

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

void ThumbRewriter::ConditionalSkip(bool force)
{
	if (this->itStateCondition != ARM_CC_AL)
	{
		auto rtlc = static_cast<int>(rtlClass) | static_cast<int>(InstrClass::Conditional);
		rtlClass = static_cast<InstrClass>(rtlc);
		auto cc = itStateCondition;
		if (itState & 0x10)
			cc = Invert(cc);
		m.BranchInMiddleOfInstruction(
			TestCond(cc),
			m.Ptr32(static_cast<uint32_t>(instr->address) + instr->size),
			InstrClass::ConditionalTransfer);
	}
}

//HExpr ThumbRewriter::GetReg(int armRegister)
//{
//	return host->EnsureRegister(0, armRegister);
//}
//
//HExpr ThumbRewriter::EffectiveAddress(const arm_op_mem & mem)
//{
//	auto baseReg = GetReg(mem.base);
//	auto ea = baseReg;
//	if (mem.disp > 0)
//	{
//		ea = m.IAdd(ea, m.Int32(mem.disp));
//	}
//	else if (mem.disp < 0)
//	{
//		ea = m.ISub(ea, m.Int32(-mem.disp));
//	}
//	else if (mem.index != ARM_REG_INVALID)
//	{
//		ea = m.IAdd(ea, GetReg(mem.index));
//	}
//	return ea;
//}
//
//HExpr ThumbRewriter::TestCond(arm_cc cond)
//{
//	switch (cond)
//	{
//	default:
//		Dump("ARM condition code %d not implemented.", cond);
//		return static_cast<HExpr>(0);
//	case ARM_CC_HS:
//		return m.Test(ConditionCode::UGE, FlagGroup(FlagM::CF, "C", BaseType::Byte));
//	case ARM_CC_LO:
//		return m.Test(ConditionCode::ULT, FlagGroup(FlagM::CF, "C", BaseType::Byte));
//	case ARM_CC_EQ:
//		return m.Test(ConditionCode::EQ, FlagGroup(FlagM::ZF, "Z", BaseType::Byte));
//	case ARM_CC_GE:
//		return m.Test(ConditionCode::GE, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV", BaseType::Byte));
//	case ARM_CC_GT:
//		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV", BaseType::Byte));
//	case ARM_CC_HI:
//		return m.Test(ConditionCode::UGT, FlagGroup(FlagM::ZF | FlagM::CF, "ZC", BaseType::Byte));
//	case ARM_CC_LE:
//		return m.Test(ConditionCode::LE, FlagGroup(FlagM::ZF | FlagM::CF | FlagM::VF, "NZV", BaseType::Byte));
//	case ARM_CC_LS:
//		return m.Test(ConditionCode::ULE, FlagGroup(FlagM::ZF | FlagM::CF, "ZC", BaseType::Byte));
//	case ARM_CC_LT:
//		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF | FlagM::VF, "NV", BaseType::Byte));
//	case ARM_CC_MI:
//		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF, "N", BaseType::Byte));
//	case ARM_CC_PL:
//		return m.Test(ConditionCode::GE, FlagGroup(FlagM::NF, "N", BaseType::Byte));
//	case ARM_CC_NE:
//		return m.Test(ConditionCode::NE, FlagGroup(FlagM::ZF, "Z", BaseType::Byte));
//	case ARM_CC_VS:
//		return m.Test(ConditionCode::OV, FlagGroup(FlagM::VF, "V", BaseType::Byte));
//	}
//}
//
//HExpr ThumbRewriter::FlagGroup(FlagM bits, const char * name, BaseType type)
//{
//		return host->EnsureFlagGroup(ARM_REG_CPSR, (int)bits, name, type);
//}
//
//void ThumbRewriter::MaybeUpdateFlags(HExpr opDst)
//{
//	if (instr->detail->arm.update_flags)
//	{
//		m.Assign(NZCV(), m.Cond(opDst));
//	}
//}
//
//HExpr ThumbRewriter::NZCV()
//{
//	return host->EnsureFlagGroup((int)ARM_REG_CPSR, 0xF, "NZCV", BaseType::Byte);
//}
