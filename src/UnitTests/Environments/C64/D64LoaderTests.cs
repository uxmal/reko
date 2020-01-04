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

using Moq;
using NUnit.Framework;
using Reko.Core.Archives;
using Reko.Environments.C64;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Reko.UnitTests.Environments.C64
{
    public class D64LoaderTests
    {
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
        }

        private byte[] CreateD64Image(
            string diskName,
            byte trackDir,
            byte sectorDir,
            Dictionary<int, byte[]> sectors)
        {
            var image = new byte[174848];
            var stm = new MemoryStream(image);
            var bamOffset = SectorRef(18, 0);
            stm.Position = bamOffset;
            stm.WriteByte(trackDir);
            stm.WriteByte(sectorDir);
            stm.WriteByte(0x41);
            stm.Position = bamOffset + 0xA0;
            stm.WriteByte(0xA0);
            stm.WriteByte(0xA0);
            stm.WriteByte(0x30);
            stm.WriteByte(0x30);
            stm.WriteByte(0xA0);
            stm.WriteByte(0x2A);
            stm.WriteByte(0xA0);
            stm.WriteByte(0xA0);
            stm.WriteByte(0xA0);
            stm.WriteByte(0xA0);
            stm.WriteByte(0x00);

            foreach (var de in sectors)
            {
                stm.Position = de.Key;
                stm.Write(de.Value, 0, de.Value.Length);
            }
            return image;
        }

        [Test]
        public void D64_EmptyImage()
        {
            var image = CreateD64Image(
                "CBM.COM",
                0, 0,
                new Dictionary<int, byte[]>());
            var loader = new D64Loader(sc, "CBM.COM", image);
            var items = loader.LoadDiskDirectory();
            Assert.AreEqual(0, items.Count);
        }

        private int SectorRef(byte track, byte sector)
        {
            return D64Loader.SectorOffset(track, sector);
        }

        private byte[] Sector(params byte[][] fragments)
        {
            var stm = new MemoryStream();
            foreach (var frag in fragments)
            {
                stm.Write(frag, 0, frag.Length);
            }
            if (stm.Length > 256)
                throw new InvalidOperationException("Sector too large.");
            while (stm.Length < 256)
            {
                stm.WriteByte(0);
            }
            return stm.ToArray();
        }

        private byte[] DirEntry(
            byte trackNextDir,
            byte sectorNextDir,
            FileType fileType,
            byte trackFile,
            byte sectorFile,
            string filename,
            short sectorSize)
        {
            var stm = new MemoryStream();
            stm.WriteByte(trackNextDir);
            stm.WriteByte(sectorNextDir);
            stm.WriteByte((byte) fileType);
            stm.WriteByte(trackFile);
            stm.WriteByte(sectorFile);
            var sName = Encoding.ASCII.GetBytes(filename.PadRight(16, (char) 0xA0));
            stm.Write(sName, 0, sName.Length);
            stm.WriteByte(0);
            stm.WriteByte(0);
            stm.WriteByte(0);   // REL file
            stm.WriteByte(0);
            stm.WriteByte(0);
            stm.WriteByte(0);
            stm.WriteByte(0);
            stm.WriteByte(0);
            stm.WriteByte(0);
            stm.WriteByte((byte) sectorSize);
            stm.WriteByte((byte) (sectorSize >> 8));
            Assert.AreEqual(0x20, stm.Length);
            return stm.ToArray();
        }

        [Test]
        public void D64_SingleFile()
        {
            var image = CreateD64Image(
                "CBM.COM",
                18, 1,
                new Dictionary<int, byte[]> 
                {
                    {
                        SectorRef(18, 1),
                        Sector(
                            DirEntry(
                                0, 0xFF,
                                FileType.PRG | FileType.Locked,
                                19, 0,
                                "FOO", 
                                1))
                    },
                    {
                        SectorRef(19, 0),
                        Sector(
                            new byte[] { 0, 0xD},
                            Encoding.ASCII.GetBytes("Hello world"))
                    }
                });
            var loader = new D64Loader(sc, "CBM.COM", image);
            var items = loader.LoadDiskDirectory();

            Assert.AreEqual(1, items.Count);
            var file = (ArchivedFile) items[0];
            Assert.AreEqual("Hello world", Encoding.ASCII.GetString(file.GetBytes()));
        }
    }
}
