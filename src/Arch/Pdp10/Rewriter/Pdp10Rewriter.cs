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

using Reko.Arch.Pdp10.Disassembler;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp10.Rewriter
{
    public partial class Pdp10Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Pdp10Architecture arch;
        private readonly Word36BeImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Pdp10Instruction> dasm;
        private readonly RtlEmitter m;
        private readonly List<RtlInstruction> instrs;
        private readonly PrimitiveType word36;
        private Pdp10Instruction instr;
        private InstrClass iclass;


        public Pdp10Rewriter(Pdp10Architecture arch, Word36BeImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Pdp10Disassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.word36 = Pdp10Architecture.Word36;
            this.instr = default!;
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
                    break;
                case Mnemonic.Invalid:
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.jrst: RewriteJrst(); break;
                case Mnemonic.move: RewriteMove(); break;
                case Mnemonic.movei: RewriteMovei(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }

        private void EmitUnitTest()
        {
            var offset = rdr.Offset;
            rdr.Offset -= 1;
            rdr.TryReadBeUInt36(out ulong word);
            var opcodeAsString = Convert.ToString((long) word, 8).PadLeft(12, '0');
            rdr.Offset = offset;
            var instr = dasm.Current;
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter(
                "Pdp10Rw", instr, instr.Mnemonic.ToString(), rdr, instr.MnemonicAsString, opcodeAsString);

            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Identifier Ac()
        {
            var id = binder.EnsureRegister(((RegisterOperand) instr.Operands[0]).Register);
            return id;
        }


        private Expression AccessEa()
        {
            var ea = (MemoryOperand) instr.Operands[1];
            if (ea.Index is null)
            {
                // PDP-10 accumulators are memory mapped, so we check for them
                // first
                if (ea.Offset < 16)
                {
                    return binder.EnsureRegister(Registers.Accumulators[ea.Offset]);
                }
                return m.Mem(word36, new Address18(ea.Offset));
            }
            throw new NotImplementedException();
        }

        private Expression RewriteEa()
        {
            var ea = (MemoryOperand) instr.Operands[1];
            if (ea.Index is null)
            {
                return Constant.Create(word36, ea.Offset);
            }
            throw new NotImplementedException();
        }
    }
}
