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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.X86.Assembler
{
    [TestFixture]
    public class X86TextAssemblerTests
    {
        private Program program;
        private ByteMemoryArea bmem;

        [SetUp]
        public void Setup()
        {
        }

        private void Given_Program()
        {
            var addrBase = Address.Ptr32(0x00100000);
            this.bmem = new ByteMemoryArea(addrBase, new byte[256]);
            var segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment("code", bmem, AccessMode.ReadWriteExecute));
            var arch = (IProcessorArchitecture) new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32", new Dictionary<string, object>());
            this.program = new Program(new ByteProgramMemory(segmentMap), arch, null);
        }

        [Test]
        public void X86Asm_AssembleAt()
        {
            Given_Program();

            var asm = program.Architecture.CreateAssembler(null);

            asm.AssembleFragmentAt(program, Address.Ptr32(0x00100002), "ret");

            Assert.AreEqual(0xC3, bmem.Bytes[2]);
        }

        [Test]
        public void X86Asm_AssembleAt_PICBranch()
        {
            Given_Program();

            var asm = program.Architecture.CreateAssembler(null);

            asm.AssembleFragmentAt(program, Address.Ptr32(0x00100002), "lupe: jmp lupe");

            Assert.AreEqual(0xE9, bmem.Bytes[2]);
            Assert.AreEqual(0xFB, bmem.Bytes[3]);
            Assert.AreEqual(0xFF, bmem.Bytes[4]);
            Assert.AreEqual(0xFF, bmem.Bytes[5]);
            Assert.AreEqual(0xFF, bmem.Bytes[6]);
            Assert.AreEqual(0x00, bmem.Bytes[7]);
        }
    }
}
