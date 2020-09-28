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

using Reko.Arch.PowerPC;
using Reko.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Arch.Mips;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class MipsPointerScannerTests
    {
        private BeImageReader CreateImageReader(Address address, params byte[] bytes)
        {
            return new BeImageReader(new MemoryArea(address, bytes), 0);
        }

        private uint[] GetItems(IEnumerator<uint> e)
        {
            var list = new List<uint>();
            while (e.MoveNext())
            {
                list.Add(e.Current);
            }
            return list.ToArray();
        }

        [Test(Description = "The instruction at address 00100004 should be calling 00100010")]
        public void MipsPs_FindInboundCalls()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),

                0x00, 0x00, 0x00, 0x00, 
                0x0C, 0x04, 0x00, 0x04,     // jal 00100010
                0x00, 0x00, 0x00, 0x00,     // nop
                0x00, 0x00, 0x00, 0x00,     // nop
                
                0x4E, 0x80, 0x00, 0x20
                );
            var items = new MipsPointerScanner32(rdr, new HashSet<uint> { 0x00100010u }, PointerScannerFlags.Calls)
                .ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100004u, items[0]);
        }

        [Test(Description = "The instruction at address 00100004 should be jumping to 00100010")]
        public void MipsPs_FindLongInboundJumps()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),

                0x00, 0x00, 0x00, 0x00,
                0x08, 0x04, 0x00, 0x04,     // j 00100010
                0x00, 0x00, 0x00, 0x00,     // nop
                0x00, 0x00, 0x00, 0x00,     // nop

                0x4E, 0x80, 0x00, 0x20
                );
            var items = new MipsPointerScanner32(rdr, new HashSet<uint> { 0x00100010u }, PointerScannerFlags.Jumps)
                .ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100004u, items[0]);
        }


        [Test(Description = "The instruction at address 00100004 should be jumping to 00100010")]
        public void MipsPs_FindShortInboundJumps()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),

                0x00, 0x00, 0x00, 0x00,
                0x10, 0x04, 0x00, 0x02,     // beq 00100010
                0x00, 0x00, 0x00, 0x00,     // nop
                0x00, 0x00, 0x00, 0x00,     // nop

                0x4E, 0x80, 0x00, 0x20
                );
            var items = new MipsPointerScanner32(rdr, new HashSet<uint> { 0x00100010u }, PointerScannerFlags.Jumps)
                .ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100004u, items[0]);
        }
    }
}
