#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Avr.Avr32
{
    public class Avr32Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static readonly FlagGroupStorage C;
        private static readonly FlagGroupStorage N;
        private static readonly FlagGroupStorage Q;
        private static readonly FlagGroupStorage V;
        private static readonly FlagGroupStorage NZ;
        private static readonly FlagGroupStorage NZC;
        private static readonly FlagGroupStorage VN;
        private static readonly FlagGroupStorage VNZ;
        private static readonly FlagGroupStorage VNZC;
        private static readonly FlagGroupStorage ZC;
        private static readonly FlagGroupStorage Z;

        private readonly Avr32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Avr32Instruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private Avr32Instruction instr;
        private InstrClass iclass;

        public Avr32Rewriter(Avr32Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Avr32Disassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = this.instr.InstructionClass;
                switch (this.instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    host.Warn(
                        dasm.Current.Address,
                        "AVR32 instruction '{0}' is not supported yet.",
                        instr.Mnemonic);
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid: m.Invalid(); break;
                case Mnemonic.abs: RewriteAbs(); break;
                case Mnemonic.acall: RewriteAcall(); break;
                case Mnemonic.acr: RewriteAcr(); break;
                case Mnemonic.adc: RewriteAdc(); break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.andh: RewriteAndh(); break;
                case Mnemonic.andl: RewriteAndl(); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.andnot: RewriteLogical((a, b) => m.And(a, m.Comp(b))); break;
                case Mnemonic.asr: RewriteShift(m.Sar); break;
                case Mnemonic.bfexts: RewriteBfexts(); break;
                case Mnemonic.bfextu: RewriteBfextu(); break;
                case Mnemonic.bld: RewriteBld(); break;
                case Mnemonic.br: RewriteBranch(); break;
                case Mnemonic.bst: RewriteBst(); break;
                case Mnemonic.casts_b: RewriteCast(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Mnemonic.casts_h: RewriteCast(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.castu_b: RewriteCast(PrimitiveType.Byte, PrimitiveType.Word32); break;
                case Mnemonic.castu_h: RewriteCast(PrimitiveType.Word16, PrimitiveType.Word32); break;
                case Mnemonic.cbr: RewriteCbr(); break;
                case Mnemonic.com: RewriteCom(); break;
                case Mnemonic.clz: RewriteClz(); break;
                case Mnemonic.cp_b: RewriteCp_b(); break;
                case Mnemonic.cp_w: RewriteCp_w(); break;
                case Mnemonic.cpc: RewriteCpc(); break;
                case Mnemonic.divs: RewriteDiv(m.SDiv); break;
                case Mnemonic.divu: RewriteDiv(m.UDiv); break;
                case Mnemonic.eor: RewriteLogical(m.Xor); break;
                case Mnemonic.eorh: RewriteOrh(m.Xor); break;
                case Mnemonic.eorl: RewriteOrh(m.Xor); break;
                case Mnemonic.icall: RewriteCall(); break;
                case Mnemonic.ld_d: RewriteLd(PrimitiveType.Word64, PrimitiveType.Word64); break;
                case Mnemonic.ld_sb: RewriteLd(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Mnemonic.ld_sh: RewriteLd(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.ld_ub: RewriteLd(PrimitiveType.Byte, PrimitiveType.Word32); break;
                case Mnemonic.ld_uh: RewriteLd(PrimitiveType.Word16, PrimitiveType.Word32); break;
                case Mnemonic.ld_w: RewriteLd(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case Mnemonic.lddpc: RewriteLddpc(); break;
                case Mnemonic.lddsp: RewriteLddsp(); break;
                case Mnemonic.ldm: RewriteLdm(); break;
                case Mnemonic.lsl: RewriteShift(m.Shl); break;
                case Mnemonic.lsr: RewriteShift(m.Shr); break;
                case Mnemonic.macs_d: RewriteMac_d(m.SMul); break;
                case Mnemonic.macu_d: RewriteMac_d(m.UMul); break;
                case Mnemonic.max: RewriteMax(); break;
                case Mnemonic.mcall: RewriteCall(); break;
                case Mnemonic.min: RewriteMin(); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movh: RewriteMovh(); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.muls_d: RewriteMul_d(m.SMul); break;
                case Mnemonic.mulu_d: RewriteMul_d(m.UMul); break;
                case Mnemonic.mustr: RewriteMustr(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteLogical(m.Or); break;
                case Mnemonic.orh: RewriteOrh(m.Or); break;
                case Mnemonic.orl: RewriteOrl(m.Or); break;
                case Mnemonic.popm: RewritePopm(); break;
                case Mnemonic.pushm: RewritePushm(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.rcall: RewriteCall(); break;
                case Mnemonic.rjmp: RewriteGoto(); break;
                case Mnemonic.rol: RewriteRol(); break;
                case Mnemonic.ror: RewriteRor(); break;
                case Mnemonic.rsub: RewriteRsub(); break;
                case Mnemonic.sats: RewriteSat(sat_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.satu: RewriteSat(sat_intrinsic, PrimitiveType.UInt32); break;
                case Mnemonic.satsub_w: RewriteSatsubW(); break;
                case Mnemonic.sbc: RewriteSbc(); break;
                case Mnemonic.sbr: RewriteSbr(); break;
                case Mnemonic.scr: RewriteScr(); break;
                case Mnemonic.sr: RewriteSr(); break;
                case Mnemonic.stm: RewriteStm(); break;
                case Mnemonic.st_b: RewriteSt(PrimitiveType.Byte); break;
                case Mnemonic.st_h: RewriteSt(PrimitiveType.Word16); break;
                case Mnemonic.st_w: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.st_d: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.stcond: RewriteStcond(); break;
                case Mnemonic.stdsp: RewriteStdsp(); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.subf: RewriteSub(); break;
                case Mnemonic.tst: RewriteTst(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Avr32Rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitCc(FlagGroupStorage grf, Expression exp)
        {
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(exp));
        }

        private void MaybeSkip()
        {
            if (instr.Condition != Avr32Condition.al)
            {
                m.BranchInMiddleOfInstruction(Condition().Invert(), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
            }
        }


        private void Push(RegisterStorage stackRegister, RegisterStorage reg)
        {
            var sp = binder.EnsureRegister(stackRegister);
            m.Assign(sp, m.ISubS(sp, reg.DataType.Size));
            m.Assign(m.Mem(reg.DataType, sp), binder.EnsureRegister(reg));
        }

        private Expression RewriteMemoryOperand(MemoryOperand mem)
        {
            var baseReg = binder.EnsureRegister(mem.Base!);
            if (mem.PostIncrement)
            {
                var tmp = binder.CreateTemporary(mem.Width);
                m.Assign(tmp, m.Mem(mem.Width, baseReg));
                m.Assign(baseReg, m.IAddS(baseReg, mem.Width.Size));
                return tmp;
            }
            if (mem.PreDecrement)
            {
                m.Assign(baseReg, m.ISubS(baseReg, mem.Width.Size));
                var tmp = binder.CreateTemporary(mem.Width);
                m.Assign(tmp, m.Mem(mem.Width, baseReg));
                return tmp;
            }
            Expression ea = baseReg;
            if (mem.Index != null)
            {
                Expression idx = binder.EnsureRegister(mem.Index);
                if (mem.Shift > 0)
                {
                    idx = m.IMul(idx, m.Word32(1 << mem.Shift));
                }
                ea = m.IAdd(ea, idx);
            }
            if (mem.Offset != 0)
            {
                ea = m.IAddS(ea, mem.Offset);
            }
            return m.Mem(mem.Width, ea);
        }

        private Expression RewriteOp(int iOp)
        {
            switch (instr.Operands[iOp])
            {
            case RegisterStorage reg:
                if (reg == Registers.pc)
                    return instr.Address;
                else
                    return binder.EnsureRegister(reg);
            case ImmediateOperand imm:
                return imm.Value;
            case AddressOperand addr:
                return addr.Address;
            case MemoryOperand mem:
                return RewriteMemoryOperand(mem);
            case RegisterImmediateOperand regimm:
                {
                    var innerReg = binder.EnsureRegister(regimm.Register);
                    switch (regimm.Mnemonic)
                    {
                    case Mnemonic.lsl:
                        return regimm.Value != 0
                            ? m.Shl(innerReg, regimm.Value)
                            : (Expression) innerReg;
                    case Mnemonic.lsr:
                        return regimm.Value != 0
                            ? m.Shl(innerReg, regimm.Value)
                            : (Expression) innerReg;
                    default: throw new NotImplementedException($"{regimm.Mnemonic} not implemented yet.");
                    }
                }
            case RegisterPairOperand pair:
                var idPair = binder.EnsureSequence(pair.Width, pair.HiRegister, pair.LoRegister);
                return idPair;
            }
            throw new NotImplementedException($"AVR32 operand type {instr.Operands[iOp].GetType()} not implemented yet.");
        }

        private Expression RewriteOpDst(int iOp, Expression src)
        {
            switch (instr.Operands[iOp])
            {
            case RegisterStorage reg:
                if (reg == Registers.pc)
                {
                    m.Goto(src);
                    return src;
                }
                else
                {
                    var id = binder.EnsureRegister(reg);
                    m.Assign(id, src);
                    return id;
                }
            case MemoryOperand mem:
                Expression ea = binder.EnsureRegister(mem.Base!);
                if (mem.PreDecrement)
                {
                    m.Assign(ea, m.ISubS(ea, mem.Width.Size));
                    m.Assign(m.Mem(mem.Width, ea), src);
                    return src;
                }
                if (mem.Index != null)
                {
                    ea = binder.EnsureRegister(mem.Index);
                    if (mem.Shift > 0)
                    {
                        ea = m.IMul(ea, m.Word32(1 << mem.Shift));
                    }
                }
                if (mem.Offset != 0)
                {
                    ea = m.IAddS(ea, mem.Offset);
                }
                m.Assign(m.Mem(mem.Width, ea), src);
                if (mem.PostIncrement)
                {
                    m.Assign(ea, m.IAddS(ea, mem.Width.Size));
                }
                return src;
            case RegisterPairOperand pair:
                var idPair = binder.EnsureSequence(pair.Width, pair.HiRegister, pair.LoRegister);
                m.Assign(idPair, src);
                return idPair;
            }
            throw new NotImplementedException($"AVR32 operand type {instr.Operands[iOp].GetType()} not implemented yet.");
        }

        private void RewriteAbs()
        {
            var i32 = PrimitiveType.Int32;
            var tmp = binder.CreateTemporary(i32);
            m.Assign(tmp, RewriteOp(0));
            var src = m.Fn(CommonOps.Abs.MakeInstance(i32), tmp);
            var dst = RewriteOpDst(0, src);
            m.Assign(binder.EnsureFlagGroup(Z), m.Eq0(dst));
        }

        private void RewriteAcall()
        {
            var acba = binder.EnsureRegister(Registers.acba);
            m.Call(m.Mem32(m.IAdd(acba, RewriteOp(0))), 0);
        }

        private void RewriteAcr()
        {
            var src = m.IAdd(RewriteOp(0), binder.EnsureFlagGroup(C));
            var dst = RewriteOpDst(0, src);
            EmitCc(VNZC, dst);
        }

        private void RewriteAdc()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var src = m.IAdd(m.IAdd(left, right), binder.EnsureFlagGroup(C));
            var dst = RewriteOpDst(0, src);
            EmitCc(VNZC, dst);
        }

        private void RewriteAdd()
        {
            MaybeSkip();
            Expression left;
            Expression right;
            if (instr.Operands.Length == 3)
            {
                left = RewriteOp(1);
                right = RewriteOp(2);
            }
            else
            {
                left = RewriteOp(0);
                right = RewriteOp(1);
            }
            var dst = RewriteOpDst(0, m.IAdd(left, right));
            if (instr.Condition == Avr32Condition.al)
            {
                EmitCc(VNZC, dst);
            }
        }

        private void RewriteAndh()
        {
            var mask = ((ImmediateOperand) instr.Operands[1]).Value.ToUInt32() << 16;
            if (instr.Operands.Length != 3)
            {
                mask |= 0x0000FFFFu;
            }
            var src = m.And(RewriteOp(0), m.Word32(mask));
            var dst = RewriteOpDst(0, src);
            EmitCc(NZ, dst);
        }

        private void RewriteAndl()
        {
            var mask = ((ImmediateOperand) instr.Operands[1]).Value.ToUInt32();
            if (instr.Operands.Length != 3)
            {
                mask |= 0xFFFF0000u;
            }
            var src = m.And(RewriteOp(0), m.Word32(mask));
            var dst = RewriteOpDst(0, src);
            EmitCc(NZ, dst);
        }

        private void RewriteBfexts()
        {
            var b = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            var w = ((ImmediateOperand) instr.Operands[3]).Value.ToInt32();
            if (w == 0 || b + w > 32)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var dt = PrimitiveType.Create(Domain.SignedInt, w);
            var slice = m.Slice(RewriteOp(1), dt, b);
            var dst = RewriteOpDst(0, slice);
            EmitCc(NZC, dst);
        }

        private void RewriteBfextu()
        {
            var b = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            var w = ((ImmediateOperand) instr.Operands[3]).Value.ToInt32();
            if (w == 0 || b + w > 32)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var dt = PrimitiveType.CreateWord(w);
            var slice = m.Slice(RewriteOp(1), dt, b);
            var dst = RewriteOpDst(0, slice);
            EmitCc(NZC, dst);
        }

        private void RewriteBld()
        {
            var src = RewriteOp(0);
            var bit = ((ImmediateOperand) instr.Operands[1]).Value.ToInt32();
            src = m.Slice(src, PrimitiveType.Bool, bit);
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, src);
            m.Assign(binder.EnsureFlagGroup(Z), tmp);
            m.Assign(binder.EnsureFlagGroup(C), tmp);
        }

        private Expression Condition()
        {
            Expression MkCond(ConditionCode cc, FlagGroupStorage grf)
            {
                return m.Test(cc, binder.EnsureFlagGroup(grf));
            }

            switch (instr.Condition)
            {
            case Avr32Condition.eq: return MkCond(ConditionCode.EQ, Z);
            case Avr32Condition.ne: return MkCond(ConditionCode.NE, Z);
            case Avr32Condition.cc: return MkCond(ConditionCode.UGE, C);
            case Avr32Condition.cs: return MkCond(ConditionCode.ULT, C);
            case Avr32Condition.ge: return MkCond(ConditionCode.GE, VN);
            case Avr32Condition.lt: return MkCond(ConditionCode.LT, VN);
            case Avr32Condition.mi: return MkCond(ConditionCode.LT, N);
            case Avr32Condition.pl: return MkCond(ConditionCode.GE, N);
            case Avr32Condition.ls: return MkCond(ConditionCode.ULE, ZC);
            case Avr32Condition.gt: return MkCond(ConditionCode.GT, VNZ);
            case Avr32Condition.le: return MkCond(ConditionCode.LE, VNZ);
            case Avr32Condition.hi: return MkCond(ConditionCode.UGT, VNZ);
            case Avr32Condition.vs: return MkCond(ConditionCode.OV, V);
            case Avr32Condition.vc: return MkCond(ConditionCode.NO, V);
            case Avr32Condition.qs: return binder.EnsureFlagGroup(Q);;
            case Avr32Condition.al: return Constant.True();
            default: throw new InvalidOperationException();
            }
        }

        private void RewriteBranch()
        {
            var addr = ((AddressOperand) instr.Operands[0]).Address;
            if (instr.Condition == Avr32Condition.al)
            {
                m.Goto(addr);
            }
            else
            {
                m.Branch(Condition(), addr);
            }
        }

        private void RewriteBst()
        {
            var reg = RewriteOp(0);
            var bit = ((ImmediateOperand) instr.Operands[1]).Value;
            var c = binder.EnsureFlagGroup(C);
            var src = m.Fn(setbit_intrinsic, reg, bit, c);
            RewriteOpDst(0, src);
        }

        private void RewriteCall()
        {
            m.Call(RewriteOp(0), 0);
        }

        private void RewriteCast(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var src = RewriteOp(0);
            if (src.DataType.BitSize > dtFrom.BitSize)
            {
                src = m.Slice(src, dtFrom);
            }
            var dst = RewriteOpDst(0, m.Convert(src, dtFrom, dtTo));
            EmitCc(NZC, dst);
        }

        private void RewriteCbr()
        {
            var bit = ((ImmediateOperand) instr.Operands[1]).Value;
            var mask = Constant.UInt32(~(1U << bit.ToInt32()));
            RewriteOpDst(0, m.And(RewriteOp(0), mask));
            m.Assign(binder.EnsureFlagGroup(Z), Constant.False());
        }

        private void RewriteCom()
        {
            var src = RewriteOp(0);
            var dst = RewriteOpDst(0, m.Comp(src));
            EmitCc(Z, dst);
        }

        private void RewriteClz()
        {
            var src = RewriteOp(1);
            var dst = RewriteOpDst(0, m.Fn(clz_intrinsic, src));
            m.Assign(binder.EnsureFlagGroup(Z), m.Eq0(dst));
            m.Assign(binder.EnsureFlagGroup(C), m.Eq(dst, 32));
        }

        private void RewriteCp_b()
        {
            var op1 = m.Slice(RewriteOp(0), PrimitiveType.Byte);
            var op2 = m.Slice(RewriteOp(1), PrimitiveType.Byte);
            var grf = binder.EnsureFlagGroup(VNZC);
            m.Assign(grf, m.Cond(m.ISub(op1, op2)));
        }

        private void RewriteCp_w()
        {
            var op1 = RewriteOp(0);
            var op2 = RewriteOp(1);
            var grf = binder.EnsureFlagGroup(VNZC);
            m.Assign(grf, m.Cond(m.ISub(op1, op2)));
        }

        private void RewriteCpc()
        {
            Expression result;
            if (instr.Operands.Length == 2)
            {
                result = m.ISub(m.ISub(RewriteOp(0), RewriteOp(1)), binder.EnsureFlagGroup(C));
            }
            else
            {
                result = m.ISub(RewriteOp(0), binder.EnsureFlagGroup(C));
            }
            EmitCc(VNZC, result);
        }

        private void RewriteDiv(Func<Expression, Expression, Expression> div)
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var src = div(left, right);
            RewriteOpDst(0, src);
        }

        private void RewriteGoto()
        {
            this.iclass = InstrClass.Transfer;
            m.Goto(((AddressOperand) instr.Operands[0]).Address);
        }

        private void RewriteLd(PrimitiveType dtCast, PrimitiveType dtDst)
        {
            MaybeSkip();
            var src = RewriteOp(1);
            if (src.DataType.BitSize < instr.Operands[0].Width.BitSize)
            {
                src = m.Convert(src, dtCast, dtDst);
            }
            RewriteOpDst(0, src);
        }

        private void RewriteLddpc()
        {
            var mem = (MemoryOperand) instr.Operands[1];
            var uAddr = (instr.Address.ToUInt32() & ~4U) + (uint)mem.Offset;
            var src = m.Mem32(m.Ptr32(uAddr));
            RewriteOpDst(0, src);
        }

        private void RewriteLddsp()
        {
            var src = RewriteOp(1);
            RewriteOpDst(0, src);
        }

        private void RewriteLdm()
        {
            var regs = instr.Operands
                .Skip(1)
                .Cast<RegisterRange>()
                .SelectMany(rr => rr.Enumerate());
            Identifier sp;
            bool postInc = false;
            if (instr.Operands[0] is MemoryOperand post)
            {
                Debug.Assert(post.PostIncrement);
                postInc = true;
                sp = binder.EnsureRegister(post.Base!);
            }
            else
            {
                sp = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            }

            bool emitReturn = false;
            int offset = 0;
            foreach (var reg in regs.Reverse())
            {
                if (reg == Registers.pc)
                {
                    emitReturn = true;
                }
                else
                {
                    m.Assign(binder.EnsureRegister(reg), m.Mem(reg.DataType, m.IAddS(sp, offset)));
                }
                offset += reg.DataType.Size;
            }
            if (postInc)
            {
                m.Assign(sp, m.IAddS(sp, offset));
            }
            if (emitReturn)
            {
                m.Return(0, 0);
            }
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> fn)
        {
            MaybeSkip();
            Expression src;
            if (instr.Operands.Length == 3)
            {
                var left = RewriteOp(1);
                var right = RewriteOp(2);
                src = fn(left, right);
            }
            else
            {
                var left = RewriteOp(0);
                var right = RewriteOp(1);
                src = fn(left, right);
            }
            var result = RewriteOpDst(0, src);
            if (instr.Condition == Avr32Condition.al)
            {
                EmitCc(NZ, result);
            }
        }

        private void RewriteMac_d(Func<Expression, Expression, Expression> fn)
        {
            var rDst = (RegisterStorage) instr.Operands[0];
            if ((rDst.Number & 1) == 1)
            {
                m.Invalid();
                iclass = InstrClass.Invalid;
                return;
            }
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var rDstHi = Registers.GpRegisters[rDst.Number + 1];
            var dst = binder.EnsureSequence(PrimitiveType.Word64, rDstHi, rDst);
            m.Assign(dst, m.IAdd(dst, fn(left, right)));
        }

        private void RewriteMax()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var src = m.Fn(CommonOps.Max.MakeInstance(PrimitiveType.Int32), left, right);
            RewriteOpDst(0, src);
        }
        
        private void RewriteMin()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var src = m.Fn(CommonOps.Min.MakeInstance(PrimitiveType.Int32), left, right);
            RewriteOpDst(0, src);
        }

        private void RewriteMov()
        {
            MaybeSkip();
            var src = RewriteOp(1);
            RewriteOpDst(0, src);
        }

        private void RewriteMovh()
        {
            var n = ((ImmediateOperand) instr.Operands[1]).Value.ToUInt32() << 16;
            RewriteOpDst(0, m.Word32(n));
        }

        private void RewriteMul()
        {
            Expression left;
            Expression right;
            if (instr.Operands.Length == 3)
            {
                left = RewriteOp(1);
                right = RewriteOp(2);
            }
            else
            {
                left = RewriteOp(0);
                right = RewriteOp(1);
            }
            RewriteOpDst(0, m.IMul(left, right));
        }

        private void RewriteMul_d(Func<Expression,Expression, Expression> fn)
        {
            var rDst = (RegisterStorage) instr.Operands[0];
            if ((rDst.Number & 1) == 1)
            {
                m.Invalid();
                iclass = InstrClass.Invalid;
                return;
            }
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var rDstHi = Registers.GpRegisters[rDst.Number + 1];
            var dst = binder.EnsureSequence(PrimitiveType.Word64, rDstHi, rDst);
            m.Assign(dst, fn(left, right));
        }

        private void RewriteMustr()
        {
            var src = m.And(binder.EnsureRegister(Registers.sr), Constant.UInt32(0xF));
            RewriteOpDst(0, src);
        }

        private void RewriteNeg()
        {
            var src = RewriteOp(0);
            var dst = RewriteOpDst(0, m.Neg(src));
            EmitCc(VNZC, dst);
        }

        private void RewriteOrh(Func<Expression, Expression, Expression> fn)
        {
            var mask = ((ImmediateOperand) instr.Operands[1]).Value.ToUInt32() << 16;
            var src = fn(RewriteOp(0), m.Word32(mask));
            var dst = RewriteOpDst(0, src);
            EmitCc(NZ, dst);
        }

        private void RewriteOrl(Func<Expression, Expression, Expression> fn)
        {
            var mask = ((ImmediateOperand) instr.Operands[1]).Value.ToUInt32();
            var src = fn(RewriteOp(0), m.Word32(mask));
            var dst = RewriteOpDst(0, src);
            EmitCc(NZ, dst);
        }

        private void RewritePopm()
        {
            List<RegisterStorage> RegisterOperands()
            {
                var registers = new List<RegisterStorage>();
                foreach (var op in instr.Operands)
                {
                    switch(op)
                    {
                    case RegisterStorage reg: registers.Add(reg); break;
                    case RegisterRange range:
                        for (int i = 0; i < range.Count; ++i)
                        {
                            registers.Add(range.Registers[range.RegisterIndex + i]);
                        }
                        break;
                    }
                }
                return registers;
            }

            var regs = RegisterOperands();
            regs.Reverse();
            var sp = binder.EnsureRegister(Registers.sp);
            bool emitReturn = false;
            int offset = 0;
            foreach (var op in regs)
            {
                if (op == Registers.pc)
                {
                    emitReturn = true;
                }
                else
                {
                    m.Assign(binder.EnsureRegister(op), m.Mem32(m.IAddS(sp, offset)));
                }
                offset += 4;
            }
            m.Assign(sp, m.IAdd(sp, offset));
            if (emitReturn)
            {
                m.Return(0, 0);
            }
        }

        private void RewritePushm()
        {
            foreach (var op in instr.Operands)
            {
                switch (op)
                {
                case RegisterStorage reg:
                    Push(Registers.sp, reg);
                    break;
                case RegisterRange range:
                    for (int i = 0; i < range.Count; ++i)
                    {
                        Push(Registers.sp, range.Registers[range.RegisterIndex + i]);
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Invalid operand type {op.GetType().Name}.");
                }
            }
        }

        private void RewriteRet()
        {
            MaybeSkip();
            var regSrc = (RegisterStorage) instr.Operands[0];
            var r12 = binder.EnsureRegister(Registers.GpRegisters[12]);
            var v = binder.EnsureFlagGroup(V);
            var c = binder.EnsureFlagGroup(C);
            switch (regSrc.Number)
            {
            case 13:
                m.Assign(r12, 0);
                m.Assign(binder.EnsureFlagGroup(N), Constant.False());
                m.Assign(binder.EnsureFlagGroup(Z), Constant.True());
                break;
            case 14:
                m.Assign(r12, -1);
                m.Assign(binder.EnsureFlagGroup(N), Constant.True());
                m.Assign(binder.EnsureFlagGroup(Z), Constant.False());
                break;
            case 15:
                m.Assign(r12, 1);
                m.Assign(binder.EnsureFlagGroup(N), Constant.False());
                m.Assign(binder.EnsureFlagGroup(Z), Constant.False());
                break;
            default:
                m.Assign(r12, binder.EnsureRegister(regSrc));
                EmitCc(NZ, r12);
                break;
            }
            m.Assign(v, Constant.False());
            m.Assign(c, Constant.False());
            m.Return(0, 0);
        }

        private void RewriteRol()
        {
            var src = RewriteOp(0);
            var c = binder.EnsureFlagGroup(C);
            var dst = RewriteOpDst(0, m.Fn(
                CommonOps.RolC.MakeInstance(src.DataType, PrimitiveType.Byte),
                src, m.Byte(1), c));
            EmitCc(NZC, dst);
        }

        private void RewriteRor()
        {
            var src = RewriteOp(0);
            var c = binder.EnsureFlagGroup(C);
            var dst = RewriteOpDst(0, m.Fn(
                CommonOps.RorC.MakeInstance(src.DataType, PrimitiveType.Byte),
                src, m.Byte(1), c));
            EmitCc(NZC, dst);
        }

        private void RewriteRsub()
        {
            MaybeSkip();
            Expression left;
            Expression right;
            if (instr.Operands.Length == 3)
            {
                left = RewriteOp(2);
                right = RewriteOp(1);
            }
            else
            {
                left = RewriteOp(1);
                right = RewriteOp(0);
            }
            var src = m.ISub(left, right);
            var dst = RewriteOpDst(0, src);
            EmitCc(VNZC, dst);
        }

        private void RewriteSat(IntrinsicProcedure intrinsic, PrimitiveType dt)
        {
            var left = RewriteOp(0);
            var right = RewriteOp(1);
            var dst = binder.EnsureRegister(((RegisterImmediateOperand) instr.Operands[0]).Register);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(dt), left, right));
            EmitCc(Q, dst);
        }

        private void RewriteSatsubW()
        {
            var left = RewriteOp(0);
            var right = RewriteOp(1);
            var src = m.Fn(satsub_intrinsic.MakeInstance(PrimitiveType.Int32), left, right);
            var dst = RewriteOpDst(0, src);
            EmitCc(VNZC, dst);
            EmitCc(Q, dst);
        }

        private void RewriteSbc()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var src = m.ISub(m.ISub(left, right), binder.EnsureFlagGroup(C));
            var dst = RewriteOpDst(0, src);
            EmitCc(VNZC, dst);
        }

        private void RewriteSbr()
        {
            var bit = ((ImmediateOperand) instr.Operands[1]).Value;
            var mask = Constant.UInt32(1U << bit.ToInt32());
            RewriteOpDst(0, m.Or(RewriteOp(0), mask));
            m.Assign(binder.EnsureFlagGroup(Z), Constant.False());
        }

        private void RewriteScr()
        {
            var src = m.ISub(RewriteOp(0), binder.EnsureFlagGroup(C));
            var dst = RewriteOpDst(0, src);
            EmitCc(VNZC, dst);
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn)
        {
            Expression left;
            Expression right;
            if (instr.Operands.Length == 3)
            {
                left = RewriteOp(1);
                right = RewriteOp(2);
            }
            else
            {
                left = RewriteOp(0);
                right = RewriteOp(1);
            }
            var dst = RewriteOpDst(0, fn(left, right));
            EmitCc(NZC, dst);
        }

        private void RewriteSr()
        {
            Expression src;
            if (instr.Condition == Avr32Condition.al)
            {
                src = m.Word32(1);
            }
            else
            {
                src = m.Convert(Condition(), PrimitiveType.Bool, PrimitiveType.Word32);
            }
            RewriteOpDst(0, src);
        }

        private void RewriteSt(PrimitiveType dt)
        {
            MaybeSkip();
            var src = RewriteOp(1);
            if (src.DataType.BitSize > dt.BitSize)
            {
                src = m.Slice(src, dt);
            }
            RewriteOpDst(0, src);
        }

        private void RewriteStcond()
        {
            var src = RewriteOp(1);
            var ea = ((MemoryAccess) RewriteOp(0)).EffectiveAddress;
            var tmp = binder.CreateTemporary(PrimitiveType.Ptr32);
            m.Assign(tmp, ea);
            m.Assign(binder.EnsureFlagGroup(Z), m.Fn(stcond_intrinsic, tmp, src));
        }

        private void RewriteStdsp()
        {
            RewriteOpDst(0, RewriteOp(1));
        }

        private void RewriteStm()
        {
            var regs = instr.Operands
                .Skip(1)
                .Cast<RegisterRange>()
                .SelectMany(rr => rr.Enumerate());
            Identifier sp;
            bool preDec = false;
            if (instr.Operands[0] is MemoryOperand pre)
            {
                Debug.Assert(pre.PreDecrement);
                sp = binder.EnsureRegister(pre.Base!);
                preDec = true;
            }
            else
            {
                sp = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            }
            int offset = 0;
            foreach (var reg in regs)
            {
                offset -= reg.DataType.Size;
                m.Assign(m.Mem(reg.DataType, m.IAddS(sp, offset)), binder.EnsureRegister(reg));
            }
            if (preDec)
            {
                m.Assign(sp, m.AddSubSignedInt(sp, offset));
            }
        }

        private void RewriteSub()
        {
            MaybeSkip();
            Expression left;
            Expression right;
            if (instr.Operands.Length == 3)
            {
                left = RewriteOp(1);
                right = RewriteOp(2);
            }
            else
            {
                left = RewriteOp(0);
                right = RewriteOp(1);
            }
            Expression src;
            if (right is Constant c)
            {
                if (c.IsZero)
                {
                    src = left;
                }
                else
                {
                    var value = c.ToInt32();
                    if (value < 0)
                    {
                        src = m.IAdd(left, m.Word32(-value));
                    }
                    else
                    {
                        src = m.ISub(left, c);
                    }
                }
            }
            else
            {
                src = m.ISub(left, right);
            }
            var dst = RewriteOpDst(0, src);
            if (instr.Mnemonic == Mnemonic.subf || instr.Condition == Avr32Condition.al)
            {
                EmitCc(VNZC, dst);
            }
        }

        private void RewriteTst()
        {
            var a = RewriteOp(0);
            var b = RewriteOp(1);
            EmitCc(NZ, m.And(a, b));
        }

        static Avr32Rewriter()
        {
            C = new FlagGroupStorage(Registers.sr, (uint) FlagM.CF, nameof(C), PrimitiveType.Bool);
            Z = new FlagGroupStorage(Registers.sr, (uint) FlagM.ZF, nameof(Z), PrimitiveType.Bool);
            N = new FlagGroupStorage(Registers.sr, (uint) FlagM.NF, nameof(N), PrimitiveType.Bool);
            Q = new FlagGroupStorage(Registers.sr, (uint) FlagM.QF, nameof(Q), PrimitiveType.Bool);
            V = new FlagGroupStorage(Registers.sr, (uint) FlagM.VF, nameof(V), PrimitiveType.Bool);
            NZ = new FlagGroupStorage(Registers.sr, (uint)(FlagM.NF | FlagM.ZF), nameof(NZ), PrimitiveType.Byte);
            NZC = new FlagGroupStorage(Registers.sr, (uint)(FlagM.NF | FlagM.ZF | FlagM.CF), nameof(NZC), PrimitiveType.Byte);
            VN = new FlagGroupStorage(Registers.sr, (uint)(FlagM.VF | FlagM.NF), nameof(VN), PrimitiveType.Byte);
            VNZ = new FlagGroupStorage(Registers.sr, (uint)(FlagM.VF | FlagM.NF | FlagM.ZF), nameof(VNZ), PrimitiveType.Byte);
            VNZC = new FlagGroupStorage(Registers.sr, (uint)(FlagM.VF | FlagM.NF | FlagM.ZF | FlagM.CF), nameof(VNZC), PrimitiveType.Byte);
            ZC = new FlagGroupStorage(Registers.sr, (uint)(FlagM.ZF | FlagM.CF), nameof(ZC), PrimitiveType.Byte);

            clz_intrinsic = new IntrinsicBuilder("__clz", false)
                .Param(PrimitiveType.Word32)
                .Returns(PrimitiveType.Int32);
            setbit_intrinsic = new IntrinsicBuilder("__setbit", false)
                .Param(PrimitiveType.Word32)
                .Param(PrimitiveType.Word32)
                .Param(PrimitiveType.Bool)
                .Returns(PrimitiveType.Word32);

            stcond_intrinsic = new IntrinsicBuilder("__stcond", true)
                .Param(PrimitiveType.Ptr32)
                .Param(PrimitiveType.Word32)
                .Returns(PrimitiveType.Bool);
        }

        private static readonly IntrinsicProcedure clz_intrinsic;
        private static readonly IntrinsicProcedure sat_intrinsic = IntrinsicBuilder.GenericBinary("__sat");
        private static readonly IntrinsicProcedure satsub_intrinsic = IntrinsicBuilder.GenericBinary("__satsub");
        private static readonly IntrinsicProcedure setbit_intrinsic;
        private static readonly IntrinsicProcedure stcond_intrinsic;

    }
}