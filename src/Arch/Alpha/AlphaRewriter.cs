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
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Alpha
{
    public partial class AlphaRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly AlphaArchitecture arch;
        private readonly IStorageBinder binder;
        private readonly EndianImageReader rdr;
        private readonly IRewriterHost host;
        private readonly IEnumerator<AlphaInstruction> dasm;
        private AlphaInstruction instr;
        private InstrClass iclass;
        private RtlEmitter m;

        public AlphaRewriter(AlphaArchitecture arch, EndianImageReader rdr, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.binder = binder;
            this.host = host;
            this.dasm = new AlphaDisassembler(this.arch, rdr).GetEnumerator();
            this.instr = null!;
            this.m = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    Invalid();
                    host.Warn(
                       instr.Address,
                       string.Format(
                           "Alpha AXP instruction '{0}' not supported yet.",
                           instr.Mnemonic));

                    break;
                case Mnemonic.invalid:
                    Invalid();
                    break;
                case Mnemonic.addf_c: RewriteFpuOp(m.FAdd); break;
                case Mnemonic.adds: RewriteFpuOp(m.FAdd); break;
                case Mnemonic.adds_c: RewriteFpuOp(m.FAdd); break;
                case Mnemonic.addl: RewriteBin(addl); break;
                case Mnemonic.addl_v: RewriteBinOv(addl); break;
                case Mnemonic.addt: RewriteFpuOp(m.FAdd); break;
                case Mnemonic.addq: RewriteBin(addq); break;
                case Mnemonic.addq_v: RewriteBinOv(addq); break;
                case Mnemonic.and: RewriteBin(and); break;
                case Mnemonic.beq: RewriteBranch(m.Eq0); break;
                case Mnemonic.bge: RewriteBranch(m.Ge0); break;
                case Mnemonic.bgt: RewriteBranch(m.Gt0); break;
                case Mnemonic.bic: RewriteBin(bic); break;
                case Mnemonic.bis: RewriteBin(bis); break;
                case Mnemonic.blbc: RewriteBranch(lbc); break;
                case Mnemonic.blbs: RewriteBranch(lbs); break;
                case Mnemonic.ble: RewriteBranch(m.Le0); break;
                case Mnemonic.blt: RewriteBranch(m.Lt0); break;
                case Mnemonic.bne: RewriteBranch(m.Ne0); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.bsr: RewriteBr(); break;
                case Mnemonic.cmovlt: RewriteCmov(m.Lt0); break;
                case Mnemonic.cmovne: RewriteCmov(m.Ne0); break;
                case Mnemonic.cmovge: RewriteCmov(m.Ge0); break;
                case Mnemonic.cmovlbc: RewriteCmov(lbc); break;
                case Mnemonic.cmovlbs: RewriteCmov(lbs); break;
                case Mnemonic.cmpbge: RewriteInstrinsic(cmpbge_intrinsic); break;
                case Mnemonic.cmpeq: RewriteCmp(m.Eq); break;
                case Mnemonic.cmple: RewriteCmp(m.Le); break;
                case Mnemonic.cmplt: RewriteCmp(m.Lt); break;
                case Mnemonic.cmpteq: RewriteCmpt(m.Eq); break;
                case Mnemonic.cmptle: RewriteCmpt(m.Le); break;
                case Mnemonic.cmptlt: RewriteCmpt(m.Lt); break;
                case Mnemonic.cmpule: RewriteCmp(m.Ule); break;
                case Mnemonic.cmpult: RewriteCmp(m.Ult); break;
                case Mnemonic.cpys: RewriteCpys(cpys_intrinsic); break;
                case Mnemonic.cpyse: RewriteCpys(cpyse_intrinsic); break;
                case Mnemonic.cpysn: RewriteCpys(cpysn_intrinsic); break;
                case Mnemonic.cvtlq: RewriteCvt(PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.cvtql: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Int32); break;
                case Mnemonic.cvtqs: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Real32); break;
                case Mnemonic.cvtqt: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Real64); break;
                case Mnemonic.cvttq_c: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Int64); break;
                case Mnemonic.cvtts: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.divs: RewriteFpuOp(m.FDiv); break;
                case Mnemonic.divt: RewriteFpuOp(m.FDiv); break;
                case Mnemonic.extbl: RewriteInstrinsic(extbl_intrinsic); break;
                case Mnemonic.extlh: RewriteInstrinsic(extlh_intrinsic); break;
                case Mnemonic.extll: RewriteInstrinsic(extll_intrinsic); break;
                case Mnemonic.extqh: RewriteInstrinsic(extqh_intrinsic); break;
                case Mnemonic.extql: RewriteInstrinsic(extql_intrinsic); break;
                case Mnemonic.extwh: RewriteInstrinsic(extwh_intrinsic); break;
                case Mnemonic.extwl: RewriteInstrinsic(extwl_intrinsic); break;
                case Mnemonic.fbeq: RewriteFBranch(Operator.Feq); break;
                case Mnemonic.fbge: RewriteFBranch(Operator.Fge); break;
                case Mnemonic.fbgt: RewriteFBranch(Operator.Fgt); break;
                case Mnemonic.fble: RewriteFBranch(Operator.Fle); break;
                case Mnemonic.fblt: RewriteFBranch(Operator.Flt); break;
                case Mnemonic.fbne: RewriteFBranch(Operator.Fne); break;
                case Mnemonic.fcmoveq: RewriteFCmov(Operator.Feq); break;
                case Mnemonic.fcmovle: RewriteFCmov(Operator.Fle); break;
                case Mnemonic.fcmovlt: RewriteFCmov(Operator.Flt); break;
                case Mnemonic.fcmovne: RewriteFCmov(Operator.Fne); break;
                case Mnemonic.halt: RewriteHalt(); break;
                case Mnemonic.implver: RewriteImplver(); break;
                case Mnemonic.insbl: RewriteInstrinsic(insbl_intrinsic); break;
                case Mnemonic.inslh: RewriteInstrinsic(inslh_intrinsic); break;
                case Mnemonic.insll: RewriteInstrinsic(insll_intrinsic); break;
                case Mnemonic.insqh: RewriteInstrinsic(insqh_intrinsic); break;
                case Mnemonic.insql: RewriteInstrinsic(insql_intrinsic); break;
                case Mnemonic.inswl: RewriteInstrinsic(inswl_intrinsic); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsr: RewriteJmp(); break;
                case Mnemonic.jsr_coroutine: RewriteJmp(); break;
                case Mnemonic.lda: RewriteLda(0); break;
                case Mnemonic.ldah: RewriteLda(16); break;
                case Mnemonic.ldbu: RewriteLd(PrimitiveType.Byte, PrimitiveType.Word64); break;
                case Mnemonic.ldf: RewriteLd(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.ldg: RewriteLd(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.ldl: RewriteLd(PrimitiveType.Int32, PrimitiveType.Word64); break;
                case Mnemonic.ldl_l: RewriteLoadInstrinsic(load_locked_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.ldq_l: RewriteLoadInstrinsic(load_locked_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.ldq: RewriteLd(PrimitiveType.Word64, PrimitiveType.Word64); break;
                case Mnemonic.ldq_u: RewriteLd(PrimitiveType.Word64, PrimitiveType.Word64); break;
                case Mnemonic.lds: RewriteLd(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.ldt: RewriteLd(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.ldwu: RewriteLd(PrimitiveType.UInt16, PrimitiveType.Word64); break;
                case Mnemonic.mskbl: RewriteInstrinsic(mskbl_intrinsic); break;
                case Mnemonic.msklh: RewriteInstrinsic(msklh_intrinsic); break;
                case Mnemonic.mskll: RewriteInstrinsic(mskll_intrinsic); break;
                case Mnemonic.mskqh: RewriteInstrinsic(mskqh_intrinsic); break;
                case Mnemonic.mskql: RewriteInstrinsic(mskql_intrinsic); break;
                case Mnemonic.mskwl: RewriteInstrinsic(mskwl_intrinsic); break;
                case Mnemonic.mull: RewriteBin(mull); break;
                case Mnemonic.mulq: RewriteBin(mulq); break;
                case Mnemonic.muls: RewriteFpuOp(m.FMul); break;
                case Mnemonic.mult: RewriteFpuOp(m.FMul); break;
                case Mnemonic.mult_c: RewriteFpuOp(m.FMul); break;
                case Mnemonic.ornot: RewriteBin(ornot); break;
                case Mnemonic.ret: RewriteJmp(); break;
                case Mnemonic.s4addl: RewriteBin(s4addl); break;
                case Mnemonic.s4addq: RewriteBin(s4addq); break;
                case Mnemonic.s8addl: RewriteBin(s8addl); break;
                case Mnemonic.s8addq: RewriteBin(s8addq); break;
                case Mnemonic.s4subl: RewriteBin(s4subl); break;
                case Mnemonic.s4subq: RewriteBin(s4subq); break;
                case Mnemonic.s8subl: RewriteBin(s8subl); break;
                case Mnemonic.s8subq: RewriteBin(s8subq); break;
                case Mnemonic.sll: RewriteBin(sll); break;
                case Mnemonic.sra: RewriteBin(sra); break;
                case Mnemonic.srl: RewriteBin(srl); break;
                case Mnemonic.stb: RewriteSt(PrimitiveType.Byte); break;
                case Mnemonic.stf: RewriteSt(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.stl: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stl_c: RewriteStoreInstrinsic(store_conditional_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.stq_c: RewriteStoreInstrinsic(store_conditional_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.stq_u: RewriteStoreInstrinsic(store_unaligned_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.stg: RewriteSt(PrimitiveType.Real64); break;
                case Mnemonic.stw: RewriteSt(PrimitiveType.Word16); break;
                case Mnemonic.sts: RewriteSt(PrimitiveType.Real32); break;
                case Mnemonic.stt: RewriteSt(PrimitiveType.Real64); break;
                case Mnemonic.stq: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.subf_s: RewriteFpuOp(m.FSub); break;
                case Mnemonic.subf_uc: RewriteFpuOp(m.FSub); break;
                case Mnemonic.subl: RewriteBin(subl); break;
                case Mnemonic.subl_v: RewriteBinOv(subl); break;
                case Mnemonic.subq: RewriteBin(subq); break;
                case Mnemonic.subq_v: RewriteBinOv(subq); break;
                case Mnemonic.subs: RewriteFpuOp(m.FSub); break;
                case Mnemonic.subt: RewriteFpuOp(m.FSub); break;
                case Mnemonic.trapb: RewriteTrapb(); break;
                case Mnemonic.umulh: RewriteBin(umulh); break;
                case Mnemonic.xor: RewriteBin(xor); break;
                case Mnemonic.zap: RewriteInstrinsic(zap_intrinsic); break;
                case Mnemonic.zapnot: RewriteInstrinsic(zapnot_intrinsic); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        private void EmitUnitTest()
        {
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("AlphaRw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, "");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Invalid()
        {
            m.Invalid();
            iclass = InstrClass.Invalid;
        }

        private Expression Rewrite(MachineOperand op, bool highWord = false)
        {
            switch (op)
            {
            case RegisterStorage rop:
            {
                if (rop.Number == 31)
                    return Constant.Word64(0);
                else if (rop.Number == 63)
                    return Constant.Real64(0.0);
                else
                    return this.binder.EnsureRegister(rop);
            }
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mop:
                int offset = highWord ? (int)mop.Offset << 16 : mop.Offset;
                Expression ea;
                if (mop.Base.Number == ZeroRegister)
                {
                    var dt = PrimitiveType.Create(Domain.Integer, arch.PointerType.BitSize);
                    ea = Constant.Create(dt, offset); //$TODO should be platform size.
                }
                else 
                {
                    ea = binder.EnsureRegister(mop.Base);
                    if (offset > 0)
                    {
                        ea = m.IAdd(ea, offset);
                    }
                    else if (offset < 0)
                    {
                        ea = m.ISub(ea, -offset);
                    }
                }
                return m.Mem(mop.DataType, ea);
            }
            throw new NotImplementedException(string.Format("{0} ({1})", op, op.GetType().Name));
        }

        static AlphaRewriter()
        {
            implver_intrinsic = new IntrinsicBuilder("__implver", true)
                .GenericTypes("T")
                .Returns("T");
            load_locked_intrinsic = new IntrinsicBuilder("__load_locked", true)
                .GenericTypes("TPtr", "TValue")
                .Param("TPtr")
                .Returns("TValue");
            ov_intrinsic = new IntrinsicBuilder("OV", false)
                .GenericTypes("T")
                .Param("T")
                .Returns(PrimitiveType.Bool);
            store_conditional_intrinsic = new IntrinsicBuilder("__store_conditional", true)
                .GenericTypes("TPtr", "TValue")
                .Param("TPtr")
                .Param("TValue")
                .Void();
            store_unaligned_intrinsic = new IntrinsicBuilder("__store_conditional", true)
                .GenericTypes("TPtr", "TValue")
                .Param("TPtr")
                .Param("TValue")
                .Void();
            trap_barrier_intrinsic = new IntrinsicBuilder("__trap_barrier", true)
                .Void(); 
        }

        private static readonly IntrinsicProcedure cpys_intrinsic = IntrinsicBuilder.GenericBinary("__cpys");
        private static readonly IntrinsicProcedure cpyse_intrinsic = IntrinsicBuilder.GenericBinary("__cpyse");
        private static readonly IntrinsicProcedure cpysn_intrinsic = IntrinsicBuilder.GenericBinary("__cpysn");
        private static readonly IntrinsicProcedure cmpbge_intrinsic = IntrinsicBuilder.GenericBinary("__cmpbge");
        private static readonly IntrinsicProcedure extbl_intrinsic = IntrinsicBuilder.GenericBinary("__extbl");
        private static readonly IntrinsicProcedure extlh_intrinsic = IntrinsicBuilder.GenericBinary("__extlh");
        private static readonly IntrinsicProcedure extll_intrinsic = IntrinsicBuilder.GenericBinary("__extll");
        private static readonly IntrinsicProcedure extqh_intrinsic = IntrinsicBuilder.GenericBinary("__extqh");
        private static readonly IntrinsicProcedure extql_intrinsic = IntrinsicBuilder.GenericBinary("__extql");
        private static readonly IntrinsicProcedure extwh_intrinsic = IntrinsicBuilder.GenericBinary("__extwh");
        private static readonly IntrinsicProcedure extwl_intrinsic = IntrinsicBuilder.GenericBinary("__extwl");
        private static readonly IntrinsicProcedure implver_intrinsic;
        private static readonly IntrinsicProcedure insbl_intrinsic = IntrinsicBuilder.GenericBinary("__insbl");
        private static readonly IntrinsicProcedure inslh_intrinsic = IntrinsicBuilder.GenericBinary("__inslh");
        private static readonly IntrinsicProcedure insll_intrinsic = IntrinsicBuilder.GenericBinary("__insll");
        private static readonly IntrinsicProcedure insqh_intrinsic = IntrinsicBuilder.GenericBinary("__insqh");
        private static readonly IntrinsicProcedure insql_intrinsic = IntrinsicBuilder.GenericBinary("__insql");
        private static readonly IntrinsicProcedure inswl_intrinsic = IntrinsicBuilder.GenericBinary("__inswl");
        private static readonly IntrinsicProcedure load_locked_intrinsic;
        private static readonly IntrinsicProcedure mskbl_intrinsic = IntrinsicBuilder.GenericBinary("__mskbl");
        private static readonly IntrinsicProcedure msklh_intrinsic = IntrinsicBuilder.GenericBinary("__msklh");
        private static readonly IntrinsicProcedure mskll_intrinsic = IntrinsicBuilder.GenericBinary("__mskll");
        private static readonly IntrinsicProcedure mskqh_intrinsic = IntrinsicBuilder.GenericBinary("__mskqh");
        private static readonly IntrinsicProcedure mskql_intrinsic = IntrinsicBuilder.GenericBinary("__mskql");
        private static readonly IntrinsicProcedure mskwl_intrinsic = IntrinsicBuilder.GenericBinary("__mskwl");
        private static readonly IntrinsicProcedure store_conditional_intrinsic;
        private static readonly IntrinsicProcedure store_unaligned_intrinsic;
        private static readonly IntrinsicProcedure trap_overflow = new IntrinsicBuilder(
            "__trap_overflow", true, new ProcedureCharacteristics
            {
                Terminates = true,
            })
            .Void();


        private static readonly IntrinsicProcedure ov_intrinsic;
        private static readonly IntrinsicProcedure trap_barrier_intrinsic;
        private static readonly IntrinsicProcedure zap_intrinsic = IntrinsicBuilder.GenericBinary("__zap");
        private static readonly IntrinsicProcedure zapnot_intrinsic = IntrinsicBuilder.GenericBinary("__zapnot");
    }
}