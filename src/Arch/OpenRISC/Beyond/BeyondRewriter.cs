#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Arch.OpenRISC.Beyond;

public class BeyondRewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly BeyondArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly List<RtlInstruction> instrs;
    private readonly RtlEmitter m;
    private readonly IEnumerator<BeyondInstruction> dasm;
    private BeyondInstruction instr;
    private InstrClass iclass;

    public BeyondRewriter(BeyondArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.dasm = new BeyondDisassembler(arch, rdr).GetEnumerator();
        this.instrs = new List<RtlInstruction>();
        this.m = new RtlEmitter(instrs);
        this.instr = default!;
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            instr = dasm.Current;
            iclass = instr.InstructionClass;
            switch (instr.Mnemonic)
            {
            default:
            case Mnemonic.Invalid:
            case Mnemonic.Nyi:
                EmitUnitTest(instr);

                break;
            }
            yield return new RtlInstructionCluster(instr.Address, instr.Length, instrs.ToArray());
            instrs.Clear();
        }
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected virtual void EmitUnitTest(BeyondInstruction instr)
    {
        var testgenSvc = arch.Services.GetService<ITestGenerationService>();
        testgenSvc?.ReportMissingRewriter("BeyondRw", instr, instr.Mnemonic.ToString(), rdr, "");
    }
}