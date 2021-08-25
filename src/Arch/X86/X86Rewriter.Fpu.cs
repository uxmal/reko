#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Types;
using System;

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
            Domain? cast)
        {
            switch (instrCur.Operands.Length)
            {
            default:
                throw new ArgumentOutOfRangeException("di.Instruction", "Instruction must have 1 or 2 operands");
            case 1:
                {
                    // implicit st(0) operand.
                    var opLeft = FpuRegister(0);
                    var opRight = SrcOp(0);
                    if (cast.HasValue)
                    {
                        opRight.DataType = PrimitiveType.Create(cast.Value, opRight.DataType.BitSize);
                        opRight = m.Convert(opRight, opRight.DataType, opLeft.DataType);
                    }
                    else if (opRight.DataType.BitSize < opLeft.DataType.BitSize)
                    {
                        opRight = m.Convert(opRight, opRight.DataType, opLeft.DataType);
                    }
                    m.Assign(
                        opLeft,
                        op(
                            fReversed ? opRight : opLeft,
                            fReversed ? opLeft : opRight));
                    break;
                }
            case 2:
                {
                    Expression op1 = SrcOp(0);
                    Expression op2 = SrcOp(1);
                    m.Assign(
                        SrcOp(0),
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
                    host.Intrinsic(
                        "pow",
                        false,
                        PrimitiveType.Real64, 
                        Constant.Real64(2.0),
                        FpuRegister(0)),
                    Constant.Real64(1.0)));
        }

        private void RewriteFabs()
        {
            m.Assign(FpuRegister(0), host.Intrinsic("fabs", false, PrimitiveType.Real64, FpuRegister(0)));
        }

        private void RewriteFbld()
        {
            GrowFpuStack(1);
            m.Assign(FpuRegister(0),
                host.Intrinsic("__fbld", false, PrimitiveType.Real64, SrcOp(0)));
        }

        private void RewriteFbstp()
        {
            instrCur.Operands[0].Width = PrimitiveType.Bcd80;
            var src = orw.FpuRegister(0);
            m.Assign(SrcOp(0), m.Convert(src, src.DataType, instrCur.Operands[0].Width));
            ShrinkFpuStack(1);
        }

        private void EmitFchs()
        {
            m.Assign(
                orw.FpuRegister(0),
                m.Neg(orw.FpuRegister(0)));		//$BUGBUG: should be Real, since we don't know the actual size.
        }

        private void RewriteFclex()
        {
            m.SideEffect(host.Intrinsic("__fclex", true, VoidType.Instance));
        }

        private void RewriteFcmov(FlagGroupStorage flag, ConditionCode cc)
        {
            m.BranchInMiddleOfInstruction(
                m.Test(cc, binder.EnsureFlagGroup(flag)),
                instrCur.Address + instrCur.Length,
                InstrClass.ConditionalTransfer);

            var dst = SrcOp(0);
            var src = SrcOp(1);
            m.Assign(dst, src);
        }

        private void RewriteFcom(int pops)
        {
            var (op1, op2) = instrCur.Operands.Length switch
            {
                0 => (FpuRegister(0), FpuRegister(1)),
                1 => (FpuRegister(0), SrcOp(0)),
                2 => (SrcOp(0), SrcOp(1)),
                _ => throw new InvalidOperationException(),
            };
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

        private void RewriteFfree(bool pop)
        {
            m.SideEffect(
                host.Intrinsic("__ffree", true, VoidType.Instance, SrcOp(0)));
            if (pop)
            {
                ShrinkFpuStack(1);
            }
        }

        private void RewriteFUnary(string name, bool hasSideEffect)
        {
            m.Assign(
                orw.FpuRegister(0),
                host.Intrinsic(name, hasSideEffect, PrimitiveType.Real64, orw.FpuRegister(0)));
        }

        private void RewriteFicom(bool pop)
        {
            var src = SrcOp(0);
            var dtSrc = PrimitiveType.Create(Domain.SignedInt, src.DataType.BitSize);
            m.Assign(
                orw.AluRegister(Registers.FPUF),
                m.Cond(
                    m.FSub(
                        orw.FpuRegister(0),
                        m.Convert(src, dtSrc, PrimitiveType.Real64))));
            if (pop)
                ShrinkFpuStack(1);
        }

        private void RewriteFild()
        {
            GrowFpuStack(1);
            var iType = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            m.Assign(
                orw.FpuRegister(0),
                m.Convert(SrcOp(0, iType), iType, PrimitiveType.Real64));
        }

        private void RewriteFincstp()
        {
            GrowFpuStack(1);
            m.Nop();
        }

        private void RewriteFist(bool pop)
        {
            var src = orw.FpuRegister(0);
            var dst = SrcOp(0);
            dst.DataType = PrimitiveType.Create(Domain.SignedInt, dst.DataType.BitSize);
            m.Assign(dst, m.Convert(src, src.DataType, dst.DataType));
            if (pop)
                ShrinkFpuStack(1);
        }

        private void RewriteFistt(bool pop)
        {
            var dtSrc = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            instrCur.Operands[0].Width = dtSrc;
            var fpuReg = orw.FpuRegister(0);
            var trunc = host.Intrinsic("trunc", false, fpuReg.DataType, fpuReg);
            m.Assign(SrcOp(0), m.Convert(trunc, trunc.DataType, dtSrc));
            if (pop)
                ShrinkFpuStack(1);
        }

        public void RewriteFld()
        {
            var src = SrcOp(0);
            if (instrCur.Operands[0] is FpuOperand)
            {
                // Because we are changing the layout of the FPU stack, we must copy the loaded
                // value into a temp.
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = tmp;
            }
            GrowFpuStack(1);
            var dst = FpuRegister(0);
            if (src.DataType.Size != dst.DataType.Size)
            {
                src = m.Convert(
                    src,
                    src.DataType,
                    PrimitiveType.Create(Domain.Real, dst.DataType.BitSize));
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
            m.SideEffect(host.Intrinsic(
                "__fldcw",
                true,
                VoidType.Instance,
                SrcOp(0)));
        }

        private void RewriteFldenv()
        {
            m.SideEffect(host.Intrinsic(
                "__fldenv",
                true,
                VoidType.Instance,
                SrcOp(0)));
        }

        private void RewriteFninit()
        {
            var intrinsic = host.Intrinsic("__fninit", true, VoidType.Instance);
            m.SideEffect(intrinsic);
        }

        private void RewriteFstenv()
        {
            m.SideEffect(host.Intrinsic(
                "__fstenv",
                true,
                VoidType.Instance,
                SrcOp(0)));
        }

        private void RewriteFpatan()
        {
            Expression op1 = FpuRegister(1);
            Expression op2 = FpuRegister(0);
            m.Assign(FpuRegister(1), host.Intrinsic("atan2", false, PrimitiveType.Real64, op1, op2));
            ShrinkFpuStack(1);
        }

        private void RewriteFprem()
        {
            Expression src = FpuRegister(1);
            Expression dst = FpuRegister(0);
            m.Assign(dst, host.Intrinsic("__fprem_x87", false, dst.DataType, dst, src));
            m.Assign(binder.EnsureFlagGroup(Registers.C2), host.Intrinsic("__fprem_incomplete", false, PrimitiveType.Bool, FpuRegister(0)));
        }

        private void RewriteFprem1()
        {
            Expression src = FpuRegister(1);
            Expression dst = FpuRegister(0);
            m.Assign(dst, m.Mod(dst, src));
            m.Assign(binder.EnsureFlagGroup(Registers.C2), host.Intrinsic("__fprem_incomplete", false, PrimitiveType.Bool, FpuRegister(0)));
        }

        private void RewriteFptan()
        {
            Expression op1 = FpuRegister(0);
            m.Assign(FpuRegister(0), host.Intrinsic("tan", false, PrimitiveType.Real64, op1));
            GrowFpuStack(1);
            m.Assign(FpuRegister(0), Constant.Real64(1.0));
        }

        private void RewriteFsincos()
        {
            Identifier itmp = binder.CreateTemporary(PrimitiveType.Real64);
            m.Assign(itmp, FpuRegister(0));

            GrowFpuStack(1);
            m.Assign(FpuRegister(1), host.Intrinsic("cos", false, PrimitiveType.Real64, itmp));
            m.Assign(FpuRegister(0), host.Intrinsic("sin", false, PrimitiveType.Real64, itmp));
        }

        private void RewriteFst(bool pop)
        {
            Expression src = FpuRegister(0);
            Expression dst = SrcOp(0);
            if (src.DataType.Size != dst.DataType.Size)
            {
                src = m.Convert(
                    src,
                    src.DataType,
                    PrimitiveType.Create(Domain.Real, dst.DataType.BitSize));
            }
            m.Assign(dst, src);
            if (pop)
                ShrinkFpuStack(1);
        }

        private void RewriterFstcw()
        {
			m.Assign(
                SrcOp(0),
                host.Intrinsic("__fstcw",  true, PrimitiveType.UInt16));
        }

        private void RewriteFrstor()
        {
            m.SideEffect(
                host.Intrinsic(
                    "__frstor",
                    true,
                    VoidType.Instance,
                    SrcOp(0)));
        }

        private void RewriteFsave()
        {
            m.SideEffect(
                host.Intrinsic(
                    "__fsave", 
                    true,
                    VoidType.Instance, 
                    SrcOp(0)));
        }

        private void RewriteFscale()
        {
            m.Assign(
                FpuRegister(0),
                host.Intrinsic("scalbn", true, PrimitiveType.Real64, FpuRegister(0), FpuRegister(1)));
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
            var icur = instrCur;
            if (MatchesFstswSequence())
                return;
            var opSrc = icur.Operands[0];
            m.Assign(
                orw.Transform(instrCur, opSrc, opSrc.Width),
                new BinaryExpression(Operator.Shl, PrimitiveType.Word16,
                        m.Convert(orw.AluRegister(Registers.FPUF), Registers.FPUF.DataType, PrimitiveType.Word16),
                        Constant.Int16(8)));
        }

        public bool MatchesFstswSequence()
        {
            for (int i = 1; i < 4; ++i)
            {
                var nextInstr = dasm.Peek(i);
                if (nextInstr is null)
                    return false;
                switch (nextInstr.Mnemonic)
                {
                case Mnemonic.sahf:
                    {
                        this.len += nextInstr.Length;
                        dasm.Skip(i);
                        m.Assign(
                            binder.EnsureFlagGroup(Registers.SCZO),
                            orw.AluRegister(Registers.FPUF));
                        return true;
                    }
                case Mnemonic.and:
                    {
                        var acc = nextInstr.Operands[0] as RegisterStorage;
                        var imm = nextInstr.Operands[1] as ImmediateOperand;
                        if (imm == null || acc == null)
                            return false;
                        int mask = imm.Value.ToInt32();
                        if (acc == Registers.ax || acc == Registers.eax)
                            mask >>= 8;
                        else if (acc != Registers.ah)
                            return false;
                        nextInstr = dasm.Peek(i+1);       // peek at the instruction past the 'and'
                        if (nextInstr is null)
                            return false;
                        var nextOp = nextInstr.Mnemonic;
                        if (nextOp != Mnemonic.cmp && nextOp != Mnemonic.xor)
                            return false;
                        dasm.Skip(i);
                        acc = nextInstr.Operands[0] as RegisterStorage;
                        imm = nextInstr.Operands[1] as ImmediateOperand;
                        if (imm == null || acc == null)
                            return false;
                        mask = imm.Value.ToInt32() & mask;
                        if (acc == Registers.ax || acc == Registers.eax)
                            mask >>= 8;
                        else if (acc != Registers.ah)
                            return false;
                        dasm.Skip(i);       // over the 'cmp'
                        if (!IgnoreIntermediateInstructions())
                        {
                            host.Warn(instrCur.Address, "Expected branch instruction after fstsw;and {0},{1}.", acc, imm.Value);
                            return false;
                        }
                        if (nextOp == Mnemonic.cmp)
                        {
                            return EvaluateFstswCmpInstructions(mask);
                        }
                        else
                        {
                            return EvaluateFstswXorInstructions(mask);
                        }
                    }
                case Mnemonic.test:
                    {
                        var acc = nextInstr.Operands[0] as RegisterStorage;
                        var imm = nextInstr.Operands[1] as ImmediateOperand;
                        if (imm == null || acc == null)
                            return false;
                        int mask = imm.Value.ToInt32();
                        if (acc == Registers.ax || acc == Registers.eax)
                            mask >>= 8;
                        else if (acc != Registers.ah)
                            return false;
                        this.len += nextInstr.Length;
                        m.Assign(
                            binder.EnsureFlagGroup(Registers.SCZO),
                            orw.AluRegister(Registers.FPUF));

                        // Advance past the 'test' instruction.
                        dasm.Skip(i);
                        if (!IgnoreIntermediateInstructions())
                        {
                            host.Warn(instrCur.Address, "Expected conditinal branch instruction after fstsw;test {0},{1}.", acc, imm.Value);
                            return false;
                        }
                        return EvaluateFstswTestInstructions(mask);
                    }
                case Mnemonic.fstp: this.instrCur = nextInstr; RewriteFst(true); break;
                case Mnemonic.fst: this.instrCur = nextInstr; RewriteFst(false); break;
                case Mnemonic.mov: this.instrCur = nextInstr; RewriteMov(); break;
                default:
                    host.Warn(instrCur.Address, "Expected a use of FPU status. Last test instruction at {0}", instrCur.Address);
                    return false;
                }
            }
            host.Warn(instrCur.Address, "Expected a use of FPU status. Last test instruction at {0}", instrCur.Address);
            return false;
        }

        private bool IgnoreIntermediateInstructions()
        {
            while (dasm.MoveNext())
            {
                instrCur = dasm.Current;
                this.len += instrCur.Length;
                switch (instrCur.Mnemonic)
                {
                //$TODO The following instructions are being added on an ad-hoc
                // basis, since they don't affect the x86 flags register.
                // The long term fix is to implement an architecture-specific
                // condition code elimination pass as described elsewhere.
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.fstp: RewriteFst(true); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.lea: RewriteLea(); break;
                default:
                    return instrCur.InstructionClass.HasFlag(InstrClass.ConditionalTransfer);
                }
            }
            return false;
        }

        private bool EvaluateFstswCmpInstructions(int mask)
        {
            switch (instrCur.Mnemonic)
            {
            case Mnemonic.jz:
                switch (mask)
                {
                case 0x40: Branch(ConditionCode.EQ, instrCur.Operands[0]); return true;
                }
                break;
            }
            this.host.Warn(instrCur.Address, "Unexpected {0} fstsw;cmp mask for {1} mnemonic.", mask, instrCur.Mnemonic);
            return false;
        }

        private bool EvaluateFstswXorInstructions(int mask)
        {
            switch (instrCur.Mnemonic)
            {
            case Mnemonic.jz:
                switch (mask)
                {
                case 0x40: Branch(ConditionCode.NE, instrCur.Operands[0]); return true;
                }
                break;
            case Mnemonic.jnz:
                switch (mask)
                {
                case 0x40: Branch(ConditionCode.EQ, instrCur.Operands[0]); return true;
            }
                break;
            }
            this.host.Warn(instrCur.Address, "Unexpected {0} fstsw;xor mask for {1} mnemonic.", mask, instrCur.Mnemonic);
            return false;
        }

        private bool EvaluateFstswTestInstructions(int mask)
        {
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
                Mnem   Mask Condition
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


            switch (instrCur.Mnemonic)
            {
            case Mnemonic.jpe:
                switch (mask)
                {
                case 0x05: { Branch(ConditionCode.GE, instrCur.Operands[0]); return true; }
                case 0x41: { Branch(ConditionCode.GT, instrCur.Operands[0]); return true; }
                case 0x44: { Branch(ConditionCode.NE, instrCur.Operands[0]); return true; }
                default:
                    this.host.Warn(instrCur.Address, "Unexpected {0} fstsw mask for {1} mnemonic.", mask, instrCur.Mnemonic);
                    return false;
                }
            case Mnemonic.jpo:
                switch (mask)
                {
                case 0x44: { Branch(ConditionCode.EQ, instrCur.Operands[0]); return true; }
                case 0x41: { Branch(ConditionCode.LE, instrCur.Operands[0]); return true; }
                case 0x05: { Branch(ConditionCode.LT, instrCur.Operands[0]); return true; }
                default:
                    this.host.Warn(instrCur.Address, "Unexpected {0} fstsw mask for {1} mnemonic.", mask, instrCur.Mnemonic);
                    return false;
                }
            case Mnemonic.jz:
                switch (mask)
                {
                case 0x40: { Branch(ConditionCode.NE, instrCur.Operands[0]); return true; }
                case 0x41: { Branch(ConditionCode.GT, instrCur.Operands[0]); return true; }
                case 0x45: { Branch(ConditionCode.GT, instrCur.Operands[0]); return true; } //$TODO: or unordered.
                case 0x01: { Branch(ConditionCode.GE, instrCur.Operands[0]); return true; }
                case 0x05: { Branch(ConditionCode.GE, instrCur.Operands[0]); return true; }   //$TODO: or unordered
                default:
                    this.host.Warn(instrCur.Address, "Unexpected {0} fstsw mask for {1} mnemonic.", mask, instrCur.Mnemonic);
                    return false;
                }
            case Mnemonic.jnz:
                switch (mask)
                {
                case 0x40: { Branch(ConditionCode.EQ, instrCur.Operands[0]); return true; }
                case 0x41: { Branch(ConditionCode.LE, instrCur.Operands[0]); return true; }
                case 0x45: { Branch(ConditionCode.LE, instrCur.Operands[0]); return true; }  //$TODO: or unordered
                case 0x01: { Branch(ConditionCode.LT, instrCur.Operands[0]); return true; }
                case 0x05: { Branch(ConditionCode.LT, instrCur.Operands[0]); return true; }   //$TODO: or unordered
                default:
                    this.host.Warn(instrCur.Address, "Unexpected {0} fstsw mask for {1} mnemonic.", mask, instrCur.Mnemonic);
                    return false;
                }
            default:
                this.host.Warn(instrCur.Address, "Unexpected instruction {0} after fstsw", instrCur);
                return false;
            }
        }

        private void Branch(ConditionCode code, MachineOperand op)
        {
            this.iclass = InstrClass.ConditionalTransfer;
            m.Branch(m.Test(code, orw.AluRegister(Registers.FPUF)), OperandAsCodeAddress(op)!, InstrClass.ConditionalTransfer);
        }

        private void RewriteFtst()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),
                m.ISub(FpuRegister(0), Constant.Real64(0.0)));
        }

        private void RewriteFcomi(bool pop)
        {
            var op1 = SrcOp(0);
            var op2 = SrcOp(1);
            m.Assign(
                binder.EnsureFlagGroup(Registers.CZP),
                m.Cond(
                    m.FSub(op1, op2)));
            m.Assign(binder.EnsureFlagGroup(Registers.O), Constant.False());
            m.Assign(binder.EnsureFlagGroup(Registers.S), Constant.False());
            if (pop)
            {
                ShrinkFpuStack(1);
            }
        }

        private void RewriteFxam()
        {
            m.Assign(orw.AluRegister(Registers.FPUF), m.Cond(FpuRegister(0)));
        }

        private void RewriteFxrstor()
        {
            m.SideEffect(host.Intrinsic("__fxrstor", true, VoidType.Instance));
        }

        private void RewriteFxsave()
        {
            m.SideEffect(host.Intrinsic("__fxsave", true, VoidType.Instance));
        }

        private void RewriteFxtract()
        {
            var fp = this.FpuRegister(0);
            var tmp = binder.CreateTemporary(fp.DataType);
            m.Assign(tmp, fp);
            GrowFpuStack(1);
            m.Assign(this.FpuRegister(1), host.Intrinsic("__exponent", false, fp.DataType, tmp));
            m.Assign(this.FpuRegister(0), host.Intrinsic("__significand", false, fp.DataType, tmp));
        }

        private void RewriteFyl2x()
        {
            //$REVIEW: Candidate for idiom search.
            var op1 = FpuRegister(0);
            var op2 = FpuRegister(1);
            m.Assign(op2, 
                m.FMul(op2, 
                      host.Intrinsic("lg2", true, PrimitiveType.Real64, op1)));
            m.Assign(
                orw.AluRegister(Registers.FPUF),
                m.Cond(op2));
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
                    host.Intrinsic(
                        "lg2",
                        true,
                        PrimitiveType.Real64,
                        m.FAdd(op1, Constant.Real64(1.0)))));
            m.Assign(
                orw.AluRegister(Registers.FPUF),
                m.Cond(op2));
            ShrinkFpuStack(1);
        }

        private void RewriteWait()
        {
            m.SideEffect(host.Intrinsic("__wait", true, VoidType.Instance));
        }

        private Expression FpuRegister(int reg)
        {
            return orw.FpuRegister(reg);
        }

        public Expression MaybeConvert(DataType? type, Expression e)
        {
            if (type != null)
                return m.Convert(e, e.DataType, type);
            else
                return e;
        }

        public Expression MaybeSlice(DataType type, Expression e)
        {
            if (type.BitSize < e.DataType.BitSize)
                return m.Slice(type, e, 0);
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
