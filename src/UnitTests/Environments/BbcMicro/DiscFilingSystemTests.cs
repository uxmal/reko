#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Memory;
using Reko.Environments.BbcMicro;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.BbcMicro
{
    [TestFixture]
    public class DiscFilingSystemTests
    {
        private byte[] image;
        private LeImageWriter writer;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.image = new byte[200_000];
            this.writer = new LeImageWriter(image);
            this.sc = new ServiceContainer();
        }

        private void Given_DiscTitleFirst(string title8)
        {
            var bytes = Encoding.ASCII.GetBytes(title8);
            Debug.Assert(bytes.Length == 8);
            writer.WriteBytes(bytes);
        }

        private void Given_FileName(string strName, char dir, bool locked)
        {
            var bytes = Encoding.ASCII.GetBytes(strName);
            Debug.Assert(bytes.Length == 7);
            byte bDir = (byte) (dir | (locked ? 0x80 : 0));
            writer.WriteBytes(bytes);
            writer.WriteByte(bDir);
        }

        private void Given_Sector01(string title, int cWrites, int cCatEntries, uint cSectors, uint cBootStartOption)
        {
            writer.Position = 0x100;    // skip to sector 1.

            var bytes = Encoding.ASCII.GetBytes(title);
            Debug.Assert(bytes.Length == 4);
            writer.WriteBytes(bytes);
            writer.WriteByte((byte) cWrites);
            writer.WriteByte((byte) (cCatEntries * 8));
            writer.WriteByte((byte)
                ((cSectors >> 8) |
                ((cBootStartOption & 3) << 4)));
            writer.WriteByte((byte) cSectors);
        }

        private void Given_FileAttributes(ushort addrLoad, ushort addrExec, ushort cbLength, uint sectStart)
        {
            writer.WriteLeUInt16(addrLoad);
            writer.WriteLeUInt16(addrExec);
            writer.WriteLeUInt16(cbLength);
            writer.WriteByte((byte)
                ((sectStart >> 8)));
            writer.WriteByte((byte) sectStart);
        }


        private void Given_FileData(int sector, byte[] bytes)
        {
            writer.Position = 0x100 * sector;
            writer.WriteBytes(bytes);
        }

        [Test]
        public void BbcDsp_LoadCatalog()
        {
            Given_DiscTitleFirst("Mydiscti");
            Given_FileName("File1  ", 'F', true);
            Given_FileName("File2  ", 'F', false);

            Given_Sector01(
                title: "tle ",
                cWrites: 0x1B,
                cCatEntries: 2,
                cSectors: 600,
                cBootStartOption: 3);
            Given_FileAttributes(
                addrLoad: 0x1B00,
                addrExec: 0x1B1D,
                cbLength: 0x42,
                sectStart: 2);
            Given_FileAttributes(
                addrLoad: 0x1B00,
                addrExec: 0x1B1D,
                cbLength: 0x13,
                sectStart: 3);
            Given_FileData(2, Enumerable.Range(0, 0x42).Select(i => (byte) i).ToArray());
            Given_FileData(2, Enumerable.Range(0, 0x13).Select(i => (byte) i).ToArray());

            var dfsLoader = new DiscFilingSystem(sc, ImageLocation.FromUri("file:foo.sdd"), this.image);
            var items = dfsLoader.LoadDirectory();
            Assert.AreEqual(2, items.Length);
        }
    }
}
