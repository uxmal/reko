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

using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tms7000
{
    public class Tms7000Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Tms7000Architecture arch;
        private EndianImageReader rdr;
        private IRewriterHost host;
        private IStorageBinder binder;
        private IEnumerator<Tms7000Instruction> dasm;
        private Tms7000Instruction instr;
        private RtlClass rtlc;
        private RtlEmitter m;

        public Tms7000Rewriter(Tms7000Architecture arch, EndianImageReader rdr, Tms7000State state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.host = host;
            this.binder = binder;
            this.dasm = new Tms7000Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.rtlc = RtlClass.Linear;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                switch (instr.Opcode)
                {
                default:
                    host.Error(instr.Address, "Rewriting x86 opcode '{0}' is not supported yet.", instr);
                    rtlc = RtlClass.Invalid;
                    break;
                case Opcode.nop: m.Nop(); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
                {
                    Class = rtlc,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
