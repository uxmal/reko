#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Analysis;
using Decompiler.Evaluation;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    class TrashRegisterFinderUpdateCodeTests
    {
        private ProgramBuilder p;
        private Program prog;
        private ProgramDataFlow flow;
        private IntelArchitecture arch;
        private TrashedRegisterFinder trf;

        [SetUp]
        public void Setup()
        {
            arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            p = new ProgramBuilder();
        }

        private void RunTest(string sExp)
        {
            prog = p.BuildProgram(arch);
            flow = new ProgramDataFlow(prog);
            trf = new TrashedRegisterFinder(prog, prog.Procedures.Values, this.flow, new FakeDecompilerEventListener());
            trf.Compute();
            trf.RewriteBasicBlocks();

            var sw = new StringWriter();
            foreach (var proc in prog.Procedures.Values)
            {
                flow[proc].EmitRegisters(arch, "// Trashed", flow[proc].TrashedRegisters);
                proc.Write(false, sw);
                sw.WriteLine();
            }
            Console.WriteLine(sw);
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void UpdateAllStackReferences()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var esp = m.Frame.EnsureRegister(Registers.esp);
                var ebp = m.Frame.EnsureRegister(Registers.ebp);
                m.Assign(esp, m.Sub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(eax, m.LoadDw(m.Add(ebp, 8)));
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.Add(esp, 4));
                m.Return();
            });

            var sExp = @"// main
void main()
main_entry:
	// succ:  l1
l1:
	esp = fp - 0x00000004
	dwLoc04 = ebp
	ebp = fp - 0x00000004
	eax = dwArg04
	ebp = dwLoc04
	esp = fp
	return
	// succ:  main_exit
main_exit:

";
            RunTest(sExp);
        }

        [Test]
        public void CallToFunction()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var ebx = m.Frame.EnsureRegister(Registers.ebx);
                var esp = m.Frame.EnsureRegister(Registers.esp);
                m.Assign(eax, 0x1234);
                m.Assign(ebx, 0x1234);
                m.Assign(esp, m.Sub(esp, 4));
                m.Store(esp, eax);
                m.Call("foo");
                m.Assign(ebx, eax);
                m.Return();
            });

            p.Add("foo", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                m.Assign(eax, 0x471100);
                m.Return();
            });

            var sExp = @"// main
void main()
main_entry:
	// succ:  l1
l1:
	eax = 0x00001234
	ebx = 0x00001234
	esp = fp - 0x00000004
	dwLoc04 = 0x00001234
	call foo (retsize: 4; depth: 8)
	ebx = 0x00471100
	return
	// succ:  main_exit
main_exit:

// foo
void foo()
foo_entry:
	// succ:  l1
l1:
	eax = 0x00471100
	return
	// succ:  foo_exit
foo_exit:

";
            RunTest(sExp);
        }

        [Test]
        public void DontCopyRegistersUnlessFramepointer()
        {
            p.Add("main", m =>
            {
                var ecx = m.Frame.EnsureRegister(Registers.ecx);
                var esi = m.Frame.EnsureRegister(Registers.esi);
                m.Assign(esi, ecx);
                m.Call("foo");
                m.Assign(esi, m.Add(esi, 1));
                m.Return();
            });

            p.Add("foo", m =>
            {
                var esp = m.Frame.EnsureRegister(Registers.esp);
                var esi = m.Frame.EnsureRegister(Registers.esi);
                var ecx = m.Frame.EnsureRegister(Registers.ecx);

                m.Assign(esp, m.Sub(esp, 4));
                m.Store(esp, esi);
                m.Assign(ecx, m.Sub(ecx, 1));
                m.Assign(esi, m.LoadDw(esp));
                m.Assign(esp, m.Add(esp, 4));
                m.Return();
            });

            var sExp = @"// main
void main()
main_entry:
	// succ:  l1
l1:
	esi = ecx
	call foo (retsize: 4; depth: 4)
	esi = esi + 0x00000001
	return
	// succ:  main_exit
main_exit:

// foo
void foo()
foo_entry:
	// succ:  l1
l1:
	esp = fp - 0x00000004
	dwLoc04 = esi
	ecx = ecx - 0x00000001
	esi = dwLoc04
	esp = fp
	return
	// succ:  foo_exit
foo_exit:

";
            RunTest(sExp);
        }
    }
}
