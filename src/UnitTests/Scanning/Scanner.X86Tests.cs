#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
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
        private Program program;

        private void BuildTest16(Action<X86Assembler> asmProg)
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            BuildTest(Address.SegPtr(0x0C00, 0x0000), new MsdosPlatform(null, arch), asmProg);
        }

        private void BuildTest32(Action<X86Assembler> asmProg)
        {
            arch = new IntelArchitecture(ProcessorMode.Protected32);
            BuildTest(Address.Ptr32(0x00100000), new FakePlatform(null, null), asmProg);
        }

        private void BuildTest(Address addrBase, Platform platform , Action<X86Assembler> asmProg)
        {
            var entryPoints = new List<EntryPoint>();
            var asm = new X86Assembler(arch, addrBase, entryPoints);
            asmProg(asm);

            var lr = asm.GetImage();
            program = new Program(
                lr.Image,
                lr.ImageMap,
                arch,
                platform);
            var project = new Project { Programs = { program } };
            scanner = new Scanner(
                program,
                new Dictionary<Address, ProcedureSignature>(),
                new ImportResolver(project),
                new FakeDecompilerEventListener());
            scanner.EnqueueEntryPoint(new EntryPoint(addrBase, arch.CreateProcessorState()));
            scanner.ScanImage();
        }

        private void DumpProgram(Scanner scanner)
        {
            var dasm = arch.CreateDisassembler(program.Image.CreateLeReader(0));
            foreach (var instr in dasm)
            {
                Console.Out.WriteLine("{0} {1}", instr.Address, instr);
            }
            
            foreach (Procedure proc in program.Procedures.Values)
            {
                proc.Write(true, Console.Out);
                Console.Out.WriteLine();
            }
        }

        [Test]
        public void Scanx86NestedProcedures()
        {
            BuildTest16(delegate(X86Assembler m)
            {
                m.Mov(m.ax, 3);
                m.Push(m.ax);
                m.Call("p2");
                m.Mov(m.MemW(Registers.ds, RegisterStorage.None, 300), m.ax);
                m.Ret();

                m.Proc("p2");
                m.Add(m.ax, 2);
                m.Ret();
            });
        }

        [Test]
        public void Scanx86RepScasw()
        {
            BuildTest16(delegate(X86Assembler m)
            {
                m.Rep();
                m.Scasw();
                m.Ret();
            });
            var sw = new StringWriter();
            program.Procedures.Values[0].Write(false, sw);
            var sExp = @"// fn0C00_0000
// Return size: 2
void fn0C00_0000()
fn0C00_0000_entry:
	// succ:  l0C00_0000
l0C00_0000:
	sp = fp
	branch cx == 0x0000 l0C00_0002
	// succ:  l0C00_0000_1 l0C00_0002
l0C00_0000_1:
	SCZO = cond(ax - Mem0[es:di:word16])
	di = di + 0x0002
	cx = cx - 0x0001
	branch Test(NE,Z) l0C00_0000
	// succ:  l0C00_0002 l0C00_0000
l0C00_0002:
	return
	// succ:  fn0C00_0000_exit
fn0C00_0000_exit:
";
            Assert.AreEqual(sExp, sw.ToString());
        }

        private void WriteProcedures(IList<Procedure> procs, StringWriter sw)
        {
            foreach (Procedure proc in procs)
            {
                proc.Write(false, false, sw);
                sw.WriteLine();
            }
        }
    }
}
