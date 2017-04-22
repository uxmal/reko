#pragma once

typedef IExpression * (IRewriter::*BinOpEmitter)(IExpression *, IExpression *);

class ArmRewriter : public IRewriter
{
public:
	ArmRewriter(
		void * rawBytes,
		int length,
		IRtlEmitter * emitter,
		IFrame * frame,
		IRewriterHost * host)
		: m(*emitter), 
		  host(*host),
		  frame(*frame)
	{
	}

	virtual void Next() override;

private:
	void RewriteStrd();
	void RewriteTeq();
	void RewriteTst();
	void RewriteUmlal();
	void RewriteUnaryOp();
	void RewriteAdcSbc(BinOpEmitter * fn);
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
	void RewriteUbfx();
	void RewriteVldmia();
	void RewriteVmov();
	void RewriteVstmia();
	void RewriteXtab(PrimitiveType);
	void RewriteXtb(PrimitiveType);

	void ConditionalSkip();
	IExpression * Operand(const ArmInstructionOperand & op);
	void NotImplementedYet();
	void MaybeUpdateFlags(IExpression * opDst);

	const ArmInstructionOperand & Dst();
	const ArmInstructionOperand & Src1();
	const ArmInstructionOperand & Src2();
	const ArmInstructionOperand & Src3();

private:
	IRtlEmitter & m;
	IRewriterHost & host;
	IFrame & frame;
	ArmInstruction instr;
};
