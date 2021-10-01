#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.Arm.AArch64
{
    // https://developer.arm.com/technologies/neon/intrinsics?_ga=2.34513633.1180694635.1535627528-1335591578.1525783726
    public partial class A64Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Arm64Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly List<RtlInstruction> cluster;
        private readonly RtlEmitter m;
        private readonly IEnumerator<AArch64Instruction> dasm;
        private AArch64Instruction instr;
        private InstrClass iclass;

        public A64Rewriter(Arm64Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new AArch64Disassembler(arch, rdr).GetEnumerator();
            this.instr = null!;
            this.cluster = new List<RtlInstruction>();
            this.m = new RtlEmitter(cluster);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    host.Warn(
                        instr.Address,
                        "AArch64 instruction {0} is not supported yet.",
                        instr);
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.adc: RewriteAdcSbc(m.IAdd); break;
                case Mnemonic.adcs: RewriteAdcSbc(m.IAdd, this.NZCV); break;
                case Mnemonic.add: RewriteMaybeSimdBinary(m.IAdd, "__add_{0}"); break;
                case Mnemonic.addhn: RewriteSimdBinary("__addhn_{0}", Domain.None); break;
                case Mnemonic.addhn2: RewriteSimdBinary("__addhn2_{0}", Domain.None); break;
                case Mnemonic.adds: RewriteBinary(m.IAdd, this.NZCV); break;
                case Mnemonic.addv: RewriteAddv(); break;
                case Mnemonic.adr: RewriteUnary(n => n); break;
                case Mnemonic.asrv: RewriteBinary(m.Sar); break;
                case Mnemonic.adrp: RewriteAdrp(); break;
                case Mnemonic.and: RewriteBinary(m.And); break;
                case Mnemonic.ands: RewriteBinary(m.And, this.NZ00); break;
                case Mnemonic.asr: RewriteBinary(m.Sar); break;
                case Mnemonic.b: RewriteB(); break;
                case Mnemonic.bfm: RewriteBfm(); break;
                case Mnemonic.bic: RewriteBinary((a, b) => m.And(a, m.Comp(b))); break;
                case Mnemonic.bics: RewriteBinary((a, b) => m.And(a, m.Comp(b)), NZ00); break;
                case Mnemonic.bit: RewriteSimdBinary("__bit_{0}", Domain.None); break;
                case Mnemonic.bl: RewriteBl(); break;
                case Mnemonic.blr: RewriteBlr(); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.brk: RewriteBrk(); break;
                case Mnemonic.bsl: RewriteBsl(); break;
                case Mnemonic.cbnz: RewriteCb(m.Ne0); break;
                case Mnemonic.cbz: RewriteCb(m.Eq0); break;
                case Mnemonic.ccmn: RewriteCcmn(); break;
                case Mnemonic.ccmp: RewriteCcmp(); break;
                case Mnemonic.clz: RewriteClz(); break;
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.cmeq: RewriteCm("__cmeq"); break;
                case Mnemonic.cmhi: RewriteCm("__cmhi"); break;
                case Mnemonic.cmhs: RewriteCm("__cmhs"); break;
                case Mnemonic.csel: RewriteCsel(); break;
                case Mnemonic.csinc: RewriteCsinc(); break;
                case Mnemonic.csinv: RewriteCsinv(); break;
                case Mnemonic.csneg: RewriteCsneg(); break;
                case Mnemonic.dmb: RewriteDmb(); break;
                case Mnemonic.dsb: RewriteDsb(); break;
                case Mnemonic.dup: RewriteDup(); break;
                case Mnemonic.eor: RewriteBinary(m.Xor); break;
                case Mnemonic.eon: RewriteBinary((a, b) => m.Xor(a, m.Comp(b))); break;
                case Mnemonic.eret: RewriteEret(); break;
                case Mnemonic.ext: RewriteExt(); break;
                case Mnemonic.extr: RewriteExtr(); break;
                case Mnemonic.fabs: RewriteFabs(); break;
                case Mnemonic.fadd: RewriteFadd(); break;
                case Mnemonic.fcmp: RewriteFcmp(); break;
                case Mnemonic.fcmpe: RewriteFcmp(); break;  //$REVIEW: this leaves out the 'e'xception part. 
                case Mnemonic.fcsel: RewriteFcsel(); break;
                case Mnemonic.fcvt: RewriteFcvt(); break;
                case Mnemonic.fcvtms: RewriteFcvtms(); break;
                case Mnemonic.fcvtps: RewriteFcvtps(); break;
                case Mnemonic.fcvtzs: RewriteFcvtzs(); break;
                case Mnemonic.fdiv: RewriteMaybeSimdBinary(m.FDiv, "__fdiv_{0}", Domain.Real); break;
                case Mnemonic.fmadd: RewriteIntrinsicFTernary("__fmaddf", "__fmadd"); break;
                case Mnemonic.fmsub: RewriteIntrinsicFTernary("__fmsubf", "__fmsub"); break;
                case Mnemonic.fmax: RewriteIntrinsicFBinary("fmaxf", "fmax"); break;
                case Mnemonic.fmin: RewriteIntrinsicFBinary("fminf", "fmin"); break;
                case Mnemonic.fmov: RewriteFmov(); break;
                case Mnemonic.fmul: RewriteFmul(); break;
                case Mnemonic.fneg: RewriteUnary(m.FNeg); break;
                case Mnemonic.fnmul: RewriteFnmul(); break;
                case Mnemonic.fsqrt: RewriteFsqrt(); break;
                case Mnemonic.fsub: RewriteMaybeSimdBinary(m.FSub, "__fsub_{0}", Domain.Real); break;
                case Mnemonic.hlt: RewriteHlt(); break;
                case Mnemonic.isb: RewriteIsb(); break;
                case Mnemonic.ld1: RewriteLdN("__ld1"); break;
                case Mnemonic.ld1r: RewriteLdNr("__ld1r"); break;
                case Mnemonic.ld2: RewriteLdN("__ld2"); break;
                case Mnemonic.ld3: RewriteLdN("__ld3"); break;
                case Mnemonic.ld4: RewriteLdN("__ld4"); break;
                case Mnemonic.ldnp: RewriteLoadStorePair(true); break;
                case Mnemonic.ldp: RewriteLoadStorePair(true); break;
                case Mnemonic.ldarh: RewriteLoadAcquire("__load_acquire_{0}", PrimitiveType.Word16); break;
                case Mnemonic.ldaxrh: RewriteLoadAcquire("__load_acquire_exclusive_{0}", PrimitiveType.Word16); break;
                case Mnemonic.ldpsw: RewriteLoadStorePair(true, PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.ldr: RewriteLdr(null); break;
                case Mnemonic.ldrb: RewriteLdr(PrimitiveType.Byte); break;
                case Mnemonic.ldrh: RewriteLdr(PrimitiveType.Word16); break;
                case Mnemonic.ldrsb: RewriteLdr(PrimitiveType.SByte); break;
                case Mnemonic.ldrsh: RewriteLdr(PrimitiveType.Int16); break;
                case Mnemonic.ldrsw: RewriteLdr(PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.ldxr: RewriteLdx(instr.Operands[0].Width); break;
                case Mnemonic.lslv: RewriteBinary(m.Shl); break;
                case Mnemonic.lsrv: RewriteBinary(m.Shr); break;
                case Mnemonic.ldur: RewriteLdr(null); break;
                case Mnemonic.ldurb: RewriteLdr(PrimitiveType.Byte); break;
                case Mnemonic.ldurh: RewriteLdr(PrimitiveType.Word16); break;
                case Mnemonic.ldursb: RewriteLdr(PrimitiveType.SByte); break;
                case Mnemonic.ldursh: RewriteLdr(PrimitiveType.Int16); break;
                case Mnemonic.ldursw: RewriteLdr(PrimitiveType.Int32); break;
                case Mnemonic.lsl: RewriteBinary(m.Shl); break;
                case Mnemonic.lsr: RewriteBinary(m.Shr); break;
                case Mnemonic.madd: RewriteMaddSub(m.IAdd); break;
                case Mnemonic.mneg: RewriteBinary((a, b) => m.Neg(m.IMul(a, b))); break;
                case Mnemonic.msub: RewriteMaddSub(m.ISub); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movi: RewriteMovi(); break;
                case Mnemonic.movk: RewriteMovk(); break;
                case Mnemonic.movn: RewriteMovn(); break;
                case Mnemonic.movz: RewriteMovz(); break;
                case Mnemonic.mrs: RewriteMrs(); break;
                case Mnemonic.msr: RewriteMsr(); break;
                case Mnemonic.mul: RewriteMaybeSimdBinary(m.IMul, "__mul_{0}"); break;
                case Mnemonic.mvn: RewriteUnary(m.Comp); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.not: RewriteMaybeSimdUnary(m.Comp, "__not_{0}"); break;
                case Mnemonic.orr: RewriteBinary(m.Or);break;
                case Mnemonic.orn: RewriteBinary((a, b) => m.Or(a, m.Comp(b))); break;
                case Mnemonic.prfm: RewritePrfm(); break;
                case Mnemonic.rbit: RewriteRbit(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.rev: RewriteRev(); break;
                case Mnemonic.rev16: RewriteRev16(); break;
                case Mnemonic.rev32: RewriteRev32(); break;
                case Mnemonic.ror: RewriteRor(); break;
                case Mnemonic.rorv: RewriteRor(); break;
                case Mnemonic.sbc: RewriteAdcSbc(m.ISub); break;
                case Mnemonic.sbcs: RewriteAdcSbc(m.ISub, NZCV); break;
                case Mnemonic.sbfiz: RewriteSbfiz(); break;
                case Mnemonic.sbfm: RewriteUSbfm("__sbfm"); break;
                case Mnemonic.scvtf: RewriteScvtf(); break;
                case Mnemonic.sdiv: RewriteBinary(m.SDiv); break;
                case Mnemonic.shrn: RewriteShrn(); break;
                case Mnemonic.smaddl: RewriteMaddl(PrimitiveType.Int64, m.SMul); break;
                case Mnemonic.smax: RewriteSmax(); break;
                case Mnemonic.smaxv: RewriteSmaxv(); break;
                case Mnemonic.smc: RewriteSmc(); break;
                case Mnemonic.smsubl: RewriteSmsubl(); break;
                case Mnemonic.smull: RewriteMull(PrimitiveType.Int32, PrimitiveType.Int64, m.SMul); break;
                case Mnemonic.smulh: RewriteMulh(PrimitiveType.Int64, PrimitiveType.Int128, m.SMul); break;
                case Mnemonic.sqdmulh: RewriteSimdBinary("__sqdmulh_{0}", Domain.SignedInt); break;
                case Mnemonic.sshr: RewriteSimdWithScalar("__sshr_{0}", Domain.SignedInt); break;
                case Mnemonic.ssubl: RewriteSimdBinary("__ssubl_{0}", Domain.SignedInt); break;
                case Mnemonic.ssubl2: RewriteSimdBinary("__ssubl2_{0}", Domain.SignedInt); break;
                case Mnemonic.st1: RewriteStN("__st1"); break;
                case Mnemonic.st2: RewriteStN("__st2"); break;
                case Mnemonic.st3: RewriteStN("__st3"); break;
                case Mnemonic.st4: RewriteStN("__st4"); break;
                case Mnemonic.stlr: RewriteStlr(instr.Operands[0].Width); break;
                case Mnemonic.stlrh: RewriteStlr(PrimitiveType.Word16); break;
                case Mnemonic.stp: RewriteLoadStorePair(false); break;
                case Mnemonic.str: RewriteStr(null); break;
                case Mnemonic.strb: RewriteStr(PrimitiveType.Byte); break;
                case Mnemonic.strh: RewriteStr(PrimitiveType.Word16); break;
                case Mnemonic.stur: RewriteStr(null); break;
                case Mnemonic.sturb: RewriteStr(PrimitiveType.Byte); break;
                case Mnemonic.sturh: RewriteStr(PrimitiveType.Word16); break;
                case Mnemonic.stxr: RewriteStx(instr.Operands[1].Width); break;
                case Mnemonic.stxrb: RewriteStx(PrimitiveType.Byte); break;
                case Mnemonic.sub: RewriteBinary(m.ISub); break;
                case Mnemonic.subs: RewriteBinary(m.ISub, NZCV); break;
                case Mnemonic.svc: RewriteSvc(); break;
                case Mnemonic.sxtb: RewriteUSxt(Domain.SignedInt, 8); break;
                case Mnemonic.sxth: RewriteUSxt(Domain.SignedInt, 16); break;
                case Mnemonic.sxtl: RewriteSimdUnary("__sxtl_{0}", Domain.SignedInt); break;
                case Mnemonic.sxtw: RewriteUSxt(Domain.SignedInt, 32); break;
                case Mnemonic.tbnz: RewriteTb(m.Ne0); break;
                case Mnemonic.tbl: RewriteTbl(); break;
                case Mnemonic.tbz: RewriteTb(m.Eq0); break;
                case Mnemonic.test: RewriteTest(); break;
                case Mnemonic.uabd: RewriteUabd(); break;
                case Mnemonic.uaddw: RewriteUaddw(); break;
                case Mnemonic.ubfm: RewriteUSbfm("__ubfm"); break;
                case Mnemonic.ucvtf: RewriteIcvt(Domain.UnsignedInt); break;
                case Mnemonic.udiv: RewriteBinary(m.UDiv); break;
                case Mnemonic.umaddl: RewriteMaddl(PrimitiveType.UInt64, m.UMul); break;
                case Mnemonic.umlal: RewriteUmlal(); break;
                case Mnemonic.umull: RewriteMull(PrimitiveType.UInt32, PrimitiveType.UInt64, m.UMul); break;
                case Mnemonic.umulh: RewriteMulh(PrimitiveType.UInt64, m.UMul); break;
                case Mnemonic.ushr: RewriteMaybeSimdBinary(m.Shr, "__ushr_{0}", Domain.UnsignedInt); break;
                case Mnemonic.usubl: RewriteSimdBinary("__usubl_{0}", Domain.UnsignedInt); break;
                case Mnemonic.usubl2: RewriteSimdBinary("__usubl2_{0}", Domain.UnsignedInt); break;
                case Mnemonic.uxtb: RewriteUSxt(Domain.UnsignedInt, 8); break;
                case Mnemonic.uxth: RewriteUSxt(Domain.UnsignedInt, 16); break;
                case Mnemonic.uxtl: RewriteSimdUnary("__uxtl_{0}", Domain.UnsignedInt); break;
                case Mnemonic.uxtl2: RewriteSimdUnary("__uxtl2_{0}", Domain.UnsignedInt); break;
                case Mnemonic.uxtw: RewriteUSxt(Domain.UnsignedInt, 32); break;
                case Mnemonic.xtn: RewriteSimdUnary("__xtn_{0}", Domain.None); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                cluster.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void NotImplementedYet()
        {
            uint wInstr;
            wInstr = rdr.PeekLeUInt32(-4);
            host.Error(instr.Address, "Rewriting A64 mnemonic '{0}' ({1:X4}) is not supported yet.", instr.Mnemonic, wInstr);
            EmitUnitTest();
            m.Invalid();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("AArch64Rw", this.instr, instr.Mnemonic.ToString(), rdr, "");
        }

        //$TODO: prefer this RewriteOp
        private Expression RewriteOp(int iop, bool maybe0 = false) => RewriteOp(instr.Operands[iop], maybe0);

        private Expression RewriteOp(MachineOperand op, bool maybe0 = false)
        {
            switch (op)
            {
            case RegisterOperand regOp:
                if (maybe0)
                {
                    if (regOp.Register == Registers.GpRegs32[31])
                        return m.Word32(0);
                    if (regOp.Register == Registers.GpRegs64[31])
                        return m.Word64(0);
                }
                return binder.EnsureRegister(regOp.Register);
            case ImmediateOperand immOp:
                return immOp.Value;
            case AddressOperand addrOp:
                return addrOp.Address;
            case VectorRegisterOperand vectorOp:
                Identifier vreg;
                if (vectorOp.Width.BitSize == 64)
                {
                    vreg = binder.EnsureRegister(Registers.SimdRegs64[vectorOp.VectorRegister.Number - 32]);
                }
                else
                {
                    vreg = binder.EnsureRegister(Registers.SimdRegs128[vectorOp.VectorRegister.Number - 32]);
                }
                if (vectorOp.Index >= 0)
                {
                    var eType = PrimitiveType.CreateWord(Bitsize(vectorOp.ElementType));
                    return m.ARef(eType, vreg, Constant.Int32(vectorOp.Index));
                }
                else
                {
                    return vreg;
                }
            case MemoryOperand mem:
                var ea = binder.EnsureRegister(mem.Base!);
                return m.Mem(mem.Width, ea);
            }
            throw new NotImplementedException($"Rewriting {op.GetType().Name} not implemented yet.");
        }

        private Expression MaybeZeroRegister(RegisterStorage reg, PrimitiveType dt)
        {
            if (reg == Registers.GpRegs32[31] ||
                reg == Registers.GpRegs64[31])
            {
                return Constant.Zero(dt);
            }
            else
            {
                return binder.EnsureRegister(reg);
            }
        }
        private Identifier NZCV()
        {
            return binder.EnsureFlagGroup(Registers.NZCV);
        }

        private void NZCV(Expression test)
        {
            var nzcv = NZCV();
            m.Assign(nzcv, test);
        }

        private void NZ00(Expression test)
        {
            var nz = binder.EnsureFlagGroup(Registers.NZ);
            var c = binder.EnsureFlagGroup(Registers.C);
            var v = binder.EnsureFlagGroup(Registers.V);
            m.Assign(nz, test);
            m.Assign(c, Constant.False());
            m.Assign(v, Constant.False());
        }

        Identifier FlagGroup(FlagGroupStorage grf)
        {
            return binder.EnsureFlagGroup(grf);
        }

        protected Expression TestCond(ArmCondition cond)
        {
            switch (cond)
            {
            default:
                throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
            case ArmCondition.HS:
                return m.Test(ConditionCode.UGE, FlagGroup(Registers.C));
            case ArmCondition.LO:
                return m.Test(ConditionCode.ULT, FlagGroup(Registers.C));
            case ArmCondition.EQ:
                return m.Test(ConditionCode.EQ, FlagGroup(Registers.Z));
            case ArmCondition.GE:
                return m.Test(ConditionCode.GE, FlagGroup(Registers.NZV));
            case ArmCondition.GT:
                return m.Test(ConditionCode.GT, FlagGroup(Registers.NZV));
            case ArmCondition.HI:
                return m.Test(ConditionCode.UGT, FlagGroup(Registers.ZC));
            case ArmCondition.LE:
                return m.Test(ConditionCode.LE, FlagGroup(Registers.NZV));
            case ArmCondition.LS:
                return m.Test(ConditionCode.ULE, FlagGroup(Registers.ZC));
            case ArmCondition.LT:
                return m.Test(ConditionCode.LT, FlagGroup(Registers.NV));
            case ArmCondition.MI:
                return m.Test(ConditionCode.LT, FlagGroup(Registers.N));
            case ArmCondition.PL:
                return m.Test(ConditionCode.GE, FlagGroup(Registers.N));
            case ArmCondition.NE:
                return m.Test(ConditionCode.NE, FlagGroup(Registers.Z));
            case ArmCondition.VC:
                return m.Test(ConditionCode.NO, FlagGroup(Registers.V));
            case ArmCondition.VS:
                return m.Test(ConditionCode.OV, FlagGroup(Registers.V));
            }
        }

        protected ArmCondition Invert(ArmCondition cc)
        {
            switch (cc)
            {
            case ArmCondition.EQ: return ArmCondition.NE;
            case ArmCondition.NE: return ArmCondition.EQ;
            case ArmCondition.HS: return ArmCondition.LO;
            case ArmCondition.LO: return ArmCondition.HS;
            case ArmCondition.MI: return ArmCondition.PL;
            case ArmCondition.PL: return ArmCondition.MI;
            case ArmCondition.VS: return ArmCondition.VC;
            case ArmCondition.VC: return ArmCondition.VS;
            case ArmCondition.HI: return ArmCondition.LS;
            case ArmCondition.LS: return ArmCondition.HI;
            case ArmCondition.GE: return ArmCondition.LT;
            case ArmCondition.LT: return ArmCondition.GE;
            case ArmCondition.GT: return ArmCondition.LE;
            case ArmCondition.LE: return ArmCondition.GT;
            case ArmCondition.AL: return ArmCondition.Invalid;
            }
            return ArmCondition.Invalid;
        }

    }
}