#include "stdafx.h"

#include "reko.h"
#include "functions.h"
#include "ComBase.h"

#include "ArmRewriter.h"
#include "ThumbRewriter.h"





/*
void ThumbRewriter::RewriteMovt()
{
	auto dst = GetReg(Dst().reg);
	auto src = m.Word16((uint16_t)Src1().imm);
	m.Assign(dst, m.Dpb(dst, src, 16));
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
		m.Assign(FlagGroup(FlagM::NF | FlagM::ZF | FlagM::CF, "NZC"),
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

void ThumbRewriter::RewriteStrd()
{
	auto ops = instr->detail->arm.operands;
	auto regLo = (int)ops[0].reg;
	auto regHi = (int)ops[1].reg;
	auto opSrc = host->EnsureSequence(regHi, regLo, BaseType::Word64);
	const auto & mem = ops[2].mem;
	auto ea = EffectiveAddress(mem);
	auto opDst = m.Mem(BaseType::Word64, ea);
	m.Assign(opDst, opSrc);
	if (instr->detail->arm.op_count == 4)
	{
		auto baseReg = GetReg(mem.base);
		m.Assign(baseReg, m.IAdd(baseReg, RewriteOp(Src3())));
	}
}





void ThumbRewriter::RewriteTst()
{
	ConditionalSkip(itStateCondition, false);
	auto dst = RewriteOp(Dst());
	auto src = RewriteOp(Src1(), BaseType::UInt32);
	m.Assign(
		FlagGroup(FlagM::NF | FlagM::ZF | FlagM::CF, "NZC"),
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


*/