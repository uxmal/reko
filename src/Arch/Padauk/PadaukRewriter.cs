#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Padauk
{
    public class PadaukRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly PadaukArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private readonly IEnumerator<PadaukInstruction> dasm;
        private PadaukInstruction instr;
        private InstrClass iclass;

        public PadaukRewriter(PadaukArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.dasm = new PadaukDisassembler(arch, rdr).GetEnumerator();
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest("");
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteAddSub(m.IAdd); break;
                case Mnemonic.addc: RewriteAddcSubc(m.IAddC, m.IAdd); break;
                case Mnemonic.and: RewriteAnd(); break;
                case Mnemonic.dec: RewriteIncDec(m.ISub); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.ceqsn: RewriteCmpSkipNext(m.Eq); break;
                case Mnemonic.clear: RewriteClear(); break;
                case Mnemonic.cneqsn: RewriteCmpSkipNext(m.Ne); break;
                case Mnemonic.dzsn: RewriteDzsn(); break;
                case Mnemonic.engint: RewriteEngint(); break;
                case Mnemonic.@goto: RewriteGoto(); break;
                case Mnemonic.idxm: RewriteMov(); break;
                case Mnemonic.inc: RewriteIncDec(m.IAdd); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.popaf: RewritePopaf(); break;
                case Mnemonic.pushaf: RewritePushaf(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.reti: RewriteReti(); break;
                case Mnemonic.set0: RewriteSetBit(m.False(), CommonOps.ClearBit); break;
                case Mnemonic.set1: RewriteSetBit(m.True(), CommonOps.SetBit); break;
                case Mnemonic.sl: RewriteShift(m.Shl); break;
                case Mnemonic.slc: RewriteShiftThroughCarry(Rolc); break;
                case Mnemonic.sr: RewriteShift(m.Shr); break;
                case Mnemonic.src: RewriteShiftThroughCarry(Rorc); break;
                case Mnemonic.stopexe: RewriteStopexe(); break;
                case Mnemonic.sub: RewriteAddSub(m.ISub); break;
                case Mnemonic.subc: RewriteAddcSubc(m.ISubC, m.ISub); break;
                case Mnemonic.swap: RewriteSwap(); break;
                case Mnemonic.t0sn: RewriteTsn(true); break;
                case Mnemonic.t1sn: RewriteTsn(false); break;
                case Mnemonic.xch: RewriteXch(); break;
                case Mnemonic.xor: RewriteXor(); break;

                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Expression EmitAssignment(int iop, Expression src, Func<Expression, Expression, Expression> func)
        {
            Expression dst;
            switch (instr.Operands[iop])
            {
            case RegisterStorage reg:
                dst = binder.EnsureRegister(reg);
                m.Assign(dst, func(dst, src));
                return dst;
            case MemoryOperand mem:
                if (mem.Bit.HasValue)
                {
                    break;
                }
                Expression e;
                Expression ea;
                if (mem.Indirect)
                {
                    ea = binder.CreateTemporary(PrimitiveType.Ptr16);
                    m.Assign(ea, m.Mem16(Address.Ptr16((ushort) mem.Offset)));
                }
                else
                {
                    ea = Address.Ptr16((ushort) mem.Offset);
                }
                e = func(m.Mem8(ea), src);
                if (e is not Constant && e is not Identifier)
                {
                    var tmp = binder.CreateTemporary(e.DataType);
                    m.Assign(tmp, e);
                    e = tmp;
                }
                m.Assign(m.Mem8(ea), e);
                return e;
            case PortOperand port:
                if (port.Bit.HasValue)
                {
                    break;
                }
                Expression portRead = m.Fn(in_intrinsic, m.Byte((byte) port.Port));
                e = func(portRead, src);
                m.SideEffect(m.Fn(out_intrinsic, m.Byte((byte) port.Port), e));
                return e;
            }
            EmitUnitTest("mem.bit");
            return InvalidConstant.Create(PrimitiveType.Byte);
        }

        private void EmitCc(FlagGroupStorage grf, Expression e)
        {
            var flags = binder.EnsureFlagGroup(grf);
            m.Assign(flags, m.Cond(grf.DataType, e));
        }

        private void EmitUnitTest(string comment)
        {
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("Pdk15Rw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, comment);
        }

        private static Expression Mov(Expression a, Expression b) => b;

        private Expression Op(int iop)
        {
            switch (instr.Operands[iop])
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                if (mem.Bit.HasValue)
                {
                    goto default;
                }
                if (mem.Indirect)
                {
                    var tmp = binder.CreateTemporary(PrimitiveType.Ptr16);
                    m.Assign(tmp, m.Mem16(Address.Ptr16((ushort) mem.Offset)));
                    return m.Mem8(tmp);
                }
                else
                {
                    return m.Mem8(Address.Ptr16((ushort) mem.Offset));
                }
            case PortOperand port:
                if (port.Bit.HasValue)
                {
                    goto default;
                }
                return m.Fn(in_intrinsic, m.Byte((byte) port.Port));
            default:
                EmitUnitTest($"Rewriting of operand type {instr.Operands[iop].GetType().Name} not implemnted yet.");
                m.Invalid();
                return InvalidConstant.Create(PrimitiveType.Byte);
            }
        }

        private void RewriteAddSub(Func<Expression, Expression, Expression> fn)
        {
            var dst = EmitAssignment(0, Op(1), fn);
            EmitCc(Registers.ZCAV, dst);
        }

        private void RewriteAddcSubc(
            Func<Expression, Expression, Expression, Expression> fn3,
            Func<Expression, Expression, Expression> fn2)
        {
            var C = binder.EnsureFlagGroup(Registers.C);
            Expression dst;
            if (instr.Operands.Length == 2)
            {
                dst = EmitAssignment(0, Op(1), (a, b) => fn3(a, b, C));
            }
            else
            {
                dst = EmitAssignment(0, C, fn2);
            }
            EmitCc(Registers.ZCAV, dst);
        }

        private void RewriteClear()
        {
            EmitAssignment(0, m.Byte(0), Mov);
        }

        private void RewriteCmpSkipNext(Func<Expression, Expression, Expression> fn)
        {
            var cond = fn(Op(0), Op(1));
            SkipIf(cond);
        }

        private void RewriteAnd()
        {
            var dst = EmitAssignment(0, Op(1), m.And);
            EmitCc(Registers.Z, dst);
        }

        private void RewriteCall()
        {
            m.Call(Op(0), 2);
        }

        private void RewriteDzsn()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.ISub(a, 1));
            SkipIf(m.Eq0(a));
        }

        private void RewriteEngint()
        {
            m.SideEffect(m.Fn(engint_intrinsic));
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn)
        {
            var dst = EmitAssignment(0, m.Byte(1), fn);
            EmitCc(Registers.ZCAV, dst);
        }

        private void RewriteGoto()
        {
            m.Goto(Op(0));
        }

        private void RewriteMov()
        {
            EmitAssignment(0, Op(1), Mov);
        }

        private void RewriteOr()
        {
            var dst = EmitAssignment(0, Op(1), m.Or);
            EmitCc(Registers.Z, dst);
        }

        private void RewritePopaf()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var a = binder.EnsureRegister(Registers.a);
            var f = binder.EnsureRegister(Registers.f);
            m.Assign(sp, m.ISubS(sp, 1));
            m.Assign(f, m.Mem8(sp));
            m.Assign(sp, m.ISubS(sp, 1));
            m.Assign(a, m.Mem8(sp));
        }

        private void RewritePushaf()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var a = binder.EnsureRegister(Registers.a);
            var f = binder.EnsureRegister(Registers.f);
            m.Assign(m.Mem8(sp), a);
            m.Assign(sp, m.IAddS(sp, 1));
            m.Assign(m.Mem8(sp), f);
            m.Assign(sp, m.IAddS(sp, 1));
        }

        private void RewriteRet()
        {
            if (instr.Operands.Length == 1)
            {
                var a = binder.EnsureRegister(Registers.a);
                m.Assign(a, Op(0));
            }
            m.Return(2, 0);
        }

        //$TODO: unclear if there are any other effects of a RETI
        private void RewriteReti()
        {
            m.Return(2, 0);
        }

        private void RewriteSetBit(Constant value, IntrinsicProcedure intrinsic)
        {
            if (instr.Operands[0] is PortOperand port)
            {
                m.SideEffect(m.Fn(
                    out_bit_intrinsic,
                    Constant.Byte((byte) port.Port),
                    Constant.Byte((byte) port.Bit!.Value),
                    value));
            }
            else
            {
                var mem = (MemoryOperand) instr.Operands[0];
                var tmp = binder.CreateTemporary(PrimitiveType.Byte);
                var ea = m.Ptr16((ushort) mem.Offset);
                m.Assign(tmp, m.Mem8(ea));
                m.Assign(tmp, m.Fn(intrinsic, tmp, m.Byte((byte) mem.Bit!.Value)));
                m.Assign(m.Mem8(ea), tmp);
            }
        }

        private void RewriteShift(Func<Expression, Expression, Expression> shift)
        {
            var dst = EmitAssignment(0, m.Byte(1), shift);
            EmitCc(Registers.C, dst);
        }

        private void RewriteStopexe()
        {
            m.SideEffect(m.Fn(stopexe_instrinsic));
        }

        private Expression Rolc(Expression a, Expression b)
        {
            var C = binder.EnsureFlagGroup(Registers.C);
            return m.Fn(
                CommonOps.RolC.MakeInstance(
                    a.DataType,
                    b.DataType),
                a, b, C);
        }

        private Expression Rorc(Expression a, Expression b)
        {
            var C = binder.EnsureFlagGroup(Registers.C);
            return m.Fn(
                CommonOps.RorC.MakeInstance(
                    a.DataType,
                    b.DataType),
                a, b, C);
        }

        private void RewriteShiftThroughCarry(Func<Expression,Expression,Expression> rotation)
        {
            var dst = EmitAssignment(0, m.Byte(1), rotation);
            EmitCc(Registers.C, dst);
        }

        private void RewriteSwap()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.Fn(swap_intrinsic, a));
        }

        private void RewriteTsn(bool invert)
        {
            if (instr.Operands[0] is PortOperand port)
            {
                Expression cond = binder.CreateTemporary(PrimitiveType.Bool);
                m.Assign(cond, m.Fn(
                    in_bit_intrinsic,
                    Constant.Byte((byte) port.Port),
                    Constant.Byte((byte) port.Bit!.Value)));
                if (invert)
                {
                    cond = m.Not(cond);
                }
                SkipIf(cond);
            }
            else
            {
                EmitUnitTest("Tsn mem");
            }
        }

        private void RewriteXch()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(tmp, Op(0));
            EmitAssignment(0, a, (a, b) => b);
            m.Assign(a, tmp);
        }

        private void RewriteXor()
        {
            var dst = EmitAssignment(0, Op(1), m.Xor);
            EmitCc(Registers.Z, dst);
        }

        private void SkipIf(Expression cond)
        {
            // All instructions are one word, this skips the following word.
            m.Branch(cond, instr.Address + 2);
        }


        private static readonly IntrinsicProcedure engint_intrinsic = new IntrinsicBuilder("__enable_global_interrupts", true)
            .Void();

        private static readonly IntrinsicProcedure in_intrinsic = new IntrinsicBuilder("__in", true)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure in_bit_intrinsic = new IntrinsicBuilder("__in_bit", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Bool);

        private static readonly IntrinsicProcedure out_intrinsic = new IntrinsicBuilder("__out", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure out_bit_intrinsic = new IntrinsicBuilder("__out_bit", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Bool)
        .Void();

        private static readonly IntrinsicProcedure stopexe_instrinsic = new IntrinsicBuilder("__stopexe", true)
            .Void();

        private static readonly IntrinsicProcedure swap_intrinsic = new IntrinsicBuilder("__swap_nybbles", false)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
    }
}