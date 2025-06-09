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
using System.Globalization;

namespace Reko.Arch.Msp430
{
    using Decoder = Decoder<Msp430Disassembler, Mnemonics, Msp430Instruction>;

    internal class Msp430Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly Msp430Architecture arch;
        private readonly ProcessorState state;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<Msp430Instruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private Msp430Instruction instr;
        private InstrClass iclass;

        public Msp430Rewriter(Msp430Architecture arch, Decoder[] decoders, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = new Msp430Disassembler(arch, decoders, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.instr = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                case Mnemonics.invalid: Invalid(); break;
                default:
                    EmitUnitTest();
                    Invalid();
                    break;
                case Mnemonics.addc: RewriteAdcSbc(m.IAdd); break;
                case Mnemonics.add: RewriteBinop(m.IAdd, arch.Registers.VNZC); break;
                case Mnemonics.and: RewriteLogop(m.And); break;
                case Mnemonics.bic: RewriteBinop(Bic,  null); break;
                case Mnemonics.bis: RewriteBinop(m.Or, null); break;
                case Mnemonics.bit: RewriteBit(); break;
                case Mnemonics.br: RewriteBr(); break;
                case Mnemonics.call: RewriteCall(); break;
                case Mnemonics.cmp: RewriteCmp(); break;
                case Mnemonics.dadd: RewriteBinop(Dadd, arch.Registers.NZC); break;
                case Mnemonics.dint: RewriteDint(); break;
                case Mnemonics.eint: RewriteEint(); break;

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
                case Mnemonics.sub: RewriteBinop(m.ISub, arch.Registers.VNZC); break;
                case Mnemonics.subc: RewriteAdcSbc(m.ISub); break;
                case Mnemonics.swpb: RewriteSwpb(); break;
                case Mnemonics.sxt: RewriteSxt(); break;
                case Mnemonics.xor: RewriteBinop(m.Xor, arch.Registers.VNZC); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                this.instrs.Clear();
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

        private Expression Bic(Expression a, Expression b)
        {
            return m.And(a, m.Comp(b));
        }

        private Expression Dadd(Expression a, Expression b)
        {
            var cy = binder.EnsureFlagGroup(arch.Registers.C);
            return m.Fn(dadd_intrinsic.MakeInstance(b.DataType), a, b, cy);
        }

        private Expression RewriteOp(MachineOperand op, DataType? dt = null)
        {
            dt ??= op.DataType ?? instr.dataWidth!;
            switch (op)
            {
            case RegisterStorage rop:
                var id = binder.EnsureRegister(rop); 
                if (dt.BitSize < id.DataType.BitSize)
                {
                    var t = binder.CreateTemporary(dt);
                    m.Assign(t, m.Slice(id, dt));
                    return t;
                }
                return id;
            case MemoryOperand mop:
                Expression ea;
                if (mop.Base is not null)
                {
                    ea = binder.EnsureRegister(mop.Base);
                    if (mop.PostIncrement)
                    {
                        var tmp = binder.CreateTemporary(dt);
                        m.Assign(tmp, m.Mem(op.DataType!, ea));
                        m.Assign(ea, m.IAddS(ea, dt.Size));
                        return tmp;
                    }
                    else if (mop.IsPcRelative)
                    {
                        ea = instr.Address + mop.Offset;
                        var tmp = binder.CreateTemporary(dt);
                        m.Assign(tmp, m.Mem(dt, ea));
                        return tmp;
                    }
                    else if (mop.Offset != 0)
                    {
                        var tmp = binder.CreateTemporary(dt);
                        m.Assign(tmp, m.Mem(dt, m.IAddS(ea, mop.Offset)));
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
            case Constant iop:
                return iop;
            case Address aop:
                return aop;
            }
            throw new NotImplementedException(op.ToString());
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression,Expression,Expression> fn)
        {
            switch (op)
            {
            case RegisterStorage rop:
                var dst = binder.EnsureRegister(rop);
                var src1 = dst;
                if (dst.DataType.BitSize > src.DataType.BitSize)
                {
                    var t = binder.CreateTemporary(src.DataType);
                    m.Assign(t, m.Slice(dst, t.DataType));
                    src1 = t;
                }
                var ev = fn(src1, src);
                if (rop == arch.Registers.sp && ev is Constant)
                {
                    m.SideEffect(m.Fn(set_stackpointer_intrinsic, src));
                    return dst;
                }
                else
                {
                    if (dst.DataType.BitSize > ev.DataType.BitSize)
                    {
                        var t = binder.CreateTemporary(ev.DataType);
                        m.Assign(t, ev);
                        m.Assign(dst, m.Convert(t, t.DataType, dst.DataType));
                        return t;
                    }
                    else
                    {
                        m.Assign(dst, ev);
                        return dst;
                    }
                }
            case Constant imm:
                return imm;
            case MemoryOperand mop:
                Expression ea;
                if (mop.Base is not null)
                {
                    if (mop.IsPcRelative)
                    {
                        ea = instr.Address + mop.Offset;
                    }
                    else
                    {
                        ea = binder.EnsureRegister(mop.Base);
                        if (mop.Offset != 0)
                        {
                            ea = m.IAddS(ea, mop.Offset);
                        }
                    }
                }
                else
                {
                    ea = Address.Ptr16((ushort) mop.Offset);
                }
                var tmp = binder.CreateTemporary(mop.DataType);
                m.Assign(tmp, m.Mem(tmp.DataType, ea));
                m.Assign(tmp, fn(tmp, src));
                m.Assign(m.Mem(tmp.DataType, ea.CloneExpression()), tmp);
                return tmp;
            case Address aop:
                var mem = m.Mem(op.DataType, aop);
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
                if (dst.Storage == arch.Registers.sp && src is Constant)
                {
                    m.SideEffect(m.Fn(set_stackpointer_intrinsic, src));
                }
                else
                {
                    src = MaybeExtend(src, dst.DataType);
                    m.Assign(dst, src);
                }
                return dst;
            case Constant imm:
                return imm;
            case MemoryOperand mop:
                Expression ea;
                if (mop.Base is not null)
                {
                    if (mop.Base == arch.Registers.pc)
                    {
                        ea = instr.Address + mop.Offset;
                    }
                    else
                    {
                        ea = binder.EnsureRegister(mop.Base);
                        if (mop.Offset != 0)
                        {
                            ea = m.IAddS(ea, mop.Offset);
                        }
                    }
                }
                else
                {
                    ea = Address.Ptr16((ushort) mop.Offset);
                }
                m.Assign(m.Mem(mop.DataType, ea), MaybeSlice(src, mop.DataType));
                return src;
            case Address aop:
                var mem = m.Mem(op.DataType, aop);
                m.Assign(mem, src);
                return src;
            }
            throw new NotImplementedException($"Unknown operand type {op.GetType().Name} ({op})");
        }



        private void EmitCc(Expression exp, FlagGroupStorage? grf)
        {
            if (grf is not null)
            {
                Assign(grf, m.Cond(exp));
            }
        }


        private Expression MaybeExtend(Expression exp, DataType dt)
        {
            if (dt.BitSize > exp.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.ExtendZ(exp, dt));
                return tmp;
            }
            return exp;
        }

        private Expression MaybeSlice(Expression exp, DataType dt)
        {
            if (dt.BitSize < exp.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(exp, dt));
                exp = tmp;
            }
            return exp;
        }

        private Constant WordAligned(DataType width)
        {
            return m.Int32(((width.Size + 1) / 2) * 2);
        }

        private void RewriteAdcSbc(Func<Expression, Expression, Expression> fn)
        {
            var c = binder.EnsureFlagGroup(this.arch.GetFlagGroup(arch.Registers.sr, (uint)FlagM.CF));
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, (a, b) => fn(fn(a, b), c));
            EmitCc(dst, arch.Registers.VNZC);
        }

        private void RewriteBinop(Func<Expression,Expression,Expression> fn, FlagGroupStorage? vnzc)
        {
            var src = RewriteOp(instr.Operands[0]);
            if (instr.Operands[1] is RegisterStorage rop &&
                rop == arch.Registers.pc)
            {
                if (instr.Operands[0] is MemoryOperand mop &&
                    mop.PostIncrement &&
                    mop.Base == arch.Registers.sp)
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
            var op0 = instr.Operands[0];
            var op1 = instr.Operands[1];
            var dtResult = (op0.DataType.BitSize < op1.DataType.BitSize ? op1 : op0).DataType;
            var left = MaybeExtend(RewriteOp(op1), dtResult);
            var right = MaybeExtend(RewriteOp(op0), dtResult);
            var tmp = binder.CreateTemporary(dtResult);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.Registers.sr, (uint)(FlagM.NF | FlagM.ZF)));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.Registers.sr, (uint)FlagM.CF));
            var v = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.Registers.sr, (uint)FlagM.VF));
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
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.Registers.sr, (uint)flags));
            m.Branch(m.Test(cc, grf), (Address)instr.Operands[0], InstrClass.ConditionalTransfer);
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
            EmitCc(m.ISub(left, right), arch.Registers.VNZC);
        }

        private void RewriteDint()
        {
            m.SideEffect(m.Fn(dint_intrinsic));
        }

        private void RewriteEint()
        {
            m.SideEffect(m.Fn(eint_intrinsic));
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
                rop == arch.Registers.pc)
            {
                if (instr.Operands[0] is MemoryOperand mop &&
                    mop.PostIncrement &&
                    mop.Base == arch.Registers.sp)
                {
                    m.Return(2, 0);
                    return;
                }
            }
            var dst = RewriteDst(instr.Operands[1], src, fn);
            EmitCc(dst, arch.Registers.NZC);
            Assign(arch.Registers.V, Constant.False());
        }

        private void RewriteMov()
        {
            var src = RewriteOp(instr.Operands[0]);
            if (instr.Operands[1] is RegisterStorage rop &&
                rop == arch.Registers.pc)
            {
                if (instr.Operands[0] is MemoryOperand mop &&
                    mop.PostIncrement &&
                    mop.Base == arch.Registers.sp)
                {
                    m.Return(2, 0);
                    return;
                }
            }
            RewriteMovDst(instr.Operands[1], src);
        }

        private void RewritePopm()
        {
            int c = ((Constant)instr.Operands[0]).ToInt32();
            int iReg = ((RegisterStorage)instr.Operands[1]).Number - c + 1;
            if (iReg < 0)
            {
                Invalid();
                return;
            }
            var sp = binder.EnsureRegister(arch.Registers.sp);
            while (c > 0)
            {
                var reg = arch.Registers.GpRegisters[iReg];
                m.Assign(binder.EnsureRegister(reg), MaybeExtend(m.Mem16(sp), reg.DataType));
                m.Assign(sp, m.IAdd(sp, WordAligned(reg.DataType)));
                ++iReg;
                --c;
            }
        }

        private void RewritePush()
        {
            var src = RewriteOp(instr.Operands[0]);
            var sp = binder.EnsureRegister(arch.Registers.sp);
            m.Assign(sp, m.ISub(sp, WordAligned(src.DataType)));
            m.Assign(m.Mem16(sp), src);
        }

        private void RewritePushm()
        {
            int c = ((Constant)instr.Operands[0]).ToInt32();
            var sp = binder.EnsureRegister(arch.Registers.sp);
            int iReg = ((RegisterStorage)instr.Operands[1]).Number;
            if (iReg < c)
            {
                Invalid();
                return;
            }
            while (c > 0)
            {
                var reg = arch.Registers.GpRegisters[iReg];
                m.Assign(sp, m.ISub(sp, WordAligned(reg.DataType)));
                m.Assign(m.Mem(reg.DataType, sp), binder.EnsureRegister(reg));
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
            EmitCc(dst, arch.Registers.NZC);
            Assign(arch.Registers.V, Constant.False());
        }

        private void RewriteRrax()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) => m.Sar(a, Repeat()));
            EmitCc(dst, arch.Registers.NZC);
            Assign(arch.Registers.V, Constant.False());
        }

        private void RewriteRrc()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteDst(
                instr.Operands[0],
                src,
                (a, b) => m.Fn(
                    CommonOps.RorC.MakeInstance(a.DataType, PrimitiveType.Byte),
                    a,
                    m.Byte(1),
                    binder.EnsureFlagGroup(arch.Registers.C)));
            EmitCc(dst, arch.Registers.NZC);
            Assign(arch.Registers.V, Constant.False());
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
            EmitCc(dst, arch.Registers.NZC);
            Assign(arch.Registers.V, Constant.False());
        }

        private void RewriteSwpb()
        {
            var src = RewriteOp(instr.Operands[0], PrimitiveType.Word16);
            RewriteMovDst(
                instr.Operands[0],
                m.Fn(swpb_intrinsic, src));
        }

        private void RewriteSxt()
        {
            var src = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.Slice(src, PrimitiveType.Byte));
            var dst = RewriteMovDst(
                instr.Operands[0],
                m.Convert(tmp, tmp.DataType, instr.Operands[0].DataType));
            EmitCc(dst, arch.Registers.NZC);
            Assign(arch.Registers.V, Constant.False());
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

        static readonly IntrinsicProcedure dadd_intrinsic = new IntrinsicBuilder("__dadd", false)
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Param(PrimitiveType.Bool)
            .Returns("T");
        static readonly IntrinsicProcedure dint_intrinsic = new IntrinsicBuilder("__disable_interrupts", true)
            .Void();
        static readonly IntrinsicProcedure eint_intrinsic = new IntrinsicBuilder("__enable_interrupts", true)
            .Void();
        static readonly IntrinsicProcedure set_stackpointer_intrinsic = new IntrinsicBuilder("__set_stackpointer", true)
            .Param(PrimitiveType.Ptr16)
            .Void();
        static readonly IntrinsicProcedure swpb_intrinsic = new IntrinsicBuilder("__swpb", false)
            .Param(PrimitiveType.Word16)
            .Returns(PrimitiveType.Word16);
    }
}