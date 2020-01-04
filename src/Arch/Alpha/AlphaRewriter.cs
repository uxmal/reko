#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        private InstrClass rtlc;
        private RtlEmitter m;

        public AlphaRewriter(AlphaArchitecture arch, EndianImageReader rdr, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.binder = binder;
            this.host = host;
            this.dasm = new AlphaDisassembler(this.arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var instrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(instrs);
                this.rtlc = instr.InstructionClass;
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
                case Mnemonic.cmpbge: RewriteInstrinsic("__cmpbge"); break;
                case Mnemonic.cmpeq: RewriteCmp(m.Eq); break;
                case Mnemonic.cmple: RewriteCmp(m.Le); break;
                case Mnemonic.cmplt: RewriteCmp(m.Lt); break;
                case Mnemonic.cmpteq: RewriteCmpt(m.Eq); break;
                case Mnemonic.cmptle: RewriteCmpt(m.Le); break;
                case Mnemonic.cmptlt: RewriteCmpt(m.Lt); break;
                case Mnemonic.cmpule: RewriteCmp(m.Ule); break;
                case Mnemonic.cmpult: RewriteCmp(m.Ult); break;
                case Mnemonic.cpys: RewriteCpys("__cpys"); break;
                case Mnemonic.cpyse: RewriteCpys("__cpyse"); break;
                case Mnemonic.cpysn: RewriteCpys("__cpysn"); break;
                case Mnemonic.cvtlq: RewriteCvt(PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.cvtql: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Int32); break;
                case Mnemonic.cvtqs: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Real32); break;
                case Mnemonic.cvtqt: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Real64); break;
                case Mnemonic.cvttq_c: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Int64); break;
                case Mnemonic.cvtts: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.divs: RewriteFpuOp(m.FDiv); break;
                case Mnemonic.divt: RewriteFpuOp(m.FDiv); break;
                case Mnemonic.extbl: RewriteInstrinsic("__extbl"); break;
                case Mnemonic.extlh: RewriteInstrinsic("__extlh"); break;
                case Mnemonic.extll: RewriteInstrinsic("__extll"); break;
                case Mnemonic.extqh: RewriteInstrinsic("__extqh"); break;
                case Mnemonic.extql: RewriteInstrinsic("__extql"); break;
                case Mnemonic.extwh: RewriteInstrinsic("__extwh"); break;
                case Mnemonic.extwl: RewriteInstrinsic("__extwl"); break;
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
                case Mnemonic.implver: RewriteInstrinsic("__implver"); break;
                case Mnemonic.insbl: RewriteInstrinsic("__insbl"); break;
                case Mnemonic.inslh: RewriteInstrinsic("__inslh"); break;
                case Mnemonic.insll: RewriteInstrinsic("__insll"); break;
                case Mnemonic.insqh: RewriteInstrinsic("__insqh"); break;
                case Mnemonic.insql: RewriteInstrinsic("__insql"); break;
                case Mnemonic.inswl: RewriteInstrinsic("__inswl"); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsr: RewriteJmp(); break;
                case Mnemonic.jsr_coroutine: RewriteJmp(); break;
                case Mnemonic.lda: RewriteLda(0); break;
                case Mnemonic.ldah: RewriteLda(16); break;
                case Mnemonic.ldbu: RewriteLd(PrimitiveType.Byte, PrimitiveType.Word64); break;
                case Mnemonic.ldf: RewriteLd(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.ldg: RewriteLd(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.ldl: RewriteLd(PrimitiveType.Int32, PrimitiveType.Word64); break;
                case Mnemonic.ldl_l: RewriteLoadInstrinsic("__ldl_l", PrimitiveType.Word32); break;
                case Mnemonic.ldq_l: RewriteLoadInstrinsic("__ldq_l", PrimitiveType.Word64); break;
                case Mnemonic.ldq: RewriteLd(PrimitiveType.Word64, PrimitiveType.Word64); break;
                case Mnemonic.ldq_u: RewriteLd(PrimitiveType.Word64, PrimitiveType.Word64); break;
                case Mnemonic.lds: RewriteLd(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.ldt: RewriteLd(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.ldwu: RewriteLd(PrimitiveType.UInt16, PrimitiveType.Word64); break;
                case Mnemonic.mskbl: RewriteInstrinsic("__mskbl"); break;
                case Mnemonic.msklh: RewriteInstrinsic("__msklh"); break;
                case Mnemonic.mskll: RewriteInstrinsic("__mskll"); break;
                case Mnemonic.mskqh: RewriteInstrinsic("__mskqh"); break;
                case Mnemonic.mskql: RewriteInstrinsic("__mskql"); break;
                case Mnemonic.mskwl: RewriteInstrinsic("__mskwl"); break;
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
                case Mnemonic.src: RewriteInstrinsic("__src"); break;
                case Mnemonic.srl: RewriteBin(srl); break;
                case Mnemonic.stb: RewriteSt(PrimitiveType.Byte); break;
                case Mnemonic.stf: RewriteSt(PrimitiveType.Real32); break;
                case Mnemonic.stl: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stl_c: RewriteStoreInstrinsic("__stl_c", PrimitiveType.Word32); break;
                case Mnemonic.stq_c: RewriteStoreInstrinsic("__stq_c", PrimitiveType.Word64); break;
                case Mnemonic.stq_u: RewriteStoreInstrinsic("__stq_u", PrimitiveType.Word64); break;
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
                case Mnemonic.zap: RewriteInstrinsic("__zap"); break;
                case Mnemonic.zapnot: RewriteInstrinsic("__zapnot"); break;

                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        private static HashSet<Mnemonic> seen = new HashSet<Mnemonic>();


        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void AlphaRw_" + dasm.Current.Mnemonic + "()");
            Debug.WriteLine("        {");
            Debug.Write("            RewriteCode(\"");
            Debug.Write(string.Join(
                "",
                bytes.Select(b => string.Format("{0:X2}", (int)b))));
            Debug.WriteLine("\");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", dasm.Current.Length);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Invalid()
        {
            m.Invalid();
            rtlc = InstrClass.Invalid;
        }

        private Expression Rewrite(MachineOperand op, bool highWord = false)
        {
            switch (op)
            {
            case RegisterOperand rop:
            {
                if (rop.Register.Number == 31)
                    return Constant.Word64(0);
                else if (rop.Register.Number == 63)
                    return Constant.Real64(0.0);
                else
                    return this.binder.EnsureRegister(rop.Register);
            }
            case ImmediateOperand imm:
                return imm.Value;
            case AddressOperand addr:
                return addr.Address;
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
                return m.Mem(mop.Width, ea);
            }
            throw new NotImplementedException(string.Format("{0} ({1})", op, op.GetType().Name));
        }
    }
}