#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.ImageLoaders.Elf;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Elf
{
    [TestFixture]
    public class ElfObjectLinkerTests
    {
        private MockRepository mr;
        private MemoryStream segnametab;
        private MemoryStream binaryContents;
        private List<ObjectSection> objectSections;
        private byte[] rawBytes;
        private ElfObjectLinker32 linker;
        private IProcessorArchitecture arch;

        public class ObjectSection
        {
            public string Name;
            public byte[] Content;
            public uint Flags;
            public SectionHeaderType Type;

            public uint Offset { get; internal set; }
        }

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.segnametab = new MemoryStream();
            this.segnametab.WriteByte(0);

            this.binaryContents = new MemoryStream();

            this.objectSections = new List<ObjectSection>
            {
                new ObjectSection { Name = "" },        // dummy section always present
                new ObjectSection { Name = ".shstrtab", Type = SectionHeaderType.SHT_STRTAB, Flags = 0 },
            };

            this.arch = mr.Stub<IProcessorArchitecture>();
            this.arch.Stub(a => a.CreateImageWriter()).Do(new Func<ImageWriter>(() => new BeImageWriter()));
            this.arch.Replay();
        }

        private int Given_SegName(string segname)
        {
            int i = (int) segnametab.Position;
            var bytes = Encoding.ASCII.GetBytes(segname);
            segnametab.Write(bytes, 0, bytes.Length);
            segnametab.WriteByte(0);
            return i;
        }

        private void Given_Section(string name, SectionHeaderType type, uint flags, byte[] blob)
        {
            var os = new ObjectSection
            {
                Name = name,
                Type = type,
                Flags = flags,
                Content = blob,
            };
            objectSections.Add(os);
        }

        private void BuildObjectFile()
        {
            var bin = new BeImageWriter();
            bin.WriteByte(0x7F);
            bin.WriteBytes(new byte[] { 0x45, 0x4C, 0x46 });
            bin.WriteByte(1);    // 32-bit
            bin.WriteByte(2);    // big-endian
            bin.WriteByte(1);    // ELF version
            bin.WriteByte(0);    // OS ABI
            bin.WriteByte(0);    // OS version
            bin.WriteBytes(0, 7); // pad

            // ELF header

            bin.WriteBeUInt16(1);   // relocatable
            bin.WriteBeUInt16(4);   // 68k
            bin.WriteBeUInt32(1);   // version
            bin.WriteBeUInt32(0);   // entry point (none in reloc file)
            bin.WriteBeUInt32(0);   // program segment table offset (none in reloc file)
            bin.WriteBeUInt32((uint)bin.Position + 20); // point to section table.

            bin.WriteBeUInt32(0);   // e_flags;
            bin.WriteBeUInt16(0);   // e_ehsize;
            bin.WriteBeUInt16(0);   // e_phentsize;
            bin.WriteBeUInt16(0);   // e_phnum;
            bin.WriteBeUInt16(0);   // e_shentsize;
            bin.WriteBeUInt16((ushort)objectSections.Count);   // e_shnum;
            bin.WriteBeUInt16(1);   // e_shstrndx;

            // Build string table.

            var strtab = new MemoryStream();
            var mpOsToiName = new Dictionary<ObjectSection, int>();
            foreach (var os in this.objectSections)
            {
                mpOsToiName[os] = (int)strtab.Position;
                var bytes = Encoding.ASCII.GetBytes(os.Name);
                strtab.Write(bytes, 0, bytes.Length);
                strtab.WriteByte(0);
            }

            // Reserve place for section table
            var iShTable = (uint) bin.Position;
            var iStrTable = iShTable + (uint)(40 * objectSections.Count);

            // Write string table
            var aStrtable = strtab.ToArray();
            objectSections[1].Content = aStrtable;
            var iContent = iStrTable;

            // write remaining sections.
            foreach (var section in objectSections.Skip(1))
            {
                section.Offset = iContent;
                iContent = Align((uint)(iContent + section.Content.Length));
            }

            // Write the section table.
            foreach (var os in this.objectSections)
            {
                bin.WriteBeUInt32((uint)mpOsToiName[os]);
                bin.WriteBeUInt32((uint)os.Type);
                bin.WriteBeUInt32(os.Flags);
                bin.WriteBeUInt32(0);

                bin.WriteBeUInt32(os.Offset);
                bin.WriteBeUInt32(os.Content != null ? (uint)os.Content.Length : 0u);
                bin.WriteBeUInt32(0);
                bin.WriteBeUInt32(0);

                bin.WriteBeUInt32(0);
                bin.WriteBeUInt32(0);
            }

            // write the non-null sections.
            foreach (var section in objectSections.Skip(1))
            {
                bin.WriteBytes(section.Content);
                Align(bin);
            }
            this.rawBytes = bin.ToArray();
        }

        private void Given_Linker()
        {
            BuildObjectFile();

            var eil = new ElfImageLoader(null, "foo.o", rawBytes);
            eil.LoadElfIdentification();
            var eh = Elf32_EHdr.Load(new BeImageReader(rawBytes, ElfImageLoader.HEADER_OFFSET));
            var el = new ElfLoader32(eil, eh);
            el.LoadSectionHeaders();
            this.linker = new ElfObjectLinker32(el, arch, rawBytes);
        }

        private static uint Align(uint n)
        {
            return 4 * (((n + 3) / 4));
        }

        private static void Align(ImageWriter strtab)
        {
            while (strtab.Position % 4 != 0)
                strtab.WriteByte(0);
        }

        private static void Align(MemoryStream strtab)
        {
            // Align the string table.
            while (strtab.Position % 4 != 0)
                strtab.WriteByte(0);
        }

//                   sh_type: SHT_NULL     sh_flags:      sh_addr; 00000000 sh_offset: 00000000 sh_size: 00000000 sh_link: 00000000 sh_info: 00000000 sh_addralign: 00000000 sh_entsize: 00000000
//.shstrtab          sh_type: SHT_STRTAB   sh_flags:      sh_addr; 00000000 sh_offset: 00000034 sh_size: 00000052 sh_link: 00000000 sh_info: 00000000 sh_addralign: 00000001 sh_entsize: 00000000
//.text              sh_type: SHT_PROGBITS sh_flags: xa   sh_addr; 00000000 sh_offset: 00000088 sh_size: 00002154 sh_link: 00000000 sh_info: 00000000 sh_addralign: 00000008 sh_entsize: 00000000
//.data              sh_type: SHT_PROGBITS sh_flags:  aw  sh_addr; 00000000 sh_offset: 000021E0 sh_size: 000000B4 sh_link: 00000000 sh_info: 00000000 sh_addralign: 00000008 sh_entsize: 00000000
//.bss               sh_type: SHT_NOBITS   sh_flags:  aw  sh_addr; 00000000 sh_offset: 00002298 sh_size: 0000012C sh_link: 00000000 sh_info: 00000000 sh_addralign: 00000008 sh_entsize: 00000000
//.stab.index        sh_type: SHT_PROGBITS sh_flags:      sh_addr; 00000000 sh_offset: 00002298 sh_size: 00000024 sh_link: 00000006 sh_info: 00000000 sh_addralign: 00000004 sh_entsize: 0000000C
//.stab.indexstr     sh_type: SHT_STRTAB   sh_flags:      sh_addr; 00000000 sh_offset: 000022BC sh_size: 00000079 sh_link: 00000000 sh_info: 00000000 sh_addralign: 00000001 sh_entsize: 00000000
//.symtab            sh_type: SHT_SYMTAB   sh_flags:  a   sh_addr; 00000000 sh_offset: 00002338 sh_size: 00000300 sh_link: 00000008 sh_info: 00000023 sh_addralign: 00000004 sh_entsize: 00000010
//.strtab            sh_type: SHT_STRTAB   sh_flags:  a   sh_addr; 00000000 sh_offset: 00002638 sh_size: 00000232 sh_link: 00000000 sh_info: 00000000 sh_addralign: 00000001 sh_entsize: 00000000
//.rela.text         sh_type: SHT_RELA     sh_flags:  a   sh_addr; 00000000 sh_offset: 0000286C sh_size: 00000150 sh_link: 00000007 sh_info: 00000002 sh_addralign: 00000004 sh_entsize: 0000000C
        [Test]
        public void Eol32_CollectNeededSegments()
        {
            int iText = Given_SegName(".text");
            int iData = Given_SegName(".data");
            Given_Section(".text", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_EXECINSTR, new byte[] { 0xc3 });
            Given_Section(".data", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_WRITE, new byte[] { 0x01, 0x02, 0x03, 0x04 });

            Given_Linker();

            var segs = linker.CollectNeededSegments();
            Assert.AreEqual(2, segs.Count);
        }

        [Test]
        public void Eol32_CreateSegmentHeaders()
        {
            int iText = Given_SegName(".text");
            int iData = Given_SegName(".data");
            Given_Section(".text", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_EXECINSTR, new byte[] { 0xc3 });
            Given_Section(".data", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_WRITE, new byte[] { 0x01, 0x02, 0x03, 0x04 });

            Given_Linker();

            var segs = linker.CollectNeededSegments();
            var imageMap = linker.CreateSegments(Address.Ptr32(0x00800000), segs);
            Assert.AreEqual(2, imageMap.Segments.Count);
        }
    }
}
