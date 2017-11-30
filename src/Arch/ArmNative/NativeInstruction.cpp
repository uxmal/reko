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
	Dump("Created NativeInstruction");
}

NativeInstruction::~NativeInstruction()
{
	if (instr != nullptr)
		cs_free(instr, 1);
}

static const GUID IID_INativeInstruction =
{ 0x2caf9227, 0x76d6, 0x4ded, { 0xbc, 0x74, 0xb9, 0x58, 0x1, 0xe1, 0x52, 0x4e } };


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
		writer.Write("Invalid");
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
	writer.Write(",");
	Write(instruction, ops[1], writer, options);
	if (instruction.detail->arm.op_count < 3)
		return;
	writer.Write(",");
	Write(instruction, ops[2], writer, options);
	if (instruction.detail->arm.op_count < 4)
		return;
	writer.Write(",");
	Write(instruction, ops[3], writer, options);
}


bool NativeInstruction::WriteRegisterSetInstruction(const cs_insn & instr, INativeInstructionWriter & writer)
{
	auto ops = &instr.detail->arm.operands[0];
	switch (instr.id)
	{
	case ARM_INS_POP:
	case ARM_INS_PUSH:
		break;
	case ARM_INS_LDM:
	case ARM_INS_STM:
	case ARM_INS_STMDB:
		Write(instr, ops[0], writer, MachineInstructionWriterOptions::None);
		if (instr.detail->arm.writeback)
			writer.Write("!");
		ops = ops + 1;
		writer.Write(",");
		break;
	default:
		return false;
	}

	writer.Write("{");
	auto sep = "";
	auto regPrev = ARM_REG_INVALID;
	auto reg = ARM_REG_INVALID;
	auto opLast = ops + instr.detail->arm.op_count;
	for (auto op = ops; op != opLast; ++op)
	{
		reg = static_cast<arm_reg>(op->reg);
		if (regPrev == ARM_REG_INVALID)
		{
			writer.Write(sep);
			writer.Write(ArmArchitecture::aRegs[reg].Name);
			sep = ",";
		}
		else if (static_cast<int>(regPrev) + 1 < static_cast<int>(reg))
		{
			if (sep == "-")
			{
				writer.Write(sep);
				writer.Write(ArmArchitecture::aRegs[regPrev].Name);
				sep = ",";
			}
			writer.Write(sep);
			writer.Write(ArmArchitecture::aRegs[reg].Name);
			sep = ",";
		}
		else
		{
			sep = "-";
		}
		regPrev = reg;
	}
	if (sep == "-")
	{
		writer.Write("-");
		writer.Write(ArmArchitecture::aRegs[reg].Name);
	}
	writer.Write("}");
	return true;
}

static const char *  nosuffixRequired = ".Ee";

void NativeInstruction::Write(const cs_insn & insn, const cs_arm_op & op, INativeInstructionWriter & writer, MachineInstructionWriterOptions options)
{
	char risky[40];
	switch (op.type)
	{
	case ARM_OP_IMM:
		if (insn.id == ARM_INS_B ||
			insn.id == ARM_INS_BL ||
			insn.id == ARM_INS_BLX)
		{
			writer.Write("$");
			snprintf(risky, sizeof(risky), "%08X", op.imm);
			writer.WriteAddress(risky, static_cast<uint32_t>(op.imm));
		}
		else
		{
			writer.Write("#");
			WriteImmediateValue(op.imm, writer);
		}
		break;
	case ARM_OP_CIMM:
		snprintf(risky, sizeof(risky), "c%d", op.imm);
		writer.Write(risky);
		break;
	case ARM_OP_PIMM:
		snprintf(risky, sizeof(risky), "p%d", op.imm);
		writer.Write(risky);
		break;
	case ARM_OP_REG:
		if (op.subtracted)
			writer.Write('-');
		writer.Write(ArmArchitecture::aRegs[op.reg].Name);
		WriteShift(op, writer);
		break;
	case ARM_OP_SYSREG:
		writer.Write("$$ SYSREG NOT IMPLEMENTED YET");
		//writer.Write(A32Registers.SysRegisterByCapstoneID[op.SysRegisterValue.Value].Name);
		break;
	case ARM_OP_MEM:
		if (op.mem.base == ARM_REG_PC)
		{
			auto uAddr = static_cast<uint32_t>(insn.address + op.mem.disp) + 8u;
			if (op.mem.index == ARM_REG_INVALID &&
				((int)options & (int)MachineInstructionWriterOptions::ResolvePcRelativeAddress))
			{
				snprintf(risky, sizeof(risky), "%08X", uAddr);
				writer.Write('[');
				writer.WriteAddress(risky, uAddr);
				writer.Write(']');
				//$TODO: annotation
				//var sr = new StringRenderer();
				//WriteMemoryOperand(op, sr);
				//writer.AddAnnotation(sr.ToString());
			}
			else
			{
				WriteMemoryOperand(insn, op, writer);
				//$TODO: annotation
				// writer.AddAnnotation(addr.ToString());
			}
			return;
		}
		WriteMemoryOperand(insn, op, writer);
		break;
	case ARM_OP_SETEND:
		writer.Write("$$ SETEND NOT IMPLEMENTED YET");
		//writer.Write(op.setend.ToString().ToLowerInvariant());
		break;
	case ARM_OP_FP:
		snprintf(risky, sizeof(risky), "#%lf", op.fp);
		if (strcspn(risky, nosuffixRequired) == strlen(risky))
			strcat_s(risky, sizeof(risky), ".0");
		writer.Write(risky);
		break;
	default:
		writer.Write("$$ UNSUPPORTED");
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
	case ARM_SFT_RRX: writer.Write(",rrx"); break;
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
	writer.Write('[');
	writer.Write(ArmArchitecture::aRegs[op.mem.base].Name);
	int displacement = op.mem.disp;
	if (displacement != 0)
	{
		if (true) // preincInternal.ArchitectureDetail)
		{
			writer.Write(",");
			if (displacement < 0)
			{
				displacement = -displacement;
				writer.Write("-");
			}
			writer.Write("#");
			WriteImmediateValue(displacement, writer);
			writer.Write("]");
			if (insn.detail->arm.writeback)
				writer.Write("!");
		}
		else
		{
			writer.Write("],");
			if (displacement < 0)
			{
				displacement = -displacement;
				writer.Write("-");
			}
			WriteImmediateValue(displacement, writer);
		}
	}
	else
	{
		if (op.mem.index != ARM_REG_INVALID)
		{
			writer.Write(",");
			if (op.subtracted)
				writer.Write("-");
			writer.Write(ArmArchitecture::aRegs[op.mem.index].Name);
		}
		if (op.shift.type != ARM_SFT_INVALID)
		{
			WriteShift(op, writer);
		}
		writer.Write(']');
		if (insn.detail->arm.writeback && IsLastOperand(insn, &op))
			writer.Write("!");
	}
}

void NativeInstruction::WriteImmShift(const char * op, int value, INativeInstructionWriter &writer)
{
	writer.Write(",");
	writer.WriteOpcode(op);
	writer.Write(" #");
	WriteImmediateValue(value, writer);
}

void NativeInstruction::WriteRegShift(const char * op, int value, INativeInstructionWriter & writer)
{
	writer.Write(",");
	writer.WriteOpcode(op);
	writer.Write(' ');
	writer.Write(ArmArchitecture::aRegs[value].Name);
}

void NativeInstruction::WriteImmediateValue(int imm8, INativeInstructionWriter & writer)
{
	if (imm8 > 256 && ((imm8 & (imm8 - 1)) == 0))
	{
		/* only one bit set, and that later than bit 8.
		* Represent as 1<<... .
		*/
		writer.Write("1<<");
		{
			uint32_t n = 0;
			while ((imm8 & 0x0F) == 0)
			{
				n += 4; imm8 = imm8 >> 4;
			}
			// Now imm8 is 1, 2, 4 or 8. 
			n += (uint32_t)((0x30002010 >> (int)(4 * (imm8 - 1))) & 15);
			writer.Write(n);
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
		writer.Write(risky);
	}
}
