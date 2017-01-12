#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Reko.Core;

namespace Reko.Arch.Avr
{
    public class Avr8Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Avr8Architecture arch;
        private Frame frame;
        private IRewriterHost host;
        private Avr8Disassembler dasm;
        private ProcessorState state;
        private AvrInstruction instr;
        private RtlInstructionCluster rtlc;
        private RtlEmitter m;

        public Avr8Rewriter(Avr8Architecture arch, ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.dasm = new Avr8Disassembler(arch, rdr);
            this.state = state;
            this.frame = frame;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            foreach (var instr in dasm)
            {
                this.instr = instr;
                this.rtlc = new RtlInstructionCluster(instr.Address, instr.Length);
                this.m = new RtlEmitter(rtlc.Instructions);
                switch (instr.opcode)
                {
                default:
                    host.Error(instr.Address, string.Format("AVR8 instruction '{0}' is not supported yet.", instr.opcode));
                    m.Invalid();
                    break;
                }
                yield return rtlc;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
