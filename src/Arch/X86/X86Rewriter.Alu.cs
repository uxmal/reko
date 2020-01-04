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
                orw.FlagGroup(FlagM.CF),
                host.PseudoProcedure("__aaa", PrimitiveType.Bool,
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
                host.PseudoProcedure("__aad", PrimitiveType.Word16,
                    orw.AluRegister(Registers.ax)));
        }

        private void RewriteAam()
        {
            m.Assign(
                orw.AluRegister(Registers.ax),
                host.PseudoProcedure("__aam", PrimitiveType.Word16,
                    orw.AluRegister(Registers.al)));
        }

        private void RewriteAas()
        {
            m.Assign(
                orw.FlagGroup(FlagM.CF),
                host.PseudoProcedure("__aas", PrimitiveType.Bool,
                    orw.AluRegister(Registers.al),
                    orw.AluRegister(Registers.ah),
                            orw.AddrOf(orw.AluRegister(Registers.al)),
                            orw.AddrOf(orw.AluRegister(Registers.ah))));
        }

        private void RewriteCli()
        {
            m.SideEffect(host.PseudoProcedure("__cli", VoidType.Instance));
        }

        private void RewriteHlt()
        {
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            var ppp = host.PseudoProcedure("__hlt", c, VoidType.Instance);
            m.SideEffect(ppp);
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
                instrCur.Operands[0],
                instrCur.Operands[0].Width,
                SrcOp(instrCur.Operands[0]),
                SrcOp(instrCur.Operands[1]),
                CopyFlags.EmitCc);
        }

        public void RewriteAdcSbb(Func<Expression,Expression,Expression> opr)
        {
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = orw.FlagGroup(FlagM.CF);
            EmitCopy(
                instrCur.Operands[0], 
                opr(
                    opr(
                        SrcOp(instrCur.Operands[0]),
                        SrcOp(instrCur.Operands[1])),
                    c),
                CopyFlags.EmitCc);
        }

        private void RewriteArpl()
        {
            m.Assign(
                orw.FlagGroup(FlagM.ZF),
                host.PseudoProcedure("__arpl", PrimitiveType.Bool, 
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1]),
                    orw.AddrOf(SrcOp(instrCur.Operands[0]))));
        }

        private void RewriteBound()
        {
            m.SideEffect(
                host.PseudoProcedure("__bound", VoidType.Instance,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1])));
        }

        private void RewriteCpuid()
        {
            m.SideEffect(
                host.PseudoProcedure("__cpuid", VoidType.Instance,
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
                host.PseudoProcedure("__xgetbv", 
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
                host.PseudoProcedure("__xsetbv",
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
                host.PseudoProcedure(
                    "__rdmsr",
                    edx_eax.DataType,
                    ecx));
        }

        private void RewriteRdpmc()
        {
            rtlc = InstrClass.System;
            Identifier edx_eax = binder.EnsureSequence(
                PrimitiveType.Word64,
                Registers.edx,
                Registers.eax);
            var ecx = binder.EnsureRegister(Registers.ecx);
            m.Assign(edx_eax,
                host.PseudoProcedure("__rdpmc",
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
                host.PseudoProcedure("__rdtsc",
                edx_eax.DataType));
        }

        public void RewriteBinOp(BinaryOperator opr)
        {
            EmitBinOp(opr, instrCur.Operands[0], instrCur.dataWidth, SrcOp(instrCur.Operands[0]), SrcOp(instrCur.Operands[1]), CopyFlags.EmitCc);
        }

        private void RewriteBsf()
        {
            Expression src = SrcOp(instrCur.Operands[1]);
            m.Assign(orw.FlagGroup(FlagM.ZF), m.Eq0(src));
            m.Assign(SrcOp(instrCur.Operands[0]), host.PseudoProcedure("__bsf", instrCur.Operands[0].Width, src));
        }

        private void RewriteBsr()
        {
            Expression src = SrcOp(instrCur.Operands[1]);
            m.Assign(orw.FlagGroup(FlagM.ZF), m.Eq0(src));
            m.Assign(SrcOp(instrCur.Operands[0]), host.PseudoProcedure("__bsr", instrCur.Operands[0].Width, src));
        }

        private void RewriteBt()
        {
		    m.Assign(
                orw.FlagGroup(FlagM.CF),
                host.PseudoProcedure("__bt", PrimitiveType.Bool, SrcOp(instrCur.Operands[0]), SrcOp(instrCur.Operands[1])));
        }

        private void RewriteBtc()
        {
            m.Assign(
                orw.FlagGroup(FlagM.CF),
                host.PseudoProcedure(
                    "__btc",
                    PrimitiveType.Bool,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1]),
                    m.Out(instrCur.Operands[0].Width, SrcOp(instrCur.Operands[0]))));
        }

        private void RewriteBtr()
        {
            m.Assign(
                orw.FlagGroup(FlagM.CF),                    // lhs
                host.PseudoProcedure(
                        "__btr", PrimitiveType.Bool,      // rhs
                        SrcOp(instrCur.Operands[0]),
                        SrcOp(instrCur.Operands[1]),
                        m.Out(instrCur.Operands[0].Width, SrcOp(instrCur.Operands[0]))));
        }

        private void RewriteBts()
        {
            m.Assign(
                orw.FlagGroup(FlagM.CF),                    // lhs
                host.PseudoProcedure(
                        "__bts", PrimitiveType.Bool,      // rhs
                        SrcOp(instrCur.Operands[0]),
                        SrcOp(instrCur.Operands[1]),
                        m.Out(instrCur.Operands[0].Width, SrcOp(instrCur.Operands[0]))));
        }

        public void RewriteBswap()
        {
            Identifier reg = (Identifier)orw.AluRegister(((RegisterOperand)instrCur.Operands[0]).Register);
            m.Assign(reg, host.PseudoProcedure("__bswap", (PrimitiveType)reg.DataType, reg));
        }

        public void RewriteCbw()
        {
            if (instrCur.dataWidth == PrimitiveType.Word32)
            {
                m.Assign(
                    orw.AluRegister(Registers.eax),
                    m.Cast(PrimitiveType.Int32, orw.AluRegister(Registers.ax)));
            }
            else
            {
                m.Assign(
                    orw.AluRegister(Registers.ax),
                    m.Cast(PrimitiveType.Int16, orw.AluRegister(Registers.al)));
            }
        }

        public void EmitBinOp(BinaryOperator binOp, MachineOperand dst, DataType dtDst, Expression left, Expression right, CopyFlags flags)
        {
            if (right is Constant c)
            {
                if (c.DataType == PrimitiveType.Byte && left.DataType != c.DataType)
                {
                    right = m.Const(left.DataType, c.ToInt32());
                }
            }
            EmitCopy(dst, new BinaryExpression(binOp, dtDst, left, right), flags);
        }

        /// <summary>
        /// Emits an assignment to a flag-group pseudoregister.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="defFlags">flags defined by the intel instruction</param>
        private void EmitCcInstr(Expression expr, FlagM defFlags)
        {
            m.Assign(orw.FlagGroup(defFlags), new ConditionOf(expr.CloneExpression()));
        }

        private void RewriteConditionalMove(ConditionCode cc, MachineOperand dst, MachineOperand src)
        {
            var test = CreateTestCondition(cc, instrCur.code).Invert();
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
            Expression op1 = SrcOp(instrCur.Operands[0]);
            Expression op2 = SrcOp(instrCur.Operands[1], instrCur.Operands[0].Width);
            m.Assign(
                orw.FlagGroup(X86Instruction.DefCc(Mnemonic.cmp)),
                new ConditionOf(m.ISub(op1, op2)));
        }

        private void RewriteCmpxchg()
        {
            var op1 = SrcOp(instrCur.Operands[0]);
            var op2 = SrcOp(instrCur.Operands[1], instrCur.Operands[0].Width);
            var acc = orw.AluRegister(Registers.rax, instrCur.Operands[0].Width);
            var Z = orw.FlagGroup(FlagM.ZF);
            m.Assign(
                Z,
                host.PseudoProcedure("__cmpxchg",
                    PrimitiveType.Bool,
                    op1, op2, acc, m.Out(instrCur.dataWidth, acc)));
        }

        private void RewriteCwd()
        {
            if (instrCur.dataWidth == PrimitiveType.Word32)
            {
                Identifier edx_eax = binder.EnsureSequence(
                    PrimitiveType.Int64,
                    Registers.edx,
                    Registers.eax);
                m.Assign(
                    edx_eax, m.Cast(edx_eax.DataType, orw.AluRegister(Registers.eax)));
            }
            else
            {
                Identifier dx_ax = binder.EnsureSequence(
                    PrimitiveType.Int32,
                    Registers.dx,
                    Registers.ax);
                m.Assign(
                    dx_ax, m.Cast(dx_ax.DataType, orw.AluRegister(Registers.ax)));
            }
        }

        private void EmitDaaDas(string fnName)
        {
            m.Assign(orw.FlagGroup(FlagM.CF), host.PseudoProcedure(
                fnName,
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
            m.Assign(
                regRemainder, 
                m.Cast(p, m.Mod(tmp, SrcOp(instrCur.Operands[0]))));
            m.Assign(
                regQuotient, 
                m.Cast(p, op(tmp, SrcOp(instrCur.Operands[0]))));
            EmitCcInstr(regQuotient, X86Instruction.DefCc(instrCur.code));
        }

        private void RewriteEnter()
        {
            var bp = orw.AluRegister(arch.GetSubregister(Registers.ebp, 0, instrCur.dataWidth.BitSize));
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
            m.Assign(itmp, SrcOp(instrCur.Operands[0]));
            m.Assign(SrcOp(instrCur.Operands[0]), SrcOp(instrCur.Operands[1]));
            m.Assign(SrcOp(instrCur.Operands[1]), itmp);
        }

        private void RewriteIncDec(int amount)
        {
            var op = Operator.IAdd;
            if (amount < 0)
            {
                op= Operator.ISub;
                amount = -amount;
            }

            EmitBinOp(op,
                instrCur.Operands[0],
                instrCur.Operands[0].Width,
                SrcOp(instrCur.Operands[0]),
                m.Const(instrCur.Operands[0].Width, amount),
                CopyFlags.EmitCc);
        }

        private void RewriteLock()
        {
            m.SideEffect(
                host.PseudoProcedure("__lock", VoidType.Instance));
        }

        private void RewriteLogical(BinaryOperator op)
        {
            if (instrCur.code == Mnemonic.and)
            {
                if (instrCur.Operands[0] is RegisterOperand r &&
                    r.Register == arch.StackRegister &&
                    instrCur.Operands[1] is ImmediateOperand)
                {
                    m.SideEffect(host.PseudoProcedure("__align", VoidType.Instance, SrcOp(instrCur.Operands[0])));
                    return;
                }
            }

            EmitBinOp(
                op,
                instrCur.Operands[0],
                instrCur.Operands[0].Width,
                SrcOp(instrCur.Operands[0]),
                SrcOp(instrCur.Operands[1]),
                0);
            EmitCcInstr(SrcOp(instrCur.Operands[0]), (X86Instruction.DefCc(instrCur.code) & ~FlagM.CF));
            m.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        private void RewriteMultiply(BinaryOperator op, Domain resultDomain)
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
                m.Assign(
                    product,
                    new BinaryExpression(
                        op, 
                        PrimitiveType.Create(resultDomain, product.DataType.BitSize),
                        SrcOp(instrCur.Operands[0]),
                        multiplicator));
                EmitCcInstr(product, X86Instruction.DefCc(instrCur.code));
                return;
            case 2:
                EmitBinOp(op, instrCur.Operands[0], instrCur.Operands[0].Width.MaskDomain(resultDomain), SrcOp(instrCur.Operands[0]), SrcOp(instrCur.Operands[1]), 
                    CopyFlags.EmitCc);
                return;
            case 3:
                EmitBinOp(op, instrCur.Operands[0], instrCur.Operands[0].Width.MaskDomain(resultDomain), SrcOp(instrCur.Operands[1]), SrcOp(instrCur.Operands[2]),
                    CopyFlags.EmitCc);
                return;
            default:
                throw new ArgumentException("Invalid number of operands");
            }
        }

        private void RewriteIn()
        {
            var ppName = "__in" + IntelSizeSuffix(instrCur.Operands[0].Width.Size);
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(ppName, instrCur.Operands[0].Width, SrcOp(instrCur.Operands[1])));
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
                src = mem.Offset;
            }
            else
            {
                src = SrcOp(instrCur.Operands[1]);
                if (src is MemoryAccess load)
                {
                    src = load.EffectiveAddress;
                }
                else
                {
                    src = orw.AddrOf(src);
                }
            }
            m.Assign(SrcOp(instrCur.Operands[0]), src);
        }

        private void RewriteLeave()
        {
            var sp = orw.AluRegister(arch.StackRegister);
            var bp = orw.AluRegister(arch.GetSubregister(Registers.rbp, 0, arch.StackRegister.DataType.BitSize));
            m.Assign(sp, bp);
            m.Assign(bp, orw.StackAccess(sp, bp.DataType));
            m.Assign(sp, m.IAddS(sp, bp.DataType.Size));
        }

        private void RewriteLxs(RegisterStorage seg)
        {
            var reg = (RegisterOperand)instrCur.Operands[0];
            MemoryOperand mem = (MemoryOperand)instrCur.Operands[1];
            if (!mem.Offset.IsValid)
            {
                mem = new MemoryOperand(mem.Width, mem.Base, mem.Index, mem.Scale, Constant.Create(instrCur.addrWidth, 0));
            }

            var ptr = PrimitiveType.Create(Domain.Pointer, seg.DataType.BitSize + reg.Width.BitSize);
            var segptr = PrimitiveType.Create(Domain.SegPointer, seg.DataType.BitSize + reg.Width.BitSize);
            m.Assign(
                binder.EnsureSequence(ptr, seg, reg.Register),
                SrcOp(mem, segptr));
        }

        private void RewriteMov()
        {
            var dst = SrcOp(instrCur.Operands[0]);
            var src = SrcOp(instrCur.Operands[1], instrCur.Operands[0].Width);
            if (dst is Identifier idDst)
            {
                this.AssignToRegister(idDst, src);
            }
            else
            {
                m.Assign(dst, src);
            }
        }

        private void RewriteMovssd(PrimitiveType dt)
        {
            var dst = SrcOp(instrCur.Operands[0]);
            var src = SrcOp(instrCur.Operands[1]);
            if (src is Identifier)
            {
                src = m.Cast(dt, src);
            }
            if (dst is Identifier)
            {
                m.Assign(dst, m.Dpb(dst, src, 0));
            }
            else
            {
                m.Assign(dst, src);
            }
        }

        private void RewriteMovsx()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                m.Cast(
                    PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize),
                    SrcOp(instrCur.Operands[1])));
        }

        private void RewriteMovzx()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                m.Cast(instrCur.Operands[0].Width, SrcOp(instrCur.Operands[1])));
        }

        private void RewritePush()
        {
            if (instrCur.Operands[0] is RegisterOperand reg && reg.Register == Registers.cs)
            {
                if (dasm.Peek(1).code == Mnemonic.call &&
                    dasm.Peek(1).Operands[0].Width == PrimitiveType.Word16)
                {
                    dasm.MoveNext();
                    m.Assign(StackPointer(), m.ISubS(StackPointer(), reg.Register.DataType.Size));
                    RewriteCall(dasm.Current.Operands[0], dasm.Current.Operands[0].Width);
                    this.len = (byte)(this.len + dasm.Current.Length);
                    return;
                }

                if (
                    dasm.Peek(1).code == Mnemonic.push && (dasm.Peek(1).Operands[0] is ImmediateOperand) &&
                    dasm.Peek(2).code == Mnemonic.push && (dasm.Peek(2).Operands[0] is ImmediateOperand) &&
                    dasm.Peek(3).code == Mnemonic.jmp && (dasm.Peek(3).Operands[0] is X86AddressOperand))
                {
                    // That's actually a far call, but the callee thinks its a near call.
                    RewriteCall(dasm.Peek(3).Operands[0], instrCur.Operands[0].Width);
                    dasm.MoveNext();
                    dasm.MoveNext();
                    dasm.MoveNext();
                    return;
                }
            }
            var imm = instrCur.Operands[0] as ImmediateOperand;
            var value = SrcOp(dasm.Current.Operands[0], arch.StackRegister.DataType);
            Debug.Assert(
                value.DataType.BitSize == 16 ||
                value.DataType.BitSize == 32 ||
                value.DataType.BitSize == 64,
                string.Format("Unexpected size {0}", dasm.Current.dataWidth));
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
            var port = SrcOp(instrCur.Operands[0]);
            var val = SrcOp(instrCur.Operands[1]);
            var suffix = NamingPolicy.Instance.Types.ShortPrefix(instrCur.Operands[1].Width);
            var ppp = host.PseudoProcedure(
                "__out" + suffix,
                VoidType.Instance,
                port, val);
            m.SideEffect(ppp);
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
            if (expr is Constant || (expr is Identifier && (expr as Identifier).Storage != arch.StackRegister))
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
            Identifier t = null;
            Expression sh;
            if (left)
            {
                sh = m.ISub(
                    Constant.Create(instrCur.Operands[1].Width, instrCur.Operands[0].Width.BitSize),
                    SrcOp(instrCur.Operands[1]));
            }
            else
            {
                sh = SrcOp(instrCur.Operands[1]);
            }
            sh = m.Shl(Constant.Create(instrCur.Operands[0].Width, 1), sh);
            t = binder.CreateTemporary(PrimitiveType.Bool);
            m.Assign(t, m.Ne0(m.And(SrcOp(instrCur.Operands[0]), sh)));
            Expression p;
            if (useCarry)
            {
                p = host.PseudoProcedure(operation, instrCur.Operands[0].Width, SrcOp(instrCur.Operands[0]), SrcOp(instrCur.Operands[1]), orw.FlagGroup(FlagM.CF));
            }
            else
            {
                p = host.PseudoProcedure(operation, instrCur.Operands[0].Width, SrcOp(instrCur.Operands[0]), SrcOp(instrCur.Operands[1]));
            }
            m.Assign(SrcOp(instrCur.Operands[0]), p);
            m.Assign(orw.FlagGroup(FlagM.CF), t);
        }

        private void RewriteSet(ConditionCode cc)
        {
            m.Assign(SrcOp(instrCur.Operands[0]), CreateTestCondition(cc, instrCur.code));
        }

        private void RewriteSetFlag(FlagM flagM, Constant value)
        {
            var reg = arch.GetFlagGroup(Registers.eflags, (uint) flagM);
            state.SetFlagGroup(reg, value);
            var id = orw.FlagGroup(flagM);
            m.Assign(id, value);
        }

        private void RewriteShxd(string name)
        {
            var arg1 = SrcOp(instrCur.Operands[0]);
            var arg2 = SrcOp(instrCur.Operands[1]);
            var arg3 = SrcOp(instrCur.Operands[2]);
            m.Assign(arg1, host.PseudoProcedure(name, arg1.DataType, arg1, arg2, arg3));
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
            Identifier regCX = null;
            if (instrCur.repPrefix != 0)
            {
                regCX = orw.AluRegister(Registers.rcx, instrCur.addrWidth);
                m.BranchInMiddleOfInstruction(m.Eq0(regCX), instrCur.Address + instrCur.Length, InstrClass.ConditionalTransfer);
            }

            bool incSi = false;
            bool incDi = false;
            var incOperator = GetIncrementOperator();

            Identifier regDX;
            switch (instrCur.code)
            {
            default:
                return;
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
                m.Assign(
                    orw.FlagGroup(X86Instruction.DefCc(Mnemonic.cmp)),
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
                m.Assign(MemDi(), host.PseudoProcedure("__in", instrCur.dataWidth, regDX));
                incDi = true;
                break;
            case Mnemonic.outs:
            case Mnemonic.outsb:
                regDX = binder.EnsureRegister(Registers.dx);
                var suffix = NamingPolicy.Instance.Types.ShortPrefix(RegAl.DataType); 
                m.SideEffect(host.PseudoProcedure("__out" + suffix, VoidType.Instance, regDX, RegAl));
                incSi = true;
                break;
            case Mnemonic.scas:
            case Mnemonic.scasb:
                m.Assign(
                    orw.FlagGroup(X86Instruction.DefCc(Mnemonic.cmp)),
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
            if (instrCur.repPrefix == 0)
                return;

            m.Assign(regCX, m.ISub(regCX, 1));

            switch (instrCur.code)
            {
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
            case Mnemonic.scas:
            case Mnemonic.scasb:
                {
                    var cc = (instrCur.repPrefix == 2)
                        ? ConditionCode.NE
                        : ConditionCode.EQ;
                    m.Branch(new TestCondition(cc, orw.FlagGroup(FlagM.ZF)).Invert(), topOfLoop, InstrClass.ConditionalTransfer);
                    break;
                }
            default:
                m.Goto(topOfLoop);
                break;
            }
        }

        private Func<Expression,int,Expression> GetIncrementOperator()
        {
            Constant direction = state.GetFlagGroup((uint)FlagM.DF);
            if (direction == null || !direction.IsValid)
                return m.IAdd;        // Better safe than sorry.
            if (direction.ToBoolean())
                return m.ISub;
            else
                return m.IAdd;
        }

        private void RewriteSti()
        {
            m.SideEffect(host.PseudoProcedure("__sti", VoidType.Instance));
        }

        private void RewriteTest()
        {
            var src = m.And(
                SrcOp(instrCur.Operands[0]),
                SrcOp(instrCur.Operands[1]));

            EmitCcInstr(src, (X86Instruction.DefCc(instrCur.code) & ~FlagM.CF));
            m.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        private void RewriteUnaryOperator(Func<Expression,Expression> op, MachineOperand opDst, MachineOperand opSrc, CopyFlags flags)
        {
            EmitCopy(opDst, op(SrcOp(opSrc)), flags);
        }

        private void RewriteXadd()
        {
            var dst = SrcOp(instrCur.Operands[0]);
            var src = SrcOp(instrCur.Operands[1]);
            m.Assign(
                dst,
                host.PseudoProcedure("__xadd", instrCur.Operands[0].Width, dst, src));
            EmitCcInstr(dst, X86Instruction.DefCc(instrCur.code));
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
                        m.Cast(offsetType, al))));
        }

        private Identifier StackPointer()
        {
            return binder.EnsureRegister(arch.ProcessorMode.StackRegister);
        }
    }
}
