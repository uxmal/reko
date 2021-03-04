#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.CompactRisc
{
    internal class Cr16Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Cr16Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Cr16Instruction> dasm;
        private Cr16Instruction instr;
        private RtlEmitter m;
        private InstrClass iclass;

        public Cr16Rewriter(Cr16Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Cr16cDisassembler(arch, rdr).GetEnumerator();
            this.instr = null!;
            this.m = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                switch (instr.Mnemonic)
                {
                default:
                    host.Warn(instr.Address, "Cr16 instruction {0} not supported yet.", this.instr);
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid: m.Invalid(); iclass = InstrClass.Invalid; break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, this.iclass);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("MCoreRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }
    }
}