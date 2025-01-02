#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Configuration;
using Reko.ImageLoaders.Omf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Omf
{
    [TestFixture]
    public class OmfObjectLoaderTests
    {
        private readonly Mock<IProcessorArchitecture> arch;
        private readonly Mock<IPlatform> platform;

        public OmfObjectLoaderTests()
        {
            var configSvc = new Mock<IConfigurationService>();
            this.arch = new Mock<IProcessorArchitecture>();
            this.arch.Setup(a => a.Name).Returns("x86-real-16");
            this.platform = new Mock<IPlatform>();
        }

        [Test]
        public void Omfol_Minimal()
        {
            var omf = new OmfRecord[]
            {
                new TheaderRecord("test.asm"),
                new LnamesRecord(new List<string>() { "", "segname2"}),
                new LnamesRecord(new List<string>() { "name3"}),
                new SegdefRecord(3, 3, 0x30, 2, 3),
                new PubdefRecord(0, 1, new List<PublicName>()
                {
                    new PublicName("method1", 0, 0),
                    new PublicName("method2", 0x10, 0)
                }),
                new DataRecord(1, 0x0, Encoding.ASCII.GetBytes("method1")),
                new DataRecord(1, 0x10, Encoding.ASCII.GetBytes("method2")),
            };
            var omfol = new OmfObjectLoader(
                new ServiceContainer(),
                null,
                Array.Empty<byte>());
            var program = omfol.LoadProgram(
                omf.AsEnumerable<OmfRecord>(),
                Address.SegPtr(0x800, 0),
                arch.Object,
                platform.Object);

            Assert.AreEqual(2, program.ImageSymbols.Count);
        }
    }
}
