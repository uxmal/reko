#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.X86
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
                host.Intrinsic("__aaa", true, PrimitiveType.Bool,
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
                host.Intrinsic("__aad", true, PrimitiveType.Word16,
                    orw.AluRegister(Registers.ax)));
        }

        private void RewriteAam()
        {
            m.Assign(
                orw.AluRegister(Registers.ax),
                host.Intrinsic("__aam", true, PrimitiveType.Word16,
                    orw.AluRegister(Registers.al)));
        }

        private void RewriteAas()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),
                host.Intrinsic("__aas", true, PrimitiveType.Bool,
                    orw.AluRegister(Registers.al),
                    orw.AluRegister(Registers.ah),
                            orw.AddrOf(orw.AluRegister(Registers.al)),
                            orw.AddrOf(orw.AluRegister(Registers.ah))));
        }

        public void RewriteAdcSbb(Func<Expression, Expression, Expression> opr)
        {
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(Registers.C);
            EmitCopy(
                instrCur.Operands[0],
                opr(
                    opr(
                        SrcOp(0),
                        SrcOp(1, instrCur.Operands[0].Width)),
                    c),
                CopyFlags.EmitCc);
        }

        private void RewriteAdcx(FlagGroupStorage carry)
        {
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

        private void RewriteCli()
        {
            m.SideEffect(host.Intrinsic("__cli", false, VoidType.Instance));
        }

        private void RewriteHlt()
        {
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            var intrinsic = host.Intrinsic("__hlt", false, c, VoidType.Instance);
            m.SideEffect(intrinsic, InstrClass.Terminates);
        }

        /// <summary>
        /// Doesn't handle the x86 idiom add ... adc => long add (and 
        /// sub ..sbc => long sub)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public void RewriteAddSub(Func<Expression,Expression,Expression> op)
        {
            EmitBinOp(
                op,
                instrCur.Operands[0],
                instrCur.Operands[0].Width,
                SrcOp(0),
                SrcOp(1),
                CopyFlags.EmitCc);
        }

        private void RewriteArpl()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.Z),
                host.Intrinsic("__arpl", false, PrimitiveType.Bool, 
                    SrcOp(0),
                    SrcOp(1),
                    orw.AddrOf(SrcOp(0))));
        }

        private void RewriteBound()
        {
            m.SideEffect(
                host.Intrinsic("__bound", false, VoidType.Instance,
                    SrcOp(0),
                    SrcOp(1)));
        }

        private void RewriteBextr()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic("__bextr", true, dst.DataType, src1, src2));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
        }

        private void RewriteBlsi()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic("__blsi", true, dst.DataType, src));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.S), m.Le0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.C), m.Eq0(src));
        }

        private void RewriteBlsmsk()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic("__blsmsk", true, dst.DataType, src));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.S), m.Le0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.C), m.Eq0(src));
        }
        private void RewriteBlsr()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic("__blsr", true, dst.DataType, src));
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.S), m.Le0(dst));
            m.Assign(binder.EnsureFlagGroup(Registers.C), m.Eq0(src));
        }

        private void RewriteBzhi()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic("__bzhi", true, dst.DataType, src1, src2));
        }

        private void RewriteCpuid()
        {
            m.SideEffect(
                host.Intrinsic("__cpuid", false, VoidType.Instance,
                    orw.AluRegister(Registers.eax),
                    orw.AluRegister(Registers.ecx),
                    orw.AddrOf(orw.AluRegister(Registers.eax)),
                    orw.AddrOf(orw.AluRegister(Registers.ebx)),
                    orw.AddrOf(orw.AluRegister(Registers.ecx)),
                    orw.AddrOf(orw.AluRegister(Registers.edx))));
        }

        private void RewriteXgetbv()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            m.Assign(edx_eax,
                host.Intrinsic("__xgetbv", 
                false,
                edx_eax.DataType,
                orw.AluRegister(Registers.ecx)));
        }

        private void RewriteXsetbv()
        {
            var edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            m.SideEffect(
                host.Intrinsic("__xsetbv",
                    false,
                    VoidType.Instance,
                    orw.AluRegister(Registers.ecx),
                    edx_eax));
        }

        private void RewriteRdmsr()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            var ecx = binder.EnsureRegister(Registers.ecx);
            m.Assign(
                edx_eax,
                host.Intrinsic(
                    "__rdmsr",
                    false,
                    edx_eax.DataType,
                    ecx));
        }

        private void RewriteRdpmc()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            var ecx = binder.EnsureRegister(Registers.ecx);
            m.Assign(edx_eax,
                host.Intrinsic("__rdpmc",
                false,
                edx_eax.DataType,
                ecx));
        }

        private void RewriteRdtsc()
        {
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            m.Assign(edx_eax,
                host.Intrinsic("__rdtsc",
                false,
                edx_eax.DataType));
        }

        public void RewriteBinOp(Func<Expression,Expression,Expression> opr)
        {
            Expression opL, opR;
            if (instrCur.Operands.Length == 3)
            {
                opL = SrcOp(1);
                opR = SrcOp(2);
            }
            else
            {
                opL = SrcOp(0);
                opR = SrcOp(1);
            }
            EmitBinOp(opr, instrCur.Operands[0], instrCur.dataWidth, opL, opR, CopyFlags.EmitCc);
        }

        private void RewriteBsf()
        {
            Expression src = SrcOp(1);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(src));
            m.Assign(SrcOp(0), host.Intrinsic("__bsf", true, instrCur.Operands[0].Width, src));
        }

        private void RewriteBsr()
        {
            Expression src = SrcOp(1);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Eq0(src));
            m.Assign(SrcOp(0), host.Intrinsic("__bsr", true, instrCur.Operands[0].Width, src));
        }

        private void RewriteBt()
        {
		    m.Assign(
                binder.EnsureFlagGroup(Registers.C),
                host.Intrinsic("__bt", true, PrimitiveType.Bool, SrcOp(0), SrcOp(1)));
        }

        private void RewriteBtc()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),
                host.Intrinsic(
                    "__btc",
                    false,
                    PrimitiveType.Bool,
                    SrcOp(0),
                    SrcOp(1),
                    m.Out(instrCur.Operands[0].Width, SrcOp(0))));
        }

        private void RewriteBtr()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),             // lhs
                host.Intrinsic(
                        "__btr", false, PrimitiveType.Bool,      // rhs
                        SrcOp(0),
                        SrcOp(1),
                        m.Out(instrCur.Operands[0].Width, SrcOp(0))));
        }

        private void RewriteBts()
        {
            m.Assign(
                binder.EnsureFlagGroup(Registers.C),    // lhs
                host.Intrinsic(
                        "__bts",
                        false, 
                        PrimitiveType.Bool,             // rhs
                        SrcOp(0),
                        SrcOp(1),
                        m.Out(instrCur.Operands[0].Width, SrcOp(0))));
        }

        public void RewriteBswap()
        {
            Identifier reg = orw.AluRegister(((RegisterOperand)instrCur.Operands[0]).Register);
            m.Assign(reg, host.Intrinsic("__bswap", true, (PrimitiveType)reg.DataType, reg));
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


        public void EmitBinOp(Func<Expression,Expression,Expression> binOp, MachineOperand dst, DataType dtDst, Expression left, Expression right, CopyFlags flags)
        {
            if (right is Constant c)
            {
                if (c.DataType.BitSize  < left.DataType.BitSize)
                {
                    right = m.Const(left.DataType, c.ToInt64());
                }
            }
            var bin = binOp(left, right);
            bin.DataType = dtDst;
            EmitCopy(dst, bin, flags);
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
            Expression op2 = SrcOp(1, instrCur.Operands[0].Width);
            m.Assign(
                binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!),
                new ConditionOf(m.ISub(op1, op2)));
        }

        private void RewriteCmpxchg()
        {
            var op1 = SrcOp(0);
            var op2 = SrcOp(1, instrCur.Operands[0].Width);
            var acc = orw.AluRegister(Registers.rax, instrCur.Operands[0].Width);
            var Z = binder.EnsureFlagGroup(Registers.Z);
            m.Assign(
                Z,
                host.Intrinsic(
                    "__cmpxchg",
                    false,
                    PrimitiveType.Bool,
                    op1, op2, acc, m.Out(instrCur.dataWidth, acc)));
        }

        private void RewriteCmpxchgNb(
            string intrinsicName,
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
                host.Intrinsic(
                    intrinsicName,
                    false,
                    Z.DataType,
                    op1, op2, acc, m.Out(instrCur.dataWidth, op1)));
        }

        private void EmitDaaDas(string fnName)
        {
            m.Assign(binder.EnsureFlagGroup(Registers.C), host.Intrinsic(
                fnName,
                true,
                PrimitiveType.Bool,
                orw.AluRegister(Registers.al),
                orw.AddrOf(orw.AluRegister(Registers.al))));
        }

        private void RewriteDivide(Func<Expression, Expression, Expression> op, Domain domain)
        {
            if (instrCur.Operands.Length != 1)
                throw new ArgumentOutOfRangeException("Intel DIV/IDIV instructions only take one operand");
            Identifier regDividend;
            Identifier regQuotient;
            Identifier regRemainder;

            switch (instrCur.dataWidth.Size)
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
                throw new ArgumentOutOfRangeException(string.Format("{0}-byte divisions not supported.", instrCur.dataWidth.Size));
            };
            PrimitiveType p = ((PrimitiveType)regRemainder.DataType).MaskDomain(domain);
            var tmp = binder.CreateTemporary(regDividend.DataType);
            m.Assign(tmp, regDividend);
            var r = m.Mod(tmp, SrcOp(0));
            var divisor = SrcOp(0);
            var q = op(tmp, divisor);
            m.Assign(
                regRemainder,
                m.Convert(r, r.DataType, p));
            m.Assign(
                regQuotient,
                m.Convert(q, q.DataType, p));
            EmitCcInstr(regQuotient, X86Instruction.DefCc(instrCur.Mnemonic));
        }

        private void RewriteEnter()
        {
            var bp = orw.AluRegister(Registers.ebp, instrCur.dataWidth)!;
            RewritePush(instrCur.dataWidth, bp);
            var sp = StackPointer();
            m.Assign(bp, sp);
            var cbExtraSavedBytes = 
                instrCur.dataWidth.Size * ((ImmediateOperand)instrCur.Operands[1]).Value.ToInt32() +
                ((ImmediateOperand)instrCur.Operands[0]).Value.ToInt32();
            if (cbExtraSavedBytes != 0)
            {
                m.Assign(sp, m.ISubS(sp, cbExtraSavedBytes));
            }
        }

        private void RewriteExchange()
        {
            Identifier itmp = binder.CreateTemporary(instrCur.Operands[0].Width);
            m.Assign(itmp, SrcOp(0));
            m.Assign(SrcOp(0), SrcOp(1));
            m.Assign(SrcOp(1), itmp);
        }

        private void RewriteIncDec(int amount)
        {
            Func<Expression,Expression,Expression> op = m.IAdd;
            if (amount < 0)
            {
                op= m.ISub;
                amount = -amount;
            }

            EmitBinOp(op,
                instrCur.Operands[0],
                instrCur.Operands[0].Width,
                SrcOp(0),
                m.Const(instrCur.Operands[0].Width, amount),
                CopyFlags.EmitCc);
        }

        private void RewriteLock()
        {
            m.SideEffect(
                host.Intrinsic("__lock", false, VoidType.Instance));
        }

        private void RewriteLogical(Func<Expression,Expression,Expression> op)
        {
            if (instrCur.Mnemonic == Mnemonic.and)
            {
                if (instrCur.Operands[0] is RegisterOperand r &&
                    r.Register == arch.StackRegister &&
                    instrCur.Operands[1] is ImmediateOperand)
                {
                    m.SideEffect(host.Intrinsic("__align", false, VoidType.Instance, SrcOp(0)));
                    return;
                }
            }
            if (instrCur.Operands.Length == 3)
            {
                EmitBinOp(
                    op,
                    instrCur.Operands[0],
                    instrCur.Operands[0].Width,
                    SrcOp(1),
                    SrcOp(2),
                    0);
            }
            else
            {
                EmitBinOp(
                    op,
                    instrCur.Operands[0],
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1),
                    0);
            }
            EmitCcInstr(SrcOp(0), Registers.SZO);
            m.Assign(binder.EnsureFlagGroup(Registers.C), Constant.False());
        }

        private void RewriteMultiply(Func<Expression,Expression,Expression> op, Domain resultDomain)
        {
            Expression product;
            switch (instrCur.Operands.Length)
            {
            case 1:
                Identifier multiplicator;

                switch (instrCur.Operands[0].Width.Size)
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
                    throw new ApplicationException(string.Format("Unexpected operand size: {0}", instrCur.Operands[0].Width));
                };
                var bin = op(SrcOp(0), multiplicator);
                bin.DataType = PrimitiveType.Create(resultDomain, product.DataType.BitSize);
                m.Assign(product, bin);
                EmitCcInstr(product, X86Instruction.DefCc(instrCur.Mnemonic));
                return;
            case 2:
                EmitBinOp(op, instrCur.Operands[0], instrCur.Operands[0].Width.MaskDomain(resultDomain), SrcOp(0), SrcOp(1), 
                    CopyFlags.EmitCc);
                return;
            case 3:
                EmitBinOp(op, instrCur.Operands[0], instrCur.Operands[0].Width.MaskDomain(resultDomain), SrcOp(1), SrcOp(2),
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
            var bits = instrCur.Operands[0].Width.BitSize + instrCur.Operands[1].Width.BitSize;
            var dt = PrimitiveType.Create(Domain.UnsignedInt, bits);
            var hi = ((RegisterOperand) instrCur.Operands[0]).Register;
            var lo = ((RegisterOperand) instrCur.Operands[1]).Register;
            var opDst = binder.EnsureSequence(dt, hi, lo);
            var mul = m.UMul(opL, opR);
            mul.DataType = dt;
            m.Assign(opDst, mul);
        }

        private void RewriteIn()
        {
            var ppName = "__in" + IntelSizeSuffix(instrCur.Operands[0].Width.Size);
            m.Assign(
                SrcOp(0),
                host.Intrinsic(ppName, false, instrCur.Operands[0].Width, SrcOp(1)));
        }

        private string IntelSizeSuffix(int size)
        {
            switch (size)
            {
            case 1: return "b";
            case 2: return "w";
            case 4: return "dw";
            case 8: return "qw";
            default: throw new ArgumentOutOfRangeException("Size is not 1,2,4 or 8");
            }
        }

        public void RewriteLahf()
        {
            m.Assign(orw.AluRegister(Registers.ah), orw.AluRegister(Registers.FPUF));
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
            var reg = (RegisterOperand)instrCur.Operands[0];
            MemoryOperand mem = (MemoryOperand)instrCur.Operands[1];
            if (mem.Offset is null)
            {
                mem = new MemoryOperand(mem.Width, mem.Base, mem.Index, mem.Scale, Constant.Create(instrCur.addrWidth, 0));
            }

            var ptr = PrimitiveType.Create(Domain.Pointer, seg.DataType.BitSize + reg.Width.BitSize);
            var segptr = PrimitiveType.Create(Domain.SegPointer, seg.DataType.BitSize + reg.Width.BitSize);
            m.Assign(
                binder.EnsureSequence(ptr, seg, reg.Register),
                orw.Transform(instrCur, mem, segptr));
        }

        private void RewriteLeadingTrailingZeros(string fnName)
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic(fnName, true, PrimitiveType.Create(Domain.SignedInt, dst.DataType.BitSize), src));
        }

        private void RewriteMov()
        {
            var dst = SrcOp(0);
            var src = SrcOp(1, instrCur.Operands[0].Width);
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
            var src = SrcOp(1, instrCur.Operands[0].Width);
            var dst = SrcOp(0);
            src = host.Intrinsic($"__movbe_{src.DataType.BitSize}", false, dst.DataType, src);
            EmitCopy(instrCur.Operands[0], src, 0);
        }

        private void RewriteMovssd(PrimitiveType dt)
        {
            var dst = SrcOp(0);
            var src = SrcOp(1);
            if (src is Identifier)
            {
                src = m.Slice(dt, src, 0);
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
                instrCur.Operands[0],
                m.Convert(src, src.DataType, instrCur.Operands[0].Width),
                0
            );
        }

        private void RewritePush()
        {
            if (instrCur.Operands[0] is RegisterOperand reg && reg.Register == Registers.cs)
            {
                // Is it a 'push cs;call near XXXX' sequence that simulates a far call?
                if (dasm.Peek(1).Mnemonic == Mnemonic.call &&
                    dasm.Peek(1).Operands[0].Width.BitSize == 16)
                {
                    dasm.MoveNext();
                    MachineOperand targetOperand = dasm.Current.Operands[0];

                    if (targetOperand is ImmediateOperand immOperand)
                    {
                        targetOperand = AddressOperand.Create(orw.ImmediateAsAddress(instrCur.Address, immOperand));
                    }
                    else
                    {
                        m.Assign(StackPointer(), m.ISubS(StackPointer(), reg.Register.DataType.Size));
                    }

                    RewriteCall(targetOperand, targetOperand.Width);
                    this.len = (byte)(this.len + dasm.Current.Length);
                    return;
                }

                if (
                    dasm.Peek(1).Mnemonic == Mnemonic.push && (dasm.Peek(1).Operands[0] is ImmediateOperand) &&
                    dasm.Peek(2).Mnemonic == Mnemonic.push && (dasm.Peek(2).Operands[0] is ImmediateOperand) &&
                    dasm.Peek(3).Mnemonic == Mnemonic.jmp && (dasm.Peek(3).Operands[0] is AddressOperand))
                {
                    // That's actually a far call, but the callee thinks its a near call.
                    RewriteCall(dasm.Peek(3).Operands[0], instrCur.Operands[0].Width);
                    dasm.MoveNext();
                    dasm.MoveNext();
                    dasm.MoveNext();
                    return;
                }
            }
            var value = SrcOp(0, instrCur.dataWidth);
            Debug.Assert(
                value.DataType.BitSize == 16 ||
                value.DataType.BitSize == 32 ||
                value.DataType.BitSize == 64,
                $"Unexpected size {dasm.Current.dataWidth}");
            RewritePush(PrimitiveType.CreateWord(value.DataType.BitSize), value);
        }

        private void RewritePush(RegisterStorage reg)
        {
            RewritePush(instrCur.dataWidth, orw.AluRegister(reg));
        }

        private void RewritePusha()
        {
            if (instrCur.dataWidth == PrimitiveType.Word16)
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

        private void RewritePushf()
        {
            RewritePush(
                dasm.Current.dataWidth,
                binder.EnsureFlagGroup(
                    Registers.eflags,
                    (uint)(FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.DF | FlagM.OF | FlagM.PF),
                    "SCZDOP", 
                    PrimitiveType.Byte));
        }

        private void RewriteNeg()
        {
            RewriteUnaryOperator(m.Neg, instrCur.Operands[0], instrCur.Operands[0], CopyFlags.EmitCc|CopyFlags.SetCfIf0);
        }

        private void RewriteNot()
        {
            RewriteUnaryOperator(m.Comp, instrCur.Operands[0], instrCur.Operands[0], 0);
        }

        private void RewriteOut()
        {
            var port = SrcOp(0);
            var val = SrcOp(1);
            var suffix = NamingPolicy.Instance.Types.ShortPrefix(instrCur.Operands[1].Width);
            var intrinsic = host.Intrinsic(
                "__out" + suffix,
                false,
                VoidType.Instance,
                port, val);
            m.SideEffect(intrinsic);
        }

        private void RewritePdep()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic("__pdep", false, dst.DataType, src1, src2));
        }

        private void RewritePext()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic("__pext", false, dst.DataType, src1, src2));
        }


        private void RewritePop()
        {
            RewritePop(dasm.Current.Operands[0], dasm.Current.Operands[0].Width);
        }

        private void RewritePop(MachineOperand op, PrimitiveType width)
        {
            var sp = StackPointer();
            m.Assign(SrcOp(op), orw.StackAccess(sp, width));
            m.Assign(sp, m.IAddS(sp, width.Size));
        }

        private void RewritePop(Identifier dst, PrimitiveType width)
        {
            var sp = StackPointer();
            m.Assign(dst, orw.StackAccess(sp, width));
            m.Assign(sp, m.IAdd(sp, width.Size));
        }

        private void EmitPop(RegisterStorage reg)
        {
            RewritePop(orw.AluRegister(reg), instrCur.dataWidth);
        }

        private void RewritePopa()
        {
            Debug.Assert(instrCur.dataWidth == PrimitiveType.Word16 || instrCur.dataWidth == PrimitiveType.Word32);
            var sp = StackPointer();
            if (instrCur.dataWidth == PrimitiveType.Word16)
            {
                EmitPop(Registers.di);
                EmitPop(Registers.si);
                EmitPop(Registers.bp);
                m.Assign(sp, m.IAdd(sp, instrCur.dataWidth.Size));
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
                m.Assign(sp, m.IAdd(sp, instrCur.dataWidth.Size));
                EmitPop(Registers.ebx);
                EmitPop(Registers.edx);
                EmitPop(Registers.ecx);
                EmitPop(Registers.eax);
            }
        }

        private void RewritePopf()
        {
            var width = instrCur.dataWidth;
            var sp = StackPointer();
            m.Assign(binder.EnsureFlagGroup(
                    Registers.eflags,
                    (uint)(FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.DF | FlagM.OF | FlagM.PF),
                    "SCZDOP",
                    PrimitiveType.Byte),
                    orw.StackAccess(sp, width));
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
            AssignToRegister(dst, host.Intrinsic("__popcnt", true, dt, src));
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

        private void RewriteRotation(string operation, bool useCarry, bool left)
        {
            Identifier? t;
            Expression sh;
            if (left)
            {
                sh = m.ISub(
                    Constant.Create(instrCur.Operands[1].Width, instrCur.Operands[0].Width.BitSize),
                    SrcOp(1));
            }
            else
            {
                sh = SrcOp(1);
            }
            sh = m.Shl(Constant.Create(instrCur.Operands[0].Width, 1), sh);
            t = binder.CreateTemporary(PrimitiveType.Bool);
            m.Assign(t, m.Ne0(m.And(SrcOp(0), sh)));
            Expression p;
            if (useCarry)
            {
                p = host.Intrinsic(operation, true, instrCur.Operands[0].Width, SrcOp(0), SrcOp(1), binder.EnsureFlagGroup(Registers.C));
            }
            else
            {
                p = host.Intrinsic(operation, true, instrCur.Operands[0].Width, SrcOp(0), SrcOp(1));
            }
            m.Assign(SrcOp(0), p);
            m.Assign(binder.EnsureFlagGroup(Registers.C), t);
        }

        private void RewriteRorx()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(dst, host.Intrinsic(IntrinsicProcedure.Ror, true, dst.DataType, src1, src2));
        }

        private void RewriteSet(ConditionCode cc)
        {
            var dst = SrcOp(0);
            m.Assign(dst, m.Convert(
                CreateTestCondition(cc, instrCur.Mnemonic),
                PrimitiveType.Bool,
                PrimitiveType.Create(Domain.SignedInt, dst.DataType.BitSize)));
        }

        private void RewriteSetFlag(FlagGroupStorage flags, Constant value)
        {
            state.SetFlagGroup(flags, value);
            var id = orw.FlagGroup(flags);
            m.Assign(id, value);
        }

        private void RewriteShxd(string name)
        {
            var arg1 = SrcOp(0);
            var arg2 = SrcOp(1);
            var arg3 = SrcOp(2);
            m.Assign(arg1, host.Intrinsic(name, true, arg1.DataType, arg1, arg2, arg3));
        }

        public MemoryAccess MemDi()
		{
			if (arch.ProcessorMode.PointerType == PrimitiveType.SegPtr32)
			{
				return new SegmentedAccess(MemoryIdentifier.GlobalMemory, orw.AluRegister(Registers.es), RegDi, instrCur.dataWidth);
			}
			else
				return new MemoryAccess(MemoryIdentifier.GlobalMemory, RegDi, instrCur.dataWidth);
		}

		public MemoryAccess MemSi()
		{
			if (arch.ProcessorMode.PointerType == PrimitiveType.SegPtr32)
			{
				return new SegmentedAccess(MemoryIdentifier.GlobalMemory, orw.AluRegister(Registers.ds), RegSi, instrCur.dataWidth);
			}
			else
				return new MemoryAccess(MemoryIdentifier.GlobalMemory, RegSi, instrCur.dataWidth);
		}

        public MemoryAccess Mem(Expression defaultSegment, Expression effectiveAddress)
        {
            if (arch.ProcessorMode != ProcessorMode.Protected32)
            {
                return new SegmentedAccess(MemoryIdentifier.GlobalMemory, defaultSegment, effectiveAddress, instrCur.dataWidth);
            }
            else
                return new MemoryAccess(MemoryIdentifier.GlobalMemory, effectiveAddress, instrCur.dataWidth);
        }

		public Identifier RegAl
		{
			get { return orw.AluRegister(Registers.rax, instrCur.dataWidth); }
		}

		public Identifier RegDi
		{
			get { return orw.AluRegister(Registers.rdi, instrCur.addrWidth); }
		}

		public Identifier RegSi
		{
			get { return orw.AluRegister(Registers.rsi, instrCur.addrWidth); }
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
            var topOfLoop = instrCur.Address;
            Identifier? regCX = null;
            if (instrCur.repPrefix != 0)
            {
                regCX = orw.AluRegister(Registers.rcx, instrCur.addrWidth);
                m.BranchInMiddleOfInstruction(m.Eq0(regCX), instrCur.Address + instrCur.Length, InstrClass.ConditionalTransfer);
            }

            bool incSi = false;
            bool incDi = false;
            var incOperator = GetIncrementOperator();

            Identifier regDX;
            switch (instrCur.Mnemonic)
            {
            default:
                return;
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
                m.Assign(
                    binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!),
                    m.Cond(m.ISub(MemSi(), MemDi())));
                incSi = true;
                incDi = true;
                break;
            case Mnemonic.lods:
            case Mnemonic.lodsb:
                m.Assign(RegAl, MemSi());
                incSi = true;
                break;
            case Mnemonic.movs:
            case Mnemonic.movsb:
                Identifier tmp = binder.CreateTemporary(instrCur.dataWidth);
                m.Assign(tmp, MemSi());
                m.Assign(MemDi(), tmp);
                incSi = true;
                incDi = true;
                break;
            case Mnemonic.ins:
            case Mnemonic.insb:
                regDX = binder.EnsureRegister(Registers.dx);
                m.Assign(MemDi(), host.Intrinsic("__in", false, instrCur.dataWidth, regDX));
                incDi = true;
                break;
            case Mnemonic.outs:
            case Mnemonic.outsb:
                regDX = binder.EnsureRegister(Registers.dx);
                var suffix = NamingPolicy.Instance.Types.ShortPrefix(RegAl.DataType); 
                m.SideEffect(host.Intrinsic("__out" + suffix, false, VoidType.Instance, regDX, RegAl));
                incSi = true;
                break;
            case Mnemonic.scas:
            case Mnemonic.scasb:
                m.Assign(
                    binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!),
                    m.Cond(m.ISub(RegAl, MemDi())));
                incDi = true;
                break;
            case Mnemonic.stos:
            case Mnemonic.stosb:
                m.Assign(MemDi(), RegAl);
                incDi = true;
                break;
            }

            if (incSi)
            {
                m.Assign(RegSi, incOperator(RegSi, instrCur.dataWidth.Size));
            }

            if (incDi)
            {
                m.Assign(RegDi, incOperator(RegDi, instrCur.dataWidth.Size));
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
                    var cc = (instrCur.repPrefix == 2)
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
            if (direction == null || !direction.IsValid)
                return m.IAddS;        // Better safe than sorry.
            if (direction.ToBoolean())
                return m.ISubS;
            else
                return m.IAddS;
        }

        private void RewriteSti()
        {
            m.SideEffect(host.Intrinsic("__sti", false, VoidType.Instance));
        }

        private void RewriteTest()
        {
            var src = m.And(
                SrcOp(0),
                SrcOp(1));

            EmitCcInstr(src, Registers.SZO);
            m.Assign(binder.EnsureFlagGroup(Registers.C), Constant.False());
        }

        private void RewriteUnaryOperator(Func<Expression,Expression> op, MachineOperand opDst, MachineOperand opSrc, CopyFlags flags)
        {
            EmitCopy(opDst, op(SrcOp(opSrc)), flags);
        }

        private void RewriteXadd()
        {
            var dst = SrcOp(0);
            var src = SrcOp(1);
            m.Assign(
                dst,
                host.Intrinsic("__xadd", false, instrCur.Operands[0].Width, dst, src));
            EmitCcInstr(dst, X86Instruction.DefCc(instrCur.Mnemonic));
        }

        private void RewriteXlat()
        {
            var al = orw.AluRegister(Registers.al);
            var bx = orw.AluRegister(Registers.rbx, instrCur.addrWidth);
            var offsetType = PrimitiveType.Create(Domain.UnsignedInt, bx.DataType.BitSize); 
            m.Assign(
                al,
                Mem(
                    orw.AluRegister(Registers.ds),
                    m.IAdd(
                        bx,
                        m.Convert(al, PrimitiveType.UInt8, offsetType))));
        }

        private Identifier StackPointer()
        {
            return binder.EnsureRegister(arch.ProcessorMode.StackRegister);
        }
    }
}
