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

using Reko.Core;
using Reko.Core.Rtl;
using System.Collections.Generic;
using System;
using System.Collections;
using Reko.Core.Machine;

namespace Reko.Arch.Alpha
{
    public class AlphaRewriter : IEnumerable<RtlInstructionCluster>
    {
        private AlphaArchitecture arch;
        private IStorageBinder binder;
        private EndianImageReader rdr;
        private IEnumerator<AlphaInstruction> dasm;
        private IRewriterHost host;
        private ProcessorState state;
        private AlphaInstruction instr;
        private RtlClass rtlc;
        private RtlEmitter m;

        public AlphaRewriter(AlphaArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
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
                switch (instr.Opcode)
                {
                default: break;
                case Opcode.invalid: break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}