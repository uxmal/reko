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

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UsedRegisterFinderTests
    {
        private ProgramDataFlow pf;
        private ProgramBuilder progBuilder;

        [SetUp]
        public void Setup()
        {
            this.pf = new ProgramDataFlow();
            this.progBuilder = new ProgramBuilder();
        }

        private static string Expect(string preserved, string trashed, string consts)
        {
            return String.Join(Environment.NewLine, new[] { preserved, trashed, consts });
        }

        private Procedure RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder();
            return RunTest(sExp, pb.Architecture, () =>
            {
                builder(pb);
                progBuilder.Add(pb);
                return pb.Procedure;
            });
        }

        private Procedure RunTest(string sExp, string fnName, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder(fnName);
            return RunTest(sExp, pb.Architecture, () =>
            {
                builder(pb);
                progBuilder.Add(pb);
                return pb.Procedure;
            });
        }

        private Procedure RunTest(string sExp, IProcessorArchitecture arch, Func<Procedure> mkProc)
        {
            var proc = mkProc();
            progBuilder.ResolveUnresolved();
            var project = new Project
            {
                Programs = { this.progBuilder.Program }
            };
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();
            var sst = new SsaTransform2(arch, proc, importResolver, new DataFlow2());
            sst.Transform();
            var vp = new ValuePropagator(arch, sst.SsaState);
            vp.Transform();

            sst.RenameFrameAccesses = true;
            sst.AddUseInstructions = true;
            sst.Transform();

            vp.Transform();

            pf.ProcedureFlows2[proc] = new ProcedureFlow2();
            var urf = new UsedRegisterFinder(
                arch, 
                pf,
                new[] { sst },
                NullDecompilerEventListener.Instance);
            var flow = urf.Compute(sst.SsaState);
            var sw = new StringWriter();
            sw.Write("Used: ");
            sw.WriteLine(string.Join(",", flow.Used.OrderBy(p => p.Key.ToString())));
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                proc.Dump(true);
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            return proc;
        }

        [Test]
        public void UrfSimple()
        {
            var sExp =
@"Used: [r1, 32]
";
            RunTest(sExp, m =>
            {
                var r1 = m.Register("r1");
                m.Store(m.Word32(0x2000), r1);
                m.Return();
            });
        }
    }
}
