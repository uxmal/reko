#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
        private Frame frame;
        private IRewriterHost host;
        private RiscVInstruction instr;
        private RtlInstructionCluster rtlc;
        private ProcessorState state;

        public RiscVRewriter(RiscVArchitecture arch, ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
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

                this.rtlc = new RtlInstructionCluster(dasm.Current.Address, dasm.Current.Length);
                this.rtlc.Class = RtlClass.Linear;
                this.m = new RtlEmitter(rtlc.Instructions);

                switch (instr.opcode)
                {
                default:
                    throw new AddressCorrelatedException(
                       instr.Address,
                       "Rewriting of Risc-V instruction '{0}' not implemented yet.",
                       instr.opcode);
                case Opcode.addi: RewriteAdd(); break;
                case Opcode.andi: RewriteAnd(); break;
                case Opcode.auipc: RewriteAuipc(); break;
                case Opcode.lb: RewriteLoad(PrimitiveType.Byte); break;
                case Opcode.ld: RewriteLoad(PrimitiveType.Word64); break;
                }
                yield return rtlc;
            }
            yield break;
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