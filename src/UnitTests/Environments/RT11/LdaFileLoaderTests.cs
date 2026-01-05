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
using Reko.Arch.Pdp;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Environments.RT11;
using System;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Environments.RT11
{
    [TestFixture]
    public class LdaFileLoaderTests
    {
        private LdaFileLoader ldaLdr;

        private void Given_LdaFile(params byte [] bytes)
        {
            var services = new ServiceContainer();
            var arch = new Pdp11Architecture(services, "pdp11", []);
            var cfgSvc = new Mock<IConfigurationService>(MockBehavior.Strict);
            var pluginSvc = new Mock<IPluginLoaderService>();
            cfgSvc.Setup(svc => svc.GetEnvironment(It.IsAny<string>()))
                .Returns(new PlatformDefinition
                {
                    TypeName = typeof(RT11Platform).FullName!
                });
            pluginSvc.Setup(svc => svc.GetType(typeof(RT11Platform).FullName!))
                .Returns(typeof(RT11Platform));
            services.AddService<IConfigurationService>(cfgSvc.Object);
            services.AddService<IPluginLoaderService>(pluginSvc.Object);
            this.ldaLdr = new LdaFileLoader(services, ImageLocation.FromUri("file:foo.lda"), bytes);
        }

        [Test]
        public void LdaLdr_LoadEmpty()
        {
            Given_LdaFile(0x00, 0x00);

            try
            {
                var program = ldaLdr.Load(Address.Ptr16(0));
                Assert.Fail("Should have thrown an exception");
            }
            catch (BadImageFormatException)
            {
            }
        }

        [Test]
        public void LdaLdr_DataBlock()
        {
            Given_LdaFile(
                0x01, 0x00,
                0x08, 0x00,
                0x00, 0x10,
                0x12, 0x34,
                0x00,

                0x01, 0x00,
                0x06, 0x00,
                0x00, 0x10,
                0x00);

            var program = ldaLdr.LoadProgram(Address.Ptr16(0), null);
            var seg = program.SegmentMap.Segments.Values.First();
            var bmem = (ByteMemoryArea) seg.MemoryArea;
            Assert.AreEqual(0x1000ul, bmem.BaseAddress.ToLinear());
            Assert.AreEqual(0x12, bmem.Bytes[0]);
            Assert.AreEqual(0x34, bmem.Bytes[1]);
        }
    }
}
