#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tms7000
{
    public class Tms7000Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Tms7000Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly IRewriterHost host;
        private readonly IStorageBinder binder;
        private readonly IEnumerator<Tms7000Instruction> dasm;
        private Tms7000Instruction instr;
        private InstrClass rtlc;
        private RtlEmitter m;

        public Tms7000Rewriter(Tms7000Architecture arch, EndianImageReader rdr, Tms7000State state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.host = host;
            this.binder = binder;
            this.dasm = new Tms7000Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.rtlc = InstrClass.Linear;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                switch (instr.Opcode)
                {
                default:
                    host.Error(instr.Address, "Rewriting x86 opcode '{0}' is not supported yet.", instr);
                    rtlc = InstrClass.Invalid;
                    break;
                case Opcode.adc: RewriteAdcSbb(m.IAdd); break;
                case Opcode.add: RewriteArithmetic(m.IAdd); break;
                case Opcode.and: RewriteLogical(m.And); break;
                case Opcode.andp: RewriteLogical(m.And); break;
                case Opcode.btjo: RewriteBtj(a => a); break;
                case Opcode.btjop: RewriteBtj(a => a); break;
                case Opcode.btjz: RewriteBtj(m.Comp); break;
                case Opcode.btjzp: RewriteBtj(m.Comp); break;
                case Opcode.br: RewriteBr(); break;
                case Opcode.call: RewriteCall(); break;
                case Opcode.clr: RewriteClr(); break;
                case Opcode.tsta: RewriteTst(arch.a); break;
                case Opcode.dac: RewriteDacDsb("__dac"); break;
                case Opcode.dec: RewriteIncDec(m.ISub); break;
                case Opcode.decd: RewriteIncdDecd(m.ISub); break;
                case Opcode.dint: RewriteDint(); break;
                case Opcode.djnz: RewriteDjnz(); break;
                case Opcode.dsb: RewriteDacDsb("__dsb"); break;
                case Opcode.eint: RewriteEint(); break;
                case Opcode.idle: RewriteIdle(); break;
                case Opcode.inc: RewriteIncDec(m.IAdd); break;
                case Opcode.inv: RewriteInv(); break;
                case Opcode.jmp: RewriteJmp(); break;
                case Opcode.jeq: RewriteJcc(ConditionCode.EQ, FlagM.ZF); break;
                case Opcode.jge: RewriteJcc(ConditionCode.GE, FlagM.NZ); break;
                case Opcode.jgt: RewriteJcc(ConditionCode.GT, FlagM.NZ); break;
                case Opcode.jhs: RewriteJcc(ConditionCode.UGE, FlagM.CF); break;
                case Opcode.jl: RewriteJcc(ConditionCode.ULT, FlagM.CF); break;
                case Opcode.jne: RewriteJcc(ConditionCode.EQ, FlagM.ZF); break;
                case Opcode.lda: RewriteLda(); break;
                case Opcode.ldsp: RewriteLdsp(); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.movd: RewriteMovd(); break;
                case Opcode.movp: RewriteMov(); break;
                case Opcode.mpy: RewriteMpy(); break; 
                case Opcode.nop: m.Nop(); break;
                case Opcode.or: RewriteLogical(m.Or); break;
                case Opcode.orp: RewriteLogical(m.Or); break;
                case Opcode.pop: RewritePop(); break;
                case Opcode.push: RewritePush(); break;
                case Opcode.reti: RewriteReti(); break;
                case Opcode.rets: RewriteRets(); break;
                case Opcode.rl: RewriteRotate(PseudoProcedure.Rol); break;
                case Opcode.rlc: RewriteRotateC(PseudoProcedure.RolC); break;
                case Opcode.rr: RewriteRotate(PseudoProcedure.Ror); break;
                case Opcode.rrc: RewriteRotateC(PseudoProcedure.RorC); break;
                case Opcode.sbb: RewriteAdcSbb(m.ISub); break;
                case Opcode.setc: RewriteSetc(); break;
                case Opcode.sta: RewriteSta(); break;
                case Opcode.stsp: RewriteStsp(); break;
                case Opcode.sub: RewriteArithmetic(m.ISub); break;
                case Opcode.trap_0: RewriteTrap(0); break;
                case Opcode.trap_1: RewriteTrap(1); break;
                case Opcode.trap_2: RewriteTrap(2); break;
                case Opcode.trap_3: RewriteTrap(3); break;
                case Opcode.trap_4: RewriteTrap(4); break;
                case Opcode.trap_5: RewriteTrap(5); break;
                case Opcode.trap_6: RewriteTrap(6); break;
                case Opcode.trap_7: RewriteTrap(7); break;
                case Opcode.trap_8: RewriteTrap(8); break;
                case Opcode.trap_9: RewriteTrap(9); break;
                case Opcode.trap_10: RewriteTrap(10); break;
                case Opcode.trap_11: RewriteTrap(11); break;
                case Opcode.trap_12: RewriteTrap(12); break;
                case Opcode.trap_13: RewriteTrap(13); break;
                case Opcode.trap_14: RewriteTrap(14); break;
                case Opcode.trap_15: RewriteTrap(15); break;
                case Opcode.trap_16: RewriteTrap(16); break;
                case Opcode.trap_17: RewriteTrap(17); break;
                case Opcode.trap_18: RewriteTrap(18); break;
                case Opcode.trap_19: RewriteTrap(19); break;
                case Opcode.trap_20: RewriteTrap(20); break;
                case Opcode.trap_21: RewriteTrap(21); break;
                case Opcode.trap_22: RewriteTrap(22); break;
                case Opcode.trap_23: RewriteTrap(23); break;
                case Opcode.tstb: RewriteTst(arch.b); break;
                case Opcode.xchb: RewriteXchb(); break;
                case Opcode.xor: RewriteLogical(m.Xor); break;
                case Opcode.xorp: RewriteLogical(m.Xor); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
                {
                    Class = rtlc,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        private Expression Operand(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand imm:
                return imm.Value;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Address != null)
                {
                    if (mem.Register != null)
                    {
                        ea = m.IAdd(
                            mem.Address,
                            m.Cast(PrimitiveType.UInt16, binder.EnsureRegister(mem.Register)));
                    }
                    else
                    {
                        ea = mem.Address;
                    }
                }
                else
                {
                    ea = RegisterPair(mem.Register);
                }
                return m.Mem(mem.Width, ea);
            case AddressOperand addr:
                return addr.Address;
            default:
                throw new NotImplementedException(op.GetType().Name);
            }
        }

        private Identifier RegisterPair(RegisterStorage reg)
        {
            return binder.EnsureSequence(
                        reg,
                        arch.GpRegs[reg.Number - 1],
                        PrimitiveType.Word16);
        }

        private void CNZ(Expression e)
        {
            var cnz = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CNZ));
            m.Assign(cnz, m.Cond(e));
        }

        private void NZ0(Expression e)
        {
            var nz = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.NZ));
            m.Assign(nz, m.Cond(e));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            m.Assign(c, Constant.False());
        }

        public void RewriteAdcSbb(Func<Expression, Expression, Expression> opr)
        {
            var dst = Operand(instr.op1);
            var src = Operand(instr.op2);
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            m.Assign(
                dst,
                opr(
                    opr(dst, src),
                    c));
            m.Assign(c, m.Cond(dst));
        }

        private void RewriteArithmetic(Func<Expression, Expression, Expression> fn)
        {
            var src = Operand(instr.op1);
            var dst = Operand(instr.op2);
            m.Assign(dst, fn(dst, src));
            CNZ(dst);
        }

        private void RewriteBtj(Func<Expression, Expression> fn)
        {
            this.rtlc = InstrClass.ConditionalTransfer;
            var opLeft = Operand(instr.op2);
            var opRight = Operand(instr.op1);
            NZ0(m.And(opLeft, fn(opRight)));
            var z = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.ZF));
            m.Branch(
                m.Test(ConditionCode.NE, z),
                ((AddressOperand)instr.op3).Address,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBr()
        {
            rtlc = InstrClass.Transfer;
            var dst = Operand(instr.op1);
            var ea = ((MemoryAccess)dst).EffectiveAddress;
            m.Goto(ea);
        }

        private void RewriteCall()
        {
            rtlc = InstrClass.Transfer | InstrClass.Call;
            var dst = Operand(instr.op1);
            var ea = ((MemoryAccess)dst).EffectiveAddress;
            m.Call(ea, 2);
        }

        private void RewriteDacDsb(string intrinsicName)
        {
            var opLeft = Operand(instr.op1);
            var opRight = Operand(instr.op1);

            var c = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            m.Assign(c, host.PseudoProcedure(
                intrinsicName,
                PrimitiveType.Bool,
                opLeft,
                opRight,
                c,
                m.Out(opLeft.DataType, opLeft)));
            CNZ(opLeft);
        }

        private void RewriteClr()
        {
            var op = Operand(instr.op1);
            m.Assign(op, Constant.Zero(op.DataType));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.NF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.ZF)), Constant.True());
        }

        private void RewriteDint()
        {
            m.SideEffect(host.PseudoProcedure("__dint", VoidType.Instance));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.NF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.ZF)), Constant.False());
        }


        private void RewriteEint()
        {
            m.SideEffect(host.PseudoProcedure("__eint", VoidType.Instance));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF)), Constant.True());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.NF)), Constant.True());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.ZF)), Constant.True());
        }


        private void RewriteIncDec(Func<Expression, Expression, Expression> fn)
        {
            var reg = Operand(instr.op1);
            m.Assign(reg, fn(reg, Constant.Word(reg.DataType.BitSize, 1)));
            CNZ(reg);
        }


        private void RewriteIncdDecd(Func<Expression, Expression, Expression> fn)
        {
            var hireg = ((RegisterOperand)instr.op1).Register;
            var loreg = arch.GpRegs[(hireg.Number - 1 & 0xFF)];
            var reg = binder.EnsureSequence(hireg, loreg, PrimitiveType.Word16);
            m.Assign(reg, fn(reg, Constant.Word(reg.DataType.BitSize, 1)));
            CNZ(reg);
        }

        private void RewriteDjnz()
        {
            rtlc = InstrClass.ConditionalTransfer;
            var reg = Operand(instr.op1);
            m.Assign(reg, m.ISub(reg, 1));
            m.Branch(m.Ne0(reg), ((AddressOperand)instr.op2).Address, rtlc);
        }

        private void RewriteIdle()
        {
            m.SideEffect(host.PseudoProcedure("__idle", VoidType.Instance));
        }

        private void RewriteInv()
        {
            var op = Operand(instr.op1);
            m.Assign(op, m.Comp(op));
            NZ0(op);
        }

        private void RewriteJcc(ConditionCode cc, FlagM grf)
        {
            rtlc = InstrClass.ConditionalTransfer;
            var dst = ((AddressOperand)instr.op1).Address;
            var flags = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)grf));
            m.Branch(m.Test(cc, flags), dst, rtlc);
        }

        private void RewriteJmp()
        {
            rtlc = InstrClass.Transfer;
            m.Goto(Operand(instr.op1));
        }

        private void RewriteLda()
        {
            var a = binder.EnsureRegister(arch.a);
            var src = Operand(instr.op1);
            m.Assign(a, src);
            NZ0(a);
        }

        private void RewriteLdsp()
        {
            var b = binder.EnsureRegister(arch.b);
            var sp = binder.EnsureRegister(arch.sp);
            m.Assign(sp, b);
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> fn)
        {
            var src = Operand(instr.op1);
            var dst = Operand(instr.op2);
            m.Assign(dst, fn(dst, src));
            NZ0(dst);
        }

        private void RewriteMov()
        {
            var src = Operand(instr.op1);
            var dst = Operand(instr.op2);
            m.Assign(dst, src);
            NZ0(dst);
        }

        private void RewriteMovd()
        {
            var dst = RegisterPair(((RegisterOperand)instr.op2).Register);
            var src = Operand(instr.op1);
            if (src is MemoryAccess mem)
            {
                src = mem.EffectiveAddress;
            }
            m.Assign(dst, src);
            NZ0(dst);
        }

        private void RewriteMpy()
        {
            var dst = binder.EnsureSequence(arch.a, arch.b, PrimitiveType.Word16);
            var left = Operand(instr.op2);
            var right = Operand(instr.op1);
            m.Assign(dst, m.IMul(left, right));
            NZ0(dst);
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(arch.sp);
            var dst = Operand(instr.op1);
            m.Assign(dst, m.Mem8(m.Cast(PrimitiveType.Ptr16, sp)));
            m.Assign(sp, m.ISub(sp, 1));
            if (!(instr.op1 is RegisterOperand reg &&
                reg.Register == arch.st))
            {
                NZ0(dst);
            }
        }

        private void RewritePush()
        {
            var sp = binder.EnsureRegister(arch.sp);
            var src = Operand(instr.op1);
            m.Assign(sp, m.IAdd(sp, 1));
            m.Assign(m.Mem8(m.Cast(PrimitiveType.Ptr16, sp)), src);
        }

        private void RewriteReti()
        {
            rtlc = InstrClass.Transfer;
            m.Return(2, 1);
        }

        private void RewriteRets()
        {
            rtlc = InstrClass.Transfer;
            m.Return(2, 0);
        }

        private void RewriteRotate(string rot)
        {
            var op = Operand(instr.op1);
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            m.Assign(
                op,
                host.PseudoProcedure(rot, op.DataType, op));
            m.Assign(C, m.Cond(op));
        }

        private void RewriteRotateC(string rot)
        {
            var op = Operand(instr.op1);
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            m.Assign(
                op,
                host.PseudoProcedure(rot, op.DataType, op, C));
            m.Assign(C, m.Cond(op));
        }

        private void RewriteSetc()
        {
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF)), Constant.True());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.NF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.ZF)), Constant.True());
        }

        private void RewriteSta()
        {
            var a = binder.EnsureRegister(arch.a);
            var dst = Operand(instr.op1);
            m.Assign(dst, a);
        }

        private void RewriteTst(RegisterStorage reg)
        {
            NZ0(binder.EnsureRegister(reg));
        }

        private void RewriteStsp()
        {
            var b = binder.EnsureRegister(arch.b);
            var sp = binder.EnsureRegister(arch.sp);
            m.Assign(b, sp);
        }

        private void RewriteSwap()
        {
            var op = Operand(instr.op1);
            m.Assign(op, host.PseudoProcedure("__swap_nybbles", op.DataType, op));
            CNZ(op);
        }

        private void RewriteTrap(int n)
        {
            m.SideEffect(host.PseudoProcedure(
                "__trap",
                VoidType.Instance,
                Constant.Byte((byte)n)));
        }

        private void RewriteXchb()
        {
            var dst = Operand(instr.op1);
            var src = binder.EnsureRegister(arch.b);
            var tmp = binder.CreateTemporary(dst.DataType);
            m.Assign(tmp, dst);
            m.Assign(dst, src);
            m.Assign(src, tmp);
        }
    }
}
