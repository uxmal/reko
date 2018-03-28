#include "stdafx.h"

#include "reko.h"
#include "functions.h"
#include "ComBase.h"
#include "ThumbRewriter.h"

void ThumbRewriter::RewriteB()
{
	auto addr = m.Ptr32((uint32_t)Dst().imm);
	if (instr->detail->arm.cc == ARM_CC_AL)
	{
		rtlClass = RtlClass::Transfer;
		m.Goto(addr);
	}
	else
	{
		rtlClass = RtlClass::ConditionalTransfer;
		m.Branch(TestCond(instr->detail->arm.cc), addr, RtlClass::ConditionalTransfer);
	}
}

void ThumbRewriter::RewriteBl()
{
	rtlClass = RtlClass::Transfer | RtlClass::Call;
	m.Call(
		m.Ptr32((uint32_t)Dst().imm),
		0);
}

void ThumbRewriter::RewriteBlx()
{
	rtlClass = RtlClass::Transfer | RtlClass::Call;
	m.Call(RewriteOp(Dst()), 0);
}

void ThumbRewriter::RewriteBx()
{
	rtlClass = RtlClass::Transfer;
	m.Goto(RewriteOp(Dst()));
}

void ThumbRewriter::RewriteCbnz(HExpr (*ctor)(INativeRtlEmitter & m, HExpr e))
{
	rtlClass = RtlClass::ConditionalTransfer;
	auto cond = RewriteOp(Dst());
	m.Branch(ctor(m, RewriteOp(Dst())),
		m.Ptr32((uint32_t)Src1().imm),
		RtlClass::ConditionalTransfer);
}

void ThumbRewriter::RewriteIt()
{
	int i;
	itState = 0;
	for (i = 1; instr->mnemonic[i]; ++i)
	{
		itState = (itState << 1) | (instr->mnemonic[i] == 't');
	}
	int sh = 5 - i;
	itState = ((itState << 1) | 1) << sh;

	itStateCondition = instr->detail->arm.cc;
	m.Nop();
	m.FinishCluster(RtlClass::Linear, instr->address, instr->size);
}

void ThumbRewriter::RewriteTrap()
{
	auto trapNo = m.UInt32(instr->bytes[0]);
	auto ppp = host->EnsurePseudoProcedure("__syscall", BaseType::Word32, 1);
	m.AddArg(trapNo);
	m.SideEffect(m.Fn(ppp));
}

void ThumbRewriter::RewriteUdf()
{
	auto trapNo = m.UInt32(instr->bytes[0]);
	auto ppp = host->EnsurePseudoProcedure("__syscall", BaseType::Word32, 1);
	m.AddArg(trapNo);
	m.SideEffect(m.Fn(ppp));
}



