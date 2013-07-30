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
using Decompiler.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Z80
{
    public class Z80Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Z80ProcessorArchitecture arch;
        private ProcessorState state;
        private Frame frame;
        private IRewriterHost host;
        private IEnumerator<DisassembledInstruction> dasm;

        public Z80Rewriter(Z80ProcessorArchitecture arch, ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            // TODO: Complete member initialization
            this.arch = arch;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = CreateDasmStream(rdr).GetEnumerator();
        }

        private class DisassembledInstruction
        {
            public Address Address;
            public Z80Instruction Instruction;
            public int Size;
        }

        private IEnumerable<DisassembledInstruction> CreateDasmStream(ImageReader rdr)
        {
            var d = new Z80Disassembler(rdr);
            while (rdr.IsValid)
            {
                var addr = rdr.Address;
                var instr = d.Disassemble();
                yield return new DisassembledInstruction
                {
                    Address = addr,
                    Instruction = instr,
                    Size = rdr.Address  -addr,
                };
            }
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var rtlc = new RtlInstructionCluster(dasm.Current.Address, (byte) dasm.Current.Size);
                yield return rtlc;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
