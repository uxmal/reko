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

namespace Reko.Arch.WE32100
{
    public class WE32100Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private WE32100Architecture arch;
        private EndianImageReader rdr;
        private ProcessorState state;
        private IStorageBinder binder;
        private IRewriterHost host;
        private IEnumerator<WE32100Instruction> dasm;
        private WE32100Instruction instr;
        private RtlEmitter m;
        private InstrClass iclass;

        public WE32100Rewriter(WE32100Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new WE32100Disassembler(arch, rdr).GetEnumerator();
            this.instr = null!;
            this.m = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var rtls = new List<RtlInstruction>();
                m = new RtlEmitter(rtls);
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(instr.Address, $"WE32100 instruction '{instr}' is not supported yet.");
                    EmitUnitTest();
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    m.Invalid(); iclass = InstrClass.Invalid; break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("PPCRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }
    }
}