#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Collections;
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

namespace Reko.Arch.Sparc
{
    public partial class SparcRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly SparcArchitecture arch;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly LookaheadEnumerator<SparcInstruction> dasm;
        private readonly List<RtlInstruction> rtlInstructions;
        private readonly RtlEmitter m;
        private SparcInstruction instrCur;
        private InstrClass iclass;

        public SparcRewriter(SparcArchitecture arch, EndianImageReader rdr, SparcProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = new LookaheadEnumerator<SparcInstruction>(CreateDisassemblyStream(rdr));
            this.rtlInstructions = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtlInstructions);
            this.instrCur = default!;
        }

        public SparcRewriter(SparcArchitecture arch, IEnumerator<SparcInstruction> instrs, SparcProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.dasm = new LookaheadEnumerator<SparcInstruction>(instrs);
            this.rtlInstructions = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtlInstructions);
            this.instrCur = default!;
            this.rdr = default!;
        }

        private IEnumerable<SparcInstruction> CreateDisassemblyStream(EndianImageReader rdr)
        {
            return new SparcDisassembler(arch, arch.Decoder, rdr);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            var regs = arch.Registers;
            while (dasm.MoveNext())
            {
                instrCur = dasm.Current;
                var addr = instrCur.Address;
                iclass = instrCur.InstructionClass;
                switch (instrCur.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    host.Warn(
                        instrCur.Address,
                        "SPARC instruction '{0}' is not supported yet.",
                        instrCur.Mnemonic);
                    goto case Mnemonic.illegal;
                case Mnemonic.illegal: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.add: RewriteAlu(m.IAdd, false); break;
                case Mnemonic.addcc: RewriteAluCc(m.IAdd, false); break;
                case Mnemonic.addx: RewriteAddxSubx(m.IAdd, false); break;
                case Mnemonic.addxcc: RewriteAddxSubx(m.IAdd, true); break;
                case Mnemonic.and: RewriteAlu(m.And, false); break;
                case Mnemonic.andcc: RewriteAluCc(m.And, false); break;
                case Mnemonic.andn: RewriteAlu(m.And, true); break;
                case Mnemonic.andncc: RewriteLogicalCc(m.And, true); break;
                case Mnemonic.ba: RewriteBranch(Constant.True()); break;
                case Mnemonic.bn: RewriteBranch(Constant.False()); break;
                case Mnemonic.bne: RewriteBranch(m.Test(ConditionCode.NE, Grf(FlagM.ZF))); break;
                case Mnemonic.be: RewriteBranch(m.Test(ConditionCode.EQ, Grf(FlagM.ZF))); break;
                case Mnemonic.bg: RewriteBranch(m.Test(ConditionCode.GT, Grf(FlagM.ZF | FlagM.NF | FlagM.VF))); break;
                case Mnemonic.bge: RewriteBranch(m.Test(ConditionCode.GE, Grf(FlagM.NF | FlagM.VF))); break;
                case Mnemonic.bgu: RewriteBranch(m.Test(ConditionCode.UGE, Grf(FlagM.CF | FlagM.ZF))); break;
                case Mnemonic.bl: RewriteBranch(m.Test(ConditionCode.LT, Grf(FlagM.ZF | FlagM.NF | FlagM.VF))); break;
                case Mnemonic.ble: RewriteBranch(m.Test(ConditionCode.LE, Grf(FlagM.ZF | FlagM.NF | FlagM.VF))); break;
                case Mnemonic.bleu: RewriteBranch(m.Test(ConditionCode.ULE, Grf(FlagM.CF | FlagM.ZF))); break;
                case Mnemonic.bcc: RewriteBranch(m.Test(ConditionCode.UGE, Grf(FlagM.CF))); break;
                case Mnemonic.bcs: RewriteBranch(m.Test(ConditionCode.ULT, Grf(FlagM.CF))); break;
                case Mnemonic.bneg: RewriteBranch(m.Test(ConditionCode.LT, Grf(FlagM.NF))); break;
                case Mnemonic.bpos: RewriteBranch(m.Test(ConditionCode.GE, Grf(FlagM.NF))); break;
                case Mnemonic.brgz: RewriteBranchReg(m.Gt0); break;
                case Mnemonic.brgez: RewriteBranchReg(m.Ge0); break;
                case Mnemonic.brlz: RewriteBranchReg(m.Lt0); break;
                case Mnemonic.brlez: RewriteBranchReg(m.Le0); break;
                case Mnemonic.brnz: RewriteBranchReg(m.Ne0); break;
                case Mnemonic.brz: RewriteBranchReg(m.Eq0); break;
                //                    Z
                //case Mnemonic.bgu  not (C or Z)
                //case Mnemonic.bleu (C or Z)
                //case Mnemonic.bcc  not C
                //case Mnemonic.bcs   C
                //case Mnemonic.bpos not N
                //case Mnemonic.bneg N
                case Mnemonic.bvc: RewriteBranch(m.Test(ConditionCode.NO, Grf(FlagM.VF))); break;
                case Mnemonic.bvs: RewriteBranch(m.Test(ConditionCode.OV, Grf(FlagM.VF))); break;

                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.fabss: RewriteFabss(); break;
                case Mnemonic.fadds: RewriteFadds(); break;
                case Mnemonic.fbne: RewriteBranch(m.Test(ConditionCode.NE, Grf(FlagM.LF | FlagM.GF))); break;
                case Mnemonic.fba: RewriteBranch(Constant.True()); break;
                case Mnemonic.fbn: RewriteBranch(Constant.False()); break;

                case Mnemonic.fbu   : RewriteBranch(m.Test(ConditionCode.NE, Grf(FlagM.UF))); break;
                case Mnemonic.fbg   : RewriteBranch(m.Test(ConditionCode.GT, Grf(FlagM.GF))); break;
                case Mnemonic.fbug  : RewriteBranch(m.Test(ConditionCode.GT, Grf(FlagM.GF | FlagM.UF))); break;
                //case Mnemonic.fbug  : on Unordered or Greater G or U
                //case Mnemonic.fbl   : on Less L
                case Mnemonic.fbul: RewriteBranch(m.Test(ConditionCode.LT, Grf(FlagM.GF|FlagM.UF))); break;
                //case Mnemonic.fbul  : on Unordered or Less L or U
                //case Mnemonic.fblg  : on Less or Greater L or G
                //case Mnemonic.fbne  : on Not Equal L or G or U
                case Mnemonic.fbe  : RewriteBranch(m.Test(ConditionCode.EQ, Grf(FlagM.EF))); break;
                case Mnemonic.fbue : RewriteBranch(m.Test(ConditionCode.EQ, Grf(FlagM.EF | FlagM.UF))); break;
                case Mnemonic.fbuge: RewriteBranch(m.Test(ConditionCode.GE, Grf(FlagM.EF | FlagM.GF | FlagM.UF))); break;

                case Mnemonic.fble  : RewriteBranch(m.Test(ConditionCode.LE, Grf(FlagM.EF | FlagM.LF))); break;
                //case Mnemonic.fbule : on Unordered or Less or Equal E or L or U
                case Mnemonic.fbule: RewriteBranch(m.Test(ConditionCode.LE, Grf(FlagM.EF | FlagM.LF | FlagM.UF))); break;
                case Mnemonic.fbge: RewriteBranch(m.Test(ConditionCode.GE, Grf(FlagM.EF | FlagM.GF))); break;
                case Mnemonic.fbo: RewriteBranch(m.Test(ConditionCode.NOT_NAN, Grf(FlagM.UF))); break;

                case Mnemonic.fcmpes: RewriteFcmpes(); break;
                case Mnemonic.fcmped: RewriteFcmped(); break;
                case Mnemonic.fcmpd: RewriteFcmpd(); break;
                case Mnemonic.fcmpq: RewriteFcmpq(); break;
                case Mnemonic.fcmps: RewriteFcmps(); break;
                case Mnemonic.fdivd: RewriteFdivs(); break;
                case Mnemonic.fdivs: RewriteFdivd(); break;
                case Mnemonic.fdtos: RewriteFdtos(); break;
                case Mnemonic.fitod: RewriteFitod(); break;
                case Mnemonic.fitoq: RewriteFitoq(); break;
                case Mnemonic.fitos: RewriteFitos(); break;
                case Mnemonic.fmovs: RewriteFmovs(); break;
                case Mnemonic.fmuld: RewriteFmuld(); break;
                case Mnemonic.fmuls: RewriteFmuls(); break;
                case Mnemonic.fnegs: RewriteFmovs(); break;
                case Mnemonic.fstod: RewriteFstod(); break;
                case Mnemonic.fsubs: RewriteFsubs(); break;
                case Mnemonic.jmpl: RewriteJmpl(); break;
                case Mnemonic.ld: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.lddf: RewriteLoad(PrimitiveType.Real64); break;
                case Mnemonic.ldf: RewriteLoad(PrimitiveType.Real32); break;
                case Mnemonic.ldd: RewriteLdd(); break;
                case Mnemonic.ldsb: RewriteLoad(PrimitiveType.SByte); break;
                case Mnemonic.ldsh: RewriteLoad(PrimitiveType.Int16); break;
                case Mnemonic.ldsw: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.ldstub: RewriteLdstub(); break;
                case Mnemonic.ldub: RewriteLoad(PrimitiveType.Byte); break;
                case Mnemonic.lduba: RewriteLoada(PrimitiveType.Byte); break;
                case Mnemonic.lduh: RewriteLoad(PrimitiveType.Word16); break;
                case Mnemonic.lduw: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.ldx: RewriteLoad(PrimitiveType.Word64); break;
                case Mnemonic.ldfsr: RewriteLoad(PrimitiveType.Word32); break;

                case Mnemonic.mova: RewriteMovcc(ConditionCode.ALWAYS, regs.Z, regs.xZ); break;
                case Mnemonic.movcc: RewriteMovcc(ConditionCode.UGE, regs.C, regs.xC); break;
                case Mnemonic.movcs: RewriteMovcc(ConditionCode.ULT, regs.C, regs.xC); break;
                case Mnemonic.move: RewriteMovcc(ConditionCode.EQ, regs.Z, regs.xZ); break;
                case Mnemonic.movfa: RewriteMovfcc(); break;
                case Mnemonic.movfe: RewriteMovfcc(); break;
                case Mnemonic.movfg: RewriteMovfcc(); break;
                case Mnemonic.movfge: RewriteMovfcc(); break;
                case Mnemonic.movfl: RewriteMovfcc(); break;
                case Mnemonic.movfle: RewriteMovfcc(); break;
                case Mnemonic.movflg: RewriteMovfcc(); break;
                case Mnemonic.movfn: RewriteMovfcc(); break;
                case Mnemonic.movfne: RewriteMovfcc(); break;
                case Mnemonic.movfo: RewriteMovfcc(); break;
                case Mnemonic.movfu: RewriteMovfcc(); break;
                case Mnemonic.movfue: RewriteMovfcc(); break;
                case Mnemonic.movfug: RewriteMovfcc(); break;
                case Mnemonic.movfuge: RewriteMovfcc(); break;
                case Mnemonic.movful: RewriteMovfcc(); break;
                case Mnemonic.movfule: RewriteMovfcc(); break;
                case Mnemonic.movg: RewriteMovcc(ConditionCode.GT, regs.NZV, regs.xNZV); break;
                case Mnemonic.movge: RewriteMovcc(ConditionCode.GE, regs.NV, regs.xNV); break;
                case Mnemonic.movgu: RewriteMovcc(ConditionCode.UGT, regs.ZC, regs.xZC); break;
                case Mnemonic.movl: RewriteMovcc(ConditionCode.LT, regs.NV, regs.xNV); break;
                case Mnemonic.movle: RewriteMovcc(ConditionCode.LE, regs.NZV, regs.xNZV); break;
                case Mnemonic.movleu: RewriteMovcc(ConditionCode.ULE, regs.ZC, regs.xZC); break;
                case Mnemonic.movn: RewriteMovcc(ConditionCode.NEVER, regs.Z, regs.xZ); break;
                case Mnemonic.movne: RewriteMovcc(ConditionCode.NE, regs.Z, regs.xZ); break;
                case Mnemonic.movneg: RewriteMovcc(ConditionCode.LT, regs.N, regs.xN); break;
                case Mnemonic.movpos: RewriteMovcc(ConditionCode.GE, regs.N, regs.xN); break;

                case Mnemonic.mulx: RewriteAlu(m.IMul, false); break;
                case Mnemonic.mulscc: RewriteMulscc(); break;
                case Mnemonic.or: RewriteAlu(m.Or, false); break;
                case Mnemonic.orcc: RewriteAluCc(m.Or, false); break;
                case Mnemonic.orn: RewriteAlu(m.Or, true); break;
                case Mnemonic.orncc: RewriteAlu(m.Or, true); break;
                case Mnemonic.restore: RewriteRestore(); break;
                case Mnemonic.rett: RewriteRett(); break;
                case Mnemonic.@return: RewriteReturn(); break;
                case Mnemonic.save: RewriteSave(); break;
                case Mnemonic.sethi: RewriteSethi(); break;
                case Mnemonic.sdiv: RewriteAlu(m.SDiv, false); break;
                case Mnemonic.sdivcc: RewriteAlu(m.SDiv, false); break;
                case Mnemonic.sll: RewriteAlu(m.Shl, false); break;
                case Mnemonic.sllx: RewriteAlu(m.Shl, false); break;
                case Mnemonic.smul: RewriteAlu(m.SMul, false); break;
                case Mnemonic.smulcc: RewriteAluCc(m.SMul, false); break;
                case Mnemonic.sra: RewriteAlu(m.Sar, false); break;
                case Mnemonic.srax: RewriteAlu(m.Sar, false); break;
                case Mnemonic.srl: RewriteAlu(m.Shr, false); break;
                case Mnemonic.srlx: RewriteAlu(m.Shr, false); break;
                case Mnemonic.st: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.stb: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.std: RewriteStd(); break;
                case Mnemonic.stdf: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.stf: RewriteStore(PrimitiveType.Real32); break;
                case Mnemonic.sth: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.stw: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.stx: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.stfsr: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.sub: RewriteAlu(m.ISub, false); break;
                case Mnemonic.subcc: RewriteAluCc(m.ISub, false); break;
                case Mnemonic.subx: RewriteAddxSubx(m.ISub, false); break;
                case Mnemonic.subxcc: RewriteAddxSubx(m.ISub, true); break;
                case Mnemonic.swap: RewriteSwap(swap_intrinsic); break;
                case Mnemonic.swapa: RewriteSwap(swapa_intrinsic); break;
                case Mnemonic.ta: RewriteTrap(Constant.True()); break;
                case Mnemonic.tn: RewriteTrap(Constant.False()); break;
                case Mnemonic.tne: RewriteTrap(m.Test(ConditionCode.NE, Grf(FlagM.ZF))); break;
                case Mnemonic.te: RewriteTrap(m.Test(ConditionCode.EQ, Grf(FlagM.ZF))); break;
                case Mnemonic.taddcc: RewriteBinaryCc(taddcc_intrinsic); break;
                case Mnemonic.udiv: RewriteAlu(m.UDiv, false); break;
                case Mnemonic.udivcc: RewriteAluCc(m.UDiv, false); break;
                case Mnemonic.umul: RewriteAlu(m.UMul, false); break;
                case Mnemonic.umulcc: RewriteAluCc(m.UMul, false); break;
                case Mnemonic.unimp: m.Invalid(); break;
                case Mnemonic.xor: RewriteAlu(m.Xor, false); break;
                case Mnemonic.xorcc: RewriteAlu(m.Xor, true); break;
                case Mnemonic.xnor: RewriteAlu(XNor, false); break;
                case Mnemonic.xnorcc: RewriteAlu(XNor, true); break;
                }
                yield return m.MakeCluster(addr, 4, iclass);
                this.rtlInstructions.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("SparcRw", instrCur, instrCur.Mnemonic.ToString(), rdr, "");
        }

        private void EmitCc(FlagGroupStorage grf32, FlagGroupStorage grf64, Expression src)
        {
            if (arch.PointerType.BitSize == 64)
            {
                m.Assign(binder.EnsureFlagGroup(grf64), m.Cond(src));
                var tmp = binder.CreateTemporary(PrimitiveType.Word32);
                m.Assign(tmp, m.Slice(src, tmp.DataType));
                m.Assign(binder.EnsureFlagGroup(grf32), m.Cond(tmp));
            }
            else
            {
                m.Assign(binder.EnsureFlagGroup(grf32), m.Cond(src));
            }
        }

        private void EmitCc(FlagGroupStorage grf, int n)
        {
            m.Assign(binder.EnsureFlagGroup(grf), n);
        }

        private Expression MaybeSlice(Expression e, PrimitiveType dt)
        {
            if (e.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(e, dt, 0));
                return tmp;
            }
            else
            {
                return e;
            }
        }

        private Expression RewriteOp(MachineOperand op)
        {
            return RewriteOp(op, false);
        }

        private Expression RewriteOp(int iOp, bool g0_becomes_null = false) => RewriteOp(instrCur.Operands[iOp], g0_becomes_null);

        private Expression RewriteOp(MachineOperand op, bool g0_becomes_null)
        {
            if (op is RegisterStorage reg)
            {
                if (reg == arch.Registers.g0)
                {
                    //$REVIEW: handle null
                    if (g0_becomes_null)
                        return null!;
                    else
                        return Constant.Zero(PrimitiveType.Word32);
                }
                else
                    return binder.EnsureRegister(reg);
            }
            if (op is ImmediateOperand imm)
                return imm.Value;
            throw new NotImplementedException(string.Format("Unsupported operand {0} ({1})", op, op.GetType().Name));
        }

        private Expression RewriteRegister(int iop)
        {
            var op = this.instrCur.Operands[iop];
            return binder.EnsureRegister((RegisterStorage)op);
        }

        private Expression RewriteMemOp(MachineOperand op, DataType size)
        {
            Expression? baseReg;
            Expression? offset;
            if (op is MemoryOperand mem)
            {
                baseReg = mem.Base == arch.Registers.g0 ? null : binder.EnsureRegister(mem.Base);
                offset = mem.Offset.IsIntegerZero ? null : mem.Offset;
            }
            else
            {
                if (op is IndexedMemoryOperand i)
                {
                    baseReg = i.Base == arch.Registers.g0 ? null : binder.EnsureRegister(i.Base);
                    offset = i.Index == arch.Registers.g0 ? null : binder.EnsureRegister(i.Index);
                }
                else
                    throw new NotImplementedException(string.Format("Unknown memory operand {0} ({1})", op, op.GetType().Name));
            }
            return m.Mem(size, SimplifySum(baseReg, offset));
        }

        private Expression SimplifySum(Expression? srcLeft, Expression? srcRight)
        {
            if (srcLeft == null && srcRight == null)
                return Constant.Zero(PrimitiveType.Ptr32);
            else if (srcLeft == null)
                return srcRight!;
            else if (srcRight == null)
                return srcLeft;
            else
                return m.IAdd(srcLeft, srcRight);
        }

        private Expression XNor(Expression left, Expression right)
        {
            return m.Comp(m.Xor(left, right));
        }

        private static readonly IntrinsicProcedure loada_intrinsic = new IntrinsicBuilder("__load_alternate", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");

        private static readonly IntrinsicProcedure swap_intrinsic = new IntrinsicBuilder("__swap", true)
             .GenericTypes("T")
             .Param("T")
             .PtrParam("T")
             .Returns("T");
        private static readonly IntrinsicProcedure swapa_intrinsic = new IntrinsicBuilder("__swap_alternate", true)
             .GenericTypes("T")
             .Param("T")
             .PtrParam("T")
             .Returns("T");
    }
}
