#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
#endregion

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.X86
{
    public partial class X86Rewriter
    {
        private int maxFpuStackWrite;

        public void EmitCommonFpuInstruction(
            Func<Expression,Expression,Expression> op,
            bool fReversed,
            bool fPopStack)
        {
            EmitCommonFpuInstruction(op, fReversed, fPopStack, null);
        }

        public void EmitCommonFpuInstruction(
            Func<Expression,Expression,Expression> op,
            bool fReversed,
            bool fPopStack,
            DataType cast)
        {
            switch (instrCur.Operands)
            {
            default:
                throw new ArgumentOutOfRangeException("di.Instruction", "Instruction must have 1 or 2 operands");
            case 1:
                {
                    // implicit st(0) operand.
                    Identifier opLeft = FpuRegister(0);
                    Expression opRight = MaybeCast(cast, SrcOp(instrCur.op1));
                    m.Assign(
                        opLeft,
                        op(
                            fReversed ? opRight : opLeft,
                            fReversed ? opLeft : opRight));
                    break;
                }
            case 2:
                {
                    Expression op1 = SrcOp(instrCur.op1);
                    Expression op2 = SrcOp(instrCur.op2);
                    m.Assign(
                        SrcOp(instrCur.op1),
                        op(
                            fReversed ? op2 : op1,
                            fReversed ? op1 : op2));
                    break;
                }
            }

            if (fPopStack)
            {
                state.ShrinkFpuStack(1);
            }
        }

        private void RewriteF2xm1()
        {
            m.Assign(
                FpuRegister(0),
                m.FSub(
                    host.PseudoProcedure(
                        "pow",
                        PrimitiveType.Real64, 
                        Constant.Real64(2.0),
                        FpuRegister(0)),
                    Constant.Real64(1.0)));
        }

        private void RewriteFabs()
        {
            m.Assign(FpuRegister(0), host.PseudoProcedure("fabs", PrimitiveType.Real64, FpuRegister(0)));
        }

        private void RewriteFbld()
        {
            state.GrowFpuStack(instrCur.Address);
            m.Assign(FpuRegister(0),
                host.PseudoProcedure("__fbld", PrimitiveType.Real64, SrcOp(instrCur.op1)));
            WriteFpuStack(0);
        }

        private void RewriteFbstp()
        {
            instrCur.op1.Width = PrimitiveType.Bcd80;
            m.Assign(SrcOp(instrCur.op1), m.Cast(instrCur.op1.Width, orw.FpuRegister(0, state)));
            state.ShrinkFpuStack(1);
        }

        private void EmitFchs()
        {
            m.Assign(
                orw.FpuRegister(0, state),
                m.Neg(orw.FpuRegister(0, state)));		//$BUGBUG: should be Real, since we don't know the actual size.
            WriteFpuStack(0);
        }

        private void RewriteFclex()
        {
            m.SideEffect(host.PseudoProcedure("__fclex", VoidType.Instance));
        }

        private void RewriteFcmov(FlagM flag, ConditionCode cc)
        {
            m.BranchInMiddleOfInstruction(
                m.Test(cc, orw.FlagGroup(flag)),
                instrCur.Address + instrCur.Length,
                InstrClass.ConditionalTransfer);

            var dst = SrcOp(instrCur.op1);
            var src = SrcOp(instrCur.op2);
            m.Assign(dst, src);
        }

        private void RewriteFcom(int pops)
        {
            Identifier op1 = FpuRegister(0);
            Expression op2 = (instrCur.code == Opcode.fcompp || instrCur.code == Opcode.fucompp)
                ? FpuRegister(1)
                : SrcOp(instrCur.op1);
            m.Assign(
                orw.FlagGroup(FlagM.FPUF),
                m.Cond(
                    m.FSub(op1, op2)));
            state.ShrinkFpuStack(pops);
        }

        private void RewriteFdecstp()
        {
            state.ShrinkFpuStack(1);
            m.Nop();
        }

        private void RewriteFfree()
        {
            m.SideEffect(
                host.PseudoProcedure("__ffree", VoidType.Instance, SrcOp(instrCur.op1)));
        }

        private void RewriteFUnary(string name)
        {
            m.Assign(
                orw.FpuRegister(0, state),
                host.PseudoProcedure(name, PrimitiveType.Real64, orw.FpuRegister(0, state)));
            WriteFpuStack(0);
        }

        private void RewriteFicom(bool pop)
        {
            m.Assign(
                orw.FlagGroup(FlagM.FPUF),
                m.Cond(
                    m.FSub(
                        orw.FpuRegister(0, state),
                        m.Cast(PrimitiveType.Real64,
                            SrcOp(instrCur.op1)))));
            if (pop)
                state.ShrinkFpuStack(1);
        }

        private void RewriteFild()
        {
            state.GrowFpuStack(instrCur.Address);
            var iType = PrimitiveType.Create(Domain.SignedInt, instrCur.op1.Width.BitSize);
            m.Assign(
                orw.FpuRegister(0, state),
                m.Cast(PrimitiveType.Real64, SrcOp(instrCur.op1, iType)));
            WriteFpuStack(0);
        }

        private void RewriteFincstp()
        {
            state.GrowFpuStack(instrCur.Address);
            m.Nop();
        }

        private void RewriteFist(bool pop)
        {
            instrCur.op1.Width = PrimitiveType.Create(Domain.SignedInt, instrCur.op1.Width.BitSize);
            m.Assign(SrcOp(instrCur.op1), m.Cast(instrCur.op1.Width, orw.FpuRegister(0, state)));
            if (pop)
                state.ShrinkFpuStack(1);
        }

        private void RewriteFistt(bool pop)
        {
            instrCur.op1.Width = PrimitiveType.Create(Domain.SignedInt, instrCur.op1.Width.BitSize);
            var fpuReg = orw.FpuRegister(0, state);
            var trunc = host.PseudoProcedure("trunc", fpuReg.DataType, fpuReg);
            m.Assign(SrcOp(instrCur.op1), m.Cast(instrCur.op1.Width, trunc));
            if (pop)
                state.ShrinkFpuStack(1);
        }

        public void RewriteFld()
        {
            state.GrowFpuStack(instrCur.Address);
            var dst = FpuRegister(0);
            var src = SrcOp(instrCur.op1);
            if (src.DataType.Size != dst.DataType.Size)
            {
                src = m.Cast(
                    PrimitiveType.Create(Domain.Real, dst.DataType.BitSize),
                    src);
            }
            m.Assign(dst, src);
            WriteFpuStack(0);
        }

        private void RewriteFldConst(double constant)
        {
            RewriteFldConst(Constant.Real64(constant));
        }

        private void RewriteFldConst(Constant c)
        {
            state.GrowFpuStack(instrCur.Address);
            m.Assign(FpuRegister(0), c);
            WriteFpuStack(0);
        }

        private void RewriteFldcw()
        {
            m.SideEffect(host.PseudoProcedure(
                "__fldcw",
                VoidType.Instance,
                SrcOp(instrCur.op1)));
        }

        private void RewriteFldenv()
        {
            m.SideEffect(host.PseudoProcedure(
                "__fldenv",
                VoidType.Instance,
                SrcOp(instrCur.op1)));
        }

        private void RewriteFstenv()
        {
            m.SideEffect(host.PseudoProcedure(
                "__fstenv",
                VoidType.Instance,
                SrcOp(instrCur.op1)));
        }

        private void RewriteFpatan()
        {
            Expression op1 = FpuRegister(1);
            Expression op2 = FpuRegister(0);
            state.ShrinkFpuStack(1);
            m.Assign(FpuRegister(0), host.PseudoProcedure("atan", PrimitiveType.Real64, op1, op2));
            WriteFpuStack(0);
        }

        private void RewriteFprem()
        {
            Expression op1 = FpuRegister(1);
            Expression op2 = FpuRegister(0);
            state.ShrinkFpuStack(1);
            m.Assign(FpuRegister(0),
                m.Mod(op2, op1));
            WriteFpuStack(0);
        }

        private void RewriteFprem1()
        {
            Expression op1 = SrcOp(instrCur.op1);
            Expression op2 = SrcOp(instrCur.op2);
            m.Assign(op1, host.PseudoProcedure("__fprem1", op1.DataType, op1, op2));
            WriteFpuStack(0);
        }

        private void RewriteFptan()
        {
            Expression op1 = FpuRegister(0);
            m.Assign(FpuRegister(0), host.PseudoProcedure("tan", PrimitiveType.Real64, op1));
            state.GrowFpuStack(instrCur.Address);
            m.Assign(FpuRegister(0), Constant.Real64(1.0));
        }

        private void RewriteFsincos()
        {
            Identifier itmp = binder.CreateTemporary(PrimitiveType.Real64);
            m.Assign(itmp, FpuRegister(0));

            state.GrowFpuStack(instrCur.Address);
            m.Assign(FpuRegister(1), host.PseudoProcedure("cos", PrimitiveType.Real64, itmp));
            m.Assign(FpuRegister(0), host.PseudoProcedure("sin", PrimitiveType.Real64, itmp));
            WriteFpuStack(0);
            WriteFpuStack(1);
        }

        private void RewriteFst(bool pop)
        {
            Expression src = FpuRegister(0);
            Expression dst = SrcOp(instrCur.op1);
            if (src.DataType.Size != dst.DataType.Size)
            {
                src = m.Cast(
                    PrimitiveType.Create(Domain.Real, dst.DataType.BitSize),
                    src);
            }
            m.Assign(dst, src);
            if (pop)
                state.ShrinkFpuStack(1);
        }

        private void RewriterFstcw()
        {
			m.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure("__fstcw", PrimitiveType.UInt16));
        }

        private void RewriteFrstor()
        {
            m.SideEffect(
                host.PseudoProcedure(
                    "__frstor",
                    VoidType.Instance,
                    SrcOp(instrCur.op1)));
        }

        private void RewriteFsave()
        {
            m.SideEffect(
                host.PseudoProcedure(
                    "__fsave", 
                    VoidType.Instance, 
                    SrcOp(instrCur.op1)));
        }

        private void RewriteFscale()
        {
            m.Assign(
                FpuRegister(0),
                host.PseudoProcedure("scalbn", PrimitiveType.Real64, FpuRegister(0), FpuRegister(1)));
        }

        // 8087 status register bits:
        // bit 8: C0
        // bit 9: C1
        // bit 10: C2
        // bit 14: C3
        // 8086 flag register bits:
        // bit 0: CF
        // bit 1: RESERVED
        // bit 2: PF
        // bit 6: ZF

        private void RewriteFstsw()
        {
            if (MatchesFstswSequence())
                return;
            m.Assign(
                SrcOp(instrCur.op1),
                new BinaryExpression(Operator.Shl, PrimitiveType.Word16,
                        new Cast(PrimitiveType.Word16, orw.FlagGroup(FlagM.FPUF)),
                        Constant.Int16(8)));
        }

        public bool MatchesFstswSequence()
        {
            var nextInstr = dasm.Peek(1);
            if (nextInstr.code == Opcode.sahf)
            {
                this.len += nextInstr.Length;
                dasm.Skip(1);
                m.Assign(
                    orw.FlagGroup(FlagM.ZF | FlagM.CF | FlagM.SF | FlagM.OF),
                    orw.FlagGroup(FlagM.FPUF));
                return true;
            }
            if (nextInstr.code == Opcode.test)
            {
                RegisterOperand acc = nextInstr.op1 as RegisterOperand;
                ImmediateOperand imm = nextInstr.op2 as ImmediateOperand;
                if (imm == null || acc == null)
                    return false;
                int mask = imm.Value.ToInt32();
                if (acc.Register == Registers.ax || acc.Register == Registers.eax)
                    mask >>= 8;
                else if (acc.Register != Registers.ah)
                    return false;
                this.len += nextInstr.Length;
                m.Assign(
                    orw.FlagGroup(FlagM.ZF | FlagM.CF | FlagM.SF | FlagM.OF),
                    orw.FlagGroup(FlagM.FPUF));

                // Advance past the 'test' instruction.
                dasm.Skip(1);
                while (dasm.MoveNext())
                {
                    instrCur = dasm.Current;
                    this.len += instrCur.Length;

                    /* fcom/fcomp/fcompp Results:
                        Condition      C3  C2  C0
                        ST(0) > SRC     0   0   0
                        ST(0) < SRC     0   0   1
                        ST(0) = SRC     1   0   0
                        Unordered       1   1   1

                       Masks:
                        Mask   Flags
                        0x01   C0
                        0x04   C2
                        0x40   C3
                        0x05   C2 and C0
                        0x41   C3 and C0
                        0x44   C3 and C2

                      Masks && jump operations:
                        Opcode Mask Condition
                        jpe    0x05    >=
                        jpe    0x41    >
                        jpe    0x44    !=
                        jpo    0x05    <
                        jpo    0x41    <=
                        jpo    0x44    =
                        jz     0x01    >=
                        jz     0x40    !=
                        jz     0x41    >
                        jnz    0x01    <
                        jnz    0x40    =
                        jnz    0x41    <=
                    */

                    switch (instrCur.code)
                    {
                    //$TODO The following instructions are being added on an ad-hoc
                    // basis, since they don't affect the x86 flags register.
                    // The long term fix is to implement an architecture-specific
                    // condition code elimination pass as described elsewhere.
                    case Opcode.mov: RewriteMov(); break;
                    case Opcode.fstp: RewriteFst(true); break;
                    case Opcode.push: RewritePush(); break;
                    case Opcode.lea: RewriteLea(); break;

                    case Opcode.jpe:
                        if (mask == 0x05) { Branch(ConditionCode.GE, instrCur.op1); return true; }
                        if (mask == 0x41) { Branch(ConditionCode.GT, instrCur.op1); return true; }
                        if (mask == 0x44) { Branch(ConditionCode.NE, instrCur.op1); return true; }
                        throw new AddressCorrelatedException(instrCur.Address, "Unexpected {0} fstsw mask for {1} opcode .", mask, instrCur.code);
                    case Opcode.jpo:
                        if (mask == 0x44) { Branch(ConditionCode.EQ, instrCur.op1); return true; }
                        if (mask == 0x41) { Branch(ConditionCode.LE, instrCur.op1); return true; }
                        if (mask == 0x05) { Branch(ConditionCode.LT, instrCur.op1); return true; }
                        throw new AddressCorrelatedException(instrCur.Address, "Unexpected {0} fstsw mask for {1} opcode .", mask, instrCur.code);
                    case Opcode.jz:
                        if (mask == 0x40) { Branch(ConditionCode.NE, instrCur.op1); return true; }
                        if (mask == 0x41) { Branch(ConditionCode.GT, instrCur.op1); return true; }
                        if (mask == 0x01) { Branch(ConditionCode.GE, instrCur.op1); return true; }
                        throw new AddressCorrelatedException(instrCur.Address, "Unexpected {0} fstsw mask for {1} opcode .", mask, instrCur.code);
                    case Opcode.jnz:
                        if (mask == 0x40) { Branch(ConditionCode.EQ, instrCur.op1); return true; }
                        if (mask == 0x41) { Branch(ConditionCode.LE, instrCur.op1); return true; }
                        if (mask == 0x01) { Branch(ConditionCode.LT, instrCur.op1); return true; }
                        throw new AddressCorrelatedException(instrCur.Address, "Unexpected {0} fstsw mask for {1} opcode .", mask, instrCur.code);
                    default:
                        throw new AddressCorrelatedException(instrCur.Address, "Unexpected instruction {0} after fstsw", instrCur);
                    }
                }
                throw new AddressCorrelatedException(instrCur.Address, "Expected branch instruction after fstsw;test {0},{1}.", acc.Register, imm.Value);
            }
            return false;
        }

        private void Branch(ConditionCode code, MachineOperand op)
        {
            this.rtlc = InstrClass.ConditionalTransfer;
            m.Branch(m.Test(code, orw.FlagGroup(FlagM.FPUF)), OperandAsCodeAddress( op), InstrClass.ConditionalTransfer);
        }

        private void RewriteFtst()
        {
            m.Assign(orw.FlagGroup(FlagM.CF),
                m.ISub(FpuRegister(0), Constant.Real64(0.0)));
        }

        private void RewrteFcomi(bool pop)
        {
            var op1 = SrcOp(instrCur.op1);
            var op2 = SrcOp(instrCur.op2);
            m.Assign(
                orw.FlagGroup(FlagM.ZF|FlagM.PF|FlagM.CF),
                m.Cond(
                    m.FSub(op1, op2)));
            m.Assign(orw.FlagGroup(FlagM.OF), Constant.False());
            m.Assign(orw.FlagGroup(FlagM.SF), Constant.False());
            if (pop)
            {
                state.ShrinkFpuStack(1);
            }
        }

        private void RewriteFxam()
        {
            m.Assign(orw.FlagGroup(FlagM.FPUF), m.Cond(FpuRegister(0)));
        }

        private void RewriteFxtract()
        {
            var fp = this.FpuRegister(0);
            var tmp = binder.CreateTemporary(fp.DataType);
            m.Assign(tmp, fp);
            state.GrowFpuStack(instrCur.Address);
            m.Assign(this.FpuRegister(1), host.PseudoProcedure("__exponent", fp.DataType, tmp));
            m.Assign(this.FpuRegister(0), host.PseudoProcedure("__significand", fp.DataType, tmp));
        }

        private void RewriteFyl2x()
        {
            //$REVIEW: Candidate for idiom search.
            Identifier op1 = FpuRegister(0);
            Identifier op2 = FpuRegister(1);
            m.Assign(op2, 
                m.FMul(op2, 
                      host.PseudoProcedure("lg2", PrimitiveType.Real64, op2)));
            state.ShrinkFpuStack(1);
            WriteFpuStack(0);
        }

        private void RewriteFyl2xp1()
        {
            //$REVIEW: Candidate for idiom search.
            Identifier op1 = FpuRegister(0);
            Identifier op2 = FpuRegister(1);
            m.Assign(op2,
                m.FMul(
                    op2,
                    host.PseudoProcedure(
                        "lg2",
                        PrimitiveType.Real64,
                        m.FAdd(op1, Constant.Real64(1.0)))));
            m.Assign(
                orw.FlagGroup(FlagM.FPUF),
                m.Cond(op2));
            state.ShrinkFpuStack(1);
            WriteFpuStack(0);
        }

        private void RewriteWait()
        {
            m.SideEffect(host.PseudoProcedure("__wait", VoidType.Instance));
        }

        private Identifier FpuRegister(int reg)
        {
            return orw.FpuRegister(reg, state);
        }


        public Expression MaybeCast(DataType type, Expression e)
        {
            if (type != null)
                return new Cast(type, e);
            else
                return e;
        }

        private void WriteFpuStack(int offset)
        {
            int o = offset - state.FpuStackItems;
            if (o > maxFpuStackWrite)
                maxFpuStackWrite = o;
        }
    }
}
