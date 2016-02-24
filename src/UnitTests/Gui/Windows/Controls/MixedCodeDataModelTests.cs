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
                .Return(new MachineInstruction[]
                {
                    Instr(0x41000),
                    Instr(0x41002)
                });
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
                Size = (uint) size,
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
            var program = new Program(imageMap, arch, platform);

            Given_CodeBlock(memText.BaseAddress, 4);

            mr.ReplayAll();

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
}
