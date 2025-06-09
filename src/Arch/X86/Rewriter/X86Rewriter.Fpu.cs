#region License
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
#endregion

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Arch.X86.Rewriter
{
    public partial class X86Rewriter
    {
        public void EmitCommonFpuInstruction(
            BinaryOperator op,
            bool fReversed,
            bool fPopStack)
        {
            EmitCommonFpuInstruction(op, fReversed, fPopStack, null);
        }

        public void EmitCommonFpuInstruction(
            BinaryOperator op,
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
                        m.Bin(
                            op,
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
                        m.Bin(
                            op,
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
                    m.Fn(
                        pow_intrinsic,
                        Constant.Real64(2.0),
                        FpuRegister(0)),
                    Constant.Real64(1.0)));
        }

        private void RewriteFabs()
        {
            m.Assign(FpuRegister(0), m.Fn(fabs_intrinsic, FpuRegister(0)));
        }

        private void RewriteFbld()
        {
            GrowFpuStack(1);
            m.Assign(FpuRegister(0),
                m.Fn(fbld_intrinsic, SrcOp(0)));
        }

        private void RewriteFbstp()
        {
            instrCur.Operands[0].DataType = PrimitiveType.Bcd80;
            var src = orw.FpuRegister(0);
            m.Assign(SrcOp(0), m.Convert(src, src.DataType, instrCur.Operands[0].DataType));
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
            m.SideEffect(m.Fn(fclex_intrinsic));
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
            m.SideEffect(m.Fn(ffree_intrinsic, SrcOp(0)));
            if (pop)
            {
                ShrinkFpuStack(1);
            }
        }

        private void RewriteFUnary(IntrinsicProcedure unaryIntrinsic)
        {
            m.Assign(
                orw.FpuRegister(0),
                m.Fn(unaryIntrinsic, orw.FpuRegister(0)));
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
            var iType = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].DataType.BitSize);
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
            var dtSrc = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].DataType.BitSize);
            instrCur.Operands[0].DataType = dtSrc;
            var fpuReg = orw.FpuRegister(0);
            var trunc = m.Fn(trunc_intrinsic, fpuReg);
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
            m.SideEffect(m.Fn(fldcw_intrinsic, SrcOp(0)));
        }

        private void RewriteFldenv()
        {
            m.SideEffect(m.Fn(fldenv_intrinsic, SrcOp(0)));
        }

        private void RewriteFninit()
        {
            var intrinsic = m.Fn(fninit_intrinsic);
            m.SideEffect(intrinsic);
        }

        private void RewriteFstenv()
        {
            m.SideEffect(m.Fn(
                fstenv_intrinsic,
                SrcOp(0)));
        }

        private void RewriteFpatan()
        {
            Expression op1 = FpuRegister(1);
            Expression op2 = FpuRegister(0);
            m.Assign(FpuRegister(1), m.Fn(atan2_intrinsic, op1, op2));
            ShrinkFpuStack(1);
        }

        private void RewriteFprem()
        {
            Expression src = FpuRegister(1);
            Expression dst = FpuRegister(0);
            m.Assign(dst, m.Fn(fprem_x87_intrinsic, dst, src));
            m.Assign(binder.EnsureFlagGroup(Registers.C2), m.Fn(fprem_incomplete_intrinsic, FpuRegister(0)));
        }

        private void RewriteFprem1()
        {
            Expression src = FpuRegister(1);
            Expression dst = FpuRegister(0);
            m.Assign(dst, m.FMod(dst, src));
            m.Assign(binder.EnsureFlagGroup(Registers.C2), m.Fn(fprem_incomplete_intrinsic, FpuRegister(0)));
        }

        private void RewriteFptan()
        {
            Expression op1 = FpuRegister(0);
            m.Assign(FpuRegister(0), m.Fn(tan_intrinsic, op1));
            GrowFpuStack(1);
            m.Assign(FpuRegister(0), Constant.Real64(1.0));
        }

        private void RewriteFsincos()
        {
            Identifier itmp = binder.CreateTemporary(PrimitiveType.Real64);
            m.Assign(itmp, FpuRegister(0));

            GrowFpuStack(1);
            m.Assign(FpuRegister(1), m.Fn(cos_intrinsic, itmp));
            m.Assign(FpuRegister(0), m.Fn(sin_intrinsic, itmp));
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
                m.Fn(fstcw_intrinsic));
        }

        private void RewriteFrstor()
        {
            m.SideEffect(
                m.Fn(
                    frstor_intrinsic,
                    SrcOp(0)));
        }

        private void RewriteFrstpm()
        {
            m.SideEffect(
                m.Fn(
                    frstpm_intrinsic));
        }

        private void RewriteFsave()
        {
            m.SideEffect(
                m.Fn(
                    fsave_intrinsic, 
                    SrcOp(0)));
        }

        private void RewriteFscale()
        {
            m.Assign(
                FpuRegister(0),
                m.Fn(scalbn_intrinsic, FpuRegister(0), FpuRegister(1)));
        }

        private void RewriteFstsw()
        {
            var opSrc = instrCur.Operands[0];
            var op = orw.Transform(instrCur, opSrc, opSrc.DataType);
            m.Assign(op, m.Fn(fstsw_intrinsic, binder.EnsureRegister(Registers.FPUF)));
            return;
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
            m.SideEffect(m.Fn(fxrstor_intrinsic));
        }

        private void RewriteFxsave()
        {
            m.SideEffect(m.Fn(fxsave_intrinsic));
        }

        private void RewriteFxtract()
        {
            var fp = this.FpuRegister(0);
            var tmp = binder.CreateTemporary(fp.DataType);
            m.Assign(tmp, fp);
            GrowFpuStack(1);
            m.Assign(this.FpuRegister(1), m.Fn(exponent_intrinsic, tmp));
            m.Assign(this.FpuRegister(0), m.Fn(significand_intrinsic, tmp));
        }

        private void RewriteFyl2x()
        {
            //$REVIEW: Candidate for idiom search.
            var op1 = FpuRegister(0);
            var op2 = FpuRegister(1);
            m.Assign(op2, 
                m.FMul(op2, 
                      m.Fn(lg2_intrinsic, op1)));
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
                    m.Fn(
                        lg2_intrinsic,
                        m.FAdd(op1, Constant.Real64(1.0)))));
            m.Assign(
                orw.AluRegister(Registers.FPUF),
                m.Cond(op2));
            ShrinkFpuStack(1);
        }

        private void RewriteWait()
        {
            m.SideEffect(m.Fn(wait_intrinsic));
        }

        private Expression FpuRegister(int reg)
        {
            return orw.FpuRegister(reg);
        }

        public Expression MaybeConvert(DataType? type, Expression e)
        {
            if (type is not null)
                return m.Convert(e, e.DataType, type);
            else
                return e;
        }

        public Expression MaybeSlice(DataType type, Expression e)
        {
            if (type.BitSize < e.DataType.BitSize)
            {
                if (e is MemoryAccess mem)
                {
                    mem.DataType = type;
                    return mem;
                }
                else
                {
                    return m.Slice(e, type);
                }
            }
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
