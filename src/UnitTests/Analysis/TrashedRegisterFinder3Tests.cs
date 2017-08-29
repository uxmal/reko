#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Services;
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
    public class TrashedRegisterFinder3Tests
    {
        private MockRepository mr;
        private ProgramBuilder builder;
        private Program program;
        private IImportResolver importResolver;
        private ProgramDataFlow dataFlow;
        private StringBuilder sbExpected;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.builder = new ProgramBuilder();
            this.importResolver = mr.Stub<IImportResolver>();
            this.sbExpected = new StringBuilder();
        }

        private void RunTest()
        {
            mr.ReplayAll();

            this.program = builder.BuildProgram();
            builder.BuildCallgraph();
            this.dataFlow = new ProgramDataFlow(program);
            var sscf = new SccFinder<Procedure>(new ProcedureGraph(program), ProcessScc);
            foreach (var procedure in program.Procedures.Values)
            {
                sscf.Find(procedure);
            }
            var sbActual = new StringBuilder();
            var sw = new StringWriter();
            foreach (var procedure in program.Procedures.Values)
            {
                var flow = dataFlow[procedure];
                sw.WriteLine("== {0} ====", procedure.Name);
                sw.Write("Preserved: ");
                sw.WriteLine(string.Join(",", flow.Preserved.OrderBy(p => p.ToString())));
                sw.Write("Trashed: ");
                sw.WriteLine(string.Join(",", flow.Trashed.OrderBy(p => p.ToString())));
                if (flow.Constants.Count > 0)
                {
                    sw.Write("Constants: ");
                    sw.Write(string.Join(
                        ",",
                        flow.Constants
                            .OrderBy(kv => kv.Key.ToString())
                            .Select(kv => string.Format(
                                "{0}:{1}", kv.Key, kv.Value))));
                }
                sw.WriteLine();
            }
            var sExp = sbExpected.ToString();
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                foreach (var proc in program.Procedures.Values)
                {
                    Debug.Print("------");
                    proc.Dump(true);
                }
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void ProcessScc(IList<Procedure> scc)
        {
            var procSet = scc.ToHashSet();
            var sstSet = new HashSet<SsaTransform>();
            foreach (var proc in scc)
            {
                var sst = new SsaTransform(
                    program,
                    proc,
                    procSet,
                    importResolver,
                    dataFlow);
                sst.Transform();
                sst.AddUsesToExitBlock();
                var vp = new ValuePropagator(program.Architecture, sst.SsaState, NullDecompilerEventListener.Instance);
                vp.Transform();
                sstSet.Add(sst);
            }

            var trf = new TrashedRegisterFinder3(
                program.Architecture,
                dataFlow,
                program.CallGraph,
                sstSet,
                NullDecompilerEventListener.Instance);
            trf.Compute();
        }

        private ProcedureFlow FlowOf(string procName)
        {
            var proc = program.Procedures.Values
                .Where(p => p.Name == procName)
                .Single();
            return dataFlow[proc];
        }

        private void Expect(string fnName, string preserved, string trashed, string consts)
        {
            fnName = string.Format("== {0} ====", fnName);
            sbExpected.AppendLine(String.Join(Environment.NewLine, new[] { fnName, preserved, trashed, consts }));
        }

        [Test]
        public void Trf3_assign()
        {
            builder.Add("main", m =>
            {
                var a = m.Reg32("a", 1);
                m.Assign(a, 0);
                m.Return();
            });

            Expect("main", "Preserved: ", "Trashed: ", "Constants: a:0x00000000");
            RunTest();
        }
    }
}
