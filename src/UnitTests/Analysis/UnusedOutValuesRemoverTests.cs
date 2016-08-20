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
                sb.Write("===");
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
            pb.Add("main", m =>
            {
                m.Call("foo", 0);
                m.Return();
            });
            pb.Add("foo", m =>
            {
                var r1 = m.Register(1);
                m.Assign(r1, m.LoadDw(m.Word32(0x123400)));
                m.Store(m.Word16(0x123408), r1);
                m.Return();
            });
            var sExp = "@@@";
            RunTest(sExp, pb.Program);
        }
    }
}
