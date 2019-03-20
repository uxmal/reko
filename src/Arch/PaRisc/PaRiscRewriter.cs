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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Rtl;

namespace Reko.Arch.PaRisc
{
    public class PaRiscRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly PaRiscArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<PaRiscInstruction> dasm;
        private RtlEmitter m;
        private PaRiscInstruction instr;
        private InstrClass iclass;

        public PaRiscRewriter(PaRiscArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new PaRiscDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.IClass;
                var instrs = new List<RtlInstruction>();
                m = new RtlEmitter(instrs);
                switch (instr.Opcode)
                { }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = iclass,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
