#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decompiler.UnitTests.Analysis
{
    [TestFixture]
    public class LongAddRewriterTests
    {
        private Frame frame;
        private LongAddRewriter rw;
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

        public LongAddRewriterTests()
        {
            arch = new ArchitectureMock();
        }

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder(arch);
            frame = m.Frame;
            ax = frame.EnsureRegister(new RegisterStorage("ax", 0, PrimitiveType.Word16));
            bx = frame.EnsureRegister(new RegisterStorage("bx", 3, PrimitiveType.Word16));
            cx = frame.EnsureRegister(new RegisterStorage("cx", 1, PrimitiveType.Word16));
            dx = frame.EnsureRegister(new RegisterStorage("dx", 2, PrimitiveType.Word16));
            SCZ = frame.EnsureFlagGroup(7, "SCZ", PrimitiveType.Byte);
            CF = frame.EnsureFlagGroup(arch.CarryFlagMask, "C", PrimitiveType.Bool);
            rw = new LongAddRewriter(arch, frame);
            Procedure proc = new Procedure("test", frame);
            block = new Block(proc, "bloke");
        }

        [Test]
        public void FindCond()
        {
            m.Assign(ax, m.Add(ax, cx));
            m.Assign(SCZ, m.Cond(ax));
            var block = m.CurrentBlock;
            m.Return();

            rw = new LongAddRewriter(arch, m.Frame);
            var cm = rw.FindCond(block.Statements, 0, ax);

            Assert.AreEqual("SCZ", cm.FlagGroup.ToString());
            Assert.AreEqual(1, cm.StatementIndex);
        }

        [Test]
        public void FindInstructionUsesCond()
        {
            m.Assign(ax, m.Add(ax, cx));
            m.Assign(SCZ, m.Cond(ax));
            var block = m.CurrentBlock;
            m.Assign(dx, m.Add(m.Add(dx, bx), CF));
            m.Return();

            rw = new LongAddRewriter(arch, m.Frame);
            var cm = rw.FindCond(block.Statements, 0, ax);
            int i = rw.IndexOfUsingOpcode(block.Statements, cm.StatementIndex, Operator.Add);
            Assert.AreEqual("dx = dx + bx + C", block.Statements[i].Instruction.ToString());
        }

        [Test]
        public void MatchAddRegMem()
        {
            var addAxMem = m.Assign(ax, m.Add(ax, m.LoadW(m.Add(bx, 0x300))));
            var adcDxMem = m.Assign(
                dx,
                m.Add(
                    m.Add(
                        dx,
                        m.LoadDw(m.Add(bx, 0x302))),
                    CF));

            var instr = rw.Match(addAxMem, adcDxMem);
            Assert.AreEqual("dx_ax = dx_ax + Mem0[bx + 0x0300:ui32]", instr.ToString());
        }

        [Test]
        public void MatchAddRecConst()
        {
            var i1 = m.Assign(ax, m.Add(ax, 0x5678));
            var i2 = m.Assign(CF, m.Cond(ax));
            var i3 = m.Assign(dx, m.Add(m.Add(dx, 0x1234), CF));
            var instr = rw.Match(i1, i3);
            Assert.AreEqual("dx_ax = dx_ax + 0x12345678", instr.ToString());
        }

        [Test]
        public void Adc_Constant()
        {
            var in1 = m.Assign(ax, m.Add(ax, 1));
            var in2 = m.Assign(CF, m.Cond(ax));
            var in3 = m.Assign(dx, m.Add(m.Add(dx, 0), CF));
            var instr = rw.Match(in1, in3);
            Assert.AreEqual("dx_ax = dx_ax + 0x00000001", instr.ToString());
        }

        [Test]
        public void CreateInstruction()
        {
            var addAxMem = m.Assign(ax, m.Add(ax, m.LoadW(m.Add(bx, 0x300))));
            var adcDxMem = m.Assign(
                dx,
                m.Add(
                    m.Add(
                        dx,
                        m.LoadW(m.Add(bx, 0x302))),
                    CF));

            var instr = rw.Match(addAxMem, adcDxMem);
            Assert.AreEqual("dx_ax = dx_ax + Mem0[bx + 0x0300:ui32]", instr.ToString());
        }

        [Test]
        public void MatchAdc()
        {
            var adc = m.Assign(ax, m.Add(m.Add(ax, cx), CF));
            var regPair = rw.MatchAdcSbc(adc);
            Assert.AreSame(ax, regPair.Left);
            Assert.AreSame(cx, regPair.Right);
        }

        [Test]
        public void MatchAdd()
        {
            var add = m.Assign(ax, m.Add(ax, cx));
            var regPair = rw.MatchAddSub(add);
            Assert.AreSame(ax, regPair.Left);
            Assert.AreSame(cx, regPair.Right);
        }
    }
}
