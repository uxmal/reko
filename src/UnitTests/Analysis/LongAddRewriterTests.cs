#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.Collections.Generic;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class LongAddRewriterTests : AnalysisTestBase
    {
        private IStorageBinder binder;
        private LongAddRewriter rw;
        private IProcessorArchitecture arch;
        private Program program;
        private Identifier ax;
        private Identifier bx;
        private Identifier cx;
        private Identifier dx;
        private Identifier SCZ;
        private Identifier CF;
        private ProcedureBuilder m;
        private SsaState ssa;
        private Block block;

        public LongAddRewriterTests()
        {
            arch = new FakeArchitecture();
            program = new Program()
            {
                Architecture = arch,
                Platform = platform,
                SegmentMap = new SegmentMap(Address.Ptr32(0))
            };
        }

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder(arch);
            binder = m.Frame;
            ax = binder.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
            bx = binder.EnsureRegister(new RegisterStorage("bx", 3, 0, PrimitiveType.Word16));
            cx = binder.EnsureRegister(new RegisterStorage("cx", 1, 0, PrimitiveType.Word16));
            dx = binder.EnsureRegister(new RegisterStorage("dx", 2, 0, PrimitiveType.Word16));
            SCZ = binder.EnsureFlagGroup(arch.GetFlagGroup("SCZ"));
            CF = binder.EnsureFlagGroup(arch.GetFlagGroup("C"));
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
                var sst = new SsaTransform(
                    program,
                    proc,
                    new HashSet<Procedure>(),
                    null, 
                    new ProgramDataFlow());
                sst.Transform();
                var vp = new ValuePropagator(
                    program.SegmentMap,
                    sst.SsaState,
                    program.CallGraph, 
                    null, 
                    eventListener);
                vp.Transform();
                sst.RenameFrameAccesses = true;
                sst.Transform();
                sst.AddUsesToExitBlock();
                sst.RemoveDeadSsaIdentifiers();

                var larw = new LongAddRewriter(sst.SsaState);
                larw.Transform();

                proc.Write(false, writer);
                writer.WriteLine();
            }
        }

        private void RunTest(Action<ProcedureBuilder> builder)
        {
            builder(m);
            var sst = new SsaTransform(
                program, 
                m.Procedure,
                new HashSet<Procedure>(),
                null,
                new ProgramDataFlow());
            sst.Transform();
            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.RemoveDeadSsaIdentifiers();

            rw = new LongAddRewriter(sst.SsaState);
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
            m.Procedure.Dump(true);
            var cm = rw.FindConditionOf(block.Statements, 0, GetId("ax_3"));
            //Assert.AreEqual("ax_3,0,SCZ_4,SCZ_4 = cond(ax_3),SCZ_4", string.Format("{0},{1},{2},{3}", cm.src, cm.StatementIndex, cm.Statement, cm.FlagGroup));
            var asc = rw.FindUsingInstruction(cm.FlagGroup, new AddSubCandidate { Left=ax, Right=cx });
            Assert.AreEqual("dx_8 = dx + bx + C_7", asc.Statement.ToString());
        }

        [Test]
        public void Match_AddRegMem()
        {
            RunTest(m =>
            {
                m.Assign(ax, m.IAdd(ax, m.Mem16(m.IAdd(bx, 0x300))));
                m.Assign(
                    dx,
                    m.IAdd(
                        m.IAdd(
                            dx,
                        m.Mem32(m.IAdd(bx, 0x302))),
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
                m.Assign(ax, m.IAdd(ax, m.Mem16(m.IAdd(bx, 0x300))));
                m.Assign(
                    dx,
                    m.IAdd(
                        m.IAdd(
                            dx,
                        m.Mem16(m.IAdd(bx, 0x302))),
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
                m.Assign(ax, m.IAdd(ax, m.Mem16(m.IAdd(bx, 0x300))));
                m.Assign(CF, m.Cond(ax));
            m.Assign(dx, m.IAdd(m.IAdd(dx, m.Mem16(m.IAdd(bx, 0x302))), CF));
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
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test(Description = "Avoid building long adds if the instructions shouldn't be paired")]
        [Category(Categories.UnitTests)]
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
	C_3 = SLICE(SCZ_2, bool, 2) (alias)
	ax_4 = 0x0000 + C_3
	SCZ_5 = cond(ax_4)
	SCZ_6 = cond(cx - 0x003A)
	C_7 = SLICE(SCZ_6, bool, 2) (alias)
	C_8 = !C_7
	ax_9 = ax_4 + ax_4 + C_8
	SCZ_10 = cond(ax_9)
	C_11 = SLICE(SCZ_10, bool, 2) (alias)
	S_12 = SLICE(SCZ_10, bool, 0) (alias)
	Z_13 = SLICE(SCZ_10, bool, 1) (alias)
	return
";
            var sb = new StringWriter();
            block.Write(sb);
            Assert.AreEqual(sExp, sb.ToString());
        }
    }
}
