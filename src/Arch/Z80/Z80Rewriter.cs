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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Z80
{
    public class Z80Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Z80ProcessorArchitecture arch;
        private IStorageBinder binder;
        private IRewriterHost host;
        private IEnumerator<Z80Instruction> dasm;
        private InstrClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private RtlEmitter m;

        public Z80Rewriter(Z80ProcessorArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.dasm = new Z80Disassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var addr = dasm.Current.Address;
                var len = dasm.Current.Length;
                this.rtlc = dasm.Current.InstructionClass;
                this.rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);
                switch (dasm.Current.Code)
                {
                default: throw new AddressCorrelatedException(
                    dasm.Current.Address,
                    "Z80 instruction '{0}' is not supported yet.",
                    dasm.Current.Code);
                case Opcode.illegal: m.Invalid(); break;
                case Opcode.adc: RewriteAdc(); break;
                case Opcode.add: RewriteAdd(); break;
                case Opcode.and: RewriteAnd(); break;
                case Opcode.bit: RewriteBit(); break;
                case Opcode.call: RewriteCall(dasm.Current); break;
                case Opcode.ccf: RewriteCcf(); break;
                case Opcode.cp: RewriteCp(); break;
                case Opcode.cpd: RewriteCp(m.ISub, false);  break;
                case Opcode.cpdr:RewriteCp(m.ISub, true);   break;
                case Opcode.cpi: RewriteCp(m.IAdd, false);  break;
                case Opcode.cpir:RewriteCp(m.IAdd, true);   break;
                case Opcode.cpl: RewriteCpl(); break;
                case Opcode.di: RewriteDi(); break;
                case Opcode.daa: RewriteDaa(); break;
                case Opcode.dec: RewriteDec(); break;
                case Opcode.djnz: RewriteDjnz(dasm.Current.Op1); break;
                case Opcode.ei: RewriteEi(); break;
                case Opcode.ex: RewriteEx(); break;
                case Opcode.ex_af: RewriteExAf(); break;
                case Opcode.exx: RewriteExx(); break;
                case Opcode.hlt: RewriteHlt(); break;
                case Opcode.@in: RewriteIn(); break;
                case Opcode.ind:  RewriteIn(m.ISub, false); break;
                case Opcode.indr: RewriteIn(m.ISub, true); break;
                case Opcode.ini: RewriteIn(m.IAdd, false); break;
                case Opcode.inir: RewriteIn(m.IAdd, true); break;
                case Opcode.im:
                    m.SideEffect(host.PseudoProcedure("__im", VoidType.Instance, RewriteOp(dasm.Current.Op1)));
                    break;
                case Opcode.inc: RewriteInc(); break;
                case Opcode.jp: RewriteJp(dasm.Current); break;
                case Opcode.jr: RewriteJr(); break;
                case Opcode.ld: RewriteLd();  break;
                case Opcode.rl: RewriteRotation(PseudoProcedure.RolC, true); break;
                case Opcode.rla: RewriteRotation(PseudoProcedure.RolC, true); break;
                case Opcode.rlc: RewriteRotation(PseudoProcedure.Rol, false); break;
                case Opcode.rlca: RewriteRotation(PseudoProcedure.Rol, false); break;
                case Opcode.rr: RewriteRotation(PseudoProcedure.RorC, true); break;
                case Opcode.rra: RewriteRotation(PseudoProcedure.RorC, true); break;
                case Opcode.rrc: RewriteRotation(PseudoProcedure.Ror, true); break;
                case Opcode.rrca: RewriteRotation(PseudoProcedure.Ror, true); break;
                case Opcode.ldd: RewriteBlockInstruction(m.ISub, false); break;
                case Opcode.lddr: RewriteBlockInstruction(m.ISub, true); break;
                case Opcode.ldi: RewriteBlockInstruction(m.IAdd, false); break;
                case Opcode.ldir: RewriteBlockInstruction(m.IAdd, true); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.or: RewriteOr(); break;
                case Opcode.@out: RewriteOut(); break;
                case Opcode.pop: RewritePop(); break;
                case Opcode.push: RewritePush(dasm.Current); break;
                case Opcode.res: RewriteResSet("__res"); break;
                case Opcode.ret: RewriteRet(); break;
                case Opcode.rst: RewriteRst(); break;
                case Opcode.sbc: RewriteSbc(); break;
                case Opcode.scf: RewriteScf(); break;
                case Opcode.set: RewriteResSet("__set"); break;
                case Opcode.sla: RewriteShift(dasm.Current, m.Shl); break;
                case Opcode.sra: RewriteShift(dasm.Current, m.Sar); break;
                case Opcode.srl: RewriteShift(dasm.Current, m.Shr); break;
                case Opcode.sub: RewriteSub(); break;
                case Opcode.xor: RewriteXor(); break;

                //$TODO: Not implemented yet; feel free to implement these!
        case Opcode.otdr: goto default;
        case Opcode.otir: goto default;
        case Opcode.outd: goto default;
        case Opcode.outi: goto default;
        case Opcode.outr: goto default;
        case Opcode.reti: goto default;
        case Opcode.retn: goto default;
        case Opcode.rld: goto default;
        case Opcode.rrd: goto default;
        case Opcode.swap: goto default;
                }
                yield return new RtlInstructionCluster(addr, len, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        private void RewriteAdc()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.Assign(dst, m.IAdd(m.IAdd(dst, src), FlagGroup(FlagM.CF)));
            AssignCond(FlagM.CF | FlagM.ZF | FlagM.SF | FlagM.PF, dst);
        }

        private void RewriteAdd()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.Assign(dst, m.IAdd(dst, src));
            AssignCond(FlagM.CF | FlagM.ZF | FlagM.SF | FlagM.PF, dst);
        }

        private void RewriteAnd()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.Assign(dst, m.And(dst, src));
            AssignCond(FlagM.ZF | FlagM.SF, dst);
            m.Assign(FlagGroup(FlagM.CF), Constant.False());
        }

        private void RewriteBlockInstruction(Func<Expression, Expression, Expression> incdec, bool repeat)
        {
            var bc = binder.EnsureRegister(Registers.bc);
            var de = binder.EnsureRegister(Registers.de);
            var hl = binder.EnsureRegister(Registers.hl);
            var V =  FlagGroup(FlagM.PF);
            m.Assign(m.Mem8(de), m.Mem8(hl));
            m.Assign(hl, incdec(hl, Constant.Int16(1)));
            m.Assign(de, incdec(de, Constant.Int16(1)));
            m.Assign(bc, m.ISub(bc, 1));
            if (repeat)
            {
                m.BranchInMiddleOfInstruction(m.Ne0(bc), dasm.Current.Address, InstrClass.Transfer);
            }
            m.Assign(V, m.Const(PrimitiveType.Bool, 0));
        }

        private void RewriteNeg()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.Neg(a));
            AssignCond(FlagM.SF | FlagM.ZF | FlagM.PF | FlagM.CF, a);
        }

        private void RewriteOr()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.Assign(dst, m.Or(dst, src));
            AssignCond(FlagM.ZF | FlagM.SF, dst);
            m.Assign(FlagGroup(FlagM.CF), Constant.False());
        }

        private void RewriteSbc()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.Assign(dst, m.ISub(m.ISub(dst, src), FlagGroup(FlagM.CF)));
            AssignCond(FlagM.CF | FlagM.ZF | FlagM.SF | FlagM.PF, dst);
        }

        private void RewriteRotation(string pseudoOp, bool useCarry)
        {
            Expression reg;
            if (dasm.Current.Op1 != null)
            {
                reg = RewriteOp(dasm.Current.Op1);
            }
            else
            {
                reg = binder.EnsureRegister(Registers.a);
            }
            var C = FlagGroup(FlagM.CF);
            var one = m.Byte(1);
            Expression src;
            if (useCarry)
            {
                src = host.PseudoProcedure(pseudoOp, reg.DataType, reg, one, C);
            }
            else 
            {
                src = host.PseudoProcedure(pseudoOp, reg.DataType, reg, one);
            }
            m.Assign(reg, src);
            m.Assign(C, m.Cond(reg));
        }

        private void RewriteScf()
        {
            AssignCond(FlagM.CF, Constant.True());
        }

        private void RewriteSub()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.Assign(dst, m.ISub(dst, src));
            AssignCond(FlagM.CF | FlagM.ZF | FlagM.SF | FlagM.CF, dst);
        }

        private void RewriteXor()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.Assign(dst, m.Xor(dst, src));
            AssignCond(FlagM.ZF | FlagM.SF, dst);
            m.Assign(FlagGroup(FlagM.CF), Constant.False());
        }

        private void AssignCond(FlagM flags, Expression dst)
        {
            m.Assign(FlagGroup(flags), m.Cond(dst));
        }

        public Identifier FlagGroup(FlagM flags)
        {
            return binder.EnsureFlagGroup(arch.GetFlagGroup((uint) flags));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitBranch(ConditionOperand cOp, Address dst)
        {
            m.Branch(
                GenerateTestExpression(cOp, false),
                dst,
                InstrClass.ConditionalTransfer);
        }

        private TestCondition GenerateTestExpression(ConditionOperand cOp, bool invert)
        {
            ConditionCode cc = ConditionCode.ALWAYS;
            FlagM flags = 0;
            switch (cOp.Code)
            {
            case CondCode.nz:  cc = invert ? ConditionCode.EQ : ConditionCode.NE; flags = FlagM.ZF;  break;
            case CondCode.z: cc = invert ? ConditionCode.NE : ConditionCode.EQ; flags = FlagM.ZF;    break;
            case CondCode.nc: cc = invert ? ConditionCode.ULT : ConditionCode.UGE; flags = FlagM.CF; break;
            case CondCode.c: cc = invert ? ConditionCode.UGE : ConditionCode.ULT; flags = FlagM.CF;  break;
            case CondCode.po: cc = invert ? ConditionCode.PE : ConditionCode.PO; flags = FlagM.PF;  break;
            case CondCode.pe: cc = invert ? ConditionCode.PO : ConditionCode.PE; flags = FlagM.PF;    break;
            case CondCode.p: cc = invert ? ConditionCode.SG : ConditionCode.NS; flags = FlagM.PF;    break;
            case CondCode.m: cc = invert ? ConditionCode.NS : ConditionCode.SG; flags = FlagM.PF;    break;
            }
            return m.Test(
                cc,
                FlagGroup(flags));
        }

        private void RewriteCall(Z80Instruction instr)
        {
            if (instr.Op1 is ConditionOperand cOp)
            {
                m.BranchInMiddleOfInstruction(
                    GenerateTestExpression(cOp, true),
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
                m.Call(((AddressOperand)instr.Op2).Address, 2);
            }
            else
            {
                m.Call(((AddressOperand)instr.Op1).Address, 2);
            }
        }

        private void RewriteCcf()
        {
            AssignCond(FlagM.CF, Constant.False());
        }

        private void RewriteCp()
        {
            var a = this.RewriteOp(dasm.Current.Op1);
            var b = this.RewriteOp(dasm.Current.Op2);
            m.Assign(
                FlagGroup(FlagM.SF | FlagM.ZF | FlagM.CF | FlagM.PF),
                m.Cond(m.ISub(a, b)));
        }

        private void RewriteCp(Func<Expression , Expression, Expression> incDec, bool repeat)
        {
            var addr = dasm.Current.Address;
            var a = binder.EnsureRegister(Registers.a);
            var bc = binder.EnsureRegister(Registers.bc);
            var hl = binder.EnsureRegister(Registers.hl);
            var z = FlagGroup(FlagM.ZF);
            m.Assign(z, m.Cond(m.ISub(a, m.Mem8(hl))));
            m.Assign(hl, incDec(hl, m.Int16(1)));
            m.Assign(bc, m.ISubS(bc, 1));
            if (repeat)
            {
                m.BranchInMiddleOfInstruction(m.Eq0(bc), addr + dasm.Current.Length, InstrClass.ConditionalTransfer);
                m.Branch(m.Test(ConditionCode.NE, z), addr, InstrClass.ConditionalTransfer);
           }
        }

        private void RewriteCpl()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.Comp(a));
        }

        private void RewriteDaa()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(
                a,
                host.PseudoProcedure("__daa", PrimitiveType.Byte, a));
            AssignCond(FlagM.CF | FlagM.ZF | FlagM.SF | FlagM.PF, a);
        }

        private void RewriteDec()
        {
            var src = RewriteOp(dasm.Current.Op1);
            var dst = RewriteOp(dasm.Current.Op1);
            m.Assign(dst, m.ISub(src, 1));
            AssignCond(FlagM.ZF | FlagM.SF | FlagM.PF, dst);
        }

        private void RewriteDjnz(MachineOperand dst)
        {
            var b = binder.EnsureRegister(Registers.b);
            m.Assign(b, m.ISub(b, 1));
            m.Branch(m.Ne0(b), ((AddressOperand)dst).Address, InstrClass.Transfer);
        }

        private void RewriteDi()
        {
            m.SideEffect(host.PseudoProcedure("__di", VoidType.Instance));
        }

        private void RewriteEi()
        {
            m.SideEffect(host.PseudoProcedure("__ei", VoidType.Instance));
        }

        private void RewriteEx()
        {
            var t = binder.CreateTemporary(dasm.Current.Op1.Width);
            m.Assign(t, RewriteOp(dasm.Current.Op1));
            m.Assign(RewriteOp(dasm.Current.Op1), RewriteOp(dasm.Current.Op2));
            m.Assign(RewriteOp(dasm.Current.Op2), t);
        }

        private void RewriteExAf()
        {
            var t = binder.CreateTemporary(Registers.af.DataType);
            var af = binder.EnsureRegister(Registers.af);
            var af_ = binder.EnsureRegister(Registers.af_);
            m.Assign(t, af);
            m.Assign(af, af_);
            m.Assign(af_, t);
        }

        private void RewriteExx()
        {
            foreach (var r in new[] { "bc", "de", "hl" })
            {
                var t = binder.CreateTemporary(PrimitiveType.Word16);
                var reg = binder.EnsureRegister(arch.GetRegister(r));
                var reg_ = binder.EnsureRegister(arch.GetRegister(r + "'"));
                m.Assign(t, reg);
                m.Assign(reg, reg_);
                m.Assign(reg_, t);
            }
        }

        private void RewriteHlt()
        {
            m.SideEffect(host.PseudoProcedure("__hlt", VoidType.Instance));
        }

        private void RewriteInc()
        {
            var src = RewriteOp(dasm.Current.Op1);
            var dst = RewriteOp(dasm.Current.Op1);
            m.Assign(dst, m.IAdd(src, 1));
            AssignCond(FlagM.ZF | FlagM.SF | FlagM.PF, dst);
        }

 
        private void RewriteJp(Z80Instruction instr)
        {
            switch (instr.Op1)
            {
            case ConditionOperand cOp:
                EmitBranch(cOp, ((AddressOperand)instr.Op2).Address);
                break;
            case AddressOperand target:
                m.Goto(target.Address);
                break;
            case MemoryOperand mTarget:
                m.Goto(binder.EnsureRegister(mTarget.Base));
                break;
            }
        }

        private void RewriteJr()
        {
            var op = dasm.Current.Op1;
            var cop = op as ConditionOperand;
            if (cop != null)
            {
                op = dasm.Current.Op2;
            }
            var target = (AddressOperand)op;
            if (cop != null)
            {
                ConditionCode cc;
                FlagM cr;
                switch (cop.Code)
                {
                case CondCode.c: cc = ConditionCode.ULT; cr = FlagM.CF; break;
                case CondCode.nz: cc = ConditionCode.NE; cr = FlagM.ZF; break;
                case CondCode.nc: cc = ConditionCode.UGE; cr = FlagM.CF; break;
                case CondCode.z: cc = ConditionCode.EQ; cr = FlagM.ZF; break;
                default: throw new NotImplementedException();
                }
                m.Branch(
                    m.Test(
                        cc,
                        binder.EnsureFlagGroup(arch.GetFlagGroup((uint)cr))),
                    target.Address,
                    rtlc);
            }
            else
            {
                m.Goto(target.Address);
            }
        }

        private void RewriteLd()
        {
            m.Assign(
                RewriteOp(dasm.Current.Op1),
                RewriteOp(dasm.Current.Op2));
        }

        private Expression RewriteOp(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rOp:
                return binder.EnsureRegister(rOp.Register);
            case ImmediateOperand immOp:
                return immOp.Value;
            case MemoryOperand memOp:
                {
                    Identifier bReg = null;
                    if (memOp.Base != null)
                        bReg = binder.EnsureRegister(memOp.Base);
                    if (memOp.Offset == null)
                    {
                        return m.Mem(memOp.Width, bReg);
                    }
                    else if (bReg == null)
                    {
                        return m.Mem(memOp.Width, memOp.Offset);
                    }
                    else
                    {
                        int s = memOp.Offset.ToInt32();
                        if (s > 0)
                        {
                            return m.Mem(memOp.Width, m.IAdd(bReg, s));
                        }
                        else if (s < 0)
                        {
                            return m.Mem(memOp.Width, m.ISub(bReg, -s));
                        }
                        else
                        {
                            return m.Mem(memOp.Width, bReg);
                        }
                    }
                }
            default:
                throw new NotImplementedException(string.Format("Rewriting of Z80 operand type {0} is not implemented yet.", op.GetType().FullName));
            }
        }

        private void RewriteIn()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.Assign(dst, host.PseudoProcedure("__in", PrimitiveType.Byte, src));
        }

        private void RewriteIn(Func<Expression,Expression,Expression> incDec, bool repeat)
        {
            var hl = binder.EnsureRegister(Registers.hl);
            var c = binder.EnsureRegister(Registers.c);
            var b = binder.EnsureRegister(Registers.b);
            var Z = binder.EnsureFlagGroup(arch.GetFlagGroup("Z"));
            m.Assign(
                m.Mem8(hl),
                host.PseudoProcedure("__in", PrimitiveType.Byte, c));
            m.Assign(hl, incDec(hl, m.Int16(1)));
            m.Assign(b, m.ISub(b, 1));
            m.Assign(Z, m.Cond(b));
            if (repeat)
            {
                m.Branch(m.Ne0(b), dasm.Current.Address, InstrClass.ConditionalTransfer);
            }
        }

        private void RewriteOut()
        {
            var dst = RewriteOp(dasm.Current.Op1);
            var src = RewriteOp(dasm.Current.Op2);
            m.SideEffect(host.PseudoProcedure("__out", PrimitiveType.Byte, dst, src));
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var op = RewriteOp(dasm.Current.Op1);
            m.Assign(op, m.Mem(PrimitiveType.Word16, sp));
            m.Assign(sp, m.IAdd(sp, op.DataType.Size));
        }

        private void RewritePush(Z80Instruction instr)
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var op = RewriteOp(instr.Op1);
            m.Assign(sp, m.ISub(sp, op.DataType.Size));
            m.Assign(m.Mem(PrimitiveType.Word16, sp), op);
        }

        private void RewriteBit()
        {
            var bit = RewriteOp(dasm.Current.Op1);
            var op = RewriteOp(dasm.Current.Op2);
            AssignCond(FlagM.ZF, host.PseudoProcedure("__bit", PrimitiveType.Bool, op, bit));
        }

        private void RewriteResSet(string pseudocode)
        {
            var bit = RewriteOp(dasm.Current.Op1);
            var op = RewriteOp(dasm.Current.Op2);
            Expression dst;
            if (op is MemoryAccess)
                dst = binder.CreateTemporary(op.DataType);
            else
                dst = op;
            m.Assign(dst, host.PseudoProcedure(pseudocode, dst.DataType, op, bit));
            if (dst != op)
            {
                m.Assign(op, dst);
            }
        }

        private void RewriteRet()
        {
            m.Return(2, 0);
        }

        private void RewriteRst()
        {
            m.Call(
                Address.Ptr16(
                    ((ImmediateOperand)dasm.Current.Op1).Value.ToUInt16()),
                2);
        }

        private void RewriteShift(Z80Instruction instr, Func<Expression, Expression, Expression> op)
        {
            var reg = RewriteOp(instr.Op1);
            var sh = m.Byte(1);
            m.Assign(reg, op(reg, sh));
            AssignCond(FlagM.CF | FlagM.ZF | FlagM.SF | FlagM.PF, reg);
        }
    }
}
