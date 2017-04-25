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
		m.SideEffect(m.Fn(host->EnsurePseudoProcedure("__cps_id", BaseType::Void, 1)));
		return;
	}
	NotImplementedYet();
}

void ArmRewriter::RewriteDmb()
{
	auto memBarrier = MemBarrierName(instr->detail->arm.mem_barrier);
	char name[100];
	snprintf(name, sizeof(name), "__dmb_%s", memBarrier);
	m.SideEffect(m.Fn(host->EnsurePseudoProcedure(name, BaseType::Void, 1)));
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
	auto ppp = host->EnsurePseudoProcedure("__mrs", BaseType::Word32, 1);
	m.AddArg(Operand(Src1()));
	m.Assign(Operand(Dst()), m.Fn(ppp));
}

void ArmRewriter::RewriteMsr()
{
	auto ppp = host->EnsurePseudoProcedure("__msr", BaseType::Word32, 2);
	m.AddArg(Operand(Dst()));
	m.AddArg(Operand(Src1()));
	m.SideEffect(m.Fn(ppp));
}

const char * ArmRewriter::MemBarrierName(arm_mem_barrier barrier)
{
	return "NOT_IMPLEMENTED";

}