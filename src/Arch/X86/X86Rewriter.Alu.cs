#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
            emitter.Assign(
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
            emitter.Assign(
                orw.AluRegister(Registers.ax),
                host.PseudoProcedure("__aad", PrimitiveType.Word16,
                    orw.AluRegister(Registers.ax)));
        }

        private void RewriteAam()
        {
            emitter.Assign(
                orw.AluRegister(Registers.ax),
                host.PseudoProcedure("__aam", PrimitiveType.Word16,
                    orw.AluRegister(Registers.al)));
        }

        private void RewriteAas()
        {
            emitter.Assign(
                orw.FlagGroup(FlagM.CF),
                host.PseudoProcedure("__aas", PrimitiveType.Bool,
                    orw.AluRegister(Registers.al),
                    orw.AluRegister(Registers.ah),
                            orw.AddrOf(orw.AluRegister(Registers.al)),
                            orw.AddrOf(orw.AluRegister(Registers.ah))));
        }

        private void RewriteCli()
        {
            emitter.SideEffect(host.PseudoProcedure("__cli", VoidType.Instance));
        }

        private void RewriteHlt()
        {
            var ppp = host.EnsurePseudoProcedure("__hlt", VoidType.Instance, 0);
            ppp.Characteristics = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            emitter.SideEffect(PseudoProc(ppp, VoidType.Instance));
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
                instrCur.op1,
                instrCur.op1.Width,
                SrcOp(instrCur.op1),
                SrcOp(instrCur.op2),
                CopyFlags.ForceBreak|CopyFlags.EmitCc);
        }

        public void RewriteAdcSbb(BinaryOperator opr)
        {
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = orw.FlagGroup(FlagM.CF);
            EmitCopy(
                instrCur.op1, 
                new BinaryExpression(
                    opr, 
                    instrCur.dataWidth,
                    new BinaryExpression(
                        opr,
                        instrCur.dataWidth, 
                        SrcOp(instrCur.op1),
                        SrcOp(instrCur.op2)),
                    c),
                CopyFlags.ForceBreak|CopyFlags.EmitCc);
        }

        private void RewriteArpl()
        {
            emitter.Assign(
                orw.FlagGroup(FlagM.ZF),
                host.PseudoProcedure("__arpl", PrimitiveType.Bool, 
                    SrcOp(instrCur.op1),
                    SrcOp(instrCur.op2),
                    orw.AddrOf(SrcOp(instrCur.op1))));
        }

        private void RewriteBound()
        {
            emitter.SideEffect(
                host.PseudoProcedure("__bound", VoidType.Instance,
                    SrcOp(instrCur.op1),
                    SrcOp(instrCur.op2)));
        }

        private void RewriteCpuid()
        {
            emitter.SideEffect(
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
            Identifier edx_eax = frame.EnsureSequence(
                Registers.edx,
                Registers.eax,
                PrimitiveType.Word64);
            emitter.Assign(edx_eax,
                host.PseudoProcedure("__xgetbv", 
                edx_eax.DataType,
                orw.AluRegister(Registers.ecx)));
        }

        private void RewriteRdtsc()
        {
            Identifier edx_eax = frame.EnsureSequence(
                Registers.edx,
                Registers.eax,
                PrimitiveType.Word64);
            emitter.Assign(edx_eax,
                host.PseudoProcedure("__rdtsc",
                edx_eax.DataType));
        }

        public void RewriteBinOp(BinaryOperator opr)
        {
            EmitBinOp(opr, instrCur.op1, instrCur.dataWidth, SrcOp(instrCur.op1), SrcOp(instrCur.op2), CopyFlags.ForceBreak|CopyFlags.EmitCc);
        }

        private void RewriteBsr()
        {
            Expression src = SrcOp(instrCur.op2);
            emitter.Assign(orw.FlagGroup(FlagM.ZF), emitter.Eq0(src));
            emitter.Assign(SrcOp(instrCur.op1), host.PseudoProcedure("__bsr", instrCur.op1.Width, src));
        }

        private void RewriteBt()
        {
		    emitter.Assign(
                orw.FlagGroup(FlagM.CF),
                host.PseudoProcedure("__bt", PrimitiveType.Bool, SrcOp(instrCur.op1), SrcOp(instrCur.op2)));
        }

        private void RewriteBtr()
        {
            emitter.Assign(
                orw.FlagGroup(FlagM.CF),                    // lhs
                host.PseudoProcedure(
                        "__btr", PrimitiveType.Bool,      // rhs
                        SrcOp(instrCur.op1),
                        SrcOp(instrCur.op2),
                        emitter.Out(instrCur.op1.Width, SrcOp(instrCur.op1))));
        }

        private void RewriteBts()
        {
            emitter.Assign(
                orw.FlagGroup(FlagM.CF),                    // lhs
                host.PseudoProcedure(
                        "__bts", PrimitiveType.Bool,      // rhs
                        SrcOp(instrCur.op1),
                        SrcOp(instrCur.op2),
                        emitter.Out(instrCur.op1.Width, SrcOp(instrCur.op1))));
        }

        public void RewriteBswap()
        {
            Identifier reg = (Identifier)orw.AluRegister(((RegisterOperand)instrCur.op1).Register);
            emitter.Assign(reg, host.PseudoProcedure("__bswap", (PrimitiveType)reg.DataType, reg));
        }

        public void RewriteCbw()
        {
            if (instrCur.dataWidth == PrimitiveType.Word32)
            {
                emitter.Assign(
                    orw.AluRegister(Registers.eax),
                    emitter.Cast(PrimitiveType.Int32, orw.AluRegister(Registers.ax)));
            }
            else
            {
                emitter.Assign(
                    orw.AluRegister(Registers.ax),
                    emitter.Cast(PrimitiveType.Int16, orw.AluRegister(Registers.al)));
            }
        }

        public void EmitBinOp(BinaryOperator binOp, MachineOperand dst, DataType dtDst, Expression left, Expression right, CopyFlags flags)
        {
            Constant c = right as Constant;
            if (c != null)
            {
                if (c.DataType == PrimitiveType.Byte && left.DataType != c.DataType)
                {
                    right = emitter.Const(left.DataType, c.ToInt32());
                }
            }
            //EmitCopy(dst, new BinaryExpression(binOp, dtDst, left, right), true, emitCc);
            EmitCopy(dst, new BinaryExpression(binOp, dtDst, left, right), flags);
        }

        /// <summary>
        /// Emits an assignment to a flag-group pseudoregister.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="defFlags">flags defined by the intel instruction</param>
        private void EmitCcInstr(Expression expr, FlagM defFlags)
        {
            emitter.Assign(orw.FlagGroup(defFlags), new ConditionOf(expr.CloneExpression()));
        }

        private void RewriteConditionalMove(ConditionCode cc, MachineOperand dst, MachineOperand src)
        {
            var opSrc = SrcOp(src);
            var opDst = SrcOp(dst);
            var test = CreateTestCondition(cc, instrCur.code);
            emitter.If(test, new RtlAssignment(opDst, opSrc));
        }

        private void RewriteCmp()
        {
            Expression op1 = SrcOp(instrCur.op1);
            Expression op2 = SrcOp(instrCur.op2, instrCur.op1.Width);
            emitter.Assign(
                orw.FlagGroup(X86Instruction.DefCc(Opcode.cmp)),
                new ConditionOf(emitter.ISub(op1, op2)));
        }

        private void RewriteCmpxchg()
        {
            var op1 = SrcOp(instrCur.op1);
            var op2 = SrcOp(instrCur.op2, instrCur.op1.Width);
            var acc = orw.AluRegister(Registers.eax, instrCur.dataWidth);
            var Z = orw.FlagGroup(FlagM.ZF);
            emitter.Assign(
                Z,
                host.PseudoProcedure("__cmpxchg",
                    PrimitiveType.Bool,
                    op1, op2, acc, emitter.Out(instrCur.dataWidth, acc)));
        }

        private void RewriteCwd()
        {
            if (instrCur.dataWidth == PrimitiveType.Word32)
            {
                Identifier edx_eax = frame.EnsureSequence(
                    Registers.edx,
                    Registers.eax,
                    PrimitiveType.Int64);
                emitter.Assign(
                    edx_eax, emitter.Cast(edx_eax.DataType, orw.AluRegister(Registers.eax)));
            }
            else
            {
                Identifier dx_ax = frame.EnsureSequence(
                    Registers.dx,
                    Registers.ax,
                    PrimitiveType.Int32);
                emitter.Assign(
                    dx_ax, emitter.Cast(dx_ax.DataType, orw.AluRegister(Registers.ax)));
            }
        }

        private void EmitDaaDas(string fnName)
        {
            emitter.Assign(orw.FlagGroup(FlagM.CF), host.PseudoProcedure(
                fnName,
                PrimitiveType.Bool,
                orw.AluRegister(Registers.al),
                orw.AddrOf(orw.AluRegister(Registers.al))));
        }

        private void RewriteDivide(BinaryOperator op, Domain domain)
        {
            if (instrCur.Operands != 1)
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
                regDividend = frame.EnsureSequence(regRemainder.Storage, regQuotient.Storage, PrimitiveType.Word32);
                break;
            case 4:
                regQuotient = orw.AluRegister(Registers.eax);
                regRemainder = orw.AluRegister(Registers.edx);
                regDividend = frame.EnsureSequence(regRemainder.Storage, regQuotient.Storage, PrimitiveType.Word64);
                break;
            case 8:
                regQuotient = orw.AluRegister(Registers.rax);
                regRemainder = orw.AluRegister(Registers.rdx);
                regDividend = frame.EnsureSequence(regRemainder.Storage, regQuotient.Storage, PrimitiveType.Word128);
                break;
            default:
                throw new ArgumentOutOfRangeException(string.Format("{0}-byte divisions not supported.", instrCur.dataWidth.Size));
            };
            PrimitiveType p = ((PrimitiveType)regRemainder.DataType).MaskDomain(domain);
            var tmp = frame.CreateTemporary(regDividend.DataType);
            emitter.Assign(tmp, regDividend);
            emitter.Assign(
                regRemainder, 
                new BinaryExpression(Operator.IMod, p, tmp, SrcOp(instrCur.op1)));
            emitter.Assign(
                regQuotient, 
                new BinaryExpression(op, p, tmp, SrcOp(instrCur.op1)));
            EmitCcInstr(regQuotient, X86Instruction.DefCc(instrCur.code));
        }

        private void RewriteEnter()
        {
            var bp = orw.AluRegister(arch.GetPart(Registers.ebp, instrCur.dataWidth));
            RewritePush(instrCur.dataWidth, bp);
            var sp = StackPointer();
            emitter.Assign(bp, sp);
            var cbExtraSavedBytes = 
                instrCur.dataWidth.Size * ((ImmediateOperand)instrCur.op2).Value.ToInt32() +
                ((ImmediateOperand)instrCur.op1).Value.ToInt32();
            if (cbExtraSavedBytes != 0)
            {
                emitter.Assign(sp, emitter.ISub(sp, cbExtraSavedBytes));
            }
        }

        private void RewriteExchange()
        {
            Identifier itmp = frame.CreateTemporary(instrCur.dataWidth);
            emitter.Assign(itmp, SrcOp(instrCur.op1));
            emitter.Assign(SrcOp(instrCur.op1), SrcOp(instrCur.op2));
            emitter.Assign(SrcOp(instrCur.op2), itmp);
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
                instrCur.op1,
                instrCur.op1.Width,
                SrcOp(instrCur.op1),
                emitter.Const(instrCur.op1.Width, amount),
                CopyFlags.ForceBreak|CopyFlags.EmitCc);
        }

        private void RewriteLock()
        {
            emitter.SideEffect(
                host.PseudoProcedure("__lock", VoidType.Instance));
        }

        private void RewriteLogical(BinaryOperator op)
        {
            if (instrCur.code == Opcode.and)
            {
                var r = instrCur.op1 as RegisterOperand;
                if (r != null && r.Register == arch.StackRegister &&
                    instrCur.op2 is ImmediateOperand)
                {
                    emitter.SideEffect(host.PseudoProcedure("__align", VoidType.Instance, SrcOp(instrCur.op1)));
                    return;
                }
            }

            EmitBinOp(
                op,
                instrCur.op1,
                instrCur.op1.Width,
                SrcOp(instrCur.op1),
                SrcOp(instrCur.op2),
                CopyFlags.ForceBreak);
            EmitCcInstr(SrcOp(instrCur.op1), (X86Instruction.DefCc(instrCur.code) & ~FlagM.CF));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        private void RewriteMultiply(BinaryOperator op, Domain resultDomain)
        {
            Expression product;
            switch (instrCur.Operands)
            {
            case 1:
                Identifier multiplicator;

                switch (instrCur.op1.Width.Size)
                {
                case 1:
                    multiplicator = orw.AluRegister(Registers.al);
                    product = orw.AluRegister(Registers.ax);
                    break;
                case 2:
                    multiplicator = orw.AluRegister(Registers.ax);
                    product = frame.EnsureSequence(
                        Registers.dx, multiplicator.Storage, PrimitiveType.Word32);
                    break;
                case 4:
                    multiplicator = orw.AluRegister(Registers.eax);
                    product = frame.EnsureSequence(
                        Registers.edx, multiplicator.Storage, PrimitiveType.Word64);
                    break;
                case 8:
                    multiplicator = orw.AluRegister(Registers.rax);
                    product = frame.EnsureSequence(
                        Registers.rdx, multiplicator.Storage, PrimitiveType.Word64);
                    break;
                default:
                    throw new ApplicationException(string.Format("Unexpected operand size: {0}", instrCur.op1.Width));
                };
                emitter.Assign(
                    product,
                    new BinaryExpression(
                        op, 
                        PrimitiveType.Create(resultDomain, product.DataType.Size),
                        SrcOp(instrCur.op1),
                        multiplicator));
                EmitCcInstr(product, X86Instruction.DefCc(instrCur.code));
                return;
            case 2:
                EmitBinOp(op, instrCur.op1, instrCur.op1.Width.MaskDomain(resultDomain), SrcOp(instrCur.op1), SrcOp(instrCur.op2), 
                    CopyFlags.ForceBreak|CopyFlags.EmitCc);
                return;
            case 3:
                EmitBinOp(op, instrCur.op1, instrCur.op1.Width.MaskDomain(resultDomain), SrcOp(instrCur.op2), SrcOp(instrCur.op3),
                    CopyFlags.ForceBreak | CopyFlags.EmitCc);
                return;
            default:
                throw new ArgumentException("Invalid number of operands");
            }
        }

        private void RewriteIn()
        {
            var ppName = "__in" + IntelSizeSuffix(instrCur.op1.Width.Size);
            emitter.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure(ppName, instrCur.op1.Width, SrcOp(instrCur.op2)));
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
            emitter.Assign(orw.AluRegister(Registers.ah), orw.FlagGroup(FlagM.FPUF));
        }

        public void RewriteLea()
        {
            Expression src;
            MemoryOperand mem = (MemoryOperand)instrCur.op2;
            if (mem.Base == RegisterStorage.None && mem.Index == RegisterStorage.None)
            {
                src = mem.Offset;
            }
            else
            {
                src = SrcOp(instrCur.op2);
                MemoryAccess load = src as MemoryAccess;
                if (load != null)
                {
                    src = load.EffectiveAddress;
                }
                else
                {
                    src = orw.AddrOf(src);
                }
            }
            emitter.Assign(SrcOp(instrCur.op1), src);
        }

        private void RewriteLeave()
        {
            var sp = orw.AluRegister(arch.StackRegister);
            var bp = orw.AluRegister(arch.GetPart(Registers.ebp, arch.StackRegister.DataType));
            emitter.Assign(sp, bp);
            emitter.Assign(bp, orw.StackAccess(sp, bp.DataType));
            emitter.Assign(sp, emitter.IAdd(sp, bp.DataType.Size));
        }

        private void RewriteLxs(RegisterStorage seg)
        {
            var reg = (RegisterOperand)instrCur.op1;
            MemoryOperand mem = (MemoryOperand)instrCur.op2;
            if (!mem.Offset.IsValid)
            {
                mem = new MemoryOperand(mem.Width, mem.Base, mem.Index, mem.Scale, Constant.Create(instrCur.addrWidth, 0));
            }

            var ass = emitter.Assign(
                frame.EnsureSequence(seg, reg.Register,
                PrimitiveType.Pointer32),
                SrcOp(mem, PrimitiveType.SegPtr32));
        }

        private void RewriteMov()
        {
            emitter.Assign(
                SrcOp(instrCur.op1),
                SrcOp(instrCur.op2, instrCur.op1.Width));
        }

        private void RewriteMovsx()
        {
            emitter.Assign(
                SrcOp(instrCur.op1),
                emitter.Cast(PrimitiveType.Create(Domain.SignedInt, instrCur.op1.Width.Size), SrcOp(instrCur.op2)));
        }

        private void RewriteMovzx()
        {
            emitter.Assign(
                SrcOp(instrCur.op1),
                emitter.Cast(instrCur.op1.Width, SrcOp(instrCur.op2)));
        }

        private void RewritePush()
        {
            RegisterOperand reg = instrCur.op1 as RegisterOperand;
            if (reg != null && reg.Register == Registers.cs)
            {
                if (dasm.Peek(1).code == Opcode.call &&
                    dasm.Peek(1).op1.Width == PrimitiveType.Word16)
                {
                    dasm.MoveNext();
                    emitter.Assign(StackPointer(), emitter.ISub(StackPointer(), reg.Register.DataType.Size));
                    RewriteCall(dasm.Current.op1, dasm.Current.op1.Width);
                    ric.Length = (byte) (ric.Length + dasm.Current.Length);
                    return;
                }

                if (
                    dasm.Peek(1).code == Opcode.push && (dasm.Peek(1).op1 is ImmediateOperand) &&
                    dasm.Peek(2).code == Opcode.push && (dasm.Peek(2).op1 is ImmediateOperand) &&
                    dasm.Peek(3).code == Opcode.jmp && (dasm.Peek(3).op1 is X86AddressOperand))
                {
                    // That's actually a far call, but the callee thinks its a near call.
                    RewriteCall(dasm.Peek(3).op1, instrCur.op1.Width);
                    dasm.MoveNext();
                    dasm.MoveNext();
                    dasm.MoveNext();
                    return;
                }
            }
            Debug.Assert(dasm.Current.dataWidth == PrimitiveType.Word16 || dasm.Current.dataWidth == PrimitiveType.Word32);
            RewritePush(dasm.Current.dataWidth, SrcOp(dasm.Current.op1));
        }

        private void RewritePush(RegisterStorage reg)
        {
            RewritePush(instrCur.dataWidth, orw.AluRegister(reg));
        }

        private void RewritePusha()
        {
            if (instrCur.dataWidth == PrimitiveType.Word16)
            {
                Identifier temp = frame.CreateTemporary(Registers.sp.DataType);
                emitter.Assign(temp, orw.AluRegister(Registers.sp));
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
                Identifier temp = frame.CreateTemporary(Registers.esp.DataType);
                emitter.Assign(temp, orw.AluRegister(Registers.esp));
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
                frame.EnsureFlagGroup(
                    Registers.eflags,
                    (uint)(FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.DF | FlagM.OF | FlagM.PF),
                    "SCZDOP", 
                    PrimitiveType.Byte));
        }

        private void RewriteNeg()
        {
            RewriteUnaryOperator(Operator.Neg, instrCur.op1, instrCur.op1, CopyFlags.ForceBreak|CopyFlags.EmitCc|CopyFlags.SetCfIf0);
        }

        private void RewriteNot()
        {
            RewriteUnaryOperator(Operator.Comp, instrCur.op1, instrCur.op1, CopyFlags.ForceBreak);
        }

        private void RewriteOut()
        {
            var ppp = host.EnsurePseudoProcedure("__out" + instrCur.op2.Width.Prefix, VoidType.Instance, 2);
            emitter.SideEffect(emitter.Fn(ppp, 
                SrcOp(instrCur.op1),
                SrcOp(instrCur.op2)));
        }
        private void RewritePop()
        {
            RewritePop(dasm.Current.op1, dasm.Current.op1.Width);
        }

        private void RewritePop(MachineOperand op, PrimitiveType width)
        {
            var sp = StackPointer();
            emitter.Assign(SrcOp(op), orw.StackAccess(sp, width));
            emitter.Assign(sp, emitter.IAdd(sp, width.Size));
        }

        private void RewritePop(Identifier dst, PrimitiveType width)
        {
            var sp = StackPointer();
            emitter.Assign(dst, orw.StackAccess(sp, width));
            emitter.Assign(sp, emitter.IAdd(sp, width.Size));
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
                emitter.Assign(sp, emitter.IAdd(sp, instrCur.dataWidth.Size));
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
                emitter.Assign(sp, emitter.IAdd(sp, instrCur.dataWidth.Size));
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
            emitter.Assign(frame.EnsureFlagGroup(
                    Registers.eflags,
                    (uint)(FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.DF | FlagM.OF | FlagM.PF),
                    "SCZDOP",
                    PrimitiveType.Byte),
                    orw.StackAccess(sp, width));
            emitter.Assign(sp, emitter.IAdd(sp, width.Size));
        }

        private void RewritePush(PrimitiveType dataWidth, Expression expr)
        {
            Constant c = expr as Constant;
            if (c != null && c.DataType != dataWidth)
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
                rhs = frame.CreateTemporary(sp.DataType);
                emitter.Assign(rhs, expr);
            }
            emitter.Assign(sp, emitter.ISub(sp, dataWidth.Size));
            emitter.Assign(orw.StackAccess(sp, dataWidth), rhs);
        }

        private void RewriteRotation(string operation, bool useCarry, bool left)
        {
            Identifier t = null;
            Expression sh;
            if (left)
            {
                sh = new BinaryExpression(
                    Operator.ISub,
                    instrCur.op2.Width,
                    Constant.Create(instrCur.op2.Width, instrCur.op1.Width.BitSize),
                    SrcOp(instrCur.op2));
            }
            else
            {
                sh = SrcOp(instrCur.op2);
            }
            sh = new BinaryExpression(
                Operator.Shl,
                instrCur.op1.Width,
                Constant.Create(instrCur.op1.Width, 1),
                sh);
            t = frame.CreateTemporary(PrimitiveType.Bool);
            emitter.Assign(t, emitter.Ne0(emitter.And(SrcOp(instrCur.op1), sh)));
            Expression p;
            if (useCarry)
            {
                p = host.PseudoProcedure(operation, instrCur.op1.Width, SrcOp(instrCur.op1), SrcOp(instrCur.op2), orw.FlagGroup(FlagM.CF));
            }
            else
            {
                p = host.PseudoProcedure(operation, instrCur.op1.Width, SrcOp(instrCur.op1), SrcOp(instrCur.op2));
            }
            emitter.Assign(SrcOp(instrCur.op1), p);
            emitter.Assign(orw.FlagGroup(FlagM.CF), t);
        }

        private void RewriteSet(ConditionCode cc)
        {
            emitter.Assign(SrcOp(instrCur.op1), CreateTestCondition(cc, instrCur.code));
        }

        private void RewriteSetFlag(FlagM flagM, Constant value)
        {
            var reg = arch.GetFlagGroup((uint) flagM);
            state.SetFlagGroup(reg, value);
            var id = orw.FlagGroup(flagM);
            emitter.Assign(id, value);
        }

        private void RewriteShxd(string name)
        {
            var ppp = host.EnsurePseudoProcedure(name, instrCur.op1.Width, 3);
            emitter.Assign(
                SrcOp(instrCur.op1), 
                emitter.Fn(
                    ppp,
                    SrcOp(instrCur.op1),
                    SrcOp(instrCur.op2),
                    SrcOp(instrCur.op3)));
        }

        public MemoryAccess MemDi()
		{
			if (arch.ProcessorMode == ProcessorMode.Real)
			{
				return new SegmentedAccess(MemoryIdentifier.GlobalMemory, orw.AluRegister(Registers.es), RegDi, instrCur.dataWidth);
			}
			else
				return new MemoryAccess(MemoryIdentifier.GlobalMemory, RegDi, instrCur.dataWidth);
		}

		public MemoryAccess MemSi()
		{
			if (arch.ProcessorMode == ProcessorMode.Real)
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
        /// <returns>False if the current instruction is not a string 
        /// instruction. This can happen when disassembling data where
        /// a "rep" prefix is followed by some other junk byte.
        /// </returns>
        private bool RewriteStringInstruction()
        {
            bool incSi = false;
            bool incDi = false;
            var incOperator = GetIncrementOperator();

            Identifier regDX;
            PseudoProcedure ppp;
            switch (instrCur.code)
            {
            default:
                return false;
            case Opcode.cmps:
            case Opcode.cmpsb:
                emitter.Assign(
                    orw.FlagGroup(X86Instruction.DefCc(Opcode.cmp)),
                    new ConditionOf(
                    new BinaryExpression(Operator.ISub, instrCur.dataWidth, MemSi(), MemDi())));
                incSi = true;
                incDi = true;
                break;
            case Opcode.lods:
            case Opcode.lodsb:
                emitter.Assign(RegAl, MemSi());
                incSi = true;
                break;
            case Opcode.movs:
            case Opcode.movsb:
                Identifier tmp = frame.CreateTemporary(instrCur.dataWidth);
                emitter.Assign(tmp, MemSi());
                emitter.Assign(MemDi(), tmp);
                incSi = true;
                incDi = true;
                break;
            case Opcode.ins:
            case Opcode.insb:
                regDX = orw.AluRegister(Registers.edx, instrCur.addrWidth);
                ppp = host.EnsurePseudoProcedure("__in", instrCur.dataWidth, 1);
                emitter.Assign(MemDi(), emitter.Fn(ppp, regDX));
                incDi = true;
                break;
            case Opcode.outs:
            case Opcode.outsb:
                regDX = orw.AluRegister(Registers.edx, instrCur.addrWidth);
                ppp = host.EnsurePseudoProcedure("__out" + RegAl.DataType.Prefix, VoidType.Instance, 2);
                emitter.SideEffect(emitter.Fn(ppp, regDX, RegAl));
                incSi = true;
                break;
            case Opcode.scas:
            case Opcode.scasb:
                emitter.Assign(
                    orw.FlagGroup(X86Instruction.DefCc(Opcode.cmp)),
                    new ConditionOf(
                    new BinaryExpression(Operator.ISub,
                    instrCur.dataWidth,
                    RegAl,
                    MemDi())));
                incDi = true;
                break;
            case Opcode.stos:
            case Opcode.stosb:
                emitter.Assign(MemDi(), RegAl);
                incDi = true;
                break;
            case Opcode.ret:
                // "AMD recommends to avoid the penalty by adding rep prefix instead of nop
                // because it saves decode bandwidth."
                RewriteRet();
                return true;
            }

            if (incSi)
            {
                emitter.Assign(RegSi,
                    new BinaryExpression(incOperator,
                    instrCur.addrWidth,
                    RegSi,
                    Constant.Create(instrCur.addrWidth, instrCur.dataWidth.Size)));
            }

            if (incDi)
            {
                emitter.Assign(RegDi,
                    new BinaryExpression(incOperator,
                    instrCur.addrWidth,
                    RegDi,
                    Constant.Create(instrCur.addrWidth, instrCur.dataWidth.Size)));
            }
            return true;
        }

        private BinaryOperator GetIncrementOperator()
        {
            Constant direction = state.GetFlagGroup((uint)FlagM.DF);
            if (direction == null || !direction.IsValid)
                return Operator.IAdd;        // Better safe than sorry.
            return direction.ToBoolean()
                ? Operator.ISub
                : Operator.IAdd;
        }

        private void RewriteSti()
        {
            emitter.SideEffect(host.PseudoProcedure("__sti", VoidType.Instance));
        }

        private void RewriteTest()
        {
            var src = new BinaryExpression(Operator.And,
                instrCur.op1.Width,
                SrcOp(instrCur.op1),
                                SrcOp(instrCur.op2));

            EmitCcInstr(src, (X86Instruction.DefCc(instrCur.code) & ~FlagM.CF));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        private void RewriteUnaryOperator(UnaryOperator op, MachineOperand opDst, MachineOperand opSrc, CopyFlags flags)
        {
            EmitCopy(opDst, new UnaryExpression(op, opSrc.Width, SrcOp(opSrc)), flags);
        }

        private void RewriteXadd()
        {
            var dst = SrcOp(instrCur.op1);
            var src = SrcOp(instrCur.op2);
            emitter.Assign(
                dst,
                host.PseudoProcedure("__xadd", instrCur.op1.Width, dst, src));
            EmitCcInstr(dst, X86Instruction.DefCc(instrCur.code));
        }

        private void RewriteXlat()
        {
            var al = orw.AluRegister(Registers.al);
            var bx = orw.AluRegister(Registers.ebx, instrCur.addrWidth);
            var offsetType = PrimitiveType.Create(Domain.UnsignedInt, bx.DataType.Size); 
            emitter.Assign(
                al,
                Mem(
                    orw.AluRegister(Registers.ds),
                    emitter.IAdd(
                        bx,
                        emitter.Cast(offsetType, al))));
        }

        private Identifier StackPointer()
        {
            return frame.EnsureRegister(arch.ProcessorMode.StackRegister);
        }
    }
}
