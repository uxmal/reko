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
using System.Diagnostics;

namespace Reko.Arch.X86.Rewriter
{
    /// <summary>
    /// Rewrite rules for simple ALU operations.
    /// </summary>
    public partial class X86Rewriter
    {
        private void RewriteAaa()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),
                m.Fn(aaa_intrinsic,
                    orw.AluRegister(Registers.al),
                    orw.AluRegister(Registers.ah),
                            orw.AddrOf(orw.AluRegister(Registers.al)),
                            orw.AddrOf(orw.AluRegister(Registers.ah))));
        }

        private void RewriteAad()
        {
            //$TODO: support for multiple register return values.
            m.Assign(
                orw.AluRegister(Registers.ax),
                m.Fn(aad_intrinsic, orw.AluRegister(Registers.ax)));
        }

        private void RewriteAam()
        {
            m.Assign(
                orw.AluRegister(Registers.ax),
                m.Fn(aam_intrinsic, orw.AluRegister(Registers.al)));
        }

        private void RewriteAas()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),
                m.Fn(aas_intrinsic,
                    orw.AluRegister(Registers.al),
                    orw.AluRegister(Registers.ah),
                            orw.AddrOf(orw.AluRegister(Registers.al)),
                            orw.AddrOf(orw.AluRegister(Registers.ah))));
        }

        public void RewriteAdcSbb(BinaryOperator opr)
        {
            //$REVIEW adc

            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(Registers.C);
            EmitCopy(
                0,
                m.Bin(
                    opr,
                    m.Bin(
                        opr,
                        SrcOp(0),
                        SrcOp(1, instrCur.Operands[0].DataType)),
                    c),
                CopyFlags.EmitCc);
        }

        private void RewriteAdcx(FlagGroupStorage carry)
        {
            //$REVIEW: adc
            var cy = binder.EnsureFlagGroup(carry);
            var dst = SrcOp(0);
            m.Assign(
                dst,
                m.IAdd(
                    m.IAdd(
                        dst,
                        SrcOp(1, dst.DataType)),
                    cy));
            m.Assign(cy, m.Cond(dst));
        }

        /// <summary>
        /// Doesn't handle the x86 idiom add ... adc => long add (and 
        /// sub ..sbc => long sub)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public void RewriteAddSub(BinaryOperator op)
        {
            EmitBinOp(
                op,
                0,
                instrCur.Operands[0].DataType,
                SrcOp(0),
                SrcOp(1),
                CopyFlags.EmitCc);
        }

        public void RewriteAnd()
        {
            if (instrCur.Operands[0] is RegisterStorage r &&
                r == arch.StackRegister &&
                instrCur.Operands[1] is Constant)
            {
                m.SideEffect(m.Fn(alignStack_intrinsic.MakeInstance(r.DataType), SrcOp(0)));
                return;
            }
            RewriteLogical(Operator.And);
        }

        private void RewriteArpl()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.Z),
                m.Fn(arpl_intrinsic,
                    SrcOp(0),
                    SrcOp(1),
                    orw.AddrOf(SrcOp(0))));
        }

        private void RewriteBextr()
        {
            //$REVIEW Is this "slice" in disguise?
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(bextr_intrinsic.MakeInstance(src1.DataType), src1, src2));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
        }

        private void RewriteBlsi()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(blsi_intrinsic.MakeInstance(src.DataType), src));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.S), m.Le0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.C), m.Eq0(src));
        }

        private void RewriteBlsmsk()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(blsmsk_intrinsic.MakeInstance(src.DataType), src));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.S), m.Le0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.C), m.Eq0(src));
        }
        private void RewriteBlsr()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(blsr_intrinsic.MakeInstance(src.DataType), src));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.S), m.Le0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.C), m.Eq0(src));
        }

        private void RewriteBound()
        {
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
            m.SideEffect(m.Fn(bound_intrinsic.MakeInstance(src1.DataType), src0, src1));
        }

        private void RewriteBzhi()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(bzhi_intrinsic.MakeInstance(src1.DataType), src1, src2));
        }

        private void RewriteCli()
        {
            m.SideEffect(m.Fn(cli_intrinsic));
        }

        private void RewriteCpuid()
        {
            m.SideEffect(
                m.Fn(cpuid_intrinsic,
                    orw.AluRegister(Registers.eax),
                    orw.AluRegister(Registers.ecx),
                    orw.AddrOf(orw.AluRegister(Registers.eax)),
                    orw.AddrOf(orw.AluRegister(Registers.ebx)),
                    orw.AddrOf(orw.AluRegister(Registers.ecx)),
                    orw.AddrOf(orw.AluRegister(Registers.edx))));
        }

        private void RewriteCrc32()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(crc32_intrinsic.MakeInstance(dst.DataType, src.DataType), dst, src));
        }

        private void RewriteRdmsr()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            var ecx = binder.EnsureRegister(Registers.ecx);
            m.Assign(edx_eax, m.Fn(rdmsr_intrinsic, ecx));
        }

        private void RewriteRdpid()
        {
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(rdpid_intrinsic.MakeInstance(dst.DataType)));
        }

        private void RewriteRdpmc()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            var ecx = binder.EnsureRegister(Registers.ecx);
            m.Assign(edx_eax, m.Fn(rdpmc_intrinsic, ecx));
        }

        private void RewriteRdtsc()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            m.Assign(edx_eax, m.Fn(rdtsc_intrinsic));
        }

        private void RewriteRdtscp()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            var ecx = binder.EnsureRegister(Registers.ecx);
            m.Assign(edx_eax, m.Fn(rdtscp_intrinsic, m.Out(arch.PointerType, ecx)));
        }

        public void RewriteBinOp(BinaryOperator opr)
        {
            int iOp = (instrCur.Operands.Length == 3) ? 1 : 0;
            var opL = SrcOp(iOp++);
            var opR = SrcOp(iOp);
            EmitBinOp(opr, 0, instrCur.DataWidth, opL, opR, CopyFlags.EmitCc);
        }

        private void RewriteBsf()
        {
            Expression src = SrcOp(1);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(src));
            m.Assign(SrcOp(0), m.Fn(bsf_intrinsic.MakeInstance(src.DataType), src));
        }

        private void RewriteBsr()
        {
            Expression src = SrcOp(1);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(src));
            m.Assign(SrcOp(0), m.Fn(bsr_intrinsic.MakeInstance(src.DataType), src));
        }

        public void RewriteBswap()
        {
            Identifier reg = binder.EnsureRegister((RegisterStorage) instrCur.Operands[0]);
            m.Assign(reg, m.Fn(bswap_intrinsic.MakeInstance(reg.DataType), reg));
        }

        private void RewriteBt()
        {
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
		    m.Assign(
                binder.EnsureFlagGroup(Registers.C),
                m.Fn(bt_intrinsic.MakeInstance(src0.DataType), src0, src1));
        }

        private void RewriteBtc()
        {
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),
                m.Fn(btc_intrinsic.MakeInstance(src0.DataType), src0, src1,
                    m.Out(src0.DataType, src0)));
        }

        private void RewriteBtr()
        {
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),             // lhs
                m.Fn(btr_intrinsic.MakeInstance(src0.DataType), src0, src1,
                    m.Out(src0.DataType, src0)));
        }

        private void RewriteBts()
        {
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),    // lhs
                m.Fn(bts_intrinsic.MakeInstance(src0.DataType), src0, src1, 
                        m.Out(src0.DataType, src0)));
        }

        public void RewriteCbw()
        {
            m.Assign(
                orw.AluRegister(Registers.ax),
                m.Convert(orw.AluRegister(Registers.al), PrimitiveType.SByte, PrimitiveType.Int16));
        }

        private void RewriteCdq()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Int64,
                Registers.edx,
                Registers.eax);
            m.Assign(
                edx_eax, 
                m.Convert(orw.AluRegister(Registers.eax), PrimitiveType.Int32, PrimitiveType.Int64));
        }

        public void RewriteCdqe()
        {
            m.Assign(
                orw.AluRegister(Registers.rax),
                m.Convert(orw.AluRegister(Registers.eax), PrimitiveType.Int32, PrimitiveType.Int64));
        }

        public void RewriteCqo()
        {
            Identifier rdx_rax = binder.EnsureSequence(
                PrimitiveType.Int128,
                Registers.rdx,
                Registers.rax);
            m.Assign(
                rdx_rax,
                m.Convert(orw.AluRegister(Registers.rax), PrimitiveType.Int64, PrimitiveType.Int128));
        }

        private void RewriteCwd()
        {
            Identifier dx_ax = binder.EnsureSequence(
                PrimitiveType.Int32,
                Registers.dx,
                Registers.ax);
            m.Assign(
                dx_ax, m.Convert(orw.AluRegister(Registers.ax), PrimitiveType.Int16, dx_ax.DataType));
        }

        public void RewriteCwde()
        {
            m.Assign(
                orw.AluRegister(Registers.eax),
                m.Convert(orw.AluRegister(Registers.ax), PrimitiveType.Int16, PrimitiveType.Int32));
        }

        public Expression EmitBinOp(BinaryOperator binOp, int iOpDst, DataType dtDst, Expression left, Expression right, CopyFlags flags = 0)
        {
            right = SignExtendConstant(right, left.DataType);
            var bin = m.Bin(binOp, dtDst, left, right);
            return EmitCopy(iOpDst, bin, flags);
        }

        public Expression EmitBinOp(Func<Expression, Expression, Expression> fn, int iOpDst, DataType dtDst, Expression left, Expression right, CopyFlags flags = 0)
        {
            right = SignExtendConstant(right, left.DataType);
            var bin = fn(left, right);
            return EmitCopy(iOpDst, bin, flags);
        }

        private Expression SignExtendConstant(Expression exp, DataType dt)
        {
            if (exp is Constant c)
            {
                if (c.DataType.BitSize < dt.BitSize)
                {
                    exp = m.Const(dt, c.ToInt64());
                }
            }
            return exp;
        }

        /// <summary>
        /// Emits an assignment to a flag-group pseudoregister if <paramref name="defFlags"/> is 
        /// not zero.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="defFlags">flags defined by the intel instruction</param>
        private void EmitCcInstr(Expression expr, FlagGroupStorage? defFlags)
        {
            if (defFlags is null)
                return;
            m.Assign(binder.EnsureFlagGroup(defFlags), new ConditionOf(expr.CloneExpression()));
        }

        private void EmitLogicalFlags(Expression result)
        {
            EmitCcInstr(result, Registers.SZ);
            m.Assign(binder.EnsureFlagGroup(Registers.O), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.C), 0);
        }

        private void RewriteConditionalMove(ConditionCode cc, MachineOperand dst, MachineOperand src)
        {
            var test = CreateTestCondition(cc, instrCur.Mnemonic).Invert();
            m.BranchInMiddleOfInstruction(
                test,
                instrCur.Address + instrCur.Length,
                InstrClass.ConditionalTransfer);
            var opSrc = SrcOp(src);
            var opDst = SrcOp(dst);
            m.Assign(opDst, opSrc);
        }

        private void RewriteCmp()
        {
            Expression op1 = SrcOp(0);
            Expression op2 = SrcOp(1, instrCur.Operands[0].DataType);
            m.Assign(
                binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!),
                new ConditionOf(m.ISub(op1, op2)));
        }

        private void RewriteCmpxchg()
        {
            var op1 = SrcOp(0);
            var op2 = SrcOp(1, instrCur.Operands[0].DataType);
            var acc = orw.AluRegister(Registers.rax, instrCur.Operands[0].DataType);
            var Z = binder.EnsureFlagGroup(Registers.Z);
            m.Assign(
                Z,
                m.Fn(
                    cmpxchg_intrinsic.MakeInstance(op1.DataType),
                    op1, op2, acc, m.Out(instrCur.DataWidth, acc)));
        }

        private void RewriteCmpxchgNb(
            IntrinsicProcedure intrinsic,
            RegisterStorage edx,
            RegisterStorage eax,
            RegisterStorage ecx,
            RegisterStorage ebx)
        {
            var bitsize = edx.DataType.BitSize * 2;
            var dt = PrimitiveType.CreateWord(bitsize);
            var op1 = binder.EnsureSequence(dt, edx, eax);
            var op2 = SrcOp(0, dt);
            var acc = binder.EnsureSequence(dt, ecx, ebx);
            var Z = binder.EnsureFlagGroup(Registers.Z);
            m.Assign(
                Z,
                m.Fn(
                    intrinsic.MakeInstance(op1.DataType),
                    op1, op2, acc, m.Out(instrCur.DataWidth, op1)));
        }

        private void EmitDaaDas(IntrinsicProcedure intrinsic)
        {
            m.Assign(binder.EnsureFlagGroup(Registers.C), m.Fn(
                intrinsic,
                orw.AluRegister(Registers.al),
                orw.AddrOf(orw.AluRegister(Registers.al))));
        }

        private void RewriteDivide(
            BinaryOperator div,
            BinaryOperator rem,
            Domain domain)
        {
            if (instrCur.Operands.Length != 1)
                throw new ArgumentOutOfRangeException("X86 DIV/IDIV instructions only take one operand.");
            Identifier regDividend;
            Identifier regQuotient;
            Identifier regRemainder;

            switch (instrCur.DataWidth.Size)
            {
            case 1:
                regQuotient = orw.AluRegister(Registers.al);
                regDividend = orw.AluRegister(Registers.ax);
                regRemainder = orw.AluRegister(Registers.ah);
                break;
            case 2:
                regQuotient = orw.AluRegister(Registers.ax);
                regRemainder = orw.AluRegister(Registers.dx);
                regDividend = binder.EnsureSequence(PrimitiveType.Word32, regRemainder.Storage, regQuotient.Storage);
                break;
            case 4:
                regQuotient = orw.AluRegister(Registers.eax);
                regRemainder = orw.AluRegister(Registers.edx);
                regDividend = binder.EnsureSequence(PrimitiveType.Word64, regRemainder.Storage, regQuotient.Storage);
                break;
            case 8:
                regQuotient = orw.AluRegister(Registers.rax);
                regRemainder = orw.AluRegister(Registers.rdx);
                regDividend = binder.EnsureSequence(PrimitiveType.Word128, regRemainder.Storage, regQuotient.Storage);
                break;
            default:
                throw new ArgumentOutOfRangeException(string.Format("{0}-byte divisions not supported.", instrCur.DataWidth.Size));
            };
            PrimitiveType p = ((PrimitiveType)regRemainder.DataType).MaskDomain(domain);
            var tmp = binder.CreateTemporary(regDividend.DataType);
            m.Assign(tmp, regDividend);
            var divisor = SrcOp(0);
            var r = m.Bin(rem, p, tmp, divisor);
            divisor = SrcOp(0);
            var q = m.Bin(div, p, tmp, divisor);
            m.Assign(regRemainder, r);
            m.Assign(regQuotient, q);
            EmitCcInstr(regQuotient, X86Instruction.DefCc(instrCur.Mnemonic));
        }

        private void RewriteEnter(DataType dtFramePointer)
        {
            var bp = orw.AluRegister(Registers.ebp, dtFramePointer)!;
            RewritePush(dtFramePointer, bp);
            var sp = StackPointer();
            m.Assign(bp, MaybeSlice(bp.DataType, sp));
            var cbExtraSavedBytes = 
                instrCur.DataWidth.Size * ((Constant)instrCur.Operands[1]).ToInt32() +
                ((Constant)instrCur.Operands[0]).ToInt32();
            if (cbExtraSavedBytes != 0)
            {
                m.Assign(sp, m.ISubS(sp, cbExtraSavedBytes));
            }
        }

        private void RewriteExchange()
        {
            Identifier itmp = binder.CreateTemporary(instrCur.Operands[0].DataType);
            m.Assign(itmp, SrcOp(0));
            m.Assign(SrcOp(0), SrcOp(1));
            m.Assign(SrcOp(1), itmp);
        }

        private void RewriteIncDec(int amount)
        {
            var op = Operator.IAdd;
            if (amount < 0)
            {
                op = Operator.ISub;
                amount = -amount;
            }

            EmitBinOp(op,
                0,
                instrCur.Operands[0].DataType,
                SrcOp(0),
                m.Const(instrCur.Operands[0].DataType, amount),
                CopyFlags.EmitCc);
        }

        private void RewriteLock()
        {
            m.SideEffect(m.Fn(lock_intrinsic));
        }

        private void RewriteLogical(BinaryOperator op)
        {
            if (instrCur.Operands.Length == 3)
            {
                EmitBinOp(
                    op,
                    0,
                    instrCur.Operands[0].DataType,
                    SrcOp(1),
                    SrcOp(2));
            }
            else
            {
                EmitBinOp(
                    op,
                    0,
                    instrCur.Operands[0].DataType,
                    SrcOp(0),
                    SrcOp(1));
            }
            EmitLogicalFlags(SrcOp(0));
        }

        private void RewriteLogical(Func<Expression, Expression,Expression> op)
        {
            if (instrCur.Operands.Length == 3)
            {
                EmitBinOp(
                    op,
                    0,
                    instrCur.Operands[0].DataType,
                    SrcOp(1),
                    SrcOp(2));
            }
            else
            {
                EmitBinOp(
                    op,
                    0,
                    instrCur.Operands[0].DataType,
                    SrcOp(0),
                    SrcOp(1));
            }
            EmitLogicalFlags(SrcOp(0));
        }

        private void RewriteMultiply(BinaryOperator op, Domain resultDomain)
        {
            Expression product;
            switch (instrCur.Operands.Length)
            {
            case 1:
                Identifier multiplicator;

                switch (instrCur.Operands[0].DataType.Size)
                {
                case 1:
                    multiplicator = orw.AluRegister(Registers.al);
                    product = orw.AluRegister(Registers.ax);
                    break;
                case 2:
                    multiplicator = orw.AluRegister(Registers.ax);
                    product = binder.EnsureSequence(
                        PrimitiveType.Word32, Registers.dx, multiplicator.Storage);
                    break;
                case 4:
                    multiplicator = orw.AluRegister(Registers.eax);
                    product = binder.EnsureSequence(
                        PrimitiveType.Word64, Registers.edx, multiplicator.Storage);
                    break;
                case 8:
                    multiplicator = orw.AluRegister(Registers.rax);
                    product = binder.EnsureSequence(
                        PrimitiveType.Word128, Registers.rdx, multiplicator.Storage);
                    break;
                default:
                    throw new ApplicationException(string.Format("Unexpected operand size: {0}", instrCur.Operands[0].DataType));
                };
                var bin = m.Bin(op, SrcOp(0), multiplicator);
                bin.DataType = PrimitiveType.Create(resultDomain, product.DataType.BitSize);
                m.Assign(product, bin);
                EmitCcInstr(product, X86Instruction.DefCc(instrCur.Mnemonic));
                return;
            case 2:
                EmitBinOp(
                    op,
                    0, 
                    PrimitiveType.Create(resultDomain, instrCur.Operands[0].DataType.BitSize),
                    SrcOp(0),
                    SrcOp(1), 
                    CopyFlags.EmitCc);
                return;
            case 3:
                EmitBinOp(op,
                    0,
                    PrimitiveType.Create(resultDomain, instrCur.Operands[0].DataType.BitSize),
                    SrcOp(1),
                    SrcOp(2),
                    CopyFlags.EmitCc);
                return;
            default:
                throw new ArgumentException("Invalid number of operands");
            }
        }

        private void RewriteMulx()
        {
            var opL = SrcOp(2);
            var opR = SrcOp(3);
            var bits = instrCur.Operands[0].DataType.BitSize + instrCur.Operands[1].DataType.BitSize;
            var dt = PrimitiveType.Create(Domain.UnsignedInt, bits);
            var hi = (RegisterStorage) instrCur.Operands[0];
            var lo = (RegisterStorage) instrCur.Operands[1];
            var opDst = binder.EnsureSequence(dt, hi, lo);
            var mul = m.UMul(opL, opR);
            mul.DataType = dt;
            m.Assign(opDst, mul);
        }

        private void RewriteIn()
        {
            var dst = SrcOp(0);
            m.Assign(
                dst,
                m.Fn(in_intrinsic.MakeInstance(dst.DataType), SrcOp(1)));
        }

        public void RewriteLahf()
        {
            //$TODO: it should actually be SCZAP, as the OF flag is not used but the AF one is
            m.Assign(orw.AluRegister(Registers.ah), orw.FlagGroup(Registers.SCZOP));
        }

        public void RewriteLea()
        {
            Expression src;
            MemoryOperand mem = (MemoryOperand)instrCur.Operands[1];
            if (mem.Base == RegisterStorage.None && mem.Index == RegisterStorage.None)
            {
                src = mem.Offset!;
            }
            else
            {
                src = SrcOp(1);
                if (src is MemoryAccess load)
                {
                    src = load.EffectiveAddress;
                    if (src is SegmentedPointer segptr)
                    {
                        src = segptr.Offset;
                    }
                }
                else
                {
                    src = orw.AddrOf(src);
                }
            }
            var dst = (Identifier)SrcOp(0);
            src = MaybeSlice(dst.DataType, src);
            if (dst.DataType.BitSize > src.DataType.BitSize)
            {
                src = m.Convert(src, src.DataType, dst.DataType);
            }
            AssignToRegister(dst, src);
        }

        private void RewriteLeave()
        {
            var sp = orw.AluRegister(arch.StackRegister);
            var bp = orw.AluRegister(Registers.rbp, arch.StackRegister.DataType)!;
            m.Assign(sp, bp);
            m.Assign(bp, orw.StackAccess(sp, bp.DataType));
            m.Assign(sp, m.IAddS(sp, bp.DataType.Size));
        }

        private void RewriteLxs(RegisterStorage seg)
        {
            var reg = (RegisterStorage)instrCur.Operands[0];
            MemoryOperand mem = (MemoryOperand)instrCur.Operands[1];
            if (mem.Offset is null)
            {
                mem = new MemoryOperand(mem.DataType, mem.Base, mem.Index, mem.Scale, Constant.Create(instrCur.AddressWidth, 0));
            }

            var ptr = PrimitiveType.Create(Domain.Pointer, seg.DataType.BitSize + reg.DataType.BitSize);
            var segptr = PrimitiveType.Create(Domain.SegPointer, seg.DataType.BitSize + reg.DataType.BitSize);
            m.Assign(
                binder.EnsureSequence(ptr, seg, reg),
                orw.Transform(instrCur, mem, segptr));
        }

        private void RewriteLeadingTrailingZeros(IntrinsicProcedure intrinsic)
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(src.DataType), src));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
        }

        private void RewriteMov()
        {
            var dst = SrcOp(0);
            var src = SrcOp(1, instrCur.Operands[0].DataType);
            if (dst is Identifier idDst)
            {
                this.AssignToRegister(idDst, src);
            }
            else
            {
                m.Assign(dst, src);
            }
        }

        private void RewriteMovbe()
        {
            var src = SrcOp(1, instrCur.Operands[0].DataType);
            var dst = SrcOp(0);
            src = m.Fn(movbe_intrinsic.MakeInstance(dst.DataType), src);
            EmitCopy(0, src);
        }

        private void RewriteMovssd(PrimitiveType dt, IntrinsicProcedure intrinsic)
        {
            if (instrCur.Operands.Length == 3)
            {
                var src1 = SrcOp(1);
                var src2 = SrcOp(2);
                var dst = SrcOp(0);
                m.Assign(dst, m.Fn(intrinsic.MakeInstance(dt), src1, src2));
            }
            else
            {
                var dst = SrcOp(0);
                var src = SrcOp(1);
                if (src is Identifier)
                {
                    src = m.Slice(src, dt);
                }
                if (dst is Identifier idDst)
                {
                    if (src is MemoryAccess mem)
                    {
                        var dtHigh = PrimitiveType.CreateWord(idDst.DataType.BitSize - mem.DataType.BitSize);
                        m.Assign(idDst, m.Seq(Constant.Zero(dtHigh), mem));
                    }
                    else
                    {
                        m.Assign(idDst, m.Dpb(idDst, src, 0));
                    }
                }
                else
                {
                    m.Assign(dst, src);
                }
            }
        }

        private void RewriteMovsx()
        {
            var dst = SrcOp(0);
            var src = SrcOp(1);
            var dstBitSize = dst.DataType.BitSize;
            if (dstBitSize != src.DataType.BitSize)
            {
                src = m.Convert(src, src.DataType, PrimitiveType.Create(Domain.SignedInt, dstBitSize));
            }
            m.Assign(dst, src);
        }

        private void RewriteMovzx()
        {
            var src = SrcOp(1);
            EmitCopy(
                0,
                m.Convert(src, src.DataType, instrCur.Operands[0].DataType));
        }

        private void RewritePush()
        {
            if (instrCur.Operands[0] is RegisterStorage reg && reg == Registers.cs)
            {
                // Is it a 'push cs;call near XXXX' sequence that simulates a far call?
                if (dasm.TryPeek(1, out X86Instruction? p1) &&
                    p1!.Mnemonic == Mnemonic.call &&
                    p1.Operands[0].DataType.BitSize == 16)
                {
                    dasm.MoveNext();
                    MachineOperand targetOperand = dasm.Current.Operands[0];

                    if (targetOperand is Constant immOperand)
                    {
                        targetOperand = orw.ImmediateAsAddress(instrCur.Address, immOperand);
                    }
                    else
                    {
                        m.Assign(StackPointer(), m.ISubS(StackPointer(), reg.DataType.Size));
                    }

                    RewriteCall(targetOperand, targetOperand.DataType);
                    this.len = (byte) (this.len + dasm.Current.Length);
                    return;
                }

                if (p1 is not null &&
                    dasm.TryPeek(2, out var p2) &&
                    dasm.TryPeek(3, out var p3) &&
                    (p1.Mnemonic == Mnemonic.push && (p1.Operands[0] is Constant)) &&
                    (p2!.Mnemonic == Mnemonic.push && (p2.Operands[0] is Constant)) &&
                    (p3!.Mnemonic == Mnemonic.jmp && (p3.Operands[0] is Address)))
                {
                    // That's actually a far call, but the callee thinks its a near call.
                    RewriteCall(p3.Operands[0], instrCur.Operands[0].DataType);
                    dasm.MoveNext();
                    dasm.MoveNext();
                    dasm.MoveNext();
                    return;
                }
            }
            var value = SrcOp(0, instrCur.DataWidth);
            Debug.Assert(
                value.DataType.BitSize == 16 ||
                value.DataType.BitSize == 32 ||
                value.DataType.BitSize == 64,
                $"Unexpected size {dasm.Current.DataWidth}");
            RewritePush(PrimitiveType.CreateWord(value.DataType.BitSize), value);
        }

        private void RewritePush(RegisterStorage reg)
        {
            RewritePush(instrCur.DataWidth, orw.AluRegister(reg));
        }

        private void RewritePusha()
        {
            if (instrCur.DataWidth == PrimitiveType.Word16)
            {
                Identifier temp = binder.CreateTemporary(Registers.sp.DataType);
                m.Assign(temp, orw.AluRegister(Registers.sp));
                RewritePush(Registers.ax);
                RewritePush(Registers.cx);
                RewritePush(Registers.dx);
                RewritePush(Registers.bx);
                RewritePush(PrimitiveType.Word16, temp);
                RewritePush(Registers.bp);
                RewritePush(Registers.si);
                RewritePush(Registers.di);
            }
            else
            {
                Identifier temp = binder.CreateTemporary(Registers.esp.DataType);
                m.Assign(temp, orw.AluRegister(Registers.esp));
                RewritePush(Registers.eax);
                RewritePush(Registers.ecx);
                RewritePush(Registers.edx);
                RewritePush(Registers.ebx);
                RewritePush(PrimitiveType.Word32, temp);
                RewritePush(Registers.ebp);
                RewritePush(Registers.esi);
                RewritePush(Registers.edi);
            }
        }

        private void RewritePushf(DataType dt)
        {
            var flags = binder.EnsureFlagGroup(Registers.SCZDOP);
            RewritePush(
                dt,
                MaybeSlice(dt, flags));
        }

        private void RewriteNeg()
        {
            var src = SrcOp(0);
            if (src is Identifier idSrc)
            {
                m.Assign(binder.EnsureFlagGroup(Registers.C), m.Ne0(idSrc));
            }
            else
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                m.Assign(binder.EnsureFlagGroup(Registers.C), m.Ne0(tmp));
                src = tmp;
            }
            EmitCopy(0, m.Neg(src), CopyFlags.EmitCc);
        }

        private void RewriteNot()
        {
            RewriteUnaryOperator(Operator.Comp, 0, instrCur.Operands[0]);
        }

        private void RewriteOut()
        {
            var port = SrcOp(0);
            var val = SrcOp(1);
            var suffix = NamingPolicy.Instance.Types.ShortPrefix(instrCur.Operands[1].DataType);
            var intrinsic = m.Fn(out_intrinsic.MakeInstance(val.DataType), port, val); 
            m.SideEffect(intrinsic);
        }

        private void RewritePdep()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(pdep_intrinsic.MakeInstance(dst.DataType), src1, src2));
        }

        private void RewritePext()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(pext_intrinsic.MakeInstance(dst.DataType), src1, src2));
        }

        private void RewritePop()
        {
            // This hack handles the occurence of
            //      pop <reg>   ; Pop the near return address
            //      push cs     ; push the CS selector
            //      push <reg>  ; push the return address
            //      <rest of procedure>
            // which "converts" a near call to a far call.
            // The code below replaces the sequence with a far call
            // to the instruction following the second push. The 
            // resulting IR instructions generated are equivalent
            // to:
            //      call <rest of procedure>
            //      ret         ; near return
            //      <rest of procedure>
            //
            // Ideally this check should only be triggered at the start 
            // of a procedure. Maybe a future version of Reko
            // will have a "hint" to the rewriter telling it
            // that the current instruction is at the start of a procedure

            if (instrCur.Operands[0] is RegisterStorage regScratch &&
                regScratch.BitSize == 16 &&
                dasm.TryPeek(1, out var pushCs) &&
                    pushCs.Mnemonic == Mnemonic.push &&
                    pushCs.Operands[0] == Registers.cs &&
                dasm.TryPeek(2, out var pushScratch) &&
                    pushScratch.Mnemonic == Mnemonic.push &&
                    pushScratch.Operands[0] == regScratch)
            {
                var callTarget = pushScratch.Address + pushScratch.Length;
                this.iclass = InstrClass.Call;
                m.Call(callTarget, 4);
                m.Return(2, 0);
                dasm.Skip(2);
                return;
            }
            RewritePop(dasm.Current.Operands[0], dasm.Current.Operands[0].DataType);
        }

        private void RewritePop(MachineOperand op, DataType width)
        {
            var sp = StackPointer();
            m.Assign(SrcOp(op), orw.StackAccess(sp, width));
            m.Assign(sp, m.IAddS(sp, width.Size));
        }

        private void RewritePop(Identifier dst, DataType width)
        {
            var sp = StackPointer();
            m.Assign(dst, orw.StackAccess(sp, width));
            m.Assign(sp, m.IAdd(sp, width.Size));
        }

        private void EmitPop(RegisterStorage reg)
        {
            RewritePop(orw.AluRegister(reg), instrCur.DataWidth);
        }

        private void RewritePopa()
        {
            Debug.Assert(instrCur.DataWidth == PrimitiveType.Word16 || instrCur.DataWidth == PrimitiveType.Word32);
            var sp = StackPointer();
            if (instrCur.DataWidth == PrimitiveType.Word16)
            {
                EmitPop(Registers.di);
                EmitPop(Registers.si);
                EmitPop(Registers.bp);
                m.Assign(sp, m.IAdd(sp, instrCur.DataWidth.Size));
                EmitPop(Registers.bx);
                EmitPop(Registers.dx);
                EmitPop(Registers.cx);
                EmitPop(Registers.ax);
            }
            else
            {
                EmitPop(Registers.edi);
                EmitPop(Registers.esi);
                EmitPop(Registers.ebp);
                m.Assign(sp, m.IAdd(sp, instrCur.DataWidth.Size));
                EmitPop(Registers.ebx);
                EmitPop(Registers.edx);
                EmitPop(Registers.ecx);
                EmitPop(Registers.eax);
            }
        }

        private void RewritePopf(DataType width)
        {
            var sp = StackPointer();
            var src = orw.StackAccess(sp, width);
            var grf = binder.EnsureFlagGroup(Registers.SCZDOP);
            if (grf.DataType.BitSize > src.DataType.BitSize)
            {
                m.Assign(grf, m.Dpb(grf, src, 0));
            }
            else
            {
                m.Assign(grf, src);
            }
            m.Assign(sp, m.IAddS(sp, width.Size));
        }

        private void RewritePopcnt()
        {
            var src = SrcOp(1);
            if (src is MemoryAccess)
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = tmp;
            }
            var dst = (Identifier) SrcOp(0);
            var dt = PrimitiveType.Create(Domain.SignedInt, dst.DataType.BitSize);
            AssignToRegister(dst, m.Fn(popcnt_intrinsic.MakeInstance(src.DataType, dt), src));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(src));
        }

        private void RewritePush(DataType dataWidth, Expression expr)
        {
            if (expr is Constant c && c.DataType != dataWidth)
            {
                expr = Constant.Create(dataWidth, c.ToInt64());
            }

            // Allocate a local variable for the push.
            var sp = StackPointer();
            Expression rhs;

            // Check if the push requires preserving the original stack pointer
            if (expr is Constant || (expr is Identifier id && id.Storage != arch.StackRegister))
            {
                rhs = expr;
            }
            else
            {
                rhs = binder.CreateTemporary(sp.DataType);
                m.Assign(rhs, expr);
            }
            m.Assign(sp, m.ISubS(sp, dataWidth.Size));
            m.Assign(orw.StackAccess(sp, dataWidth), rhs);
        }

        private void RewriteRotation(IntrinsicProcedure operation, Expression sh)
        {
            var cy = binder.EnsureFlagGroup(Registers.C);
            m.Assign(cy, m.Ne0(m.And(SrcOp(0), sh)));
            Expression p;
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
            p = m.Fn(
                operation.MakeInstance(src0.DataType, src1.DataType),
                src0, src1);
            m.Assign(SrcOp(0), p);
        }

        private void RewriteRotationWithCarry(IntrinsicProcedure operation, Expression sh)
        {
            Identifier? t;
            var cy = binder.EnsureFlagGroup(Registers.C);
            t = binder.CreateTemporary(PrimitiveType.Bool);
            m.Assign(t, cy);
            m.Assign(cy, m.Ne0(m.And(SrcOp(0), sh)));
            Expression p;
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
            p = m.Fn(
                operation.MakeInstance(src0.DataType, src1.DataType),
                src0, src1, t);
            m.Assign(SrcOp(0), p);
        }

        private Expression RotateMaskLeft()
        {
            var ops = instrCur.Operands;
            Expression sh = m.ISub(
                    Constant.Create(ops[1].DataType, ops[0].DataType.BitSize),
                    SrcOp(1));
            return m.Shl(Constant.Create(ops[0].DataType, 1), sh);
        }

        private Expression RotateMaskRight()
        {
            Expression sh = SrcOp(1);
            sh = m.Shl(Constant.Create(instrCur.Operands[0].DataType, 1), m.ISub(sh, 1));
            return sh;
        }

        private void RewriteRorx()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            var rotation = Core.Intrinsics.CommonOps.Ror.MakeInstance(src1.DataType, src2.DataType);
            m.Assign(dst, m.Fn(rotation, src1, src2));
        }

        private void RewriteSar()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            var value = EmitBinOp(Operator.Sar, 0, instrCur.DataWidth, dst, src);

            if (src is Constant c && c.ToInt32() == 1)
            {
                EmitCcInstr(value, Registers.SCZ);
                m.Assign(binder.EnsureFlagGroup(Registers.O), 0);
            }
            else
            {
                EmitCcInstr(value, Registers.SCZO);
            }
        }

        private void RewriteSet(ConditionCode cc)
        {
            var dst = SrcOp(0);
            m.Assign(dst, m.Convert(
                CreateTestCondition(cc, instrCur.Mnemonic),
                PrimitiveType.Bool,
                PrimitiveType.CreateWord(dst.DataType.BitSize)));
        }

        private void RewriteSetFlag(FlagGroupStorage flags, uint value)
        {
            state.SetFlagGroup(flags, Constant.Create(flags.DataType, value));
            var id = orw.FlagGroup(flags);
            m.Assign(id, value);
        }

        private void RewriteShxd(IntrinsicProcedure intrinsic)
        {
            var arg1 = SrcOp(0);
            var arg2 = SrcOp(1);
            var arg3 = SrcOp(2);
            m.Assign(arg1, m.Fn(intrinsic.MakeInstance(arg1.DataType), arg1, arg2, arg3));
        }

        public Expression MemIndexPtr(int iOp, RegisterStorage defaultSeg, Identifier indexRegister)
        {
            Expression ea = indexRegister;
            if (arch.ProcessorMode.PointerType.Domain == Domain.SegPointer)
            {
                var mem = (MemoryOperand) instrCur.Operands[iOp];
                var seg = mem.SegOverride != RegisterStorage.None
                    ? mem.SegOverride
                    : defaultSeg;
                ea = new SegmentedPointer(arch.ProcessorMode.PointerType, binder.EnsureRegister(seg), ea);
            }
            return ea;
        }

        public MemoryAccess MemIndex(int iOp, RegisterStorage defaultSeg, Identifier indexRegister)
        {
            var ea = MemIndexPtr(iOp, defaultSeg, indexRegister);
            return new MemoryAccess(MemoryStorage.GlobalMemory, ea, instrCur.DataWidth);
        }

        public MemoryAccess Mem(Expression defaultSegment, Expression effectiveAddress)
        {
            var ptrType = arch.ProcessorMode.PointerType;
            if (ptrType.Domain == Domain.SegPointer)
            {
                effectiveAddress = new SegmentedPointer(ptrType, defaultSegment, effectiveAddress);
            }
            return new MemoryAccess(MemoryStorage.GlobalMemory, effectiveAddress, instrCur.DataWidth);
        }

		public Identifier RegAl
		{
			get { return orw.AluRegister(Registers.rax, instrCur.DataWidth); }
		}

        public Identifier RegCx
        {
            get { return orw.AluRegister(Registers.rcx, instrCur.AddressWidth); }
        }

        public Identifier RegDi
		{
			get { return orw.AluRegister(Registers.rdi, instrCur.AddressWidth); }
		}

		public Identifier RegSi
		{
			get { return orw.AluRegister(Registers.rsi, instrCur.AddressWidth); }
		}

        /// <summary>
        /// Rewrites the current instruction as a string instruction.
        /// </summary>
        /// If a rep prefix is present, converts it into a loop: 
        /// <code>
        /// while ([e]cx != 0)
        ///		[string instruction]
        ///		--ecx;
        ///		if (zF)				; only cmps[b] and scas[b]
        ///			goto follow;
        /// follow: ...	
        /// </code>
        /// </summary>
        private void RewriteStringInstruction()
        {
            if (RewriteStringIntrinsic())
                return;
            var topOfLoop = instrCur.Address;
            Identifier? regCX = null;
            if (instrCur.RepPrefix != 0)
            {
                regCX = orw.AluRegister(Registers.rcx, instrCur.AddressWidth);
                m.BranchInMiddleOfInstruction(m.Eq0(regCX), instrCur.Address + instrCur.Length, InstrClass.ConditionalTransfer);
            }

            bool incSi = false;
            bool incDi = false;
            var incOperator = GetIncrementOperator();
            var ds = Registers.ds;
            var es = Registers.es;
            Identifier regDX;
            switch (instrCur.Mnemonic)
            {
            default:
                return;
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
                m.Assign(
                    binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!),
                    m.Cond(m.ISub(MemIndex(0, ds, RegSi), MemIndex(1, es, RegDi))));
                incSi = true;
                incDi = true;
                break;
            case Mnemonic.lods:
            case Mnemonic.lodsb:
                m.Assign(RegAl, MemIndex(1, ds, RegSi));
                incSi = true;
                break;
            case Mnemonic.movs:
            case Mnemonic.movsb:
                Identifier tmp = binder.CreateTemporary(instrCur.DataWidth);
                m.Assign(tmp, MemIndex(1, ds, RegSi));
                m.Assign(MemIndex(0, es, RegDi), tmp);
                incSi = true;
                incDi = true;
                break;
            case Mnemonic.ins:
            case Mnemonic.insb:
                regDX = binder.EnsureRegister(Registers.dx);
                m.Assign(RegAl, m.Fn(in_intrinsic.MakeInstance(instrCur.DataWidth), regDX));
                incDi = true;
                break;
            case Mnemonic.outs:
            case Mnemonic.outsb:
                regDX = binder.EnsureRegister(Registers.dx);
                m.SideEffect(m.Fn(out_intrinsic.MakeInstance(RegAl.DataType), regDX, RegAl));
                incSi = true;
                break;
            case Mnemonic.scas:
            case Mnemonic.scasb:
                m.Assign(
                    binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!),
                    m.Cond(m.ISub(RegAl, MemIndex(1, es, RegDi))));
                incDi = true;
                break;
            case Mnemonic.stos:
            case Mnemonic.stosb:
                m.Assign(MemIndex(0, es, RegDi), RegAl);
                incDi = true;
                break;
            }

            if (incSi)
            {
                m.Assign(RegSi, incOperator(RegSi, instrCur.DataWidth.Size));
            }

            if (incDi)
            {
                m.Assign(RegDi, incOperator(RegDi, instrCur.DataWidth.Size));
            }
            if (regCX is null)
                return;

            m.Assign(regCX, m.ISub(regCX, 1));

            switch (instrCur.Mnemonic)
            {
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
            case Mnemonic.scas:
            case Mnemonic.scasb:
                {
                    var cc = (instrCur.RepPrefix == 2)
                        ? ConditionCode.NE
                        : ConditionCode.EQ;
                    m.Branch(new TestCondition(cc, binder.EnsureFlagGroup(Registers.Z)).Invert(), topOfLoop, InstrClass.ConditionalTransfer);
                    break;
                }
            default:
                m.Goto(topOfLoop);
                break;
            }
        }

        private Func<Expression,long,Expression> GetIncrementOperator()
        {
            Constant direction = state.GetFlagGroup((uint)FlagM.DF);
            if (direction is null || !direction.IsValid)
                return m.IAddS;        // Better safe than sorry.
            if (direction.ToBoolean())
                return m.ISubS;
            else
                return m.IAddS;
        }

        private void RewriteSti()
        {
            m.SideEffect(m.Fn(sti_intrinsic));
        }

        private void RewriteTest()
        {
            var src = m.And(
                SrcOp(0),
                SrcOp(1));

            EmitCcInstr(src, Registers.SZP);
            m.Assign(binder.EnsureFlagGroup(Registers.O), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.C), 0);
        }

        private void RewriteUnaryOperator(UnaryOperator op, int iOp, MachineOperand opSrc, CopyFlags flags = 0)
        {
            EmitCopy(iOp, m.Unary(op, SrcOp(opSrc)), flags);
        }

        private void RewriteXadd()
        {
            var dst = SrcOp(0);
            var src = SrcOp(1);
            m.Assign(
                dst,
                m.Fn(xadd_intrinsic.MakeInstance(src.DataType), dst, src));
            EmitCcInstr(dst, X86Instruction.DefCc(instrCur.Mnemonic));
        }

        private void RewriteXgetbv()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            m.Assign(edx_eax,
                m.Fn(xgetbv_intrinsic, orw.AluRegister(Registers.ecx)));
        }

        private void RewriteXlat()
        {
            var al = orw.AluRegister(Registers.al);
            var bx = orw.AluRegister(Registers.rbx, instrCur.AddressWidth);
            var offsetType = PrimitiveType.Create(Domain.UnsignedInt, bx.DataType.BitSize); 
            m.Assign(
                al,
                Mem(
                    orw.AluRegister(Registers.ds),
                    m.IAdd(
                        bx,
                        m.Convert(al, PrimitiveType.UInt8, offsetType))));
        }

        private void RewriteXorp(bool isVex, IntrinsicProcedure intrinsic, PrimitiveType dtElem)
        {
            if (HasSameRegisterOperands(isVex))
            { // selfie!
                var dst = SrcOp(0);
                m.Assign(dst, Constant.Zero(dst.DataType));
                return;
            }
            RewritePackedBinop(isVex, intrinsic, dtElem);
        }

        private void RewriteXsetbv()
        {
            var edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            m.SideEffect(m.Fn(xsetbv_intrinsic, orw.AluRegister(Registers.ecx),edx_eax));
        }
    }
}
