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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Machine;
using Reko.UserInterfaces.WindowsForms.Controls;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Gui.Windows.Controls
{
    [TestFixture]
    public class DisassemblyTextModelTests
    {
        private MockRepository mr;
        private DisassemblyTextModel model;
        private List<MachineInstruction> instrs;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            var arch = mr.Stub<IProcessorArchitecture>();
            arch.Stub(a => a.InstructionBitSize).Return(8);
            
            program = new Program
            {
                Architecture = arch
            };
        }

        private MemoryArea Given_MemoryArea(int size)
        {
            var bytes = Enumerable.Range(0, size).Select(b => (byte)b).ToArray();
            var mem = new MemoryArea(Address.Ptr32(0x1000000), bytes);
            return mem;
        }

        private ImageSegment Given_Image(int size)
        {
            var mem = Given_MemoryArea(size);
            var seg = new ImageSegment(".text", mem, AccessMode.ReadExecute);
            program.SegmentMap = new SegmentMap(mem.BaseAddress, seg);
            return seg;
        }

        private ImageSegment Given_Image(int memSize, int segOffset, uint segSize)
        {
            var mem = Given_MemoryArea(memSize);
            var seg = new ImageSegment(
                ".text",
                mem.BaseAddress + segOffset,
                mem,
                AccessMode.ReadExecute)
            {
                Size = segSize
            };
            program.SegmentMap = new SegmentMap(mem.BaseAddress, seg);
            return seg;
        }

        private void Given_Model(
            int memSize = 1000,
            int segOffset = 0,
            uint segSize = 0)
        {
            var seg = (segSize == 0) ?
                Given_Image(memSize) :
                Given_Image(memSize, segOffset, segSize);
            model = new DisassemblyTextModel(program, seg);
        }

        [Test]
        public void Dtm_GetFraction()
        {
            Given_Model();

            var frac = model.GetPositionAsFraction();

            Assert.AreEqual(0, frac.Item1);
        }

        [Test]
        public void Dtm_ReadItems()
        {
            Given_Model();
            Given_MachineInstructions(3);
            Given_Disassembler();
            mr.ReplayAll();

            var items = model.GetLineSpans(3);

            Assert.AreEqual(3, items.Length);
            mr.VerifyAll();
        }

        [Test]
        public void Dtm_SetPositionAsFraction_SharedMemory()
        {
            Given_Model(1000, 2, 3);
            Given_MachineInstructions(3);
            Given_Disassembler();
            mr.ReplayAll();

            model.SetPositionAsFraction(3, 4);

            Assert.AreEqual("01000004", model.CurrentPosition.ToString());
            mr.VerifyAll();
        }

        [Test]
        public void Dtm_ReadItems_DisplayAddress()
        {
            Given_Model();
            Given_MachineInstructions(3);
            Given_Disassembler();
            mr.ReplayAll();

            var items = model.GetLineSpans(1);

            var line = items[0];
            Assert.AreEqual("01000000 ", line.TextSpans[0].GetText());
            mr.VerifyAll();
        }

        [Test]
        public void Dtm_ReadItems_DisplayInstructionBytes()
        {
            Given_Model();
            Given_MachineInstructions(3);
            Given_Disassembler();
            mr.ReplayAll();

            var items = model.GetLineSpans(3);

            var line = items[2];
            Assert.AreEqual("02 03 04 ", line.TextSpans[1].GetText());
            mr.VerifyAll();
        }

        public void Dtm_ReadItems_DisplayInstructionOpcode()
        {
            Given_Model();
            Given_MachineInstructions(3);
            Given_Disassembler();
            mr.ReplayAll();

            var items = model.GetLineSpans(3);

            var line = items[3];
            Assert.AreEqual("02 03 04 ", line.TextSpans[1].GetText());
            mr.VerifyAll();
        }

        private void Given_Disassembler()
        {
            program.Architecture.Stub(a => a.CreateImageReader(null, null)).IgnoreArguments()
                .Do(new Func<MemoryArea, Address, EndianImageReader>((i, a) => new LeImageReader(i, a)));
            program.Architecture.Stub(a => a.CreateDisassembler(Arg<EndianImageReader>.Is.NotNull))
                .Return(instrs);
        }

        private class TestInstruction : MachineInstruction
        {
            public override int OpcodeAsInteger
            {
                get { throw new NotImplementedException(); }
            }

            public override MachineOperand GetOperand(int i)
            {
                throw new NotImplementedException();
            }

            public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
            {
                writer.WriteOpcode("opcode.l");
            }

            public override InstructionClass InstructionClass
            {
                get { return InstructionClass.Invalid; } 
            }

            public override bool IsValid
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void Given_MachineInstructions(int c)
        {
            instrs = new List<MachineInstruction>();
            for (int i = 0; i < c; ++i)
            {
                instrs.Add(new TestInstruction
                {
                    Address = program.SegmentMap.BaseAddress + i,
                    Length = c % 5
                });
            }
        }
    }
}
