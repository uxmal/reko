#pragma once
/*
* Copyright (C) 1999-2025 John Källén.
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


class NativeInstruction : public ComBase, public INativeInstruction
{
public:
	NativeInstruction(cs_insn * instr, NativeInstructionInfo info);
	virtual ~NativeInstruction() override;

	STDMETHODIMP QueryInterface(REFIID iid, void ** ppvOut) override;
	STDMETHODIMP_(ULONG) AddRef() override { return ComBase::AddRef(); }
	STDMETHODIMP_(ULONG) Release() override { return ComBase::Release(); }

	STDMETHODIMP GetInfo(NativeInstructionInfo * info) override;
	STDMETHODIMP Render(INativeInstructionRenderer * w, MachineInstructionRendererFlags options) override;
private:
	bool WriteRegisterSetInstruction(const cs_insn & instr, INativeInstructionRenderer & renderer);
	void Write(const cs_insn & insn, const cs_arm_op & op, INativeInstructionRenderer & renderer, MachineInstructionRendererFlags options);
	void WriteShift(const cs_arm_op & op, INativeInstructionRenderer & renderer);
	void WriteMemoryOperand(const cs_insn & insn, const cs_arm_op & op, INativeInstructionRenderer & renderer);
	static bool IsLastOperand(const cs_insn & instr, const cs_arm_op * op)
	{
		auto ops = &instr.detail->arm.operands[0];
		return op == ops + (instr.detail->arm.op_count - 1);
	}

	void WriteImmShift(const char * op, int value, INativeInstructionRenderer & renderer);
	void WriteRegShift(const char * op, int value, INativeInstructionRenderer & renderer);
	static void WriteImmediateValue(int imm8, INativeInstructionRenderer & renderer);
	const char * RegName(int reg);
protected:
	cs_insn * instr;
	NativeInstructionInfo info;
};

