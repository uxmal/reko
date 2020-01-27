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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Services;
using Reko.ImageLoaders.MzExe;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.MzExe
{
    [TestFixture]
    public class NeImageLoaderTests
    {
        private ServiceContainer services;
        private byte[] bytes;
        private LeImageWriter writer;

        [SetUp]
        public void Setup()
        {
            this.services = new ServiceContainer();
            this.services.AddService<IDiagnosticsService>(new FakeDiagnosticsService());
            this.bytes = new byte[4096];
            this.writer = new LeImageWriter(bytes);
        }
        private void Given_Bundle(byte nEntries, byte iSeg, params BundleEntry[] entries)
        {
            writer.WriteByte(nEntries);
            writer.WriteByte(iSeg);
            foreach (var entry in entries)
            {
                writer.WriteByte(entry.flags);
                if (entry.flags == 0)
                    break;
                if (entry.iSeg != 0)
                {
                    writer.WriteBeUInt16(0xCD3F);   // INT 3F [sic]
                    writer.WriteByte(entry.iSeg);
                }
                writer.WriteLeUInt16(entry.offset);
            }
        }

        private BundleEntry Given_BundleEntry(byte flags, short offset)
        {
            return new BundleEntry { flags = flags, iSeg = 0, offset = (ushort) offset };
        }

        private BundleEntry Given_BundleEntry(byte flags, byte iSeg, short offset)
        {
            return new BundleEntry { flags = flags, iSeg = iSeg, offset = (ushort) offset };
        }

        //private BundleEntry Given_BundleEntry(byte flags)
        //{
        //    return new BundleEntry { flags = 0 };
        //}

        private class BundleEntry
        {
            public byte flags;
            public byte iSeg;
            public ushort offset;
        }

        private void Given_Segment(string v)
        {
            throw new NotImplementedException();
        }

        [Test(Description = "In response to GitHub issue #703")]
        public void Neldr_ReadEntryTable_Fixed()
        {
            var segs = new[]
            {
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x17, 0) },
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x27, 0) },
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x37, 0) },
            };

            Given_Bundle(3, 1,
                Given_BundleEntry(3, 0x42),
                Given_BundleEntry(3, 0x4B),
                Given_BundleEntry(3, 0x114B));
            Given_Bundle(0, 0);

            var neldr = new NeImageLoader(services, "FOO.DLL", bytes, 0);
            var syms = neldr.LoadEntryPoints(
                0,
                segs,
                new Dictionary<int, string>
                {
                    { 1, "FN0042" },
                    { 2, "FN004B" },
                    { 3, "FN114B" }
                },
                new X86ArchitectureProtected16("x86-protected-16"));
            Assert.AreEqual(3, syms.Count);
            Assert.AreEqual("FN0042 (0017:0042)", syms[0].ToString());
            Assert.AreEqual("FN004B (0017:004B)", syms[1].ToString());
            Assert.AreEqual("FN114B (0017:114B)", syms[2].ToString());
        }

        [Test(Description = "Sparse bundles")]
        public void Neldr_ReadEntryTable_Fixed_Sparse()
        {
            var segs = new[]
            {
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x17, 0) },
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x27, 0) },
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x37, 0) },
            };

            Given_Bundle(1, 1,
                Given_BundleEntry(2, 0x42));
            Given_Bundle(3, 0);
            Given_Bundle(2, 1,
                Given_BundleEntry(3, 0x4B),
                Given_BundleEntry(3, 0x3B));
            Given_Bundle(0, 0);

            var neldr = new NeImageLoader(services, "FOO.DLL", bytes, 0);
            var syms = neldr.LoadEntryPoints(
                0,
                segs,
                new Dictionary<int, string>
                {
                    { 1, "ORDINAL1" },
                    { 2, "**BOOM**" },
                    { 5, "ORDINAL5" },
                    { 6, "ORDINAL6" },
                },
                new X86ArchitectureProtected16("x86-protected-16"));
            Assert.AreEqual(3, syms.Count);
            Assert.AreEqual("ORDINAL1 (0017:0042)", syms[0].ToString());
            Assert.AreEqual("ORDINAL5 (0017:004B)", syms[1].ToString());
            Assert.AreEqual("ORDINAL6 (0017:003B)", syms[2].ToString());
        }

        [Test(Description = "Moveable entries")]
        public void Neldr_ReadEntryTable_Moveable()
        {
            var segs = new[]
            {
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x17, 0) },
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x27, 0) },
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x37, 0) },
            };

            Given_Bundle(1, 0xFF,
                Given_BundleEntry(2, 1, 0x42));
            Given_Bundle(3, 0);
            Given_Bundle(2, 0xFF,
                Given_BundleEntry(3, 2, 0x4B),
                Given_BundleEntry(3, 3, 0x3B));
            Given_Bundle(0, 0);

            var neldr = new NeImageLoader(services, "FOO.DLL", bytes, 0);
            var syms = neldr.LoadEntryPoints(
                0,
                segs,
                new Dictionary<int, string>
                {
                    { 1, "ORDINAL1" },
                    { 2, "**BOOM**" },
                    { 5, "ORDINAL5" },
                    { 6, "ORDINAL6" },
                },
                new X86ArchitectureProtected16("x86-protected-16"));
            Assert.AreEqual(3, syms.Count);
            Assert.AreEqual("ORDINAL1 (0017:0042)", syms[0].ToString());
            Assert.AreEqual("ORDINAL5 (0027:004B)", syms[1].ToString());
            Assert.AreEqual("ORDINAL6 (0037:003B)", syms[2].ToString());
        }

        [Test(Description = "Moveable entries")]
        public void Neldr_ReadEntryTable_SkipBundle()
        {
            var segs = new[]
            {
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x17, 0) },
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x27, 0) },
                new NeImageLoader.NeSegment { Address = Address.ProtectedSegPtr(0x37, 0) },
            };

            Given_Bundle(1, 1,
                Given_BundleEntry(2, 0x42));
            Given_Bundle(7, 0x00);      // Skip 7 bundles.
            Given_Bundle(2, 2,
                Given_BundleEntry(3, 0x4B),
                Given_BundleEntry(3, 0x3B));
            Given_Bundle(0, 0);
            var neldr = new NeImageLoader(services, "FOO.DLL", bytes, 0);
            var syms = neldr.LoadEntryPoints(
                0,
                segs,
                new Dictionary<int, string>
                {
                    { 1, "ORDINAL1" },
                    { 2, "**BOOM**" },
                    { 9, "ORDINAL9" },
                    { 10, "ORDINAL10" },
                },
                new X86ArchitectureProtected16("x86-protected-16"));
            Assert.AreEqual(3, syms.Count);
            Assert.AreEqual("ORDINAL1 (0017:0042)", syms[0].ToString());
            Assert.AreEqual("ORDINAL9 (0027:004B)", syms[1].ToString());
            Assert.AreEqual("ORDINAL10 (0027:003B)", syms[2].ToString());
        }

    }
}
