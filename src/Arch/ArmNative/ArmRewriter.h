#pragma once

typedef IExpression * (IRtlEmitter::*UnaryOpEmitter)(IExpression *);
typedef IExpression * (IRtlEmitter::*BinOpEmitter)(IExpression *, IExpression *);

enum class FlagM
{
	CF, VF, ZF, NF,
};
inline FlagM operator | (FlagM a, FlagM b) { return (FlagM)((int)a | (int)b); }

class ArmRewriter : public IRewriter
{
public:
	ArmRewriter(
		void * rawBytes,
		int length,
		IRtlEmitter * emitter,
		IFrame * frame,
		IRewriterHost * host);
	virtual void Next() override;

private:
	void AddConditional(void(*mkInstr)());
	void ConditionalSkip();
	void ConditionalAssign(IExpression * dst, IExpression * src);
	IExpression * FlagGroup(FlagM bits, const char * name, PrimitiveType type);
	ArmCodeCondition Invert(ArmCodeCondition);
	//IExpression * Operand(const ArmInstructionOperand & op);
	void NotImplementedYet();
	void MaybeUpdateFlags(IExpression * opDst);
	void MaybePostOperand(const ArmInstructionOperand & op);
	IExpression * MaybeShiftOperand(IExpression * exp, ArmInstructionOperand op);
	IExpression * Operand(const ArmInstructionOperand & op);
	IExpression * Reg(int reg) { return frame.EnsureRegister(reg); }
	IExpression * Reg(ArmRegister reg) { return frame.EnsureRegister((int)reg); }

	PrimitiveType SizeFromLoadStore();
	IExpression * TestCond(ArmCodeCondition cond);
	const char * ArmRewriter::VectorElementType();

	const ArmInstructionOperand & Dst() { return instr.ArchitectureDetail.Operands[0]; }
	const ArmInstructionOperand & Src1() { return instr.ArchitectureDetail.Operands[1]; }
	const ArmInstructionOperand & Src2() { return instr.ArchitectureDetail.Operands[2]; }
	const ArmInstructionOperand & Src3() { return instr.ArchitectureDetail.Operands[3]; }
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
	void RewriteLdm(IExpression  * dst, const ArmInstructionOperand * range, int length, int offset, bool writeback);
	void RewriteLdr(PrimitiveType);
	void RewriteLdrd();
	void RewriteMcr();
	void RewriteMov();
	void RewriteMovt();
	void RewriteMrc();
	void RewriteMrs();
	void RewriteMsr();
	void RewriteMulbb(bool, bool, PrimitiveType, BinOpEmitter);
	void RewriteMull(PrimitiveType, BinOpEmitter);
	void RewriteMultiplyAccumulate(BinOpEmitter);
	void RewritePop();
	void RewritePush();
	void RewriteRev();
	void RewriteRevBinOp(BinOpEmitter, bool setflags);
	void RewriteSbfx();
	void RewriteStm();
	void RewriteStmib();
	void RewriteStr(PrimitiveType);
	void RewriteSvc();
	void RewriteUnaryOp(UnaryOpEmitter);
	void RewriteUbfx();
	void RewriteVldmia();
	void RewriteVmov();
	void RewriteVstmia();
	void RewriteXtab(PrimitiveType);
	void RewriteXtb(PrimitiveType);

private:
	IRtlEmitter & m;
	IRewriterHost & host;
	IFrame & frame;
	ArmInstruction instr;
};
