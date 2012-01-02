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

using Decompiler.Core;
using Decompiler.Core.Code;
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

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    [Ignore("Should be moved to analysis, because we need data flow analysis to trace the uses of carry flags.")]
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
            frame = arch.CreateFrame();
            ax = frame.EnsureRegister(new MachineRegister("ax", 0, PrimitiveType.Word16));
            bx = frame.EnsureRegister(new MachineRegister("bx", 3, PrimitiveType.Word16));
            cx = frame.EnsureRegister(new MachineRegister("Cx", 1, PrimitiveType.Word16));
            dx = frame.EnsureRegister(new MachineRegister("dx", 2, PrimitiveType.Word16));
            SCZ = frame.EnsureFlagGroup(7, "SCZ", PrimitiveType.Byte);
            CF = frame.EnsureFlagGroup(1, "C", PrimitiveType.Bool);
            m = new ProcedureBuilder();
            rw = new LongAddRewriter(arch, frame);
            Procedure proc = new Procedure("test", frame);
            block = new Block(proc, "bloke");
        }

        [Test]
        public void FindCarryLinkedInstructions()
        {
            m.Assign(ax, m.Add(ax, cx));
            m.Assign(SCZ, m.Cond(ax));
            m.Assign(dx, m.Add(m.Add(dx, 0), CF));
            m.Assign(SCZ, m.Cond(dx));

            var instrs = rw.FindCarryLinkedInstructions(m.CurrentBlock).ToArray();
            Assert.AreEqual(1, instrs.Length);
            Assert.AreEqual("ax = ax + cx", instrs[0].Low);
            Assert.AreEqual("dx = dx + 0x0000 + C", instrs[0].High);
        }

        [Test]
        public void MatchAddRegMem()
        {
            var addAxMem = m.Assign(ax, m.Add(ax, m.LoadW(m.Add(bx, 0x300))));
            var adcDxMem = m.Assign(
                dx,
                m.Add(
                    m.Add(
                        ax,
                        m.LoadDw(m.Add(bx, 0x302))),
                    frame.EnsureFlagGroup(arch.CarryFlagMask, "C", PrimitiveType.Bool)));

            Assert.IsTrue(rw.Match(addAxMem, adcDxMem));
            Assert.AreEqual("dx_ax", rw.Dst.ToString());
            Assert.AreEqual("Mem0[ds:bx + 0x0300:ui32]", rw.Src.ToString());
        }

        [Test]
        public void MatchAddRecConst()
        {
            var i1 = m.Assign(ax, m.Add(ax, 0x5678));
            var i2 = m.Assign(CF, m.Cond(ax));
            var i3 = m.Assign(dx, m.Add(m.Add(dx, 0x1234), CF));
            Assert.IsTrue(rw.Match(i1, i2));
            Assert.AreEqual("0x12345678", rw.Src.ToString());
        }


        [Test]
        public void Adc1()
        {
            var in1 = m.Assign(ax, m.Add(ax, 1));
            var in2 = m.Assign(CF, m.Cond(ax));
            var in3 = m.Assign(dx, m.Add(m.Add(dx, 0), CF));
            Assert.IsTrue(rw.Match(in1, in2));
            Assert.AreEqual("dx_ax", rw.Dst.ToString());
            Assert.AreEqual("0x00000001", rw.Src.ToString());
        }

        [Test]
        public void CreateInstruction()
        {
            var addAxMem = m.Assign(ax, m.Add(ax, m.LoadW(m.Add(bx, 0x300))));
            var adcDxMem = m.Assign(
                dx,
                m.Add(
                    m.Add(
                        ax,
                        m.LoadDw(m.Add(bx, 0x302))),
                    frame.EnsureFlagGroup(arch.CarryFlagMask, "C", PrimitiveType.Bool)));

            rw.Match(addAxMem, adcDxMem);
            rw.EmitInstruction(Operator.Add, m);
            Assert.AreEqual("dx_ax = dx_ax + Mem0[ds:bx + 0x0300:ui32]", block.Statements[0].ToString());
        }
    }
}
