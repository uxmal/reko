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
            this.dataFlow = new ProgramDataFlow(progBuilder.Program);


            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();
            var sst = new SsaTransform(
                progBuilder.Program,
                proc,
                new HashSet<Procedure>(),
                importResolver,
                dataFlow);
            sst.Transform();
            var vp = new ValuePropagator(arch, sst.SsaState, NullDecompilerEventListener.Instance);
            vp.Transform();

            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();

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

                m.Assign(r1, m.Mem32(m.Word32(0x123400)));
                m.Store(m.Word32(0x123400), r1);

                m.Assign(r1, m.Mem32(sp)); // pop r1
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
                m.Assign(ds, m.Mem16(sp));
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
                m.Assign(cx, m.Mem16(ax));
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
        public void TrfSubroutine_WithRegisterParameters()
        {
            var sExp1 = string.Join(Environment.NewLine,new []{"Preserved: ","Trashed: r1",""});

            // Subroutine does a small calculation in registers
            RunTest(sExp1, "Addition", m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Assign(r1, m.IAdd(r1, r2));
                m.Return();
            });

            var sExp2 = string.Join(Environment.NewLine,new []{"Preserved: ","Trashed: r1,r2",""});

            RunTest(sExp2, m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Assign(r2 ,m.Mem32(m.IAdd(r1, 4)));
                m.Assign(r1, m.Mem32(m.IAdd(r1, 8)));
                m.Call("Addition", 4);
                m.Store(m.Word32(0x123000), r1);
                m.Return();
            });
        }
    }
}
