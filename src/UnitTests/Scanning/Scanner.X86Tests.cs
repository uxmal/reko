#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Environments.Msdos;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class Scanner_x86Tests
    {
        private IntelArchitecture arch;
        private Scanner scanner;

        private void BuildTest16(Action<IntelAssembler> asmProg)
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            var emitter = new IntelEmitter();
            var entryPoints = new List<EntryPoint>();
            var addrBase = new Address(0xC00, 0);
            var asm = new IntelAssembler(arch, addrBase, entryPoints);
            asmProg(asm);

            scanner = new Scanner(
                arch,
                new ProgramImage(addrBase, emitter.Bytes),
                new MsdosPlatform(arch),
                new Dictionary<Address, ProcedureSignature>(),
                new FakeDecompilerEventListener());
            scanner.EnqueueEntryPoint(new EntryPoint(addrBase, arch.CreateProcessorState()));
            scanner.ProcessQueue();
            DumpProgram(scanner);
        }

        private void DumpProgram(Scanner scanner)
        {
            foreach (Procedure proc in scanner.Procedures.Values)
            {
                proc.Write(true, Console.Out);
                Console.Out.WriteLine();
            }
        }

        [Test]
        public void NestedProcedures()
        {
            BuildTest16(delegate(IntelAssembler m)
            {
                m.Mov(m.ax, 3);
                m.Push(m.ax);
                m.Call("p2");
                m.Mov(m.MemW(Registers.ds, MachineRegister.None, 300), m.ax);
                m.Ret();

                m.Proc("p2");
                m.Add(m.ax, 2);
                m.Ret();
            });
        }

        [Test]
        public void RepScasw()
        {
            BuildTest16(delegate(IntelAssembler m)
            {
                m.Rep();
                m.Scasw();
                m.Ret();
            });
            var sw = new StringWriter();
            scanner.Procedures.Values[0].Write(false, sw);
            Console.WriteLine(sw.ToString());
            var sExp = @"// fn0C00_0000
void fn0C00_0000()
fn0C00_0000_entry:
l0C00_0000:
	branch cx == 0x0000 l0C00_0002
l
l0C00_0002:
	v3 = Mem0[ds:si:word16]
	store(Mem0[es:di:word16]) = v3
	si = si + 0x0002
	di = di + 0x0002
	cx = cx - 0x0001
	return
fn0C00_0000_exit:
";
            Assert.AreEqual(sExp, sw.ToString());
        }
    }
}
