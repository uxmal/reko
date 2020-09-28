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

using Reko.Arch.M68k;
using Reko.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.M68k
{
    [TestFixture]
    public class M68kPointerScannerTests
    {
        private BeImageReader CreateImageReader(Address address, params ushort[] words)
        {
            var bytes = words
                .Select(w => new byte[] { (byte) (w >> 8), (byte) w })
                .SelectMany(ab => ab)
                .ToArray();
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

        [Test]
        public void M68kps_Bsrl()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x0000, 0x61FF, 0x0000, 0x000C, 0x4E75, 0x4E71, 0x4E71, 0x4E71,
                0x4E75);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00100010u }, PointerScannerFlags.Calls).ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100002u, items[0]);
        }

        [Test]
        public void M68kps_bsrw()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x0000, 0x6100, 0x000C, 0x4E75, 0x4E71, 0x4E71, 0x4E71, 0x4E71,
                0x4E75);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00100010u }, PointerScannerFlags.Calls).ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100002u, items[0]);
        }

        [Test]
        public void M68kps_bsrb()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x4E75, 0x61FC);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00100000u }, PointerScannerFlags.Calls).ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100002u, items[0]);
        }

        [Test]
        public void M68kps_jsrw()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00000100),
                0x4EB8, 0x0108, 0x4E75, 0x4E71, 0x4E75);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00108u }, PointerScannerFlags.Calls).ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100u, items[0]);
        }

        [Test]
        public void M68kps_jsrl()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00000100),
                0x4EB9, 0x0000, 0x0108, 0x4E75, 0x4E75);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00108u }, PointerScannerFlags.Calls).ToArray();

            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100u, items[0]);
        }

        [Test]
        public void M68kps_brab()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x4E75, 0x60FC, 0x4E75);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00100000u }, PointerScannerFlags.Jumps).ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100002u, items[0]);
        }

        [Test]
        public void M68kps_jmpw()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100),
                0x4e75, 0x4EF8, 0x0100);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00100 }, PointerScannerFlags.Jumps).ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00102u, items[0]);
        }

        [Test]
        public void M68kps_jmpl()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100),
                0x4e75, 0x4EF9, 0x0000, 0x0100);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00100 }, PointerScannerFlags.Jumps).ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00102u, items[0]);
        }

        [Test]
        public void M68kps_FindPointer_Flat32()
        {
            var rdr = CreateImageReader(
                Address.Ptr32(0x00100000),
                0x2222, 0x0010, 0x0008);
            var items = new M68kPointerScanner(rdr, new HashSet<uint> { 0x00100008u }, PointerScannerFlags.Pointers).ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual(0x00100002u, items[0]);
        }
    }
}
