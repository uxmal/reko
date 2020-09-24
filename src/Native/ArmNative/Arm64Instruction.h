#pragma once
class Arm64Instruction : public NativeInstruction
{
public:
	Arm64Instruction(cs_insn * instr, NativeInstructionInfo info) :
		NativeInstruction(instr, info)
	{}

	STDMETHODIMP Render(INativeInstructionWriter * w, MachineInstructionWriterFlags options) override;

private:
	void Write(const cs_insn & instruction, const cs_arm64_op & , INativeInstructionWriter &, MachineInstructionWriterFlags);

};