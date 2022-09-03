#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Moq;
using System.ComponentModel.Design;
using Reko.Core.Operators;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class LongAddRewriterTests : AnalysisTestBase
    {
        private IStorageBinder binder;
        private LongAddRewriter rw;
        private readonly IProcessorArchitecture arch;
        private readonly Program program;
        private Identifier ax;
        private Identifier bx;
        private Identifier cx;
        private Identifier dx;
        private Identifier rdx;
        private Identifier es;
        private Identifier SCZ;
        private Identifier CF;
        private ProcedureBuilder m;
        private SsaState ssa;
        private Block block;

        public LongAddRewriterTests()
        {
            arch = new FakeArchitecture(new ServiceContainer());
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
            ax = binder.EnsureRegister(RegisterStorage.Reg16("ax", 0));
            bx = binder.EnsureRegister(RegisterStorage.Reg16("bx", 3));
            cx = binder.EnsureRegister(RegisterStorage.Reg16("cx", 1));
            dx = binder.EnsureRegister(RegisterStorage.Reg16("dx", 2));
            rdx = binder.EnsureRegister(RegisterStorage.Reg64("rdx", 2));
            es = binder.EnsureRegister(RegisterStorage.Reg16("es", 14));
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
                    program,
                    sst.SsaState,
                    null, 
                    sc);
                vp.Transform();
                sst.RenameFrameAccesses = true;
                sst.Transform();
                sst.AddUsesToExitBlock();
                sst.RemoveDeadSsaIdentifiers();

                var larw = new LongAddRewriter(sst.SsaState, eventListener);
                larw.Transform();

                proc.Write(false, writer);
                writer.WriteLine();
            }
        }

        private void RunTest(Action<ProcedureBuilder> builder)
        {
            builder(m);
            var dynamicLinker = new Mock<IDynamicLinker>();
            var sst = new SsaTransform(
                program, 
                m.Procedure,
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                new ProgramDataFlow());
            sst.Transform();
            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.RemoveDeadSsaIdentifiers();

            rw = new LongAddRewriter(sst.SsaState, new FakeDecompilerEventListener());
            this.ssa = sst.SsaState;
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            builder(m);
            var dynamicLinker = new Mock<IDynamicLinker>();
            var sst = new SsaTransform(
                program,
                m.Procedure,
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                new ProgramDataFlow());
            sst.Transform();
            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.RemoveDeadSsaIdentifiers();

            rw = new LongAddRewriter(sst.SsaState, new FakeDecompilerEventListener());
            this.ssa = sst.SsaState;

            rw.Transform();
            var sb = new StringWriter();
            block.Write(sb);
            var sActual = sb.ToString();
            if (sExp != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
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
            var asc = rw.FindUsingInstruction(block, cm.FlagGroup, new AddSubCandidate(Operator.IAdd, ax, cx));
            Assert.AreEqual("dx_8 = dx + bx + C_7", asc.Statement.ToString());
        }

        [Test]
        public void Larw_Match_AddRecConst()
        {
            var sExp =
@"l1:
	dx_ax_6 = SEQ(dx, ax)
	dx_ax_7 = dx_ax_6 + 0x12345678<32>
	ax_2 = SLICE(dx_ax_7, word16, 0) (alias)
	dx_5 = SLICE(dx_ax_7, word16, 16) (alias)
	C_3 = cond(ax_2)
	return
";
            RunTest(sExp, m =>
            {
                m.Assign(ax, m.IAdd(ax, 0x5678));
                m.Assign(CF, m.Cond(ax));
                m.Assign(dx, m.IAdd(m.IAdd(dx, 0x1234), CF));
                block = m.Block;
                m.Return();
            });
        }

        [Test]
        public void Larw_Match_AddConstant()
        {
            var sExp =
@"l1:
	dx_ax_6 = SEQ(dx, ax)
	dx_ax_7 = dx_ax_6 + 1<32>
	ax_2 = SLICE(dx_ax_7, word16, 0) (alias)
	dx_5 = SLICE(dx_ax_7, word16, 16) (alias)
	C_3 = cond(ax_2)
	return
";
            RunTest(sExp, m =>
            {
                m.Assign(ax, m.IAdd(ax, 1));
                m.Assign(CF, m.Cond(ax));
                m.Assign(dx, m.IAdd(m.IAdd(dx, 0), CF));
                block = m.Block;
                m.Return();
            });
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
            var sExp =
@"l1:
	dx_ax_9 = SEQ(dx, ax)
	dx_ax_10 = dx_ax_9 + Mem0[bx + 0x300<16>:ui32]
	ax_4 = SLICE(dx_ax_10, word16, 0) (alias)
	dx_7 = SLICE(dx_ax_10, word16, 16) (alias)
	C_5 = cond(ax_4)
	C_8 = cond(dx_7)
	return
";
            RunTest(sExp, m =>
            {
                m.Assign(ax, m.IAdd(ax, m.Mem16(m.IAdd(bx, 0x300))));
                m.Assign(CF, m.Cond(ax));
                m.Assign(dx, m.IAdd(m.IAdd(dx, m.Mem16(m.IAdd(bx, 0x302))), CF));
                m.Assign(CF, m.Cond(dx));
                block = m.Block;
                m.Return();
            });
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
	SCZ_2 = cond(cx - 0x30<16>)
	C_3 = SLICE(SCZ_2, bool, 2) (alias)
	ax_4 = 0<16> + C_3
	SCZ_5 = cond(ax_4)
	SCZ_6 = cond(cx - 0x3A<16>)
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

        [Test]
        public void Larw_InterleavedMemoryAccesses()
        {
            var sExp =
@"l1:
	ax_2 = Mem0[0x0210<p16>:word16]
	dx_3 = Mem0[0x0212<p16>:word16]
	es_cx_4 = Mem0[0x0214<p16>:word32]
	es_5 = SLICE(es_cx_4, word16, 16) (alias)
	cx_7 = SLICE(es_cx_4, word16, 0) (alias)
	bx_6 = es_5
	dx_ax_14 = SEQ(dx_3, ax_2)
	bx_cx_15 = SEQ(bx_6, cx_7)
	dx_ax_16 = dx_ax_14 - bx_cx_15
	ax_8 = SLICE(dx_ax_16, word16, 0) (alias)
	dx_12 = SLICE(dx_ax_16, word16, 16) (alias)
	SCZ_9 = cond(ax_8)
	C_11 = SLICE(SCZ_9, bool, 2) (alias)
	Mem10[0x0218<p16>:word16] = ax_8
	Mem13[0x021A<p16>:word16] = dx_12
";
            RunTest(sExp, m =>
            {
                var es_cx = m.Procedure.Frame.EnsureSequence(PrimitiveType.Word32, es.Storage, cx.Storage);
                m.Assign(ax, m.Mem16(m.Ptr16(0x210)));
                m.Assign(dx, m.Mem16(m.Ptr16(0x212)));
                m.Assign(es_cx, m.Mem32(m.Ptr16(0x214)));
                m.Assign(bx, es);
                m.Assign(ax, m.ISub(ax, cx));
                m.Assign(this.SCZ, m.Cond(ax));
                m.MStore(m.Ptr16(0x218), ax);
                m.Assign(dx, m.ISub(m.ISub(dx, bx), this.CF));
                m.MStore(m.Ptr16(0x21A), dx);
                block = m.Block;
            });
        }

        // We don't wish to carry out a long-add replacement if the ADC part is in 
        // a different block from the ADD part. A really pathological program might
        // have this behavior, at which point we might need to reconsider.
        [Test]
        public void Larw_do_not_span_multiple_blocks()
        {
            var sExp =
            #region Expected
@"l1:
	ax_2 = Mem0[0x0210<p16>:word16]
	dx_3 = Mem0[0x0212<p16>:word16]
	ax_4 = ax_2 + Mem0[0x0220<p16>:word16]
	SCZ_5 = cond(ax_4)
	C_6 = SLICE(SCZ_5, bool, 2) (alias)
";
            #endregion

            RunTest(sExp, m =>
            {
                m.Assign(ax, m.Mem16(m.Ptr16(0x210)));
                block = m.Block;
                m.Assign(dx, m.Mem16(m.Ptr16(0x212)));
                m.Assign(ax, m.IAdd(ax, m.Mem16(m.Ptr16(0x0220))));
                m.Assign(this.SCZ, m.Cond(ax));
                m.Goto("m2");

                m.Label("m2");
                m.Assign(dx, m.IAdd(m.IAdd(dx, m.Mem16(m.Ptr16(0x0222))), this.CF));
            });
        }

        [Test]
        public void Larw_Multiply_Accumulate()
        {
            var sExp =
@"l1:
	eax_2 = CONVERT(Mem0[0x5418<32>:word16], word16, int32)
	edx_3 = 0xF000<32>
	edx_eax_4 = edx_3 *s64 eax_2
	eax_5 = SLICE(edx_eax_4, word32, 0) (alias)
	edx_9 = SLICE(edx_eax_4, word32, 32) (alias)
	edx_eax_17 = SEQ(edx_9, eax_5)
	tmp2_tmp1_18 = Mem0[0x6FF0<32>:ui64] - edx_eax_17
	tmp1_6 = SLICE(tmp2_tmp1_18, word32, 0) (alias)
	tmp2_11 = SLICE(tmp2_tmp1_18, word32, 32) (alias)
	Mem7[0x6FF0<32>:word32] = tmp1_6
	SCZ_8 = cond(tmp1_6)
	C_10 = SLICE(SCZ_8, bool, 2) (alias)
	Mem12[0x6FF4<32>:word32] = tmp2_11
	SCZ_13 = cond(tmp2_11)
	C_14 = SLICE(SCZ_13, bool, 2) (alias)
	S_15 = SLICE(SCZ_13, bool, 0) (alias)
	Z_16 = SLICE(SCZ_13, bool, 1) (alias)
	return
";
            RunTest(sExp, m =>
            {
                var eax = m.Reg32("eax", 0);
                var edx = m.Reg32("edx", 2);
                var edx_eax = m.Frame.EnsureSequence(PrimitiveType.Word64, edx.Storage, eax.Storage);
                var tmp1 = m.Temp(PrimitiveType.Word32, "tmp1");
                var tmp2 = m.Temp(PrimitiveType.Word32, "tmp2");
                m.Assign(eax, m.Convert(m.Mem16(m.Word32(0x5418)), PrimitiveType.Word16, PrimitiveType.Int32));
                m.Assign(edx, m.Word32(0xF000));
                m.Assign(edx_eax, m.SMul(PrimitiveType.Int64, edx, eax));
                m.Assign(tmp1, m.ISub(m.Mem32(m.Word32(0x6FF0)), eax));
                m.MStore(m.Word32(0x6FF0), tmp1);
                m.Assign(this.SCZ, m.Cond(tmp1));
                m.Assign(tmp2, m.ISub(m.ISub(m.Mem32(m.Word32(0x6FF4)), edx), this.CF));
                m.MStore(m.Word32(0x6FF4), tmp2);
                m.Assign(this.SCZ, m.Cond(tmp2));
                block = m.Block;
                m.Return();
            });
        }

        [Test]
        public void Larw_Add16to32()
        {
            var sExp =
            #region Expected
@"l1:
	dx_ax_17 = SEQ(dx, ax)
	dx_ax_18 = dx_ax_17 + SEQ(0<16>, Mem0[bx + 2<16>:word16])
	ax_4 = SLICE(dx_ax_18, word16, 0) (alias)
	dx_8 = SLICE(dx_ax_18, word16, 16) (alias)
	SCZ_5 = cond(ax_4)
	C_7 = SLICE(SCZ_5, bool, 2) (alias)
	dx_ax_19 = SEQ(dx_8, ax_4)
	dx_ax_20 = dx_ax_19 + Mem0[bx + 6<16>:ui32]
	ax_9 = SLICE(dx_ax_20, word16, 0) (alias)
	dx_12 = SLICE(dx_ax_20, word16, 16) (alias)
	SCZ_10 = cond(ax_9)
	C_11 = SLICE(SCZ_10, bool, 2) (alias)
	SCZ_13 = cond(dx_12)
	C_14 = SLICE(SCZ_13, bool, 2) (alias)
	S_15 = SLICE(SCZ_13, bool, 0) (alias)
	Z_16 = SLICE(SCZ_13, bool, 1) (alias)
	return
";
            #endregion

            RunTest(sExp, m =>
            {
                m.Assign(ax, m.IAdd(ax, m.Mem16(m.IAdd(bx, 2))));
                m.Assign(SCZ, m.Cond(ax));
                //m.Alias(CF, m.Slice(PrimitiveType.Bool, SCZ, 1));
                m.Assign(dx, m.IAdd(dx, CF));

                m.Assign(ax, m.IAdd(ax, m.Mem16(m.IAdd(bx, 6))));
                m.Assign(SCZ, m.Cond(ax));
                //m.Alias(CF, m.Slice(PrimitiveType.Bool, SCZO, 1));
                m.Assign(dx, m.IAdd(m.IAdd(dx, m.Mem16(m.IAdd(bx, 8))), CF));
                m.Assign(SCZ, m.Cond(dx));
                this.block = m.Block;
                m.Return();
            });
            ssa.Validate(s => Assert.Fail(s));
        }

        [Test(Description = "PDP-11 had a single operand ADC instruction")]
        [Category(Categories.UnitTests)]
        public void Larw_Pdp11LongAdd()
        {
            var sExp =
            #region Expected
@"l1:
	dx_ax_11 = SEQ(dx, ax)
	bx_cx_12 = SEQ(bx, cx)
	dx_ax_13 = dx_ax_11 + bx_cx_12
	ax_3 = SLICE(dx_ax_13, word16, 0) (alias)
	dx_10 = SLICE(dx_ax_13, word16, 16) (alias)
	SCZ_4 = cond(ax_3)
	C_6 = SLICE(SCZ_4, bool, 2) (alias)
	dx_7 = dx + C_6
	SCZ_8 = cond(dx_7)
";
            #endregion
            RunTest(sExp, m =>
            {
                m.Assign(ax, m.IAdd(ax, cx));
                m.Assign(SCZ, m.Cond(ax));
                m.Assign(dx, m.IAdd(dx, CF));
                m.Assign(SCZ, m.Cond(dx));
                m.Assign(dx, m.IAdd(dx, bx));
                this.block = m.Block;
            });
        }

        [Test]
        public void Larw_Non_related_add_sbc()
        {
            var sExp =
            #region Expected
@"l1:
	ax_2 = ax + ax
	SCZ_3 = cond(ax_2)
	C_4 = SLICE(SCZ_3, bool, 1)
	rdx_6 = rdx - 3<64> - C_4
";
            #endregion
            RunTest(sExp, m =>
                {
                    m.Assign(ax, m.IAdd(ax, ax));
                    m.Assign(SCZ, m.Cond(ax));
                    m.Assign(CF, m.Slice(SCZ, PrimitiveType.Bool, 1));
                    m.Assign(rdx, m.ISub(m.ISub(rdx, 3), CF));
                    this.block = m.Block;
                });
        }
    }
}
