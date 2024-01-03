/*
* Copyright (C) 1999-2024 John K�ll�n.
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

#include "stdafx.h"
#include "reko.h"

#include "ComBase.h"
#include "ArmRewriter.h"

void ArmRewriter::RewriteVecBinOp(BinOpEmitter fn)
{
	auto src1 = Operand(Src1());
	auto src2 = Operand(Src2());
	auto dst = Operand(Dst(), BaseType::Word32, true);
	m.Assign(dst, (m.*fn)(src1, src2));
}

void ArmRewriter::RewriteVcmp()
{
	auto src1 = Operand(Dst(), BaseType::Word32, true);
	auto src2 = Operand(Src1());
	auto fpscr = host->EnsureFlagGroup((int)ARM_REG_FPSCR, 0xF0000000, "NZCV", BaseType::Word32);
	m.Assign(fpscr, m.Cond(m.FSub(src1, src2)));
}

void ArmRewriter::RewriteVcvt()
{
	auto src = Operand(Src1());
	auto dst = Operand(Dst(), BaseType::Word32, true);
	BaseType dstType;
	switch (instr->detail->arm.vector_data)
	{
	case ARM_VECTORDATA_F64S32: dstType = BaseType::Real64; break;
	case ARM_VECTORDATA_F32S32: dstType = BaseType::Real32; break;
	default: NotImplementedYet(); return;
	}
	m.Assign(dst, m.Cast(dstType, src));
}

void ArmRewriter::RewriteVext()
{
	auto src1 = Operand(Src1());
	auto src2 = Operand(Src2());
	auto src3 = Operand(Src3());
	auto dst = Operand(Dst(), BaseType::Word32, true);
	auto intrinsic = host->EnsureIntrinsicProcedure("__vext", false, register_types[Dst().reg], 3);
	m.AddArg(src1);
	m.AddArg(src2);
	m.AddArg(src3);
	m.Assign(dst, m.Fn(intrinsic));
}

void ArmRewriter::RewriteVldmia()
{
	auto rSrc = this->Operand(Dst(), BaseType::Word32, true);
	auto offset = 0;
	auto begin = &instr->detail->arm.operands[1];
	auto end = begin + instr->detail->arm.op_count - 1;
	for (auto r = begin; r != end; ++r)
	{
		auto dst = this->Operand(*r);
		HExpr ea =
			offset != 0
			? m.IAdd(rSrc, m.Int32(offset))
			: rSrc;
		auto dt = register_types[r->reg];
		m.Assign(dst, m.Mem(register_types[r->reg], ea));
		offset += type_sizes[(int)dt];
	}
	if (instr->detail->arm.writeback)
	{
		m.Assign(rSrc, m.IAdd(rSrc, m.Int32(offset)));
	}
}

void ArmRewriter::RewriteVldr()
{
	auto dst = this->Operand(Dst(), BaseType::Word32, true);
	auto src = this->Operand(Src1());
	m.Assign(dst, src);
}

void ArmRewriter::RewriteVmov()
{
	auto dst = this->Operand(Dst(), BaseType::Word32, true);
	auto src = this->Operand(Src1());
	if (instr->detail->arm.vector_data != ARM_VECTORDATA_INVALID)
	{
		auto dt = VectorElementDataType(instr->detail->arm.vector_data);
		auto dstType = register_types[Dst().reg];
		auto srcType = register_types[Src1().reg];
		auto srcElemSize = type_sizes[(int)VectorElementDataType(instr->detail->arm.vector_data)];
		auto celemSrc = type_sizes[(int)dstType] / srcElemSize;
		auto arrDst = ntf.ArrayOf((HExpr)dstType, celemSrc);

		if (Src1().type == ARM_OP_IMM)
		{
			auto arrSrc = ntf.ArrayOf((HExpr)dt, celemSrc);
			for (int i = 0; i < celemSrc; ++i)
			{
				auto arg = Operand(Src1());
				m.AddArg(arg);
			}
			m.Assign(dst, m.Seq(arrSrc));
			return;
		}

		char fname[200];
		snprintf(fname, sizeof(fname), "__vmov_%s", VectorElementType(instr->detail->arm.vector_data));
		auto ppp = host->EnsureIntrinsicProcedure(fname, false, register_types[Dst().reg], 1);
		m.AddArg(src);
		m.Assign(dst, m.Fn(ppp));
	}
	else
	{
		m.Assign(dst, src);
	}
}

void ArmRewriter::RewriteVmrs()
{
	m.Assign(Operand(Dst()), Operand(Src1()));
}

void ArmRewriter::RewriteVmvn()
{
	if (Src1().type == ARM_OP_IMM)
	{
		RewriteVectorBinOp("__vmvn_imm_%s");
	}
	else
	{
		RewriteVectorBinOp("__vmvn_%s");
	}
}

void ArmRewriter::RewriteVpop()
{
	int offset = 0;
	auto begin = &instr->detail->arm.operands[0];
	auto end = begin + instr->detail->arm.op_count;
	auto sp = Reg(ARM_REG_SP);
	auto reg_size = type_sizes[(int)register_types[begin->reg]];
	for (auto r = begin; r != end; ++r)
	{
		auto dst = Reg(r->reg);
		HExpr ea = offset != 0
			? m.IAdd(sp, m.Int32(offset))
			: sp;
		auto dt = register_types[r->reg];
		m.Assign(dst, m.Mem(dt, ea));
		offset += type_sizes[(int)dt];
	}
	// Release space used by registers
	m.Assign(sp, m.IAdd(sp, m.Int32(instr->detail->arm.op_count * reg_size)));
}

void ArmRewriter::RewriteVpush()
{
	int offset = 0;
	auto begin = &instr->detail->arm.operands[0];
	auto end = begin + instr->detail->arm.op_count;
	auto sp = Reg(ARM_REG_SP);
	auto reg_size = type_sizes[(int)register_types[begin->reg]];
	// Allocate space for the registers
	m.Assign(sp, m.ISub(sp, m.Int32(instr->detail->arm.op_count * reg_size)));
	for (auto r = begin; r != end; ++r)
	{
		auto src = Reg(r->reg);
		HExpr ea = offset != 0
			? m.IAdd(sp, m.Int32(offset))
			: sp;
		auto dt = register_types[r->reg];
		m.Assign(m.Mem(dt, ea), src);
		offset += type_sizes[(int)dt];
	}
}

void ArmRewriter::RewriteVstmia()
{
	auto rSrc = this->Operand(Dst(), BaseType::Word32, true);
	int offset = 0;
	auto begin = &instr->detail->arm.operands[1];
	auto end = begin + instr->detail->arm.op_count - 1;
	for (auto r = begin; r != end; ++r)
	{
		auto dst = this->Operand(*r);
		HExpr ea =
			offset != 0
			? m.IAdd(rSrc, m.Int32(offset))
			: rSrc;
		auto dt = register_types[r->reg];
		m.Assign(m.Mem(dt, ea), dst);
		offset += type_sizes[(int)dt];
	}
	if (instr->detail->arm.writeback)
	{
		m.Assign(rSrc, m.IAdd(rSrc, m.Int32(offset)));
	}
}

void ArmRewriter::RewriteVsqrt()
{
	auto fnname = instr->detail->arm.vector_data == ARM_VECTORDATA_F32 ? "sqrtf" : "sqrt";
	auto dt = instr->detail->arm.vector_data == ARM_VECTORDATA_F32 ? BaseType::Real32 : BaseType::Real64;
	auto src = this->Operand(Src1());
	auto dst = this->Operand(Dst(), BaseType::Word32, true);
	auto ppp = host->EnsureIntrinsicProcedure(fnname, false, dt, 1);
	m.AddArg(src);
	m.Assign(dst, m.Fn(ppp));
}

void ArmRewriter::RewriteVdup()
{
	auto src = this->Operand(Src1());
	auto dst = this->Operand(Dst(), BaseType::Word32, true);
	ntf.AddRef();
	auto dstType = register_types[Dst().reg];
	auto srcType = register_types[Src1().reg];
	auto celem = type_sizes[(int)dstType] / (instr->detail->arm.vector_size / 8);
	auto arrType = ntf.ArrayOf((HExpr)srcType, celem);
	char fnName[20];
	snprintf(fnName, sizeof(fnName), "__vdup_%d", instr->detail->arm.vector_size);
	auto intrinsic = host->EnsureIntrinsicProcedure(fnName, false, (BaseType) (int)arrType, 1);
	m.AddArg(src);
	m.Assign(dst, m.Fn(intrinsic));
}

void ArmRewriter::RewriteVmul()
{
	auto src1 = this->Operand(Src1());
	auto src2 = this->Operand(Src2());
	auto dst = this->Operand(Dst(), BaseType::Word32, true);
	ntf.AddRef();
	auto dstType = register_types[Dst().reg];
	auto srcType = register_types[Src1().reg];
	auto srcElemSize = type_sizes[(int)VectorElementDataType(instr->detail->arm.vector_data)];
	auto celemSrc = type_sizes[(int)srcType] / srcElemSize;
	auto arrSrc = ntf.ArrayOf((HExpr)srcType, celemSrc);
	auto arrDst = ntf.ArrayOf((HExpr)dstType, celemSrc);
	char fnName[20];
	snprintf(fnName, sizeof(fnName), "__vmul_%s", VectorElementType(instr->detail->arm.vector_data));
	auto intrinsic = host->EnsureIntrinsicProcedure(fnName, false, (BaseType)(int)arrDst, 1);
	m.AddArg(src1);
	m.AddArg(src2);
	m.Assign(dst, m.Fn(intrinsic));
}

void ArmRewriter::RewriteVectorUnaryOp(const char * fnNameFormat)
{
	RewriteVectorUnaryOp(fnNameFormat, instr->detail->arm.vector_data);
}

void ArmRewriter::RewriteVectorUnaryOp(const char * fnNameFormat, arm_vectordata_type elemType)
{
	auto src1 = this->Operand(Src1());
	auto dst = this->Operand(Dst(), BaseType::Word32, true);
	auto dstType = register_types[Dst().reg];
	auto srcType = register_types[Src1().reg];
	auto srcElemSize = type_sizes[(int)VectorElementDataType(elemType)];
	auto celemSrc = type_sizes[(int)srcType] / srcElemSize;
	auto arrSrc = ntf.ArrayOf((HExpr)srcType, celemSrc);
	auto arrDst = ntf.ArrayOf((HExpr)dstType, celemSrc);
	char fnName[20];
	snprintf(fnName, sizeof(fnName), fnNameFormat, VectorElementType(elemType));
	auto intrinsic = host->EnsureIntrinsicProcedure(fnName, false, (BaseType)(int)arrDst, 1);
	m.AddArg(src1);
	m.Assign(dst, m.Fn(intrinsic));
}

void ArmRewriter::RewriteVectorBinOp(const char * fnNameFormat)
{
	RewriteVectorBinOp(fnNameFormat, instr->detail->arm.vector_data);
}

void ArmRewriter::RewriteVectorBinOp(const char * fnNameFormat, arm_vectordata_type elemType)
{
	auto src1 = this->Operand(Src1());
	auto src2 = this->Operand(Src2());
	auto dst = this->Operand(Dst(), BaseType::Word32, true);
	auto dstType = register_types[Dst().reg];
	auto srcType = register_types[Src1().reg];
	auto srcElemSize = type_sizes[(int)VectorElementDataType(elemType)];
	auto celemSrc = type_sizes[(int)srcType] / srcElemSize;
	auto arrSrc = ntf.ArrayOf((HExpr)srcType, celemSrc);
	auto arrDst = ntf.ArrayOf((HExpr)dstType, celemSrc);
	char fnName[20];
	snprintf(fnName, sizeof(fnName), fnNameFormat, VectorElementType(elemType));
	auto intrinsic = host->EnsureIntrinsicProcedure(fnName, false, (BaseType)(int)arrDst, 1);
	m.AddArg(src1);
	m.AddArg(src2);
	m.Assign(dst, m.Fn(intrinsic));
}

void ArmRewriter::RewriteVstr()
{
	auto src = this->Operand(Dst(), BaseType::Word32, true);
	auto dst = this->Operand(Src1());
	m.Assign(dst, src);
}

const char * ArmRewriter::VectorElementType(arm_vectordata_type elemType)
{
	switch (elemType)
	{
	case ARM_VECTORDATA_I8: return "i8";
	case ARM_VECTORDATA_S8: return "s8";
	case ARM_VECTORDATA_U8: return "u8";
	case ARM_VECTORDATA_I16: return "i16";
	case ARM_VECTORDATA_S16: return "s16";
	case ARM_VECTORDATA_U16: return "u16";
	case ARM_VECTORDATA_F32: return "f32";
	case ARM_VECTORDATA_I32: return "i32";
	case ARM_VECTORDATA_S32: return "s32";
	case ARM_VECTORDATA_U32: return "u32";
	case ARM_VECTORDATA_F64: return "f64";
	case ARM_VECTORDATA_S64: return "s64";
	default: NotImplementedYet(); return "(NYI)";
	}
}

BaseType ArmRewriter::VectorElementDataType(arm_vectordata_type elemType)
{
	switch (elemType)
	{
	case ARM_VECTORDATA_I8: return BaseType::SByte;
	case ARM_VECTORDATA_S8: return BaseType::SByte;
	case ARM_VECTORDATA_U8: return BaseType::Byte;
	case ARM_VECTORDATA_I16: return BaseType::Int16;
	case ARM_VECTORDATA_S16: return BaseType::Int16;
	case ARM_VECTORDATA_U16: return BaseType::UInt16;
	case ARM_VECTORDATA_F32: return BaseType::Real32;
	case ARM_VECTORDATA_I32: return BaseType::Int32;
	case ARM_VECTORDATA_S32: return BaseType::Int32;
	case ARM_VECTORDATA_U32: return BaseType::UInt32;
	case ARM_VECTORDATA_F64: return BaseType::Real64;
	case ARM_VECTORDATA_S64: return BaseType::Int64;
	default: NotImplementedYet(); return BaseType::Void;
	}
}


