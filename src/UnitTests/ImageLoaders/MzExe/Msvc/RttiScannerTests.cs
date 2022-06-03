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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.ImageLoaders.MzExe.Msvc;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.MzExe.Msvc
{
    [TestFixture]
    public class RttiScannerTests
    {
        private ByteMemoryArea mem = default!;
        private Program program = default!;
        private DecompilerEventListener listener = default!;

        [SetUp]
        public void Setup()
        {
            this.mem = new ByteMemoryArea(Address.Ptr32(0x1000), new byte[256]);
            var segments = new SegmentMap(new ImageSegment(".text", mem, AccessMode.ReadExecute));
            var arch = new Mock<IProcessorArchitecture>(MockBehavior.Strict);
            var platform = new Mock<IPlatform>(MockBehavior.Strict);
            arch.Setup(a => a.Name).Returns("fakeArch");
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<MemoryArea>(),
                It.IsAny<Address>())).Returns(new Func<MemoryArea, Address, EndianImageReader>(
                    (m, a) => mem.CreateLeReader(a)));
            this.program = new Program(segments, arch.Object, platform.Object);
            this.listener = new FakeDecompilerEventListener();
        }

        private LeImageWriter CreateWriterAt(uint uAddr)
        {
            var w = new LeImageWriter(mem.Bytes);
            w.Position = (int) (uAddr - mem.BaseAddress.ToLinear());
            return w;
        }

        private void Given_Col32(uint uAddr, uint uAddrTypeDescriptor, uint uAddrClassDescriptor)
        {
            LeImageWriter w = CreateWriterAt(uAddr);
            w.WriteLeUInt32(0); // signature.
            w.WriteLeUInt32(0); // offset of vtable within class.
            w.WriteLeUInt32(0); // constructor displacement offset.
            w.WriteLeUInt32(uAddrTypeDescriptor);
            w.WriteLeUInt32(uAddrClassDescriptor);
        }

        private void Given_TypeDescriptor(uint uAddr, uint uAddr_VFtable, string name)
        {
            var w = CreateWriterAt(uAddr);
            w.WriteLeUInt32(uAddr_VFtable); // VFTable pointer
            w.WriteLeUInt32(0);             // always zero
            w.WriteString(name, Encoding.UTF8);
            w.WriteByte(0);
        }

        [Test]
        public void Rttis_col_sniff()
        {
            const uint ptrCol = 0x1000;
            const uint ptrTypeDesc = 0x1020;
            const uint ptrVFtable = 0x1038;
            const uint ptrClassDesc = 0x1080;


            Given_Col32(ptrCol, ptrTypeDesc, ptrClassDesc);
            Given_TypeDescriptor(ptrTypeDesc, ptrVFtable, ".?Atest");

            var rttis = new RttiScanner(program, listener);
            var result =  rttis.Scan();
        }
    }
}
