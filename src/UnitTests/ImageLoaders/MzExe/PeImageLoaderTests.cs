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

using Reko.Core.Configuration;
using Reko.Core;
using Reko.ImageLoaders.MzExe;
using NUnit.Framework;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System.Diagnostics;
using Reko.Core.Services;

namespace Reko.UnitTests.ImageLoaders.MzExe
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
    /// | 4000 - 4FFF | didata | .didata
    /// </remarks>
    [TestFixture]
    public class PeImageLoaderTests
    {
        private ServiceContainer sc;
        private byte[] fileImage;
        private PeImageLoader peldr;
        private LeImageWriter writer;
        private Address addrLoad;
        private int rvaDirectories;
        private const int RvaPeHdr = 0x0040;
        private const int RvaText = 0x1000;
        private const int RvaImportDescriptor = 0x2000;
        private const int RvaData = 0x3000;
        private const int RvaDelayImportDescriptor = 0x1800;

        private Mock<IProcessorArchitecture> arch_386;
        private Mock<Win32Platform> win32;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            addrLoad = Address.Ptr32(0x00100000);
            fileImage = new byte[0x4000];
            writer = new LeImageWriter(fileImage);
            var cfgSvc = new Mock<IConfigurationService>();
            var dcSvc = new Mock<DecompilerEventListener>();
            Given_i386_Architecture();
            this.win32 = new Mock<Win32Platform>(sc, arch_386.Object) { CallBase = true };
            // Avoid complications with the FindMainProcedure call.
            this.win32.Setup(w => w.FindMainProcedure(
                It.IsAny<Program>(),
                It.IsAny<Address>())).Returns((ImageSymbol)null);
            var win32Env = new Mock<PlatformDefinition>();
            cfgSvc.Setup(c => c.GetArchitecture("x86-protected-32")).Returns(arch_386.Object);
            cfgSvc.Setup(c => c.GetEnvironment("win32")).Returns(win32Env.Object);
            win32Env.Setup(w => w.Load(
                It.IsAny<IServiceProvider>(),
                It.IsAny<IProcessorArchitecture>())).Returns(win32.Object);
            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<DecompilerEventListener>(dcSvc.Object);
        }

        private void Given_i386_Architecture()
        {
            this.arch_386 = new Mock<IProcessorArchitecture>();
            arch_386.Setup(a => a.Name).Returns("x86-protected-32");
            arch_386.Setup(a => a.CreateFrame()).Returns(new Frame(PrimitiveType.Ptr32));
            arch_386.Setup(a => a.WordWidth).Returns(PrimitiveType.Word32);
            arch_386.Setup(a => a.PointerType).Returns(PrimitiveType.Ptr32);
            var map = new SegmentMap(addrLoad);
            var state = new Mocks.FakeProcessorState(this.arch_386.Object);
            arch_386.Setup(a => a.CreateProcessorState()).Returns(state);
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
            short sections = MemoryArea.ReadLeInt16(fileImage, RvaPeHdr + 6);
            MemoryArea.WriteLeInt16(fileImage, RvaPeHdr + 6, (short)(sections + 1));
        }

        private void Given_DelayLoadDirectories(params DelayLoadDirectoryEntry [] delayLoadDirectory)
        {
            writer.Position = RvaDelayImportDescriptor;
            foreach (var entry in delayLoadDirectory)
            {
                uint offset = ((entry.Attributes & PeImageLoader.DID_RvaBased) != 0)
                    ? 0
                    : (uint)addrLoad.ToUInt32();

                entry.rvaName = (int) (writer.Position + offset);
                writer.WriteString(entry.Name, Encoding.UTF8).WriteByte(0);
                entry.arvaImportNames = new List<int>();
                foreach (var impName in entry.ImportNames)
                {
                    entry.arvaImportNames.Add((int)(writer.Position + offset));
                    writer.WriteLeInt16(0);
                    writer.WriteString(impName, Encoding.UTF8).WriteByte(0);
                }
                Align();
                entry.rvaImportNames = (int)(writer.Position + offset);
                foreach (var rva in entry.arvaImportNames)
                {
                    writer.WriteLeUInt32((uint)rva);
                }
                writer.WriteLeUInt32(0);

                entry.rvaImportAddressTable = (int)(writer.Position + offset);
                foreach (var rva in entry.arvaImportNames)
                {
                    writer.WriteLeUInt32(0xCCCCCCCC);
                }
                writer.WriteLeUInt32(0);
            }
            var rvaDld = writer.Position;
            foreach (var entry in delayLoadDirectory)
            {
                writer.WriteLeUInt32(entry.Attributes);
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
            writer.WriteLeInt32((int)rvaDld);
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
            public uint Attributes;
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

            rvaDirectories = (int)writer.Position;
            writer.Position = rvaDirectories + 2 * 12;
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

            rvaDirectories = (int)writer.Position;
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
            peldr = new PeImageLoader(sc, "test.exe", fileImage, RvaPeHdr);
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
            writer.WriteLeInt32((int)rvaDllName);
            writer.WriteLeInt32(rvaIat);
            return (int)rvaId;
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
                    writer.WriteLeInt32((int)strWriter.Position);
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
            return (int)rvaTable;
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
                    writer.WriteLeInt64((int)strWriter.Position);
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
            return (int)rvaTable;
        }

        [Test]
        public void Pil32_DelayLoads_Absolute()
        {
            Given_Pe32Header(0x00100000);
            Given_Section(".text", 0x1000u, 0x1000u);
            Given_DelayLoadDirectories(
                new DelayLoadDirectoryEntry
                {
                    Attributes = 0,
                    Name = "user32.dll",
                    ImportNames = new string[] {
                        "GetDesktopWindow",
                        "GetFocus"
                    }
                });
            sc.AddService<IDiagnosticsService>(new Mock<IDiagnosticsService>().Object);

            Given_PeLoader();

            var program = peldr.Load(addrLoad);
            peldr.Relocate(program, addrLoad);

            Assert.AreEqual(2, program.ImportReferences.Count);
            Assert.AreEqual("user32.dll!GetDesktopWindow", program.ImportReferences[Address.Ptr32(0x0010183C)].ToString());
            Assert.AreEqual("user32.dll!GetFocus", program.ImportReferences[Address.Ptr32(0x00101840)].ToString());
        }
        [Test]
        public void Pil32_DelayLoads_RvaBased()
        {
            Given_Pe32Header(0x00100000);
            Given_Section(".text", 0x1000u, 0x1000u);
            Given_DelayLoadDirectories(
                new DelayLoadDirectoryEntry
                {
                    Attributes = PeImageLoader.DID_RvaBased,
                    Name = "user32.dll",
                    ImportNames = new string[] {
                        "GetDesktopWindow",
                        "GetFocus"
                    }
                });
            sc.AddService<IDiagnosticsService>(new Mock<IDiagnosticsService>().Object);

            Given_PeLoader();

            var program = peldr.Load(addrLoad);
            peldr.Relocate(program, addrLoad);

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

        private void AssertImageSymbols(string sExp)
        {
            var sb = new StringBuilder();
            foreach (var item in this.peldr.ImageSymbols.Values)
            {
                sb.AppendFormat("{0:X} {1:X4} {2} {3}",
                    item.Address,
                    item.DataType.Size,
                    item.Type,
                    item.DataType);
                sb.AppendLine();
            }
            var sActual = sb.ToString();
            if (sActual != sExp)
                Debug.Print(sActual);
            Assert.AreEqual(sExp, sActual);
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
            var sExp =
@"00102000 0004 Data word32
00102004 0004 Data word32
00102008 0004 Data word32
0010202A 0004 Data (ptr32 code)
0010202E 0004 Data (ptr32 code)
00102032 0004 Data (ptr32 code)
";
            AssertImageSymbols(sExp);
        }

        [Test]
        public void Pil32_DllMain()
        {
            Given_Pe32Header(0x00100000);

            Given_PeLoader();
            var ep = peldr.CreateMainEntryPoint(true, Address.Ptr32(0x10000000), this.win32.Object);

            Assert.AreEqual("DllMain", ep.Name);
            Assert.AreEqual("fn(stdapi,arg(BOOL),(arg(hModule,HANDLE),arg(dwReason,DWORD),arg(lpReserved,LPVOID)))", ep.Signature.ToString());
        }

        [Test]
        public void Pil32_Win32CrtStartup()
        {
            Given_Pe32Header(0x00100000);

            Given_PeLoader();
            var ep = peldr.CreateMainEntryPoint(false, Address.Ptr32(0x10000000), this.win32.Object);

            Assert.AreEqual("Win32CrtStartup", ep.Name);
            Assert.AreEqual("fn(__cdecl,arg(DWORD),())", ep.Signature.ToString());
        }

        [Test]
        public void Pil32_IdenticallyNamedSections()
        {
            Given_Pe32Header(0x00100000);
            Given_Section("hehe", 0x1000, 0x1000);
            Given_Section("hehe", 0x2000, 0x2000);

            Given_PeLoader();
            var program = peldr.Load(addrLoad);
            Assert.AreEqual(3, program.SegmentMap.Segments.Count);
            Assert.AreEqual("hehe", program.SegmentMap.Segments[Address.Ptr32(0x00101000)].Name);
            Assert.AreEqual("hehe", program.SegmentMap.Segments[Address.Ptr32(0x00102000)].Name);
        }

		[Test]
		public void Pil32_NullSectionNames()
		{
			Given_Pe32Header(0x00100000);
			Given_Section("\x00\x00", 0x1000, 0x1000);
			Given_Section("\x00\x00\x00", 0x2000, 0x2000);

			Given_PeLoader();
			var program = peldr.Load(addrLoad);
			Assert.AreEqual(3, program.SegmentMap.Segments.Count);
			Assert.AreEqual(".reko_0000000000001000", program.SegmentMap.Segments[Address.Ptr32(0x00101000)].Name);
			Assert.AreEqual(".reko_0000000000002000", program.SegmentMap.Segments[Address.Ptr32(0x00102000)].Name);
		}
    }
}
