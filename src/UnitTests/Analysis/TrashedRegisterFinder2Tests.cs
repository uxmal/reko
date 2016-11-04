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
using Reko.Core.Expressions;
using Reko.Core.Serialization;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class TrashedRegisterFinder2Tests
    {
        private MockRepository mr;
        private ProgramBuilder progBuilder;
        private ExternalProcedure fnExit;
        private IPlatform platform;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.platform = mr.Stub<IPlatform>();
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

        private static string Expect(string preserved, string trashed, string consts)
        {
            return String.Join(Environment.NewLine, new[] { preserved, trashed, consts });
        }

        private void Given_PlatformTrashedRegisters(params RegisterStorage[] regs)
        {
            platform.Stub(p => p.CreateTrashedRegisters()).Return(regs.ToHashSet());
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
            progBuilder.Program.Platform = platform;
            mr.ReplayAll();
           
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();

            var program = progBuilder.Program;
            var dataFlow = new ProgramDataFlow();
            var sst = new SsaTransform(program, proc, importResolver, dataFlow);
            sst.Transform();
            var vp = new ValuePropagator(arch, sst.SsaState);
            vp.Transform();

            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.RemoveDeadSsaIdentifiers();

            vp.Transform();

            var trf = new TrashedRegisterFinder2(
                arch, 
                dataFlow,
                new[] { sst },
                NullDecompilerEventListener.Instance);
            var flow = trf.Compute(sst.SsaState);
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
                proc.Dump(true);
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            return proc;
        }

        [Test]
        public void TrfSimple()
        {
            var sExp =
@"Preserved: 
Trashed: r1
Constants: r1:0x0000002A
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
@"Preserved: r1,r63
Trashed: 
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
@"Preserved: r63
Trashed: ds
Constants: ds:0x0C00
";

            RunTest(sExp, m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var ds = m.Reg16("ds", 10);
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
@"Preserved: 
Trashed: cl,cx
";
            RunTest(sExp, m =>
            {
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
                var cl = m.Frame.EnsureRegister(new RegisterStorage("cl", 9, 0, PrimitiveType.Byte));
                var cx = m.Frame.EnsureRegister(new RegisterStorage("cx", 1, 0, PrimitiveType.Byte));
                m.BranchIf(m.Eq0(ax), "zero");

                m.Assign(cl, 0);
                m.Assign(cx, m.Dpb(cx, cl, 0));
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
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
                var cl = m.Frame.EnsureRegister(new RegisterStorage("cl", 9, 0, PrimitiveType.Byte));
                var cx = m.Frame.EnsureRegister(new RegisterStorage("cx", 1, 0, PrimitiveType.Byte));
                m.BranchIf(m.Eq0(ax), "zero");
                m.Assign(cl, 0);
                m.Assign(cx, m.Dpb(cx, cl, 0));
                m.Goto("done");

                m.Label("zero");
                m.Assign(cl, 0);
                m.Assign(cx, m.Dpb(cx, cl, 0));
                
                m.Label("done");
                m.Return();
            });
        }

        [Test(Description="Tests propagation between caller and callee.")]
        [Category(Categories.UnitTests)]
        public void TrfSubroutine_WithRegisterParameters()
        {

            var sExp1 = Expect(
                "Preserved: ",
                "Trashed: r1", "");

            // Subroutine does a small calculation in registers
            RunTest(sExp1, "Addition", m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                Given_PlatformTrashedRegisters();
                m.Assign(r1, m.IAdd(r1, r2));
                m.Return();
            });


            var sExp2 = Expect(
                "Preserved: ",
                "Trashed: r1,r2",
                "");

            RunTest(sExp2, m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Assign(r2, m.LoadDw(m.IAdd(r1, 4)));
                m.Assign(r1, m.LoadDw(m.IAdd(r1, 8)));
                m.Call("Addition", 4);
                m.Store(m.Word32(0x123000), r1);
                m.Return();
            });
        }

        [Test(Description = "Tests detection of trashed variables in the presence of recursion")]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void TrfRecursion()
        {
            var sExp1 = Expect(
                "Preserved: r2",
                "Trashed: r1",
                "");
            RunTest(sExp1, "fact", m =>
            {
                var fp = m.Frame.FramePointer;
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Store(m.ISub(fp, 4), r2);     // save r2
                m.BranchIf(m.Le(r2, 2), "m2Base");

                m.Label("m1Recursive");
                m.Assign(r2, m.ISub(r2, 1));
                m.Call("fact", 0);
                m.Assign(r2, m.LoadDw(m.ISub(fp, 4)));
                m.Assign(r1, m.IMul(r1, r2));   // r1 clobbered as it is the return value.
                m.Goto("m3Done");

                m.Label("m2Base");  // Base case just returns 1.
                m.Assign(r1, 1);

                m.Label("m3Done");
                m.Assign(r2, m.LoadDw(m.ISub(fp, 4)));    // restore r2
                m.Return();
            });
        }

        [Test(Description = "Test that functions that don't return don't affect register state")]
        public void TrfNonReturningProcedure()
        {
            var sExp = Expect(
                "Preserved: ",
                "Trashed: ",
                "");
            RunTest(sExp, "callExit", m =>
            {
                var sp = m.Frame.EnsureIdentifier(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                m.Label("m1");
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.LoadDw(m.IAdd(sp, 4)));
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r1);
                m.Call(fnExit, 4);
                // No return, so registers are not affected.
            });
        }

        [Test(Description = "Only registers modified on paths that reach the exit affect register state")]
        public void TrfBranchedNonReturningProcedure()
        {
            var sExp = Expect("Preserved: r63", "Trashed: r1", "");
            RunTest(sExp, "callExitBranch", m =>
            {
                var sp = m.Frame.EnsureIdentifier(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Label("m1");
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.LoadDw(m.IAdd(sp, 4)));
                m.BranchIf(m.Eq0(r1), "m3return");

                m.Label("m2exit");
                m.Assign(r2, 3);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r2);
                m.Call(fnExit, 4);
                m.ExitThread();

                m.Label("m3return");
                m.Return();
            });
        }

        [Test(Description="Respect user-provided signatures")]
        public void TrfUserSignature()
        {
            var sExp = Expect("Preserved: ", "Trashed: r1", "");
            RunTest(sExp, "fnSig", m =>
            {
                var sp = m.Frame.EnsureIdentifier(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var r3 = m.Reg32("r3", 3);
                m.Assign(sp, m.Frame.FramePointer);
                m.Store(m.ISub(sp, 4), r3);
                m.Assign(r2, m.LoadDw(m.Word32(0x123400)));
                m.Assign(r3, m.LoadDw(m.ISub(sp, 4)));
                m.Return();

                m.Procedure.Signature = FunctionType.Func(
                    new Identifier("", PrimitiveType.Word32, r1.Storage),
                    new Identifier("arg1", PrimitiveType.Word32, r1.Storage));
                });
        }

        [Test(Description = "Exercises a self-recursive function")]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void TrfRecursive()
        {
            var sExp = Expect("@@@", "@@@", "@@@" );
            RunTest(sExp, "fnSig", m =>
            {
                var sp = m.Frame.EnsureIdentifier(m.Architecture.StackRegister);
                Given_PlatformTrashedRegisters((RegisterStorage)sp.Storage);

                var r1 = m.Reg32("r1", 1);


                m.Assign(sp, m.Frame.FramePointer); // establish frame
                m.Assign(sp, m.ISub(sp, 4));        // preserve r1
                m.Store(sp, r1);
                m.BranchIf(m.Eq0(r1), "m3");

                m.Label("m2");
                m.Store(m.Word32(0x123400), r1);    // do something stupid
                m.Assign(r1, m.ISub(r1, 1));
                m.Call("fnSig", 0);                 // recurse

                m.Label("m3");
                m.Assign(r1, m.LoadDw(sp));
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
        }
    }
}
