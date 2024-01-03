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
#include "NativeInstruction.h"
#include "Arm64Instruction.h"
#include "Arm64Architecture.h"

STDMETHODIMP Arm64Instruction::Render(INativeInstructionRenderer * r, MachineInstructionRendererFlags options)
{
	auto & renderer = *r;
	if (this->instr == nullptr)
	{
		renderer.WriteMnemonic("Invalid");
		return S_OK;
	}
	auto & instruction = *static_cast<cs_insn*>(this->instr);
	renderer.WriteMnemonic(instruction.mnemonic);
	auto & ops = instruction.detail->arm64.operands;
	if (instruction.detail->arm64.op_count < 1)
		return S_OK;
	renderer.Tab();
	Write(instruction, ops[0], renderer, options);
	if (instruction.detail->arm64.op_count < 2)
		return S_OK;
	renderer.WriteString(",");
	Write(instruction, ops[1], renderer, options);
	if (instruction.detail->arm64.op_count < 3)
		return S_OK;
	renderer.WriteString(",");
	Write(instruction, ops[2], renderer, options);
	if (instruction.detail->arm64.op_count < 4)
		return S_OK;
	renderer.WriteString(",");
	Write(instruction, ops[3], renderer, options);
	return S_OK;
}

void Arm64Instruction::Write(const cs_insn & instruction, const cs_arm64_op & op, INativeInstructionRenderer & renderer, MachineInstructionRendererFlags options)
{
	char risky[120];
	switch (op.type)
	{
	case ARM64_OP_REG:
		renderer.WriteString(Arm64Architecture::aRegs[op.reg].Name);
		return;
	case ARM64_OP_IMM:
		snprintf(risky, sizeof(risky), "#&%llX", (unsigned long long int)op.imm);
		renderer.WriteString(risky);
		return;
	default: 
		renderer.WriteUInt32(op.type);
		renderer.WriteString(" $$ NOT IMPLEMENTED YET");
		return;
	}
}
