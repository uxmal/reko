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
        private RtlEmitter emitter;
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
                emitter = new RtlEmitter(rtlCluster.Instructions);
                Expression src;
                Expression dst;
                switch (instr.Opcode)
                {
                default: throw new AddressCorrelatedException(
                    instr.Address,
                    "Rewriting of PDP-11 instruction {0} not supported yet.", instr.Opcode);
                case Opcodes.adc: RewriteAdc(instr); break;
                case Opcodes.add: RewriteAdd(instr); break;
                case Opcodes.addb: RewriteAdd(instr); break;
                case Opcodes.ash: RewriteShift(instr); break;
                case Opcodes.asl: RewriteAsl(instr); break;
                case Opcodes.bcs: RewriteBxx(instr, ConditionCode.ULT, FlagM.CF); break;
                case Opcodes.beq: RewriteBxx(instr, ConditionCode.EQ, FlagM.ZF); break;
                case Opcodes.bge: RewriteBxx(instr, ConditionCode.GE, FlagM.VF|FlagM.NF); break;
                case Opcodes.bgt: RewriteBxx(instr, ConditionCode.GT, FlagM.ZF|FlagM.NF|FlagM.VF); break;
                case Opcodes.bhi: RewriteBxx(instr, ConditionCode.UGT, FlagM.ZF|FlagM.CF); break;
                case Opcodes.bic: RewriteBic(instr); break;
                case Opcodes.bis: RewriteBis(instr); break;
                case Opcodes.bisb: RewriteBis(instr); break;
                case Opcodes.bit: RewriteBit(instr); break;
                case Opcodes.bitb: RewriteBit(instr); break;
                case Opcodes.ble: RewriteBxx(instr, ConditionCode.LE, FlagM.ZF|FlagM.NF|FlagM.VF); break;
                case Opcodes.blos: RewriteBxx(instr, ConditionCode.ULE, FlagM.ZF | FlagM.CF); break;
                case Opcodes.blt: RewriteBxx(instr, ConditionCode.LT, FlagM.NF|FlagM.VF); break;
                case Opcodes.bmi: RewriteBxx(instr, ConditionCode.LT, FlagM.NF); break;
                case Opcodes.bne: RewriteBxx(instr, ConditionCode.NE, FlagM.ZF); break;
                case Opcodes.bpl: RewriteBxx(instr, ConditionCode.GT, FlagM.NF); break;
                case Opcodes.br: RewriteBr(instr); break;
                case Opcodes.clr: RewriteClr(instr, emitter.Word16(0)); break;
                case Opcodes.clrb: RewriteClr(instr, emitter.Byte(0)); break;
                case Opcodes.cmp: RewriteCmp(instr); break;
                case Opcodes.dec: RewriteIncDec(instr, emitter.ISub); break;
                case Opcodes.div: RewriteDiv(instr); break;
                case Opcodes.emt: RewriteEmt(instr); break;
                case Opcodes.halt: RewriteHalt(); break;
                case Opcodes.inc: RewriteIncDec(instr, emitter.IAdd); break;
                case Opcodes.jmp: RewriteJmp(instr); break;
                case Opcodes.jsr: RewriteJsr(instr); break;
                case Opcodes.mov: RewriteMov(instr); break;
                case Opcodes.movb: RewriteMov(instr); break;
                case Opcodes.neg: RewriteNeg(instr); break;
                case Opcodes.nop: emitter.Nop(); break;
                case Opcodes.rts: RewriteRts(instr); break;
                case Opcodes.sub: RewriteSub(instr); break;
                case Opcodes.sxt: RewriteSxt(instr); break;
                case Opcodes.trap: RewriteTrap(instr); break;
                case Opcodes.tst: RewriteTst(instr); break;
                case Opcodes.tstb: RewriteTst(instr); break;
                case Opcodes.xor: RewriteXor(instr); break;
                }
                yield return rtlCluster;
            }
        }

        private void SetFlags(Expression e, FlagM changed, FlagM zeroed, FlagM set)
        {
            uint uChanged = (uint)changed;
            if (uChanged != 0)
            {
                var grfChanged = frame.EnsureFlagGroup(this.arch.GetFlagGroup(uChanged));
                emitter.Assign(grfChanged, emitter.Cond(e));
            }
            uint grfMask = 1;
            while (grfMask <= (uint)zeroed)
            {
                if ((grfMask & (uint)zeroed) != 0)
                {
                    var grfZeroed = frame.EnsureFlagGroup(this.arch.GetFlagGroup(grfMask));
                    emitter.Assign(grfZeroed, 0);
                }
                grfMask <<= 1;
            }
            grfMask = 1;
            while (grfMask <= (uint)set)
            {
                if ((grfMask & (uint)set) != 0)
                {
                    var grfZeroed = frame.EnsureFlagGroup(this.arch.GetFlagGroup(grfMask));
                    emitter.Assign(grfZeroed, 1);
                }
                grfMask <<= 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
                emitter.Assign(tmp, emitter.Load(op.Width, emitter.Load(PrimitiveType.Ptr16, r)));
                emitter.Assign(r, emitter.IAdd(r, memOp.Width.Size));
                break;
            case AddressMode.AutoIncrDef:
                emitter.Assign(tmp, emitter.Load(op.Width, r));
                emitter.Assign(r, emitter.IAdd(r, memOp.Width.Size));
                break;
            case AddressMode.AutoDecr:
                emitter.Assign(r, emitter.ISub(r, memOp.Width.Size));
                return emitter.Load(op.Width, r);
            case AddressMode.AutoDecrDef:
                emitter.Assign(r, emitter.ISub(r, memOp.Width.Size));
                emitter.Assign(tmp, emitter.Load(op.Width, emitter.Load(PrimitiveType.Ptr16, r)));
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
                    return emitter.Load(
                        this.instrs.Current.DataWidth,
                        emitter.IAdd(r, Constant.Word16(memOp.EffectiveAddress)));
                }
            case AddressMode.IndexedDef:
                if (memOp.Register == Registers.pc)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    return emitter.Load(
                        PrimitiveType.Ptr16,
                        emitter.IAdd(r, Constant.Word16(memOp.EffectiveAddress)));
                }
            }
            return tmp;
        
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
                    return emitter.Load(this.instrs.Current.DataWidth, r);
                case AddressMode.Absolute:
                    return emitter.Load(
                           instrs.Current.DataWidth,
                           Address.Ptr16(memOp.EffectiveAddress));
                case AddressMode.AutoIncr:
                    emitter.Assign(tmp, emitter.Load(op.Width, r));
                    emitter.Assign(r, emitter.IAdd(r, memOp.Width.Size));
                    break;
                case AddressMode.AutoIncrDef:
                    emitter.Assign(tmp, emitter.Load(op.Width, emitter.Load(PrimitiveType.Ptr16, r)));
                    emitter.Assign(r, emitter.IAdd(r, memOp.Width.Size));
                    break;
                case AddressMode.AutoDecr:
                    emitter.Assign(r, emitter.ISub(r, memOp.Width.Size));
                    return emitter.Load(op.Width, r);
                case AddressMode.AutoDecrDef:
                    emitter.Assign(r, emitter.ISub(r, memOp.Width.Size));
                    emitter.Assign(tmp, emitter.Load(op.Width, emitter.Load(PrimitiveType.Ptr16, r)));
                    return tmp;
                case AddressMode.Indexed:
                    if (memOp.Register == Registers.pc)
                    {
                        var offset = (short)memOp.EffectiveAddress;
                        var addrBase = (long) rtlCluster.Address.ToLinear();
                        var addr = Address.Ptr16((ushort)(2 + addrBase + offset));
                        return addr;
                    }
                    else
                    {
                        return emitter.Load(
                            this.instrs.Current.DataWidth,
                            emitter.IAdd(r, Constant.Word16(memOp.EffectiveAddress)));
                    }
                case AddressMode.IndexedDef:
                    return emitter.Load(
                        this.instrs.Current.DataWidth,
                        emitter.Load(
                            PrimitiveType.Ptr16,
                            emitter.IAdd(r, Constant.Word16(memOp.EffectiveAddress))));
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
                    src = emitter.Dpb(dst, src, 0);
                }
                emitter.Assign(dst, src);
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
                    emitter.Assign(
                        emitter.Load(
                            instrs.Current.DataWidth,
                            Address.Ptr16(memOp.EffectiveAddress)),
                        gen(src));
                    break;
                case AddressMode.RegDef:
                    emitter.Assign(emitter.Load(tmp.DataType, r), gen(src));
                    break;
                case AddressMode.AutoIncr:
                    emitter.Assign(tmp, gen(src));
                    emitter.Assign(emitter.Load(tmp.DataType, r), tmp);
                    emitter.Assign(r, emitter.IAdd(r, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    emitter.Assign(r, emitter.ISub(r, tmp.DataType.Size));
                    emitter.Assign(emitter.Load(tmp.DataType, r), gen(src));
                    break;
                case AddressMode.AutoDecrDef:
                    emitter.Assign(r, emitter.ISub(r, tmp.DataType.Size));
                    emitter.Assign(
                        emitter.Load(
                            tmp.DataType, 
                            emitter.Load(PrimitiveType.Ptr16, r)),
                        gen(src));
                    break;
                case AddressMode.Indexed:
                    if (r.Storage == Registers.pc)
                    {
                        var addr = instrs.Current.Address + instrs.Current.Length;
                        emitter.Assign(
                           tmp,
                           gen(emitter.Load(instrs.Current.DataWidth, addr)));
                        emitter.Assign(
                            emitter.Load(instrs.Current.DataWidth, addr),
                            tmp);
                    }
                    else
                    {
                        emitter.Assign(
                            emitter.Load(
                                this.instrs.Current.DataWidth,
                                emitter.IAdd(
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
                        emitter.Assign(
                            tmp,
                            gen(emitter.Load(instrs.Current.DataWidth, addr)));
                        emitter.Assign(
                            emitter.Load(instrs.Current.DataWidth, addr),
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
                emitter.Assign(dst, gen(dst, src));
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
                    emitter.Assign(tmp, gen(src, emitter.Load(tmp.DataType, r)));
                    emitter.Assign(emitter.Load(tmp.DataType, r), tmp);
                    break;
                case AddressMode.AutoIncr:
                    emitter.Assign(tmp, gen(src, emitter.Load(tmp.DataType, r)));
                    emitter.Assign(emitter.Load(tmp.DataType, r), tmp);
                    emitter.Assign(r, emitter.IAdd(r, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    emitter.Assign(r, emitter.ISub(r, tmp.DataType.Size));
                    emitter.Assign(tmp, gen(src, emitter.Load(tmp.DataType, r)));
                    emitter.Assign(emitter.Load(tmp.DataType, r), tmp);
                    break;
                case AddressMode.AutoDecrDef:
                    emitter.Assign(r, emitter.ISub(r, tmp.DataType.Size));
                    emitter.Assign(tmp, gen(src, emitter.Load(tmp.DataType, emitter.Load(PrimitiveType.Ptr16, r))));
                    emitter.Assign(emitter.Load(tmp.DataType, emitter.Load(PrimitiveType.Ptr16, r)), tmp);
                    break;
                case AddressMode.Absolute:
                    emitter.Assign(
                        tmp,
                        gen(
                            emitter.Load(
                                instrs.Current.DataWidth,
                                Address.Ptr16(memOp.EffectiveAddress)),
                            src));
                    emitter.Assign(
                        emitter.Load(
                           instrs.Current.DataWidth,
                           Address.Ptr16(memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.Indexed:
                    emitter.Assign(
                        tmp,
                        gen(
                            emitter.Load(
                                instrs.Current.DataWidth,
                                emitter.IAdd(
                                    r, memOp.EffectiveAddress)),
                            src));
                    emitter.Assign(
                        emitter.Load(
                            instrs.Current.DataWidth,
                            emitter.IAdd(
                                r, memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.IndexedDef:
                    if (r.Storage == Registers.pc)
                    {
                        //$REVIEW: what if there are two of these?
                        var addr = instrs.Current.Address + instrs.Current.Length;
                        emitter.Assign(
                            tmp,
                            gen(
                                emitter.Load(instrs.Current.DataWidth, addr),
                                src));
                        emitter.Assign(
                            emitter.Load(instrs.Current.DataWidth, addr),
                            tmp);
                    }
                    else
                    {
                        emitter.Assign(
                           tmp,
                           gen(
                               emitter.Load(
                                   instrs.Current.DataWidth,
                                   emitter.Load(
                                       PrimitiveType.Ptr16,
                                       emitter.IAdd(
                                           r, memOp.EffectiveAddress))),
                                   src));
                        emitter.Assign(
                            emitter.Load(
                                instrs.Current.DataWidth,
                                emitter.Load(
                                    PrimitiveType.Ptr16,
                                    emitter.IAdd(
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
