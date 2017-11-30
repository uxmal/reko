#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Arch.Arm;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public class ArmArchitectureTests
    {
        private Arm32ArchitectureNew arch;

        [Test]
        public void ArmArch_CreateRewriter()
        {
            this.arch = new Arm32ArchitectureNew();
            var mem = new MemoryArea(Address.Ptr32(0x00123400), new byte[] { 0x03, 0x10, 0x12, 0xE0 });

            var rdr = mem.CreateLeReader(0);
            var dasm = arch.CreateDisassembler(rdr);
            var str = dasm.First().ToString();
            Assert.AreEqual("@@@", str);
            rdr = mem.CreateLeReader(0);
            var rw = arch.CreateRewriter(rdr, new ArmProcessorState(arch), new StorageBinder(), null);
            var rtlc = rw.First();
            rtlc.ToString();
        }
    }
}
