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
using System.Text;

namespace Reko.Arch.Pdp11
{
    public partial class Pdp11Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Pdp11Architecture arch;
        private readonly IEnumerator<Pdp11Instruction> dasm;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private Pdp11Instruction instr;
        private InstrClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private RtlEmitter m;

        public Pdp11Rewriter(
            Pdp11Architecture arch,
            IEnumerable<Pdp11Instruction> instrs,
            IStorageBinder binder,
            IRewriterHost host)
        {
            this.arch = arch;
            this.dasm = instrs.GetEnumerator();
            this.binder = binder;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.rtlInstructions = new List<RtlInstruction>();
                this.rtlc = instr.InstructionClass;
                m = new RtlEmitter(this.rtlInstructions);
                switch (instr.Opcode)
                {
                default:
                    host.Warn(
                        instr.Address,
                        "PDP-11 instruction {0} is not supported yet.", 
                        instr.Opcode);
                    rtlc = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.illegal: rtlc = InstrClass.Invalid; m.Invalid(); break;
                case Opcode.adc: RewriteAdcSbc(m.IAdd); break;
                case Opcode.add: RewriteAdd(); break;
                case Opcode.addb: RewriteAdd(); break;
                case Opcode.ash: RewriteShift(); break;
                case Opcode.ashc: RewriteAshc(); break;
                case Opcode.asl: RewriteAsl(); break;
                case Opcode.asr: RewriteAsr(); break;
                case Opcode.bcc: RewriteBxx(ConditionCode.UGE, FlagM.CF); break;
                case Opcode.bcs: RewriteBxx(ConditionCode.ULT, FlagM.CF); break;
                case Opcode.beq: RewriteBxx(ConditionCode.EQ, FlagM.ZF); break;
                case Opcode.bge: RewriteBxx(ConditionCode.GE, FlagM.VF|FlagM.NF); break;
                case Opcode.bgt: RewriteBxx(ConditionCode.GT, FlagM.ZF|FlagM.NF|FlagM.VF); break;
                case Opcode.bhi: RewriteBxx(ConditionCode.UGT, FlagM.ZF|FlagM.CF); break;
                case Opcode.bvs: RewriteBxx(ConditionCode.OV, FlagM.VF); break;
                case Opcode.bic: RewriteBic(); break;
                case Opcode.bis: RewriteBis(); break;
                case Opcode.bisb: RewriteBis(); break;
                case Opcode.bit: RewriteBit(); break;
                case Opcode.bitb: RewriteBit(); break;
                case Opcode.ble: RewriteBxx(ConditionCode.LE, FlagM.ZF|FlagM.NF|FlagM.VF); break;
                case Opcode.blos: RewriteBxx(ConditionCode.ULE, FlagM.ZF | FlagM.CF); break;
                case Opcode.blt: RewriteBxx(ConditionCode.LT, FlagM.NF|FlagM.VF); break;
                case Opcode.bmi: RewriteBxx(ConditionCode.LT, FlagM.NF); break;
                case Opcode.bne: RewriteBxx(ConditionCode.NE, FlagM.ZF); break;
                case Opcode.bpl: RewriteBxx(ConditionCode.GT, FlagM.NF); break;
                case Opcode.bpt: RewriteBpt(); break;
                case Opcode.br: RewriteBr(); break;
                case Opcode.clr: RewriteClr(instr, m.Word16(0)); break;
                case Opcode.clrb: RewriteClr(instr, m.Byte(0)); break;
                case Opcode.clrflags: RewriteClrSetFlags(Constant.False); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.com: RewriteCom(); break;
                case Opcode.dec: RewriteIncDec(m.ISub); break;
                case Opcode.div: RewriteDiv(); break;
                case Opcode.emt: RewriteEmt(); break;
                case Opcode.halt: RewriteHalt(); break;
                case Opcode.iot: RewriteIot(); break;
                case Opcode.inc: RewriteIncDec(m.IAdd); break;
                case Opcode.jmp: RewriteJmp(); break;
                case Opcode.jsr: RewriteJsr(); break;
                case Opcode.mark: RewriteMark(); break;
                case Opcode.mfpd: RewriteMfpd(); break;
                case Opcode.mfpi: RewriteMfpi(); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.movb: RewriteMov(); break;
                case Opcode.mtpi: RewriteMtpi(); break;
                case Opcode.mul: RewriteMul(); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.reset: RewriteReset(); break;
                case Opcode.rol: RewriteRotate(PseudoProcedure.Rol); break;
                case Opcode.ror: RewriteRotate(PseudoProcedure.Ror); break;
                case Opcode.rti: RewriteRti(); break;
                case Opcode.rts: RewriteRts(); break;
                case Opcode.rtt: RewriteRtt(); break;
                case Opcode.sbc: RewriteAdcSbc(m.ISub); break;
                case Opcode.setflags: RewriteClrSetFlags(Constant.True); break;
                case Opcode.stcdi: RewriteStcdi(); break;
                case Opcode.sob: RewriteSob(); break;
                case Opcode.stexp: RewriteStexp(); break;
                case Opcode.sub: RewriteSub(); break;
                case Opcode.swab: RewriteSwab(); break;
                case Opcode.sxt: RewriteSxt(); break;
                case Opcode.trap: RewriteTrap(); break;
                case Opcode.tst: RewriteTst(); break;
                case Opcode.tstb: RewriteTst(); break;
                case Opcode.wait: RewriteWait(); break;
                case Opcode.xor: RewriteXor(); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, this.rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void SetFlags(Expression e, FlagM changed, FlagM zeroed, FlagM set)
        {
            if (e == null)
            {
                Invalid();
                return;
            }
            uint uChanged = (uint)changed;
            if (uChanged != 0)
            {
                var grfChanged = binder.EnsureFlagGroup(this.arch.GetFlagGroup(uChanged));
                m.Assign(grfChanged, m.Cond(e));
            }
            uint grfMask = 1;
            while (grfMask <= (uint)zeroed)
            {
                if ((grfMask & (uint)zeroed) != 0)
                {
                    var grfZeroed = binder.EnsureFlagGroup(this.arch.GetFlagGroup(grfMask));
                    m.Assign(grfZeroed, 0);
                }
                grfMask <<= 1;
            }
            grfMask = 1;
            while (grfMask <= (uint)set)
            {
                if ((grfMask & (uint)set) != 0)
                {
                    var grfZeroed = binder.EnsureFlagGroup(this.arch.GetFlagGroup(grfMask));
                    m.Assign(grfZeroed, 1);
                }
                grfMask <<= 1;
            }
        }

        private Expression RewriteJmpSrc(MachineOperand op)
        {
            var memOp = op as MemoryOperand;
            if (memOp == null)
            {
                // PDP-11 always has a memory reference 
                // for the destination of a transfer instruction.
                return null;
            }
            var r = memOp.Register != null
                ? binder.EnsureRegister(memOp.Register)
                : null;
            var tmp = binder.CreateTemporary(op.Width);
            switch (memOp.Mode)
            {
            default:
                throw new AddressCorrelatedException(
                    dasm.Current.Address,
                    "Not implemented: addressing mode {0}.",
                    memOp.Mode);
            case AddressMode.RegDef:
                return r;
            case AddressMode.Absolute:
                return Address.Ptr16(memOp.EffectiveAddress);
            case AddressMode.AutoIncr:
                m.Assign(tmp, m.Mem(PrimitiveType.Ptr16, r));
                m.Assign(r, m.IAdd(r, memOp.Width.Size));
                break;
            case AddressMode.AutoIncrDef:
                m.Assign(tmp, m.Mem(op.Width, r));
                m.Assign(r, m.IAdd(r, memOp.Width.Size));
                break;
            case AddressMode.AutoDecr:
                m.Assign(r, m.ISub(r, memOp.Width.Size));
                return m.Mem(op.Width, r);
            case AddressMode.AutoDecrDef:
                m.Assign(r, m.ISub(r, memOp.Width.Size));
                m.Assign(tmp, m.Mem(op.Width, m.Mem(PrimitiveType.Ptr16, r)));
                return tmp;
            case AddressMode.Indexed:
                if (memOp.Register == Registers.pc)
                {
                    var offset = (short)memOp.EffectiveAddress;
                    var addrBase = (long)instr.Address.ToLinear();
                    var addr = Address.Ptr16((ushort)(instr.Length + addrBase + offset));
                    return addr;
                }
                else
                {
                    return m.Mem(
                        this.dasm.Current.DataWidth,
                        m.IAdd(r, Constant.Word16(memOp.EffectiveAddress)));
                }
            case AddressMode.IndexedDef:
                if (memOp.Register == Registers.pc)
                {
                    var offset = (short)memOp.EffectiveAddress;
                    var addrBase = (long)instr.Address.ToLinear() + instr.Length;
                    var addr = Constant.Word16((ushort) (addrBase + offset));
                    return m.Mem(
                        PrimitiveType.Word16,
                        addr);
                }
                else
                {
                    return m.Mem(
                        PrimitiveType.Ptr16,
                        m.IAdd(r, Constant.Word16(memOp.EffectiveAddress)));
                }
            }
            return tmp;
        /*
            var immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                return immOp.Value;
            }
            var addrOp = op as AddressOperand;
            if (addrOp != null)
            {
                return addrOp.Address;
            }
            throw new NotImplementedException();
            */
        }

        private Expression RewriteSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand regOp:
                if (regOp.Register == Registers.pc)
                    return instr.Address + instr.Length;
                else
                    return binder.EnsureRegister(regOp.Register);
            case ImmediateOperand immOp:
                if (dasm.Current.DataWidth.Size == 1)
                {
                    return Constant.Byte((byte) immOp.Value.ToInt32());
                }
                else
                {
                    return immOp.Value;
                }
            case AddressOperand addrOp:
                return addrOp.Address;
            case MemoryOperand memOp:
                var r = binder.EnsureRegister(memOp.Register);
                var tmp = binder.CreateTemporary(op.Width);
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        dasm.Current.Address,
                        "Not implemented: addressing mode {0}.", 
                        memOp.Mode);
                case AddressMode.RegDef:
                    return m.Mem(this.dasm.Current.DataWidth, r);
                case AddressMode.Absolute:
                    return m.Mem(
                           dasm.Current.DataWidth,
                           Address.Ptr16(memOp.EffectiveAddress));
                case AddressMode.AutoIncr:
                    m.Assign(tmp, m.Mem(op.Width, r));
                    m.Assign(r, m.IAdd(r, memOp.Width.Size));
                    break;
                case AddressMode.AutoIncrDef:
                    m.Assign(tmp, m.Mem(op.Width, m.Mem(PrimitiveType.Ptr16, r)));
                    m.Assign(r, m.IAdd(r, memOp.Width.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r, m.ISub(r, memOp.Width.Size));
                    return m.Mem(op.Width, r);
                case AddressMode.AutoDecrDef:
                    m.Assign(r, m.ISub(r, memOp.Width.Size));
                    m.Assign(tmp, m.Mem(op.Width, m.Mem(PrimitiveType.Ptr16, r)));
                    return tmp;
                case AddressMode.Indexed:
                    if (memOp.Register == Registers.pc)
                    {
                        var offset = (short)memOp.EffectiveAddress;
                        var addrBase = (long) instr.Address.ToLinear();
                        var addr = Address.Ptr16((ushort)(2 + addrBase + offset));
                        return m.Mem(memOp.Width, addr);
                    }
                    else
                    {
                        return m.Mem(
                            this.dasm.Current.DataWidth,
                            m.IAdd(r, Constant.Word16(memOp.EffectiveAddress)));
                    }
                case AddressMode.IndexedDef:
                    if (memOp.Register == Registers.pc)
                    {
                        var addr = this.dasm.Current.Address + this.dasm.Current.Length + memOp.EffectiveAddress;
                        m.Assign(tmp, m.Mem(PrimitiveType.Ptr16, addr));
                        m.Assign(tmp, m.Mem(memOp.Width, tmp));
                        return tmp;
                    }
                    else
                    {
                    return m.Mem(
                        this.dasm.Current.DataWidth,
                        m.Mem(
                            PrimitiveType.Ptr16,
                            m.IAdd(r, Constant.Word16(memOp.EffectiveAddress))));
                    }
                }
                return tmp;
            }
            throw new NotImplementedException();
        }

        // Rewrites a destination operand when the source is unary.
        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression> gen)
        {
            switch (op)
            {
            case RegisterOperand regOp:
                var dst = binder.EnsureRegister(regOp.Register);
                src = gen(src);
                if (src.DataType.Size < dst.DataType.Size)
                {
                    src = m.Dpb(dst, src, 0);
                }
                m.Assign(dst, src);
                return dst;
            case MemoryOperand memOp:
                var r = binder.EnsureRegister(memOp.Register);
                Expression tmp = MaybeAssignTmp(gen(src));
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        dasm.Current.Address,
                        "Not implemented: addressing mode {0}.",
                        memOp.Mode);
                case AddressMode.Absolute:
                    m.Assign(
                        m.Mem(
                            dasm.Current.DataWidth,
                            Address.Ptr16(memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.RegDef:
                    m.Assign(m.Mem(tmp.DataType, r), tmp);
                    break;
                case AddressMode.AutoIncr:
                    m.Assign(m.Mem(tmp.DataType, r), tmp);
                    m.Assign(r, m.IAdd(r, tmp.DataType.Size));
                    break;
                case AddressMode.AutoIncrDef:
                    m.Assign(m.Mem(PrimitiveType.Ptr16, m.Mem(tmp.DataType, r)), tmp);
                    m.Assign(r, m.IAdd(r, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r, m.ISub(r, tmp.DataType.Size));
                    m.Assign(m.Mem(tmp.DataType, r), tmp);
                    break;
                case AddressMode.AutoDecrDef:
                    m.Assign(r, m.ISub(r, tmp.DataType.Size));
                    m.Assign(
                        m.Mem(
                            tmp.DataType, 
                            m.Mem(PrimitiveType.Ptr16, r)),
                        tmp);
                    break;
                case AddressMode.Indexed:
                    if (r.Storage == Registers.pc)
                    {
                        var addr = dasm.Current.Address + dasm.Current.Length + memOp.EffectiveAddress;
                        m.Assign(
                            m.Mem(dasm.Current.DataWidth, addr),
                            tmp);
                    }
                    else
                    {
                        m.Assign(
                            m.Mem(
                                this.dasm.Current.DataWidth,
                                m.IAdd(
                                    r,
                                    Constant.Word16(memOp.EffectiveAddress))),
                            tmp);
                    }
                    break;
                case AddressMode.IndexedDef:
                    if (r.Storage == Registers.pc)
                    {
                        //$REVIEW: what if there are two of these?
                        var addr = dasm.Current.Address + dasm.Current.Length + memOp.EffectiveAddress;
                        var deferred = binder.CreateTemporary(PrimitiveType.Ptr16);
                        m.Assign(
                            deferred,
                            m.Mem(PrimitiveType.Ptr16, addr));
                        m.Assign(
                            m.Mem(dasm.Current.DataWidth, deferred),
                            tmp);
                    }
                    else
                    {
                        m.Assign(
                            m.Mem(
                                PrimitiveType.Ptr16,
                                m.Mem(
                                    this.dasm.Current.DataWidth,
                                    m.IAdd(
                                        r,
                                        Constant.Word16(memOp.EffectiveAddress)))),
                            tmp);
                    }
                    break;
                }
                return tmp;
            }
            return null;
        }

        private Expression MaybeAssignTmp(Expression exp)
        {
            if (exp is Constant || exp is Identifier || exp is Address)
                return exp;
            var tmp = binder.CreateTemporary(exp.DataType);
            m.Assign(tmp, exp);
            return tmp;
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression, Expression> gen)
        {
            switch (op)
            {
            case RegisterOperand regOp:
                var dst = binder.EnsureRegister(regOp.Register);
                m.Assign(dst, gen(dst, src));
                return dst;
            case MemoryOperand memOp:
                var r = binder.EnsureRegister(memOp.Register);
                var tmp = binder.CreateTemporary(dasm.Current.DataWidth);
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        dasm.Current.Address,
                        "Not implemented: addressing mode {0}.",
                        memOp.Mode);
                case AddressMode.RegDef:
                    m.Assign(tmp, gen(m.Mem(tmp.DataType, r), src));
                    m.Assign(m.Mem(tmp.DataType, r), tmp);
                    break;
                case AddressMode.AutoIncr:
                    m.Assign(tmp, gen(m.Mem(tmp.DataType, r), src));
                    m.Assign(m.Mem(tmp.DataType, r), tmp);
                    m.Assign(r, m.IAdd(r, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r, m.ISub(r, tmp.DataType.Size));
                    m.Assign(tmp, gen(m.Mem(tmp.DataType, r), src));
                    m.Assign(m.Mem(tmp.DataType, r), tmp);
                    break;
                case AddressMode.AutoIncrDef:
                    m.Assign(tmp, gen(m.Mem(PrimitiveType.Ptr16, m.Mem(tmp.DataType, r)), src));
                    m.Assign(m.Mem(tmp.DataType, r), tmp);
                    m.Assign(r, m.IAdd(r, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecrDef:
                    m.Assign(r, m.ISub(r, tmp.DataType.Size));
                    m.Assign(tmp, gen(m.Mem(tmp.DataType, m.Mem(PrimitiveType.Ptr16, r)), src));
                    m.Assign(m.Mem(tmp.DataType, m.Mem(PrimitiveType.Ptr16, r)), tmp);
                    break;
                case AddressMode.Absolute:
                    m.Assign(
                        tmp,
                        gen(
                            m.Mem(
                                dasm.Current.DataWidth,
                                Address.Ptr16(memOp.EffectiveAddress)),
                            src));
                    m.Assign(
                        m.Mem(
                           dasm.Current.DataWidth,
                           Address.Ptr16(memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.Indexed:
                    m.Assign(
                        tmp,
                        gen(
                            m.Mem(
                                dasm.Current.DataWidth,
                                m.IAdd(
                                    r, memOp.EffectiveAddress)),
                            src));
                    m.Assign(
                        m.Mem(
                            dasm.Current.DataWidth,
                            m.IAdd(
                                r, memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.IndexedDef:
                    if (r.Storage == Registers.pc)
                    {
                        //$REVIEW: what if there are two of these?
                        var addr = dasm.Current.Address + dasm.Current.Length;
                        m.Assign(
                            tmp,
                            gen(
                                m.Mem(dasm.Current.DataWidth, addr),
                                src));
                        m.Assign(
                            m.Mem(dasm.Current.DataWidth, addr),
                            tmp);
                    }
                    else
                    {
                        m.Assign(
                           tmp,
                           gen(
                               m.Mem(
                                   dasm.Current.DataWidth,
                                   m.Mem(
                                       PrimitiveType.Ptr16,
                                       m.IAdd(
                                           r, memOp.EffectiveAddress))),
                                   src));
                        m.Assign(
                            m.Mem(
                                dasm.Current.DataWidth,
                                m.Mem(
                                    PrimitiveType.Ptr16,
                                    m.IAdd(
                                        r, memOp.EffectiveAddress))),
                            tmp);
                    }
                    break;
                }
                return tmp;
            }
            return null;
        }

        private void Invalid()
        {
            rtlInstructions.Clear();
            rtlc = InstrClass.Invalid;
            m.Invalid();
        }
    }
}
