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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
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
        private IEnumerator<AArch64Instruction> dasm;
        private AArch64Instruction instr;
        private RtlEmitter m;
        private InstrClass rtlc;

        public A64Rewriter(Arm64Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new AArch64Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var cluster = new List<RtlInstruction>();
                m = new RtlEmitter(cluster);
                rtlc = instr.InstructionClass;
                switch (instr.opcode)
                {
                default:
                    EmitUnitTest();
                    host.Warn(
                        instr.Address,
                        "AArch64 instruction {0} is not supported yet.",
                        instr);
                    goto case Opcode.Invalid;
                case Opcode.Invalid:
                    rtlc = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.add: RewriteMaybeSimdBinary(m.IAdd, "__add_{0}"); break;
                case Opcode.adds: RewriteBinary(m.IAdd, this.NZCV); break;
                case Opcode.addv: RewriteAddv(); break;
                case Opcode.adr: RewriteUnary(n => n); break;
                case Opcode.asrv: RewriteBinary(m.Sar); break;
                case Opcode.bic: RewriteBinary((a, b) => m.And(a, m.Comp(b))); break;
                case Opcode.adrp: RewriteAdrp(); break;
                case Opcode.and: RewriteBinary(m.And); break;
                case Opcode.ands: RewriteBinary(m.And, this.NZ00); break;
                case Opcode.asr: RewriteBinary(m.Sar); break;
                case Opcode.b: RewriteB(); break;
                case Opcode.bfm: RewriteBfm(); break;
                case Opcode.bl: RewriteBl(); break;
                case Opcode.blr: RewriteBlr(); break;
                case Opcode.br: RewriteBr(); break;
                case Opcode.cbnz: RewriteCb(m.Ne0); break;
                case Opcode.cbz: RewriteCb(m.Eq0); break;
                case Opcode.ccmp: RewriteCcmp(); break;
                case Opcode.clz: RewriteClz(); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.cmeq: RewriteCmeq(); break;
                case Opcode.csel: RewriteCsel(); break;
                case Opcode.csinc: RewriteCsinc(); break;
                case Opcode.csinv: RewriteCsinv(); break;
                case Opcode.csneg: RewriteCsneg(); break;
                case Opcode.dup: RewriteDup(); break;
                case Opcode.eor: RewriteBinary(m.Xor); break;
                case Opcode.fabs: RewriteFabs(); break;
                case Opcode.fadd: RewriteFadd(); break;
                case Opcode.fcmp: RewriteFcmp(); break;
                case Opcode.fcsel: RewriteFcsel(); break;
                case Opcode.fcvt: RewriteFcvt(); break;
                case Opcode.fcvtms: RewriteFcvtms(); break;
                case Opcode.fcvtps: RewriteFcvtps(); break;
                case Opcode.fcvtzs: RewriteFcvtzs(); break;
                case Opcode.fdiv: RewriteMaybeSimdBinary(m.FDiv, "__fdiv_{0}", Domain.Real); break;
                case Opcode.fmadd: RewriteIntrinsicFTernary("__fmaddf", "__fmadd"); break;
                case Opcode.fmsub: RewriteIntrinsicFTernary("__fmsubf", "__fmsub"); break;
                case Opcode.fmax: RewriteIntrinsicFBinary("fmaxf", "fmax"); break;
                case Opcode.fmin: RewriteIntrinsicFBinary("fminf", "fmin"); break;
                case Opcode.fmov: RewriteFmov(); break;
                case Opcode.fmul: RewriteFmul(); break;
                case Opcode.fneg: RewriteUnary(m.FNeg); break;
                case Opcode.fnmul: RewriteFnmul(); break;
                case Opcode.fsqrt: RewriteFsqrt(); break;
                case Opcode.fsub: RewriteMaybeSimdBinary(m.FSub, "__fsub_{0}", Domain.Real); break;
                case Opcode.ld1r: RewriteLdNr("__ld1r"); break;
                case Opcode.ld2: RewriteLdN("__ld2"); break;
                case Opcode.ld3: RewriteLdN("__ld3"); break;
                case Opcode.ld4: RewriteLdN("__ld4"); break;
                case Opcode.ldp: RewriteLoadStorePair(true); break;
                case Opcode.ldpsw: RewriteLoadStorePair(true, PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Opcode.ldr: RewriteLdr(null); break;
                case Opcode.ldrb: RewriteLdr(PrimitiveType.Byte); break;
                case Opcode.ldrh: RewriteLdr(PrimitiveType.Word16); break;
                case Opcode.ldrsb: RewriteLdr(PrimitiveType.SByte); break;
                case Opcode.ldrsh: RewriteLdr(PrimitiveType.Int16); break;
                case Opcode.ldrsw: RewriteLdr(PrimitiveType.Int32); break;
                case Opcode.lslv: RewriteBinary(m.Shl); break;
                case Opcode.lsrv: RewriteBinary(m.Shr); break;
                case Opcode.ldur: RewriteLdr(null); break;
                case Opcode.ldurb: RewriteLdr(PrimitiveType.Byte); break;
                case Opcode.ldurh: RewriteLdr(PrimitiveType.Word16); break;
                case Opcode.ldursb: RewriteLdr(PrimitiveType.SByte); break;
                case Opcode.ldursh: RewriteLdr(PrimitiveType.Int16); break;
                case Opcode.ldursw: RewriteLdr(PrimitiveType.Int32); break;
                case Opcode.lsl: RewriteBinary(m.Shl); break;
                case Opcode.lsr: RewriteBinary(m.Shr); break;
                case Opcode.madd: RewriteMaddSub(m.IAdd); break;
                case Opcode.mneg: RewriteBinary((a, b) => m.Neg(m.IMul(a, b))); break;
                case Opcode.msub: RewriteMaddSub(m.ISub); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.movi: RewriteMovi(); break;
                case Opcode.movk: RewriteMovk(); break;
                case Opcode.movn: RewriteMovn(); break;
                case Opcode.movz: RewriteMovz(); break;
                case Opcode.mul: RewriteMaybeSimdBinary(m.IMul, "__mul_{0}"); break;
                case Opcode.mvn: RewriteUnary(m.Comp); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.not: RewriteMaybeSimdUnary(m.Comp, "__not_{0}"); break;
                case Opcode.orr: RewriteBinary(m.Or);break;
                case Opcode.orn: RewriteBinary((a, b) => m.Or(a, m.Comp(b))); break;
                case Opcode.prfm: RewritePrfm(); break;
                case Opcode.ret: RewriteRet(); break;
                case Opcode.rev16: RewriteRev16(); break;
                case Opcode.ror: RewriteRor(); break;
                case Opcode.rorv: RewriteRor(); break;
                case Opcode.sbfiz: RewriteSbfiz(); break;
                case Opcode.sbfm: RewriteUSbfm("__sbfm"); break;
                case Opcode.scvtf: RewriteScvtf(); break;
                case Opcode.sdiv: RewriteBinary(m.SDiv); break;
                case Opcode.shrn: RewriteShrn(); break;
                case Opcode.smaddl: RewriteMaddl(PrimitiveType.Int64, m.SMul); break;
                case Opcode.smax: RewriteSmax(); break;
                case Opcode.smaxv: RewriteSmaxv(); break;
                case Opcode.smull: RewriteMull(PrimitiveType.Int64, m.SMul); break;
                case Opcode.st1: RewriteStN("__st1"); break;
                case Opcode.st2: RewriteStN("__st2"); break;
                case Opcode.st3: RewriteStN("__st3"); break;
                case Opcode.st4: RewriteStN("__st4"); break;
                case Opcode.stp: RewriteLoadStorePair(false); break;
                case Opcode.str: RewriteStr(null); break;
                case Opcode.strb: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.strh: RewriteStr(PrimitiveType.Word16); break;
                case Opcode.stur: RewriteStr(null); break;
                case Opcode.sturb: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.sturh: RewriteStr(PrimitiveType.Word16); break;
                case Opcode.sub: RewriteBinary(m.ISub); break;
                case Opcode.subs: RewriteBinary(m.ISub, NZCV); break;
                case Opcode.sxtb: RewriteUSxt(Domain.SignedInt, 8); break;
                case Opcode.sxth: RewriteUSxt(Domain.SignedInt, 16); break;
                case Opcode.sxtl: RewriteSimdUnary("__sxtl_{0}", Domain.SignedInt); break;
                case Opcode.sxtw: RewriteUSxt(Domain.SignedInt, 32); break;
                case Opcode.tbnz: RewriteTb(m.Ne0); break;
                case Opcode.tbz: RewriteTb(m.Eq0); break;
                case Opcode.test: RewriteTest(); break;
                case Opcode.uaddw: RewriteUaddw(); break;
                case Opcode.ubfm: RewriteUSbfm("__ubfm"); break;
                case Opcode.ucvtf: RewriteIcvt(Domain.UnsignedInt); break;
                case Opcode.udiv: RewriteBinary(m.UDiv); break;
                case Opcode.umaddl: RewriteMaddl(PrimitiveType.UInt64, m.UMul); break;
                case Opcode.umlal: RewriteUmlal(); break;
                case Opcode.umull: RewriteMull(PrimitiveType.UInt64, m.UMul); break;
                case Opcode.umulh: RewriteMulh(PrimitiveType.UInt64, m.UMul); break;
                case Opcode.uxtb: RewriteUSxt(Domain.UnsignedInt, 8); break;
                case Opcode.uxth: RewriteUSxt(Domain.UnsignedInt, 16); break;
                case Opcode.uxtl: RewriteSimdUnary("__uxtl_{0}", Domain.UnsignedInt); break;
                case Opcode.uxtw: RewriteUSxt(Domain.UnsignedInt, 32); break;
                case Opcode.xtn: RewriteSimdUnary("__xtn_{0}", Domain.None); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, cluster.ToArray())
                {
                    Class = rtlc,
                };
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
            host.Error(instr.Address, "Rewriting A64 opcode '{0}' ({1:X4}) is not supported yet.", instr.opcode, wInstr);
            EmitUnitTest();
            m.Invalid();
        }

        private static HashSet<Opcode> seen = new HashSet<Opcode>();

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.opcode))
                return;
            seen.Add(dasm.Current.opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var wInstr = r2.ReadUInt32();
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void A64Rw_" + dasm.Current.opcode + "()");
            Debug.WriteLine("        {");
            Debug.Write("            Given_Instruction(");
            Debug.Write($"0x{wInstr:X8}");
            Debug.WriteLine(");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine($"                \"0|L--|{dasm.Current.Address}({dasm.Current.Length}): 1 instructions\",");
            Debug.WriteLine( "                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }

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
                    vreg= binder.EnsureRegister(Registers.SimdRegs64[vectorOp.VectorRegister.Number - 32]);
                }
                else
                {
                    vreg= binder.EnsureRegister(Registers.SimdRegs128[vectorOp.VectorRegister.Number - 32]);
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
            default:
                throw new NotImplementedException($"Rewriting {op.GetType().Name} not implemented yet.");
            }
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
            var nzcv = arch.GetFlagGroup((uint)(FlagM.NF | FlagM.ZF | FlagM.CF | FlagM.VF));
            return binder.EnsureFlagGroup(nzcv);
        }

        private void NZCV(Expression test)
        {
            var nzcv = NZCV();
            m.Assign(nzcv, test);
        }

        private void NZ00(Expression test)
        {
            var nz = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)(FlagM.NF | FlagM.ZF)));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            var v = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.VF));
            m.Assign(nz, test);
            m.Assign(c, Constant.False());
            m.Assign(v, Constant.False());
        }

        Identifier FlagGroup(FlagM bits, string name, PrimitiveType type)
        {
            var grf = arch.GetFlagGroup((uint)bits);
            return binder.EnsureFlagGroup(grf);
        }

        protected Expression TestCond(ArmCondition cond)
        {
            switch (cond)
            {
            //default:
            //	throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
            case ArmCondition.HS:
                return m.Test(ConditionCode.UGE, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case ArmCondition.LO:
                return m.Test(ConditionCode.ULT, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case ArmCondition.EQ:
                return m.Test(ConditionCode.EQ, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case ArmCondition.GE:
                return m.Test(ConditionCode.GE, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.GT:
                return m.Test(ConditionCode.GT, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.HI:
                return m.Test(ConditionCode.UGT, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case ArmCondition.LE:
                return m.Test(ConditionCode.LE, FlagGroup(FlagM.ZF | FlagM.CF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.LS:
                return m.Test(ConditionCode.ULE, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case ArmCondition.LT:
                return m.Test(ConditionCode.LT, FlagGroup(FlagM.NF | FlagM.VF, "NV", PrimitiveType.Byte));
            case ArmCondition.MI:
                return m.Test(ConditionCode.LT, FlagGroup(FlagM.NF, "N", PrimitiveType.Byte));
            case ArmCondition.PL:
                return m.Test(ConditionCode.GE, FlagGroup(FlagM.NF, "N", PrimitiveType.Byte));
            case ArmCondition.NE:
                return m.Test(ConditionCode.NE, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case ArmCondition.VC:
                return m.Test(ConditionCode.NO, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            case ArmCondition.VS:
                return m.Test(ConditionCode.OV, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            }
            return null;
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