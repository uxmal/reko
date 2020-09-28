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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.M6800.M6812
{
    public class M6812Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly M6812Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly M6812State state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<M6812Instruction> dasm;
        private M6812Instruction instr;
        private RtlEmitter m;
        private InstrClass rtlc;

        public M6812Rewriter(M6812Architecture arch, EndianImageReader rdr, M6812State state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new M6812Disassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var rtlInstrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtlInstrs);
                this.rtlc = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                case Mnemonic.mov: 
                case Mnemonic.rev: 
                case Mnemonic.revw:
                case Mnemonic.tbl: 
                    host.Warn(
                        instr.Address,
                        "M6812 instruction '{0}' is not supported yet.",
                        instr.Mnemonic);
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    this.rtlc = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.aba: RewriteAba(); break;
                case Mnemonic.adca: RewriteAdcSbc(Registers.a, m.IAdd); break;
                case Mnemonic.adcb: RewriteAdcSbc(Registers.b, m.IAdd); break;
                case Mnemonic.adda: RewriteArithmetic(Registers.a, m.IAdd); break;
                case Mnemonic.addb: RewriteArithmetic(Registers.b, m.IAdd); break;
                case Mnemonic.addd: RewriteArithmetic(Registers.d, m.IAdd); break;
                case Mnemonic.anda: RewriteLogical(Registers.a, m.And); break;
                case Mnemonic.andb: RewriteLogical(Registers.b, m.And); break;
                case Mnemonic.andcc: RewriteAndcc(); break;
                case Mnemonic.asr: RewriteShiftMem(m.Sar); break;
                case Mnemonic.asra: RewriteArithmetic(Registers.a, m.Sar); break;
                case Mnemonic.asrb: RewriteArithmetic(Registers.b, m.Sar); break;
                case Mnemonic.bcc: RewriteBcc(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonic.bclr: RewriteBclr(); break;
                case Mnemonic.bcs: RewriteBcc(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonic.beq: RewriteBcc(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.bge: RewriteBcc(ConditionCode.GE, FlagM.NF | FlagM.VF); break;
                case Mnemonic.bgt: RewriteBcc(ConditionCode.GT, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.bhi: RewriteBcc(ConditionCode.UGT, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.ble: RewriteBcc(ConditionCode.LE, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.blt: RewriteBcc(ConditionCode.LT, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.bls: RewriteBcc(ConditionCode.ULE, FlagM.VF | FlagM.ZF); break;
                case Mnemonic.bmi: RewriteBcc(ConditionCode.LT, FlagM.NF); break;
                case Mnemonic.bne: RewriteBcc(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonic.bpl: RewriteBcc(ConditionCode.GT, FlagM.NF); break;
                case Mnemonic.bvc: RewriteBcc(ConditionCode.NO, FlagM.VF); break;
                case Mnemonic.bvs: RewriteBcc(ConditionCode.OV, FlagM.VF); break;
                case Mnemonic.bgnd: RewriteBgnd(); break;
                case Mnemonic.bita: RewriteBit(Registers.a); break;
                case Mnemonic.bitb: RewriteBit(Registers.b); break;
                case Mnemonic.bra: RewriteBra(); break;
                case Mnemonic.brclr: RewriteBrclr(); break;
                case Mnemonic.brn: m.Nop(); break;
                case Mnemonic.brset: RewriteBrset(); break;
                case Mnemonic.bset: RewriteBset(); break;
                case Mnemonic.bsr: RewriteBsr(); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.cba: RewriteCba(); break;
                case Mnemonic.clr: RewriteClr(); break;
                case Mnemonic.clra: RewriteClr(Registers.a); break;
                case Mnemonic.clrb: RewriteClr(Registers.b); break;
                case Mnemonic.cmpa: RewriteCmp(Registers.a); break;
                case Mnemonic.cmpb: RewriteCmp(Registers.b); break;
                case Mnemonic.com: RewriteCom(); break;
                case Mnemonic.coma: RewriteCom(Registers.a); break;
                case Mnemonic.comb: RewriteCom(Registers.b); break;
                case Mnemonic.cpd: RewriteCmp(Registers.d); break;
                case Mnemonic.cps: RewriteCmp(Registers.sp); break;
                case Mnemonic.cpx: RewriteCmp(Registers.x); break;
                case Mnemonic.cpy: RewriteCmp(Registers.y); break;
                case Mnemonic.daa: RewriteDaa(); break;
                case Mnemonic.dbeq: RewriteDb(m.Eq0); break;
                case Mnemonic.dbne: RewriteDb(m.Ne0); break;
                case Mnemonic.dec: RewriteIncDec(m.ISub); break;
                case Mnemonic.deca: RewriteIncDec(Registers.a, m.ISub); break;
                case Mnemonic.decb: RewriteIncDec(Registers.b, m.ISub); break;
                case Mnemonic.dex: RewriteIncDecXY(Registers.x, m.ISub); break;
                case Mnemonic.dey: RewriteIncDecXY(Registers.y, m.ISub); break;
                case Mnemonic.ediv: RewriteEdiv(m.UDiv, m.Remainder); break;
                case Mnemonic.edivs: RewriteEdiv(m.SDiv, m.Remainder); break;
                case Mnemonic.emacs: RewriteEmacs(); break;
                case Mnemonic.emaxd: RewriteEmaxmind("__umax"); break;
                case Mnemonic.emaxm: RewriteEmaxminm("__umax"); break;
                case Mnemonic.emind: RewriteEmaxmind("__umin"); break;
                case Mnemonic.eminm: RewriteEmaxminm("__umin"); break;
                case Mnemonic.emul: RewriteEmul(m.UMul); break;
                case Mnemonic.emuls: RewriteEmul(m.SMul); break;
                case Mnemonic.eora: RewriteLogical(Registers.a, m.Xor); break;
                case Mnemonic.eorb: RewriteLogical(Registers.b, m.Xor); break;
                case Mnemonic.etbl: RewriteEtbl(); break;
                case Mnemonic.fdiv: RewriteFdiv(); break;
                case Mnemonic.ibeq: RewriteIb(m.Eq0); break;
                case Mnemonic.ibne: RewriteIb(m.Ne0); break;
                case Mnemonic.idiv: RewriteIdiv(m.UDiv); break;
                case Mnemonic.idivs: RewriteIdiv(m.SDiv); break;
                case Mnemonic.inc: RewriteIncDec(m.IAdd); break;
                case Mnemonic.inca: RewriteIncDec(Registers.a, m.IAdd); break;
                case Mnemonic.incb: RewriteIncDec(Registers.b, m.IAdd); break;
                case Mnemonic.inx: RewriteIncDecXY(Registers.x, m.IAdd); break;
                case Mnemonic.iny: RewriteIncDecXY(Registers.y, m.IAdd); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.lbcc: RewriteBcc(ConditionCode.UGE, FlagM.CF); break;
                case Mnemonic.lbcs: RewriteBcc(ConditionCode.ULT, FlagM.CF); break;
                case Mnemonic.lbeq: RewriteBcc(ConditionCode.EQ, FlagM.ZF); break;
                case Mnemonic.lbge: RewriteBcc(ConditionCode.GE, FlagM.NF | FlagM.VF); break;
                case Mnemonic.lbgt: RewriteBcc(ConditionCode.GT, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.lbhi: RewriteBcc(ConditionCode.UGT, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.lble: RewriteBcc(ConditionCode.LE, FlagM.NF | FlagM.VF | FlagM.ZF); break;
                case Mnemonic.lblt: RewriteBcc(ConditionCode.LT, FlagM.CF | FlagM.ZF); break;
                case Mnemonic.lbls: RewriteBcc(ConditionCode.ULE, FlagM.VF | FlagM.ZF); break;
                case Mnemonic.lbmi: RewriteBcc(ConditionCode.LT, FlagM.NF); break;
                case Mnemonic.lbne: RewriteBcc(ConditionCode.NE, FlagM.ZF); break;
                case Mnemonic.lbpl: RewriteBcc(ConditionCode.GT, FlagM.NF); break;
                case Mnemonic.lbra: RewriteBra(); break;
                case Mnemonic.lbrn: m.Nop(); break;
                case Mnemonic.lbvc: RewriteBcc(ConditionCode.NO, FlagM.VF); break;
                case Mnemonic.lbvs: RewriteBcc(ConditionCode.OV, FlagM.VF); break;
                case Mnemonic.ldaa: RewriteLd(Registers.a); break;
                case Mnemonic.ldab: RewriteLd(Registers.b); break;
                case Mnemonic.ldd: RewriteLd(Registers.d); break;
                case Mnemonic.lds: RewriteLd(Registers.sp); break;
                case Mnemonic.ldx: RewriteLd(Registers.x); break;
                case Mnemonic.ldy: RewriteLd(Registers.y); break;
                case Mnemonic.leas: RewriteLea(Registers.sp); break;
                case Mnemonic.leax: RewriteLea(Registers.x); break;
                case Mnemonic.leay: RewriteLea(Registers.y); break;
                case Mnemonic.lsl: RewriteShiftMem(m.Shl); break;
                case Mnemonic.lsla: RewriteArithmetic(Registers.a, m.Shl); break;
                case Mnemonic.lslb: RewriteArithmetic(Registers.b, m.Shl); break;
                case Mnemonic.lsld: RewriteArithmetic(Registers.d, m.Shl); break;
                case Mnemonic.lsr: RewriteShiftMem(m.Shr); break;
                case Mnemonic.lsra: RewriteArithmetic(Registers.a, m.Shr); break;
                case Mnemonic.lsrb: RewriteArithmetic(Registers.b, m.Shr); break;
                case Mnemonic.lsrd: RewriteArithmetic(Registers.d, m.Shr); break;
                case Mnemonic.maxa: RewriteMaxmina("__umax_b"); break;
                case Mnemonic.maxm: RewriteMaxminm("__umax_b"); break;
                case Mnemonic.mem: RewriteMem(); break;
                case Mnemonic.mina: RewriteMaxmina("__umin_b"); break;
                case Mnemonic.minm: RewriteMaxminm("__umin_b"); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.nega: RewriteNeg(Registers.a); break;
                case Mnemonic.negb: RewriteNeg(Registers.b); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.oraa: RewriteLogical(Registers.a, m.Or); break;
                case Mnemonic.orab: RewriteLogical(Registers.b, m.Or); break;
                case Mnemonic.orcc: RewriteOrcc(); break;
                case Mnemonic.psha: RewritePsh(Registers.a); break;
                case Mnemonic.pshb: RewritePsh(Registers.b); break;
                case Mnemonic.pshc: RewritePsh(Registers.ccr); break;
                case Mnemonic.pshd: RewritePsh(Registers.d); break;
                case Mnemonic.pshx: RewritePsh(Registers.x); break;
                case Mnemonic.pshy: RewritePsh(Registers.y); break;
                case Mnemonic.pula: RewritePul(Registers.a); break;
                case Mnemonic.pulb: RewritePul(Registers.b); break;
                case Mnemonic.pulc: RewritePul(Registers.ccr); break;
                case Mnemonic.puld: RewritePul(Registers.d); break;
                case Mnemonic.pulx: RewritePul(Registers.x); break;
                case Mnemonic.puly: RewritePul(Registers.y); break;
                
                case Mnemonic.rol: RewriteShiftMem(Rol); break;
                case Mnemonic.rola: RewriteArithmetic(Registers.a,Rol); break;
                case Mnemonic.rolb: RewriteArithmetic(Registers.b,Rol); break;
                case Mnemonic.ror: RewriteShiftMem(Ror); break;
                case Mnemonic.rora: RewriteArithmetic(Registers.a, Ror); break;
                case Mnemonic.rorb: RewriteArithmetic(Registers.a, Ror); break;
                case Mnemonic.rtc: RewriteRtc(); break;
                case Mnemonic.rti: RewriteRti(); break;
                case Mnemonic.rts: RewriteRts(); break;
                case Mnemonic.sba: RewriteSba(); break;
                case Mnemonic.sbca: RewriteAdcSbc(Registers.a, m.ISub); break;
                case Mnemonic.sbcb: RewriteAdcSbc(Registers.b, m.ISub); break;
                case Mnemonic.sex: RewriteSex(); break;
                case Mnemonic.staa: RewriteSt(Registers.a); break;
                case Mnemonic.stab: RewriteSt(Registers.b); break;
                case Mnemonic.std: RewriteSt(Registers.d); break;
                case Mnemonic.stop: RewriteStop(); break;
                case Mnemonic.sts: RewriteSt(Registers.sp); break;
                case Mnemonic.stx: RewriteSt(Registers.x); break;
                case Mnemonic.sty: RewriteSt(Registers.y); break;
                case Mnemonic.suba: RewriteSub(Registers.a); break;
                case Mnemonic.subb: RewriteSub(Registers.b); break;
                case Mnemonic.subd: RewriteSub(Registers.d); break;
                case Mnemonic.swi: RewriteSwi(); break;
                case Mnemonic.tab: RewriteTab(); break;
                case Mnemonic.tba: RewriteTba(); break;
                case Mnemonic.tbeq: RewriteTb(m.Eq0); break;
                case Mnemonic.tbne: RewriteTb(m.Ne0); break;
                case Mnemonic.tfr: RewriteTfr(); break;
                case Mnemonic.trap: RewriteTrap(); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.tsta: RewriteTst(Registers.a); break;
                case Mnemonic.tstb: RewriteTst(Registers.b); break;
                case Mnemonic.wai: RewriteWai(); break;
                case Mnemonic.wav: RewriteWav(); break;
                }
                yield return new RtlInstructionCluster(
                    instr.Address,
                    instr.Length,
                    rtlInstrs.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression RewriteOp(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop:
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand immop:
                return immop.Value;
            case MemoryOperand memop:
                return RewriteMemoryOperand(memop);
            }
            throw new NotImplementedException();
        }

        private MemoryAccess RewriteMemoryOperand(MemoryOperand memop)
        {
            if (memop.Base != null)
            {
                Expression baseReg;
                if (memop.Base == Registers.pc)
                    baseReg = instr.Address + instr.Length;
                else
                    baseReg = binder.EnsureRegister(memop.Base);
                Expression ea = baseReg;
                if (memop.Index != null)
                {
                    Expression idx = binder.EnsureRegister(memop.Index);
                    if (idx.DataType.BitSize < ea.DataType.BitSize)
                    {
                        idx = m.Cast(PrimitiveType.UInt16, idx);
                    }
                    ea = m.IAdd(baseReg, idx);
                }
                else if (memop.PreIncrement)
                {
                    m.Assign(ea, m.IAddS(ea, memop.Offset.Value));
                }
                else if (memop.PostIncrement)
                {
                    var tmp = binder.CreateTemporary(baseReg.DataType);
                    m.Assign(tmp, ea);
                    m.Assign(baseReg, m.IAddS(baseReg, memop.Offset.Value));
                    ea = tmp;
                }
                else
                {
                    ea = m.IAdd(baseReg, (ushort)memop.Offset.Value);
                }
                if (memop.Indirect)
                {
                    ea = m.Mem(PrimitiveType.Ptr16, ea);
                }
                return m.Mem(memop.Width, ea);
            }
            else
            {
                Debug.Assert(memop.Offset != null);
                return m.Mem(memop.Width, Address.Ptr16((ushort)memop.Offset.Value));
            }
        }

        private Expression Rol(Expression a, Expression b)
        {
            var intrinsic = host.PseudoProcedure(PseudoProcedure.RolC, a.DataType, a, b);
            return intrinsic;
        }

        private Expression Ror(Expression a, Expression b)
        {
            var intrinsic = host.PseudoProcedure(PseudoProcedure.RorC, a.DataType, a, b);
            return intrinsic;
        }

        private void NZV_(Expression e)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)(FlagM.NF | FlagM.ZF | FlagM.VF));
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(e));
        }

        private void NZVC(Expression e)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)(FlagM.NF | FlagM.ZF | FlagM.VF | FlagM.CF));
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(e));
        }

        private void NZ_C(Expression e)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)(FlagM.NF | FlagM.ZF | FlagM.CF));
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(e));
        }

        private void _ZVC(Expression e)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)(FlagM.ZF | FlagM.VF | FlagM.CF));
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(e));
        }

        private void _Z_C(Expression e)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)(FlagM.ZF | FlagM.CF));
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(e));
        }

        private void NZ0_(Expression e)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)(FlagM.NF | FlagM.ZF));
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(e));
            AssignFlag(FlagM.VF, false);
        }

        private void NZ00(Expression e)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)(FlagM.NF | FlagM.ZF));
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(e));
            AssignFlag(FlagM.VF, false);
            AssignFlag(FlagM.CF, false);
        }

        private void AssignFlag(FlagM flag, bool value)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)flag);
            m.Assign(binder.EnsureFlagGroup(grf), Constant.Bool(value));
        }


        private void RewriteAba()
        {
            var left = binder.EnsureRegister(Registers.a);
            var right = binder.EnsureRegister(Registers.b);
            m.Assign(left, m.IAdd(left, right));
            NZVC(left);
        }

        private void RewriteAdcSbc(RegisterStorage reg, Func<Expression,Expression,Expression> fn)
        {
            var left = binder.EnsureRegister(reg);
            var right = RewriteOp(instr.Operands[0]);
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.ccr, (uint)FlagM.CF));
            m.Assign(left, fn(fn(left, right), C));
            NZVC(left);
        }

        private void RewriteArithmetic(RegisterStorage reg, Func<Expression,Expression,Expression> fn)
        {
            var left = binder.EnsureRegister(reg);
            var right = RewriteOp(instr.Operands[0]);
            m.Assign(left, fn(left, right));
            NZVC(left);
        }

        private void RewriteAndcc()
        {
            var left = binder.EnsureRegister(Registers.ccr);
            var right = RewriteOp(instr.Operands[0]);
            m.Assign(left, m.And(left, right));
        }

        private void RewriteShiftMem(Func<Expression,Expression,Expression> fn)
        {
            var mem = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, fn(mem, m.Int8(1)));
            m.Assign(mem, tmp);
            NZVC(tmp);
        }

        private void RewriteBcc(ConditionCode cc, FlagM flags)
        {
            var grf = arch.GetFlagGroup(Registers.ccr, (uint)flags);
            var addr = ((AddressOperand)instr.Operands[0]).Address;
            m.Branch(
                m.Test(cc, binder.EnsureFlagGroup(grf)),
                addr,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBclr()
        {
            var mem = RewriteOp(instr.Operands[0]);
            var mask = ((Constant)RewriteOp(instr.Operands[1])).Complement();
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.And(mem, mask));
            m.Assign(mem, tmp);
            NZ0_(tmp);
        }

        private void RewriteBgnd()
        {
            var intrinsic = host.PseudoProcedure("__bgnd", VoidType.Instance);
            m.SideEffect(intrinsic);
        }

        private void RewriteBit(RegisterStorage reg)
        {
            var left = binder.EnsureRegister(reg);
            var right = RewriteOp(instr.Operands[0]);
            NZ0_(m.And(left, right));
        }

        private void RewriteBra()
        {
            m.Goto(((AddressOperand)instr.Operands[0]).Address);
        }

        private void RewriteBrclr()
        {
            var mem = RewriteOp(instr.Operands[0]);
            var mask = RewriteOp(instr.Operands[1]);
            var dst = ((AddressOperand)instr.Operands[2]).Address;
            m.Branch(m.Eq0(m.And(mem, mask)), dst, rtlc);
        }

        private void RewriteBrset()
        {
            var mem = RewriteOp(instr.Operands[0]);
            var mask = RewriteOp(instr.Operands[1]);
            var dst = ((AddressOperand)instr.Operands[2]).Address;
            m.Branch(m.Eq0(m.And(m.Comp(mem), mask)), dst, rtlc);
        }


        private void RewriteBset()
        {
            var mem = RewriteOp(instr.Operands[0]);
            var mask = ((Constant)RewriteOp(instr.Operands[1]));
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.Or(mem, mask));
            m.Assign(mem, tmp);
            NZ0_(tmp);
        }

        private void RewriteBsr()
        {
            var dst = ((AddressOperand)instr.Operands[0]).Address;
            m.Call(dst, 2);
        }

        private void RewriteCall()
        {
            var dst = (ushort)((AddressOperand)instr.Operands[0]).Address.ToLinear();
            var page = (ImmediateOperand)instr.Operands[1];
            var addr = Address.SegPtr(page.Value.ToByte(), dst);
            m.Call(addr, 3);
        }

        private void RewriteCba()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = binder.EnsureRegister(Registers.b);
            NZVC(m.ISub(a, b));
        }

        private void RewriteClr()
        {
            var mem = RewriteOp(instr.Operands[0]);
            m.Assign(mem, 0);
            AssignFlag(FlagM.NF, false);
            AssignFlag(FlagM.ZF, true);
            AssignFlag(FlagM.VF, false);
            AssignFlag(FlagM.CF, false);
        }

        private void RewriteClr(RegisterStorage reg)
        {
            var dst = binder.EnsureRegister(reg);
            m.Assign(dst, 0);
            AssignFlag(FlagM.NF, false);
            AssignFlag(FlagM.ZF, true);
            AssignFlag(FlagM.VF, false);
            AssignFlag(FlagM.CF, false);
        }

        private void RewriteCmp(RegisterStorage reg)
        {
            var left = binder.EnsureRegister(reg);
            var right = RewriteOp(instr.Operands[0]);
            right.DataType = left.DataType;
            NZVC(m.ISub(left, right));
        }

        private void RewriteCom(RegisterStorage reg)
        {
            var r = binder.EnsureRegister(reg);
            m.Assign(r, m.Comp(r));
            NZ0_(r);
        }

        private void RewriteCom()
        {
            var mem = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(mem.DataType);
            m.Assign(tmp, m.Comp(mem));
            m.Assign(mem, tmp);
            NZ0_(tmp);
        }

        private void RewriteDaa()
        {
            var a = binder.EnsureRegister(Registers.a);
            var intrinsic = host.PseudoProcedure("__daa", PrimitiveType.Byte, a, m.Out(a.DataType, a));
            NZVC(a);
        }

        private void RewriteDb(Func<Expression, Expression> cmp)
        {
            var reg = RewriteOp(instr.Operands[0]);
            m.Assign(reg, m.ISub(reg, 1));
            m.Branch(cmp(reg), ((AddressOperand) instr.Operands[1]).Address, rtlc);
        }

        private void RewriteEdiv(
            Func<Expression, Expression, Expression> div,
            Func<Expression, Expression, Expression> rem)
        {
            var d = binder.EnsureRegister(Registers.d);
            var x = binder.EnsureRegister(Registers.x);
            var y = binder.EnsureRegister(Registers.y);
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, m.Seq(y, d));
            m.Assign(y, div(tmp, x));
            m.Assign(d, rem(tmp, x));
            NZVC(y);
        }

        private void RewriteEmacs()
        {
            var w16 = PrimitiveType.Word16;
            var left = m.Mem16(binder.EnsureRegister(Registers.x));
            var right = m.Mem16(binder.EnsureRegister(Registers.y));
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, m.SMul(left, right));
            var mem = RewriteOp(instr.Operands[0]);
            mem.DataType = PrimitiveType.Word32;
            m.Assign(tmp, m.IAdd(tmp, mem));
            m.Assign(mem, tmp);
            NZVC(tmp);
        }

        private void RewriteEmaxmind(string fnname)
        {
            var d = binder.EnsureRegister(Registers.d);
            var mem = RewriteOp(instr.Operands[0]);
            m.Assign(d, host.PseudoProcedure(fnname, PrimitiveType.UInt16, d, mem));
            NZVC(d);
        }

        private void RewriteEmaxminm(string fnname)
        {
            var d = binder.EnsureRegister(Registers.d);
            var mem = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(mem.DataType);
            m.Assign(tmp, host.PseudoProcedure(fnname, PrimitiveType.UInt16, d, mem));
            m.Assign(mem, tmp);
            NZVC(tmp);
        }

        private void RewriteEmul(Func<Expression,Expression,Expression> fn)
        {
            var d = binder.EnsureRegister(Registers.d);
            var y = binder.EnsureRegister(Registers.y);
            var e = m.Seq(y, d);
            m.Assign(e, fn(d, y));
            NZVC(e);
        }

        private void RewriteEtbl()
        {
            var b = binder.EnsureRegister(Registers.b);
            var d = binder.EnsureRegister(Registers.d);
            var mem = RewriteMemoryOperand((MemoryOperand)instr.Operands[0]);
            m.Assign(d, host.PseudoProcedure("__etbl", PrimitiveType.Word16,
                mem.EffectiveAddress, b));
            NZ_C(d);
        }

        private void RewriteFdiv()
        {
            var d = binder.EnsureRegister(Registers.d);
            var x = binder.EnsureRegister(Registers.x);
            var tmp = binder.CreateTemporary(PrimitiveType.UInt32);
            m.Assign(tmp, m.Shl(m.Cast(tmp.DataType, d), 16));
            m.Assign(d, m.Remainder(tmp, x));
            m.Assign(x, m.UDiv(tmp, x));
            _ZVC(x);
        }

        private void RewriteIdiv(Func<Expression,Expression,Expression> fn)
        {
            var d = binder.EnsureRegister(Registers.d);
            var x = binder.EnsureRegister(Registers.x);
            var tmp = binder.CreateTemporary(d.DataType);
            m.Assign(tmp, d);
            m.Assign(d, m.Remainder(tmp, x));
            m.Assign(x, fn(tmp, x));
            _Z_C(x);
            AssignFlag(FlagM.VF, false);
        }

        private void RewriteIb(Func<Expression, Expression> cmp)
        {
            var reg = RewriteOp(instr.Operands[0]);
            m.Assign(reg, m.IAdd(reg, 1));
            m.Branch(cmp(reg), ((AddressOperand)instr.Operands[1]).Address, InstrClass.ConditionalTransfer);
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn)
        {
            var mem = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(mem.DataType);
            m.Assign(tmp, fn(mem, m.Int8(1)));
            m.Assign(mem, tmp);
            NZV_(tmp);
        }

        private void RewriteIncDec(RegisterStorage reg, Func<Expression,Expression,Expression> fn)
        {
            var r = binder.EnsureRegister(reg);
            m.Assign(r, fn(r, m.Int8(1)));
            NZV_(r);
        }

        private void RewriteIncDecXY(RegisterStorage reg, Func<Expression, Expression, Expression> fn)
        {
            var r = binder.EnsureRegister(reg);
            m.Assign(r, fn(r, m.Int8(1)));
            var Z = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.ccr, (uint)FlagM.ZF));
            m.Assign(Z, m.Cond(r));
        }

     
        private void RewriteJmp()
        {
            var mem = RewriteMemoryOperand((MemoryOperand)instr.Operands[0]);
            m.Goto(mem.EffectiveAddress);
        }

        private void RewriteJsr()
        {
            var mem = RewriteMemoryOperand((MemoryOperand)instr.Operands[0]);
            m.Call(mem.EffectiveAddress, 2);
        }

        private void RewriteLd(RegisterStorage reg)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = binder.EnsureRegister(reg);
            src.DataType = dst.DataType;
            m.Assign(dst, src);
            NZ0_(dst);
        }

        private void RewriteLea(RegisterStorage reg)
        {
            var dst = binder.EnsureRegister(reg);
            var mem = RewriteMemoryOperand((MemoryOperand)instr.Operands[0]);
            m.Assign(dst, mem.EffectiveAddress);
        }

        private void RewriteLogical(RegisterStorage reg, Func<Expression, Expression, Expression> fn)
        {
            var left = binder.EnsureRegister(reg);
            var right = RewriteOp(instr.Operands[0]);
            m.Assign(left, fn(left, right));
            NZ0_(left);
        }

        private void RewriteMaxmina(string fnname)
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOp(instr.Operands[0]);
            m.Assign(a, host.PseudoProcedure(fnname, PrimitiveType.Byte, a, mem));
            NZVC(a);
        }

        private void RewriteMem()
        {
            var a = binder.EnsureRegister(Registers.a);
            var x = binder.EnsureRegister(Registers.x);
            var y = binder.EnsureRegister(Registers.y);
            var intrinsic = host.PseudoProcedure("__membership", VoidType.Instance,
                a, x, y,
                m.Out(x.DataType, x),
                m.Out(y.DataType, y));
            m.SideEffect(intrinsic);
        }

        private void RewriteMaxminm(string fnname)
        {
            var a = binder.EnsureRegister(Registers.a);
            var mem = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(mem.DataType);
            m.Assign(tmp, host.PseudoProcedure(fnname, PrimitiveType.Byte, a, mem));
            m.Assign(mem, tmp);
            NZVC(tmp);
        }

        private void RewriteMul()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = binder.EnsureRegister(Registers.b);
            var d = binder.EnsureRegister(Registers.d);
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.ccr, (uint)FlagM.CF));
            m.Assign(d, m.UMul(a, b));
            m.Assign(C, m.Cond(d));
        }

        private void RewriteNeg(RegisterStorage reg)
        {
            var r = binder.EnsureRegister(reg);
            m.Assign(r, m.Neg(r));
            NZVC(r);
        }

        private void RewriteNeg()
        {
            var mem = RewriteOp(instr.Operands[0]);
            var tmp = binder.CreateTemporary(mem.DataType);
            m.Assign(tmp, m.Neg(mem));
            m.Assign(mem, tmp);
            NZVC(tmp);
        }

        private void RewriteOrcc()
        {
            var left = binder.EnsureRegister(Registers.ccr);
            var right = RewriteOp(instr.Operands[0]);
            m.Assign(left, m.Or(left, right));
        }

        private void RewritePsh(RegisterStorage reg)
        {
            var val = binder.EnsureRegister(reg);
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISubS(sp, (short)val.DataType.Size));
            m.Assign(m.Mem(val.DataType, sp), val);
        }

        private void RewritePul(RegisterStorage reg)
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var dst = binder.EnsureRegister(reg);
            m.Assign(dst, m.Mem(reg.DataType, sp));
            m.Assign(sp, m.IAdd(sp, (short)dst.DataType.Size));
        }

        private void RewriteRtc()
        {
            m.Return(3, 0);
        }

        private void RewriteRti()
        {
            m.Return(9, 0);
        }

        private void RewriteRts()
        {
            m.Return(2, 0);
        }

        private void RewriteSba()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = binder.EnsureRegister(Registers.b);
            m.Assign(a, m.ISub(a, b));
            NZVC(a);
        }

        private void RewriteSex()
        {
            var mem = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            m.Assign(dst, m.Cast(PrimitiveType.Int16, m.Cast(PrimitiveType.SByte, mem)));
        }

        private void RewriteSt(RegisterStorage reg)
        {
            var src = binder.EnsureRegister(reg);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, src);
            NZ0_(src);
        }

        private void RewriteStop()
        {
            var intrinsic = host.PseudoProcedure("__stop", VoidType.Instance);
            m.SideEffect(intrinsic);
        }

        private void RewriteSub(RegisterStorage reg)
        {
            var left = binder.EnsureRegister(reg);
            var right = RewriteOp(instr.Operands[0]);
            m.Assign(left, m.ISub(left, right));
            NZVC(left);
        }

        private void RewriteSwi()
        {
            var intrinsic = host.PseudoProcedure("__swi", VoidType.Instance);
            m.SideEffect(intrinsic);
        }
        private void RewriteTab()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = binder.EnsureRegister(Registers.b);
            m.Assign(b, a);
            NZ0_(b);
        }

        private void RewriteTb(Func<Expression, Expression> test)
        {
            var src = RewriteOp(instr.Operands[0]);
            var addr = ((AddressOperand)instr.Operands[1]).Address;
            m.Branch(test(src), addr, rtlc);
        }

        private void RewriteTba()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = binder.EnsureRegister(Registers.b);
            m.Assign(a, b);
            NZ0_(b);
        }

        private void RewriteTfr()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteTrap()
        {
            var intrinsic = host.PseudoProcedure("__swi", VoidType.Instance);
            m.SideEffect(intrinsic);
        }

        private void RewriteTst(RegisterStorage reg)
        {
            var src = binder.EnsureRegister(reg);
            NZ00(m.ISub(src, 0));
        }

        private void RewriteTst()
        {
            var mem = RewriteOp(instr.Operands[0]);
            NZ00(m.ISub(mem, 0));
        }

        private void RewriteWai()
        {
            var intrinsic = host.PseudoProcedure("__wai", VoidType.Instance);
            m.SideEffect(intrinsic);
        }

        private void RewriteWav()
        {
            var b = binder.EnsureRegister(Registers.b);
            var x = binder.EnsureRegister(Registers.x);
            var y = binder.EnsureRegister(Registers.y);
            var intrinsic = host.PseudoProcedure("__wav",
                VoidType.Instance,
                b, x, y,
                m.Out(b.DataType, b), m.Out(x.DataType, x), m.Out(y.DataType, y));
            m.SideEffect(intrinsic);
            AssignFlag(FlagM.ZF, true);
        }
    }
}
