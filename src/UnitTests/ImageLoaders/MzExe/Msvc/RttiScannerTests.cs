#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
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
        private IEventListener listener = default!;

        [SetUp]
        public void Setup()
        {
            this.mem = new ByteMemoryArea(Address.Ptr32(0x1000), new byte[256]);
            var segments = new SegmentMap(new ImageSegment(".text", mem, AccessMode.ReadExecute));
            var platform = new Mock<IPlatform>(MockBehavior.Strict);
            var arch = new Mock<IProcessorArchitecture>(MockBehavior.Strict);
            arch.Setup(a => a.Name).Returns("fakeArch");
            arch.Setup(a => a.Endianness).Returns(EndianServices.Little);
            arch.Setup(a => a.TryCreateImageReader(
                It.IsAny<IMemory>(),
                It.IsAny<Address>(),
                out It.Ref<EndianImageReader>.IsAny))
                .Callback(new CreateReaderDelegate((IMemory m, Address a, out EndianImageReader r) =>
                    m.TryCreateLeReader(a, out r)))
                .Returns(true);
            arch.Setup(a => a.TryCreateImageReader(
                It.IsAny<IMemory>(),
                It.IsAny<Address>(),
                It.IsAny<long>(),
                out It.Ref<EndianImageReader>.IsAny))
                .Callback(new CreateSizedReaderDelegate((IMemory m, Address a, long b, out EndianImageReader r) =>
                    m.TryCreateLeReader(a, b, out r)))
                .Returns(true);
            arch.Setup(a => a.PointerType).Returns(PrimitiveType.Ptr32);
            this.program = new Program(new ByteProgramMemory(segments), arch.Object, platform.Object);
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

        private void Given_VFTable_At(uint uAddr, params uint[] fnptrs)
        {
            var w = CreateWriterAt(uAddr);
            foreach (var ptr in fnptrs)
            {
                w.WriteLeUInt32(ptr);
            }
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

        [Test]
        public void Rttis_scan_vtable_ending_in_junk()
        {
            const uint ptrCol = 0x1030;
            const uint ptrTypeDesc = 0x1040;

            Given_VFTable_At(0x1010,
                ptrCol,
                0x1042,
                0x1047,
                0x1043,
                0x64322121);        // junk following vtable.
            var rttis = new RttiScanner(program, listener);
            var col = new CompleteObjectLocator(
                Address.Ptr32(ptrCol), 4, 4, Address.Ptr32(ptrTypeDesc), Address.Ptr32(0x1020));
            var td = new TypeDescriptor(
                Address.Ptr32(ptrTypeDesc), Address.Ptr32(0x1008), "bob.class");
            var result = new RttiScannerResults
            {
                CompleteObjectLocators = { { col.Address, col } },
                TypeDescriptors = { { td.Address, td} }
            };
            rttis.ScanVFTables(result);
            Assert.AreEqual(3, result.VFTables[Address.Ptr32(0x1014)].Count);
        }
    }
}
