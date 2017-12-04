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

#include "functions.h"
#include "ComBase.h"
#include "NativeInstruction.h"
#include "ArmArchitecture.h"

NativeInstruction::NativeInstruction(cs_insn * instr, NativeInstructionInfo info) :
	instr(instr), info(info)
{
	AddRef();
}

NativeInstruction::~NativeInstruction()
{
	if (instr != nullptr)
		cs_free(instr, 1);
	Dump("Destroying instruction");
}


HRESULT STDAPICALLTYPE NativeInstruction::QueryInterface(REFIID iid, void ** ppvOut)
{
	//OLECHAR risky[200];
	//StringFromGUID2(iid, risky, sizeof(risky));
	//Dump("NativeInstruction::QI(%S)", risky);
	if (iid == IID_INativeInstruction || iid == IID_IUnknown)
	{
		AddRef();
		*ppvOut = static_cast<INativeInstruction *>(this);
		return S_OK;
	}
	*ppvOut = nullptr;
	return E_NOINTERFACE;
}

void STDAPICALLTYPE NativeInstruction::GetInfo(NativeInstructionInfo * info)
{
	*info = this->info;
}

void STDAPICALLTYPE NativeInstruction::Render(INativeInstructionWriter * w, MachineInstructionWriterOptions options)
{
	auto & writer = *w;
	if (this->instr == nullptr)
	{
		writer.WriteOpcode("Invalid");
		return;
	}
	auto & instruction = *static_cast<cs_insn*>(this->instr);
	writer.WriteOpcode(instruction.mnemonic);
	auto ops = instruction.detail->arm.operands;
	if (instruction.detail->arm.op_count < 1)
		return;
	writer.Tab();
	if (WriteRegisterSetInstruction(instruction, writer))
		return;
	Write(instruction, ops[0], writer, options);
	if (instruction.detail->arm.op_count < 2)
		return;
	writer.WriteString(",");
	Write(instruction, ops[1], writer, options);
	if (instruction.detail->arm.op_count < 3)
		return;
	writer.WriteString(",");
	Write(instruction, ops[2], writer, options);
	if (instruction.detail->arm.op_count < 4)
		return;
	writer.WriteString(",");
	Write(instruction, ops[3], writer, options);
}


bool NativeInstruction::WriteRegisterSetInstruction(const cs_insn & instr, INativeInstructionWriter & writer)
{
	auto iStart = 0;
	switch (instr.id)
	{
	case ARM_INS_POP:
	case ARM_INS_PUSH:
		break;
	case ARM_INS_LDM:
	case ARM_INS_STM:
	case ARM_INS_STMDB:
		Write(instr, instr.detail->arm.operands[0], writer, MachineInstructionWriterOptions::None);
		if (instr.detail->arm.writeback)
			writer.WriteString("!");
		iStart = 1;
		writer.WriteString(",");
		break;
	default:
		return false;
	}

	writer.WriteString("{");
	auto sep = "";
	auto regPrev = ARM_REG_INVALID;
	auto reg = ARM_REG_INVALID;
	for (int i = iStart; i < instr.detail->arm.op_count; ++i)
	{
		reg = static_cast<arm_reg>(instr.detail->arm.operands[i].reg);
		if (regPrev == ARM_REG_INVALID)
		{
			writer.WriteString(sep);
			writer.WriteString(ArmArchitecture::aRegs[reg].Name);
			sep = ",";
		}
		else if (static_cast<int>(regPrev) + 1 == static_cast<int>(reg))
		{
			sep = "-";
		}
		else
		{
			if (sep == "-")
			{
				writer.WriteString(sep);
				writer.WriteString(ArmArchitecture::aRegs[regPrev].Name);
				sep = ",";
			}
			writer.WriteString(sep);
			writer.WriteString(ArmArchitecture::aRegs[reg].Name);
			sep = ",";
		}
		regPrev = reg;
	}
	if (sep[0] == '-')
	{
		writer.WriteChar('-');
		writer.WriteString(ArmArchitecture::aRegs[reg].Name);
	}
	writer.WriteString("}");
	return true;
}

static const char *  nosuffixRequired = ".Ee";

void NativeInstruction::Write(const cs_insn & insn, const cs_arm_op & op, INativeInstructionWriter & writer, MachineInstructionWriterOptions options)
{
	char risky[40];
	switch (op.type)
	{
	case ARM64_OP_IMM:
		if (insn.id == ARM_INS_B ||
			insn.id == ARM_INS_BL ||
			insn.id == ARM_INS_BLX)
		{
			writer.WriteString("$");
			snprintf(risky, sizeof(risky), "%08X", op.imm);
			writer.WriteAddress(risky, static_cast<uint32_t>(op.imm));
		}
		else
		{
			writer.WriteString("#");
			WriteImmediateValue(op.imm, writer);
		}
		break;
	case ARM64_OP_CIMM:
		snprintf(risky, sizeof(risky), "c%d", op.imm);
		writer.WriteString(risky);
		break;
	case ARM64_OP_REG:
		if (op.subtracted)
			writer.WriteChar('-');
		writer.WriteString(ArmArchitecture::aRegs[op.reg].Name);
		WriteShift(op, writer);
		break;
	case ARM64_OP_MEM:
		WriteMemoryOperand(insn, op, writer);
		return;
	case ARM64_OP_FP:
		snprintf(risky, sizeof(risky), "#%lf", op.fp);
		if (strcspn(risky, nosuffixRequired) == strlen(risky))
			strcat_s(risky, sizeof(risky), ".0");
		writer.WriteString(risky);
		break;
	default:
		writer.WriteString("$$ UNSUPPORTED");
		//throw new NotImplementedException(string.Format(
		//	"Can't disassemble {0} {1}. Unknown operand type: {2}",
		//	instruction.Mnemonic,
		//	instruction.Operand,
		//	op.Type));
	}
}

void NativeInstruction::WriteShift(const cs_arm_op & op, INativeInstructionWriter & writer)
{
	switch (op.shift.type)
	{
	case ARM_SFT_ASR: WriteImmShift("asr", op.shift.value, writer); break;
	case ARM_SFT_LSL: WriteImmShift("lsl", op.shift.value, writer); break;
	case ARM_SFT_LSR: WriteImmShift("lsr", op.shift.value, writer); break;
	case ARM_SFT_ROR: WriteImmShift("ror", op.shift.value, writer); break;
	case ARM_SFT_RRX: writer.WriteString(",rrx"); break;
	case ARM_SFT_ASR_REG: WriteRegShift("asr", op.shift.value, writer); break;
	case ARM_SFT_LSL_REG: WriteRegShift("lsl", op.shift.value, writer); break;
	case ARM_SFT_LSR_REG: WriteRegShift("lsr", op.shift.value, writer); break;
	case ARM_SFT_ROR_REG: WriteRegShift("ror", op.shift.value, writer); break;
	case ARM_SFT_RRX_REG: WriteRegShift("rrx", op.shift.value, writer); break;
	case ARM_SFT_INVALID: break;
	}
}

void NativeInstruction::WriteMemoryOperand(const cs_insn & insn, const cs_arm_op & op, INativeInstructionWriter & writer)
{
	writer.WriteChar('[');
	writer.WriteString(ArmArchitecture::aRegs[op.mem.base].Name);
	int displacement = op.mem.disp;
	if (displacement != 0)
	{
		if (true) // preincInternal.ArchitectureDetail)
		{
			writer.WriteString(",");
			if (displacement < 0)
			{
				displacement = -displacement;
				writer.WriteString("-");
			}
			writer.WriteString("#");
			WriteImmediateValue(displacement, writer);
			writer.WriteString("]");
			if (insn.detail->arm.writeback)
				writer.WriteString("!");
		}
		else
		{
			writer.WriteString("],");
			if (displacement < 0)
			{
				displacement = -displacement;
				writer.WriteString("-");
			}
			WriteImmediateValue(displacement, writer);
		}
	}
	else
	{
		if (op.mem.index != ARM_REG_INVALID)
		{
			writer.WriteString(",");
			if (op.subtracted)
				writer.WriteString("-");
			writer.WriteString(ArmArchitecture::aRegs[op.mem.index].Name);
		}
		if (op.shift.type != ARM_SFT_INVALID)
		{
			WriteShift(op, writer);
		}
		writer.WriteChar(']');
		if (insn.detail->arm.writeback && IsLastOperand(insn, &op))
			writer.WriteString("!");
	}
}

void NativeInstruction::WriteImmShift(const char * op, int value, INativeInstructionWriter &writer)
{
	writer.WriteString(",");
	writer.WriteOpcode(op);
	writer.WriteString(" #");
	WriteImmediateValue(value, writer);
}

void NativeInstruction::WriteRegShift(const char * op, int value, INativeInstructionWriter & writer)
{
	writer.WriteString(",");
	writer.WriteOpcode(op);
	writer.WriteChar(' ');
	writer.WriteString(ArmArchitecture::aRegs[value].Name);
}

void NativeInstruction::WriteImmediateValue(int imm8, INativeInstructionWriter & writer)
{
	if (imm8 > 256 && ((imm8 & (imm8 - 1)) == 0))
	{
		/* only one bit set, and that later than bit 8.
		* Represent as 1<<... .
		*/
		writer.WriteString("1<<");
		{
			uint32_t n = 0;
			while ((imm8 & 0x0F) == 0)
			{
				n += 4; imm8 = imm8 >> 4;
			}
			// Now imm8 is 1, 2, 4 or 8. 
			n += (uint32_t)((0x30002010 >> (int)(4 * (imm8 - 1))) & 15);
			writer.WriteUInt32(n);
		}
	}
	else
	{
		auto fmt = (-9 <= imm8 && imm8 <= 9) ? "%s%X" : "&%s%X";
		auto sign = "";
		if (((int)imm8) < 0 && ((int)imm8) > -100)
		{
			imm8 = -imm8;
			sign = "-";
		}
		char risky[200];
		snprintf(risky, sizeof(risky), fmt, sign, imm8);
		writer.WriteString(risky);
	}
}
