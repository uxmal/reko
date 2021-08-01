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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Services;
using Reko.Environments.Msdos;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class Scanner_x86Tests
    {
        private IntelArchitecture arch;
        private Scanner scanner;
        private Program program;
        private ServiceContainer sc;

        private void BuildTest16(Action<X86Assembler> asmProg)
        {
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            BuildTest(Address.SegPtr(0x0C00, 0x0000), new MsdosPlatform(sc, arch), asmProg);
        }

        private void BuildTest32(Action<X86Assembler> asmProg)
        {
            sc = new ServiceContainer();
            arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            BuildTest(Address.Ptr32(0x00100000), new FakePlatform(sc, null), asmProg);
        }

        private void BuildTest(Address addrBase, IPlatform platform, Action<X86Assembler> asmProg)
        {
            var sc = new ServiceContainer();
            var eventListener = new FakeDecompilerEventListener();
            sc.AddService<DecompilerEventListener>(eventListener);
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            var entryPoints = new List<ImageSymbol>();
            var asm = new X86Assembler(arch, addrBase, entryPoints);
            asmProg(asm);

            program = asm.GetImage();
            program.Platform = platform;
            var project = new Project { Programs = { program } };
            scanner = new Scanner(
                program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, eventListener),
                sc);
            scanner.EnqueueImageSymbol(ImageSymbol.Procedure(arch, addrBase), true);
            scanner.ScanImage();
        }

        private void DumpProgram(Scanner scanner)
        {
            var dasm = arch.CreateDisassembler(program.SegmentMap.Segments.Values.First().MemoryArea.CreateLeReader(0));
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
            BuildTest16(m =>
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
            BuildTest16(m =>
            {
                m.Rep();
                m.Scasw();
                m.Ret();
            });
            var sw = new StringWriter();
            program.Procedures.Values[0].Write(false, sw);
            var sExp = @"// fn0C00_0000
// Return size: 2
define fn0C00_0000
fn0C00_0000_entry:
	sp = fp
	Top = 0<i8>
	// succ:  l0C00_0000
l0C00_0000:
	branch cx == 0<16> l0C00_0002
	// succ:  l0C00_0000_1 l0C00_0002
l0C00_0000_1:
	SCZO = cond(ax - Mem0[es:di:word16])
	di = di + 2<i16>
	cx = cx - 1<16>
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
