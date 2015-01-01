#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Machine;
using Decompiler.Gui.Controls;
using Decompiler.Gui.Windows.Controls;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Gui.Windows.Controls
{
    [TestFixture]
    public class DisassemblyTextModelTests
    {
        private IProcessorArchitecture arch;
        private LoadedImage image;
        private MockRepository mr;
        private DisassemblyTextModel model;
        private List<MachineInstruction> instrs;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            arch = mr.Stub<IProcessorArchitecture>();
        }

        private LoadedImage Image(int size)
        {
            var bytes = Enumerable.Range(0, size).Select(b => (byte)b).ToArray();
            image = new LoadedImage(new Address(0x1000000), bytes);
            return image;
        }

        private void Given_Model()
        {
            Image(1000);
            var imageMap = new ImageMap(image.BaseAddress, image.Bytes.Length);
             model = new DisassemblyTextModel(arch, image , imageMap);
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
        public void Dtm_ReadItems_DisplayAddress()
        {
            Given_Model();
            Given_MachineInstructions(3);
            Given_Disassembler();
            mr.ReplayAll();

            var items = model.GetLineSpans(1);

            var line = items[0];
            Assert.AreEqual("01000000 ", line[0].GetText());
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
            Assert.AreEqual("02 03 04 ", line[1].GetText());
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
            Assert.AreEqual("02 03 04 ", line[1].GetText());
            mr.VerifyAll();
        }

        private void Given_Disassembler()
        {
            arch.Stub(a => a.CreateImageReader(null, null)).IgnoreArguments()
                .Do(new Func<LoadedImage, Address, ImageReader>((i, a) => new LeImageReader(i, a)));
            arch.Stub(a => a.CreateDisassembler(Arg<ImageReader>.Is.NotNull))
                .Return(instrs);
        }

        private class TestInstruction : MachineInstruction
        {
            public override void Render(MachineInstructionWriter writer)
            {
                writer.WriteOpcode("opcode.l");
            }
        }

        private void Given_MachineInstructions(int c)
        {
            instrs = new List<MachineInstruction>();
            for (int i = 0; i < c; ++i)
            {
                instrs.Add(new TestInstruction
                {
                    Address = image.BaseAddress + i,
                    Length = c % 5
                });
            }
        }
    }
}
