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
using Decompiler.Core.Lib;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
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
    public class TrashedRegisterFinder2Tests
    {
        private ProgramDataFlow pf;
        private ProgramBuilder progBuilder;

        [SetUp]
        public void Setup()
        {
            this.pf = new ProgramDataFlow();
            this.progBuilder = new ProgramBuilder();
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

            var ssa = new SsaTransform(pf, proc, proc.CreateBlockDominatorGraph());
            var vp = new ValuePropagator(ssa.SsaState.Identifiers, proc);
            vp.Transform();

            ssa.RenameFrameAccesses = true;
            ssa.AddUseInstructions = true;
            ssa.Transform();

            vp.Transform();

            var trf = new TrashedRegisterFinder2(
                arch, 
                pf,
                proc, 
                ssa.SsaState.Identifiers,
                NullDecompilerEventListener.Instance);
            var flow = trf.Compute();
            var sw = new StringWriter();
            sw.Write("Preserved: ");
            sw.WriteLine(string.Join(",", flow.Preserved.OrderBy(p => p.ToString())));
            sw.Write("Trashed: ");
            sw.WriteLine(string.Join(",", flow.Trashed.OrderBy(p => p.ToString())));
            if (flow.Constants.Count > 0)
            {
                sw.Write("Constants: ");
                sw.WriteLine(string.Join(
                    ",",
                    flow.Constants
                        .OrderBy(kv => kv.Key.ToString())
                        .Select(kv => string.Format(
                            "{0}:{1}", kv.Key, kv.Value))));
            }
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                proc.Dump(true, false);
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            pf.ProcedureFlows2.Add(proc, flow);
            return proc;
        }

        [Test]
        public void TrfSimple()
        {
            var sExp =
@"Preserved: 
Trashed: r1
";
            RunTest(sExp, m =>
            {
                var r1 = m.Register("r1");
                m.Assign(r1, m.Word32(42));
                m.Return();
            });
        }

        [Test(Description = "Tests that registers pushed on stack are preserved")]
        public void TrfStackPreserved()
        {
            var sExp =
@"Preserved: fp,r1,r63
Trashed: Global memory,Local -0004
";
            RunTest(sExp, m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var r1 = m.Register("r1");
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r1);        // push r1

                m.Assign(r1, m.LoadDw(m.Word32(0x123400)));
                m.Store(m.Word32(0x123400), r1);

                m.Assign(r1, m.LoadDw(sp)); // pop r1
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
        }

        [Test(Description = "Tests that constants are discovered")]
        public void TrfConstants()
        {
            var sExp =
@"Preserved: fp,r63
Trashed: ds,Local -0002
Constants: ds:0x0C00,Local -0002:0x0C00
";

            RunTest(sExp, m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var ds = m.Reg16("ds");
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 2));
                m.Store(sp, m.Word16(0x0C00));
                m.Assign(ds, m.LoadW(sp));
                m.Assign(sp, m.IAdd(sp, 2));
                m.Return();
            });
        }

        [Test(Description = "Constant in one branch, not constant in the other")]
        public void TrfConstNonConst()
        {
            var sExp =
@"Preserved: ax
Trashed: cl,cx
";
            RunTest(sExp, m =>
            {
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, PrimitiveType.Word16));
                var cl = m.Frame.EnsureRegister(new RegisterStorage("cl", 9, PrimitiveType.Byte));
                var cx = m.Frame.EnsureRegister(new RegisterStorage("cx", 1, PrimitiveType.Byte));
                m.BranchIf(m.Eq0(ax), "zero");

                m.Assign(cl, 0);
                m.Assign(cx, m.Dpb(cx, cl, 0, 8));
                m.Return();

                m.Label("zero");
                m.Assign(cx, m.LoadW(ax));
                m.Return();
            });
        }

        [Test(Description = "Same constant in both branches")]
        [Ignore("Allowing this in Phi functions broke lots of unit tests.")]
        public void TrfConstConst()
        {
            var sExp =
@"Preserved: ax
Trashed: cl,cx
Constants: cl:0x00
";
            RunTest(sExp, m =>
            {
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, PrimitiveType.Word16));
                var cl = m.Frame.EnsureRegister(new RegisterStorage("cl", 9, PrimitiveType.Byte));
                var cx = m.Frame.EnsureRegister(new RegisterStorage("cx", 1, PrimitiveType.Byte));
                m.BranchIf(m.Eq0(ax), "zero");
                m.Assign(cl, 0);
                m.Assign(cx, m.Dpb(cx, cl, 0, 8));
                m.Jump("done");

                m.Label("zero");
                m.Assign(cl, 0);
                m.Assign(cx, m.Dpb(cx, cl, 0, 8));
                
                m.Label("done");
                m.Return();
            });
        }

        [Test(Description="Tests propagation between caller and callee.")]
        public void TrfSubroutine_WithRegisterParameters()
        {
            var sExp1 = "Preserved: r2\r\nTrashed: r1\r\n";

            // Subroutine does a small calculation in registers
            RunTest(sExp1, "Addition", m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Assign(r1, m.IAdd(r1, r2));
                m.Return();
            });

            var sExp2 = "Preserved: \r\nTrashed: Global memory,r1,r2\r\n";

            RunTest(sExp2, m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Assign(r2 ,m.LoadDw(m.IAdd(r1, 4)));
                m.Assign(r1, m.LoadDw(m.IAdd(r1, 8)));
                m.Call("Addition");
                m.Store(m.Word32(0x123000), r1);
                m.Return();
            });
        }
    }
}
