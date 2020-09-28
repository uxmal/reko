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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    public class X86PointerScannerTests
    {
        private LeImageReader CreateImageReader(Address address, params byte[] bytes)
        {
            return new LeImageReader(new MemoryArea(address, bytes), 0);
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

        [Test]
        public void X86Ps_FindInboundCalls_Flat32()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x00, 0xE8, 0x02, 0x00, 0x00, 0x00, 0xC3, 0x90,
                0xC3);
            var items = new X86PointerScanner32(rdr, new HashSet<uint> { 0x00100008u }, PointerScannerFlags.Calls).ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100001u, items[0]);
        }

        [Test]
        public void X86Ps_FindInboundJumps_Flat32()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x00, 0xE9, 0x02, 0x00, 0x00, 0x00, 0xC3, 0x90,
                0xC3);
            var items = new X86PointerScanner32(rdr, new HashSet<uint> { 0x00100008u }, PointerScannerFlags.Jumps).ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100001u, items[0]);
        }

        [Test]
        public void X86Ps_FindShortBranches_Flat32()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x00, 0x74, 0x02, 0xC3, 0x90, 0xC3);
            var items = new X86PointerScanner32(rdr, new HashSet<uint> { 0x00100005u }, PointerScannerFlags.Jumps).ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100001u, items[0]);
        }

        [Test]
        public void X86Ps_FindLongBranches_Flat32()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x0F, 0x84, 0x02, 0x00, 0x00, 0x00, 0xC3, 0x90, 0xC3);
            var items = new X86PointerScanner32(rdr, new HashSet<uint> { 0x00100008u }, PointerScannerFlags.Jumps).ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100000u, items[0]);
        }

        [Test]
        public void X86Ps_FindPointer_Flat32()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x22, 0x22, 0x22, 0x08, 0x00, 0x10, 0x00);
            var items = new X86PointerScanner32(rdr, new HashSet<uint> { 0x00100008u }, PointerScannerFlags.Pointers).ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100003u, items[0]);
        }
    }
}
