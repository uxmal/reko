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
	esp = fp0 - 0x00000004
	store(Mem0[fp0 - 0x00000004:word32]) = ebp
	ebp = fp0 - 0x00000004
	eax = Mem0[fp0 + 0x00000004:word32]
	ebp = Mem0[fp0 - 0x00000004:word32]
	esp = fp0
	return
	// succ:  main_exit
main_exit:
";
            RunTest(sExp);
        }
    }
}
