#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.OpenRISC;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class AeonAssemblerTests
    {
        private void AssertAsm(string asmSrc, string hexBytesExpected)
        {
            var arch = new AeonArchitecture(new ServiceContainer(), "aeon", new Dictionary<string, object>());
            var asm = arch.CreateAssembler(null);
            var program = asm.AssembleFragment(Address.Ptr32(0x10_0000), asmSrc);
            var bmem = (ByteMemoryArea) program.SegmentMap.Segments.Values.First().MemoryArea;
            var bytesExpected = BytePattern.FromHexBytes(hexBytesExpected);
            Assert.AreEqual(bytesExpected, bmem.Bytes);
        }

        [Test]
        public void AeonAsm_bg_andi()
        {
            // confirmed with source
            AssertAsm("bg.andi\tr3,r3,0x8000", "C4 63 80 00");
        }

        [Test]
        public void AeonAsm_bg_beqi()
        {
            AssertAsm("bg.beqi\tr10,0x1,0x00100095", "D1 41 04 AA");
        }

        [Test]
        public void AeonAsm_bg_lbz()
        {
            AssertAsm("bg.lbz\tr6,0x1EEC(r7)", "F0 C7 1E EC");
        }

        [Test]
        public void AeonAsm_bg_movhi()
        {
            AssertAsm("bg.movhi\tr7,0xA020", "C0 F4 04 01");
        }

        [Test]
        public void AeonAsm_bg_ori()
        {
            AssertAsm("bg.ori\tr5,r7,0x2400", "C8 A7 24 00");
        }

        [Test]
        public void AeonAsm_bg_sb()
        {
            AssertAsm("bg.sb\t0x36D8(r10),r7", "F8 EA 36 D8");
        }

        /*
beqi
lbz
andi
        */
    }
}
