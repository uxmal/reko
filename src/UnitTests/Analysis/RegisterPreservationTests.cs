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
using Reko.UnitTests.Mocks;
using Reko.UnitTests.TestCode;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class RegisterPreservationTests
    {
        private DataFlow2 dataFlow;
        private Program program;
        private IImportResolver importResolver;

        private void AssertProgram(string sExp, IEnumerable<Procedure> procs)
        {
            var sw = new StringWriter();
            foreach (var proc in procs)
                proc.Write(false, sw);
            sw.WriteLine();
            foreach (var de in dataFlow.ProcedureFlows.OrderBy(e => e.Key.Name))
            {
                sw.WriteLine("{0}:", de.Key.Name);
                DumpSet("Preserved: ", de.Value.Preserved, sw);
                DumpSet("Trashed:   ", de.Value.Trashed, sw);
               // DumpMap("Constants: ", de.Value.Constants, sw);
            }

            var sActual = sw.ToString();
            if (sExp != sActual)
                Debug.WriteLine(sActual);
            Assert.AreEqual(sExp, sw.ToString());
        }

        private void DumpSet<T>(string caption, ISet<T> items, StringWriter sw)
        {
            sw.Write("    {0}", caption);
            sw.WriteLine(string.Join(" ", items.Select(e => e.ToString()).OrderBy(e => e)));
        }

        private void DumpMap<K, V>(string caption, IDictionary<K, V> items, StringWriter sw)
        {
            sw.Write("    {0}", caption);
            sw.WriteLine(string.Join(" ", items.Select(e => string.Format("{0}:{1}", e.Key, e.Value)).OrderBy(e => e)));
        }

        public void RunTest(IEnumerable<Procedure> procs)
        {
            importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();
            var flow = new ProgramDataFlow(program);
            var scc = new Dictionary<Procedure, SsaState>();
            foreach (var proc in procs)
            {
                Aliases alias = new Aliases(proc, program.Architecture, flow);
                alias.Transform();

                // Transform the procedure to SSA state. When encountering 'call' instructions,
                // they can be to functions already visited. If so, they have a "ProcedureFlow" 
                // associated with them. If they have not been visited, or are computed destinations
                // (e.g. vtables) they will have no "ProcedureFlow" associated with them yet, in
                // which case the the SSA treats the call as a "hell node".
                var doms = proc.CreateBlockDominatorGraph();
                var sst = new SsaTransform(
                    flow, 
                    proc,
                    importResolver,
                    doms, 
                    program.Platform.CreateImplicitArgumentRegisters());
                sst.AddUseInstructions = true;
                sst.Transform();
                var ssa = sst.SsaState;

                scc.Add(proc, ssa);
            }

            this.dataFlow = new DataFlow2();
            var regp = new RegisterPreservation(scc, dataFlow);
            regp.Compute();
        }

        [Test]
        public void Regp_Simple()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m =>
            {
                var r1 = m.Register(1);
                m.Assign(r1, 3);
                m.Return();
            });
            program = pb.BuildProgram();
            RunTest(program.Procedures.Values);

            var sExp =
            #region Expected
@"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	r1_0 = 0x00000003
	return
	// succ:  test_exit
test_exit:
	use r1_0

test:
    Preserved: 
    Trashed:   r1
";
            #endregion
            AssertProgram(sExp, program.Procedures.Values);
        }

        [Test(Description = "Loading from memory trashes the loaded register but not memory.")]
        public void Regp_Preserve()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m =>
            {
                var r1 = m.Register(1);
                m.Assign(r1, m.LoadDw(m.Word32(0x3000)));
                m.Return();
            });
            program = pb.BuildProgram();
            RunTest(program.Procedures.Values);

            var sExp =
            #region Expected
@"// test
// Return size: 0
void test()
test_entry:
	def Mem0
	// succ:  l1
l1:
	r1_1 = Mem0[0x00003000:word32]
	return
	// succ:  test_exit
test_exit:
	use Mem0
	use r1_1

test:
    Preserved: Global memory
    Trashed:   r1
";
            #endregion
            AssertProgram(sExp, program.Procedures.Values);
        }

        [Test(Description = "Both branches of a phi should be followed.")]
        public void Regp_If_Then()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m =>
            {
                var r1 = m.Register(1);
                m.BranchIf(m.Ge(r1, 0), "m_ge");

                m.Label("m_lt");
                m.Assign(r1, 0);

                m.Label("m_ge");
                m.Return();
            });
            program = pb.BuildProgram();
            RunTest(program.Procedures.Values);

            var sExp =
            #region Expected
@"// test
// Return size: 0
void test()
test_entry:
	def r1
	// succ:  l1
l1:
	branch r1 >= 0x00000000 m_ge
	goto m_lt
	// succ:  m_lt m_ge
m_ge:
	r1_1 = PHI(r1, r1_2)
	return
	// succ:  test_exit
m_lt:
	r1_2 = 0x00000000
	goto m_ge
	// succ:  m_ge
test_exit:
	use r1_1

test:
    Preserved: 
    Trashed:   r1
";
            #endregion
            AssertProgram(sExp, program.Procedures.Values);
        }

        /// <summary>
        /// Looks a little redundant, but some compilers actually emit
        /// code like this.
        /// </summary>
        [Test(Description = "Both branches of a phi should be followed.")]
        public void Regp_Phi_Branches()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Assign(r2, r1);
                m.BranchIf(m.Ge(r1, 0), "m_ge");

                m.Label("m_lt");
                m.Assign(r1, r2);
                m.Goto("m_done");

                m.Label("m_ge");
                m.Assign(r1, r2);

                m.Label("m_done");
                m.Return();
            });
            program = pb.BuildProgram();
            RunTest(program.Procedures.Values);

            var sExp =
            #region Expected
@"// test
// Return size: 0
void test()
test_entry:
	def r1
	// succ:  l1
l1:
	r2_1 = r1
	branch r1 >= 0x00000000 m_ge
	goto m_lt
	// succ:  m_lt m_ge
m_done:
	r1_2 = PHI(r1_3, r1_4)
	return
	// succ:  test_exit
m_ge:
	r1_4 = r2_1
	goto m_done
	// succ:  m_done
m_lt:
	r1_3 = r2_1
	goto m_done
	// succ:  m_done
test_exit:
	use r1_2
	use r2_1

test:
    Preserved: r1
    Trashed:   r2
";
            #endregion
            AssertProgram(sExp, program.Procedures.Values);

        }

        [Test(Description = "While-do loops shouldn't confuse this analysis.")]
        public void Regp_WhileDo()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                var r3 = m.Register(3);
                m.Assign(r2, r1);
                m.Assign(r3, r2);
                m.Assign(r1, 10);
                m.Goto("m_loopHead");

                m.Label("m_loopStatements");
                m.Assign(r1, m.ISub(r1, 1));
                m.Assign(r2, r3);

                m.Label("m_loopHead");
                m.BranchIf(m.Ge(r1, 0), "m_loopStatements");

                m.Label("m_xit");
                m.Assign(r1, r2);
                m.Return();
            });
            program = pb.BuildProgram();
            RunTest(program.Procedures.Values);

            var sExp =
            #region Expected
@"// test
// Return size: 0
void test()
test_entry:
	def r1
	// succ:  l1
l1:
	r2_1 = r1
	r3_2 = r2_1
	r1_3 = 0x0000000A
	// succ:  m_loopHead
m_loopHead:
	r2_4 = PHI(r2_1, r2_8)
	r1_5 = PHI(r1_3, r1_7)
	branch r1_5 >= 0x00000000 m_loopStatements
	goto m_xit
	// succ:  m_xit m_loopStatements
m_loopStatements:
	r1_7 = r1_5 - 0x00000001
	r2_8 = r3_2
	goto m_loopHead
	// succ:  m_loopHead
m_xit:
	r1_6 = r2_4
	return
	// succ:  test_exit
test_exit:
	use r1_6
	use r2_4
	use r3_2

test:
    Preserved: r1
    Trashed:   r2 r3
";
            #endregion
            AssertProgram(sExp, program.Procedures.Values);
        }

        [Test(Description = "Processes a self-recursive program")]
        [Ignore("")]
        public void Regp_Factorial()
        {
            program = Factorial.BuildSample();
            RunTest(program.Procedures.Values.Take(1));

            var sExp =
            #region Expected
@"// fact
// Return size: 0
void fact()
fact_entry:
	def fp
	def r1
	def r3
	// succ:  l1
l1:
	r63_1 = fp
	r2_3 = r1
	r1_4 = 0x00000001
	cc_5 = cond(r2_3 - r1_4)
	branch Test(LE,cc_5) m_done
	// succ:  l2 m_done
l2:
	r63_12 = r63_1 - 0x00000004
	Mem13[r63_12:word32] = r2_3
	r1_14 = r2_3 - r1_4
	call fact (retsize: 0;)
		uses: cc_5,r1_14,r2_3,r3,r63_12
		defs: cc_19,r1_16,r2_17,r3_18,r63_15
	r2_20 = Mem13[r63_15:word32]
	r63_21 = r63_15 + 0x00000004
	r1_22 = r1_16 * r2_20
	// succ:  m_done
m_done:
	cc_7 = PHI(cc_5, cc_19)
	r3_8 = PHI(r3, r3_18)
	r2_9 = PHI(r2_3, r2_20)
	r63_10 = PHI(r63_1, r63_21)
	r1_11 = PHI(r1_4, r1_22)
	return
	// succ:  fact_exit
fact_exit:
	use cc_7
	use fp
	use r1_11
	use r2_9
	use r3_8
	use r63_10

fact:
    Preserved: fp
    Trashed:   Flags r1 r2 r3 r63
";
            #endregion
            AssertProgram(sExp, program.Procedures.Values.Take(1));
        }
    }

}
