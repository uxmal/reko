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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Expressions;
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
    [TestFixture]
    [Ignore("Do the subanalyses first")]
    public class DataFlowAnalysisTests2
    {
        private ProgramBuilder pb;
        private void GivenProgram(ProgramBuilder pb)
        {
        }

        private void AssertProgram(string sExp, ProgramBuilder pb)
        {
            var sw = new StringWriter();
            pb.Program.Procedures.Values.First().Write(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual) 
                Debug.WriteLine(sActual);
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void DfaSimple()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m=>
                {
                    var r1 = m.Reg32("r1");
                    var r2 = m.Reg32("r2");
                    m.Assign(r1, m.LoadDw(m.Word32(0x010000)));
                    m.Assign(r2, m.LoadDw(m.Word32(0x010004)));
                    m.Store(m.Word32(0x010008), m.IAdd(r1, r2));
                    m.Return();
                });
            var dfa = new DataFlowAnalysis(pb.BuildProgram(), new FakeDecompilerEventListener());
            dfa.UntangleProcedures2();
            var sExp = @"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	Mem6[0x00010008:word32] = Mem0[0x00010000:word32] + Mem0[0x00010004:word32]
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb);
        }

        [Test]
        public void DfaStackArgs()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.LoadDw(m.IAdd(sp, 4)));
                m.Assign(r2, m.LoadDw(m.IAdd(sp, 8)));
                m.Assign(r1, m.IAdd(r1, r2));
                m.Store(m.Word32(0x010008), r1);
                m.Return();
            });
            var dfa = new DataFlowAnalysis(pb.BuildProgram(), new FakeDecompilerEventListener());
            dfa.UntangleProcedures2();
            var sExp = @"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	Mem9[0x00010008:word32] = Mem0[fp + 0x00000004:word32] + Mem0[fp + 0x00000008:word32]
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb);
        }

        private ProcedureBase GivenFunction(string name, RegisterStorage ret, params object [] args)
        {
            var frame = pb.Program.Architecture.CreateFrame();
            Identifier idRet = null;
            if (ret != null)
            {
                idRet = frame.EnsureRegister(ret);
            }
            List<Identifier> parameters = new List<Identifier>();
            foreach (int offset in args)
            {
                parameters.Add(frame.EnsureStackArgument(offset, pb.Program.Architecture.WordWidth));
            }
            var proc = new ExternalProcedure(name, new ProcedureSignature(
                idRet, 
                parameters.ToArray()));
            return proc;
        }

        [Test]
        public void DfaCallProc()
        {
            pb = new ProgramBuilder();
            pb.Add("test", m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);

                var fooProc = GivenFunction("foo", m.Architecture.GetRegister(1), 4, 8);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, 2);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, 1);
                m.Call(fooProc);
                m.Assign(sp, m.IAdd(sp, 8));
                m.Return();
            });

            var dfa = new DataFlowAnalysis(pb.BuildProgram(), new FakeDecompilerEventListener());
            dfa.UntangleProcedures2();
            var sExp = @"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	Mem9[0x00010008:word32] = Mem0[fp + 0x00000004:word32] + Mem0[fp + 0x00000008:word32]
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb);
        }
    }
}
