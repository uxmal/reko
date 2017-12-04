#include "stdafx.h"

#include "reko.h"
#include "ComBase.h"
#include "NativeInstruction.h"
#include "Arm64Instruction.h"
#include "Arm64Architecture.h"

void Arm64Instruction::Render(INativeInstructionWriter * w, MachineInstructionWriterOptions options)
{
	auto & writer = *w;
	if (this->instr == nullptr)
	{
		writer.WriteOpcode("Invalid");
		return;
	}
	auto & instruction = *static_cast<cs_insn*>(this->instr);
	writer.WriteOpcode(instruction.mnemonic);
	auto & ops = instruction.detail->arm64.operands;
	if (instruction.detail->arm64.op_count < 1)
		return;
	writer.Tab();
	Write(instruction, ops[0], writer, options);
	if (instruction.detail->arm64.op_count < 2)
		return;
	writer.WriteString(",");
	Write(instruction, ops[1], writer, options);
	if (instruction.detail->arm64.op_count < 3)
		return;
	writer.WriteString(",");
	Write(instruction, ops[2], writer, options);
	if (instruction.detail->arm64.op_count < 4)
		return;
	writer.WriteString(",");
	Write(instruction, ops[3], writer, options);
}

void Arm64Instruction::Write(const cs_insn & instruction, const cs_arm64_op & op, INativeInstructionWriter & writer, MachineInstructionWriterOptions options)
{
	char risky[120];
	switch (op.type)
	{
	case ARM64_OP_REG:
		writer.WriteString(Arm64Architecture::aRegs[op.reg].Name);
		return;
	case ARM64_OP_IMM:
		snprintf(risky, sizeof(risky), "#&%X", op.imm);
		writer.WriteString(risky);
		return;
	default: 
		writer.WriteUInt32(op.type);
		writer.WriteString(" $$ NOT IMPLEMENTED YET");
		return;
	}
}
