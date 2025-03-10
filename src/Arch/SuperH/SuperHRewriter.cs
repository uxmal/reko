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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.SuperH
{
    public class SuperHRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly SuperHArchitecture arch;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly SuperHState state;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<SuperHInstruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private SuperHInstruction instr;
        private InstrClass iclass;

        public SuperHRewriter(
            SuperHArchitecture arch,
            Decoder<SuperHDisassembler, Mnemonic, SuperHInstruction> rootDecoder,
            EndianImageReader rdr,
            SuperHState state,
            IStorageBinder binder,
            IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new SuperHDisassembler(arch, rootDecoder, rdr).GetEnumerator();
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
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(
                        dasm.Current.Address,
                        string.Format(
                            "SuperH instruction {0} not supported yet.",
                        dasm.Current.Mnemonic));
                    EmitUnitTest();
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    Invalid();
                    break;
                case Mnemonic.add: RewriteBinOp(m.IAdd, n => (sbyte) n); break;
                case Mnemonic.addc: RewriteAddcSubc(m.IAdd); break;
                case Mnemonic.addv: RewriteAddv(m.IAdd); break;
                case Mnemonic.and: RewriteBinOp(m.And, n => (byte) n); break;
                case Mnemonic.and_b: RewriteBinOp(m.And, n => (byte) n); break;
                case Mnemonic.bf: RewriteBranch(false, false); break;
                case Mnemonic.bf_s: RewriteBranch(false, true); break;
                case Mnemonic.bra: RewriteGoto(); break;
                case Mnemonic.braf: RewriteBraf(); break;
                case Mnemonic.brk: RewriteBrk(); break;
                case Mnemonic.bsr: RewriteBsr(); break;
                case Mnemonic.bsrf: RewriteBsrf(); break;
                case Mnemonic.bt: RewriteBranch(true, false); break;
                case Mnemonic.bt_s: RewriteBranch(true, true); break;
                case Mnemonic.clrmac: RewriteClr(Registers.mac); break;
                case Mnemonic.clrs: RewriteClrtSet(Registers.S, Constant.False()); break;
                case Mnemonic.clrt: RewriteClrtSet(Registers.T, Constant.False()); break;
                case Mnemonic.cmp_eq: RewriteCmp(m.Eq); break;
                case Mnemonic.cmp_ge: RewriteCmp(m.Ge); break;
                case Mnemonic.cmp_gt: RewriteCmp(m.Gt); break;
                case Mnemonic.cmp_hs: RewriteCmp(m.Uge); break;
                case Mnemonic.cmp_hi: RewriteCmp(m.Ugt); break;
                case Mnemonic.cmp_pl: RewriteCmp0(m.Gt0); break;
                case Mnemonic.cmp_pz: RewriteCmp0(m.Ge0); break;
                case Mnemonic.cmp_str: RewriteCmpStr(); break;
                case Mnemonic.div0s: RewriteDiv0s(); break;
                case Mnemonic.div0u: RewriteDiv0u(); break;
                case Mnemonic.div1: RewriteDiv1(); break;
                case Mnemonic.divs: RewriteDivs(); break;
                case Mnemonic.dmuls_l: RewriteDmul(Operator.SMul, PrimitiveType.Int64); break;
                case Mnemonic.dmulu_l: RewriteDmul(Operator.UMul, PrimitiveType.UInt64); break;
                case Mnemonic.dt: RewriteDt(); break;
                case Mnemonic.exts_b: RewriteExt(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Mnemonic.exts_w: RewriteExt(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.extu_b: RewriteExt(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Mnemonic.extu_w: RewriteExt(PrimitiveType.UInt16, PrimitiveType.UInt32); break;
                case Mnemonic.fabs: RewriteFabs(); break;
                case Mnemonic.fadd: RewriteBinOp(m.FAdd, null); break;
                case Mnemonic.fcmp_eq: RewriteCmp(m.FEq); break;
                case Mnemonic.fcmp_gt: RewriteCmp(m.FGt); break;
                case Mnemonic.fcnvds: RewriteUnary(d => m.Convert(d, PrimitiveType.Real64, PrimitiveType.Real32)); break;
                case Mnemonic.fcnvsd: RewriteUnary(d => m.Convert(d, PrimitiveType.Real32, PrimitiveType.Real64)); break;
                case Mnemonic.fdiv: RewriteBinOp(m.FDiv, null); break;
                case Mnemonic.fldi0: RewriteFldi(0.0F); break;
                case Mnemonic.fldi1: RewriteFldi(1.0F); break;
                case Mnemonic.flds: RewriteMov(); break;
                case Mnemonic.@float: RewriteFloat(); break;
                case Mnemonic.fmac: RewriteFmac(); break;
                case Mnemonic.fmov: RewriteMov(); break;
                case Mnemonic.fmov_d: RewriteMov(); break;
                case Mnemonic.fmov_s: RewriteMov(); break;
                case Mnemonic.fmul: RewriteBinOp(m.FMul, n => n); break;
                case Mnemonic.fsts: RewriteMov(); break;
                case Mnemonic.ftrc: RewriteFtrc(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.lds: RewriteMov(); break;
                case Mnemonic.ldc: RewriteMov(); break;
                case Mnemonic.ldc_l: RewriteMov(); break;
                case Mnemonic.lds_l: RewriteMov(); break;
                case Mnemonic.ldtlb: RewriteLdtlb(); break;
                case Mnemonic.mac_l: RewriteMac(PrimitiveType.Int64); break;
                case Mnemonic.mac_w: RewriteMac(PrimitiveType.Int32); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.mov_b: RewriteMov(); break;
                case Mnemonic.mov_w: RewriteMov(); break;
                case Mnemonic.mov_l: RewriteMov(); break;
                case Mnemonic.mova: RewriteMova(); break;
                case Mnemonic.movca_l: RewriteMovca(); break;
                case Mnemonic.movco_l: RewriteMovco(); break;
                case Mnemonic.movt: RewriteMovt(); break;
                case Mnemonic.mul_l: RewriteMul_l(); break;
                case Mnemonic.mulr: RewriteBinOp(m.IMul, null); break;
                case Mnemonic.muls_w: RewriteMul_w(PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.mulu_w: RewriteMul_w(PrimitiveType.UInt16, m.UMul); break;
                case Mnemonic.neg: RewriteUnary(m.Neg); break;
                case Mnemonic.negc: RewriteNegc(); break;
                case Mnemonic.not: RewriteUnary(m.Comp); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.ocbi: RewriteOcb(ocbi_intrinsic); break;
                case Mnemonic.ocbp: RewriteOcb(ocbp_intrinsic); break;
                case Mnemonic.or: RewriteBinOp(m.Or, u => (byte) u); break;
                case Mnemonic.pref: RewritePrefetch(CommonOps.Prefetch); break;
                case Mnemonic.prefi: RewritePrefetch(CommonOps.PrefetchInstruction); break;
                case Mnemonic.rotcl: RewriteRotc(CommonOps.RolC); break;
                case Mnemonic.rotcr: RewriteRotc(CommonOps.RorC); break;
                case Mnemonic.rotl: RewriteRot(CommonOps.Rol); break;
                case Mnemonic.rotr: RewriteRot(CommonOps.Ror); break;
                case Mnemonic.rts: RewriteRts(); break;
                case Mnemonic.sets: RewriteClrtSet(Registers.S, Constant.True()); break;
                case Mnemonic.sett: RewriteClrtSet(Registers.T, Constant.True()); break;
                case Mnemonic.shad: RewriteShd(m.Shl, m.Sar); break;
                case Mnemonic.shal: RewriteShift1(m.Shl, arch.WordWidth.BitSize-1); break;
                case Mnemonic.shar: RewriteShift1(m.Sar, 0); break;
                case Mnemonic.shld: RewriteShd(m.Shl, m.Shr); break;
                case Mnemonic.shll: RewriteShift(m.Shl, 1); break;
                case Mnemonic.shll2: RewriteShift(m.Shl, 2); break;
                case Mnemonic.shll8: RewriteShift(m.Shl, 8); break;
                case Mnemonic.shll16: RewriteShift(m.Shl, 16); break;
                case Mnemonic.shlr: RewriteShift(m.Shr, 1); break;
                case Mnemonic.shlr2: RewriteShift(m.Shr, 2); break;
                case Mnemonic.shlr8: RewriteShift(m.Shr, 8); break;
                case Mnemonic.shlr16: RewriteShift(m.Shr, 16); break;
                case Mnemonic.stc: RewriteMov(); break;
                case Mnemonic.stc_l: RewriteMov(); break;
                case Mnemonic.sts: RewriteMov(); break;
                case Mnemonic.sts_l: RewriteMov(); break;
                case Mnemonic.sub: RewriteBinOp(m.ISub, null); break;
                case Mnemonic.subc: RewriteAddcSubc(m.ISub); break;
                case Mnemonic.swap_w: RewriteSwapW(); break;
                case Mnemonic.tas_b: RewriteTas(PrimitiveType.Byte); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.xor: RewriteBinOp(m.Xor, n => (byte) n); break;
                case Mnemonic.xtrct: RewriteXtrct(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, this.iclass);
                this.instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Invalid()
        {
            this.iclass = InstrClass.Invalid;
            m.Invalid();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("SHRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression SrcOp(int iop, Func<int, int>? immediateFn = null)
            => SrcOp(instr.Operands[iop], immediateFn);

        private Expression SrcOp(MachineOperand op, Func<int, int>? immediateFn = null)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                var id = binder.EnsureRegister(regOp);
                return id;
            case Constant immOp:
                return Constant.Word32(immediateFn!(immOp.ToInt32()));

            case Address addrOp:
                return addrOp;
            case MemoryOperand mem:
                Identifier reg;
                switch (mem.mode)
                {
                default:
                    throw new NotImplementedException(mem.mode.ToString());
                case AddressingMode.Indirect:
                    return m.Mem(mem.DataType, binder.EnsureRegister(mem.reg));
                case AddressingMode.IndirectPreDecr:
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(reg, m.ISubS(reg, mem.DataType.Size));
                    return m.Mem(mem.DataType, reg);
                case AddressingMode.IndirectPostIncr:
                    var t = binder.CreateTemporary(mem.DataType);
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(t, m.Mem(mem.DataType, reg));
                    m.Assign(reg, m.IAddS(reg, t.DataType.Size));
                    return t;
                case AddressingMode.IndirectDisplacement:
                    reg = binder.EnsureRegister(mem.reg);
                    return m.Mem(
                        mem.DataType,
                        m.IAddS(reg, mem.disp));
                case AddressingMode.IndexedIndirect:
                    return m.Mem(mem.DataType, m.IAdd(
                        binder.EnsureRegister(Registers.r0),
                        binder.EnsureRegister(mem.reg)));
                case AddressingMode.PcRelativeDisplacement:
                    var addr = instr.Address.ToUInt32();
                    if (mem.DataType.Size == 4)
                    {
                        addr &= ~3u;
                    }
                    addr += (uint) (mem.disp + 4);
                    return m.Mem(mem.DataType, Address.Ptr32(addr));
                }
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Expression DstOp(MachineOperand op, Expression src, Func<Expression, Expression, Expression> fn)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                var id = binder.EnsureRegister(regOp);
                var result = fn(id, src);
                if (result.DataType.BitSize < regOp.DataType.BitSize)
                {
                    result = m.ExtendS(result, regOp.DataType);
                }
                m.Assign(id, result);
                return id;

            case MemoryOperand mem:
                Identifier r0;
                Identifier gbr;
                var tmp = binder.CreateTemporary(op.DataType);
                switch (mem.mode)
                {
                case AddressingMode.GbrIndexedIndirect:
                    r0 = binder.EnsureRegister(Registers.r0);
                    gbr = binder.EnsureRegister(Registers.gbr);
                    m.Assign(tmp, m.Mem(tmp.DataType, m.IAdd(r0, gbr)));
                    m.Assign(
                        m.Mem(tmp.DataType, m.IAdd(r0, gbr)),
                        fn(tmp, src));
                    return tmp;
                default: throw new NotImplementedException();
                }
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Expression? DstOp(MachineOperand op, Expression src, Func<Expression, Expression> fn)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                var id = binder.EnsureRegister(regOp);
                var result = fn(src);
                if (result.DataType.BitSize < regOp.DataType.BitSize)
                {
                    result = m.ExtendS(result, regOp.DataType);
                }
                m.Assign(id, result);
                return id;
            case MemoryOperand mem:
                Identifier r0;
                Identifier gbr;
                Identifier reg;
                var tmp = binder.CreateTemporary(op.DataType);
                switch (mem.mode)
                {
                case AddressingMode.Indirect:
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(
                        m.Mem(mem.DataType, reg),
                        fn(src));
                    return null;
                case AddressingMode.IndirectDisplacement:
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(
                        m.Mem(mem.DataType, m.IAddS(reg, mem.disp)),
                        fn(src));
                    return null;
                case AddressingMode.IndirectPreDecr:
                    reg = binder.EnsureRegister(mem.reg);
                    m.Assign(reg, m.ISubS(reg, mem.DataType.Size));
                    m.Assign(
                        m.Mem(tmp.DataType, reg),
                        fn(src));
                    return null;
                case AddressingMode.IndexedIndirect:
                    m.Assign(
                        m.Mem(mem.DataType, m.IAdd(
                            binder.EnsureRegister(Registers.r0),
                            binder.EnsureRegister(mem.reg))),
                        fn(src));
                    return null;
                case AddressingMode.GbrIndexedIndirect:
                    r0 = binder.EnsureRegister(Registers.r0);
                    gbr = binder.EnsureRegister(Registers.gbr);
                    m.Assign(tmp, m.Mem(tmp.DataType, m.IAdd(r0, gbr)));
                    m.Assign(
                        m.Mem(tmp.DataType, m.IAdd(r0, gbr)),
                        fn(src));
                    return tmp;
                default: throw new NotImplementedException(mem.mode.ToString());
                }
            }
            throw new AddressCorrelatedException(instr.Address,
                new NotImplementedException(op.GetType().Name),
                $"Unimplemented DstOp: {instr}.");
        }


        private void RewriteAddcSubc(Func<Expression, Expression, Expression> fn)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[1], src, (a, b) =>
                fn(fn(a, b), t));
        }

        private void RewriteNegc()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[1], src, (d, s) =>
                m.ISub(m.Neg(s), t));
        }

        private void RewriteAddv(Func<Expression, Expression, Expression> fn)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[1], src, fn);
            m.Assign(t, m.Test(ConditionCode.OV, dst));
        }

        private void RewriteBinOp(
            Func<Expression, Expression, Expression> fn,
            Func<int, int>? immediateFn)
        {
            var src = SrcOp(instr.Operands[0], immediateFn);
            var dst = DstOp(instr.Operands[1], src, fn);
        }

        private void RewriteBranch(bool takenOnTset, bool delaySlot)
        {
            this.iclass = delaySlot
                ? InstrClass.ConditionalTransfer | InstrClass.Delay
                : InstrClass.ConditionalTransfer;
            Expression cond = binder.EnsureFlagGroup(Registers.T);
            var addr = (Address) instr.Operands[0];
            if (!takenOnTset)
                cond = m.Not(cond);
            m.Branch(cond, addr, this.iclass);
        }

        private void RewriteBraf()
        {
            this.iclass = InstrClass.Delay | InstrClass.Transfer;
            var reg = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            m.GotoD(m.IAdd(instr.Address + 4, reg));
        }

        private void RewriteBrk()
        {
            m.SideEffect(m.Fn(brk_intrinsic));
        }

        private void RewriteBsr()
        {
            this.iclass = InstrClass.Transfer | InstrClass.Call | InstrClass.Delay;
            var dst = SrcOp(instr.Operands[0], null);
            m.CallD(dst, 0);
        }

        private void RewriteBsrf()
        {
            this.iclass = InstrClass.Transfer | InstrClass.Delay;
            var src = SrcOp(instr.Operands[0], null);
            var reg = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            m.CallD(m.IAdd(instr.Address + 4, src), 0);
        }

        private void RewriteGoto()
        {
            this.iclass = InstrClass.Transfer | InstrClass.Delay;
            var addr = (Address)instr.Operands[0];
            m.GotoD(addr);
        }

        private void RewriteCmp(Func<Expression, Expression, Expression> fn)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var op1 = SrcOp(instr.Operands[0], n => (sbyte) n);
            var op2 = SrcOp(instr.Operands[1], null);
            m.Assign(t, fn(op2, op1));
        }


        private void RewriteClr(RegisterStorage reg)
        {
            iclass = InstrClass.Linear;
            var dst = binder.EnsureRegister(reg);
            var z = Constant.Zero(dst.DataType);
            m.Assign(dst, z);
        }

        private void RewriteClrtSet(FlagGroupStorage f, Expression e)
        {
            var t = binder.EnsureFlagGroup(f);
            m.Assign(t, e);
        }


        private void RewriteCmp0(Func<Expression, Expression> fn)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var op1 = SrcOp(instr.Operands[0], n => (sbyte) n);
            m.Assign(t, fn(op1));
        }

        private void RewriteCmpStr()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var left = SrcOp(instr.Operands[1]);
            var right = SrcOp(instr.Operands[0]);
            m.Assign(t, m.Fn(cmp_str_intrinsic, left, right));
        }


        private void RewriteDiv0s()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = SrcOp(instr.Operands[1]);
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, m.Fn(div0s_intrinsic.MakeInstance(src.DataType), dst, src));
        }

        private void RewriteDiv0u()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, m.Fn(div0u_intrinsic));
        }


        private void RewriteDiv1()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = SrcOp(instr.Operands[1]);
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(dst, m.Fn(div1_intrinsic.MakeInstance(src.DataType), dst, src));
        }

        private void RewriteDivs()
        {
            var src = SrcOp(0);
            var dst = SrcOp(1);
            m.Assign(dst, m.SDiv(dst, src));
        }

        private void RewriteDmul(BinaryOperator fn, PrimitiveType dtProduct)
        {
            var op1 = SrcOp(0);
            var op2 = SrcOp(1);
            var mac = binder.EnsureRegister(Registers.mac);
            m.Assign(mac, m.Bin(fn, dtProduct, op2, op1));
        }

        private void RewriteDt()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var r = DstOp(instr.Operands[0], Constant.Word32(1), m.ISub);
            m.Assign(t, m.Eq0(r));
        }

        private void RewriteExt(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[1], src, (a, b) =>
            {
                if (b.DataType.BitSize > dtSrc.BitSize)
                    b = m.Slice(b, dtSrc);
                return m.Convert(b, dtSrc, dtDst);
            });
        }

        private void RewriteFabs()
        {
            Expression select_fabs(Expression a)
            {
                var intrinsic = (a.DataType.BitSize == 32)
                    ? FpOps.fabsf
                    : FpOps.fabs;
                return m.Fn(intrinsic, a);
            }
            var src = SrcOp(instr.Operands[0], null);
            var dst = DstOp(instr.Operands[0], src, select_fabs);
        }

        private void RewriteFldi(float f)
        {
            DstOp(instr.Operands[0], Constant.Real32(f), a => a);
        }

        private void RewriteFloat()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = DstOp(instr.Operands[1], src, (a, b) => m.Convert(
                b,
                PrimitiveType.Create(Domain.SignedInt, b.DataType.BitSize),
                PrimitiveType.Create(Domain.Real, a.DataType.BitSize)));
        }

        private void RewriteFmac()
        {
            var f0 = binder.EnsureRegister(Registers.fr0);
            var mul = SrcOp(instr.Operands[1]);
            var dst = SrcOp(instr.Operands[2]);
            m.Assign(dst, m.FAdd(m.FMul(f0, mul), dst));
        }

        private void RewriteFtrc()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = DstOp(instr.Operands[1], src, (d, s) =>
            {
                Expression e;
                if (s.DataType.BitSize == 64)
                {
                    e = m.Fn(FpOps.trunc, s);
                }
                else
                {
                    e = m.Fn(FpOps.truncf, s);
                }
                return m.Convert(e, e.DataType, PrimitiveType.Int32);
            });
        }

        private void RewriteJmp()
        {
            this.iclass = InstrClass.Transfer | InstrClass.Delay;
            var src = SrcOp(instr.Operands[0]);
            m.GotoD(((MemoryAccess) src).EffectiveAddress);
        }

        private void RewriteJsr()
        {
            this.iclass = InstrClass.Transfer | InstrClass.Delay;
            var dst = SrcOp(instr.Operands[0], null);
            m.CallD(dst, 0);
        }

        private void RewriteLdtlb()
        {
            m.SideEffect(m.Fn(ldtlb_intrinsic));
        }

        private void RewriteMac(PrimitiveType dt)
        {
            var left = SrcOp(instr.Operands[0]);
            var right = SrcOp(instr.Operands[1]);
            var mac = binder.EnsureRegister(Registers.mac);
            var mul = m.SMul(left, right);
            mul.DataType = dt;
            m.Assign(mac, m.IAdd(mul, mac));
        }

        private void RewriteMov()
        {
            var src = SrcOp(instr.Operands[0], a => (sbyte)a);
            var dst = DstOp(instr.Operands[1], src, a => a);
        }

        private void RewriteMova()
        {
            var src = (MemoryAccess)SrcOp(instr.Operands[0], a => (sbyte)a);
            var dst = DstOp(instr.Operands[1], src.EffectiveAddress, a => a);
        }

        private void RewriteMovca()
        {
            var src = SrcOp(0);
            var dst = SrcOp(1);
            m.SideEffect(m.Fn(movca_intrinsic,
                src,
                m.AddrOf(PrimitiveType.Ptr32, dst)));
        }

        private void RewriteMovco()
        {
            var src = SrcOp(0);
            var dst = SrcOp(1);
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, m.Fn(
                movco_intrinsic,
                src,
                m.AddrOf(PrimitiveType.Ptr32, dst)));
        }

        private void RewriteMovt()
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var dtDst = instr.Operands[0].DataType;
            var dst = DstOp(instr.Operands[0], t, a => m.ExtendZ(m.Ne0(a), dtDst));
        }

        private void RewriteMul_l()
        {
            var macl = binder.EnsureRegister(Registers.macl);
            var op1 = SrcOp(instr.Operands[0]);
            var op2 = SrcOp(instr.Operands[1]);
            m.Assign(macl, m.IMul(op2, op1));
        }

        private void RewriteMul_w(DataType dt, Func<Expression, Expression, Expression> fn)
        {
            var macl = binder.EnsureRegister(Registers.macl);
            var op1 = m.Convert(SrcOp(instr.Operands[0]), instr.Operands[0].DataType, dt);
            var op2 = m.Convert(SrcOp(instr.Operands[1]), instr.Operands[1].DataType, dt);
            m.Assign(macl, fn(op2, op1));
        }

        private void RewriteOcb(IntrinsicProcedure intrinsic)
        {
            var mem = (MemoryAccess)SrcOp(0);
            mem.DataType = PrimitiveType.Ptr32;
            var call = m.Fn(intrinsic, m.AddrOf(PrimitiveType.Ptr32, mem));
            m.SideEffect(call);
        }

        private void RewriteRot(IntrinsicProcedure intrinsic)
        {
            var op1 = SrcOp(0);
            m.Assign(op1, m.Fn(intrinsic, op1, m.Int32(1)));
        }

        private void RewritePrefetch(IntrinsicProcedure intrinsic)
        {
            var op = SrcOp(0);
            m.SideEffect(
                m.Fn(intrinsic.MakeInstance(arch.PointerType.BitSize, arch.PointerType),
                m.AddrOf(arch.PointerType, op)));
        }

        private void RewriteRotc(IntrinsicProcedure intrinsic)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            var op1 = SrcOp(instr.Operands[0]);
            var one = m.Byte(1);
            m.Assign(op1, m.Fn(
                intrinsic.MakeInstance(op1.DataType, one.DataType),
                op1, one, t));
        }

        private void RewriteRts()
        {
            m.Return(0, 0);
        }

        private void RewriteShd(Func<Expression, Expression, Expression> fnLeft, Func<Expression, Expression, Expression> fnRight)
        {
            var sh = SrcOp(instr.Operands[0]);
            var dst = DstOp(instr.Operands[1], sh, (d, s) =>
                m.Conditional(d.DataType, m.Ge0(s), fnLeft(d, s), fnRight(d, s)));
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn, int c)
        {
            var src = Constant.Int32(c);
            var dst = DstOp(instr.Operands[0], src, fn);
        }

        private void RewriteShift1(Func<Expression, Expression, Expression> fn, int bit)
        {
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, m.Fn(CommonOps.Bit, SrcOp(0), Constant.Int32(bit)));
            var dst = DstOp(instr.Operands[0], Constant.Int32(1), fn);
        }

        private void RewriteSwapW()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = SrcOp(instr.Operands[1]);
            m.Assign(dst, m.Fn(swap_w_intrinsic, src));
        }

        private void RewriteTas(PrimitiveType dt)
        {
            var ea = SrcOp(0);
            ea.DataType = dt;
            var fn = AtomicOps.atomic_test_and_set.MakeInstance(arch.PointerType.BitSize, dt);
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, m.Fn(fn, m.AddrOf(arch.PointerType, ea)));
        }

        private void RewriteTst()
        {
            var op1 = SrcOp(instr.Operands[0], u => (byte)u);
            var op2 = SrcOp(instr.Operands[1]);
            var t = binder.EnsureFlagGroup(Registers.T);
            m.Assign(t, m.Eq0(m.And(op2, op1)));
        }

        private void RewriteUnary(Func<Expression, Expression> fn)
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = DstOp(instr.Operands[1], src, fn);
        }

        private void RewriteXtrct()
        {
            var src = SrcOp(instr.Operands[0]);
            var dst = SrcOp(instr.Operands[1]);
            m.Assign(dst, m.Fn(xtrct_intrinsic, dst, src));
        }


        private static readonly IntrinsicProcedure brk_intrinsic = new IntrinsicBuilder("__brk", true, new()
        {
             Terminates = true,
        })
            .Void();
        private static readonly IntrinsicProcedure cmp_str_intrinsic = new IntrinsicBuilder("__cmp_str", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure div0s_intrinsic = new IntrinsicBuilder("__div0s", true)
            .GenericTypes("T")
            .Params("T", "T")
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure div0u_intrinsic = new IntrinsicBuilder("__div0u", true)
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure div1_intrinsic = new IntrinsicBuilder("__div1", true)
            .GenericTypes("T")
            .Params("T", "T")
            .Returns(PrimitiveType.Bool);

        private static readonly IntrinsicProcedure ldtlb_intrinsic = new IntrinsicBuilder("__load_tlb", true)
            .Void();
        private static readonly IntrinsicProcedure movca_intrinsic = new IntrinsicBuilder("__move_with_cache_block_allocation", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure movco_intrinsic = new IntrinsicBuilder("__move_conditional", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Bool);


        private static readonly IntrinsicProcedure ocbi_intrinsic = new IntrinsicBuilder("__operand_cache_block_invalidate", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure ocbp_intrinsic = new IntrinsicBuilder("__operand_cache_block_purge", true)
            .Param(PrimitiveType.Ptr32)
            .Void();

        private static readonly IntrinsicProcedure swap_w_intrinsic = IntrinsicBuilder.Pure("__swap_w")
            .GenericTypes("T")
            .Param("T")
            .Returns("T");

        private static readonly IntrinsicProcedure xtrct_intrinsic = IntrinsicBuilder.Pure("__xtrct")
            .GenericTypes("T1", "T2")
            .Params("T1", "T2")
            .Returns("T2");


    }
}
