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
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(instr.Address, "Rewriting x86 opcode '{0}' is not supported yet.", instr);
                    rtlc = InstrClass.Invalid;
                    break;
                case Mnemonic.adc: RewriteAdcSbb(m.IAdd); break;
                case Mnemonic.add: RewriteArithmetic(m.IAdd); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.andp: RewriteLogical(m.And); break;
                case Mnemonic.btjo: RewriteBtj(a => a); break;
                case Mnemonic.btjop: RewriteBtj(a => a); break;
                case Mnemonic.btjz: RewriteBtj(m.Comp); break;
                case Mnemonic.btjzp: RewriteBtj(m.Comp); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.clr: RewriteClr(); break;
                case Mnemonic.tsta: RewriteTst(arch.a); break;
                case Mnemonic.dac: RewriteDacDsb("__dac"); break;
                case Mnemonic.dec: RewriteIncDec(m.ISub); break;
                case Mnemonic.decd: RewriteIncdDecd(m.ISub); break;
                case Mnemonic.dint: RewriteDint(); break;
                case Mnemonic.djnz: RewriteDjnz(); break;
                case Mnemonic.dsb: RewriteDacDsb("__dsb"); break;
                case Mnemonic.eint: RewriteEint(); break;
                case Mnemonic.idle: RewriteIdle(); break;
                case Mnemonic.inc: RewriteIncDec(m.IAdd); break;
                case Mnemonic.inv: RewriteInv(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jeq: RewriteJcc(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.jge: RewriteJcc(ConditionCode.GE, FlagM.NZ); break;
                case Mnemonic.jgt: RewriteJcc(ConditionCode.GT, FlagM.NZ); break;
                case Mnemonic.jhs: RewriteJcc(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonic.jl: RewriteJcc(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonic.jne: RewriteJcc(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.lda: RewriteLda(); break;
                case Mnemonic.ldsp: RewriteLdsp(); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movd: RewriteMovd(); break;
                case Mnemonic.movp: RewriteMov(); break;
                case Mnemonic.mpy: RewriteMpy(); break; 
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteLogical(m.Or); break;
                case Mnemonic.orp: RewriteLogical(m.Or); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.reti: RewriteReti(); break;
                case Mnemonic.rets: RewriteRets(); break;
                case Mnemonic.rl: RewriteRotate(PseudoProcedure.Rol); break;
                case Mnemonic.rlc: RewriteRotateC(PseudoProcedure.RolC); break;
                case Mnemonic.rr: RewriteRotate(PseudoProcedure.Ror); break;
                case Mnemonic.rrc: RewriteRotateC(PseudoProcedure.RorC); break;
                case Mnemonic.sbb: RewriteAdcSbb(m.ISub); break;
                case Mnemonic.setc: RewriteSetc(); break;
                case Mnemonic.sta: RewriteSta(); break;
                case Mnemonic.stsp: RewriteStsp(); break;
                case Mnemonic.sub: RewriteArithmetic(m.ISub); break;
                case Mnemonic.trap_0: RewriteTrap(0); break;
                case Mnemonic.trap_1: RewriteTrap(1); break;
                case Mnemonic.trap_2: RewriteTrap(2); break;
                case Mnemonic.trap_3: RewriteTrap(3); break;
                case Mnemonic.trap_4: RewriteTrap(4); break;
                case Mnemonic.trap_5: RewriteTrap(5); break;
                case Mnemonic.trap_6: RewriteTrap(6); break;
                case Mnemonic.trap_7: RewriteTrap(7); break;
                case Mnemonic.trap_8: RewriteTrap(8); break;
                case Mnemonic.trap_9: RewriteTrap(9); break;
                case Mnemonic.trap_10: RewriteTrap(10); break;
                case Mnemonic.trap_11: RewriteTrap(11); break;
                case Mnemonic.trap_12: RewriteTrap(12); break;
                case Mnemonic.trap_13: RewriteTrap(13); break;
                case Mnemonic.trap_14: RewriteTrap(14); break;
                case Mnemonic.trap_15: RewriteTrap(15); break;
                case Mnemonic.trap_16: RewriteTrap(16); break;
                case Mnemonic.trap_17: RewriteTrap(17); break;
                case Mnemonic.trap_18: RewriteTrap(18); break;
                case Mnemonic.trap_19: RewriteTrap(19); break;
                case Mnemonic.trap_20: RewriteTrap(20); break;
                case Mnemonic.trap_21: RewriteTrap(21); break;
                case Mnemonic.trap_22: RewriteTrap(22); break;
                case Mnemonic.trap_23: RewriteTrap(23); break;
                case Mnemonic.tstb: RewriteTst(arch.b); break;
                case Mnemonic.xchb: RewriteXchb(); break;
                case Mnemonic.xor: RewriteLogical(m.Xor); break;
                case Mnemonic.xorp: RewriteLogical(m.Xor); break;
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
                        PrimitiveType.Word16,
                reg,
                arch.GpRegs[reg.Number - 1]);
        }

        private void CNZ(Expression e)
        {
            var cnz = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CNZ));
            m.Assign(cnz, m.Cond(e));
        }

        private void NZ0(Expression e)
        {
            var nz = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.NZ));
            m.Assign(nz, m.Cond(e));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF));
            m.Assign(c, Constant.False());
        }

        public void RewriteAdcSbb(Func<Expression, Expression, Expression> opr)
        {
            var dst = Operand(instr.Operands[0]);
            var src = Operand(instr.Operands[1]);
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF));
            m.Assign(
                dst,
                opr(
                    opr(dst, src),
                    c));
            m.Assign(c, m.Cond(dst));
        }

        private void RewriteArithmetic(Func<Expression, Expression, Expression> fn)
        {
            var src = Operand(instr.Operands[0]);
            var dst = Operand(instr.Operands[1]);
            m.Assign(dst, fn(dst, src));
            CNZ(dst);
        }

        private void RewriteBtj(Func<Expression, Expression> fn)
        {
            this.rtlc = InstrClass.ConditionalTransfer;
            var opLeft = Operand(instr.Operands[1]);
            var opRight = Operand(instr.Operands[0]);
            NZ0(m.And(opLeft, fn(opRight)));
            var z = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.ZF));
            m.Branch(
                m.Test(ConditionCode.NE, z),
                ((AddressOperand)instr.Operands[2]).Address,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBr()
        {
            rtlc = InstrClass.Transfer;
            var dst = Operand(instr.Operands[0]);
            var ea = ((MemoryAccess)dst).EffectiveAddress;
            m.Goto(ea);
        }

        private void RewriteCall()
        {
            rtlc = InstrClass.Transfer | InstrClass.Call;
            var dst = Operand(instr.Operands[0]);
            var ea = ((MemoryAccess)dst).EffectiveAddress;
            m.Call(ea, 2);
        }

        private void RewriteDacDsb(string intrinsicName)
        {
            var opLeft = Operand(instr.Operands[0]);
            var opRight = Operand(instr.Operands[0]);

            var c = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF));
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
            var op = Operand(instr.Operands[0]);
            m.Assign(op, Constant.Zero(op.DataType));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.NF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.ZF)), Constant.True());
        }

        private void RewriteDint()
        {
            m.SideEffect(host.PseudoProcedure("__dint", VoidType.Instance));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.NF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.ZF)), Constant.False());
        }


        private void RewriteEint()
        {
            m.SideEffect(host.PseudoProcedure("__eint", VoidType.Instance));
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF)), Constant.True());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.NF)), Constant.True());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.ZF)), Constant.True());
        }


        private void RewriteIncDec(Func<Expression, Expression, Expression> fn)
        {
            var reg = Operand(instr.Operands[0]);
            m.Assign(reg, fn(reg, Constant.Word(reg.DataType.BitSize, 1)));
            CNZ(reg);
        }


        private void RewriteIncdDecd(Func<Expression, Expression, Expression> fn)
        {
            var hireg = ((RegisterOperand)instr.Operands[0]).Register;
            var loreg = arch.GpRegs[(hireg.Number - 1 & 0xFF)];
            var reg = binder.EnsureSequence(PrimitiveType.Word16, hireg, loreg);
            m.Assign(reg, fn(reg, Constant.Word(reg.DataType.BitSize, 1)));
            CNZ(reg);
        }

        private void RewriteDjnz()
        {
            rtlc = InstrClass.ConditionalTransfer;
            var reg = Operand(instr.Operands[0]);
            m.Assign(reg, m.ISub(reg, 1));
            m.Branch(m.Ne0(reg), ((AddressOperand)instr.Operands[1]).Address, rtlc);
        }

        private void RewriteIdle()
        {
            m.SideEffect(host.PseudoProcedure("__idle", VoidType.Instance));
        }

        private void RewriteInv()
        {
            var op = Operand(instr.Operands[0]);
            m.Assign(op, m.Comp(op));
            NZ0(op);
        }

        private void RewriteJcc(ConditionCode cc, FlagM grf)
        {
            rtlc = InstrClass.ConditionalTransfer;
            var dst = ((AddressOperand)instr.Operands[0]).Address;
            var flags = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)grf));
            m.Branch(m.Test(cc, flags), dst, rtlc);
        }

        private void RewriteJmp()
        {
            rtlc = InstrClass.Transfer;
            m.Goto(Operand(instr.Operands[0]));
        }

        private void RewriteLda()
        {
            var a = binder.EnsureRegister(arch.a);
            var src = Operand(instr.Operands[0]);
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
            var src = Operand(instr.Operands[0]);
            var dst = Operand(instr.Operands[1]);
            m.Assign(dst, fn(dst, src));
            NZ0(dst);
        }

        private void RewriteMov()
        {
            var src = Operand(instr.Operands[0]);
            var dst = Operand(instr.Operands[1]);
            m.Assign(dst, src);
            NZ0(dst);
        }

        private void RewriteMovd()
        {
            var dst = RegisterPair(((RegisterOperand)instr.Operands[1]).Register);
            var src = Operand(instr.Operands[0]);
            if (src is MemoryAccess mem)
            {
                src = mem.EffectiveAddress;
            }
            m.Assign(dst, src);
            NZ0(dst);
        }

        private void RewriteMpy()
        {
            var dst = binder.EnsureSequence(PrimitiveType.Word16, arch.a, arch.b);
            var left = Operand(instr.Operands[1]);
            var right = Operand(instr.Operands[0]);
            m.Assign(dst, m.IMul(left, right));
            NZ0(dst);
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(arch.sp);
            var dst = Operand(instr.Operands[0]);
            m.Assign(dst, m.Mem8(m.Cast(PrimitiveType.Ptr16, sp)));
            m.Assign(sp, m.ISub(sp, 1));
            if (!(instr.Operands[0] is RegisterOperand reg &&
                reg.Register == arch.st))
            {
                NZ0(dst);
            }
        }

        private void RewritePush()
        {
            var sp = binder.EnsureRegister(arch.sp);
            var src = Operand(instr.Operands[0]);
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
            var op = Operand(instr.Operands[0]);
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF));
            m.Assign(
                op,
                host.PseudoProcedure(rot, op.DataType, op));
            m.Assign(C, m.Cond(op));
        }

        private void RewriteRotateC(string rot)
        {
            var op = Operand(instr.Operands[0]);
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF));
            m.Assign(
                op,
                host.PseudoProcedure(rot, op.DataType, op, C));
            m.Assign(C, m.Cond(op));
        }

        private void RewriteSetc()
        {
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.CF)), Constant.True());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.NF)), Constant.False());
            m.Assign(binder.EnsureFlagGroup(arch.GetFlagGroup(arch.st, (uint)FlagM.ZF)), Constant.True());
        }

        private void RewriteSta()
        {
            var a = binder.EnsureRegister(arch.a);
            var dst = Operand(instr.Operands[0]);
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
            var op = Operand(instr.Operands[0]);
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
            var dst = Operand(instr.Operands[0]);
            var src = binder.EnsureRegister(arch.b);
            var tmp = binder.CreateTemporary(dst.DataType);
            m.Assign(tmp, dst);
            m.Assign(dst, src);
            m.Assign(src, tmp);
        }
    }
}
