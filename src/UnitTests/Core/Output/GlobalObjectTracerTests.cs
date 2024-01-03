#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class GlobalObjectTracerTests
    {
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.program = new Program();
        }

        private void Given_Program(uint uAddrBase)
        {
            var addrBase = Address.Ptr32(uAddrBase);
            var sc = new ServiceContainer();
            var arch = new FakeArchitecture(sc);
            this.program = new Program(new SegmentMap(addrBase), arch, new DefaultPlatform(sc, arch));
        }

        private void Given_DataSegment(string name, uint uAddr, uint size)
        {
            var seg = new ImageSegment(
                name,
                new ByteMemoryArea(Address.Ptr32(uAddr), new byte[size]),
                AccessMode.ReadWrite);
            program.SegmentMap.AddSegment(seg);
        }

        private void Given_Data(uint uAddr, string hexBytes)
        {
            var addr = Address.Ptr32(uAddr);
            var isSeg = program.SegmentMap.TryFindSegment(addr, out var seg);
            Assert.IsTrue(isSeg);
            var w = program.Architecture.CreateImageWriter(seg.MemoryArea, addr);
            w.WriteBytes(BytePattern.FromHexBytes(hexBytes));
        }

        [Test]
        public void Gotr_Ptr()
        {
            Given_Program(0x0010_0000);
            Given_DataSegment(".data", 0x0010_1000, 0x4000);
            var wl = new WorkList<(StructureField, Address)>();
            var gotr = new GlobalObjectTracer(program, wl, new FakeDecompilerEventListener());
            Given_Data(0x0010_1000, "34 12 10 00");

            gotr.TraceObject(new Pointer(PrimitiveType.Int32, 32), Address.Ptr32(0x0010_1000));

            Assert.AreEqual(1, wl.Count);
        }
    }
}
