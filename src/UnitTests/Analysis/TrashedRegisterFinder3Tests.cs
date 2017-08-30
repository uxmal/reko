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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
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
        private IPlatform platform;
        private ProgramBuilder builder;
        private Program program;
        private IImportResolver importResolver;
        private ProgramDataFlow dataFlow;
        private StringBuilder sbExpected;
        private ExternalProcedure fnExit;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.platform = mr.Stub<IPlatform>();
            this.builder = new ProgramBuilder();
            this.importResolver = mr.Stub<IImportResolver>();
            this.sbExpected = new StringBuilder();
            this.fnExit = new ExternalProcedure(
                "exit",
                FunctionType.Action(new Identifier("code", PrimitiveType.Int32, new StackArgumentStorage(4, PrimitiveType.Int32))),
                new ProcedureCharacteristics
                {
                    Terminates = true,
                });
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

        private void Given_PlatformTrashedRegisters(params RegisterStorage[] regs)
        {
            platform.Stub(p => p.CreateTrashedRegisters()).Return(regs.ToHashSet());
        }

        [Test]
        public void Trf3_assign()
        {
            builder.Add("main", m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var a = m.Reg32("a", 1);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(a, 0);
                m.Return();
            });

            Expect("main", "Preserved: r63", "Trashed: a", "Constants: a:0x00000000");
            RunTest();
        }

        [Test]
        public void TrfSimple()
        {
            Expect("TrfSimple", "Preserved: r63", "Trashed: r1", "Constants: r1:0x0000002A");
            builder.Add("TrfSimple", m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var r1 = m.Register("r1");
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.Word32(42));
                m.Return();
            });
            RunTest();
        }

        [Test(Description = "Tests that registers pushed on stack are preserved")]
        public void TrfStackPreserved()
        {
            Expect(
                "TrfStackPreserved",
                "Preserved: r1,r63",
                "Trashed: ",
                "");
            builder.Add("TrfStackPreserved", m =>
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
            RunTest();
        }

        [Test(Description = "Tests that constants are discovered")]
        public void TrfConstants()
        {
            Expect("TrfConstants", "Preserved: r63", "Trashed: ds", "Constants: ds:0x0C00");
            builder.Add("TrfConstants", m =>
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
            RunTest();
        }

        [Test(Description = "Constant in one branch, not constant in the other")]
        public void TrfConstNonConst()
        {
            Expect("TrfConstNonConst", "Preserved: r63", "Trashed: cl,cx", "");
            builder.Add("TrfConstNonConst", m =>
            {
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
                var cl = m.Frame.EnsureRegister(new RegisterStorage("cl", 1, 0, PrimitiveType.Byte));
                var cx = m.Frame.EnsureRegister(new RegisterStorage("cx", 1, 0, PrimitiveType.Word16));
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);

                m.Assign(sp, m.Frame.FramePointer);
                m.BranchIf(m.Eq0(ax), "zero");

                m.Assign(cl, 0);
                m.Assign(cx, m.Dpb(cx, cl, 0));
                m.Return();

                m.Label("zero");
                m.Assign(cx, m.LoadW(ax));
                m.Return();
            });
            RunTest();
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
            builder.Add(sExp, m =>
            {
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
                var cl = m.Frame.EnsureRegister(new RegisterStorage("cl", 1, 0, PrimitiveType.Byte));
                var cx = m.Frame.EnsureRegister(new RegisterStorage("cx", 1, 0, PrimitiveType.Word16));
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
            RunTest();
        }

        [Test(Description = "Tests propagation between caller and callee.")]
        [Category(Categories.UnitTests)]
        public void TrfSubroutine_WithRegisterParameters()
        {
            Given_PlatformTrashedRegisters();
            Expect(
                "Addition",
                "Preserved: r63",
                "Trashed: r1",
                "");

            // Subroutine does a small calculation in registers
            builder.Add("Addition", m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Assign(
                    m.Frame.EnsureRegister(m.Architecture.StackRegister),
                    m.Frame.FramePointer);
                m.Assign(r1, m.IAdd(r1, r2));
                m.Return();
            });

            Expect(
                "main",
                "Preserved: r63",
                "Trashed: r1,r2",
                "");

            builder.Add("main", m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);

                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r2, m.LoadDw(m.IAdd(r1, 4)));
                m.Assign(r1, m.LoadDw(m.IAdd(r1, 8)));
                m.Call("Addition", 4);
                m.Store(m.Word32(0x123000), r1);
                m.Return();
            });

            RunTest();
        }

        [Test(Description = "Tests detection of trashed variables in the presence of recursion")]
        public void TrfRecursion()
        {
            Expect(
                "fact",
                "Preserved: r2,r63",
                "Trashed: r1",
                "Constants: r1:<invalid>");
            builder.Add("fact", m =>
            {
                Given_PlatformTrashedRegisters();

                var fp = m.Frame.FramePointer;
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);

                m.Assign(sp, fp);
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
            RunTest();
        }

        [Test(Description = "Test that functions that don't return don't affect register state")]
        public void TrfNonReturningProcedure()
        {
            Expect(
                "callExit",
                "Preserved: ",
                "Trashed: ",
                "");
            builder.Add("callExit", m =>
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
            RunTest();
        }

        [Test(Description = "Only registers modified on paths that reach the exit affect register state")]
        public void TrfBranchedNonReturningProcedure()
        {
            builder.Add("callExitBranch", m =>
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
                m.ExitThread();         // Never reaches end.

                m.Label("m3return");
                m.Return();
            });
            Expect("callExitBranch", "Preserved: r63", "Trashed: r1", "");
            RunTest();
        }

        [Test(Description = "Respect user-provided signatures")]
        public void TrfUserSignature()
        {
            builder.Add("fnSig", m =>
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
            Expect("fnSig", "Preserved: ", "Trashed: r1", "");
            RunTest();
        }

        [Test(Description = "Exercises a self-recursive function")]
        public void TrfRecursive()
        {
            Expect("fnSig", "Preserved: r1,r63", "Trashed: ", "");
            builder.Add("fnSig", m =>
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
            RunTest();
        }

        [Test]
        public void TrfFpuReturn()
        {
            Expect(
                "TrfFpuReturn",
                "Preserved: r63",
                "Trashed: Top",
                "Constants: Top:0xFF");

            builder.Add("TrfFpuReturn", m =>
            {
                var sp = m.Frame.EnsureIdentifier(m.Architecture.StackRegister);
                var ST = new MemoryIdentifier("ST", PrimitiveType.Pointer32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));
                var Top = m.Frame.EnsureRegister(new RegisterStorage("Top", 76, 0, PrimitiveType.Byte));

                m.Assign(sp, m.Frame.FramePointer); // establish frame
                m.Assign(Top, 0);
                m.Assign(Top, m.ISub(Top, 1));
                m.Store(ST, Top, Constant.Real64(2.0));
                m.Return();
            });
            RunTest();
        }

        [Test]
        public void TrfFpuReturnTwoValues()
        {
            Expect(
                "TrfFpuReturnTwoValues",
                "Preserved: r63",
                "Trashed: Top",
                "Constants: Top:0xFE");

            builder.Add("TrfFpuReturnTwoValues", m =>
            {
                var sp = m.Frame.EnsureIdentifier(m.Architecture.StackRegister);
                var ST = new MemoryIdentifier("ST", PrimitiveType.Pointer32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));
                var Top = m.Frame.EnsureRegister(new RegisterStorage("Top", 76, 0, PrimitiveType.Byte));


                m.Assign(sp, m.Frame.FramePointer); // establish frame
                m.Assign(Top, 0);
                m.Assign(Top, m.ISub(Top, 1));
                m.Store(ST, Top, Constant.Real64(2.0));
                m.Assign(Top, m.ISub(Top, 1));
                m.Store(ST, Top, Constant.Real64(1.0));
                m.Return();
            });
            RunTest();
        }

        [Test(Description = "Pops three values off FPU stack and places one back.")]
        public void TrfFpuMultiplyAdd()
        {
            Expect(
                "TrfFpuMultiplyAdd",
                "Preserved: r63",
                "Trashed: Top",
                "Constants: Top:0x02");
            builder.Add("TrfFpuMultiplyAdd", m =>
            {
                var sp = m.Frame.EnsureIdentifier(m.Architecture.StackRegister);
                var ST = new MemoryIdentifier("ST", PrimitiveType.Pointer32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));
                var Top = m.Frame.EnsureRegister(new RegisterStorage("Top", 76, 0, PrimitiveType.Byte));
                var dt = PrimitiveType.Real64;

                m.Assign(sp, m.Frame.FramePointer); // establish frame
                m.Assign(Top, 0);
                m.Store(ST, m.IAdd(Top, 1), m.FAdd(
                    m.Load(ST, dt, m.IAdd(Top, 1)),
                    m.Load(ST, dt, Top)));
                m.Assign(Top, m.IAdd(Top, 1));
                m.Store(ST, m.IAdd(Top, 1), m.FAdd(
                    m.Load(ST, dt, m.IAdd(Top, 1)),
                    m.Load(ST, dt, Top)));
                m.Assign(Top, m.IAdd(Top, 1));

                m.Return();
            });
            RunTest();
        }

        /// <summary>
        /// This code has duplicated procedure tails (which restore the caller's
        /// stack frame. This may cause accuracy problems when computing 
        /// trashed registers.
        /// </summary>
        [Test]
        public void TrfRecursive_duplicate_tails()
        {
            Given_PlatformTrashedRegisters();
            Expect(
                "recursive",
                "Preserved: ebp,esp",
                "Trashed: esp",
                "");
            builder.Add("recursive", m =>
            {
                var ebp = m.Frame.EnsureRegister(new RegisterStorage("ebp", 4, 0, PrimitiveType.Word32));
                var esp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(esp, m.Frame.FramePointer);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
                m.BranchIf(m.LoadB(m.Word32(0x123400)), "m2base_case");

                m.Label("m1recursive");
                m.Call("recursive", 4);
                m.Assign(esp, ebp);
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Goto("m3done");

                m.Label("m2base_case");
                m.Assign(esp, ebp);
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));

                m.Label("m3done");
                m.Return();
            });
            RunTest();
        }

        [Test]
        public void TrfFibonacci()
        {
            Given_PlatformTrashedRegisters();
            Expect(
                "recursive",
                "Preserved: ebp,esp",
                "Trashed: esp",
                "");
            builder.Add("recursive", m =>
            {
                var eax = m.Frame.EnsureRegister(new RegisterStorage("eax", 0, 0, PrimitiveType.Word32));
                var ebp = m.Frame.EnsureRegister(new RegisterStorage("ebp", 4, 0, PrimitiveType.Word32));
                var esp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(esp, m.Frame.FramePointer);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(esp, m.ISub(esp, 4));
                m.BranchIf(m.Lt(m.LoadDw(m.IAdd(ebp, 8)), 2), "m2base_case");

                m.Label("m1recursive");
                m.Assign(eax, m.LoadDw(m.IAdd(ebp, 8)));
                m.Assign(eax, m.ISub(eax, 1));
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, eax);
                m.Call("recursive", 4);
                m.Assign(esp, m.IAdd(esp, 4));
                m.Store(m.ISub(ebp, 4), eax);

                m.Assign(eax, m.LoadDw(m.IAdd(ebp, 8)));
                m.Assign(eax, m.ISub(eax, 2));
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, eax);
                m.Call("recursive", 4);
                m.Assign(esp, m.IAdd(esp, 4));
                m.Assign(eax, m.IAdd(eax, m.LoadDw(m.ISub(ebp, 4))));

                m.Assign(esp, ebp);
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Goto("m3done");

                m.Label("m2base_case");
                m.Assign(eax, 1);
                m.Assign(esp, ebp);
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));

                m.Label("m3done");
                m.Return();
            });
            RunTest();
        }

        [Test(Description = "Tests that phi nodes with constants result in constants.")]
        public void TrfSameConstant()
        {
            Expect(
                "recursive",
                "Preserved: ",
                "Trashed: r1",
                "Constants: r1:0x00000002");
            builder.Add("recursive", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(r1, m.Frame.FramePointer);

                m.BranchIf(r1, "m2_one_way");

                m.Label("m1_other_way");
                m.Assign(r1, 1);
                m.Assign(r1, m.IAdd(r1, 1));
                m.Goto("m3_join");

                m.Label("m2_one_way");
                m.Assign(r1, 2);

                m.Label("m3_join");
                m.Return();
            });
            RunTest();
        }
    }
}
