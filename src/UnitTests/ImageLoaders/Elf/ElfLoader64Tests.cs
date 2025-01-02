#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.ImageLoaders.Elf;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;

namespace Reko.UnitTests.ImageLoaders.Elf
{
    [TestFixture]
    public class ElfLoader64Tests
    {
        private byte[] bytes;
        private List<ElfSegment> programHeaders;
        private List<ElfSection> sections;
        private Mock<IPlatform> platform;
        private ElfLoader64 el64;
        private Elf64_EHdr eih;
        private ServiceContainer sc;
        private ElfImageLoader eil;

        [SetUp]
        public void Setup()
        {
            programHeaders = new List<ElfSegment>();
            sections = new List<ElfSection>();
            platform = new Mock<IPlatform>();
            this.sc = new ServiceContainer();
            var cfgSvc = new Mock<IConfigurationService>();
            var arch = new Mock<IProcessorArchitecture>();
            platform.Setup(p => p.MakeAddressFromLinear(It.IsAny<ulong>(), It.IsAny<bool>()))
                .Returns((ulong u, bool b) => Address.Ptr64(u));
            cfgSvc.Setup(c => c.GetArchitecture("x86-protected-64")).Returns(arch.Object);
            sc.AddService<IConfigurationService>(cfgSvc.Object);
        }

        private void Given_RawImage(string bytes)
        {
            this.bytes = bytes.Split(' ')
                .Select(s => byte.Parse(s, NumberStyles.HexNumber))
                .ToArray();
        }

        private void Given_ProgramHeader(
            ProgramHeaderType type,
            uint imageOffset,
            uint virtualAddress,
            uint fileSize,
            uint loadedSize)
        {
            programHeaders.Add(new ElfSegment
            {
                p_type = type,
                FileOffset = imageOffset,
                VirtualAddress = Address.Ptr64(virtualAddress),
                FileSize = fileSize,
                MemorySize = loadedSize,
            });
        }

        private void Given_Section(string name, uint addr, string mode)
        {
            sections.Add(new ElfSection
            {
                Name = name,
                VirtualAddress = Address.Ptr32(addr),
                Flags = AccessFlags(mode)
            });
        }

        private uint AccessFlags(string mode)
        {
            uint flags = 0;
            foreach (var ch in mode)
            {
                switch (ch)
                {
                case 'w': flags |= ElfLoader.SHF_WRITE; break;
                case 'x': flags |= ElfLoader.SHF_EXECINSTR; break;
                }
            }
            return flags;
        }

        private void Given_ImageHeader(ElfMachine machine)
        {
            eih = new Elf64_EHdr
            {
                e_machine = (ushort)machine,
            };
        }

        private void When_CreateLoader64(bool big_endian)
        {
            this.eil = new ElfImageLoader(sc, ImageLocation.FromUri("file:foo"), this.bytes);
            var eh = new ElfHeader();
            var bin = new ElfBinaryImage(eh, big_endian ? EndianServices.Big : EndianServices.Little);
            this.el64 = new ElfLoader64(sc, bin, this.bytes);
            el64.BinaryImage.AddSegments(programHeaders);
            el64.BinaryImage.AddSections(sections);
        }

        [Test]
        public void EL64_DisjointSegments()
        {
            Given_RawImage("C0 DE 00 00 00 00 00 00 DA 7A 00 00");
            Given_ImageHeader(ElfMachine.EM_X86_64);
            Given_ProgramHeader(ProgramHeaderType.PT_LOAD, 0, 0x1000, 8, 8);
            Given_ProgramHeader(ProgramHeaderType.PT_LOAD, 8, 0x2000, 4, 16);
            Given_Section(".text", 0x1000, "rx");
            Given_Section(".data", 0x2000, "rw");
            Given_Section(".bss", 0x2008, "rw");

            When_CreateLoader64(false);
            var segmentMap = el64.LoadImageBytes(platform.Object, this.bytes, Address.Ptr64(0x1000));

            ImageSegment segText;
            Assert.IsTrue(segmentMap.TryFindSegment(Address.Ptr64(0x1001), out segText));
            Assert.AreEqual(".text", segText.Name);
            Assert.AreEqual(8, segText.Size);
        }


        [Test]
        public void El64_Executable_NoSections()
        {
            Given_RawImage("C0 DE 00 00 00 00 00 00 DA 7A 00 00");
            Given_ImageHeader(ElfMachine.EM_X86_64);
            Given_ProgramHeader(ProgramHeaderType.PT_LOAD, 0, 0x1000, 8, 8);

            When_CreateLoader64(false);
            var segmentMap = el64.LoadImageBytes(platform.Object, this.bytes, Address.Ptr64(0x1000));

            Assert.AreEqual(1, segmentMap.Segments.Count);

        }
    }
}