#include "stdafx.h"

#include "reko.h"
#include "functions.h"
#include "ComBase.h"

#include "ThumbRewriter.h"

void ThumbRewriter::RewriteAdr()
{
	auto dst = RewriteOp(Dst());
	auto src = m.Ptr32(static_cast<uint32_t>(instr->address) + 4 + Src1().imm);
	m.Assign(dst, src);
}

void ThumbRewriter::RewriteAnd()
{
	auto dst = RewriteOp(Dst());
	auto src = RewriteOp(Src1());
	m.Assign(dst, m.And(dst, src));
	if (this->instr->detail->arm.update_flags)
		m.Assign(FlagGroup(FlagM::NF | FlagM::ZF | FlagM::CF, "NZC", BaseType::Byte),
			m.Cond(dst));
}


void ThumbRewriter::RewriteBic()
{
	auto dst = RewriteOp(Dst());
	auto src1 = RewriteOp(Src1());
	auto src2 = RewriteOp(Src2());
	m.Assign(dst, m.And(src1, m.Comp(src2)));
}

void ThumbRewriter::RewriteBinop(HExpr (*op)(INativeRtlEmitter & m, HExpr a, HExpr b))
{
	auto dst = RewriteOp(Dst());
	auto src = RewriteOp(Src1());
	m.Assign(dst, op(m, dst, src));
}

void ThumbRewriter::RewriteCmp()
{
	auto dst = RewriteOp(Dst());
	auto src = RewriteOp(Src1());
	auto flags = FlagGroup(static_cast<FlagM>(0x0F), "NZCV", BaseType::Byte);
	m.Assign(flags, m.Cond(
		m.ISub(dst, src)));
}

void ThumbRewriter::RewriteEor()
{
	auto dst = RewriteOp(Dst());
	auto src = RewriteOp(Src1());
	m.Assign(dst, m.Xor(dst, src));
	if (instr->detail->arm.update_flags)
		m.Assign(FlagGroup(FlagM::NF | FlagM::ZF | FlagM::CF, "NZC", BaseType::Byte),
			m.Cond(dst));
}


void ThumbRewriter::RewriteLdr(BaseType dtDst, BaseType dtSrc)
{
	this->ConditionalSkip(this->itStateCondition, false);
	const auto & mem = Src1().mem;
	bool isJump = (Dst().reg == ARM_REG_PC);

	HExpr dst = RewriteOp(Dst());
	HExpr src;
	if (instr->detail->arm.op_count == 2 && instr->detail->arm.writeback)
	{
		// Pre-index operand.
		HExpr baseReg = GetReg(mem.base);
		HExpr ea = EffectiveAddress(mem);
		m.Assign(baseReg, ea);
		src = m.Mem(dtSrc, ea);
	}
	else
	{
		src = RewriteOp(Src1(), dtSrc);
	}
	if (dtDst != dtSrc)
		src = m.Cast(dtDst, src);
	if (instr->detail->arm.op_count == 3)
	{
		// Post-index operand.
		auto tmp = host->CreateTemporary(dtDst);
		auto baseReg = GetReg(Src1().mem.base);
		m.Assign(tmp, src);
		m.Assign(baseReg, m.IAdd(baseReg, RewriteOp(Src2())));
		src = tmp;
	}
	if (isJump)
	{
		rtlClass = RtlClass::Transfer;
		m.Goto(src);
	}
	else
	{
		m.Assign(dst, src);
	}
}


void ThumbRewriter::RewriteLdm(int initialOffset, BinOpEmitter op)
{
	auto dst = this->RewriteOp(Dst());
	RewriteLdm(dst, 1, initialOffset, op, instr->detail->arm.writeback);
}

void ThumbRewriter::RewriteLdm(HExpr dst, int skip, int offset, BinOpEmitter op, bool writeback)
{
	bool pcRestored = false;
	auto begin = &instr->detail->arm.operands[skip];
	auto end = begin + (instr->detail->arm.op_count - skip);
	for (auto r = begin; r != end; ++r)
	{
		HExpr ea = offset != 0
			? (m.*op)(dst, m.Int32(offset))
			: dst;
		if (r->reg == ARM_REG_PC)
		{
			pcRestored = true;
		}
		else
		{
			auto dstReg = GetReg(r->reg);
			m.Assign(dstReg, m.Mem32(ea));
		}
		offset += 4;
	}
	if (writeback)
	{
		m.Assign(dst, m.IAdd(dst, m.Int32(offset)));
	}
	if (pcRestored)
	{
		rtlClass = RtlClass::Transfer;
		m.Return(0, 0);
	}
}

void ThumbRewriter::RewriteLdrex()
{
	auto ppp = host->EnsurePseudoProcedure("__ldrex", BaseType::Void, 0);
	m.SideEffect(m.Fn(ppp));
}

void ThumbRewriter::RewriteShift(HExpr (*ctor)(INativeRtlEmitter & m, HExpr, HExpr))
{
	ConditionalSkip(itStateCondition, false);
	auto dst = RewriteOp(Dst());
	auto src1 = RewriteOp(Src1());
	auto src2 = RewriteOp(Src2());
	m.Assign(dst, ctor(m, src1, src2));
	if (instr->detail->arm.update_flags)
	{
		m.Assign(this->NZCV(), m.Cond(dst));
	}
}

void ThumbRewriter::RewriteMov()
{
	ConditionalSkip(itStateCondition, false);
	auto dst = GetReg(Dst().reg);
	auto src = RewriteOp(Src1());
	m.Assign(dst, src);
}

void ThumbRewriter::RewriteMovt()
{
	auto dst = GetReg(Dst().reg);
	auto src = m.Word16((uint16_t)Src1().imm);
	m.Assign(dst, m.Dpb(dst, src, 16));
}

void ThumbRewriter::RewriteMovw()
{
	auto dst = GetReg(Dst().reg);
	auto src = m.Word32((uint16_t)Src1().imm);
	m.Assign(dst, src);
}

void ThumbRewriter::RewriteMrc()
{
	auto ppp = host->EnsurePseudoProcedure("__mrc", BaseType::Void, 0);
	m.SideEffect(m.Fn(ppp));
}

void ThumbRewriter::RewriteMvn()
{
	auto dst = RewriteOp(Dst());
	m.Assign(dst, m.Comp(RewriteOp(Src1(), BaseType::UInt32)));
}

void ThumbRewriter::RewriteOrr()
{
	auto dst = RewriteOp(Dst());
	auto src = RewriteOp(Src1());
	m.Assign(dst, m.Or(dst, src));
	if (this->instr->detail->arm.update_flags)
		m.Assign(FlagGroup(FlagM::NF | FlagM::ZF | FlagM::CF, "NZC", BaseType::Byte),
			m.Cond(dst));
}

void ThumbRewriter::RewritePop()
{
	auto sp = GetReg(ARM_REG_SP);
	RewriteLdm(sp, 0, 0, &INativeRtlEmitter::IAdd, true);
}

void ThumbRewriter::RewritePush()
{
	ConditionalSkip(itStateCondition, false);

	auto dst = this->GetReg(ARM_REG_SP);
	m.Assign(dst, m.ISub(dst, m.Int32(instr->detail->arm.op_count * 4)));

	int offset = 0;
	auto begin = &instr->detail->arm.operands[0];
	auto end = begin + instr->detail->arm.op_count;
	for (auto op = begin; op != end; ++op)
	{
		auto ea = offset != 0
			? m.IAdd(dst, m.Int32(offset))
			: dst;
		auto reg = GetReg(op->reg);
		m.Assign(m.Mem32(ea), reg);
		offset += 4;
	}
}

void ThumbRewriter::RewriteRsb()
{
	auto dst = RewriteOp(Dst());
	auto src1 = RewriteOp(Src2());    // _R_everse subtract/
	auto src2 = RewriteOp(Src1());
	m.Assign(dst, m.ISub(src1, src2));
}

void ThumbRewriter::RewriteStm(int offset, bool inc)
{
	auto dst = this->RewriteOp(Dst());
	auto begin = &instr->detail->arm.operands[1];	// Skip the dst register
	auto end = begin + instr->detail->arm.op_count - 1;
	auto increment = inc ? 4 : -4;
	for (auto r = begin; r != end; ++r)
	{
		auto ea = offset > 0
			? m.IAdd(dst, m.Int32(offset))
			: offset < 0
			? m.ISub(dst, m.Int32(abs(offset)))
			: dst;
		auto srcReg = GetReg(r->reg);
		m.Assign(m.Mem32(ea), srcReg);
		offset += increment;
	}
	if (instr->detail->arm.writeback)
	{
		if (offset > 0)
		{
			m.Assign(dst, m.IAdd(dst, m.Int32(offset)));
		}
		else if (offset < 0)
		{
			m.Assign(dst, m.ISub(dst, m.Int32(abs(offset))));
		}
	}
}

void ThumbRewriter::RewriteStr(BaseType dtDst)
{
	ConditionalSkip(itStateCondition, false);
	auto src = RewriteOp(Dst());
	if (dtDst != BaseType::Word32)
	{
		src = m.Cast(dtDst, src);
	}
	auto dst = RewriteOp(Src1(), dtDst);
	const auto & mem = Src1().mem;
	if (instr->detail->arm.op_count == 2 && instr->detail->arm.writeback)
	{
		// Pre-index operand.
		HExpr baseReg = GetReg(mem.base);
		HExpr ea = EffectiveAddress(mem);
		m.Assign(baseReg, ea);
		dst = m.Mem(dtDst, baseReg);
	}
	else
	{
		dst = RewriteOp(Src1(), dtDst);
	}
	m.Assign(dst, src);
	if (instr->detail->arm.op_count == 3)
	{
		auto baseReg = GetReg(mem.base);
		m.Assign(baseReg, m.IAdd(baseReg, RewriteOp(Src2())));
	}
}

void ThumbRewriter::RewriteStrex()
{
	auto ppp = host->EnsurePseudoProcedure("__strex", BaseType::Void, 0);
	m.SideEffect(m.Fn(ppp));
}


void ThumbRewriter::RewriteAddw()
{
	ConditionalSkip(itStateCondition, false);
	auto dst = RewriteOp(Dst());
	auto src1 = RewriteOp(Src1());
	auto src2 = RewriteOp(Src2());
	m.Assign(dst, m.IAdd(src1, src2));
}

void ThumbRewriter::RewriteSubw()
{
	ConditionalSkip(itStateCondition, false);
	auto dst = RewriteOp(Dst());
	auto src1 = RewriteOp(Src1());
	auto src2 = RewriteOp(Src2());
	m.Assign(dst, m.ISub(src1, src2));
}

void ThumbRewriter::RewriteTst()
{
	ConditionalSkip(itStateCondition, false);
	auto dst = RewriteOp(Dst());
	auto src = RewriteOp(Src1(), BaseType::UInt32);
	m.Assign(
		FlagGroup(FlagM::NF | FlagM::ZF | FlagM::CF, "NZC", BaseType::Byte),
		m.Cond(m.And(dst, src)));
}

void ThumbRewriter::RewriteUxth()
{
	ConditionalSkip(itStateCondition, false);
	auto dst = RewriteOp(Dst());
	auto src = RewriteOp(Src1());
	m.Assign(
		dst,
		m.Cast(
			BaseType::UInt32,
			m.Cast(
				BaseType::UInt16,
				src)));
}


