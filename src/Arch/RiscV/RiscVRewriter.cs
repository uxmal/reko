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

using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Rtl;
using System.Collections;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using Reko.Core.Types;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.RiscV
{
    public partial class RiscVRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly RiscVArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<RiscVInstruction> dasm;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly ProcessorState state;
        private RiscVInstruction instr;
        private RtlEmitter m;
        private InstrClass rtlc;

        public RiscVRewriter(RiscVArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.dasm = new RiscVDisassembler(arch, rdr).GetEnumerator();
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var addr = dasm.Current.Address;
                var len = dasm.Current.Length;
                var rtlInstructions = new List<RtlInstruction>();
                this.rtlc = this.instr.InstructionClass;
                this.m = new RtlEmitter(rtlInstructions);

                switch (instr.Mnemonic)
                {
                default:
                    host.Warn(
                        instr.Address, 
                        "Risc-V instruction '{0}' not supported yet.",
                        instr.Mnemonic);
                    EmitUnitTest();
                    rtlc = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.invalid: rtlc = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.addi: RewriteAdd(); break;
                case Mnemonic.addiw: RewriteAddw(); break;
                case Mnemonic.addw: RewriteAddw(); break;
                case Mnemonic.and: RewriteBinOp(m.And); break;
                case Mnemonic.andi: RewriteBinOp(m.And); break;
                case Mnemonic.auipc: RewriteAuipc(); break;
                case Mnemonic.beq: RewriteBranch(m.Eq); break;
                case Mnemonic.bge: RewriteBranch(m.Ge); break;
                case Mnemonic.bgeu: RewriteBranch(m.Uge); break;
                case Mnemonic.blt: RewriteBranch(m.Lt); break;
                case Mnemonic.bltu: RewriteBranch(m.Ult); break;
                case Mnemonic.bne: RewriteBranch(m.Ne); break;
                case Mnemonic.c_add: RewriteCompressedBinOp(m.IAdd); break;
                case Mnemonic.c_addi: RewriteCompressedBinOp(m.IAdd); break;
                case Mnemonic.c_addi16sp: RewriteAddi16sp(); break;
                case Mnemonic.c_addi4spn: RewriteAddi4spn(); break;
                case Mnemonic.c_addiw: RewriteCompressedAdd(PrimitiveType.Word32); break;
                case Mnemonic.c_addw: RewriteCompressedAdd(PrimitiveType.Word32); break;
                case Mnemonic.c_and: RewriteCompressedBinOp(m.And); break;
                case Mnemonic.c_andi: RewriteCompressedBinOp(m.And); break;
                case Mnemonic.c_beqz: RewriteCompressedBranch(m.Eq); break;
                case Mnemonic.c_bnez: RewriteCompressedBranch(m.Ne); break;
                case Mnemonic.c_fld: RewriteFload(PrimitiveType.Real64); break;
                case Mnemonic.c_fldsp: RewriteLxsp(PrimitiveType.Real64); break;
                case Mnemonic.c_fsd: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.c_fsdsp: RewriteSxsp(PrimitiveType.Real64); break;
                case Mnemonic.c_j: RewriteCompressedJ(); break;
                case Mnemonic.c_jalr: RewriteCompressedJalr(); break;
                case Mnemonic.c_jr: RewriteCompressedJr(); break;
                case Mnemonic.c_ld: RewriteLoad(PrimitiveType.Word64); break;
                case Mnemonic.c_li: RewriteLi(); break;
                case Mnemonic.c_ldsp: RewriteLxsp(PrimitiveType.Word64); break;
                case Mnemonic.c_lui: RewriteLui(); break;
                case Mnemonic.c_lw: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.c_lwsp: RewriteLxsp(PrimitiveType.Word32); break;
                case Mnemonic.c_mv: RewriteCompressedMv(); break;
                case Mnemonic.c_or: RewriteCompressedBinOp(m.Or); break;
                case Mnemonic.c_slli: RewriteCompressedBinOp(SllI); break;
                case Mnemonic.c_srai: RewriteCompressedBinOp(SraI); break;
                case Mnemonic.c_srli: RewriteCompressedBinOp(SrlI); break;
                case Mnemonic.c_sub: RewriteCompressedBinOp(m.ISub); break;
                case Mnemonic.c_sd: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.c_sdsp: RewriteSxsp(PrimitiveType.Word64); break;
                case Mnemonic.c_subw: RewriteCompressedBinOp(m.ISub, PrimitiveType.Word32); break;
                case Mnemonic.c_sw: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.c_swsp: RewriteSxsp(PrimitiveType.Word32); break;
                case Mnemonic.c_xor: RewriteCompressedBinOp(m.Xor); break;
                case Mnemonic.divuw: RewriteBinOp(m.UDiv, PrimitiveType.Word32); break;
                case Mnemonic.divw: RewriteBinOp(m.SDiv, PrimitiveType.Word32); break;
                case Mnemonic.fcvt_d_s: RewriteFcvt(PrimitiveType.Real64); break;
                case Mnemonic.feq_s: RewriteFcmp(PrimitiveType.Real32, m.FEq); break;
                case Mnemonic.fmadd_s: RewriteFmadd(PrimitiveType.Real32, m.FAdd); break;
                case Mnemonic.fmv_d_x: RewriteFcvt(PrimitiveType.Real64); break;
                case Mnemonic.fmv_w_x: RewriteFcvt(PrimitiveType.Real32); break;
                case Mnemonic.fld: RewriteFload(PrimitiveType.Real64); break;
                case Mnemonic.flw: RewriteFload(PrimitiveType.Real32); break;
                case Mnemonic.fsd: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.fsw: RewriteStore(PrimitiveType.Real32); break;
                case Mnemonic.jal: RewriteJal(); break;
                case Mnemonic.jalr: RewriteJalr(); break;
                case Mnemonic.lb: RewriteLoad(PrimitiveType.SByte); break;
                case Mnemonic.lbu: RewriteLoad(PrimitiveType.Byte); break;
                case Mnemonic.ld: RewriteLoad(PrimitiveType.Word64); break;
                case Mnemonic.lh: RewriteLoad(PrimitiveType.Int16); break;
                case Mnemonic.lhu: RewriteLoad(PrimitiveType.UInt16); break;
                case Mnemonic.lui: RewriteLui(); break;
                case Mnemonic.lw: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.lwu: RewriteLoad(PrimitiveType.UInt32); break;
                case Mnemonic.mulw: RewriteBinOp(m.IMul, PrimitiveType.Word32); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.ori: RewriteOr(); break;
                    //$TODO: Reko has no unsigned modulus operator
                case Mnemonic.remuw: RewriteBinOp(m.Mod, PrimitiveType.Word32); break;
                case Mnemonic.remw: RewriteBinOp(m.Mod, PrimitiveType.Word32); break;
                case Mnemonic.sb: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.sd: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.sh: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.sw: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.sll: RewriteBinOp(m.Shl); break;
                case Mnemonic.slli: RewriteShift(m.Shl); break;
                case Mnemonic.slliw: RewriteShiftw(m.Shl); break;
                case Mnemonic.sllw: RewriteShiftw(m.Shl); break;
                case Mnemonic.slt: RewriteSlt(false); break;
                case Mnemonic.sltiu: RewriteSlti(true); break;
                case Mnemonic.sltu: RewriteSlt(true); break;
                case Mnemonic.srai: RewriteShift(m.Sar); break;
                case Mnemonic.sraiw: RewriteShiftw(m.Sar); break;
                case Mnemonic.srl: RewriteBinOp(m.Shr); break;
                case Mnemonic.srli: RewriteShift(m.Shr); break;
                case Mnemonic.srliw: RewriteShiftw(SrlI); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.subw: RewriteSubw(); break;
                case Mnemonic.xor: RewriteXor(); break;
                case Mnemonic.xori: RewriteXor(); break;
                }
                yield return new RtlInstructionCluster(
                    addr,
                    len,
                    rtlInstructions.ToArray())
                {
                    Class = rtlc,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression RewriteOp(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand rop:
                if (rop.Register.Number == 0)
                {
                    //$TODO: 32-bit!
                    return Constant.Word64(0);
                }
                return binder.EnsureRegister(rop.Register);
            case ImmediateOperand immop:
                return immop.Value;
            case AddressOperand addrop:
                return addrop.Address;
            }
            throw new NotImplementedException($"Rewriting RiscV addressing mode {op.GetType().Name} is not implemented yet.");
        }

        private void MaybeSignExtend(Expression dst, Expression src, DataType dt)
        {
            if (dt != null && dt.BitSize < dst.DataType.BitSize)
            {
                src = m.Cast(arch.NaturalSignedInteger, m.Cast(dt, src));
            }
            m.Assign(dst, src);
        }

        private Expression SllI(Expression a, Expression b)
        {
            b = Constant.Int32(((Constant) b).ToInt32());
            return m.Shl(a, b);
        }

        private Expression SraI(Expression a, Expression b)
        {
            b = Constant.Int32(((Constant) b).ToInt32());
            return m.Sar(a, b);
        }

        private Expression SrlI(Expression a, Expression b)
        {
            b = Constant.Int32(((Constant) b).ToInt32());
            return m.Shr(a, b);
        }

        private static HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(instr.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void RiscV_rw_" + instr.Mnemonic + "()");
            Debug.WriteLine("        {");
            Debug.Write("            RewriteCode(\"");
            Debug.Write(string.Join(
                "",
                bytes.Select(b => string.Format("{0:X2}", (int) b))));
            Debug.WriteLine("\");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|0000000000010000({0}): 1 instructions\",", dasm.Current.Length);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
    }
}