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

void ArmRewriter::RewriteAdcSbc(BinOpEmitter opr)
{
		ConditionalSkip();
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1());
		auto opSrc2 = this->Operand(Src2());
		// We do not take the trouble of widening the CF to the word size
		// to simplify code analysis in later stages. 
		auto c = frame.EnsureFlagGroup((int)ArmRegister::CPSR, (int)FlagM::CF, "C", PrimitiveType::Bool);
		ConditionalAssign(
			opDst,
			(m.*opr)(
				(m.*opr)(opSrc1, opSrc2),
				c));
		MaybeUpdateFlags(opDst);
	}

class Bits
{
public:
	static uint32_t Mask(int, int) { return 0; }
};

	void ArmRewriter::RewriteBfc()
	{
		auto opDst = this->Operand(Dst());
		auto lsb = instr.ArchitectureDetail.Operands[1].ImmediateValue;
		auto bitsize = instr.ArchitectureDetail.Operands[2].ImmediateValue;
		ConditionalSkip();
		m.Assign(opDst, m.And(opDst, ~Bits::Mask(lsb, bitsize)));
	}

	void ArmRewriter::RewriteBfi()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1());
		//$TODO
		/*
		auto tmp = frame.CreateTemporary(opDst.DataType);
		auto lsb = instr.ArchitectureDetail.Operands[2].ImmediateValue;
		auto bitsize = instr.ArchitectureDetail.Operands[3].ImmediateValue;
		ConditionalSkip();
		m.Assign(tmp, m.Slice(opSrc, 0, bitsize));
		m.Assign(opDst, m.Dpb(opDst, tmp, lsb));
		*/
	}

	void ArmRewriter::RewriteBinOp(BinOpEmitter op, bool setflags)
	{
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1());
		auto opSrc2 = this->Operand(Src2());
		ConditionalAssign(opDst, (m.*op)(opSrc1, opSrc2));
		if (setflags)
		{
			ConditionalAssign(frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte), m.Cond(opDst));
		}
	}

	void ArmRewriter::RewriteRev()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1());
		ConditionalAssign(
			opDst,
			host.PseudoProcedure("__rev", PrimitiveType::Word32, opSrc));
	}

	void ArmRewriter::RewriteRevBinOp(BinOpEmitter op, bool setflags)
	{
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1());
		auto opSrc2 = this->Operand(Src2());
		ConditionalAssign(opDst, (m.*op)(opSrc1, opSrc2));
		if (setflags)
		{
			ConditionalAssign(frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte), m.Cond(opDst));
		}
	}

	void ArmRewriter::RewriteUnaryOp(UnaryOpEmitter op)
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1());
		ConditionalAssign(opDst, (m.*op)(opSrc));
		if (instr.ArchitectureDetail.UpdateFlags)
		{
			ConditionalAssign(frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte), m.Cond(opDst));
		}
	}

	void ArmRewriter::RewriteBic()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1());
		auto opSrc2 = this->Operand(Src2());
		ConditionalAssign(opDst, m.And(opSrc1, m.Comp(opSrc2)));
	}

	void ArmRewriter::RewriteClz()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1());

		ConditionalAssign(
			opDst,
			host.PseudoProcedure("__clz", PrimitiveType::Int32, opSrc));
	}

	void ArmRewriter::RewriteCmn()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1());
		ConditionalAssign(
			frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte),
			m.Cond(
				m.IAdd(opDst, opSrc)));
	}

	void ArmRewriter::RewriteCmp()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1());
		ConditionalAssign(
			frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte),
			m.Cond(
				m.ISub(opDst, opSrc)));
	}

	void ArmRewriter::RewriteTeq()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1());
		m.Assign(
			frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte),
			m.Cond(m.Xor(opDst, opSrc)));
	}

	void ArmRewriter::RewriteTst()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1());
		m.Assign(
			frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte),
			m.Cond(m.And(opDst, opSrc)));
	}

	void ArmRewriter::RewriteLdr(PrimitiveType size)
	{
		auto opSrc = this->Operand(Src1());
		auto opDst = this->Operand(Dst());
		auto rDst = Dst().RegisterValue;
		if (rDst == ArmRegister::PC)
		{
			// Assignment to PC is the same as a jump
			m.SetRtlClass(RtlClass::Transfer);
			m.Goto(opSrc);
			return;
		}
		m.Assign(opDst, opSrc);
		MaybePostOperand(Src1());
	}

	void ArmRewriter::RewriteLdrd()
	{
		auto ops = instr.ArchitectureDetail.Operands;
		auto regLo = (int)ops[0].RegisterValue;
		auto regHi = (int)ops[1].RegisterValue;
		auto opDst = frame.EnsureSequence(regHi, regLo, PrimitiveType::Word64);
		auto opSrc = this->Operand(ops[2]);
		m.Assign(opDst, opSrc);
		MaybePostOperand(ops[2]);
	}

	void ArmRewriter::RewriteStr(PrimitiveType size)
	{
		auto opSrc = this->Operand(Dst());
		auto opDst = this->Operand(Src1());
		m.Assign(opDst, opSrc);
		MaybePostOperand(Src1());
	}

	void ArmRewriter::RewriteStrd()
	{
		auto ops = instr.ArchitectureDetail.Operands;
		auto regLo = (int)ops[0].RegisterValue;
		auto regHi = (int)ops[1].RegisterValue;
		auto opSrc = frame.EnsureSequence(regHi, regLo, PrimitiveType::Word64);
		auto opDst = this->Operand(ops[2]);
		m.Assign(opDst, opSrc);
		MaybePostOperand(ops[2]);
	}

	void ArmRewriter::RewriteMultiplyAccumulate(BinOpEmitter op)
	{
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1());
		auto opSrc2 = this->Operand(Src2());
		auto opSrc3 = this->Operand(Src3());
		ConditionalAssign(opDst, (m.*op)(opSrc3, m.IMul(opSrc1, opSrc2)));
		if (instr.ArchitectureDetail.UpdateFlags)
		{
			ConditionalAssign(frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte), m.Cond(opDst));
		}
	}

	void ArmRewriter::RewriteMov()
	{
		if (Dst().Type == ArmInstructionOperandType::Register && Dst().RegisterValue == ArmRegister::PC)
		{
			m.SetRtlClass(RtlClass::Transfer);
			if (Src1().Type == ArmInstructionOperandType::Register && Src1().RegisterValue == ArmRegister::LR)
			{
				//$TODO:
				//AddConditional([]() => { m.RtlReturn(0, 0, RtlClass.Transfer);));
			}
			else
			{
				//AddConditional(new RtlGoto(Operand(Src1()), RtlClass.Transfer));
			}
			return;
		}
		auto opDst = Operand(Dst());
		auto opSrc = Operand(Src1());
		ConditionalAssign(opDst, opSrc);
	}

	void ArmRewriter::RewriteMovt()
	{
		auto opDst = Operand(Dst());
		auto iSrc = Src1().ImmediateValue;
		auto opSrc = m.Dpb(opDst, m.Word16((uint16_t)iSrc), 16);
		ConditionalAssign(opDst, opSrc);
	}

	void ArmRewriter::RewriteLdm(int initialOffset)
	{
		auto dst = this->Operand(Dst());
		auto range = instr.ArchitectureDetail.Operands + 1;
		int count = instr.ArchitectureDetail.Length - 1;
		RewriteLdm(dst, range, count, initialOffset, instr.ArchitectureDetail.WriteBack);
	}

	void ArmRewriter::RewriteLdm(IExpression  * dst, const ArmInstructionOperand * range, int length, int offset, bool writeback)
	{
		ConditionalSkip();
		bool pcRestored = false;
		/*
		for (auto r : range)
		{
			IExpression * ea = offset != 0
				? m.IAdd(dst, m.Int32(offset))
				: dst;
			if (r.RegisterValue.Value == ArmRegister::PC)
			{
				pcRestored = true;
			}
			else
			{
				auto dstReg = frame.EnsureRegister(r.RegisterValue);
				m.Assign(dstReg, m.Mem32(ea));
			}
			offset += 4;
		}
		if (writeback)
		{
			m.Assign(dst, m.IAdd(dst, m.Int32(offset)));
		}
		if (pcRestored)
		{
			m.Return(0, 0);
		}
		*/
	}

	void ArmRewriter::RewriteMulbb(bool hiLeft, bool hiRight, PrimitiveType dtMultiplicand, BinOpEmitter mul)
	{
		if (hiLeft || hiRight)
		{
			NotImplementedYet();
			return;
		}
		auto opDst = this->Operand(Dst());
		auto opLeft = m.Cast(dtMultiplicand, this->Operand(Src1()));
		auto opRight = m.Cast(dtMultiplicand, this->Operand(Src2()));
		m.Assign(opDst, (m.*mul)(opLeft, opRight));
	}

	void ArmRewriter::RewriteMull(PrimitiveType dtResult, BinOpEmitter op)
	{
		auto ops = instr.ArchitectureDetail.Operands;
		auto regLo = (int)ops[0].RegisterValue;
		auto regHi = (int)ops[1].RegisterValue;

		auto opDst = frame.EnsureSequence(regHi, regLo, dtResult);
		auto opSrc1 = this->Operand(Src3());
		auto opSrc2 = this->Operand(Src2());
		ConditionalAssign(opDst, (m.*op)(opSrc1, opSrc2));
		if (instr.ArchitectureDetail.UpdateFlags)
		{
			ConditionalAssign(frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte), m.Cond(opDst));
		}
	}

	void ArmRewriter::RewritePop()
	{
		auto sp = Reg(ArmRegister::SP);
		//$TODO
		//RewriteLdm(sp, instr.ArchitectureDetail.Operands, 0, true);
	}

	void ArmRewriter::RewritePush()
	{
		//$TODO:
		/*
		int offset = 0;
		auto dst = frame.EnsureRegister(A32Registers.sp);
		foreach(auto op in instr.ArchitectureDetail.Operands)
		{
			Expression ea = offset != 0
				? m.ISub(dst, offset)
				: (Expression)dst;
			auto reg = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[op.RegisterValue.Value]);
			m.Assign(m.LoadDw(ea), reg);
			offset += reg.DataType.Size;
		}
		m.Assign(dst, m.ISub(dst, offset));
		*/
	}

	void ArmRewriter::RewriteSbfx()
	{
		auto dst = this->Operand(Dst());
		auto src = m.Cast(
			PrimitiveType::Int32,
			m.Slice(
				this->Operand(Src1()),
				Src2().ImmediateValue,
				Src3().ImmediateValue));
		ConditionalAssign(dst, src);
	}

	void ArmRewriter::RewriteStm()
	{
		auto dst = this->Operand(Dst());
		//$TODO
		/*
		auto range = instr.ArchitectureDetail.Operands.Skip(1);
		int offset = 0;
		foreach(auto r in range)
		{
			Expression ea = offset != 0
				? m.ISub(dst, offset)
				: (Expression)dst;
			auto srcReg = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[r.RegisterValue.Value]);
			m.Assign(m.LoadDw(ea), srcReg);
			offset += srcReg.DataType.Size;
		}
		if (offset != 0 && instr.ArchitectureDetail.WriteBack)
		{
			m.Assign(dst, m.ISub(dst, offset));
		}
		*/
	}

	void ArmRewriter::RewriteStmib()
	{
		//$TODO
		/*
		auto dst = this->Operand(Dst());
		auto range = instr.ArchitectureDetail.Operands.Skip(1);
		int offset = 4;
		foreach(auto r in range)
		{
			Expression ea = m.IAdd(dst, Constant.Int32(offset));
			auto srcReg = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[r.RegisterValue.Value]);
			m.Assign(m.LoadDw(ea), srcReg);
			offset += 4;
		}
		if (offset != 4 && instr.ArchitectureDetail.WriteBack)
		{
			m.Assign(dst, m.IAdd(dst, Constant.Int32(offset)));
		}
#if NYI
		auto dst = frame.EnsureRegister(((RegisterOperand)Dst()).Register);
		auto range = (RegisterRangeOperand)Src1();
		int offset = 0;
		foreach(auto r in range.GetRegisters())
		{
			auto srcReg = frame.EnsureRegister(arch.GetRegister(r));
			offset += srcReg.DataType.Size;
			Expression ea = offset != 0
				? emitter.ISub(dst, offset)
				: (Expression)dst;
			emitter.Assign(emitter.LoadDw(ea), srcReg);
		}
		if (offset != 0 && instr.Update)
		{
			emitter.Assign(dst, emitter.ISub(dst, offset));
		}
#endif
*/
	}

	void ArmRewriter::RewriteUbfx()
	{
		auto dst = this->Operand(Dst());
		auto src = m.Cast(
			PrimitiveType::UInt32,
			m.Slice(
				this->Operand(Src1()),
				Src2().ImmediateValue,
				Src3().ImmediateValue));
		ConditionalAssign(dst, src);
	}

	void ArmRewriter::RewriteUmlal()
	{
		auto dst = frame.EnsureSequence(
			(int)Src1().RegisterValue,
			(int)Dst().RegisterValue,
			PrimitiveType::Word64);
		auto left = this->Operand(Src2());
		auto right = this->Operand(Src3());
		ConditionalSkip();
		m.Assign(dst, m.IAdd(m.UMul(left, right), dst));
		MaybeUpdateFlags(dst);
	}

	void ArmRewriter::RewriteXtab(PrimitiveType dt)
	{
		auto dst = this->Operand(Dst());
		auto src = frame.EnsureRegister((int)Src2().RegisterValue);
		if (Src2().Shifter.Type == ArmShifterType::ROR)
		{
			src = m.Shr(src, Src2().Shifter.Value);
		}
		src = m.Cast(dt, src);
		ConditionalSkip();
		m.Assign(dst, m.IAdd(this->Operand(Src1()), src));
	}

	void ArmRewriter::RewriteXtb(PrimitiveType dt)
	{
		auto dst = this->Operand(Dst());
		auto src = frame.EnsureRegister((int)Src1().RegisterValue);
		if (Src1().Shifter.Type == ArmShifterType::ROR)
		{
			src = m.Shr(src, Src1().Shifter.Value);
		}
		src = m.Cast(dt, src);
		ConditionalSkip();
		m.Assign(dst, src);
	}
