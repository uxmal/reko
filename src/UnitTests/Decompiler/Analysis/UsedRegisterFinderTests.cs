#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Analysis
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
            platform.Setup(p => p.TrashedRegisters).Returns(new HashSet<RegisterStorage>());
            platform.Setup(p => p.PreservedRegisters).Returns(new HashSet<RegisterStorage>());
            platform.Setup(p => p.PointerType).Returns(arch.PointerType);
            progBuilder.Program.Platform = platform.Object;
            progBuilder.Program.SegmentMap = segmentMap;
            progBuilder.Program.Memory = new ProgramMemory(segmentMap);
            var sc = new ServiceContainer();
            sc.AddService<IDecompilerEventListener>(new FakeDecompilerEventListener());
            var sst = new SsaTransform(
                progBuilder.Program,
                proc,
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                new ProgramDataFlow());
            sst.Transform();

            var vp = new ValuePropagator(
                progBuilder.Program,
                sst.SsaState,
                dynamicLinker.Object,
                sc);
            vp.Transform();

            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.RemoveDeadSsaIdentifiers();

            vp.Transform();

            return RunTest(sExp, progBuilder.Program, sst.SsaState);
        }

        private Procedure RunSsaTest(string sExp, Action<SsaProcedureBuilder> builder, Func<Procedure, ProcedureFlow> mkFlow = null)
        {
            var m = new SsaProcedureBuilder();
            builder(m);
            var arch = m.Procedure.Architecture;
            var platform = new DefaultPlatform(arch.Services, arch);
            var program = new Program { Architecture = arch, Platform = platform };
            return RunTest(sExp, program, m.Ssa, mkFlow);
        }

        private Procedure RunTest(string sExp, Program program, SsaState ssa, Func<Procedure, ProcedureFlow> mkFlow = null)
        {
            mkFlow ??= p => new ProcedureFlow(p);
            pf.ProcedureFlows[ssa.Procedure] = mkFlow(ssa.Procedure);
            var urf = new UsedRegisterFinder(
                program,
                pf,
                new Procedure[] { ssa.Procedure },
                NullDecompilerEventListener.Instance);
            var flow = urf.ComputeLiveIn(ssa, true);
            var sw = new StringWriter();
            sw.WriteLine();
            sw.Write("Used: ");
            sw.WriteLine(string.Join(",", flow.BitsUsed.OrderBy(p => p.Key.ToString())));
            sw.WriteLine("DataTypes:");
            foreach (var de in flow.LiveInDataTypes.OrderBy(p => p.Key.ToString()))
            {
                sw.WriteLine("  {0}: {1}", de.Key, de.Value);
            }
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                ssa.Procedure.Dump(true);
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            return ssa.Procedure;
        }

        private void RunClassifyTest(
            string sExp,
            string sidName,
            Action<SsaProcedureBuilder> gen,
            Func<Procedure, ProcedureFlow> mkFlow)
        {
            var m = new SsaProcedureBuilder();
            gen(m);
            var ssa = m.Ssa;
            pf.ProcedureFlows[ssa.Procedure] = mkFlow(ssa.Procedure);
            var program = new Program();
            var urf = new UsedRegisterFinder(
                program,
                pf,
                new Procedure[] { ssa.Procedure },
                NullDecompilerEventListener.Instance);
            var sid = ssa.Identifiers.Single(s => s.Identifier.Name == sidName);
            var flow = urf.Classify(ssa, sid, sid.Identifier.Storage, true);
            var sw = new StringWriter();
            sw.Write("Used: ");
            sw.Write(string.Join(",", flow));
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                ssa.Procedure.Dump(true);
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfRegisterArg()
        {
            var sExp = @"
Used: [r1, [0..31]]
DataTypes:
  r1: word32
";
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
            var sExp = @"
Used: [Stack +0004, [0..31]]
DataTypes:
  Stack +0004: word32
";
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
            var sExp = @"
Used: [r1, [0..15]]
DataTypes:
  r1: word32
";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var tmp = m.Temp(PrimitiveType.Word16, "tmp");
                m.Assign(tmp, m.Slice(r1, PrimitiveType.Word16));
                m.MStore(m.Word32(0x2000), tmp);
                m.Return();
            });
        }

        [Test(Description = "Identifiers are not considered used if they only are copied.")]
        [Category(Categories.UnitTests)]
        public void UrfCopy()
        {
            var sExp = @"
Used: 
DataTypes:
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
        [Category(Categories.UnitTests)]
        public void UrfBranch()
        {
            var sExp = @"
Used: [r1, [0..31]]
DataTypes:
  r1: word32
";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                m.BranchIf(m.Ge(m.Mem8(m.Word32(0x02000)), 4), "mge");
                m.Label("mlt");
                m.MStore(m.Word32(0x02004), r1);
                m.Goto("mxit");
                m.Label("mge");
                m.MStore(m.Word32(0x02008), m.Slice(r1, PrimitiveType.Word16));
                m.Label("mxit");
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfSequence()
        {
            var sExp = @"
Used: [r1, [0..31]],[r2, [0..31]]
DataTypes:
  r1: word32
  r2: word32
";

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
            var sExp = @"
Used: 
DataTypes:
";

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
            var sExp = @"
Used: [r1, [16..31]]
DataTypes:
  r1: word32
";

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);

                m.MStore(m.Word16(0x00123400), m.Slice(r1, PrimitiveType.Word16, 16));
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfCall()
        {
            var sExp = @"
Used: [r1, [0..31]],[r2, [0..31]]
DataTypes:
  r1: word32
  r2: word32
";
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
        [Category(Categories.UnitTests)]
        public void UrfDpb()
        {
            var sExp = @"
Used: [bx, [0..15]]
DataTypes:
  bx: word16
";
            RunTest(sExp, m =>
            {
                var _bx = RegisterStorage.Reg16("bx", 3);
                var _cx = RegisterStorage.Reg16("cx", 1);
                var _cl = RegisterStorage.Reg8("cl", 1);
                var _ch = RegisterStorage.Reg8("ch", 1, 8);
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
        [Category(Categories.UnitTests)]
        public void UrfDpb2()
        {
            var sExp = @"
Used: [bx, [0..15]],[cl, [0..7]]
DataTypes:
  bx: word16
  cl: byte
";
            RunTest(sExp, new X86ArchitectureFlat32(new ServiceContainer(), "", new Dictionary<string, object>()), m =>
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
        [Category(Categories.UnitTests)]
        public void UrfSequence2()
        {
            var sExp = @"
Used: [Sequence r1:r2, [32..63]]
DataTypes:
  Sequence r1:r2: word64
";
            RunSsaTest(sExp, m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                var r1_r2 = m.SeqId("r1_r2", PrimitiveType.Word64, r1.Storage, r2.Storage);
                m.AddDefToEntryBlock(r1_r2);
                m.MStore(m.Word32(0x00123400), m.Slice(r1_r2, r2.DataType, 32));
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfMkSequence()
        {
            var sExp = @"
Used: [r1, [0..31]],[r2, [0..31]]
DataTypes:
  r1: word32
  r2: word32
";
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
        [Category(Categories.UnitTests)]
        public void UrfMkSequence_unused()
        {
            var sExp = @"
Used: 
DataTypes:
";
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
        [Category(Categories.UnitTests)]
        public void UrfArray()
        {
            var sExp = @"
Used: [r1, [0..31]],[r2, [0..31]]
DataTypes:
  r1: (ptr32 (struct (0 (arr (struct 0001)) a0000)))
  r2: word32
";
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

        [Test]
        [Category(Categories.UnitTests)]
        public void UrfAliasedSequence()
        {
            var sExp = "Used: [0..31]";
            var regDx = RegisterStorage.Reg16("dx", 2);
            var regAx = RegisterStorage.Reg16("ax", 0);
            var seqDxAx = new SequenceStorage(regDx, regAx);
            RunClassifyTest(
                sExp,
                "dx_ax",
                m =>
                {
                    var ax = m.Reg("dx", regAx);
                    var dx = m.Reg("ax", regDx);
                    var dx_ax = m.SeqId("dx_ax", PrimitiveType.Word32, regDx, regAx);
                    m.Assign(dx_ax, m.Mem32(m.Word32(0x00123400)));
                    m.Alias(ax, m.Slice(dx_ax, ax.DataType, 0));
                    m.Alias(dx, m.Slice(dx_ax, ax.DataType, 16));
                    m.AddUseToExitBlock(ax);
                    m.AddUseToExitBlock(dx);
                },
                p =>
                {
                    var flow = new ProcedureFlow(p);
                    flow.BitsLiveOut.Add(seqDxAx, new BitRange(0, 32));
                    return flow;
                });
        }

        [Category(Categories.UnitTests)]
        [Test]
        public void UrfPointerArg()
        {
            var sExp = @"
Used: [r1, [0..31]]
DataTypes:
  r1: (ptr32 (struct 0008))
";
            RunSsaTest(sExp, m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                var r3 = m.Reg64("r3");

                m.AddDefToEntryBlock(r1);
                m.AddDefToEntryBlock(r2);

                m.Assign(r3, m.Mem32(r1));
                m.Assign(r3, m.Mem32(m.IAddS(r1, 4)));

                m.AddUseToExitBlock(r3);
            });
        }
    }
}
