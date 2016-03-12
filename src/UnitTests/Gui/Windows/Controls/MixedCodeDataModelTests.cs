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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Gui.Windows.Controls;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Gui.Windows.Controls
{
    [TestFixture]
    public class MixedCodeDataModelTests
    {
        private MockRepository mr;
        private IProcessorArchitecture arch;
        private IPlatform platform;
        private ImageMap imageMap;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.arch = mr.Stub<IProcessorArchitecture>();
            this.platform = mr.Stub<IPlatform>();
            this.platform.Stub(p => p.Architecture).Return(arch);
            this.arch.Stub(a => a.CreateImageReader(null, null))
                .IgnoreArguments()
                .Do(new Func<MemoryArea, Address, ImageReader>((m, a) => new LeImageReader(m, a)));
            this.arch.Stub(a => a.CreateDisassembler(null))
                .IgnoreArguments()
                .Do(new Func<ImageReader, IEnumerable<MachineInstruction>>((rdr) => new MachineInstruction[]
                {
                    Instr(rdr.Address.ToUInt32()),
                    Instr(rdr.Address.ToUInt32()+2)
                }));
        }

        private void Given_Program()
        {
            this.program = new Program(imageMap, arch, platform);
        }

        private FakeInstruction Instr(uint addr)
        {
            var reg = new RegisterStorage("r2", 2, 0, PrimitiveType.Word32);
            return new FakeInstruction(Operation.Add, new RegisterOperand(reg), new RegisterOperand(reg))
            {
                Address = Address.Ptr32(addr),
                Length = 2,
            };
        }

        private void Given_CodeBlock(Address addr, int size)
        {
            imageMap.AddItem(addr, new ImageMapBlock
            {
                Address = addr,
                DataType = new CodeType(),
                Size = (uint)size,
            });
        }

        [Test]
        public void Mcdm_AddDasm()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[8]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.imageMap = new ImageMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 2);
            mr.ReplayAll();

            this.imageMap.Dump();
            var mcdm = new MixedCodeDataModel(program);
            var lines = mcdm.GetLineSpans(2);
            Assert.AreEqual(2, lines.Length);
        }

        [Test]
        public void Mcdm_AddDasmAndMemory()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[8]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.imageMap = new ImageMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(imageMap, arch, platform);

            Given_CodeBlock(memText.BaseAddress, 4);

            mr.ReplayAll();

            var mcdm = new MixedCodeDataModel(program);
            var lines = mcdm.GetLineSpans(2);
            Assert.AreEqual(2, lines.Length);
        }

        [Test]
        public void Mcdm_Move()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[8]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.imageMap = new ImageMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(imageMap, arch, platform);

            Given_CodeBlock(memText.BaseAddress, 4);

            mr.ReplayAll();

            var mcdm = new MixedCodeDataModel(program);
            var lines = mcdm.GetLineSpans(2);
            Assert.AreEqual(2, lines.Length);
        }

        [Test]
        public void Mcdm_FindInstructionIndex()
        {
            var instrs = new MachineInstruction[]
            {
                new FakeInstruction(Operation.Add) { Address = Address.Ptr32(0x1000), Length = 2 },
                new FakeInstruction(Operation.Add) { Address = Address.Ptr32(0x1002), Length = 2 },
            };
            Func<uint, int> Idx = u =>
                MixedCodeDataModel.FindIndexOfInstructionAddress(
                    instrs,
                    Address.Ptr32(u));
            Assert.AreEqual(-1, Idx(0x0FFF));
            Assert.AreEqual(0, Idx(0x1000));
            Assert.AreEqual(0, Idx(0x1001));
            Assert.AreEqual(1, Idx(0x1002));
            Assert.AreEqual(1, Idx(0x1003));
            Assert.AreEqual(-1, Idx(0x1004));
        }

        [Test]
        public void Mcdm_MoveForward()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[4]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[32]);
            this.imageMap = new ImageMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(imageMap, arch, platform);

            Given_CodeBlock(memText.BaseAddress, 4);

            mr.ReplayAll();

            imageMap.Dump();

            var mcdm = new MixedCodeDataModel(program);
            // Advance 1 line into another piece of code.
            int delta = mcdm.MoveToLine(mcdm.CurrentPosition, 1);
            Assert.AreEqual(1, delta);
            // move another line of code and then into data.
            delta = mcdm.MoveToLine(mcdm.CurrentPosition, 2);
            Assert.AreEqual(2, delta);
            Assert.AreEqual("00042010", mcdm.CurrentPosition.ToString());
            // Another line of data
            delta = mcdm.MoveToLine(mcdm.CurrentPosition, 1);
            Assert.AreEqual("00042020", mcdm.CurrentPosition.ToString());
            Assert.AreEqual(1, delta);
            // Pegged at end
            delta = mcdm.MoveToLine(mcdm.CurrentPosition, 1);
            Assert.AreEqual("00042020", mcdm.CurrentPosition.ToString());
            Assert.AreEqual(0, delta);

            mcdm.MoveToLine(mcdm.StartPosition, 2);
            Assert.AreEqual("00042000", mcdm.CurrentPosition.ToString());
        }

        [Test]
        public void Mcdm_MoveToLastLineOfItem()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x40FD5), new byte[64]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[32]);
            this.imageMap = new ImageMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(imageMap, arch, platform);

            Given_CodeBlock(Address.Ptr32(0x40FF9), 4);

            mr.ReplayAll();

            var mcdm = new MixedCodeDataModel(program);

            int delta = mcdm.MoveToLine(mcdm.CurrentPosition, 2);
            var curPos = (Address)mcdm.CurrentPosition;

/*
            ***************start position**************************
            0x40FD5                FF FF FF FF FF FF FF FF FF FF FF
            0x40FE0 FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF
            *********curent position after moving******************
            0x40FF0 FF FF FF FF FF FF FF FF FF
            0x40FF9 add r2,r2
            0x40FFA add r2,r2
*/

            Assert.AreEqual("00040FF0", curPos.ToString());
        }

        [Test]
        public void Mcdm_MoveFraction()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[4]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[32]);
            this.imageMap = new ImageMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(imageMap, arch, platform);

            Given_CodeBlock(memText.BaseAddress, 4);

            mr.ReplayAll();

            var mcdm = new MixedCodeDataModel(program);
            Debug.Print("LineCount: {0}", mcdm.LineCount);
            mcdm.SetPositionAsFraction(0, 1);
            Assert.AreSame(mcdm.StartPosition, mcdm.CurrentPosition);
            mcdm.SetPositionAsFraction(2, 1);
            Assert.AreSame(mcdm.EndPosition, mcdm.CurrentPosition);
            mcdm.SetPositionAsFraction(1, 2);
            Assert.AreEqual("00042000", mcdm.CurrentPosition.ToString());
        }
    }
}
