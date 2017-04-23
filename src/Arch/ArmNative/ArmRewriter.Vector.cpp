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
#include "arm.h"
#include "ArmRewriter.h"

	void ArmRewriter::RewriteVldmia()
	{
		ConditionalSkip();
		//$TODO:
		/*
		auto rSrc = this->Operand(Dst());
		auto offset = 0;
		for (auto r : instr.ArchitectureDetail.Operands.Skip(1))
		{
			auto dst = this->Operand(r);
			IExpression * ea =
				offset != 0
				? m.IAdd(rSrc, m.Int32(offset))
				: rSrc;
			m.Assign(dst, m.Mem(dst.DataType, ea));
			offset += dst.DataType.Size;
		}
		if (instr.ArchitectureDetail.WriteBack)
		{
			m.Assign(rSrc, m.IAdd(rSrc, m.Int32(offset)));
		}
		*/
	}

	void ArmRewriter::RewriteVmov()
	{
		ConditionalSkip();
		//$TODO
		/*
		auto dst = this->Operand(Dst);
		auto src = this->Operand(Src1);
		auto fname = "__vmov_" + VectorElementType();
		m.Assign(dst,
			host.PseudoProcedure(fname, dst.DataType, src));
			*/
	}

	void ArmRewriter::RewriteVstmia()
	{
		ConditionalSkip();
		//$TODO
		/*
		auto rSrc = this->Operand(Dst());
		int offset = 0;
		for (auto r : instr.ArchitectureDetail.Operands.Skip(1))
		{
			auto dst = this->Operand(r);
			IExpression * ea =
				offset != 0
				? m.IAdd(rSrc, m.Int32(offset))
				: rSrc;
			m.Assign(m.Mem(dst.DataType, ea), dst);
			offset += dst.DataType.Size;
		}
		if (instr.ArchitectureDetail.WriteBack)
		{
			m.Assign(rSrc, m.IAdd(rSrc, m.Int32(offset)));
		}
		*/
	}

	const char * ArmRewriter::VectorElementType()
	{
		switch (instr.ArchitectureDetail.VectorDataType)
		{
		case ArmVectorDataType::I32: return "i32";
		default: NotImplementedYet(); return "(NYI)";
		}
	}

