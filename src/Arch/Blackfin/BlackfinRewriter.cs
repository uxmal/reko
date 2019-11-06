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
using Reko.Core;
using Reko.Core.Rtl;

namespace Reko.Arch.Blackfin
{
    internal class BlackfinRewriter : IEnumerable<RtlInstructionCluster>
    {
        private static HashSet<Mnemonic> opcode_seen = new HashSet<Mnemonic>();

        private readonly BlackfinArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<BlackfinInstruction> dasm;
        private RtlEmitter m;
        private InstrClass iclass;
        private BlackfinInstruction instr;

        public BlackfinRewriter(BlackfinArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new BlackfinDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest(instr);
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
                {
                    Class = iclass
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void EmitUnitTest(BlackfinInstruction instr)
        {
            if (opcode_seen.Contains(instr.Mnemonic))
                return;
            opcode_seen.Add(instr.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= instr.Length;

            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void BlackfinRw_{0}()", instr.Mnemonic);
            Console.WriteLine("        {");

            if (instr.Length > 2)
            {
                var wInstr = r2.ReadBeUInt32();
                Console.WriteLine($"            RewriteCode(\"{wInstr:X8}\");");
            }
            else
            {
                var wInstr = r2.ReadBeUInt16();
                Console.WriteLine($"            RewriteCode(\"{wInstr:X4}\");");
            }
            Console.WriteLine("            AssertCode(");
            Console.WriteLine($"                \"0|L--|00100000({instr.Length}): 1 instructions\",");
            Console.WriteLine($"                \"1|L--|@@@\");");
            Console.WriteLine("        }");
        }

    }
}