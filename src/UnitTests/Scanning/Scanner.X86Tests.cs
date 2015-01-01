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

        private void BuildTest16(Action<IntelAssembler> asmProg)
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            BuildTest(new Address(0x0C00, 0x0000), new MsdosPlatform(null, arch), asmProg);
        }

        private void BuildTest32(Action<IntelAssembler> asmProg)
        {
            arch = new IntelArchitecture(ProcessorMode.Protected32);
            BuildTest(new Address(0x00100000), new FakePlatform(null, null), asmProg);
        }

        private void BuildTest(Address addrBase, Platform platform , Action<IntelAssembler> asmProg)
        {
            var entryPoints = new List<EntryPoint>();
            var asm = new IntelAssembler(arch, addrBase, entryPoints);
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
                project,
                new Dictionary<Address, ProcedureSignature>(),
                new ImportResolver(project),
                new FakeDecompilerEventListener());
            scanner.EnqueueEntryPoint(new EntryPoint(addrBase, arch.CreateProcessorState()));
            scanner.ScanImage();
        }

        private void DumpProgram(Scanner scanner)
        {
            var dasm = arch.CreateDisassembler(scanner.Image.CreateLeReader(0));
            foreach (var instr in dasm)
            {
                Console.Out.WriteLine("{0} {1}", instr.Address, instr);
            }
            
            foreach (Procedure proc in scanner.Procedures.Values)
            {
                proc.Write(true, Console.Out);
                Console.Out.WriteLine();
            }
        }

        [Test]
        public void Scanx86NestedProcedures()
        {
            BuildTest16(delegate(IntelAssembler m)
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
            BuildTest16(delegate(IntelAssembler m)
            {
                m.Rep();
                m.Scasw();
                m.Ret();
            });
            var sw = new StringWriter();
            program.Procedures.Values[0].Write(false, sw);
            var sExp = @"// fn0C00_0000
void fn0C00_0000()
fn0C00_0000_entry:
	// succ:  l0C00_0000
l0C00_0000:
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

        /// <summary>
        /// If a procedure p1 makes a call into the body of an existing procedure  p2 (not the entry block p2),
        /// a new procedure p3 should be created.
        /// </summary>
        [Test]
        public void Scanx86JumpIntoProc()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Proc("p1");
                m.Call("p2");
                m.Call("p3");
                m.Mov(m.MemDw(Registers.ebx, 0x123C), m.eax);
                m.Ret();

                m.Proc("p2");
                m.Mov(m.MemDw(Registers.ebx, 0x1234), 4);

                m.Label("p3");
                m.Mov(m.eax, 1);
                m.Cmp(m.MemDw(Registers.ebx, 0x1238), 5);
                m.Jnz("p2_done");
                m.Mov(m.eax, 0);
                m.Label("p2_done");
                m.Ret();
            });
            var sw = new StringWriter();
            var sExp = @"// fn00100000
void fn00100000()
fn00100000_entry:
l00100000:
	call fn00100011 (retsize: 4;)
	call fn0010001B (retsize: 4;)
	Mem0[ebx + 0x0000123C:word32] = eax
	return
fn00100000_exit:

// fn00100011
void fn00100011()
fn00100011_entry:
l00100011:
	Mem0[ebx + 0x00001234:word32] = 0x00000004
    call fn0010001B
    return
fn00100011_exit:

// fn0010001B
// fn0010001B
void fn0010001B()
fn0010001B_entry:
l0010001B:
    eax = 0x00000001
	SCZO = cond(Mem0[ebx + 0x00001238:word32] - 0x00000005)
	branch Test(NE,Z) l0010002E
l00100029:
	eax = 0x00000000
l0010002E:
	return
fn0010001B_exit:

";
            WriteProcedures(scanner.Procedures.Values, sw);
            Console.WriteLine(sw.ToString());
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
