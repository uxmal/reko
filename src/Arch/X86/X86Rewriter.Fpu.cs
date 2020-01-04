#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
            switch (instrCur.Operands.Length)
            {
            default:
                throw new ArgumentOutOfRangeException("di.Instruction", "Instruction must have 1 or 2 operands");
            case 1:
                {
                    // implicit st(0) operand.
                    var opLeft = FpuRegister(0);
                    var opRight = MaybeCast(cast, SrcOp(instrCur.Operands[0]));
                    m.Assign(
                        opLeft,
                        op(
                            fReversed ? opRight : opLeft,
                            fReversed ? opLeft : opRight));
                    break;
                }
            case 2:
                {
                    Expression op1 = SrcOp(instrCur.Operands[0]);
                    Expression op2 = SrcOp(instrCur.Operands[1]);
                    m.Assign(
                        SrcOp(instrCur.Operands[0]),
                        op(
                            fReversed ? op2 : op1,
                            fReversed ? op1 : op2));
                    break;
                }
            }

            if (fPopStack)
            {
                ShrinkFpuStack(1);
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
            GrowFpuStack(1);
            m.Assign(FpuRegister(0),
                host.PseudoProcedure("__fbld", PrimitiveType.Real64, SrcOp(instrCur.Operands[0])));
        }

        private void RewriteFbstp()
        {
            instrCur.Operands[0].Width = PrimitiveType.Bcd80;
            m.Assign(SrcOp(instrCur.Operands[0]), m.Cast(instrCur.Operands[0].Width, orw.FpuRegister(0, state)));
            ShrinkFpuStack(1);
        }

        private void EmitFchs()
        {
            m.Assign(
                orw.FpuRegister(0, state),
                m.Neg(orw.FpuRegister(0, state)));		//$BUGBUG: should be Real, since we don't know the actual size.
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

            var dst = SrcOp(instrCur.Operands[0]);
            var src = SrcOp(instrCur.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteFcom(int pops)
        {
            var op1 = FpuRegister(0);
            var op2 = (instrCur.code == Mnemonic.fcompp || instrCur.code == Mnemonic.fucompp)
                ? FpuRegister(1)
                : SrcOp(instrCur.Operands[0]);
            m.Assign(
                binder.EnsureRegister(Registers.FPUF),
                m.Cond(
                    m.FSub(op1, op2)));
            ShrinkFpuStack(pops);
        }

        private void RewriteFdecstp()
        {
            ShrinkFpuStack(1);
            m.Nop();
        }

        private void RewriteFfree()
        {
            m.SideEffect(
                host.PseudoProcedure("__ffree", VoidType.Instance, SrcOp(instrCur.Operands[0])));
        }

        private void RewriteFUnary(string name)
        {
            m.Assign(
                orw.FpuRegister(0, state),
                host.PseudoProcedure(name, PrimitiveType.Real64, orw.FpuRegister(0, state)));
        }

        private void RewriteFicom(bool pop)
        {
            m.Assign(
                orw.AluRegister(Registers.FPUF),
                m.Cond(
                    m.FSub(
                        orw.FpuRegister(0, state),
                        m.Cast(PrimitiveType.Real64,
                            SrcOp(instrCur.Operands[0])))));
            if (pop)
                ShrinkFpuStack(1);
        }

        private void RewriteFild()
        {
            GrowFpuStack(1);
            var iType = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            m.Assign(
                orw.FpuRegister(0, state),
                m.Cast(PrimitiveType.Real64, SrcOp(instrCur.Operands[0], iType)));
        }

        private void RewriteFincstp()
        {
            GrowFpuStack(1);
            m.Nop();
        }

        private void RewriteFist(bool pop)
        {
            instrCur.Operands[0].Width = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            m.Assign(SrcOp(instrCur.Operands[0]), m.Cast(instrCur.Operands[0].Width, orw.FpuRegister(0, state)));
            if (pop)
                ShrinkFpuStack(1);
        }

        private void RewriteFistt(bool pop)
        {
            instrCur.Operands[0].Width = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            var fpuReg = orw.FpuRegister(0, state);
            var trunc = host.PseudoProcedure("trunc", fpuReg.DataType, fpuReg);
            m.Assign(SrcOp(instrCur.Operands[0]), m.Cast(instrCur.Operands[0].Width, trunc));
            if (pop)
                ShrinkFpuStack(1);
        }

        public void RewriteFld()
        {
            GrowFpuStack(1);
            var dst = FpuRegister(0);
            var src = SrcOp(instrCur.Operands[0]);
            if (src.DataType.Size != dst.DataType.Size)
            {
                src = m.Cast(
                    PrimitiveType.Create(Domain.Real, dst.DataType.BitSize),
                    src);
            }
            m.Assign(dst, src);
        }

        private void RewriteFldConst(double constant)
        {
            RewriteFldConst(Constant.Real64(constant));
        }

        private void RewriteFldConst(Constant c)
        {
            GrowFpuStack(1);
            m.Assign(FpuRegister(0), c);
        }

        private void RewriteFldcw()
        {
            m.SideEffect(host.PseudoProcedure(
                "__fldcw",
                VoidType.Instance,
                SrcOp(instrCur.Operands[0])));
        }

        private void RewriteFldenv()
        {
            m.SideEffect(host.PseudoProcedure(
                "__fldenv",
                VoidType.Instance,
                SrcOp(instrCur.Operands[0])));
        }

        private void RewriteFstenv()
        {
            m.SideEffect(host.PseudoProcedure(
                "__fstenv",
                VoidType.Instance,
                SrcOp(instrCur.Operands[0])));
        }

        private void RewriteFpatan()
        {
            Expression op1 = FpuRegister(1);
            Expression op2 = FpuRegister(0);
            ShrinkFpuStack(1);
            m.Assign(FpuRegister(0), host.PseudoProcedure("atan", PrimitiveType.Real64, op1, op2));
        }

        private void RewriteFprem()
        {
            Expression op1 = FpuRegister(1);
            Expression op2 = FpuRegister(0);
            ShrinkFpuStack(1);
            m.Assign(FpuRegister(0),
                m.Mod(op2, op1));
        }

        private void RewriteFprem1()
        {
            Expression op1 = SrcOp(instrCur.Operands[0]);
            Expression op2 = SrcOp(instrCur.Operands[1]);
            m.Assign(op1, host.PseudoProcedure("__fprem1", op1.DataType, op1, op2));
        }

        private void RewriteFptan()
        {
            Expression op1 = FpuRegister(0);
            m.Assign(FpuRegister(0), host.PseudoProcedure("tan", PrimitiveType.Real64, op1));
            GrowFpuStack(1);
            m.Assign(FpuRegister(0), Constant.Real64(1.0));
        }

        private void RewriteFsincos()
        {
            Identifier itmp = binder.CreateTemporary(PrimitiveType.Real64);
            m.Assign(itmp, FpuRegister(0));

            GrowFpuStack(1);
            m.Assign(FpuRegister(1), host.PseudoProcedure("cos", PrimitiveType.Real64, itmp));
            m.Assign(FpuRegister(0), host.PseudoProcedure("sin", PrimitiveType.Real64, itmp));
        }

        private void RewriteFst(bool pop)
        {
            Expression src = FpuRegister(0);
            Expression dst = SrcOp(instrCur.Operands[0]);
            if (src.DataType.Size != dst.DataType.Size)
            {
                src = m.Cast(
                    PrimitiveType.Create(Domain.Real, dst.DataType.BitSize),
                    src);
            }
            m.Assign(dst, src);
            if (pop)
                ShrinkFpuStack(1);
        }

        private void RewriterFstcw()
        {
			m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure("__fstcw", PrimitiveType.UInt16));
        }

        private void RewriteFrstor()
        {
            m.SideEffect(
                host.PseudoProcedure(
                    "__frstor",
                    VoidType.Instance,
                    SrcOp(instrCur.Operands[0])));
        }

        private void RewriteFsave()
        {
            m.SideEffect(
                host.PseudoProcedure(
                    "__fsave", 
                    VoidType.Instance, 
                    SrcOp(instrCur.Operands[0])));
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
                SrcOp(instrCur.Operands[0]),
                new BinaryExpression(Operator.Shl, PrimitiveType.Word16,
                        new Cast(PrimitiveType.Word16, orw.AluRegister(Registers.FPUF)),
                        Constant.Int16(8)));
        }

        public bool MatchesFstswSequence()
        {
            var nextInstr = dasm.Peek(1);
            if (nextInstr.code == Mnemonic.sahf)
            {
                this.len += nextInstr.Length;
                dasm.Skip(1);
                m.Assign(
                    orw.FlagGroup(FlagM.ZF | FlagM.CF | FlagM.SF | FlagM.OF),
                    orw.AluRegister(Registers.FPUF));
                return true;
            }
            if (nextInstr.code == Mnemonic.test)
            {
                RegisterOperand acc = nextInstr.Operands[0] as RegisterOperand;
                ImmediateOperand imm = nextInstr.Operands[1] as ImmediateOperand;
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
                    orw.AluRegister(Registers.FPUF));

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
                    case Mnemonic.mov: RewriteMov(); break;
                    case Mnemonic.fstp: RewriteFst(true); break;
                    case Mnemonic.push: RewritePush(); break;
                    case Mnemonic.lea: RewriteLea(); break;

                    case Mnemonic.jpe:
                        if (mask == 0x05) { Branch(ConditionCode.GE, instrCur.Operands[0]); return true; }
                        if (mask == 0x41) { Branch(ConditionCode.GT, instrCur.Operands[0]); return true; }
                        if (mask == 0x44) { Branch(ConditionCode.NE, instrCur.Operands[0]); return true; }
                        throw new AddressCorrelatedException(instrCur.Address, "Unexpected {0} fstsw mask for {1} opcode .", mask, instrCur.code);
                    case Mnemonic.jpo:
                        if (mask == 0x44) { Branch(ConditionCode.EQ, instrCur.Operands[0]); return true; }
                        if (mask == 0x41) { Branch(ConditionCode.LE, instrCur.Operands[0]); return true; }
                        if (mask == 0x05) { Branch(ConditionCode.LT, instrCur.Operands[0]); return true; }
                        throw new AddressCorrelatedException(instrCur.Address, "Unexpected {0} fstsw mask for {1} opcode .", mask, instrCur.code);
                    case Mnemonic.jz:
                        if (mask == 0x40) { Branch(ConditionCode.NE, instrCur.Operands[0]); return true; }
                        if (mask == 0x41) { Branch(ConditionCode.GT, instrCur.Operands[0]); return true; }
                        if (mask == 0x01) { Branch(ConditionCode.GE, instrCur.Operands[0]); return true; }
                        throw new AddressCorrelatedException(instrCur.Address, "Unexpected {0} fstsw mask for {1} opcode .", mask, instrCur.code);
                    case Mnemonic.jnz:
                        if (mask == 0x40) { Branch(ConditionCode.EQ, instrCur.Operands[0]); return true; }
                        if (mask == 0x41) { Branch(ConditionCode.LE, instrCur.Operands[0]); return true; }
                        if (mask == 0x01) { Branch(ConditionCode.LT, instrCur.Operands[0]); return true; }
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
            m.Branch(m.Test(code, orw.AluRegister(Registers.FPUF)), OperandAsCodeAddress( op), InstrClass.ConditionalTransfer);
        }

        private void RewriteFtst()
        {
            m.Assign(orw.FlagGroup(FlagM.CF),
                m.ISub(FpuRegister(0), Constant.Real64(0.0)));
        }

        private void RewriteFcomi(bool pop)
        {
            var op1 = SrcOp(instrCur.Operands[0]);
            var op2 = SrcOp(instrCur.Operands[1]);
            m.Assign(
                orw.FlagGroup(FlagM.ZF|FlagM.PF|FlagM.CF),
                m.Cond(
                    m.FSub(op1, op2)));
            m.Assign(orw.FlagGroup(FlagM.OF), Constant.False());
            m.Assign(orw.FlagGroup(FlagM.SF), Constant.False());
            if (pop)
            {
                ShrinkFpuStack(1);
            }
        }

        private void RewriteFxam()
        {
            m.Assign(orw.AluRegister(Registers.FPUF), m.Cond(FpuRegister(0)));
        }

        private void RewriteFxtract()
        {
            var fp = this.FpuRegister(0);
            var tmp = binder.CreateTemporary(fp.DataType);
            m.Assign(tmp, fp);
            GrowFpuStack(1);
            m.Assign(this.FpuRegister(1), host.PseudoProcedure("__exponent", fp.DataType, tmp));
            m.Assign(this.FpuRegister(0), host.PseudoProcedure("__significand", fp.DataType, tmp));
        }

        private void RewriteFyl2x()
        {
            //$REVIEW: Candidate for idiom search.
            var op1 = FpuRegister(0);
            var op2 = FpuRegister(1);
            m.Assign(op2, 
                m.FMul(op2, 
                      host.PseudoProcedure("lg2", PrimitiveType.Real64, op2)));

            ShrinkFpuStack(1);
        }

        private void RewriteFyl2xp1()
        {
            //$REVIEW: Candidate for idiom search.
            var op1 = FpuRegister(0);
            var op2 = FpuRegister(1);
            m.Assign(op2,
                m.FMul(
                    op2,
                    host.PseudoProcedure(
                        "lg2",
                        PrimitiveType.Real64,
                        m.FAdd(op1, Constant.Real64(1.0)))));
            m.Assign(
                orw.AluRegister(Registers.FPUF),
                m.Cond(op2));
            ShrinkFpuStack(1);
        }

        private void RewriteWait()
        {
            m.SideEffect(host.PseudoProcedure("__wait", VoidType.Instance));
        }

        private Expression FpuRegister(int reg)
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

        private void GrowFpuStack(int amount)
        {
            //$TODO: do this in SSA.
            //if (FpuStackItems > 7)
            //{
            //    Debug.WriteLine(string.Format("Possible FPU stack overflow at address {0}", addrInstr));    //$BUGBUG: should be an exception
            //}
            var top = binder.EnsureRegister(Registers.Top);
            m.Assign(top, m.ISubS(top, amount));
        }

        private void ShrinkFpuStack(int amount)
        {
            var top = binder.EnsureRegister(Registers.Top);
            m.Assign(top, m.IAddS(top, amount));
        }
    }
}
