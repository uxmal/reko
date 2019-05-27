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

                switch (instr.opcode)
                {
                default:
                    host.Warn(
                        instr.Address, 
                        "Risc-V instruction '{0}' not supported yet.",
                        instr.opcode);
                    EmitUnitTest();
                    rtlc = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.invalid: rtlc = InstrClass.Invalid; m.Invalid(); break;
                case Opcode.add: RewriteAdd(); break;
                case Opcode.addi: RewriteAdd(); break;
                case Opcode.addiw: RewriteAddw(); break;
                case Opcode.addw: RewriteAddw(); break;
                case Opcode.and: RewriteBinOp(m.And); break;
                case Opcode.andi: RewriteBinOp(m.And); break;
                case Opcode.auipc: RewriteAuipc(); break;
                case Opcode.beq: RewriteBranch(m.Eq); break;
                case Opcode.bge: RewriteBranch(m.Ge); break;
                case Opcode.bgeu: RewriteBranch(m.Uge); break;
                case Opcode.blt: RewriteBranch(m.Lt); break;
                case Opcode.bltu: RewriteBranch(m.Ult); break;
                case Opcode.bne: RewriteBranch(m.Ne); break;
                case Opcode.c_add: RewriteCompressedBinOp(m.IAdd); break;
                case Opcode.c_addi: RewriteCompressedBinOp(m.IAdd); break;
                case Opcode.c_addi16sp: RewriteAddi16sp(); break;
                case Opcode.c_addi4spn: RewriteAddi4spn(); break;
                case Opcode.c_addiw: RewriteCompressedAdd(PrimitiveType.Word32); break;
                case Opcode.c_addw: RewriteCompressedAdd(PrimitiveType.Word32); break;
                case Opcode.c_and: RewriteCompressedBinOp(m.And); break;
                case Opcode.c_andi: RewriteCompressedBinOp(m.And); break;
                case Opcode.c_beqz: RewriteCompressedBranch(m.Eq); break;
                case Opcode.c_bnez: RewriteCompressedBranch(m.Ne); break;
                case Opcode.c_fld: RewriteFload(PrimitiveType.Real64); break;
                case Opcode.c_fldsp: RewriteLxsp(PrimitiveType.Real64); break;
                case Opcode.c_fsd: RewriteStore(PrimitiveType.Real64); break;
                case Opcode.c_fsdsp: RewriteSxsp(PrimitiveType.Real64); break;
                case Opcode.c_j: RewriteCompressedJ(); break;
                case Opcode.c_jalr: RewriteCompressedJalr(); break;
                case Opcode.c_jr: RewriteCompressedJr(); break;
                case Opcode.c_ld: RewriteLoad(PrimitiveType.Word64); break;
                case Opcode.c_li: RewriteLi(); break;
                case Opcode.c_ldsp: RewriteLxsp(PrimitiveType.Word64); break;
                case Opcode.c_lui: RewriteLui(); break;
                case Opcode.c_lw: RewriteLoad(PrimitiveType.Word32); break;
                case Opcode.c_lwsp: RewriteLxsp(PrimitiveType.Word32); break;
                case Opcode.c_mv: RewriteCompressedMv(); break;
                case Opcode.c_or: RewriteCompressedBinOp(m.Or); break;
                case Opcode.c_slli: RewriteCompressedBinOp(SllI); break;
                case Opcode.c_srai: RewriteCompressedBinOp(SraI); break;
                case Opcode.c_srli: RewriteCompressedBinOp(SrlI); break;
                case Opcode.c_sub: RewriteCompressedBinOp(m.ISub); break;
                case Opcode.c_sd: RewriteStore(PrimitiveType.Word64); break;
                case Opcode.c_sdsp: RewriteSxsp(PrimitiveType.Word64); break;
                case Opcode.c_subw: RewriteCompressedBinOp(m.ISub, PrimitiveType.Word32); break;
                case Opcode.c_sw: RewriteStore(PrimitiveType.Word32); break;
                case Opcode.c_swsp: RewriteSxsp(PrimitiveType.Word32); break;
                case Opcode.c_xor: RewriteCompressedBinOp(m.Xor); break;
                case Opcode.divuw: RewriteBinOp(m.UDiv, PrimitiveType.Word32); break;
                case Opcode.divw: RewriteBinOp(m.SDiv, PrimitiveType.Word32); break;
                case Opcode.fcvt_d_s: RewriteFcvt(PrimitiveType.Real64); break;
                case Opcode.feq_s: RewriteFcmp(PrimitiveType.Real32, m.FEq); break;
                case Opcode.fmadd_s: RewriteFmadd(PrimitiveType.Real32, m.FAdd); break;
                case Opcode.fmv_d_x: RewriteFcvt(PrimitiveType.Real64); break;
                case Opcode.fmv_s_x: RewriteFcvt(PrimitiveType.Real32); break;
                case Opcode.fld: RewriteFload(PrimitiveType.Real64); break;
                case Opcode.flw: RewriteFload(PrimitiveType.Real32); break;
                case Opcode.fsd: RewriteStore(PrimitiveType.Real64); break;
                case Opcode.fsw: RewriteStore(PrimitiveType.Real32); break;
                case Opcode.jal: RewriteJal(); break;
                case Opcode.jalr: RewriteJalr(); break;
                case Opcode.lb: RewriteLoad(PrimitiveType.SByte); break;
                case Opcode.lbu: RewriteLoad(PrimitiveType.Byte); break;
                case Opcode.ld: RewriteLoad(PrimitiveType.Word64); break;
                case Opcode.lh: RewriteLoad(PrimitiveType.Int16); break;
                case Opcode.lhu: RewriteLoad(PrimitiveType.UInt16); break;
                case Opcode.lui: RewriteLui(); break;
                case Opcode.lw: RewriteLoad(PrimitiveType.Int32); break;
                case Opcode.lwu: RewriteLoad(PrimitiveType.UInt32); break;
                case Opcode.mulw: RewriteBinOp(m.IMul, PrimitiveType.Word32); break;
                case Opcode.or: RewriteOr(); break;
                case Opcode.ori: RewriteOr(); break;
                    //$TODO: Reko has no unsigned modulus operator
                case Opcode.remuw: RewriteBinOp(m.Mod, PrimitiveType.Word32); break;
                case Opcode.remw: RewriteBinOp(m.Mod, PrimitiveType.Word32); break;
                case Opcode.sb: RewriteStore(PrimitiveType.Byte); break;
                case Opcode.sd: RewriteStore(PrimitiveType.Word64); break;
                case Opcode.sh: RewriteStore(PrimitiveType.Word16); break;
                case Opcode.sw: RewriteStore(PrimitiveType.Word32); break;
                case Opcode.sll: RewriteBinOp(m.Shl); break;
                case Opcode.slli: RewriteShift(m.Shl); break;
                case Opcode.slliw: RewriteShiftw(m.Shl); break;
                case Opcode.sllw: RewriteShiftw(m.Shl); break;
                case Opcode.slt: RewriteSlt(false); break;
                case Opcode.sltiu: RewriteSlti(true); break;
                case Opcode.sltu: RewriteSlt(true); break;
                case Opcode.srai: RewriteShift(m.Sar); break;
                case Opcode.sraiw: RewriteShiftw(m.Sar); break;
                case Opcode.srl: RewriteBinOp(m.Shr); break;
                case Opcode.srli: RewriteShift(m.Shr); break;
                case Opcode.srliw: RewriteShiftw(SrlI); break;
                case Opcode.sub: RewriteSub(); break;
                case Opcode.subw: RewriteSubw(); break;
                case Opcode.xor: RewriteXor(); break;
                case Opcode.xori: RewriteXor(); break;
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

        private static HashSet<Opcode> seen = new HashSet<Opcode>();

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(instr.opcode))
                return;
            seen.Add(dasm.Current.opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void RiscV_rw_" + instr.opcode + "()");
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