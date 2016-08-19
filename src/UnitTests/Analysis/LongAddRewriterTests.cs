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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class LongAddRewriterTests : AnalysisTestBase
    {
        private Frame frame;
        private LongAddRewriter2 rw;
        private IProcessorArchitecture arch;
        private Identifier ax;
        private Identifier bx;
        private Identifier cx;
        private Identifier dx;
        private Identifier SCZ;
        private Identifier CF;
        private PrimitiveType w16 = PrimitiveType.Word16;
        private ProcedureBuilder m;
        private Block block;
        private FlagRegister flags;
        private SsaState ssa;

        public LongAddRewriterTests()
        {
            arch = new FakeArchitecture();
        }

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder(arch);
            frame = m.Frame;
            ax = frame.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
            bx = frame.EnsureRegister(new RegisterStorage("bx", 3, 0, PrimitiveType.Word16));
            cx = frame.EnsureRegister(new RegisterStorage("cx", 1, 0, PrimitiveType.Word16));
            dx = frame.EnsureRegister(new RegisterStorage("dx", 2, 0, PrimitiveType.Word16));
            flags = new FlagRegister("flags", 0, PrimitiveType.Word16);
            SCZ = frame.EnsureFlagGroup(flags, 7, "SCZ", PrimitiveType.Byte);
            CF = frame.EnsureFlagGroup(flags, arch.CarryFlagMask, "C", PrimitiveType.Bool);

        }

        private Identifier GetId(string idName)
        {
            return ssa.Identifiers
                .Where(sid => sid.Identifier.Name == idName)
                .Select(sid => sid.Identifier)
                .First();
        }

        public bool CreateLongInstruction(Statement loInstr, Statement hiInstr)
        {
            var loAss = rw.MatchAddSub(loInstr);
            var hiAss = rw.MatchAdcSbc(hiInstr);
            if (loAss == null || hiAss == null)
                return false;
            if (loAss.Op != hiAss.Op)
                return false;

            rw.CreateLongInstruction(loAss, hiAss);
            return true;
        }

        protected override void RunTest(Program program, TextWriter writer)
        {
            var eventListener = new FakeDecompilerEventListener();
            foreach (var proc in program.Procedures.Values)
            {
                var sst = new SsaTransform2(program.Architecture, proc, null, new DataFlow2());
                sst.Transform();
                var vp = new ValuePropagator(arch, sst.SsaState);
                vp.Transform();
                sst.RenameFrameAccesses = true;
                sst.Transform();
                sst.AddUsesToExitBlock();
                sst.RemoveDeadSsaIdentifiers();

                var larw = new LongAddRewriter2(program.Architecture, sst.SsaState);
                larw.Transform();

                proc.Write(false, writer);
                writer.WriteLine();
            }
        }

        private void RunTest(Action<ProcedureBuilder> builder)
        {
            builder(m);
            var sst = new SsaTransform2(arch, m.Procedure, null, new DataFlow2());
            sst.Transform();
            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.RemoveDeadSsaIdentifiers();

            rw = new LongAddRewriter2(arch, sst.SsaState);
            this.ssa = sst.SsaState;
        }

        [Test]
        public void Larw_FindCond()
        {
            Block block = null;
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, cx));
                m.Assign(SCZ, m.Cond(ax));
                block = m.CurrentBlock;
                m.Return();
            });
            var ax_3 = GetId("ax_3");
            var cm = rw.FindConditionOf(block.Statements, 0, ax_3);

            Assert.AreEqual("SCZ_4", cm.FlagGroup.ToString());
            Assert.AreEqual("SCZ_4 = cond(ax_3)", cm.Statement.ToString());
        }

        [Test]
        public void Larw_FindInstructionUsesCond()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, cx));
                m.Assign(SCZ, m.Cond(ax));
                block = m.CurrentBlock;
                m.Assign(dx, m.IAdd(m.IAdd(dx, bx), CF));
                m.Return();
            });
            var cm = rw.FindConditionOf(block.Statements, 0, GetId("ax_3"));
            var asc = rw.FindUsingInstruction(cm.FlagGroup, new AddSubCandidate { Left=ax, Right=cx });
            Assert.AreEqual("dx_7 = dx + bx + SCZ_4", asc.Statement.ToString());
        }

        [Test]
        public void Match_AddRegMem()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, m.LoadW(m.IAdd(bx, 0x300))));
                m.Assign(
                    dx,
                    m.IAdd(
                        m.IAdd(
                            dx,
                            m.LoadDw(m.IAdd(bx, 0x302))),
                        CF));
                block = m.Block;
                m.Return();
            });
            CreateLongInstruction(block.Statements[0], block.Statements[1]);
            Assert.AreEqual("dx_ax_9 = dx_ax_8 + Mem0[bx + 0x0300:ui32]", block.Statements[2].ToString());
        }

        [Test]
        public void Larw_Match_AddRecConst()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, 0x5678));
                m.Assign(CF, m.Cond(ax));
                m.Assign(dx, m.IAdd(m.IAdd(dx, 0x1234), CF));
                block = m.Block;
                m.Return();
            });
            CreateLongInstruction(block.Statements[0], block.Statements[2]);
            Assert.AreEqual("dx_ax_7 = dx_ax_6 + 0x12345678", block.Statements[3].ToString());
        }

        [Test]
        public void Larw_Match_AddConstant()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, 1));
                m.Assign(CF, m.Cond(ax));
                m.Assign(dx, m.IAdd(m.IAdd(dx, 0), CF));
                block = m.Block;
                m.Return();
            });
            CreateLongInstruction(block.Statements[0], block.Statements[2]);
            Assert.AreEqual("dx_ax_7 = dx_ax_6 + 0x00000001", block.Statements[3].ToString());
        }

        [Test]
        public void Larw_Match_RegMem()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, m.LoadW(m.IAdd(bx, 0x300))));
                m.Assign(
                    dx,
                    m.IAdd(
                        m.IAdd(
                            dx,
                            m.LoadW(m.IAdd(bx, 0x302))),
                        CF));
                block = m.Block;
                m.Return();
            });
            CreateLongInstruction(block.Statements[0], block.Statements[1]);
            Assert.AreEqual("dx_ax_9 = dx_ax_8 + Mem0[bx + 0x0300:ui32]", block.Statements[2].ToString());
        }

        [Test]
        public void Larw_MatchAdcSbc()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(m.IAdd(ax, cx), CF));
                block = m.Block;
                m.Return();
            });
            var regPair = rw.MatchAdcSbc(block.Statements[0]);
            Assert.AreSame(ax, regPair.Left);
            Assert.AreSame(cx, regPair.Right);
        }

        [Test]
        public void Larw_MatchAddSub()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, cx));
                block = m.Block;
                m.Return();
            });
            var regPair = rw.MatchAddSub(block.Statements[0]);
            Assert.AreSame(ax, regPair.Left);
            Assert.AreSame(cx, regPair.Right);
        }

        [Test]
        public void Larw_Replace_AddReg()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, m.LoadW(m.IAdd(bx, 0x300))));
                m.Assign(CF, m.Cond(ax));
                m.Assign(dx, m.IAdd(m.IAdd(dx, m.LoadW(m.IAdd(bx, 0x302))), CF));
                m.Assign(CF, m.Cond(dx));
                block = m.Block;
                m.Return();
            });

            rw.ReplaceLongAdditions(block);

            //$TODO: remove the C = cond(ax)
            var sExp = @"l1:
	C_5 = cond(ax_4)
	dx_ax_9 = SEQ(dx, ax)
	Mem0[bx + 0x0300:ui32] = SEQ(Mem0[bx + 0x0302:word16], Mem0[bx + 0x0300:word16])
	dx_ax_10 = dx_ax_9 + Mem0[bx + 0x0300:ui32]
	ax_4 = (word16) dx_ax_10
	dx_7 = SLICE(dx_ax_10, word16, 16)
	C_8 = cond(dx_7)
	return
";
            var sb = new StringWriter();
            block.Write(sb);
            Console.WriteLine(sb);
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void Larw_Avoid()
        {
            RunTest(m =>
            {
                m.Assign(SCZ, m.Cond(m.ISub(cx, 0x0030)));
                m.Assign(ax, m.IAdd(m.Word16(0x0000), CF));
                m.Assign(SCZ, m.Cond(ax));
                m.Assign(SCZ, m.Cond(m.ISub(cx, 0x003A)));
                m.Assign(CF, m.Not(CF));
                m.Assign(ax, m.IAdd(m.IAdd(ax, ax), CF));
                m.Assign(SCZ, m.Cond(ax));
                block = m.Block;
                m.Return();
            });

            rw.Transform();

            var sExp = @"l1:
	SCZ_2 = cond(cx - 0x0030)
	ax_3 = 0x0000 + SCZ_2
	SCZ_4 = cond(ax_3)
	SCZ_5 = cond(cx - 0x003A)
	C_6 = !SCZ_5
	ax_7 = ax_3 + ax_3 + C_6
	SCZ_8 = cond(ax_7)
	return
";
            var sb = new StringWriter();
            block.Write(sb);
            Console.WriteLine(sb);
            Assert.AreEqual(sExp, sb.ToString());
        }
    }
}
