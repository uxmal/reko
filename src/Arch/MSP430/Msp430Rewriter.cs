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

using System;
using System.Collections;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Rtl;
using System.Diagnostics;
using System.Linq;
using Reko.Core.Machine;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Services;
using Reko.Core.Memory;
using Reko.Core.Intrinsics;

namespace Reko.Arch.Msp430
{
    internal class Msp430Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly Msp430Architecture arch;
        private readonly ProcessorState state;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<Msp430Instruction> dasm;
        private Msp430Instruction instr;
        private RtlEmitter m;
        private InstrClass iclass;

        public Msp430Rewriter(Msp430Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = new Msp430Disassembler(arch, rdr).GetEnumerator();
            this.instr = null!;
            this.m = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                case Mnemonics.invalid: Invalid(); break;
                default:
                    EmitUnitTest();
                    Invalid();
                    break;
                case Mnemonics.addc: RewriteAdcSbc(m.IAdd); break;
                case Mnemonics.add: RewriteBinop(m.IAdd, Registers.VNZC); break;
                case Mnemonics.and: RewriteLogop(m.And); break;
                case Mnemonics.bic: RewriteBinop(Bis,  null); break;
                case Mnemonics.bis: RewriteBinop(m.Or, null); break;
                case Mnemonics.bit: RewriteBit(); break;
                case Mnemonics.br: RewriteBr(); break;
                case Mnemonics.call: RewriteCall(); break;
                case Mnemonics.cmp: RewriteCmp(); break;
                case Mnemonics.dadd: RewriteBinop(Dadd, Registers.NZC); break;

                case Mnemonics.jc:  RewriteBranch(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonics.jge: RewriteBranch(ConditionCode.GE, FlagM.VF|FlagM.NF); break;
                case Mnemonics.jl:  RewriteBranch(ConditionCode.LT, FlagM.VF | FlagM.NF); break;
                case Mnemonics.jmp: RewriteGoto(); break;
                case Mnemonics.jn:  RewriteBranch(ConditionCode.SG, FlagM.NF); break;
                case Mnemonics.jnc: RewriteBranch(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonics.jnz: RewriteBranch(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonics.jz:  RewriteBranch(ConditionCode.EQ, FlagM.ZF); break;

                case Mnemonics.mov: RewriteMov(); break;
                case Mnemonics.mova: RewriteMov(); break;
                case Mnemonics.popm: RewritePopm(); break;
                case Mnemonics.push: RewritePush(); break;
                case Mnemonics.pushm: RewritePushm(); break;
                case Mnemonics.ret: RewriteRet(); break;
                case Mnemonics.reti: RewriteReti(); break;
                case Mnemonics.rra: RewriteRra(); break;
                case Mnemonics.rrax: RewriteRrax(); break;
                case Mnemonics.rrc: RewriteRrc(); break;
                case Mnemonics.rrum: RewriteRrum(); break;
                case Mnemonics.sub: RewriteBinop(m.ISub, Registers.VNZC); break;
                case Mnemonics.subc: RewriteAdcSbc(m.ISub); break;
                case Mnemonics.swpb: RewriteSwpb(); break;
                case Mnemonics.sxt: RewriteSxt(); break;
                case Mnemonics.xor: RewriteBinop(m.Xor, Registers.VNZC); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Assign(FlagGroupStorage grf, Expression e)
        {
            m.Assign(binder.EnsureFlagGroup(grf), e);
        }

        private Expression Bis(Expression a, Expression b)
        {
            return m.And(a, m.Comp(b));
        }

        private Expression Dadd(Expression a, Expression b)
        {
            return host.Intrinsic("__dadd", true, a.DataType, a, b);
        }

        private Expression RewriteOp(MachineOperand op, DataType? dt = null)
        {
            dt ??= op.Width ?? instr.dataWidth!;
            switch (op)
            {
            case RegisterStorage rop:
                return binder.EnsureRegister(rop);
            case MemoryOperand mop:
                Expression ea;
                if (mop.Base != null)
                {
                    ea = binder.EnsureRegister(mop.Base);
                    if (mop.PostIncrement)
                    {
                        var tmp = binder.CreateTemporary(dt);
                        m.Assign(tmp, m.Mem(op.Width!, ea));
                        m.Assign(ea, m.IAdd(ea, m.Int16((short) dt.Size)));
                        return tmp;
                    }
                    else if (mop.Base == Registers.pc)
                    {
                        ea = instr.Address + mop.Offset;
                        var tmp = binder.CreateTemporary(dt);
                        m.Assign(tmp, m.Mem(dt, ea));
                        return tmp;
                    }
                    else if (mop.Offset != 0)
                    {
                        var tmp = binder.CreateTemporary(dt);
                        m.Assign(tmp, m.Mem(dt, m.IAdd(ea, m.Int16(mop.Offset))));
                        return tmp;
                    }
                    else
                    {
                        var tmp = binder.CreateTemporary(dt);
                        m.Assign(tmp, m.Mem(dt, ea));
                        return tmp;
                    }
                }
                else
                {
                    var tmp = binder.CreateTemporary(dt);
                    ea = Address.Ptr16((ushort) mop.Offset);
                    m.Assign(tmp, m.Mem(dt, ea));
                    return tmp;
                }
            case ImmediateOperand iop:
                return iop.Value;
            case AddressOperand aop:
                return aop.Address;
            }
            throw new NotImplementedException(op.ToString());
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression,Expression,Expression> fn)
        {
            switch (op)
            {
            case RegisterStorage rop:
                var dst = binder.EnsureRegister(rop);
                var ev = fn(dst, src);
                if (dst.Storage == Registers.sp && ev is Constant)
                {
                    m.SideEffect(host.Intrinsic("__set_stackpointer", true, VoidType.Instance, src));
                }
                else
                {
                    m.Assign(dst, ev);
                }
                return dst;
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mop:
                Expression ea;
                if (mop.Base != null)
                {
                    if (mop.Base == Registers.pc)
                    {
                        ea = instr.Address + mop.Offset;
                    }
                    else
                    {
                        ea = binder.EnsureRegister(mop.Base);
                        if (mop.Offset != 0)
                        {
                            ea = m.IAdd(ea, m.Int16(mop.Offset));
                        }
                    }
                }
                else
                {
                    ea = Address.Ptr16((ushort) mop.Offset);
                }
                var tmp = binder.CreateTemporary(mop.Width);
                m.Assign(tmp, m.Mem(tmp.DataType, ea));
                m.Assign(tmp, fn(tmp, src));
                m.Assign(m.Mem(tmp.DataType, ea.CloneExpression()), tmp);
                return tmp;
            case AddressOperand aop:
                var mem = m.Mem(op.Width, aop.Address);
                m.Assign(mem, fn(mem, src));
                return mem;
            }
            throw new NotImplementedException($"Unknown operand type {op.GetType().Name} ({op})");
        }

        private Expression RewriteMovDst(MachineOperand op, Expression src)
        {
            switch (op)
            {
            case RegisterStorage rop:
                var dst = binder.EnsureRegister(rop);
                if (dst.Storage == Registers.sp && src is Constant)
                {
                    m.SideEffect(host.Intrinsic("__set_stackpointer", true, VoidType.Instance, src));
                }
                else
                {
                    m.Assign(dst, src);
                }
                return dst;
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mop:
                Expression ea;
                if (mop.Base != null)
                {
                    if (mop.Base == Registers.pc)
                    {
                        ea = instr.Address + mop.Offset;
                    }
                    else
                    {
                        ea = binder.EnsureRegister(mop.Base);
                        if (mop.Offset != 0)
                        {
                            ea = m.IAdd(ea, m.Int16(mop.Offset));
                        }
                    }
                }
                else
                {
                    ea = Address.Ptr16((ushort) mop.Offset);
                }
                m.Assign(m.Mem(mop.Width, ea), src);
                return src;
            case AddressOperand aop:
                var mem = m.Mem(op.Width, aop.Address);
                m.Assign(mem, src);
                return src;
            }
            throw new NotImplementedException($"Unknown operand type {op.GetType().Name} ({op})");
        }

        private void EmitCc(Expression exp, FlagGroupStorage? grf)
        {
            if (grf != null)
            {
                Assign(grf, m.Cond(exp));
            }
        }

        private void RewriteAdcSbc(Func<Expression, Expression, Expression> fn)
        {
            var c = binder.EnsureFlagGroup(this.arch.GetFlagGroup(Registers.sr, (uint)FlagM.CF));
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, (a, b) => fn(fn(a, b), c));
            EmitCc(dst, Registers.VNZC);
        }

        private void RewriteBinop(Func<Expression,Expression,Expression> fn, FlagGroupStorage? vnzc)
        {
            var src = RewriteOp(instr.Operands[0]);
            if (instr.Operands[1] is RegisterStorage rop &&
                rop == Registers.pc)
            {
                if (instr.Operands[0] is MemoryOperand mop &&
                    mop.PostIncrement &&
                    mop.Base == Registers.sp)
                {
                    m.Return(2, 0);
                    return;
                }
            }
            var dst = RewriteDst(instr.Operands[1], src, fn);
            EmitCc(dst, vnzc);
        }

        private void RewriteBit()
        {
            var left = RewriteOp(instr.Operands[1]);
            var right = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(instr.Operands[0].Width);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint)(FlagM.NF | FlagM.ZF)));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint)FlagM.CF));
            var v = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint)FlagM.VF));
            m.Assign(tmp, m.And(left, right));
            m.Assign(grf, m.Cond(tmp));
            m.Assign(c, m.Test(ConditionCode.NE, tmp));
            m.Assign(v, Constant.Bool(false));
        }

        private void RewriteBr()
        {
            iclass = InstrClass.Transfer;
            m.Goto(RewriteOp(instr.Operands[0]));
        }

        private void RewriteBranch(ConditionCode cc, FlagM flags)
        {
            iclass = InstrClass.ConditionalTransfer;
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.sr, (uint)flags));
            m.Branch(m.Test(cc, grf), ((AddressOperand)instr.Operands[0]).Address, InstrClass.ConditionalTransfer);
        }

        private void RewriteCall()
        {
            iclass = InstrClass.Transfer | InstrClass.Call;
            instr.dataWidth = PrimitiveType.Word16;
            m.Call(RewriteOp(instr.Operands[0]), 2);
        }

        private void RewriteCmp()
        {
            var right = RewriteOp(instr.Operands[0]);
            var left = RewriteOp(instr.Operands[1]);
            EmitCc(m.ISub(left, right), Registers.VNZC);
        }

        private void RewriteGoto()
        {
            iclass = InstrClass.Transfer;
            m.Goto(RewriteOp(instr.Operands[0]));
        }

        private void RewriteLogop(Func<Expression, Expression, Expression> fn)
        {
            var src = RewriteOp(instr.Operands[0]);
            if (instr.Operands[1] is RegisterStorage rop &&
                rop == Registers.pc)
            {
                if (instr.Operands[0] is MemoryOperand mop &&
                    mop.PostIncrement &&
                    mop.Base == Registers.sp)
                {
                    m.Return(2, 0);
                    return;
                }
            }
            var dst = RewriteDst(instr.Operands[1], src, fn);
            EmitCc(dst, Registers.NZC);
            Assign(Registers.V, Constant.False());
        }

        private void RewriteMov()
        {
            var src = RewriteOp(instr.Operands[0]);
            if (instr.Operands[1] is RegisterStorage rop &&
                rop == Registers.pc)
            {
                if (instr.Operands[0] is MemoryOperand mop &&
                    mop.PostIncrement &&
                    mop.Base == Registers.sp)
                {
                    m.Return(2, 0);
                    return;
                }
            }
            RewriteMovDst(instr.Operands[1], src);
        }

        private void RewritePopm()
        {
            int c = ((ImmediateOperand)instr.Operands[0]).Value.ToInt32();
            int iReg = ((RegisterStorage)instr.Operands[1]).Number - c + 1;
            if (iReg < 0)
            {
                Invalid();
                return;
            }
            var sp = binder.EnsureRegister(Registers.sp);
            while (c > 0)
            {
                m.Assign(binder.EnsureRegister(Registers.GpRegisters[iReg]), m.Mem16(sp));
                m.Assign(sp, m.IAdd(sp, m.Int32(2)));
                ++iReg;
                --c;
            }
        }

        private void RewritePush()
        {
            var src = RewriteOp(instr.Operands[0]);
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISub(sp, m.Int32(2)));
            m.Assign(m.Mem16(sp), src);
        }

        private void RewritePushm()
        {
            int c = ((ImmediateOperand)instr.Operands[0]).Value.ToInt32();
            var sp = binder.EnsureRegister(Registers.sp);
            int iReg = ((RegisterStorage)instr.Operands[1]).Number;
            if (iReg < c)
            {
                Invalid();
                return;
            }
            while (c > 0)
            {
                m.Assign(sp, m.ISub(sp, m.Int32(2)));
                m.Assign(m.Mem16(sp), binder.EnsureRegister(Registers.GpRegisters[iReg]));
                --iReg;
                --c;
            }
        }

        private void RewriteRet()
        {
            m.Return(2, 0);
        }

        private void RewriteReti()
        {
            m.Return(2, 0);
        }

        private void RewriteRra()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) => m.Sar(a, m.Byte(1)));
            EmitCc(dst, Registers.NZC);
            Assign(Registers.V, Constant.False());
        }

        private void RewriteRrax()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) => m.Sar(a, Repeat()));
            EmitCc(dst, Registers.NZC);
            Assign(Registers.V, Constant.False());
        }

        private void RewriteRrc()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(
                instr.Operands[0],
                src,
                (a, b) => m.Fn(
                    CommonOps.RorC,
                    a,
                    m.Byte(1),
                    binder.EnsureFlagGroup(Registers.C)));
            EmitCc(dst, Registers.NZC);
            Assign(Registers.V, Constant.False());
        }

        private Expression Repeat()
        {
            if (instr.repeatImm != 0)
            {
                return Constant.Byte((byte)instr.repeatImm);
            }
            else
            {
                return binder.EnsureRegister(instr.repeatReg!);
            }
        }

        private void RewriteRrum()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, (a, b) => m.Shr(a, b));
            EmitCc(dst, Registers.NZC);
            Assign(Registers.V, Constant.False());
        }

        private void RewriteSwpb()
        {
            var src = RewriteOp(instr.Operands[0], PrimitiveType.Word16);
            RewriteMovDst(
                instr.Operands[0],
                host.Intrinsic(
                    "__swpb",
                    false,
                    PrimitiveType.Word16,
                    src));
        }

        private void RewriteSxt()
        {
            var src = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.Slice(PrimitiveType.Byte, src, 0));
            var dst = RewriteDst(instr.Operands[0], tmp, (a, b) => m.Convert(b, b.DataType, PrimitiveType.Int16));
            EmitCc(dst, Registers.NZC);
            Assign(Registers.V, Constant.False());

        }

        private void Invalid()
        {
            m.Invalid();
            iclass = InstrClass.Invalid;
        }

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Msp430Rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }
    }
}