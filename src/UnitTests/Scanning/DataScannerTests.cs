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
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class DataScannerTests
    {
        private Mock<IProcessorArchitecture> arch;

        public DataScannerTests()
        { }

        [Test]
        public void DataScanner_Readstring()
        {
            var addrBase = Address.Ptr32(0x00123000);
            var mem = new MemoryArea(addrBase, new byte[0x1000]);
            var segmap = new SegmentMap(addrBase,
                new ImageSegment("code", mem, AccessMode.ReadWrite));
            Given_Architecture();
            var program = new Program(
                segmap,
                arch.Object,
                new Mock<IPlatform>().Object);
            var sr = new ScanResults();
            var addr = Address.Ptr32(0x00123400);
            Given_String(mem, addr, "Hello");

            var dsc = new DataScanner(program, sr, new FakeDecompilerEventListener());
            dsc.EnqueueUserGlobalData(addr, StringType.NullTerminated(PrimitiveType.Char), "sHello");
            dsc.ProcessQueue();

            var item = program.ImageMap.Items[addr];
            Assert.AreEqual(6, item.Size);
        }

        private void Given_String(MemoryArea mem, Address addr, string str)
        {
            var w = new LeImageWriter(mem, addr);
            var bytes = Encoding.ASCII.GetBytes(str);
            w.WriteBytes(bytes);
        }

        private void Given_Architecture()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("Fake");
            arch.Setup(a => a.CreateImageReader(
                It.IsNotNull<MemoryArea>(),
                It.IsNotNull<Address>())).
                Returns((MemoryArea m, Address a) => new LeImageReader(m, a)); 
        }
    }
}
