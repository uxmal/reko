#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class TrashedRegisterFinderTests
    {
        private IProcessorArchitecture arch;
        private ProgramBuilder builder;
        private Program program;
        private Mock<IDynamicLinker> dynamicLinker;
        private Mock<IPlatform> platform;
        private ProgramDataFlow dataFlow;
        private StringBuilder sbExpected;
        private ExternalProcedure fnExit;

        [SetUp]
        public void Setup()
        {
            this.platform = new Mock<IPlatform>();
            this.arch = new FakeArchitecture();
            this.builder = new ProgramBuilder(arch);
            this.dynamicLinker = new Mock<IDynamicLinker>();
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
            this.program = builder.BuildProgram();
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
            var procSet = new HashSet<Procedure>(scc);
            var sstSet = new HashSet<SsaTransform>();
            foreach (var proc in scc)
            {
                var sst = new SsaTransform(
                    program,
                    proc,
                    procSet,
                    dynamicLinker.Object,
                    dataFlow);
                sst.Transform();
                sst.AddUsesToExitBlock();
                var vp = new ValuePropagator(
                    program.SegmentMap, 
                    sst.SsaState,
                    program.CallGraph,
                    dynamicLinker.Object,
                    NullDecompilerEventListener.Instance);
                vp.Transform();
                sstSet.Add(sst);
            }

            var trf = new TrashedRegisterFinder(
                program,
                dataFlow,
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
            platform.Setup(p => p.CreateTrashedRegisters())
                .Returns(new HashSet<RegisterStorage>(regs));
        }

        private void Given_Architecture(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.builder = new ProgramBuilder(arch);
        }

        [Test]
        public void Trf3_assign()
        {
            builder.Add("main", m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, 0);
                m.Return();
            });

            Expect("main", "Preserved: r63", "Trashed: r1", "Constants: r1:0x00000000");
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
                m.MStore(sp, r1);        // push r1

                m.Assign(r1, m.Mem32(m.Word32(0x123400)));
                m.MStore(m.Word32(0x123400), r1);

                m.Assign(r1, m.Mem32(sp)); // pop r1
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
            RunTest();
        }

        [Test(Description = "Tests that constants are discovered")]
        public void TrfConstants()
        {
            Given_Architecture(new Reko.Arch.X86.X86ArchitectureReal("x86-real-16"));
            Expect("TrfConstants", "Preserved: sp", "Trashed: ds", "Constants: ds:0x0C00");
            builder.Add("TrfConstants", m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var ds = m.Frame.EnsureRegister(m.Architecture.GetRegister("ds"));
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 2));
                m.MStore(sp, m.Word16(0x0C00));
                m.Assign(ds, m.Mem16(sp));
                m.Assign(sp, m.IAdd(sp, 2));
                m.Return();
            });
            RunTest();
        }

        [Test(Description = "Constant in one branch, not constant in the other")]
        public void TrfConstNonConst()
        {
            Given_Architecture(new Reko.Arch.X86.X86ArchitectureFlat32("x86-protected-32"));
            Expect("TrfConstNonConst", "Preserved: esp", "Trashed: cx", "");
            builder.Add("TrfConstNonConst", m =>
            {
                var ax = m.Frame.EnsureRegister(arch.GetRegister("ax"));
                var cl = m.Frame.EnsureRegister(arch.GetRegister("cl"));
                var cx = m.Frame.EnsureRegister(arch.GetRegister("cx"));
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);

                m.Assign(sp, m.Frame.FramePointer);
                m.BranchIf(m.Eq0(ax), "zero");

                m.Assign(cl, 0);
                m.Assign(cx, m.Dpb(cx, cl, 0));
                m.Return();

                m.Label("zero");
                m.Assign(cx, m.Mem16(ax));
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
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
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
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);

                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r2, m.Mem32(m.IAdd(r1, 4)));
                m.Assign(r1, m.Mem32(m.IAdd(r1, 8)));
                m.Call("Addition", 4);
                m.MStore(m.Word32(0x123000), r1);
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
                "");
            builder.Add("fact", m =>
            {
                Given_PlatformTrashedRegisters();

                var fp = m.Frame.FramePointer;
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);

                m.Assign(sp, fp);
                m.MStore(m.ISub(fp, 4), r2);     // save r2
                m.BranchIf(m.Le(r2, 2), "m2Base");

                m.Label("m1Recursive");
                m.Assign(r2, m.ISub(r2, 1));
                m.Call("fact", 0);
                m.Assign(r2, m.Mem32(m.ISub(fp, 4)));
                m.Assign(r1, m.IMul(r1, r2));   // r1 clobbered as it is the return value.
                m.Goto("m3Done");

                m.Label("m2Base");  // Base case just returns 1.
                m.Assign(r1, 1);

                m.Label("m3Done");
                m.Assign(r2, m.Mem32(m.ISub(fp, 4)));    // restore r2
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
                m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r1);
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
                m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
                m.BranchIf(m.Eq0(r1), "m3return");

                m.Label("m2exit");
                m.Assign(r2, 3);
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r2);
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
                m.MStore(m.ISub(sp, 4), r3);
                m.Assign(r2, m.Mem32(m.Word32(0x123400)));
                m.Assign(r3, m.Mem32(m.ISub(sp, 4)));
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
                m.MStore(sp, r1);
                m.BranchIf(m.Eq0(r1), "m3");

                m.Label("m2");
                m.MStore(m.Word32(0x123400), r1);    // do something stupid
                m.Assign(r1, m.ISub(r1, 1));
                m.Call("fnSig", 0);                 // recurse

                m.Label("m3");
                m.Assign(r1, m.Mem32(sp));
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
                var ST = new MemoryIdentifier("ST", PrimitiveType.Ptr32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));
                var Top = m.Frame.EnsureRegister(new RegisterStorage("Top", 76, 0, PrimitiveType.Byte));

                m.Assign(sp, m.Frame.FramePointer); // establish frame
                m.Assign(Top, 0);
                m.Assign(Top, m.ISub(Top, 1));
                m.MStore(ST, Top, Constant.Real64(2.0));
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
                var ST = new MemoryIdentifier("ST", PrimitiveType.Ptr32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));
                var Top = m.Frame.EnsureRegister(new RegisterStorage("Top", 76, 0, PrimitiveType.Byte));


                m.Assign(sp, m.Frame.FramePointer); // establish frame
                m.Assign(Top, 0);
                m.Assign(Top, m.ISub(Top, 1));
                m.MStore(ST, Top, Constant.Real64(2.0));
                m.Assign(Top, m.ISub(Top, 1));
                m.MStore(ST, Top, Constant.Real64(1.0));
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
                var ST = new MemoryIdentifier("ST", PrimitiveType.Ptr32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));
                var Top = m.Frame.EnsureRegister(new RegisterStorage("Top", 76, 0, PrimitiveType.Byte));
                var dt = PrimitiveType.Real64;

                m.Assign(sp, m.Frame.FramePointer); // establish frame
                m.Assign(Top, 0);
                m.MStore(ST, m.IAdd(Top, 1), m.FAdd(
                    m.Mem(ST, dt, m.IAdd(Top, 1)),
                    m.Mem(ST, dt, Top)));
                m.Assign(Top, m.IAdd(Top, 1));
                m.MStore(ST, m.IAdd(Top, 1), m.FAdd(
                    m.Mem(ST, dt, m.IAdd(Top, 1)),
                    m.Mem(ST, dt, Top)));
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
            Given_Architecture(new Reko.Arch.X86.X86ArchitectureFlat32("x86-protected-32"));
            Given_PlatformTrashedRegisters();
            Expect(
                "recursive",
                "Preserved: ebp,esp",
                "Trashed: ",
                "");
            builder.Add("recursive", m =>
            {
                var ebp = m.Frame.EnsureRegister(arch.GetRegister("ebp"));
                var esp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(esp, m.Frame.FramePointer);
                m.Assign(esp, m.ISub(esp, 4));
                m.MStore(esp, ebp);
                m.Assign(ebp, esp);
                m.BranchIf(m.Mem8(m.Word32(0x123400)), "m2base_case");

                m.Label("m1recursive");
                m.Call("recursive", 4);
                m.Assign(esp, ebp);
                m.Assign(ebp, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Goto("m3done");

                m.Label("m2base_case");
                m.Assign(esp, ebp);
                m.Assign(ebp, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));

                m.Label("m3done");
                m.Return();
            });
            RunTest();
        }

        [Test]
        public void TrfFibonacci()
        {
            Given_Architecture(new Reko.Arch.X86.X86ArchitectureFlat32("x86-protected-32"));
            Given_PlatformTrashedRegisters();
            Expect(
                "recursive",
                "Preserved: ebp,esp",
                "Trashed: eax",
                "");
            builder.Add("recursive", m =>
            {
                var eax = m.Frame.EnsureRegister(arch.GetRegister("eax"));
                var ebp = m.Frame.EnsureRegister(arch.GetRegister("ebp"));
                var esp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(esp, m.Frame.FramePointer);
                m.Assign(esp, m.ISub(esp, 4));
                m.MStore(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(esp, m.ISub(esp, 4));
                m.BranchIf(m.Lt(m.Mem32(m.IAdd(ebp, 8)), 2), "m2base_case");

                m.Label("m1recursive");
                m.Assign(eax, m.Mem32(m.IAdd(ebp, 8)));
                m.Assign(eax, m.ISub(eax, 1));
                m.Assign(esp, m.ISub(esp, 4));
                m.MStore(esp, eax);
                m.Call("recursive", 4);
                m.Assign(esp, m.IAdd(esp, 4));
                m.MStore(m.ISub(ebp, 4), eax);

                m.Assign(eax, m.Mem32(m.IAdd(ebp, 8)));
                m.Assign(eax, m.ISub(eax, 2));
                m.Assign(esp, m.ISub(esp, 4));
                m.MStore(esp, eax);
                m.Call("recursive", 4);
                m.Assign(esp, m.IAdd(esp, 4));
                m.Assign(eax, m.IAdd(eax, m.Mem32(m.ISub(ebp, 4))));

                m.Assign(esp, ebp);
                m.Assign(ebp, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Goto("m3done");

                m.Label("m2base_case");
                m.Assign(eax, 1);
                m.Assign(esp, ebp);
                m.Assign(ebp, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));

                m.Label("m3done");
                m.Return();
            });
            RunTest();
        }

        [Test(Description = "Tests that phi nodes with constants result in constants.")]
        [Category(Categories.UnitTests)]
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

        [Test]
        [Category(Categories.UnitTests)]
        public void TrfSaveRegistersOnStack()
        {
            Expect("main", "Preserved: r5,r63", "Trashed: r0", "");
            builder.Add("main", m =>
            {
                var eax = m.Reg32("eax", 0);
                var esp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var ebp = m.Reg32("ebp", 5);
                m.Assign(esp, m.Frame.FramePointer);
                m.Assign(esp, m.ISub(esp, 4));
                m.MStore(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(eax, m.Mem32(m.IAdd(ebp, 8)));
                m.Assign(ebp, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();
            });
            RunTest();
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void TrfSaveRegistersOnStack_TwoExits()
        {
            Given_Architecture(new Reko.Arch.X86.X86ArchitectureFlat32("x86-protected-32"));
            Expect("main", "Preserved: ebp,esp", "Trashed: eax", "");
            builder.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(arch.GetRegister("eax"));
                var esp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var ebp = m.Frame.EnsureRegister(arch.GetRegister("ebp"));
                m.Assign(esp, m.Frame.FramePointer);
                m.Assign(esp, m.ISub(esp, 4));
                m.MStore(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(eax, m.Mem32(m.IAdd(ebp, 8)));
                m.BranchIf(m.Eq0(eax), "zero");

                m.Label("not_zero");
                m.Assign(eax, 1);
                m.Assign(ebp, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();

                m.Label("zero");
                m.Assign(eax, 0);
                m.Assign(ebp, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();
            });
            RunTest();
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void TrfFlags()
        {
            Expect("main", "Preserved: r63", "Trashed: r2", "");
            builder.Add("main", m =>
            {
                var grf1 = m.Flags("SNZV");
                var grf2 = m.Flags("CZS");
                var r2 = m.Reg32("r2", 2);
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);

                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(grf1, m.Cond(m.ISub(r2, 3)));
                m.BranchIf(m.Test(ConditionCode.EQ, grf1), "mEq");

                m.Assign(r2, 4);
                m.Assign(grf2, m.Cond(r2));

                m.Label("mEq");
                m.Return();
            });
            RunTest();
            Assert.AreEqual("[flags, 15]", FlowOf("main").grfTrashed.First().ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void TrfCtx_MergeState_Stack()
        {
            var state = new Dictionary<Identifier, Tuple<Expression, BitRange>>();
            var stateOther = new Dictionary<Identifier, Tuple<Expression, BitRange>>();
            var procFlow = new ProcedureFlow(null);
            var ctx = new TrashedRegisterFinder.Context(
                null, null, state, procFlow);
            var ctxOther = new TrashedRegisterFinder.Context(
                null, null, state, procFlow);
            var ebp = new Identifier("ebp", PrimitiveType.Word32, new RegisterStorage("ebp", 5, 0, PrimitiveType.Word32));
            var esi = new Identifier("esi", PrimitiveType.Word32, new RegisterStorage("esi", 6, 0, PrimitiveType.Word32));
            var edi = new Identifier("edi", PrimitiveType.Word32, new RegisterStorage("edi", 7, 0, PrimitiveType.Word32));

            ctx.StackState[-4] = ebp;
            ctx.StackState[-8] = esi;
            ctx.StackState[-16] = Constant.Word32(0x42);
            ctx.StackState[-20] = Constant.Word32(0x42);
            ctxOther.StackState[-4] = ebp;
            ctxOther.StackState[-12] = edi;
            ctxOther.StackState[-16] = Constant.Word32(0x42);
            ctxOther.StackState[-20] = Constant.Word32(0x4711);

            ctx.MergeWith(ctxOther);

            Assert.AreEqual(ebp, ctx.StackState[-4]);
            Assert.AreEqual(esi, ctx.StackState[-8]);
            Assert.AreEqual(edi, ctx.StackState[-12]);
            Assert.AreEqual("0x00000042", ctx.StackState[-16].ToString());
            Assert.AreSame(Constant.Invalid, ctx.StackState[-20]);
        }

        [Test(Description = "This analysis is flow sensitive, so if a called procedure terminates, no effects are propagated")]
        [Category(Categories.UnitTests)]
        public void TrfTerminates()
        {
            Expect("main", "Preserved: r63", "Trashed: ", "");
            builder.Add("main", m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var fn = new ExternalProcedure(
                    "exit",
                    FunctionType.Action(r1),
                    new ProcedureCharacteristics { Terminates = true });
                m.Assign(sp, m.Frame.FramePointer);
                m.BranchIf(m.Ne0(r2), "m2notzero");

                m.Label("m1fail");
                m.Assign(r1, 1);
                m.SideEffect(m.Fn(fn, r1));

                m.Label("m2notzero");
                m.Return();
            });
            RunTest();
        }

        [Test(Description = "Part of handling x86 and Z80 sub-registers.")]
        public void TrfDpb()
        {
            var arch = new Reko.Arch.X86.X86ArchitectureFlat32("x86-protected-32");
            Given_Architecture(arch);
            Expect("main", "Preserved: esp", "Trashed: ecx", "");
            builder.Add("main", m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var ecx = m.Frame.EnsureRegister(arch.GetRegister("ecx"));
                var cl = m.Frame.EnsureRegister(arch.GetRegister("cl"));
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(cl, m.Mem8(m.Word32(0x00123400)));
                m.Assign(ecx, m.Seq(m.Slice(ecx, 8, 24), cl));
                m.Return();
            });
            RunTest();
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void TrfWideThenNarrow()
        {
            var arch = new Reko.Arch.X86.X86ArchitectureFlat32("x86-protected-32");
            Given_Architecture(arch);
            Expect("main", "Preserved: esp", "Trashed: ecx", "");
            builder.Add("main", m =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var ecx = m.Frame.EnsureRegister(arch.GetRegister("ecx"));
                var cl = m.Frame.EnsureRegister(arch.GetRegister("cl"));
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(ecx, m.Mem32(m.Word32(0x00123480)));
                m.Assign(cl, m.Mem8(m.Word32(0x00123400)));
                m.Return();
            });
            RunTest();
        }

        [Test(Description = "Tests that loops with phi nodes terminate.")]
        [Category(Categories.UnitTests)]
        public void TrfLoop()
        {
            Expect(
                "recursive",
                "Preserved: ",
                "Trashed: r1,r2,r3,r63",
                "");
            builder.Add("recursive", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var r3 = m.Reg32("r3", 3);
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(r1, m.Frame.FramePointer);
                m.Call(new ExternalProcedure("ext_code", new FunctionType()), 0);  // Hell node: indirect call forces a definition of r2

                m.Label("m1_loop_body");
                m.MStore(m.Word32(0x00123400), m.IAdd(m.Mem32(m.Word32(0x00123400)), r2));
                m.Assign(r2, m.IAdd(r2, 1));
                m.BranchIf(m.Lt(r2, 0x1000), "m1_loop_body");

                m.Label("m3_exit");
                m.Return();
            });
            RunTest();
        }


    }
}
