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

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcPointerScannerTests
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

        [Test(Description="The instruction at address 00100000 should be calling 00100008")]
        public void PpcPs_FindInboundCalls()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x00, 0x00, 0x00, 0x00,  0x48, 0x00, 0x00, 0x05,
                0x4E, 0x80, 0x00, 0x20
                );
            var items = new PowerPcPointerScanner32(rdr, new HashSet<uint> { 0x00100008u }, PointerScannerFlags.Calls)
                .ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100004u, items[0]);
        }
    }
}
