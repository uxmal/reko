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

void ArmRewriter::RewriteCps()
{
	if (instr->detail->arm.cps_mode == ARM_CPSMODE_ID)
	{
		m.SideEffect(host->PseudoProcedure("__cps_id", BaseType::Void));
		return;
	}
	NotImplementedYet();
}

void ArmRewriter::RewriteDmb()
{
	//$TODO
	/*
	auto memBarrier = instr->detail->arm.MemoryBarrier.ToString().ToLowerInvariant();
	auto name = "__dmb_" + memBarrier;
	m.SideEffect(host->PseudoProcedure(name, VoidType.Instance));
	*/
}

void ArmRewriter::RewriteMcr()
{
	//$TODO
	/*
	m.SideEffect(host->PseudoProcedure(
		"__mcr",
		PrimitiveType::Void,
		instr->detail->arm.Operands
		.Select(o = > Operand(o))
		.ToArray()));
		*/
}

void ArmRewriter::RewriteMrc()
{
	//$TODO
/*
	auto ops = instr->detail->arm.Operands
		.Select(o = > Operand(o))
		.ToList();
	auto regDst = ops.OfType<Identifier>().SingleOrDefault();
	ops.Remove(regDst);
	m.Assign(regDst, host->PseudoProcedure(
		"__mrc",
		PrimitiveType::Word32,
		ops.ToArray()));
		*/
}

void ArmRewriter::RewriteMrs()
{
	ConditionalSkip();
	m.Assign(Operand(Dst()), host->PseudoProcedure("__mrs", BaseType::Word32, Operand(Src1())));
}

void ArmRewriter::RewriteMsr()
{
	ConditionalSkip();
	m.SideEffect(host->PseudoProcedure("__msr", BaseType::Word32, Operand(Dst()), Operand(Src1())));
}

