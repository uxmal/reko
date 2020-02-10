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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.X86.Assembler
{
    [TestFixture]
    public class X86TextAssemblerTests
    {
        private Program program;
        private MemoryArea mem;

        [SetUp]
        public void Setup()
        {
        }

        private void Given_Program()
        {
            var addrBase = Address.Ptr32(0x00100000);
            this.mem = new MemoryArea(addrBase, new byte[256]);
            var segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            var arch = (IProcessorArchitecture) new X86ArchitectureFlat32("x86-protected-32");
            this.program = new Program(segmentMap, arch, null);
        }

        [Test]
        public void X86Asm_AssembleAt()
        {
            Given_Program();

            var asm = program.Architecture.CreateAssembler(null);

            asm.AssembleFragmentAt(program, Address.Ptr32(0x00100002), "ret");

            Assert.AreEqual(0xC3, mem.Bytes[2]);
        }

        [Test]
        public void X86Asm_AssembleAt_PICBranch()
        {
            Given_Program();

            var asm = program.Architecture.CreateAssembler(null);

            asm.AssembleFragmentAt(program, Address.Ptr32(0x00100002), "lupe: jmp lupe");

            Assert.AreEqual(0xE9, mem.Bytes[2]);
            Assert.AreEqual(0xFB, mem.Bytes[3]);
            Assert.AreEqual(0xFF, mem.Bytes[4]);
            Assert.AreEqual(0xFF, mem.Bytes[5]);
            Assert.AreEqual(0xFF, mem.Bytes[6]);
            Assert.AreEqual(0x00, mem.Bytes[7]);
        }
    }
}
