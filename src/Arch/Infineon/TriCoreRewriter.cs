#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Infineon
{
    public class TriCoreRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly TriCoreArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<TriCoreInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private TriCoreInstruction instr;
        private InstrClass iclass;

        public TriCoreRewriter(TriCoreArchitecture triCoreArchitecture, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = triCoreArchitecture;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new TriCoreDisassembler(arch, rdr).GetEnumerator();
            this.rtls = new();
            this.m = new RtlEmitter(rtls);
            instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        private void EmitUnitTest()
        {
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("TriCoreRw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, "");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}