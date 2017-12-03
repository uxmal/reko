#pragma once

//$REFACTOR: a lot of this code is duplicated across Arm32 and Thumb. Extract a
// base class (ArmCommon?) and put this stuff there.
typedef HExpr(STDAPICALLTYPE INativeRtlEmitter::*UnaryOpEmitter)(HExpr);
typedef HExpr(STDAPICALLTYPE INativeRtlEmitter::*BinOpEmitter)(HExpr, HExpr);

enum class FlagM
{
	NF = 8,
	ZF = 4,
	CF = 2,
	VF = 1
};
inline FlagM operator | (FlagM a, FlagM b) { return (FlagM)((int)a | (int)b); }

class ThumbRewriter : public ComBase, public INativeRewriter
{
public:
	ThumbRewriter(
		const uint8_t * rawBytes,
		size_t length,
		uint64_t address,
		INativeRtlEmitter * emitter,
		INativeTypeFactory * ntf,
		INativeRewriterHost * host);

	STDMETHOD(QueryInterface)(REFIID iid, void ** ppvOut) override;
	STDMETHOD_(ULONG, AddRef)() override { return ComBase::AddRef(); }
	STDMETHOD_(ULONG, Release)() override { return ComBase::Release(); }

	STDMETHOD(Next)();
	int32_t STDMETHODCALLTYPE GetCount();
private:
	HExpr RewriteOp(const cs_arm_op & op, BaseType  dt = BaseType::Word32);
	HExpr GetReg(int armRegister);
	HExpr EffectiveAddress(const arm_op_mem & mem);
	HExpr TestCond(arm_cc cond);
	HExpr FlagGroup(FlagM bits, const char * name, BaseType type);
	const char * MemBarrierName(arm_mem_barrier barrier);
	void ConditionalSkip(arm_cc cc, bool force);
	arm_cc Invert(arm_cc cc);
	HExpr NZCV();
	void NotImplementedYet();

	const cs_arm_op & Dst() { return instr->detail->arm.operands[0]; }
	const cs_arm_op & Src1() { return instr->detail->arm.operands[1]; }
	const cs_arm_op & Src2() { return instr->detail->arm.operands[2]; }
	const cs_arm_op & Src3() { return instr->detail->arm.operands[3]; }

	void RewriteAdr();
	void RewriteAnd();
	void RewriteAddw();
	void RewriteB();
	void RewriteBic();
	void RewriteBl();
	void RewriteBlx();
	void RewriteBx();
	void RewriteBinop(HExpr(*ctor)(INativeRtlEmitter &m, HExpr, HExpr));
	void RewriteCbnz(HExpr(*cons)(INativeRtlEmitter & m, HExpr e));
	void RewriteCmp();
	void RewriteDmb();
	void RewriteEor();
	void RewriteIt();
	void RewriteLdm(int initialOffset, BinOpEmitter op);
	void RewriteLdm(HExpr dst, int skip, int offset, BinOpEmitter op, bool writeback);
	void RewriteLdr(BaseType dtDst, BaseType dtSrc);
	void RewriteLdrex();
	void RewriteShift(HExpr(*ctor)(INativeRtlEmitter &m, HExpr, HExpr));
	void RewriteMov();
	void RewriteMovt();
	void RewriteMovw();
	void RewriteMrc();
	void RewriteMvn();
	void RewritePop();
	void RewritePush();
	void RewriteRsb();
	void RewriteStm(int, bool);
	void RewriteStr(BaseType dt);
	void RewriteStrex();
	void RewriteSubw();
	void RewriteTrap();
	void RewriteTst();
	void RewriteUdf();
	void RewriteUxth();

private:
	ULONG cRef;	// COM ref count.

	csh hcapstone;
	INativeRtlEmitter & m;
	INativeTypeFactory & ntf;
	INativeRewriterHost * host;
	cs_insn * instr;
	const uint8_t * rawBytes;
	size_t available;			// Available bytes left past rawBytes
	uint64_t address;
	RtlClass rtlClass;
	int32_t itState;
	arm_cc itStateCondition;

#if (_DEBUG || DEBUG) && !MONODEVELOP
	void EmitUnitTest();
	static int opcode_seen[];
#else
	void EmitUnitTest() {
	}
#endif
};