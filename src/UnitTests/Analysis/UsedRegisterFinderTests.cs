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
            var sst = new SsaTransform2(arch, proc, importResolver, new ProgramDataFlow());
            sst.Transform();
            var vp = new ValuePropagator(arch, sst.SsaState);
            vp.Transform();

            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.RemoveDeadSsaIdentifiers();

            vp.Transform();

            pf.ProcedureFlows[proc] = new ProcedureFlow(proc);
            var urf = new UsedRegisterFinder(
                arch, 
                pf,
                new[] { sst },
                NullDecompilerEventListener.Instance);
            urf.IgnoreUseInstructions = true;
            var flow = urf.Compute(sst.SsaState);
            var sw = new StringWriter();
            sw.Write("Used: ");
            sw.WriteLine(string.Join(",", flow.BitsUsed.OrderBy(p => p.Key.ToString())));
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
        public void UrfRegisterArg()
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

        [Test]
        public void UrfStackArg()
        {
            var sExp = 
@"Used: [Stack +0004, 32]
";
            RunTest(sExp, m =>
            {
                var fp = m.Frame.FramePointer;
                var r1 = m.Reg32("r1", 1);
                m.Assign(r1, m.LoadDw(m.IAdd(fp, 4)));
                m.Store(m.Word32(0x2000), r1);
                m.Return();
            });
        }

        [Test]
        public void UrfCast()
        {
            var sExp =
@"Used: [r1, 16]
";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var tmp = m.Temp(PrimitiveType.Word16, "tmp");
                m.Assign(tmp, m.Cast(PrimitiveType.Word16, r1));
                m.Store(m.Word32(0x2000), tmp);
                m.Return();
            });
        }


        [Test(Description = "Identifiers are not considered used if they only are copied.")]
        public void UrfCopy()
        {
            var sExp =
@"Used: [r1, 0]
";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(r2, r1);
                m.Return();
            });
        }

        [Test]
        public void UrfBranch()
        {
            var sExp = @"Used: [r1, 32]
";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                m.BranchIf(m.Ge(m.LoadB(m.Word32(0x02000)), 4), "mge");
                m.Label("mlt");
                m.Store(m.Word32(0x02004), r1);
                m.Goto("mxit");
                m.Label("mge");
                m.Store(m.Word32(0x02008), m.Cast(PrimitiveType.Word16, r1));
                m.Label("mxit");
                m.Return();
            });
        }

        [Test]
        public void UrfSequence()
        {
            var sExp = @"Used: [r1, 32],[r2, 32]
";

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var r2_r1 = m.Frame.EnsureSequence(r2.Storage, r1.Storage, PrimitiveType.Word64);

                m.Store(m.Word32(0x2000), m.Shr(r2_r1, 2));
                m.Return();
            });
        }
    }
}
