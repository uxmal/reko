#pragma once

enum class PrimitiveType
{
	Void, 
	
	Bool,

	Byte,
	SByte,
	Char8,

	Int16,
	UInt16,
	Ptr16,
	Word16,

	Int32,
	UInt32,
	Ptr32,
	Word32,

	Int64,
	UInt64,
	Ptr64,
	Word64,

	Real32,
	Real64,
};

enum class ConditionCode
{
	None,
	UGT,	// Unsigned >
	ULE,	// Unsigned <=
	ULT,	// Unsigned <
	GT,		// >
	GE,		// >=
	LT,		// <
	LE,		// <=
	UGE,	// Unsigned >=
	NO,		// No overflow
	NS,		// >= 0
	NE,		// != 
	OV,		// Overflow
	SG,		// < 0
	EQ,		// ==	
	PE,     // Parity even
	PO,     // parity odd

	ALWAYS, // Some architectures have this.
	NEVER,
};

enum class RtlClass
{
	Linear,
	Transfer,
	ConditionalTransfer,
};

enum class TestCondition
{

};

class IExpression
{
};

class IFrame
{
public:
	virtual IExpression * CreateTemporary(PrimitiveType size) = 0;
	virtual IExpression * EnsureRegister(int reg) = 0;
	virtual IExpression * EnsureSequence(int regHi, int regLo, PrimitiveType size) = 0;
	virtual IExpression * EnsureFlagGroup(int baseReg, int bitmask, const char * name, PrimitiveType size) = 0;

};

class IRtlEmitter
{
public:
	virtual void Assign(IExpression * dst, IExpression * src) = 0;
	virtual void Branch(IExpression * exp, IExpression * dst, RtlClass rtlClass) = 0;
	virtual void BranchInMiddleOfInstruction(IExpression * exp, IExpression * dst, RtlClass) = 0;
	virtual void Call(IExpression * dst, int bytesOnStack) = 0;
	virtual void Goto(IExpression * dst) = 0;
	virtual void Invalid() = 0;
	virtual void Nop()=0;
	virtual void Return(int, int) = 0;
	virtual void SetRtlClass(RtlClass) = 0;
	virtual void SideEffect(IExpression *) = 0;

	virtual IExpression * And(IExpression *a, IExpression * b) = 0;
	virtual IExpression * And(IExpression *a, int n) = 0;
	virtual IExpression * Cast(PrimitiveType type, IExpression *a) = 0;
	virtual IExpression * Comp(IExpression *a) = 0;
	virtual IExpression * Cond(IExpression *a) = 0;
	virtual IExpression * Dpb(IExpression *dst, IExpression* src, int32_t pos) = 0;
	virtual IExpression * IAdc(IExpression * a, IExpression * b, IExpression * c) = 0;
	virtual IExpression * IAdd(IExpression * a, IExpression * b) = 0;
	virtual IExpression * IAdd(IExpression * a, int n) = 0;
	virtual IExpression * IMul(IExpression * a, IExpression * b) = 0;
	virtual IExpression * IMul(IExpression * a, int n) = 0;
	virtual IExpression * ISub(IExpression * a, IExpression * b) = 0;
	virtual IExpression * ISub(IExpression * a, int n) = 0;
	virtual IExpression * Mem(PrimitiveType dt, IExpression * ea) = 0;
	virtual IExpression * Mem8(IExpression * ea) = 0;
	virtual IExpression * Mem16(IExpression * ea) = 0;
	virtual IExpression * Mem32(IExpression * ea) = 0;
	virtual IExpression * Mem64(IExpression * ea) = 0;
	virtual IExpression * Not(IExpression * a) = 0;
	virtual IExpression * Or(IExpression *a, IExpression *b) = 0;
	virtual IExpression * Ror(IExpression * a, IExpression *b) = 0;
	virtual IExpression * Ror(IExpression * a, int n) = 0;
	virtual IExpression * Rrc(IExpression * a, IExpression *b) = 0;
	virtual IExpression * Rrc(IExpression * a, int n) = 0;
	virtual IExpression * Sar(IExpression * a, IExpression *b) = 0;
	virtual IExpression * Sar(IExpression * a, int n) = 0;
	virtual IExpression * Slice(IExpression *a, int32_t pos, int32_t bits) = 0;
	virtual IExpression * Shl(IExpression * a, IExpression *b) = 0;
	virtual IExpression * Shl(IExpression * a, int n) = 0;
	virtual IExpression * Shr(IExpression * a, IExpression *b) = 0;
	virtual IExpression * Shr(IExpression * a, int n) = 0;
	virtual IExpression * Test(ConditionCode cc, IExpression * exp) = 0;
	virtual IExpression * SMul(IExpression * a, IExpression * b) = 0;
	virtual IExpression * UMul(IExpression * a, IExpression * b) = 0;
	virtual IExpression * Xor(IExpression *a, IExpression *b) = 0;

	virtual IExpression * Byte(uint8_t) = 0;
	virtual IExpression * Int16(int16_t) = 0;
	virtual IExpression * Int32(int32_t) = 0;
	virtual IExpression * Int64(int64_t) = 0;
	virtual IExpression * Ptr16(uint16_t) = 0;
	virtual IExpression * Ptr32(uint32_t) = 0;
	virtual IExpression * Ptr64(uint64_t) = 0;
	virtual IExpression * UInt16(uint16_t) = 0;
	virtual IExpression * UInt32(uint32_t) = 0;
	virtual IExpression * UInt64(uint64_t) = 0;
	virtual IExpression * Word16(uint16_t) = 0;
	virtual IExpression * Word32(uint32_t) = 0;
	virtual IExpression * Word64(uint64_t) = 0;
};

class IRewriter
{
public:
	virtual void Next() = 0;
};

class IRewriterHost
{
public:
	virtual void Error(uint64_t uAddress, const char * error) = 0;
	virtual IExpression * PseudoProcedure(const char *name, PrimitiveType retType, /* args */...) = 0;
};
