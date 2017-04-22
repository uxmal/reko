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

void ArmRewriter::RewriteAdcSbc(IExpression * (IRewriter::*opr)(IExpression *, IExpression *))
{
		ConditionalSkip();
		auto opDst = this->Operand(Dst()());
		auto opSrc1 = this->Operand(Src1());
		auto opSrc2 = this->Operand(Src2());
		// We do not take the trouble of widening the CF to the word size
		// to simplify code analysis in later stages. 
		auto c = frame.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
		ConditionalAssign(
			opDst,
			opr(
				opr(opSrc1, opSrc2),
				c));
		MaybeUpdateFlags(opDst);
	}

	void ArmRewriter::RewriteBfc()
	{
		auto opDst = this->Operand(Dst());
		auto lsb = instr.ArchitectureDetail.Operands[1].ImmediateValue.Value;
		auto bitsize = instr.ArchitectureDetail.Operands[2].ImmediateValue.Value;
		ConditionalSkip();
		m.Assign(opDst, m.And(opDst, (uint)~Bits.Mask(lsb, bitsize)));
	}

	void ArmRewriter::RewriteBfi()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1);
		auto tmp = frame.CreateTemporary(opDst.DataType);
		auto lsb = instr.ArchitectureDetail.Operands[2].ImmediateValue.Value;
		auto bitsize = instr.ArchitectureDetail.Operands[3].ImmediateValue.Value;
		ConditionalSkip();
		m.Assign(tmp, m.Slice(opSrc, 0, bitsize));
		m.Assign(opDst, m.Dpb(opDst, tmp, lsb));
	}

	void ArmRewriter::RewriteBinOp(Func<Expression,Expression,Expression> op, bool setflags)
	{
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1);
		auto opSrc2 = this->Operand(Src2);
		ConditionalAssign(opDst, op(opSrc1, opSrc2));
		if (setflags)
		{
			ConditionalAssign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte), m.Cond(opDst));
		}
	}

	void ArmRewriter::RewriteRev()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1);
		ConditionalAssign(
			opDst,
			host.PseudoProcedure("__rev", PrimitiveType.Word32, opSrc));
	}

	void ArmRewriter::RewriteRevBinOp(Operator op, bool setflags)
	{
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1);
		auto opSrc2 = this->Operand(Src2);
		ConditionalAssign(opDst, new BinaryExpression(op, PrimitiveType.Word32, opSrc1, opSrc2));
		if (setflags)
		{
			ConditionalAssign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte), m.Cond(opDst));
		}
	}

	void ArmRewriter::RewriteUnaryOp(UnaryOperator op)
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1);
		ConditionalAssign(opDst, new UnaryExpression(op,  PrimitiveType.Word32, opSrc));
		if (instr.ArchitectureDetail.UpdateFlags)
		{
			ConditionalAssign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "SZCO", PrimitiveType.Byte), m.Cond(opDst));
		}
	}

	void ArmRewriter::RewriteBic()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1);
		auto opSrc2 = this->Operand(Src2);
		ConditionalAssign(opDst, m.And(opSrc1, m.Comp(opSrc2)));
	}

	void ArmRewriter::RewriteClz()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1);

		ConditionalAssign(
			opDst,
			host.PseudoProcedure("__clz", PrimitiveType.Int32, opSrc));
	}

	void ArmRewriter::RewriteCmn()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1);
		ConditionalAssign(
			frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte),
			m.Cond(
				m.IAdd(opDst, opSrc)));
	}

	void ArmRewriter::RewriteCmp()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1);
		ConditionalAssign(
			frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte),
			m.Cond(
				m.ISub(opDst, opSrc)));
	}

	void ArmRewriter::RewriteTeq()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1);
		m.Assign(
			frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte),
			m.Cond(m.Xor(opDst, opSrc)));
	}

	void ArmRewriter::RewriteTst()
	{
		auto opDst = this->Operand(Dst());
		auto opSrc = this->Operand(Src1);
		m.Assign(
			frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte),
			m.Cond(m.And(opDst, opSrc)));
	}

	void ArmRewriter::RewriteLdr(PrimitiveType size)
	{
		auto opSrc = this->Operand(Src1);
		auto opDst = this->Operand(Dst());
		Identifier dst = (Identifier)opDst;
		auto rDst = dst.Storage as RegisterStorage;
		if (rDst == A32Registers.pc)
		{
			// Assignment to PC is the same as a jump
			ric.Class = RtlClass.Transfer;
			m.Goto(opSrc);
			return;
		}
		m.Assign(opDst, opSrc);
		MaybePostOperand(Src1);
	}

	void ArmRewriter::RewriteLdrd()
	{
		auto ops = instr.ArchitectureDetail.Operands;
		auto regLo = A32Registers.RegisterByCapstoneID[ops[0].RegisterValue.Value];
		auto regHi = A32Registers.RegisterByCapstoneID[ops[1].RegisterValue.Value];
		auto opDst = frame.EnsureSequence(regHi, regLo, PrimitiveType.Word64);
		auto opSrc = this->Operand(ops[2]);
		m.Assign(opDst, opSrc);
		MaybePostOperand(ops[2]);
	}

	void ArmRewriter::RewriteStr(PrimitiveType size)
	{
		auto opSrc = this->Operand(Dst());
		auto opDst = this->Operand(Src1);
		m.Assign(opDst, opSrc);
		MaybePostOperand(Src1);
	}

	void ArmRewriter::RewriteStrd()
	{
		auto ops = instr.ArchitectureDetail.Operands;
		auto regLo = A32Registers.RegisterByCapstoneID[ops[0].RegisterValue.Value];
		auto regHi = A32Registers.RegisterByCapstoneID[ops[1].RegisterValue.Value];
		auto opSrc = frame.EnsureSequence(regHi, regLo, PrimitiveType.Word64);
		auto opDst = this->Operand(ops[2]);
		m.Assign(opDst, opSrc);
		MaybePostOperand(ops[2]);
	}

	private void RewriteMultiplyAccumulate(Func<Expression,Expression,Expression> op)
	{
		auto opDst = this->Operand(Dst());
		auto opSrc1 = this->Operand(Src1);
		auto opSrc2 = this->Operand(Src2);
		auto opSrc3 = this->Operand(Src3);
		ConditionalAssign(opDst, op(opSrc3, m.IMul(opSrc1, opSrc2)));
		if (instr.ArchitectureDetail.UpdateFlags)
		{
			ConditionalAssign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte), m.Cond(opDst));
		}
	}

	private void RewriteMov()
	{
		if (Dst().Type == ArmInstructionOperandType.Register && Dst().RegisterValue.Value == ArmRegister.PC)
		{
			ric.Class = RtlClass.Transfer;
			if (Src1.Type == ArmInstructionOperandType.Register && Src1.RegisterValue.Value == ArmRegister.LR)
			{
				AddConditional(new RtlReturn(0, 0, RtlClass.Transfer));
			}
			else
			{
				AddConditional(new RtlGoto(Operand(Src1), RtlClass.Transfer));
			}
			return;
		}
		auto opDst = Operand(Dst());
		auto opSrc = Operand(Src1);
		ConditionalAssign(opDst, opSrc);
	}

	private void RewriteMovt()
	{
		auto opDst = Operand(Dst());
		auto iSrc = ((Constant)Operand(Src1)).ToUInt32();
		auto opSrc = m.Dpb(opDst, Constant.Word16((ushort)iSrc), 16);
		ConditionalAssign(opDst, opSrc);
	}

	private void RewriteLdm(int initialOffset)
	{
		auto dst = this->Operand(Dst());
		auto range = instr.ArchitectureDetail.Operands.Skip(1);
		RewriteLdm(dst, range, initialOffset, instr.ArchitectureDetail.WriteBack);
	}

	private void RewriteLdm(Expression dst, IEnumerable<ArmInstructionOperand> range, int offset, bool writeback)
	{
		ConditionalSkip();
		bool pcRestored = false;
		foreach(auto r in range)
		{
			Expression ea = offset != 0
				? m.IAdd(dst, Constant.Int32(offset))
				: dst;
			if (r.RegisterValue.Value == ArmRegister.PC)
			{
				pcRestored = true;
			}
			else
			{
				auto dstReg = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[r.RegisterValue.Value]);
				m.Assign(dstReg, m.LoadDw(ea));
			}
			offset += 4;
		}
		if (writeback)
		{
			m.Assign(dst, m.IAdd(dst, Constant.Int32(offset)));
		}
		if (pcRestored)
		{
			m.Return(0, 0);
		}
	}

	private void RewriteMulbb(bool hiLeft, bool hiRight, DataType dtMultiplicand, Func<Expression,Expression,Expression> mul)
	{
		if (hiLeft || hiRight)
		{
			NotImplementedYet();
			return;
		}
		auto opDst = this->Operand(Dst());
		auto opLeft = m.Cast(dtMultiplicand, this->Operand(Src1));
		auto opRight = m.Cast(dtMultiplicand, this->Operand(Src2));
		m.Assign(opDst, mul(opLeft, opRight));
	}

	private void RewriteMull(PrimitiveType dtResult, Func<Expression, Expression, Expression> op)
	{
		auto ops = instr.ArchitectureDetail.Operands;
		auto regLo = A32Registers.RegisterByCapstoneID[ops[0].RegisterValue.Value];
		auto regHi = A32Registers.RegisterByCapstoneID[ops[1].RegisterValue.Value];

		auto opDst = frame.EnsureSequence(regHi, regLo, dtResult);
		auto opSrc1 = this->Operand(Src3);
		auto opSrc2 = this->Operand(Src2);
		ConditionalAssign(opDst, op(opSrc1, opSrc2));
		if (instr.ArchitectureDetail.UpdateFlags)
		{
			ConditionalAssign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte), m.Cond(opDst));
		}
	}

	private void RewritePop()
	{
		auto sp = frame.EnsureRegister(A32Registers.sp);
		RewriteLdm(sp, instr.ArchitectureDetail.Operands, 0, true);
	}

	private void RewritePush()
	{
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
	}

	private void RewriteSbfx()
	{
		auto dst = this->Operand(Dst());
		auto src = m.Cast(
			PrimitiveType.Int32,
			m.Slice(
				this->Operand(Src1),
				Src2.ImmediateValue.Value,
				Src3.ImmediateValue.Value));
		ConditionalAssign(dst, src);
	}

	private void RewriteStm()
	{
		auto dst = this->Operand(Dst());
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
	}

	private void RewriteStmib()
	{
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
		auto range = (RegisterRangeOperand)Src1;
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
	}

	private void RewriteUbfx()
	{
		auto dst = this->Operand(Dst());
		auto src = m.Cast(
			PrimitiveType.UInt32,
			m.Slice(
				this->Operand(Src1),
				Src2.ImmediateValue.Value,
				Src3.ImmediateValue.Value));
		ConditionalAssign(dst, src);
	}

	private void RewriteUmlal()
	{
		auto dst = frame.EnsureSequence(
			A32Registers.RegisterByCapstoneID[Src1.RegisterValue.Value],
			A32Registers.RegisterByCapstoneID[Dst().RegisterValue.Value],
			PrimitiveType.Word64);
		auto left = this->Operand(Src2);
		auto right = this->Operand(Src3);
		ConditionalSkip();
		m.Assign(dst, m.IAdd(m.UMul(left, right), dst));
		MaybeUpdateFlags(dst);
	}

	private void RewriteXtab(DataType dt)
	{
		auto dst = this->Operand(Dst());
		Expression src = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[Src2.RegisterValue.Value]);
		if (Src2.Shifter.Type == ArmShifterType.ROR)
		{
			src = m.Shr(src, Src2.Shifter.Value);
		}
		src = m.Cast(dt, src);
		ConditionalSkip();
		m.Assign(dst, m.IAdd(this->Operand(Src1), src));
	}

	private void RewriteXtb(DataType dt)
	{
		auto dst = this->Operand(Dst());
		Expression src = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[Src1.RegisterValue.Value]);
		if (Src1.Shifter.Type == ArmShifterType.ROR)
		{
			src = m.Shr(src, Src1.Shifter.Value);
		}
		src = m.Cast(dt, src);
		ConditionalSkip();
		m.Assign(dst, src);
	}
}
}