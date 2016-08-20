#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Analysis;
using System.IO;
using System.Diagnostics;
using Rhino.Mocks;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UnusedOutValuesRemoverTests
    {
        private MockRepository mr;
        private IImportResolver import;
        private DecompilerEventListener eventListener;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.import = mr.Stub<IImportResolver>();
            this.eventListener = mr.Stub<DecompilerEventListener>();
        }

        private void RunTest(string sExp, Program program)
        {
            mr.ReplayAll();

            var dfa = new DataFlowAnalysis(program, import, eventListener);
            var ssts = dfa.RewriteProceduresToSsa();

            var uvr = new UnusedOutValuesRemover(program, ssts, dfa.DataFlow);
            uvr.Transform();

            var sb = new StringWriter();
            foreach (var proc in program.Procedures.Values)
            {
                proc.Write(false, sb);
                sb.WriteLine("===");
            }
            var sActual = sb.ToString();
            if (sExp != sActual)
            {
                Debug.Print(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            mr.VerifyAll();
        }

        [Test]
        public void Uvr_Simple()
        {
            var pb = new ProgramBuilder();
            var _r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);

            pb.Add("main", m =>
            {
                m.Call("foo", 0);
            });
            pb.Add("foo", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                m.Assign(r1, m.LoadDw(m.Word32(0x123400)));
                m.Store(m.Word16(0x123408), r1);
                m.Return();
            });

            pb.BuildProgram();

            var sExp =
            #region Expected
@"// main
// Return size: 0
void main()
main_entry:
	// succ:  l1
l1:
	call foo (retsize: 0;)
		defs: r1_1
main_exit:
===
// foo
// Return size: 0
void foo()
foo_entry:
	def Mem0
	// succ:  l1
l1:
	r1_2 = Mem0[0x00123400:word32]
	Mem3[0x3408:word32] = r1_2
	return
	// succ:  foo_exit
foo_exit:
===
";
            #endregion

            RunTest(sExp, pb.Program);
        }
    }
}
