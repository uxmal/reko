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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class LongAddRewriterTests : AnalysisTestBase
    {
        private IStorageBinder frame;
        private LongAddRewriter rw;
        private IProcessorArchitecture arch;
        private Identifier ax;
        private Identifier bx;
        private Identifier cx;
        private Identifier dx;
        private Identifier SCZ;
        private Identifier CF;
        private ProcedureBuilder m;
        private FlagRegister flags;

        public LongAddRewriterTests()
        {
            arch = new FakeArchitecture();
        }

        public Instruction CreateLongInstruction(Instruction loInstr, Instruction hiInstr)
        {
            var loAss = rw.MatchAddSub(loInstr);
            var hiAss = rw.MatchAdcSbc(hiInstr);
            if (loAss == null || hiAss == null)
                return null;
            if (loAss.Op != hiAss.Op)
                return null;

            return rw.CreateLongInstruction(loAss, hiAss);
        }

        protected override void RunTest(Program prog, TextWriter writer)
        {
            var eventListener = new FakeDecompilerEventListener();
            var dfa = new DataFlowAnalysis(prog, null, eventListener);
            var trf = new TrashedRegisterFinder(prog, prog.Procedures.Values, dfa.ProgramDataFlow, eventListener);
            trf.Compute();
            trf.RewriteBasicBlocks();
            RegisterLiveness rl = RegisterLiveness.Compute(prog, dfa.ProgramDataFlow, eventListener);
            foreach (Procedure proc in prog.Procedures.Values)
            {
                LongAddRewriter larw = new LongAddRewriter(proc, prog.Architecture);
                larw.Transform();
                proc.Write(false, writer);
                writer.WriteLine();
            }
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
            flags = new FlagRegister("flags", 4, PrimitiveType.Word16);
            SCZ = frame.EnsureFlagGroup(flags, 7, "SCZ", PrimitiveType.Byte);
            CF = frame.EnsureFlagGroup(flags, arch.CarryFlagMask, "C", PrimitiveType.Bool);
            rw = new LongAddRewriter(m.Procedure, arch);
        }

        [Test]
        public void FindCond()
        {
            m.Assign(ax, m.IAdd(ax, cx));
            m.Assign(SCZ, m.Cond(ax));
            var block = m.CurrentBlock;
            m.Return();

            var cm = rw.FindConditionOf(block.Statements, 0, ax);

            Assert.AreEqual("SCZ", cm.FlagGroup.ToString());
            Assert.AreEqual(1, cm.StatementIndex);
        }

        [Test]
        public void FindInstructionUsesCond()
        {
            m.Assign(ax, m.IAdd(ax, cx));
            m.Assign(SCZ, m.Cond(ax));
            var block = m.CurrentBlock;
            m.Assign(dx, m.IAdd(m.IAdd(dx, bx), CF));
            m.Return();

            var cm = rw.FindConditionOf(block.Statements, 0, ax);
            var asc = rw.FindUsingInstruction(block.Statements, cm.StatementIndex, new AddSubCandidate { Left=ax, Right=cx });
            Assert.AreEqual("dx = dx + bx + C", block.Statements[asc.StatementIndex].Instruction.ToString());
        }

        [Test]
        public void Match_AddRegMem()
        {
            var addAxMem = m.Assign(ax, m.IAdd(ax, m.LoadW(m.IAdd(bx, 0x300))));
            var adcDxMem = m.Assign(
                dx,
                m.IAdd(
                    m.IAdd(
                        dx,
                        m.LoadDw(m.IAdd(bx, 0x302))),
                    CF));

            var instr = CreateLongInstruction(addAxMem, adcDxMem);
            Assert.AreEqual("dx_ax = dx_ax + Mem0[bx + 0x0300:ui32]", instr.ToString());
        }

        [Test]
        public void Match_AddRecConst()
        {
            var i1 = m.Assign(ax, m.IAdd(ax, 0x5678));
            m.Assign(CF, m.Cond(ax));
            var i3 = m.Assign(dx, m.IAdd(m.IAdd(dx, 0x1234), CF));
            var instr = CreateLongInstruction(i1, i3);
            Assert.AreEqual("dx_ax = dx_ax + 0x12345678", instr.ToString());
        }

        [Test]
        public void Match_AddConstant()
        {
            var in1 = m.Assign(ax, m.IAdd(ax, 1));
            m.Assign(CF, m.Cond(ax));
            var in3 = m.Assign(dx, m.IAdd(m.IAdd(dx, 0), CF));
            var instr = CreateLongInstruction(in1, in3);
            Assert.AreEqual("dx_ax = dx_ax + 0x00000001", instr.ToString());
        }

        [Test]
        public void Match_RegMem()
        {
            var addAxMem = m.Assign(ax, m.IAdd(ax, m.LoadW(m.IAdd(bx, 0x300))));
            var adcDxMem = m.Assign(
                dx,
                m.IAdd(
                    m.IAdd(
                        dx,
                        m.LoadW(m.IAdd(bx, 0x302))),
                    CF));

            var instr = CreateLongInstruction(addAxMem, adcDxMem);
            Assert.AreEqual("dx_ax = dx_ax + Mem0[bx + 0x0300:ui32]", instr.ToString());
        }

        [Test]
        public void MatchAdcSbc()
        {
            var adc = m.Assign(ax, m.IAdd(m.IAdd(ax, cx), CF));
            var regPair = rw.MatchAdcSbc(adc);
            Assert.AreSame(ax, regPair.Left);
            Assert.AreSame(cx, regPair.Right);
        }

        [Test]
        public void MatchAddSub()
        {
            var add = m.Assign(ax, m.IAdd(ax, cx));
            var regPair = rw.MatchAddSub(add);
            Assert.AreSame(ax, regPair.Left);
            Assert.AreSame(cx, regPair.Right);
        }

        [Test]
        public void Replace_AddReg()
        {
            m.Assign(ax, m.IAdd(ax, m.LoadW(m.IAdd(bx, 0x300))));
            m.Assign(CF, m.Cond(ax));
            m.Assign(dx, m.IAdd(m.IAdd(dx, m.LoadW(m.IAdd(bx, 0x302))), CF));
            m.Assign(CF, m.Cond(dx));
            var block = m.Block;
            m.Return();

            rw.ReplaceLongAdditions(block);

            var sExp = @"l1:
	ax = ax + Mem0[bx + 0x0300:word16]
	C = cond(ax)
	dx_ax = dx_ax + Mem0[bx + 0x0300:ui32]
	C = cond(dx_ax)
	return
";
            var sb = new StringWriter();
            block.Write(sb);
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void Avoid()
        {
            m.Assign(SCZ, m.Cond(m.ISub(cx, 0x0030)));
        	m.Assign(ax, m.IAdd(m.Word16(0x0000) ,CF));
            m.Assign(SCZ, m.Cond(ax));
            m.Assign(SCZ, m.Cond(m.ISub(cx , 0x003A)));
            m.Assign(CF, m.Not(CF));
            m.Assign(ax, m.IAdd(m.IAdd(ax, ax),CF));
            m.Assign(SCZ, m.Cond(ax));
            var block = m.Block;
            m.Return();

            rw.Transform();

            var sExp = @"l1:
	SCZ = cond(cx - 0x0030)
	ax = 0x0000 + C
	SCZ = cond(ax)
	SCZ = cond(cx - 0x003A)
	C = !C
	ax = ax + ax + C
	SCZ = cond(ax)
	return
";
            var sb = new StringWriter();
            block.Write(sb);
            Assert.AreEqual(sExp, sb.ToString());
        }
    }
}
