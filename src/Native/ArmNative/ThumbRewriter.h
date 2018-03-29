#pragma once

//$REFACTOR: a lot of this code is duplicated across Arm32 and Thumb. Extract a
// base class (ArmCommon?) and put this stuff there.

class ThumbRewriter : public ArmRewriter
{
public:
	ThumbRewriter(
		const uint8_t * rawBytes,
		size_t length,
		uint64_t address,
		INativeRtlEmitter * emitter,
		INativeTypeFactory * ntf,
		INativeRewriterHost * host);

protected: 
	virtual void ConditionalSkip(bool force) override;
	virtual void PostRewrite() override;

	virtual void RewriteIt() override;

private:
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

