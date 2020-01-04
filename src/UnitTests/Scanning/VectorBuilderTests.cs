#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class VectorBuilderTests
    {
        private Program program;
        private MemoryArea mem;
        private ServiceContainer sc;
        private Mock<IProcessorArchitecture> arch;

        [SetUp]
        public void Setup()
        {
            var eventListener = new Mock<DecompilerEventListener>();
            sc = new ServiceContainer();
            sc.AddService<DecompilerEventListener>(eventListener.Object);
        }

        private void Given_Program(byte [] bytes)
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.arch.Setup(a => a.Name).Returns("FakeArch");
            arch.Setup(a => a.ReadCodeAddress(
                It.IsAny<int>(),
                It.IsNotNull<EndianImageReader>(),
                It.IsAny<ProcessorState>()))
                .Returns((int s, EndianImageReader r, ProcessorState st) =>
                     Address.Ptr32(r.ReadLeUInt32()));
            mem = new MemoryArea(Address.Ptr32(0x00010000), bytes);
            this.program = new Program(
                new SegmentMap(mem.BaseAddress,
                    new ImageSegment(".text", mem, AccessMode.ReadExecute)),
                arch.Object,
                null);
        }

        [Test(Description = "Should create a list of vector destinations")]
        public void Vb_CreateVector_ModifiesImageMap()
        {
            Given_Program(new byte[]
            {
                0x10, 0x00, 0x01, 0x00,
                0x11, 0x00, 0x01, 0x00,
                0x12, 0x00, 0x01, 0x00,
                0xCC, 0xCC, 0xCC, 0xCC,
                0xC3, 0xC3, 0xC3, 0xCC,
            });
            var scanner = new Mock<IScanner>();
            scanner.Setup(s => s.Services).Returns(sc);
            arch.Setup(s => s.CreateImageReader(this.mem, this.program.ImageMap.BaseAddress))
                .Returns(this.mem.CreateLeReader(0));
            var state = new FakeProcessorState(arch.Object);
        
            var vb = new VectorBuilder(scanner.Object.Services, program, new DirectedGraphImpl<object>());
            var vector = vb.BuildTable(this.program.ImageMap.BaseAddress, 12, null, 4, state);
            Assert.AreEqual(3, vector.Count);
        }
    }
}
