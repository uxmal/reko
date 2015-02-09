#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Configuration;
using Decompiler.Core;
using Decompiler.ImageLoaders.MzExe;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.ImageLoaders.MzExe
{
    [TestFixture]
    public class PeImageLoaderTests
    {
        private ServiceContainer sc;
        private MockRepository mr;
        private LoadedImage image;
        private PeImageLoader peldr;
        private LeImageWriter writer;
        private Address addrLoad;
        private const int rvaPeHdr = 0x40;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            mr = new MockRepository();
            addrLoad = new Address(0x00100000);
        }

        [Test]
        public void Pil_DelayLoads()
        {
            Given_PeHeader();
            Given_Section(".text");
            Given_DelayLoadDirectories(
                new DelayLoadDirectoryEntry
                {
                    Name = "user32.dll",
                    ImportNames = new string[] {
                        "GetDesktopWindow",
                        "GetFocus"
                    }
                });
            Given_PeLoader();

            var program = peldr.Load(addrLoad);
            var rel=peldr.Relocate(addrLoad);

            Assert.AreEqual(2, program.ImportReferences.Count);
            Assert.AreEqual("user32.dll!GetDesktopWindow", program.ImportReferences[new Address(0x0010103C)].ToString());
            Assert.AreEqual("user32.dll!GetFocus", program.ImportReferences[new Address(0x00101040)].ToString());
        }

        private void Given_Section(string section)
        {
            var bytes = Encoding.UTF8.GetBytes(section);
            writer.WriteBytes(bytes).WriteBytes(0, 8 - (uint)bytes.Length);
            writer.WriteLeInt32(0x100);
            writer.WriteLeInt32(0x1000);
            writer.WriteLeInt32(0x100); // raw data
            writer.WriteLeInt32(0x1000);    // rva to raw data
            writer.WriteLeInt32(0);         // relocs
            writer.WriteLeInt32(0);         // line numbers
            writer.WriteLeInt32(0);         // #relocs
            writer.WriteLeInt32(0);         // #line numbers
            writer.WriteLeInt32(0);         // characteristics
        }

        private void Given_DelayLoadDirectories(params DelayLoadDirectoryEntry [] delayLoadDirectory)
        {
            writer.Position = 0x1000;
            foreach (var entry in delayLoadDirectory)
            {
                entry.rvaName = writer.Position;
                writer.WriteString(entry.Name, Encoding.UTF8).WriteByte(0);
                entry.arvaImportNames = new List<int>();
                foreach (var impName in entry.ImportNames)
                {
                    entry.arvaImportNames.Add(writer.Position);
                    writer.WriteLeInt16(0);
                    writer.WriteString(impName, Encoding.UTF8).WriteByte(0);
                }
                Align();
                entry.rvaImportNames = writer.Position;
                foreach (var rva in entry.arvaImportNames)
                {
                    writer.WriteLeUInt32((uint)rva);
                }
                writer.WriteLeUInt32(0);

                entry.rvaImportAddressTable = writer.Position;
                foreach (var rva in entry.arvaImportNames)
                {
                    writer.WriteLeUInt32(0xCCCCCCCC);
                }
                writer.WriteLeUInt32(0);
            }
            var rvaDld = writer.Position;
            foreach (var entry in delayLoadDirectory)
            {
                writer.WriteLeUInt32(1);
                writer.WriteLeUInt32((uint)entry.rvaName);
                writer.WriteLeUInt32(0);    // module handle
                writer.WriteLeUInt32((uint)entry.rvaImportAddressTable);
                writer.WriteLeUInt32((uint)entry.rvaImportNames);
                writer.WriteLeUInt32(0); 
                writer.WriteLeUInt32(0); 
                writer.WriteLeUInt32(0); 
            }
            writer.WriteLeUInt32(0);
            writer.WriteLeUInt32(0);
            writer.Position = rvaPeHdr + 0xE0;
            writer.WriteLeInt32(rvaDld);
        }

        private void Align()
        {
            var misAlign = writer.Position & 0xF;
            if (misAlign != 0)
                writer.WriteBytes(0, 0x10u - (uint)misAlign);
        }

        public class DelayLoadDirectoryEntry
        {
            public string Name;
            public string[] ImportNames;

            public int rvaName;
            public int rvaImportNames;
            public int rvaImportAddressTable;
            public List<int> arvaImportNames;
        }

        private void Given_PeHeader()
        {
            image = new LoadedImage(addrLoad, new byte[0x10000]);
            writer = new LeImageWriter(image.Bytes, rvaPeHdr);
            writer.WriteBytes(new byte[] { 0x50, 0x45, 0, 0 });

            writer.WriteLeInt16(0x14C);
            writer.WriteLeInt16(1); // sections.
            writer.WriteLeInt32(0);		// timestamp.
            writer.WriteLeInt32(0);		// COFF symbol
            writer.WriteLeInt32(0);		// #of symbols
            var rvaOptionalHeaderSize = writer.Position;
            writer.WriteLeInt16(0);       // optionalHeaderSize
            writer.WriteLeInt16(0);     //  short fileFlags 

            // Optional header
            var rvaOptHdr = writer.Position;
            writer.WriteLeInt16(0x010B);
            writer.WriteBytes(0, 0xCE);

            var pos = writer.Position;
            var optHdrSize = pos - rvaOptHdr;
            writer.Position = rvaOptionalHeaderSize;
            writer.WriteLeInt16((short)optHdrSize);
            writer.Position = pos;
        }

        private void Given_PeLoader()
        {
            peldr = new PeImageLoader(null, "test.exe", image.Bytes, rvaPeHdr);
        }
    }
}
