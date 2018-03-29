#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class SsaMutatorTests
    {
        private MockRepository mr;
        private FakeArchitecture arch;
        private ProgramDataFlow flow;
        private HashSet<RegisterStorage> implicitRegs;
        private IImportResolver imp;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            this.flow = new ProgramDataFlow();
            this.implicitRegs = new HashSet<RegisterStorage>();
            this.imp = mr.Stub<IImportResolver>();
            this.arch = new FakeArchitecture();
            imp.Replay();
        }

        private SsaTransform BuildSsa(Action<ProcedureBuilder>builder)
        {
            var m = new ProcedureBuilder(arch);
            builder(m);
            var gr = m.Procedure.CreateBlockDominatorGraph();
            var sst = new SsaTransform(flow, m.Procedure, imp, gr, implicitRegs);
            return sst;
        }

        private void AssertEqual(string sExpected, SsaState ssa)
        {
            var sw = new StringWriter();
            ssa.Write(sw);
            ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        [Test]
        public void SsamCallBypass()
        {
            var sst = BuildSsa(m =>
            {
                var a7 = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var a1 = m.Reg32("a1", 0x09);
                var a3 = m.Reg32("a3", 0x0B);
                m.Assign(a7, m.Frame.FramePointer);
                m.Assign(a1, m.Mem32(m.IAdd(a3, 8)));
                m.Call(a1, 4);
                m.Return();
            });
            var ssam = new SsaMutator(sst.SsaState);
            var block = sst.SsaState.Procedure.EntryBlock.Succ[0];
            var stmCall = block.Statements[2];
            var call = (CallInstruction)stmCall.Instruction;

            ssam.AdjustRegisterAfterCall(stmCall, call, arch.StackRegister, 0);
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_1 = fp
r63_1: orig: r63
    def:  r63_1 = fp
    uses: call a1_4 (retsize: 4;)	uses: a1_4,a3,r63_1	defs: a1_6,a3_7
          r63_5 = r63_1
a3:a3
    def:  def a3
    uses: a1_4 = Mem0[a3 + 0x00000008:word32]
          call a1_4 (retsize: 4;)	uses: a1_4,a3,r63_1	defs: a1_6,a3_7
Mem0:Global memory
    def:  def Mem0
    uses: a1_4 = Mem0[a3 + 0x00000008:word32]
a1_4: orig: a1
    def:  a1_4 = Mem0[a3 + 0x00000008:word32]
    uses: call a1_4 (retsize: 4;)	uses: a1_4,a3,r63_1	defs: a1_6,a3_7
          call a1_4 (retsize: 4;)	uses: a1_4,a3,r63_1	defs: a1_6,a3_7
r63_5: orig: r63
    def:  r63_5 = r63_1
a1_6: orig: a1
    def:  call a1_4 (retsize: 4;)	uses: a1_4,a3,r63_1	defs: a1_6,a3_7
a3_7: orig: a3
    def:  call a1_4 (retsize: 4;)	uses: a1_4,a3,r63_1	defs: a1_6,a3_7
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def fp
	def a3
	def Mem0
	// succ:  l1
l1:
	r63_1 = fp
	a1_4 = Mem0[a3 + 0x00000008:word32]
	call a1_4 (retsize: 4;)
		uses: a1_4,a3,r63_1
		defs: a1_6,a3_7
	r63_5 = r63_1
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion
            AssertEqual(sExp, sst.SsaState);
        }
    }
}
