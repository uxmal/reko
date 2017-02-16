#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        private Pdp11Architecture arch;
        private IEnumerator<Pdp11Instruction> instrs;
        private Frame frame;
        private RtlInstructionCluster rtlCluster;
        private RtlEmitter m;
        private IRewriterHost host;

        public Pdp11Rewriter(
            Pdp11Architecture arch,
            IEnumerable<Pdp11Instruction> instrs,
            Frame frame,
            IRewriterHost host)
        {
            this.arch = arch;
            this.instrs = instrs.GetEnumerator();
            this.frame = frame;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (instrs.MoveNext())
            {
                var instr = instrs.Current;
                this.rtlCluster = new RtlInstructionCluster(instr.Address, instr.Length);
                this.rtlCluster.Class = RtlClass.Linear;
                m = new RtlEmitter(rtlCluster.Instructions);
                switch (instr.Opcode)
                {
                default: throw new AddressCorrelatedException(
                    instr.Address,
                    "Rewriting of PDP-11 instruction {0} not supported yet.", instr.Opcode);
                case Opcode.adc: RewriteAdc(instr); break;
                case Opcode.add: RewriteAdd(instr); break;
                case Opcode.addb: RewriteAdd(instr); break;
                case Opcode.ash: RewriteShift(instr); break;
                case Opcode.asl: RewriteAsl(instr); break;
                case Opcode.asr: RewriteAsr(instr); break;
                case Opcode.bcc: RewriteBxx(instr, ConditionCode.UGE, FlagM.CF); break;
                case Opcode.bcs: RewriteBxx(instr, ConditionCode.ULT, FlagM.CF); break;
                case Opcode.beq: RewriteBxx(instr, ConditionCode.EQ, FlagM.ZF); break;
                case Opcode.bge: RewriteBxx(instr, ConditionCode.GE, FlagM.VF|FlagM.NF); break;
                case Opcode.bgt: RewriteBxx(instr, ConditionCode.GT, FlagM.ZF|FlagM.NF|FlagM.VF); break;
                case Opcode.bhi: RewriteBxx(instr, ConditionCode.UGT, FlagM.ZF|FlagM.CF); break;
                case Opcode.bic: RewriteBic(instr); break;
                case Opcode.bis: RewriteBis(instr); break;
                case Opcode.bisb: RewriteBis(instr); break;
                case Opcode.bit: RewriteBit(instr); break;
                case Opcode.bitb: RewriteBit(instr); break;
                case Opcode.ble: RewriteBxx(instr, ConditionCode.LE, FlagM.ZF|FlagM.NF|FlagM.VF); break;
                case Opcode.blos: RewriteBxx(instr, ConditionCode.ULE, FlagM.ZF | FlagM.CF); break;
                case Opcode.blt: RewriteBxx(instr, ConditionCode.LT, FlagM.NF|FlagM.VF); break;
                case Opcode.bmi: RewriteBxx(instr, ConditionCode.LT, FlagM.NF); break;
                case Opcode.bne: RewriteBxx(instr, ConditionCode.NE, FlagM.ZF); break;
                case Opcode.bpl: RewriteBxx(instr, ConditionCode.GT, FlagM.NF); break;
                case Opcode.br: RewriteBr(instr); break;
                case Opcode.clr: RewriteClr(instr, m.Word16(0)); break;
                case Opcode.clrb: RewriteClr(instr, m.Byte(0)); break;
                case Opcode.cmp: RewriteCmp(instr); break;
                case Opcode.com: RewriteCom(instr); break;
                case Opcode.dec: RewriteIncDec(instr, m.ISub); break;
                case Opcode.div: RewriteDiv(instr); break;
                case Opcode.emt: RewriteEmt(instr); break;
                case Opcode.halt: RewriteHalt(); break;
                case Opcode.inc: RewriteIncDec(instr, m.IAdd); break;
                case Opcode.jmp: RewriteJmp(instr); break;
                case Opcode.jsr: RewriteJsr(instr); break;
                case Opcode.mov: RewriteMov(instr); break;
                case Opcode.movb: RewriteMov(instr); break;
                case Opcode.neg: RewriteNeg(instr); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.reset: RewriteReset(); break;
                case Opcode.rol: RewriteRol(instr); break;
                case Opcode.rts: RewriteRts(instr); break;
                case Opcode.stcdi: RewriteStcdi(instr); break;
                case Opcode.sub: RewriteSub(instr); break;
                case Opcode.sxt: RewriteSxt(instr); break;
                case Opcode.trap: RewriteTrap(instr); break;
                case Opcode.tst: RewriteTst(instr); break;
                case Opcode.tstb: RewriteTst(instr); break;
                case Opcode.wait: RewriteWait(); break;
                case Opcode.xor: RewriteXor(instr); break;
                }
                yield return rtlCluster;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void SetFlags(Expression e, FlagM changed, FlagM zeroed, FlagM set)
        {
            uint uChanged = (uint)changed;
            if (uChanged != 0)
            {
                var grfChanged = frame.EnsureFlagGroup(this.arch.GetFlagGroup(uChanged));
                m.Assign(grfChanged, m.Cond(e));
            }
            uint grfMask = 1;
            while (grfMask <= (uint)zeroed)
            {
                if ((grfMask & (uint)zeroed) != 0)
                {
                    var grfZeroed = frame.EnsureFlagGroup(this.arch.GetFlagGroup(grfMask));
                    m.Assign(grfZeroed, 0);
                }
                grfMask <<= 1;
            }
            grfMask = 1;
            while (grfMask <= (uint)set)
            {
                if ((grfMask & (uint)set) != 0)
                {
                    var grfZeroed = frame.EnsureFlagGroup(this.arch.GetFlagGroup(grfMask));
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
                throw new AddressCorrelatedException(
                      instrs.Current.Address,
                      "Invalid addressing mode for transfer functions.",
                      memOp.Mode);
            }
            var r = frame.EnsureRegister(memOp.Register);
            var tmp = frame.CreateTemporary(op.Width);
            switch (memOp.Mode)
            {
            default:
                throw new AddressCorrelatedException(
                    instrs.Current.Address,
                    "Not implemented: addressing mode {0}.",
                    memOp.Mode);
            case AddressMode.RegDef:
                return r;
            case AddressMode.Absolute:
                return Address.Ptr16(memOp.EffectiveAddress);
            case AddressMode.AutoIncr:
                m.Assign(tmp, m.Load(op.Width, m.Load(PrimitiveType.Ptr16, r)));
                m.Assign(r, m.IAdd(r, memOp.Width.Size));
                break;
            case AddressMode.AutoIncrDef:
                m.Assign(tmp, m.Load(op.Width, r));
                m.Assign(r, m.IAdd(r, memOp.Width.Size));
                break;
            case AddressMode.AutoDecr:
                m.Assign(r, m.ISub(r, memOp.Width.Size));
                return m.Load(op.Width, r);
            case AddressMode.AutoDecrDef:
                m.Assign(r, m.ISub(r, memOp.Width.Size));
                m.Assign(tmp, m.Load(op.Width, m.Load(PrimitiveType.Ptr16, r)));
                return tmp;
            case AddressMode.Indexed:
                if (memOp.Register == Registers.pc)
                {
                    var offset = (short)memOp.EffectiveAddress;
                    var addrBase = (long)rtlCluster.Address.ToLinear();
                    var addr = Address.Ptr16((ushort)(2 + addrBase + offset));
                    return addr;
                }
                else
                {
                    return m.Load(
                        this.instrs.Current.DataWidth,
                        m.IAdd(r, Constant.Word16(memOp.EffectiveAddress)));
                }
            case AddressMode.IndexedDef:
                if (memOp.Register == Registers.pc)
                {
                    var offset = (short)memOp.EffectiveAddress;
                    var addrBase = (long)rtlCluster.Address.ToLinear() + rtlCluster.Length;
                    var addr = Constant.Word16((ushort) (addrBase + offset));
                    return m.Load(
                        PrimitiveType.Word16,
                        addr);
                }
                else
                {
                    return m.Load(
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
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                var r = frame.EnsureRegister(memOp.Register);
                var tmp = frame.CreateTemporary(op.Width);
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        instrs.Current.Address,
                        "Not implemented: addressing mode {0}.", 
                        memOp.Mode);
                case AddressMode.RegDef:
                    return m.Load(this.instrs.Current.DataWidth, r);
                case AddressMode.Absolute:
                    return m.Load(
                           instrs.Current.DataWidth,
                           Address.Ptr16(memOp.EffectiveAddress));
                case AddressMode.AutoIncr:
                    m.Assign(tmp, m.Load(op.Width, r));
                    m.Assign(r, m.IAdd(r, memOp.Width.Size));
                    break;
                case AddressMode.AutoIncrDef:
                    m.Assign(tmp, m.Load(op.Width, m.Load(PrimitiveType.Ptr16, r)));
                    m.Assign(r, m.IAdd(r, memOp.Width.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r, m.ISub(r, memOp.Width.Size));
                    return m.Load(op.Width, r);
                case AddressMode.AutoDecrDef:
                    m.Assign(r, m.ISub(r, memOp.Width.Size));
                    m.Assign(tmp, m.Load(op.Width, m.Load(PrimitiveType.Ptr16, r)));
                    return tmp;
                case AddressMode.Indexed:
                    if (memOp.Register == Registers.pc)
                    {
                        var offset = (short)memOp.EffectiveAddress;
                        var addrBase = (long) rtlCluster.Address.ToLinear();
                        var addr = Address.Ptr16((ushort)(2 + addrBase + offset));
                        return m.Load(memOp.Width, addr);
                    }
                    else
                    {
                        return m.Load(
                            this.instrs.Current.DataWidth,
                            m.IAdd(r, Constant.Word16(memOp.EffectiveAddress)));
                    }
                case AddressMode.IndexedDef:
                    return m.Load(
                        this.instrs.Current.DataWidth,
                        m.Load(
                            PrimitiveType.Ptr16,
                            m.IAdd(r, Constant.Word16(memOp.EffectiveAddress))));
                }
                return tmp;
            }
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                return frame.EnsureRegister(regOp.Register);
            }
            var immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                if (instrs.Current.DataWidth.Size == 1)
                {
                    return Constant.Byte((byte)immOp.Value.ToInt32());
                }
                else
                {
                    return immOp.Value;
                }
            }
            var addrOp = op as AddressOperand;
            if (addrOp != null)
            {
                return addrOp.Address;
            }
            throw new NotImplementedException();
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression> gen)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                var dst = frame.EnsureRegister(regOp.Register);
                src = gen(src);
                if (src.DataType.Size < dst.DataType.Size)
                {
                    src = m.Dpb(dst, src, 0);
                }
                m.Assign(dst, src);
                return dst;
            }
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                var r = frame.EnsureRegister(memOp.Register);
                var tmp = frame.CreateTemporary(instrs.Current.DataWidth);
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        instrs.Current.Address,
                        "Not implemented: addressing mode {0}.",
                        memOp.Mode);
                case AddressMode.Absolute:
                    m.Assign(
                        m.Load(
                            instrs.Current.DataWidth,
                            Address.Ptr16(memOp.EffectiveAddress)),
                        gen(src));
                    break;
                case AddressMode.RegDef:
                    m.Assign(m.Load(tmp.DataType, r), gen(src));
                    break;
                case AddressMode.AutoIncr:
                    m.Assign(tmp, gen(src));
                    m.Assign(m.Load(tmp.DataType, r), tmp);
                    m.Assign(r, m.IAdd(r, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r, m.ISub(r, tmp.DataType.Size));
                    m.Assign(m.Load(tmp.DataType, r), gen(src));
                    break;
                case AddressMode.AutoDecrDef:
                    m.Assign(r, m.ISub(r, tmp.DataType.Size));
                    m.Assign(
                        m.Load(
                            tmp.DataType, 
                            m.Load(PrimitiveType.Ptr16, r)),
                        gen(src));
                    break;
                case AddressMode.Indexed:
                    if (r.Storage == Registers.pc)
                    {
                        var addr = instrs.Current.Address + instrs.Current.Length;
                        m.Assign(
                           tmp,
                           gen(m.Load(instrs.Current.DataWidth, addr)));
                        m.Assign(
                            m.Load(instrs.Current.DataWidth, addr),
                            tmp);
                    }
                    else
                    {
                        m.Assign(
                            m.Load(
                                this.instrs.Current.DataWidth,
                                m.IAdd(
                                    r,
                                    Constant.Word16(memOp.EffectiveAddress))),
                            gen(src));
                    }
                    break;
                case AddressMode.IndexedDef:
                    if (r.Storage == Registers.pc)
                    {
                        //$REVIEW: what if there are two of these?
                        var addr = instrs.Current.Address + instrs.Current.Length;
                        m.Assign(
                            tmp,
                            gen(m.Load(instrs.Current.DataWidth, addr)));
                        m.Assign(
                            m.Load(instrs.Current.DataWidth, addr),
                            tmp);
                    }
                    else
                        throw new NotImplementedException();
                    break;
                }
                return tmp;
            }
            throw new NotImplementedException(string.Format("Not implemented: addressing mode {0}.", op.GetType().Name));
        }

        private Expression RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression, Expression> gen)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                var dst = frame.EnsureRegister(regOp.Register);
                m.Assign(dst, gen(dst, src));
                return dst;
            }
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                var r = frame.EnsureRegister(memOp.Register);
                var tmp = frame.CreateTemporary(instrs.Current.DataWidth);
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        instrs.Current.Address,
                        "Not implemented: addressing mode {0}.",
                        memOp.Mode);
                case AddressMode.RegDef:
                    m.Assign(tmp, gen(src, m.Load(tmp.DataType, r)));
                    m.Assign(m.Load(tmp.DataType, r), tmp);
                    break;
                case AddressMode.AutoIncr:
                    m.Assign(tmp, gen(src, m.Load(tmp.DataType, r)));
                    m.Assign(m.Load(tmp.DataType, r), tmp);
                    m.Assign(r, m.IAdd(r, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r, m.ISub(r, tmp.DataType.Size));
                    m.Assign(tmp, gen(src, m.Load(tmp.DataType, r)));
                    m.Assign(m.Load(tmp.DataType, r), tmp);
                    break;
                case AddressMode.AutoDecrDef:
                    m.Assign(r, m.ISub(r, tmp.DataType.Size));
                    m.Assign(tmp, gen(src, m.Load(tmp.DataType, m.Load(PrimitiveType.Ptr16, r))));
                    m.Assign(m.Load(tmp.DataType, m.Load(PrimitiveType.Ptr16, r)), tmp);
                    break;
                case AddressMode.Absolute:
                    m.Assign(
                        tmp,
                        gen(
                            m.Load(
                                instrs.Current.DataWidth,
                                Address.Ptr16(memOp.EffectiveAddress)),
                            src));
                    m.Assign(
                        m.Load(
                           instrs.Current.DataWidth,
                           Address.Ptr16(memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.Indexed:
                    m.Assign(
                        tmp,
                        gen(
                            m.Load(
                                instrs.Current.DataWidth,
                                m.IAdd(
                                    r, memOp.EffectiveAddress)),
                            src));
                    m.Assign(
                        m.Load(
                            instrs.Current.DataWidth,
                            m.IAdd(
                                r, memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.IndexedDef:
                    if (r.Storage == Registers.pc)
                    {
                        //$REVIEW: what if there are two of these?
                        var addr = instrs.Current.Address + instrs.Current.Length;
                        m.Assign(
                            tmp,
                            gen(
                                m.Load(instrs.Current.DataWidth, addr),
                                src));
                        m.Assign(
                            m.Load(instrs.Current.DataWidth, addr),
                            tmp);
                    }
                    else
                    {
                        m.Assign(
                           tmp,
                           gen(
                               m.Load(
                                   instrs.Current.DataWidth,
                                   m.Load(
                                       PrimitiveType.Ptr16,
                                       m.IAdd(
                                           r, memOp.EffectiveAddress))),
                                   src));
                        m.Assign(
                            m.Load(
                                instrs.Current.DataWidth,
                                m.Load(
                                    PrimitiveType.Ptr16,
                                    m.IAdd(
                                        r, memOp.EffectiveAddress))),
                            tmp);
                    }
                    break;
                }
                return tmp;
            }
            throw new NotImplementedException(string.Format("Not implemented: addressing mode {0}.", op.GetType().Name));
        }
    }
}
