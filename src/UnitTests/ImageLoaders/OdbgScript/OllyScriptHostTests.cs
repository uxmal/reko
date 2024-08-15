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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.ImageLoaders.OdbgScript;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.OdbgScript
{
    [TestFixture]
    public class OllyScriptHostTests
    {
        private Program program;
        private OdbgScriptHost host;
        private IServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.program = null;
            this.host = null;
            this.sc = new ServiceContainer();
        }

        private void Given_X86Program()
        {
            var arch = new Reko.Arch.X86.X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            var addrBase = Address.Ptr32(0x00100000);
            var segmentMap = new SegmentMap(addrBase);
            segmentMap.AddSegment(new ByteMemoryArea(addrBase, new byte[0xFF]), ".text", AccessMode.ReadWrite);
            this.program = new Program(new ProgramMemory(segmentMap), arch, null);
        }

        private void Given_Host()
        {
            this.host = new OdbgScriptHost(sc, null, program);
        }

        [Test]
        public void OdbgHost_Alloc()
        {
            Given_X86Program();
            Given_Host();

            var addr = host.AllocateMemory(4);
            Assert.AreEqual(0x00100100, addr.ToUInt32()); 
        }

        [Test]
        public void OdbgHost_Alloc_TwoAllocations()
        {
            Given_X86Program();
            Given_Host();

            var addr1 = host.AllocateMemory(4);
            var addr2 = host.AllocateMemory(4);
            Assert.AreEqual(0x00100100, addr1.ToUInt32());
            Assert.AreEqual(0x00100110, addr2.ToUInt32());
        }

        [Test]
        public void OdbgHost_Asm()
        {
            Given_X86Program();
            Given_Host();

            var len = host.Assemble("mov bl,03", Address.Ptr32(0x0010_0002));
            Assert.AreEqual(2, len);
        }

        [Test]
        public void OdbgHost_Asm_invalid_instruction()
        {
            Given_X86Program();
            Given_Host();

            var len = host.Assemble("zlorgo", Address.Ptr32(0x0010_0002));
            Assert.AreEqual(0, len);
        }
    }
}
