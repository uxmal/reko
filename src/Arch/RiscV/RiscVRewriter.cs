#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.RiscV
{
    public partial class RiscVRewriter : IEnumerable<RtlInstructionCluster>
    {
        private RiscVArchitecture arch;
        private IEnumerator<RiscVInstruction> dasm;
        private RtlEmitter m;
        private IStorageBinder frame;
        private IRewriterHost host;
        private RiscVInstruction instr;
        private List<RtlInstruction> rtlInstructions;
        private RtlClass rtlc;
        private ProcessorState state;

        public RiscVRewriter(RiscVArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
        {
            this.arch = arch;
            this.dasm = new RiscVDisassembler(arch, rdr).GetEnumerator();
            this.state = state;
            this.frame = frame;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var addr = dasm.Current.Address;
                var len = dasm.Current.Length;
                this.rtlInstructions = new List<RtlInstruction>();
                this.rtlc = RtlClass.Linear;
                this.m = new RtlEmitter(rtlInstructions);

                switch (instr.opcode)
                {
                default:
                    host.Warn(
                        instr.Address, 
                        "Rewriting of Risc-V instruction '{0}' not implemented yet.",
                        instr.opcode);
                    rtlc = RtlClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.invalid: rtlc = RtlClass.Invalid; m.Invalid(); break;
                case Opcode.add: RewriteAdd(); break;
                case Opcode.addi: RewriteAdd(); break;
                case Opcode.addiw: RewriteAddw(); break;
                case Opcode.addw: RewriteAddw(); break;
                case Opcode.and: RewriteAnd(); break;
                case Opcode.andi: RewriteAnd(); break;
                case Opcode.auipc: RewriteAuipc(); break;
                case Opcode.beq: RewriteBranch(m.Eq); break;
                case Opcode.bge: RewriteBranch(m.Ge); break;
                case Opcode.bgeu: RewriteBranch(m.Uge); break;
                case Opcode.blt: RewriteBranch(m.Lt); break;
                case Opcode.bltu: RewriteBranch(m.Ult); break;
                case Opcode.bne: RewriteBranch(m.Ne); break;
                case Opcode.fcvt_d_s: RewriteFcvt(PrimitiveType.Real64); break;
                case Opcode.feq_s: RewriteFcmp(PrimitiveType.Real32, m.FEq); break;
                case Opcode.fmadd_s: RewriteFmadd(PrimitiveType.Real32, m.FAdd); break;
                case Opcode.fmv_d_x: RewriteFcvt(PrimitiveType.Real64); break;
                case Opcode.fmv_s_x: RewriteFcvt(PrimitiveType.Real32); break;
                case Opcode.flw: RewriteFload(PrimitiveType.Real32); break;
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
                case Opcode.or: RewriteOr(); break;
                case Opcode.ori: RewriteOr(); break;
                case Opcode.sb: RewriteStore(PrimitiveType.Byte); break;
                case Opcode.sd: RewriteStore(PrimitiveType.Word64); break;
                case Opcode.sh: RewriteStore(PrimitiveType.Word16); break;
                case Opcode.sw: RewriteStore(PrimitiveType.Word32); break;
                case Opcode.slli: RewriteShift(m.Shl); break;
                case Opcode.slliw: RewriteShiftw(m.Shl); break;
                case Opcode.sllw: RewriteShiftw(m.Shl); break;
                case Opcode.slt: RewriteSlt(false); break;
                case Opcode.sltu: RewriteSlt(true); break;
                case Opcode.srai: RewriteShift(m.Sar); break;
                case Opcode.sraiw: RewriteShiftw(m.Sar); break;
                case Opcode.srli: RewriteShift(m.Shr); break;
                case Opcode.srliw: RewriteShiftw(m.Shr); break;
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
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                if (rop.Register.Number == 0)
                {
                    //$TODO: 32-bit!
                    return Constant.Word64(0);
                }
                return frame.EnsureRegister(rop.Register);
            }
            var immop = op as ImmediateOperand;
            if (immop != null)
            {
                return immop.Value;
            }
            var addrop = op as AddressOperand;
            if (addrop != null)
            {
                return addrop.Address;
            }
            throw new NotImplementedException();
        }
    }
}