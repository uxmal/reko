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
                switch (instr.Opcode)
                {
                default:
                    EmitUnitTest();
                    Invalid();
                    host.Warn(
                       instr.Address,
                       string.Format(
                           "Alpha AXP instruction '{0}' not supported yet.",
                           instr.Opcode));

                    break;
                case Opcode.invalid:
                    Invalid();
                    break;
                case Opcode.addf_c: RewriteFpuOp(m.FAdd); break;
                case Opcode.adds: RewriteFpuOp(m.FAdd); break;
                case Opcode.adds_c: RewriteFpuOp(m.FAdd); break;
                case Opcode.addl: RewriteBin(addl); break;
                case Opcode.addl_v: RewriteBinOv(addl); break;
                case Opcode.addt: RewriteFpuOp(m.FAdd); break;
                case Opcode.addq: RewriteBin(addq); break;
                case Opcode.addq_v: RewriteBinOv(addq); break;
                case Opcode.and: RewriteBin(and); break;
                case Opcode.beq: RewriteBranch(m.Eq0); break;
                case Opcode.bge: RewriteBranch(m.Ge0); break;
                case Opcode.bgt: RewriteBranch(m.Gt0); break;
                case Opcode.bic: RewriteBin(bic); break;
                case Opcode.bis: RewriteBin(bis); break;
                case Opcode.blbc: RewriteBranch(lbc); break;
                case Opcode.blbs: RewriteBranch(lbs); break;
                case Opcode.ble: RewriteBranch(m.Le0); break;
                case Opcode.blt: RewriteBranch(m.Lt0); break;
                case Opcode.bne: RewriteBranch(m.Ne0); break;
                case Opcode.br: RewriteBr(); break;
                case Opcode.bsr: RewriteBr(); break;
                case Opcode.cmovlt: RewriteCmov(m.Lt0); break;
                case Opcode.cmovne: RewriteCmov(m.Ne0); break;
                case Opcode.cmovge: RewriteCmov(m.Ge0); break;
                case Opcode.cmovlbc: RewriteCmov(lbc); break;
                case Opcode.cmovlbs: RewriteCmov(lbs); break;
                case Opcode.cmpbge: RewriteInstrinsic("__cmpbge"); break;
                case Opcode.cmpeq: RewriteCmp(m.Eq); break;
                case Opcode.cmple: RewriteCmp(m.Le); break;
                case Opcode.cmplt: RewriteCmp(m.Lt); break;
                case Opcode.cmpteq: RewriteCmpt(m.Eq); break;
                case Opcode.cmptle: RewriteCmpt(m.Le); break;
                case Opcode.cmptlt: RewriteCmpt(m.Lt); break;
                case Opcode.cmpule: RewriteCmp(m.Ule); break;
                case Opcode.cmpult: RewriteCmp(m.Ult); break;
                case Opcode.cpys: RewriteCpys("__cpys"); break;
                case Opcode.cpyse: RewriteCpys("__cpyse"); break;
                case Opcode.cpysn: RewriteCpys("__cpysn"); break;
                case Opcode.cvtlq: RewriteCvt(PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Opcode.cvtql: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Int32); break;
                case Opcode.cvtqs: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Real32); break;
                case Opcode.cvtqt: RewriteCvt(PrimitiveType.Int64, PrimitiveType.Real64); break;
                case Opcode.cvttq_c: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Int64); break;
                case Opcode.cvtts: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Opcode.divs: RewriteFpuOp(m.FDiv); break;
                case Opcode.divt: RewriteFpuOp(m.FDiv); break;
                case Opcode.extbl: RewriteInstrinsic("__extbl"); break;
                case Opcode.extlh: RewriteInstrinsic("__extlh"); break;
                case Opcode.extll: RewriteInstrinsic("__extll"); break;
                case Opcode.extqh: RewriteInstrinsic("__extqh"); break;
                case Opcode.extql: RewriteInstrinsic("__extql"); break;
                case Opcode.extwh: RewriteInstrinsic("__extwh"); break;
                case Opcode.extwl: RewriteInstrinsic("__extwl"); break;
                case Opcode.fbeq: RewriteFBranch(Operator.Feq); break;
                case Opcode.fbge: RewriteFBranch(Operator.Fge); break;
                case Opcode.fbgt: RewriteFBranch(Operator.Fgt); break;
                case Opcode.fble: RewriteFBranch(Operator.Fle); break;
                case Opcode.fblt: RewriteFBranch(Operator.Flt); break;
                case Opcode.fbne: RewriteFBranch(Operator.Fne); break;
                case Opcode.fcmoveq: RewriteFCmov(Operator.Feq); break;
                case Opcode.fcmovle: RewriteFCmov(Operator.Fle); break;
                case Opcode.fcmovlt: RewriteFCmov(Operator.Flt); break;
                case Opcode.fcmovne: RewriteFCmov(Operator.Fne); break;
                case Opcode.halt: RewriteHalt(); break;
                case Opcode.implver: RewriteInstrinsic("__implver"); break;
                case Opcode.insbl: RewriteInstrinsic("__insbl"); break;
                case Opcode.inslh: RewriteInstrinsic("__inslh"); break;
                case Opcode.insll: RewriteInstrinsic("__insll"); break;
                case Opcode.insqh: RewriteInstrinsic("__insqh"); break;
                case Opcode.insql: RewriteInstrinsic("__insql"); break;
                case Opcode.inswl: RewriteInstrinsic("__inswl"); break;
                case Opcode.jmp: RewriteJmp(); break;
                case Opcode.jsr: RewriteJmp(); break;
                case Opcode.jsr_coroutine: RewriteJmp(); break;
                case Opcode.lda: RewriteLda(0); break;
                case Opcode.ldah: RewriteLda(16); break;
                case Opcode.ldbu: RewriteLd(PrimitiveType.Byte, PrimitiveType.Word64); break;
                case Opcode.ldf: RewriteLd(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Opcode.ldg: RewriteLd(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Opcode.ldl: RewriteLd(PrimitiveType.Int32, PrimitiveType.Word64); break;
                case Opcode.ldl_l: RewriteLoadInstrinsic("__ldl_l", PrimitiveType.Word32); break;
                case Opcode.ldq_l: RewriteLoadInstrinsic("__ldq_l", PrimitiveType.Word64); break;
                case Opcode.ldq: RewriteLd(PrimitiveType.Word64, PrimitiveType.Word64); break;
                case Opcode.ldq_u: RewriteLd(PrimitiveType.Word64, PrimitiveType.Word64); break;
                case Opcode.lds: RewriteLd(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Opcode.ldt: RewriteLd(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Opcode.ldwu: RewriteLd(PrimitiveType.UInt16, PrimitiveType.Word64); break;
                case Opcode.mskbl: RewriteInstrinsic("__mskbl"); break;
                case Opcode.msklh: RewriteInstrinsic("__msklh"); break;
                case Opcode.mskll: RewriteInstrinsic("__mskll"); break;
                case Opcode.mskqh: RewriteInstrinsic("__mskqh"); break;
                case Opcode.mskql: RewriteInstrinsic("__mskql"); break;
                case Opcode.mskwl: RewriteInstrinsic("__mskwl"); break;
                case Opcode.mull: RewriteBin(mull); break;
                case Opcode.mulq: RewriteBin(mulq); break;
                case Opcode.muls: RewriteFpuOp(m.FMul); break;
                case Opcode.mult: RewriteFpuOp(m.FMul); break;
                case Opcode.mult_c: RewriteFpuOp(m.FMul); break;
                case Opcode.ornot: RewriteBin(ornot); break;
                case Opcode.ret: RewriteJmp(); break;
                case Opcode.s4addl: RewriteBin(s4addl); break;
                case Opcode.s4addq: RewriteBin(s4addq); break;
                case Opcode.s8addl: RewriteBin(s8addl); break;
                case Opcode.s8addq: RewriteBin(s8addq); break;
                case Opcode.s4subl: RewriteBin(s4subl); break;
                case Opcode.s4subq: RewriteBin(s4subq); break;
                case Opcode.s8subl: RewriteBin(s8subl); break;
                case Opcode.s8subq: RewriteBin(s8subq); break;
                case Opcode.sll: RewriteBin(sll); break;
                case Opcode.src: RewriteInstrinsic("__src"); break;
                case Opcode.srl: RewriteBin(srl); break;
                case Opcode.stb: RewriteSt(PrimitiveType.Byte); break;
                case Opcode.stf: RewriteSt(PrimitiveType.Real32); break;
                case Opcode.stl: RewriteSt(PrimitiveType.Word32); break;
                case Opcode.stl_c: RewriteStoreInstrinsic("__stl_c", PrimitiveType.Word32); break;
                case Opcode.stq_c: RewriteStoreInstrinsic("__stq_c", PrimitiveType.Word64); break;
                case Opcode.stq_u: RewriteStoreInstrinsic("__stq_u", PrimitiveType.Word64); break;
                case Opcode.stg: RewriteSt(PrimitiveType.Real64); break;
                case Opcode.stw: RewriteSt(PrimitiveType.Word16); break;
                case Opcode.sts: RewriteSt(PrimitiveType.Real32); break;
                case Opcode.stt: RewriteSt(PrimitiveType.Real64); break;
                case Opcode.stq: RewriteSt(PrimitiveType.Word64); break;
                case Opcode.subf_s: RewriteFpuOp(m.FSub); break;
                case Opcode.subf_uc: RewriteFpuOp(m.FSub); break;
                case Opcode.subl: RewriteBin(subl); break;
                case Opcode.subl_v: RewriteBinOv(subl); break;
                case Opcode.subq: RewriteBin(subq); break;
                case Opcode.subq_v: RewriteBinOv(subq); break;
                case Opcode.subs: RewriteFpuOp(m.FSub); break;
                case Opcode.subt: RewriteFpuOp(m.FSub); break;
                case Opcode.trapb: RewriteTrapb(); break;
                case Opcode.umulh: RewriteBin(umulh); break;
                case Opcode.xor: RewriteBin(xor); break;
                case Opcode.zap: RewriteInstrinsic("__zap"); break;
                case Opcode.zapnot: RewriteInstrinsic("__zapnot"); break;

                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        private static HashSet<Opcode> seen = new HashSet<Opcode>();


        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Opcode))
                return;
            seen.Add(dasm.Current.Opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void AlphaRw_" + dasm.Current.Opcode + "()");
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