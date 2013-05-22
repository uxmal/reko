#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Operators;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public partial class Rewriter : IEnumerable<RtlInstructionCluster>
    {
        internal M68kArchitecture arch;
        private M68kState state;
        internal Frame frame;
        private IRewriterHost host;
        private IEnumerator<DisassembledInstruction> dasm;
        internal DisassembledInstruction di;
        private RtlInstructionCluster ric;
        internal RtlEmitter emitter;
        private OperandRewriter orw;

        public Rewriter(M68kArchitecture m68kArchitecture, ImageReader rdr, M68kState m68kState, Frame frame, IRewriterHost host)
        {
            this.arch = m68kArchitecture;
            this.state = m68kState;
            this.frame = frame;
            this.host = host;
            this.dasm = CreateDisassemblyStream(rdr);
        }

        protected IEnumerator<DisassembledInstruction> CreateDisassemblyStream(ImageReader rdr)
        {
            var d = (M68kDisassembler2) arch.CreateDisassembler(rdr);
            while (rdr.IsValid)
            {
                var addr = d.Address;
                var instr = d.Disassemble();
                if (instr == null)
                    yield break;
                var length = (uint)(d.Address - addr);
                yield return new DisassembledInstruction(addr, instr, length);
            }
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                di = dasm.Current;
                ric = new RtlInstructionCluster(di.Address, (byte)di.Length);
                emitter = new RtlEmitter(ric.Instructions);
                orw = new OperandRewriter(this);
                switch (di.Instruction.code)
                {
                case Opcode.adda: RewriteAdda(); break;
                case Opcode.eor: RewriteEor(); break;
                case Opcode.movea: RewriteMove(false); break;
                default:
                    throw new AddressCorrelatedException(string.Format("Rewriting x86 opcode '{0}' is not supported yet.",
                        di.Instruction.code),
                        di.Address);
                }
                 yield return ric;

            }
            yield break;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
