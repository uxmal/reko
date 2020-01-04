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
using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Environments.SegaGenesis;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Environments.SegaGenesis
{
    [TestFixture]
    public class RomLoaderTests
    {
        private void Given_AbsoluteMemoryMap(SegaGenesisPlatform platform)
        {
            platform.MemoryMap = new MemoryMap_v1
            {
                Segments = new MemorySegment_v1[]
                {
                    new MemorySegment_v1
                    {
                        Address = "0",
                        Name= ".text",
                        Attributes = "rx",
                        Size = "400000",
                    },
                     new MemorySegment_v1
                    {
                        Address = "E00000",
                        Name= ".data",
                        Attributes = "rwx",
                        Size = "200000",
                    },
                }
            };
        }

        [Test]
        public void Sgrom_LoadImage()
        {
            var sc = new ServiceContainer();
            var cfgSvc = new Mock<IConfigurationService>();
            var openv = new Mock<PlatformDefinition>();
            var diagSvc = new Mock<IDiagnosticsService>();
            var arch = new M68kArchitecture("m68k");
            var platform = new SegaGenesisPlatform(sc, arch);
            cfgSvc.Setup(c => c.GetArchitecture("m68k")).Returns(arch);
            cfgSvc.Setup(c => c.GetEnvironment("sega-genesis")).Returns(openv.Object);
            openv.Setup(o => o.Load(sc, arch)).Returns(platform);
            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<IDiagnosticsService>(diagSvc.Object);
            Given_AbsoluteMemoryMap(platform);

            var rawBytes = new byte[0x300];
            var sgrom = new RomLoader(sc, "foo.bin", rawBytes);
            var program = sgrom.Load(Address.Ptr32(0));

            var romSegment = program.SegmentMap.Segments.Values.First();
            Assert.IsNotNull(romSegment.MemoryArea, "ROM image should have been loaded into first segment");
            Assert.AreSame(rawBytes, romSegment.MemoryArea.Bytes, "ROM image should have been loaded into first segment");
            Assert.AreEqual(rawBytes.Length, romSegment.ContentSize);
            var ramSegment = program.SegmentMap.Segments.Values.First(s => s.Name == ".data");
            Assert.IsNotNull(ramSegment.MemoryArea, "RAM segment should have a MemoryArea");
        }
    }
}
