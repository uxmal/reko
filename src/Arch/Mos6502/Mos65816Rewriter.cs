#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.Mos6502
{
    internal class Mos65816Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Mos65816Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Instruction> dasm;
        private Instruction instr;
        private RtlEmitter m;
        private InstrClass iclass;

        public Mos65816Rewriter(Mos65816Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Mos65816Disassembler(arch, rdr).GetEnumerator();
            this.instr = null!;
            this.m = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                var rtls = new List<RtlInstruction>();
                m = new RtlEmitter(rtls);
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(
                        instr.Address,
                        string.Format("Mos65816 instruction '{0}' is not supported yet.", instr));
                    EmitUnitTest();
                    goto case Mnemonic.illegal;
                case Mnemonic.illegal: iclass = InstrClass.Invalid; m.Invalid(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            }
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Mos65816", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}