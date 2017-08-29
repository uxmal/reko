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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Code;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    //[Ignore(Categories.WorkInProgress)]
    public class TrashedRegisterFinder2Tests
    {
        private MockRepository mr;
        private ProgramBuilder progBuilder;
        private ExternalProcedure fnExit;
        private IPlatform platform;
        private IImportResolver importResolver;
        private Program program;
        private ProgramDataFlow dataFlow;
        private StringBuilder sbExpected;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.platform = mr.Stub<IPlatform>();
            this.importResolver = mr.Stub<IImportResolver>();
            importResolver.Replay();

            this.sbExpected = new StringBuilder();
            this.progBuilder = new ProgramBuilder();
            this.fnExit = new ExternalProcedure(
                "exit",
                FunctionType.Action(new Identifier("code", PrimitiveType.Int32, new StackArgumentStorage(4, PrimitiveType.Int32))),
                new ProcedureCharacteristics
                {
                    Terminates = true,
                });
            this.fnExit.Signature.ReturnAddressOnStack = 4;
        }

        private void Expect(string fnName, string preserved, string trashed, string consts)
        {
            fnName = string.Format("== {0} ====", fnName);
            sbExpected.AppendLine(String.Join(Environment.NewLine, new[] { fnName, preserved, trashed, consts }));
        }

        private void Given_PlatformTrashedRegisters(params RegisterStorage[] regs)
        {
            platform.Stub(p => p.CreateTrashedRegisters()).Return(regs.ToHashSet());
        }

        private void AddProcedure(string fnName, Action<ProcedureBuilder> builder)
        {
            progBuilder.Add(fnName, builder);
        }

        public void RunTest()
        {
            progBuilder.ResolveUnresolved();
            progBuilder.Program.Platform = platform;
            mr.ReplayAll();

            this.program = progBuilder.Program;
            this.dataFlow = new ProgramDataFlow(program);
            var sscf = new SccFinder<Procedure>(new ProcedureGraph(program), ProcessScc);
            foreach (var procedure in program.Procedures.Values)
            {
                sscf.Find(procedure);
            }
            var trf = new TrashedRegisterFinder(program, program.Procedures.Values, dataFlow, NullDecompilerEventListener.Instance);
            trf.Compute();

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
            var sccSet = scc.ToHashSet();
            foreach (var proc in scc)
            {
                var sst = new SsaTransform(
                    program,
                    proc,
                    sccSet,
                    importResolver,
                    dataFlow);
                sst.Transform();
                sst.AddUsesToExitBlock();
                var vp = new ValuePropagator(program.Architecture, sst.SsaState, NullDecompilerEventListener.Instance);
                vp.Transform();
            }
        }


    }
}
