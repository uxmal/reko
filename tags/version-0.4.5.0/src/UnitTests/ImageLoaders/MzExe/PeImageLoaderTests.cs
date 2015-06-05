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
    /// <summary>
    /// Tests for PeImageLoader
    /// </summary>
    /// <remarks>
    /// File map:
    /// +--------------------------------------+
    /// | 0000 - 0FFF | Header |
    /// | 1000 - 1FFF | Text   | .text
    /// | 2000 - 21FF | ILT    | .reloc
    /// | 2200 - 23FF | IAT    |
    /// | 3000 - 3FFF | data   | .data
    /// </remarks>
    [TestFixture]
    public class PeImageLoaderTests
    {
        private ServiceContainer sc;
        private MockRepository mr;
        private byte[] fileImage;
        private PeImageLoader peldr;
        private LeImageWriter writer;
        private Address addrLoad;
        private int rvaDirectories;
        private const int RvaPeHdr = 0x0040;
        private const int RvaText = 0x1000;
        private const int RvaImportDescriptor = 0x2000;
        private const int RvaData= 0x3000;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            mr = new MockRepository();
            addrLoad = Address.Ptr32(0x00100000);
            fileImage = new byte[0x4000];
            writer = new LeImageWriter(fileImage);
        }

        // PE section headers are always 40 bytes.
        private void Given_Section(string section, uint virtAddress, uint rvaData)
        {
            var bytes = Encoding.UTF8.GetBytes(section);
            writer.WriteBytes(bytes).WriteBytes(0, 8 - (uint)bytes.Length);
            writer.WriteLeUInt32(0x1000);     // Section size in memory
            writer.WriteLeUInt32(virtAddress);   // Where to load this
            writer.WriteLeUInt32(0x1000);     // raw data
            writer.WriteLeUInt32(rvaData);    // rva to raw data
            writer.WriteLeInt32(0);         // relocs
            writer.WriteLeInt32(0);         // line numbers
            writer.WriteLeInt16(0);         // #relocs
            writer.WriteLeInt16(0);         // #line numbers
            writer.WriteLeInt32(0);         // characteristics

            // Increment the section count in the optional header.
            short sections = LoadedImage.ReadLeInt16(fileImage, RvaPeHdr + 6);
            LoadedImage.WriteLeInt16(fileImage, RvaPeHdr + 6, (short)(sections + 1));
        }

        private void Given_DelayLoadDirectories(params DelayLoadDirectoryEntry [] delayLoadDirectory)
        {
            writer.Position = 0x1800;
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
            writer.Position = rvaDirectories + 13 * 8;
            writer.WriteLeInt32(rvaDld);
            writer.Position = rvaDirectories - 4;
            writer.WriteLeInt32(14);
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

        private void Given_Pe32Header(uint uAddrBase)
        {
            writer.Position = RvaPeHdr;

            writer.WriteBytes(new byte[] { 0x50, 0x45, 0, 0 });
            var p = writer.Position;
            writer.WriteLeInt16(0x14C);
            writer.WriteLeInt16(0); // sections.
            writer.WriteLeInt32(0);		// timestamp.
            writer.WriteLeInt32(0);		// COFF symbol
            writer.WriteLeInt32(0);		// #of symbols
            var rvaOptionalHeaderSize = writer.Position;
            writer.WriteLeInt16(0);       // optionalHeaderSize
            writer.WriteLeInt16(0);     //  short fileFlags 

            // Optional header
            var rvaOptHdr = writer.Position;
            writer.WriteLeInt16(0x010B);
            writer.WriteLeInt16(0);         // linker
            writer.WriteLeInt32(0);         // size of code
            writer.WriteLeInt32(0);         // size of init data
            writer.WriteLeInt32(0);         // size of uninit data
            writer.WriteLeUInt32(RvaText);   // RVA of entry point.
            writer.WriteLeUInt32(RvaText);   // RVA of base of code.
            writer.WriteLeUInt32(RvaData);   // RVA of base of data.
            writer.WriteLeUInt32(uAddrBase);    // preferred image base.
            writer.WriteLeUInt32(0);        // section alignment
            writer.WriteLeUInt32(0);        // file alignment
            writer.WriteLeUInt32(0);        // OS version
            writer.WriteLeUInt32(0);        // image version
            writer.WriteLeUInt32(0);        // subsystem version
            writer.WriteLeUInt32(0);        // Win32 version
            writer.WriteLeUInt32(0);        // size of image
            writer.WriteLeUInt32(0);        // size of headers
            writer.WriteLeUInt32(0);        // checksum
            writer.WriteLeUInt16(0);        // subsystem
            writer.WriteLeUInt16(0);        // DLL characeristics
            writer.WriteLeUInt32(0);        // stack reserve
            writer.WriteLeUInt32(0);        // stack commit
            writer.WriteLeUInt32(0);        // heap reserve
            writer.WriteLeUInt32(0);        // heap commit
            writer.WriteLeUInt32(0);        // loader flags
            writer.WriteLeUInt32(2);        // number of data directory entries

            rvaDirectories = writer.Position;
            writer.Position = rvaDirectories + 2 * 8;
            var optHdrSize = writer.Position - rvaOptHdr;
            var rvaSections = writer.Position;
            writer.Position = rvaOptionalHeaderSize;
            writer.WriteLeInt16((short)optHdrSize);
            writer.Position = rvaSections;
        }

        private void Given_Pe64Header(uint rvaEntryPoint, ulong uAddrBase, uint rvaImportTable)
        {
            writer.Position = RvaPeHdr;
            writer.WriteBytes(new byte[] { 0x50, 0x45, 0, 0 });

            writer.WriteLeUInt16((ushort)0x8664u);
            writer.WriteLeInt16(0);     // sections.
            writer.WriteLeInt32(0);		// timestamp.
            writer.WriteLeInt32(0);		// COFF symbol
            writer.WriteLeInt32(0);		// #of symbols
            var rvaOptionalHeaderSize = writer.Position;
            writer.WriteLeInt16(0);       // optionalHeaderSize
            writer.WriteLeInt16(0);     //  short fileFlags 

            // Optional header
            var rvaOptHdr = writer.Position;
            writer.WriteLeInt16(0x020B);    // magic
            writer.WriteLeInt16(0);         // linker
            writer.WriteLeInt32(0);         // size of code
            writer.WriteLeInt32(0);         // size of init data
            writer.WriteLeInt32(0);         // size of uninit data
            writer.WriteLeUInt32(rvaEntryPoint); // RVA of entry point.
            writer.WriteLeUInt32(RvaText);   // RVA of base of code.

            writer.WriteLeUInt64(uAddrBase); // 0x0000000140000000L);  // preferred image base
            writer.WriteLeUInt32(0);        // section alignment
            writer.WriteLeUInt32(0);        // file alignment
            writer.WriteLeUInt32(0);        // OS version
            writer.WriteLeUInt32(0);        // image version
            writer.WriteLeUInt32(0);        // subsystem version
            writer.WriteLeUInt32(0);        // Win32 version
            writer.WriteLeUInt32(0);        // size of image
            writer.WriteLeUInt32(0);        // size of headers
            writer.WriteLeUInt32(0);        // checksum
            writer.WriteLeUInt16(0);        // subsystem
            writer.WriteLeUInt16(0);        // DLL characeristics
            writer.WriteLeUInt64(0);        // stack reserve
            writer.WriteLeUInt64(0);        // stack commit
            writer.WriteLeUInt64(0);        // heap reserve
            writer.WriteLeUInt64(0);        // heap commit
            writer.WriteLeUInt32(0);        // loader flags
            writer.WriteLeUInt32(2);        // number of data directory entries

            rvaDirectories = writer.Position;
            var optHdrSize = rvaDirectories - rvaOptHdr;
            writer.Position = rvaOptionalHeaderSize;
            writer.WriteLeInt16((short)optHdrSize);
            writer.Position = rvaDirectories;

            writer.WriteLeUInt32(0);        // Export table rva
            writer.WriteLeUInt32(0);        // Export table size
            writer.WriteLeUInt32(rvaImportTable);
            writer.WriteLeUInt32(0x0800);
        }

        private void Given_PeLoader()
        {
            peldr = new PeImageLoader(null, "test.exe", fileImage, RvaPeHdr);
        }

        private int Given_ImportDescriptor32(
            int rvaIlt,
            string dllName,
            int rvaIat)
        {
            var rvaDllName = writer.Position;
            writer.WriteString(dllName, Encoding.UTF8);
            writer.WriteByte(0);

            var rvaId = writer.Position;
            writer.WriteLeInt32(rvaIlt);
            writer.WriteLeInt32(0);     // (ignored) datestamp
            writer.WriteLeInt32(0);     // forwarder chain
            writer.WriteLeInt32(rvaDllName);
            writer.WriteLeInt32(rvaIat);
            return rvaId;
        }

        private int Given_Ilt32(params object [] import)
        {
            var rvaTable = writer.Position;
            writer.WriteBytes(0, (uint)(1 + import.Length) * 4);  // Reserve space for uints and terminating zero.
            var strWriter = writer.Clone();                 // write strings after
            writer.Position = rvaTable;                     // rewind to beginning of table.
            foreach (object imp in import)
            {
                var s = imp as string;
                if (s != null)
                {
                    writer.WriteLeInt32(strWriter.Position);
                    strWriter.WriteLeInt16(0);
                    strWriter.WriteString(s, Encoding.UTF8);
                    strWriter.WriteByte(0);
                }
                else if (imp is uint)
                {
                    if ((uint)imp != 0)
                    {
                        writer.WriteLeUInt32((uint)imp | 0x80000000);
                    }
                    else
                    {
                        writer.WriteLeUInt32((uint)imp);
                    }
                }
            }
            writer.WriteLeInt32(0);
            writer.Position = strWriter.Position;
            return rvaTable;
        }

        private int Given_Ilt64(params object[] import)
        {
            var rvaTable = writer.Position;
            writer.WriteBytes(0, (uint)(1 + import.Length) * 8);  // Reserve space for ulongs and terminating zero.
            var strWriter = writer.Clone();                 // write strings after
            writer.Position = rvaTable;                     // rewind to beginning of table.
            foreach (object imp in import)
            {
                var s = imp as string;
                if (s != null)
                {
                    writer.WriteLeInt64(strWriter.Position);
                    strWriter.WriteLeInt16(0);
                    strWriter.WriteString(s, Encoding.UTF8);
                    strWriter.WriteByte(0);
                }
                else if (imp is uint)
                {
                    writer.WriteLeUInt64((ulong)imp | 0x8000000000000000);
                }
            }
            writer.WriteLeInt32(0);
            writer.Position = strWriter.Position;
            return rvaTable;
        }

        [Test]
        public void Pil32_DelayLoads()
        {
            Given_Pe32Header(0x00100000);
            Given_Section(".text", 0x1000u, 0x1000u);
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
            var rel = peldr.Relocate(addrLoad);

            Assert.AreEqual(2, program.ImportReferences.Count);
            Assert.AreEqual("user32.dll!GetDesktopWindow", program.ImportReferences[Address.Ptr32(0x0010183C)].ToString());
            Assert.AreEqual("user32.dll!GetFocus", program.ImportReferences[Address.Ptr32(0x00101840)].ToString());
        }

        [Test]
        public void Pil32_SaneIat()
        {
            Given_Pe32Header(0x00100000);
            Given_Section(".text", 0x1000u, 0x1000u);
            Given_Section(".idata", 0x2000u, 0x2000u);
            writer.Position = RvaImportDescriptor;
            var rvaId = Given_ImportDescriptor32(
                Given_Ilt32("malloc", "free", "realloc"),
                "msvcrt.dll",
                Given_Ilt32("malloc", "free", "realloc"));
            Given_PeLoader();
            var program = peldr.Load(addrLoad);

            var rdrId = new LeImageReader(fileImage, (uint)rvaId);
            var ret = peldr.ReadImportDescriptor(rdrId, addrLoad);
            Assert.IsTrue(ret);
            Assert.AreEqual(3, program.ImportReferences.Count); ;
            Assert.AreEqual("msvcrt.dll!malloc", program.ImportReferences[Address.Ptr32(0x0010202A)].ToString());
            Assert.AreEqual("msvcrt.dll!free", program.ImportReferences[Address.Ptr32(0x0010202E)].ToString());
            Assert.AreEqual("msvcrt.dll!realloc", program.ImportReferences[Address.Ptr32(0x00102032)].ToString());
        }

        [Test]
        public void Pil32_BlankIat()
        {
            Given_Pe32Header(0x00100000);
            Given_Section(".text", 0x1000, 0x1000);
            Given_Section(".idata", 0x2000, 0x2000);
            writer.Position = RvaImportDescriptor;
            var rvaId = Given_ImportDescriptor32(
                Given_Ilt32("malloc", "free", "realloc"),
                "msvcrt.dll",
                Given_Ilt32(0u, 0u, 0u));
            Given_PeLoader();
            var program = peldr.Load(addrLoad);

            var rdrId = new LeImageReader(fileImage, (uint)rvaId);
            var ret = peldr.ReadImportDescriptor(rdrId, addrLoad);
            Assert.IsTrue(ret);
            Assert.AreEqual(3, program.ImportReferences.Count); ;
            Assert.AreEqual("msvcrt.dll!malloc", program.ImportReferences[Address.Ptr32(0x0010202A)].ToString());
            Assert.AreEqual("msvcrt.dll!free", program.ImportReferences[Address.Ptr32(0x0010202E)].ToString());
            Assert.AreEqual("msvcrt.dll!realloc", program.ImportReferences[Address.Ptr32(0x00102032)].ToString());
        }
    }
}
