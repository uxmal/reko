#pragma once

enum class PrimitiveType
{
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

enum class TestCondition
{

};

class IExpression
{
};

class IFrame
{

};

class IRtlEmitter
{
public:
	void Assign(IExpression * dst, IExpression * src);
	void Nop();

	IExpression * And(IExpression *a, IExpression * b);
	IExpression * And(IExpression *a, int n);
	IExpression * IAdd(IExpression * a, IExpression * b);
	IExpression * IAdd(IExpression * a, int n);
	IExpression * IMul(IExpression * a, IExpression * b);
	IExpression * IAdd(IExpression * a, int n);
	IExpression * ISub(IExpression * a, IExpression * b);
	IExpression * ISub(IExpression * a, int n);
	IExpression * Mem(PrimitiveType dt, IExpression * ea);
	IExpression * Mem8(IExpression * ea);
	IExpression * Mem16(IExpression * ea);
	IExpression * Mem32(IExpression * ea);
	IExpression * Mem64(IExpression * ea);
	IExpression * Or(IExpression *a, IExpression *b);
	IExpression * Ptr16(uint16_t);
	IExpression * Ptr32(uint32_t);
	IExpression * Ptr64(uint64_t);
	IExpression * SMul(IExpression * a, IExpression * b);
	IExpression * UMul(IExpression * a, IExpression * b);
	IExpression * Xor(IExpression *a, IExpression *b);
};

class IRewriter
{
public:
	virtual void Next();
};

class IRewriterHost
{
public:
	virtual void Frobulate() = 0;
};
