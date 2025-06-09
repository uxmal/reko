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

namespace Reko.Arch.Pdp.Pdp11
{
    public partial class Pdp11Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Pdp11Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Pdp11Instruction> dasm;
        private readonly List<RtlInstruction> rtlInstructions;
        private readonly RtlEmitter m;
        private Pdp11Instruction instr;
        private InstrClass iclass;

        public Pdp11Rewriter(
            Pdp11Architecture arch,
            EndianImageReader rdr,
            IStorageBinder binder,
            IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.dasm = new Pdp11Disassembler(rdr, arch).GetEnumerator();
            this.binder = binder;
            this.host = host;
            this.rtlInstructions = new List<RtlInstruction>();
            this.m = new RtlEmitter(this.rtlInstructions);
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
                    host.Warn(
                        instr.Address,
                        "PDP-11 instruction {0} is not supported yet.", 
                        instr.Mnemonic);
                    EmitUnitTest();
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.illegal: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.adc: RewriteAdcSbc(m.IAdd); break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.addb: RewriteAdd(); break;
                case Mnemonic.ash: RewriteShift(); break;
                case Mnemonic.ashc: RewriteAshc(); break;
                case Mnemonic.asl: RewriteAsl(); break;
                case Mnemonic.aslb: RewriteAsl(); break;
                case Mnemonic.asr: RewriteAsr(); break;
                case Mnemonic.asrb: RewriteAsr(); break;
                case Mnemonic.bcc: RewriteBxx(ConditionCode.UGE, Registers.C); break;
                case Mnemonic.bcs: RewriteBxx(ConditionCode.ULT, Registers.C); break;
                case Mnemonic.beq: RewriteBxx(ConditionCode.EQ,  Registers.Z); break;
                case Mnemonic.bge: RewriteBxx(ConditionCode.GE,  Registers.NV); break;
                case Mnemonic.bgt: RewriteBxx(ConditionCode.GT,  Registers.NZV); break;
                case Mnemonic.bhi: RewriteBxx(ConditionCode.UGT, Registers.ZC); break;
                case Mnemonic.bvs: RewriteBxx(ConditionCode.OV,  Registers.V); break;
                case Mnemonic.bic: RewriteBic(); break;
                case Mnemonic.bicb: RewriteBic(); break;
                case Mnemonic.bis: RewriteBis(); break;
                case Mnemonic.bisb: RewriteBis(); break;
                case Mnemonic.bit: RewriteBit(); break;
                case Mnemonic.bitb: RewriteBit(); break;
                case Mnemonic.ble: RewriteBxx(ConditionCode.LE, Registers.NZV); break;
                case Mnemonic.blos: RewriteBxx(ConditionCode.ULE, Registers.ZC); break;
                case Mnemonic.blt: RewriteBxx(ConditionCode.LT, Registers.NV); break;
                case Mnemonic.bmi: RewriteBxx(ConditionCode.LT, Registers.N); break;
                case Mnemonic.bne: RewriteBxx(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.bpl: RewriteBxx(ConditionCode.GT, Registers.N); break;
                case Mnemonic.bpt: RewriteBpt(); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.clr: RewriteClr(instr, m.Word16(0)); break;
                case Mnemonic.clrb: RewriteClr(instr, m.Byte(0)); break;
                case Mnemonic.clrflags: RewriteClrSetFlags(false); break;
                case Mnemonic.cmp: RewriteCmp(false); break;
                case Mnemonic.cmpb: RewriteCmp(true); break;
                case Mnemonic.com: RewriteCom(); break;
                case Mnemonic.comb: RewriteCom(); break;
                case Mnemonic.dec: RewriteIncDec(m.ISub); break;
                case Mnemonic.decb: RewriteIncDec(m.ISub); break;
                case Mnemonic.div: RewriteDiv(); break;
                case Mnemonic.emt: RewriteEmt(); break;
                case Mnemonic.halt: RewriteHalt(); break;
                case Mnemonic.iot: RewriteIot(); break;
                case Mnemonic.inc: RewriteIncDec(m.IAdd); break;
                case Mnemonic.incb: RewriteIncDec(m.IAdd); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.mark: RewriteMark(); break;
                case Mnemonic.mfpd: RewriteMfpd(); break;
                case Mnemonic.mfpi: RewriteMfpi(); break;
                case Mnemonic.mfpt: RewriteMfpt(); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movb: RewriteMov(); break;
                case Mnemonic.mtpi: RewriteMtpi(); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.negb: RewriteNeg(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.reset: RewriteReset(); break;
                case Mnemonic.rol: RewriteRotate(CommonOps.RolC, 0x8000); break;
                case Mnemonic.rolb: RewriteRotate(CommonOps.RolC, 0x80); break;
                case Mnemonic.ror: RewriteRotate(CommonOps.RorC, 0x1); break;
                case Mnemonic.rorb: RewriteRotate(CommonOps.RorC, 0x1); break;
                case Mnemonic.rti: RewriteRti(); break;
                case Mnemonic.rts: RewriteRts(); break;
                case Mnemonic.rtt: RewriteRtt(); break;
                case Mnemonic.sbc: RewriteAdcSbc(m.ISub); break;
                case Mnemonic.sbcb: RewriteAdcSbc(m.ISub); break;
                case Mnemonic.setflags: RewriteClrSetFlags(true); break;
                case Mnemonic.stcdi: RewriteStcdi(); break;
                case Mnemonic.sob: RewriteSob(); break;
                case Mnemonic.spl: RewriteSpl(); break;
                case Mnemonic.stexp: RewriteStexp(); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.swab: RewriteSwab(); break;
                case Mnemonic.sxt: RewriteSxt(); break;
                case Mnemonic.trap: RewriteTrap(); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.tstb: RewriteTst(); break;
                case Mnemonic.wait: RewriteWait(); break;
                case Mnemonic.xor: RewriteXor(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                this.rtlInstructions.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Pdp11Rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }


        private void SetFlags(Expression? e, FlagGroupStorage changed)
        {
            if (e is null)
            {
                Invalid();
                return;
            }
            if (changed is not null)
            {
                var grfChanged = binder.EnsureFlagGroup(changed);
                m.Assign(grfChanged, m.Cond(e));
            }
        }

        private void SetFalse(FlagGroupStorage flag)
        {
            m.Assign(binder.EnsureFlagGroup(flag), 0);
        }

        private void SetTrue(FlagGroupStorage flag)
        {
            m.Assign(binder.EnsureFlagGroup(flag), flag.FlagGroupBits);
        }

        private Expression? RewriteJmpSrc(MachineOperand op)
        {
            if (!(op is MemoryOperand memOp))
            {
                // PDP-11 always has a memory reference 
                // for the destination of a transfer instruction.
                return null;
            }
            var r = memOp.Register is not null
                ? binder.EnsureRegister(memOp.Register)
                : null;
            var tmp = binder.CreateTemporary(op.DataType);
            switch (memOp.Mode)
            {
            default:
                throw new AddressCorrelatedException(
                    dasm.Current.Address,
                    $"Not implemented: addressing mode {memOp.Mode}.");
            case AddressMode.RegDef:
                return r!;
            case AddressMode.Absolute:
                return Address.Ptr16(memOp.EffectiveAddress);
            case AddressMode.AutoIncr:
                m.Assign(tmp, m.Mem(PrimitiveType.Ptr16, r!));
                m.Assign(r!, m.IAdd(r!, memOp.DataType.Size));
                break;
            case AddressMode.AutoIncrDef:
                m.Assign(tmp, m.Mem(op.DataType, r!));
                m.Assign(r!, m.IAdd(r!, memOp.DataType.Size));
                break;
            case AddressMode.AutoDecr:
                m.Assign(r!, m.ISub(r!, memOp.DataType.Size));
                return m.Mem(op.DataType, r!);
            case AddressMode.AutoDecrDef:
                m.Assign(r!, m.ISub(r!, memOp.DataType.Size));
                m.Assign(tmp, m.Mem(op.DataType, m.Mem(PrimitiveType.Ptr16, r!)));
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
                        this.dasm.Current.DataWidth!,
                        m.IAdd(r!, Constant.Word16(memOp.EffectiveAddress)));
                }
            case AddressMode.IndexedDef:
                if (memOp.Register == Registers.pc)
                {
                    var offset = (short)memOp.EffectiveAddress;
                    var addrBase = (long)instr.Address.ToLinear() + instr.Length;
                    var addr = m.Ptr16((ushort) (addrBase + offset));
                    return m.Mem(
                        PrimitiveType.Word16,
                        addr);
                }
                else
                {
                    return m.Mem(
                        PrimitiveType.Ptr16,
                        m.IAdd(r!, Constant.Word16(memOp.EffectiveAddress)));
                }
            }
            return tmp;
        /*
            var immOp = op as ImmediateOperand;
            if (immOp is not null)
            {
                return immOp.Value;
            }
            var addrOp = op as AddressOperand;
            if (addrOp is not null)
            {
                return addrOp.Address;
            }
            throw new NotImplementedException();
            */
        }

        private Expression RewriteSrc(MachineOperand op, bool signExtendByte = false)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                if (regOp == Registers.pc)
                    return instr.Address + instr.Length;
                var reg = binder.EnsureRegister(regOp);
                if (!signExtendByte)
                    return reg;
                var tmpb = binder.CreateTemporary(PrimitiveType.Byte);
                m.Assign(tmpb, m.Slice(reg, tmpb.DataType));
                return tmpb;
            case Constant immOp:
                if (dasm.Current.DataWidth!.Size == 1)
                {
                    return Constant.Byte((byte) immOp.ToInt32());
                }
                else
                {
                    return immOp;
                }
            case Address addrOp:
                return addrOp;
            case MemoryOperand memOp:
                var r = memOp.Register is not null
                    ? binder.EnsureRegister(memOp.Register)
                    : null;
                var tmp = binder.CreateTemporary(op.DataType);
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        dasm.Current.Address,
                        $"Not implemented: addressing mode {memOp.Mode}.");
                case AddressMode.RegDef:
                    return m.Mem(this.dasm.Current.DataWidth!, r!);
                case AddressMode.Absolute:
                    return m.Mem(
                           dasm.Current.DataWidth!,
                           Address.Ptr16(memOp.EffectiveAddress));
                case AddressMode.AutoIncr:
                    m.Assign(tmp, m.Mem(op.DataType, r!));
                    m.Assign(r!, m.IAdd(r!, memOp.DataType.Size));
                    break;
                case AddressMode.AutoIncrDef:
                    m.Assign(tmp, m.Mem(op.DataType, m.Mem(PrimitiveType.Ptr16, r!)));
                    m.Assign(r!, m.IAdd(r!, memOp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r!, m.ISub(r!, memOp.DataType.Size));
                    return m.Mem(op.DataType, r!);
                case AddressMode.AutoDecrDef:
                    m.Assign(r!, m.ISub(r!, memOp.DataType.Size));
                    m.Assign(tmp, m.Mem(op.DataType, m.Mem(PrimitiveType.Ptr16, r!)));
                    return tmp;
                case AddressMode.Indexed:
                    if (memOp.Register == Registers.pc)
                    {
                        var offset = (short)memOp.EffectiveAddress;
                        var addrBase = (long) instr.Address.ToLinear();
                        var addr = Address.Ptr16((ushort)(2 + addrBase + offset));
                        return m.Mem(memOp.DataType, addr);
                    }
                    else
                    {
                        return m.Mem(
                            this.dasm.Current.DataWidth!,
                            m.IAdd(r!, Constant.Word16(memOp.EffectiveAddress)));
                    }
                case AddressMode.IndexedDef:
                    if (memOp.Register == Registers.pc)
                    {
                        var addr = this.dasm.Current.Address + this.dasm.Current.Length + memOp.EffectiveAddress;
                        m.Assign(tmp, m.Mem(PrimitiveType.Ptr16, addr));
                        m.Assign(tmp, m.Mem(memOp.DataType, tmp));
                        return tmp;
                    }
                    else
                    {
                    return m.Mem(
                        this.dasm.Current.DataWidth!,
                        m.Mem(
                            PrimitiveType.Ptr16,
                            m.IAdd(r!, Constant.Word16(memOp.EffectiveAddress))));
                    }
                }
                return tmp;
            }
            throw new NotImplementedException();
        }

        // Rewrites a destination operand when the source is unary.
        private Expression? RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression> gen)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                var dst = binder.EnsureRegister(regOp);
                src = gen(src);
                if (src.DataType.Size < dst.DataType.Size)
                {
                    src = m.Dpb(dst, src, 0);
                }
                m.Assign(dst, src);
                return dst;
            case MemoryOperand memOp:
                var r = memOp.Register is not null
                    ? binder.EnsureRegister(memOp.Register)
                    : null;
                Expression tmp = MaybeAssignTmp(gen(src));
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        dasm.Current.Address,
                        $"Not implemented: addressing mode {memOp.Mode}.");
                case AddressMode.Absolute:
                    m.Assign(
                        m.Mem(
                            dasm.Current.DataWidth!,
                            Address.Ptr16(memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.RegDef:
                    m.Assign(m.Mem(tmp.DataType, r!), tmp);
                    break;
                case AddressMode.AutoIncr:
                    m.Assign(m.Mem(tmp.DataType, r!), tmp);
                    m.Assign(r!, m.IAdd(r!, tmp.DataType.Size));
                    break;
                case AddressMode.AutoIncrDef:
                    m.Assign(m.Mem(PrimitiveType.Ptr16, m.Mem(tmp.DataType, r!)), tmp);
                    m.Assign(r!, m.IAdd(r!, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r!, m.ISub(r!, tmp.DataType.Size));
                    m.Assign(m.Mem(tmp.DataType, r!), tmp);
                    break;
                case AddressMode.AutoDecrDef:
                    m.Assign(r!, m.ISub(r!, tmp.DataType.Size));
                    m.Assign(
                        m.Mem(
                            tmp.DataType, 
                            m.Mem(PrimitiveType.Ptr16, r!)),
                        tmp);
                    break;
                case AddressMode.Indexed:
                    if (r!.Storage == Registers.pc)
                    {
                        var addr = dasm.Current.Address + dasm.Current.Length + memOp.EffectiveAddress;
                        m.Assign(
                            m.Mem(dasm.Current.DataWidth!, addr),
                            tmp);
                    }
                    else
                    {
                        m.Assign(
                            m.Mem(
                                this.dasm.Current.DataWidth!,
                                m.IAdd(
                                    r,
                                    Constant.Word16(memOp.EffectiveAddress))),
                            tmp);
                    }
                    break;
                case AddressMode.IndexedDef:
                    if (r!.Storage == Registers.pc)
                    {
                        //$REVIEW: what if there are two of these?
                        var addr = dasm.Current.Address + dasm.Current.Length + memOp.EffectiveAddress;
                        var deferred = binder.CreateTemporary(PrimitiveType.Ptr16);
                        m.Assign(
                            deferred,
                            m.Mem(PrimitiveType.Ptr16, addr));
                        m.Assign(
                            m.Mem(dasm.Current.DataWidth!, deferred),
                            tmp);
                    }
                    else
                    {
                        m.Assign(
                            m.Mem(
                                PrimitiveType.Ptr16,
                                m.Mem(
                                    this.dasm.Current.DataWidth!,
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

        private Expression? RewriteDst(MachineOperand op, Expression src, Func<Expression, Expression, Expression> gen)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                var dst = binder.EnsureRegister(regOp);
                m.Assign(dst, gen(dst, src));
                return dst;
            case MemoryOperand memOp:
                var r = memOp.Register is not null
                    ? binder.EnsureRegister(memOp.Register)
                    : null;
                var tmp = binder.CreateTemporary(dasm.Current.DataWidth!);
                switch (memOp.Mode)
                {
                default:
                    throw new AddressCorrelatedException(
                        dasm.Current.Address,
                        $"Not implemented: addressing mode {memOp.Mode}.");
                case AddressMode.RegDef:
                    m.Assign(tmp, gen(m.Mem(tmp.DataType, r!), src));
                    m.Assign(m.Mem(tmp.DataType, r!), tmp);
                    break;
                case AddressMode.AutoIncr:
                    m.Assign(tmp, gen(m.Mem(tmp.DataType, r!), src));
                    m.Assign(m.Mem(tmp.DataType, r!), tmp);
                    m.Assign(r!, m.IAdd(r!, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecr:
                    m.Assign(r!, m.ISub(r!, tmp.DataType.Size));
                    m.Assign(tmp, gen(m.Mem(tmp.DataType, r!), src));
                    m.Assign(m.Mem(tmp.DataType, r!), tmp);
                    break;
                case AddressMode.AutoIncrDef:
                    m.Assign(tmp, gen(m.Mem(PrimitiveType.Ptr16, m.Mem(tmp.DataType, r!)), src));
                    m.Assign(m.Mem(tmp.DataType, r!), tmp);
                    m.Assign(r!, m.IAdd(r!, tmp.DataType.Size));
                    break;
                case AddressMode.AutoDecrDef:
                    m.Assign(r!, m.ISub(r!, tmp.DataType.Size));
                    m.Assign(tmp, gen(m.Mem(tmp.DataType, m.Mem(PrimitiveType.Ptr16, r!)), src));
                    m.Assign(m.Mem(tmp.DataType, m.Mem(PrimitiveType.Ptr16, r!)), tmp);
                    break;
                case AddressMode.Absolute:
                    m.Assign(
                        tmp,
                        gen(
                            m.Mem(
                                dasm.Current.DataWidth!,
                                Address.Ptr16(memOp.EffectiveAddress)),
                            src));
                    m.Assign(
                        m.Mem(
                           dasm.Current.DataWidth!,
                           Address.Ptr16(memOp.EffectiveAddress)),
                        tmp);
                    break;
                case AddressMode.Indexed:
                    if (r!.Storage == Registers.pc)
                    {
                        var addr = dasm.Current.Address + memOp.EffectiveAddress;
                        m.Assign(tmp, gen(m.Mem(dasm.Current.DataWidth!, addr), src));
                        m.Assign(m.Mem(dasm.Current.DataWidth!, addr), tmp);
                    }
                    else
                    {
                    m.Assign(
                        tmp,
                        gen(
                            m.Mem(
                                dasm.Current.DataWidth!,
                                m.IAdd(
                                    r!, memOp.EffectiveAddress)),
                            src));
                    m.Assign(
                        m.Mem(
                            dasm.Current.DataWidth!,
                            m.IAdd(
                                r!, memOp.EffectiveAddress)),
                        tmp);
                    }
                    break;
                case AddressMode.IndexedDef:
                    if (r!.Storage == Registers.pc)
                    {
                        //$REVIEW: what if there are two of these?
                        var addr = dasm.Current.Address + dasm.Current.Length;
                        m.Assign(
                            tmp,
                            gen(
                                m.Mem(dasm.Current.DataWidth!, addr),
                                src));
                        m.Assign(
                            m.Mem(dasm.Current.DataWidth!, addr),
                            tmp);
                    }
                    else
                    {
                        m.Assign(
                           tmp,
                           gen(
                               m.Mem(
                                   dasm.Current.DataWidth!,
                                   m.Mem(
                                       PrimitiveType.Ptr16,
                                       m.IAdd(
                                           r, memOp.EffectiveAddress))),
                                   src));
                        m.Assign(
                            m.Mem(
                                dasm.Current.DataWidth!,
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
            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        static readonly IntrinsicProcedure bpt_intrinsic = new IntrinsicBuilder("__bpt", true)
            .Returns(PrimitiveType.Byte);
        static readonly IntrinsicProcedure mfpd_intrinsic = new IntrinsicBuilder("__mfpd", true)
            .Param(PrimitiveType.Word16)
            .Returns(PrimitiveType.Word16);
        static readonly IntrinsicProcedure mfpi_intrinsic = new IntrinsicBuilder("__mfpi", true)
            .Param(PrimitiveType.Word16)
            .Returns(PrimitiveType.Word16);
        static readonly IntrinsicProcedure mfpt_intrinsic = new IntrinsicBuilder("__mfpt", true)
            .Returns(PrimitiveType.Word16);
        static readonly IntrinsicProcedure mtpi_intrinsic = new IntrinsicBuilder("__mtpi", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Word16)
            .Void();
        static readonly IntrinsicProcedure reset_intrinsic = new IntrinsicBuilder("__reset", true)
            .Void();
        static readonly IntrinsicProcedure shift_intrinsic = new IntrinsicBuilder("__shift", false)
            .GenericTypes("TValue", "TShift")
            .Param("TValue")
            .Param("TShift")
            .Returns("TValue");
        static readonly IntrinsicProcedure spl_intrinsic = new IntrinsicBuilder("__set_priority_level", true)
            .Param(PrimitiveType.Byte)
            .Void();
        static readonly IntrinsicProcedure stexp_intrinsic = new IntrinsicBuilder("__stexp", true)
            .GenericTypes("T")
            .Param("T")
            .Returns(PrimitiveType.Int16);
        static readonly IntrinsicProcedure swab_intrinsic = IntrinsicBuilder.Unary("__swab", PrimitiveType.Word16);
        static readonly IntrinsicProcedure wait_intrinsic = new IntrinsicBuilder("__wait", true)
            .Void();
    }
}
