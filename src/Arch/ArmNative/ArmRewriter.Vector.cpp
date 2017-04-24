/*
* Copyright (C) 1999-2017 John Källén.
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
#include "ArmRewriter.h"

void ArmRewriter::RewriteVldmia()
{
	auto rSrc = this->Operand(Dst());
	auto offset = 0;
	auto begin = &instr->detail->arm.operands[0];
	auto end = begin + instr->detail->arm.op_count - 1;
	for (auto r = begin; r != end; ++r)
	{
		auto dst = this->Operand(*r);
		HExpr ea =
			offset != 0
			? m.IAdd(rSrc, m.Int32(offset))
			: rSrc;
		//m.Assign(dst, m.Mem(dst->DataType, ea));
		//offset += dst.DataType.Size;
	}
	if (instr->detail->arm.writeback)
	{
		m.Assign(rSrc, m.IAdd(rSrc, m.Int32(offset)));
	}
}

void ArmRewriter::RewriteVmov()
{
	auto dst = this->Operand(Dst());
	auto src = this->Operand(Src1());
	char fname[200];
	snprintf(fname, sizeof(fname), "__vmov_%s", VectorElementType());
	auto dt = VectorElementDataType();
	//m.Assign(dst,
	//	host->PseudoProcedure(fname, dst.DataType, src));
}

void ArmRewriter::RewriteVstmia()
{
	auto rSrc = this->Operand(Dst());
	int offset = 0;
	auto begin = &instr->detail->arm.operands[0];
	auto end = begin + instr->detail->arm.op_count - 1;
	for (auto r = begin; r != end; ++r)
	{
		auto dst = this->Operand(*r);
		HExpr ea =
			offset != 0
			? m.IAdd(rSrc, m.Int32(offset))
			: rSrc;
		//m.Assign(m.Mem(dst.DataType, ea), dst);
		//offset += dst.DataType.Size;
	}
	if (instr->detail->arm.writeback)
	{
		m.Assign(rSrc, m.IAdd(rSrc, m.Int32(offset)));
	}
}

const char * ArmRewriter::VectorElementType()
{
	switch (instr->detail->arm.vector_size)
	{
	case ARM_VECTORDATA_I32: return "i32";
	default: NotImplementedYet(); return "(NYI)";
	}
}

BaseType ArmRewriter::VectorElementDataType()
{
	switch (instr->detail->arm.vector_size)
	{
	case ARM_VECTORDATA_I32: return BaseType::Int32;
	default: NotImplementedYet(); return BaseType::Void;
	}
}


