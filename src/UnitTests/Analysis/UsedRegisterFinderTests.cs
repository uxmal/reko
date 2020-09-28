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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UsedRegisterFinderTests
    {
        private ProgramDataFlow pf;
        private ProgramBuilder progBuilder;
        private SegmentMap segmentMap;

        [SetUp]
        public void Setup()
        {
            this.pf = new ProgramDataFlow();
            this.progBuilder = new ProgramBuilder();
            this.segmentMap = new SegmentMap(Address.Ptr32(0));
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

        private Procedure RunTest(string sExp, IProcessorArchitecture arch, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder(arch);
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
            var dynamicLinker = new Mock<IDynamicLinker>();
            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.CreateTrashedRegisters()).Returns(new HashSet<RegisterStorage>());
            progBuilder.Program.Platform = platform.Object;

            var sst = new SsaTransform(
                progBuilder.Program,
                proc,
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                new ProgramDataFlow());
            sst.Transform();

            var vp = new ValuePropagator(
                segmentMap,
                sst.SsaState,
                new CallGraph(),
                dynamicLinker.Object,
                NullDecompilerEventListener.Instance);
            vp.Transform();

            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.RemoveDeadSsaIdentifiers();

            vp.Transform();

            return RunTest(sExp, sst.SsaState);
        }

        private Procedure RunSsaTest(string sExp, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder();
            builder(m);
            return RunTest(sExp, m.Ssa);
        }

        private Procedure RunTest(string sExp, SsaState ssa)
        {
            pf.ProcedureFlows[ssa.Procedure] = new ProcedureFlow(ssa.Procedure);
            var urf = new UsedRegisterFinder(
                pf,
                new Procedure[] { ssa.Procedure },
                NullDecompilerEventListener.Instance);
            var flow = urf.ComputeLiveIn(ssa, true);
            var sw = new StringWriter();
            sw.Write("Used: ");
            sw.Write(string.Join(",", flow.BitsUsed.OrderBy(p => p.Key.ToString())));
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                ssa.Procedure.Dump(true);
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            return ssa.Procedure;
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfRegisterArg()
        {
            var sExp = "Used: [r1, [0..31]]";
            RunTest(sExp, m =>
            {
                var r1 = m.Register("r1");
                m.MStore(m.Word32(0x2000), r1);
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfStackArg()
        {
            var sExp = "Used: [Stack +0004, [0..31]]";
            RunTest(sExp, m =>
            {
                var fp = m.Frame.FramePointer;
                var r1 = m.Reg32("r1", 1);
                m.Assign(r1, m.Mem32(m.IAdd(fp, 4)));
                m.MStore(m.Word32(0x2000), r1);
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfCast()
        {
            var sExp = @"Used: [r1, [0..15]]";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var tmp = m.Temp(PrimitiveType.Word16, "tmp");
                m.Assign(tmp, m.Cast(PrimitiveType.Word16, r1));
                m.MStore(m.Word32(0x2000), tmp);
                m.Return();
            });
        }


        [Test(Description = "Identifiers are not considered used if they only are copied.")]
        [Category(Categories.UnitTests)]
        public void UrfCopy()
        {
            var sExp ="Used: ";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(r2, r1);
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfBranch()
        {
            var sExp = @"Used: [r1, [0..31]]";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                m.BranchIf(m.Ge(m.Mem8(m.Word32(0x02000)), 4), "mge");
                m.Label("mlt");
                m.MStore(m.Word32(0x02004), r1);
                m.Goto("mxit");
                m.Label("mge");
                m.MStore(m.Word32(0x02008), m.Cast(PrimitiveType.Word16, r1));
                m.Label("mxit");
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfSequence()
        {
            var sExp = "Used: [r1, [0..31]],[r2, [0..31]]";

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var r2_r1 = m.Frame.EnsureSequence(PrimitiveType.Word64, r2.Storage, r1.Storage);

                m.MStore(m.Word32(0x2000), m.Shr(r2_r1, 2));
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfPhiBranch()
        {
            var sExp = "Used: ";

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(r1, 0);
                m.BranchIf(m.Eq0(m.Mem32(m.Word32(0x00123400))), "skip");
                m.Assign(r2, m.Mem32(m.Word32(0x0123408)));
                m.Assign(r1, m.IMul(r2, 9));
                m.Label("skip");
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfSlice()
        {
            var sExp = "Used: [r1, [16..31]]";

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);

                m.MStore(m.Word16(0x00123400), m.Slice(PrimitiveType.Word16, r1, 16));
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfCall()
        {
            var sExp = "Used: [r1, [0..31]],[r2, [0..31]]";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);

                m.Assign(r2, m.IAdd(r2, 0));
                m.Call(r1, 0);
                m.Return();
            });
        }

        [Test]
        public void UrfDpb()
        {
            var sExp = "Used: [bx, [0..15]]";
            RunTest(sExp, m =>
            {
                var _bx = new RegisterStorage("bx", 3, 0, PrimitiveType.Word16);
                var _cx = new RegisterStorage("cx", 1, 0, PrimitiveType.Word16);
                var _cl = new RegisterStorage("cl", 1, 0, PrimitiveType.Byte);
                var _ch = new RegisterStorage("ch", 1, 8, PrimitiveType.Byte);
                var bx = m.Frame.EnsureRegister(_bx);
                var cx = m.Frame.EnsureRegister(_cx);
                var cl = m.Frame.EnsureRegister(_cl);
                var ch = m.Frame.EnsureRegister(_ch);
                m.Label("m1Loop");
                m.Assign(cl, m.Mem8(bx));
                m.Assign(ch, m.Mem8(m.IAdd(bx, 1)));
                m.MStore(m.IAdd(bx, 40), cx);
                m.Assign(bx, m.ISub(bx, 1));
                m.BranchIf(m.Ne0(bx), "m1Loop");
                m.Label("m2Done");
                m.Return();
            });
        }

        [Test]
        public void UrfDpb2()
        {
            var sExp = "Used: [bx, [0..15]],[cl, [0..7]]";
            RunTest(sExp, new X86ArchitectureFlat32(""), m =>
            {
                var bx = m.Frame.EnsureRegister(Registers.bx);
                var cx = m.Frame.EnsureRegister(Registers.cx);
                var cl = m.Frame.EnsureRegister(Registers.cl);
                var ch = m.Frame.EnsureRegister(Registers.ch);
                m.Label("m1Loop");
                m.Assign(ch, m.Mem8(bx));
                m.MStore(m.IAdd(bx, 40), cx);
                m.Assign(bx, m.ISub(bx, 1));
                m.BranchIf(m.Ne0(bx), "m1Loop");
                m.Label("m2Done");
                m.Return();
            });
        }

        [Test]
        public void UrfSequence2()
        {
            var sExp = "Used: [Sequence r1:r2, [32..63]]";
            RunSsaTest(sExp, m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                var r1_r2 = m.SeqId("r1_r2", PrimitiveType.Word64, r1.Storage, r2.Storage);
                m.AddDefToEntryBlock(r1_r2);
                m.MStore(m.Word32(0x00123400), m.Slice(r2.DataType, r1_r2, 32));
                m.Return();
            });
        }

        [Test]
        public void UrfMkSequence()
        {
            var sExp = "Used: [r1, [0..31]],[r2, [0..31]]";
            RunSsaTest(sExp, m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                var r3 = m.Reg64("r3");

                m.AddDefToEntryBlock(r1);
                m.AddDefToEntryBlock(r2);
                m.Assign(r3, m.Seq(r1, r2));
                m.MStore(m.Word32(0x00123400), r3);
            });
        }

        [Test]
        public void UrfMkSequence_unused()
        {
            var sExp = "Used: ";
            RunSsaTest(sExp, m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                var r3 = m.Reg64("r3");

                m.AddDefToEntryBlock(r1);
                m.AddDefToEntryBlock(r2);
                m.Assign(r3, m.Seq(r1, r2));
            });
        }

        [Test]
        public void UrfArray()
        {
            var sExp = "Used: [r1, [0..31]],[r2, [0..31]]";
            RunSsaTest(sExp, m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                var r3 = m.Reg64("r3");

                m.AddDefToEntryBlock(r1);
                m.AddDefToEntryBlock(r2);

                m.Assign(r3, m.ARef(PrimitiveType.Word32, r1, m.IAdd(r2, 1)));

                m.AddUseToExitBlock(r3);
            });
        }
    }
}
