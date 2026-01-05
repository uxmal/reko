#pragma once
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


typedef HExpr (STDAPICALLTYPE INativeRtlEmitter::*UnaryOpEmitter)(HExpr);
typedef HExpr (STDAPICALLTYPE INativeRtlEmitter::*BinOpEmitter)(HExpr, HExpr);

enum class FlagM
{
	NF = 8,
	ZF = 4,
	CF = 2,
	VF = 1
};
inline FlagM operator | (FlagM a, FlagM b) { return (FlagM)((int)a | (int)b); }

class ArmRewriter : public ComBase, public INativeRewriter
{
public:
	ArmRewriter(
		cs_mode mode,
		const uint8_t * rawBytes,
		size_t length,
		uint64_t address,
		INativeRtlEmitter * emitter,
		INativeTypeFactory * ntf,
		INativeRewriterHost * host);

	STDMETHOD(QueryInterface)(REFIID iid, void ** ppvOut) override;
	STDMETHOD_(ULONG, AddRef)() override { return ComBase::AddRef(); }
	STDMETHOD_(ULONG, Release)() override { return ComBase::Release(); }

	STDMETHOD_(int32_t,Next)();
	STDMETHOD_(int32_t, GetCount)();

protected:
	virtual void ConditionalSkip(bool force);
	virtual void PostRewrite() {}
	virtual void RewriteIt() {}

protected:
	HExpr EffectiveAddress(const arm_op_mem & mem);
	HExpr FlagGroup(FlagM bits, const char * name);
	arm_cc Invert(arm_cc);
	bool IsLastOperand(const cs_arm_op & op);
	void NotImplementedYet();
	void MaybeUpdateFlags(HExpr opDst);
	void MaybePostOperand(const cs_arm_op & op);
	HExpr MaybeShiftOperand(HExpr exp, const cs_arm_op & op);
	const char * MemBarrierName(arm_mem_barrier barrier);
	HExpr NZC();
	HExpr NZCV();
	HExpr Operand(const cs_arm_op & op, BaseType dt = BaseType::Word32, bool write = false);
	HExpr Q();
	HExpr Reg(int reg) { return host->EnsureRegister(0, reg); }
	HExpr Reg(arm_reg reg) {  return host->EnsureRegister(0, (int)reg); }

	BaseType SizeFromLoadStore();
	HExpr TestCond(arm_cc cond);
	const char * VectorElementType(arm_vectordata_type elemType);
	BaseType VectorElementDataType(arm_vectordata_type elemType);
	const cs_arm_op & Dst() { return instr->detail->arm.operands[0]; }
	const cs_arm_op & Src1() { return instr->detail->arm.operands[1]; }
	const cs_arm_op & Src2() { return instr->detail->arm.operands[2]; }
	const cs_arm_op & Src3() { return instr->detail->arm.operands[3]; }

	void RewriteAdcSbc(BinOpEmitter fn, bool reverse);
	void RewriteAddw();
	void RewriteAdr();
	void RewriteCbnz(HExpr(*cons)(INativeRtlEmitter & m, HExpr e));
	void RewriteB(bool link);
	void RewriteBfc();
	void RewriteBfi();
	void RewriteBic();
	void RewriteBinOp(BinOpEmitter fn);
	void RewriteLogical(HExpr(*cons)(INativeRtlEmitter & m, HExpr a, HExpr b));
	void RewriteCdp(const char * intrinsic);
	void RewriteClz();
	void RewriteCmp(BinOpEmitter);
	void RewriteCps();
	void RewriteDiv(BinOpEmitter);
	void RewriteDmb();
	void RewriteHint();
	void RewriteLdc(const char * intrinsic);
	void RewriteLdm(int offset, BinOpEmitter);
	void RewriteLdm(HExpr dst, int skip_ops, int offset, BinOpEmitter, bool writeback);
	void RewriteLdr(BaseType dtDst, BaseType dtSrc);
	void RewriteLdrd();
	void RewriteLdrex();
	void RewriteMcr();
	void RewriteMla(bool hiLeft, bool hiRight, BaseType, BinOpEmitter);
	void RewriteMlxd(bool swap, BaseType, BinOpEmitter mul, BinOpEmitter addSub);
	void RewriteMlal(bool hiLeft, bool hiRight, BaseType, BinOpEmitter);
	void RewriteMov();
	void RewriteMovt();
	void RewriteMovw();
	void RewriteMrc();
	void RewriteMrs();
	void RewriteMsr();
	void RewriteMulbb(bool, bool, BaseType, BinOpEmitter);
	void RewriteMull(BaseType, BinOpEmitter);
	void RewriteMulw(bool);
	void RewriteMultiplyAccumulate(BinOpEmitter);
	void RewritePop();
	void RewritePush();
	void RewriteQAddSub(BinOpEmitter);
	void RewriteQDAddSub(BinOpEmitter);
	void RewriteRev();
	void RewriteRevBinOp(BinOpEmitter, bool setflags);
	void RewriteSbfx();
	void RewriteShift(HExpr(*ctor)(INativeRtlEmitter &m, HExpr, HExpr));
	void RewriteSmlal();
	void RewriteSmlaw(bool highPart);
	void RewriteStc(const char * name);
	void RewriteStm(int offset, bool incr);
	void RewriteStmib();
	void RewriteStr(BaseType);
	void RewriteStrd();
	void RewriteStrex();
	void RewriteSubw();
	void RewriteSvc();
	void RewriteSwp(BaseType);
	void RewriteTeq();
	void RewriteTrap();
	void RewriteTst();
	void RewriteUmlal();
	void RewriteUnaryOp(UnaryOpEmitter);
	void RewriteUbfx();
	void RewriteUdf();
	void RewriteUmaal();
	void RewriteVecBinOp(BinOpEmitter);
	void RewriteVectorBinOp(const char * fnNameFormat);
	void RewriteVectorBinOp(const char * fnNameFormat, arm_vectordata_type elemType);
	void RewriteVectorUnaryOp(const char * fnNameFormat);
	void RewriteVectorUnaryOp(const char * fnNameFormat, arm_vectordata_type elemType);
	void RewriteVcmp();
	void RewriteVcvt();
	void RewriteVdup();
	void RewriteVext();
	void RewriteVldmia();
	void RewriteVldr();
	void RewriteVmov();
	void RewriteVmrs();
	void RewriteVmul();
	void RewriteVmvn();
	void RewriteVpop();
	void RewriteVpush();
	void RewriteVstmia();
	void RewriteVsqrt();
	void RewriteVstr();
	void RewriteXtab(BaseType);
	void RewriteXtb(BaseType, BaseType);
	void RewriteYield();

protected:
	csh hcapstone;
	INativeRtlEmitter & m;
	INativeTypeFactory & ntf;
	INativeRewriterHost * host;
	cs_insn * instr;
	const uint8_t * rawBytes;
	size_t available;			// Available bytes left past rawBytes
	uint64_t address;
	InstrClass rtlClass;

	static const BaseType register_types[];
	static const int type_sizes[];
	static int s_count;			//$DEBUG: tracking number of "live" objects 

#if (_DEBUG || DEBUG) && !MONODEVELOP
	void EmitUnitTest();
	static int opcode_seen[];
#else
	void EmitUnitTest(){
		
	}
#endif
};
inline InstrClass operator|(InstrClass a, InstrClass b) {
	return static_cast<InstrClass>(static_cast<int>(a) | static_cast<int>(b));
}
