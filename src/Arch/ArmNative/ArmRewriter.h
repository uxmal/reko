#pragma once

typedef HExpr (STDAPICALLTYPE IRtlNativeEmitter::*UnaryOpEmitter)(HExpr);
typedef HExpr (STDAPICALLTYPE IRtlNativeEmitter::*BinOpEmitter)(HExpr, HExpr);

enum class FlagM
{
	CF, VF, ZF, NF,
};
inline FlagM operator | (FlagM a, FlagM b) { return (FlagM)((int)a | (int)b); }

class ArmRewriter : public INativeRewriter
{
public:
	ArmRewriter(
		const uint8_t * rawBytes,
		size_t length,
		uint64_t address,
		IRtlNativeEmitter * emitter,
		INativeRewriterHost * host);

	STDMETHOD(QueryInterface)(REFIID iid, void ** ppvOut);
	STDMETHOD_(ULONG, AddRef)();
	STDMETHOD_(ULONG, Release)();

	STDMETHOD(Next)();

private:
	void AddConditional(void(*mkInstr)());
	void ConditionalSkip();
	void ConditionalAssign(HExpr dst, HExpr src);
	HExpr FlagGroup(FlagM bits, const char * name, BaseType type);
	arm_cc Invert(arm_cc);
	bool IsLastOperand(const cs_arm_op & op);
	//HExpr Operand(const ArmInstructionOperand & op);
	void NotImplementedYet();
	void MaybeUpdateFlags(HExpr opDst);
	void MaybePostOperand(const cs_arm_op & op);
	HExpr MaybeShiftOperand(HExpr exp, const cs_arm_op & op);
	HExpr NZCV();
	HExpr Operand(const cs_arm_op & op);
	HExpr Reg(int reg) { 
		return host->EnsureRegister(reg);
	}
	HExpr Reg(arm_reg reg) { 
		return host->EnsureRegister((int)reg);
	}

	BaseType SizeFromLoadStore();
	HExpr TestCond(arm_cc cond);
	const char * ArmRewriter::VectorElementType();

	const cs_arm_op & Dst() { return instr->detail->arm.operands[0]; }
	const cs_arm_op & Src1() { return instr->detail->arm.operands[1]; }
	const cs_arm_op & Src2() { return instr->detail->arm.operands[2]; }
	const cs_arm_op & Src3() { return instr->detail->arm.operands[3]; }

	void RewriteStrd();
	void RewriteTeq();
	void RewriteTst();
	void RewriteUmlal();
	void RewriteAdcSbc(BinOpEmitter fn);
	void RewriteB(bool link);
	void RewriteBfc();
	void RewriteBfi();
	void RewriteBic();
	void RewriteBinOp(BinOpEmitter fn, bool updateFlags);
	void RewriteClz();
	void RewriteCmn();
	void RewriteCmp();
	void RewriteCps();
	void RewriteDmb();
	void RewriteLdm(int);
	void RewriteLdm(HExpr dst, const cs_arm_op * range, int length, int offset, bool writeback);
	void RewriteLdr(BaseType);
	void RewriteLdrd();
	void RewriteMcr();
	void RewriteMov();
	void RewriteMovt();
	void RewriteMrc();
	void RewriteMrs();
	void RewriteMsr();
	void RewriteMulbb(bool, bool, BaseType, BinOpEmitter);
	void RewriteMull(BaseType, BinOpEmitter);
	void RewriteMultiplyAccumulate(BinOpEmitter);
	void RewritePop();
	void RewritePush();
	void RewriteRev();
	void RewriteRevBinOp(BinOpEmitter, bool setflags);
	void RewriteSbfx();
	void RewriteStm();
	void RewriteStmib();
	void RewriteStr(BaseType);
	void RewriteSvc();
	void RewriteUnaryOp(UnaryOpEmitter);
	void RewriteUbfx();
	void RewriteVldmia();
	void RewriteVmov();
	void RewriteVstmia();
	void RewriteXtab(BaseType);
	void RewriteXtb(BaseType);

private:
	ULONG cRef;	// COM ref count.

	csh hcapstone;
	IRtlNativeEmitter & m;
	INativeRewriterHost * host;
	cs_insn * instr;
	const uint8_t * rawBytes;
	size_t available;			// Available bytes left past rawBytes
	uint64_t address;
	RtlClass rtlClass;
};
