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
using Reko.Core;
using Reko.Core.Configuration;
using Reko.ImageLoaders.Elf;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Elf
{
    [TestFixture]
    public class ElfObjectLinkerTests : ElfTests
    {
        private ElfObjectLinker32 linker;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            Given_BeArchitecture();
        }



        private void Given_Linker(bool big_endian)
        {
            BuildObjectFile32();

            var eil = new ElfImageLoader(sc, "foo.o", rawBytes);
            eil.LoadElfIdentification();
            var eh = Elf32_EHdr.Load(new BeImageReader(rawBytes, ElfImageLoader.HEADER_OFFSET));
            var el = new ElfLoader32(eil, eh, rawBytes, big_endian ? ElfLoader.ELFDATA2MSB : ElfLoader.ELFDATA2LSB);
            el.LoadSectionHeaders();
            el.LoadSymbolsFromSections();
            this.linker = new ElfObjectLinker32(el, arch.Object, rawBytes);
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
            Given_SegName(".text");
            Given_SegName(".data");
            Given_Section(".text", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_EXECINSTR, new byte[] { 0xc3 });
            Given_Section(".data", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_WRITE, new byte[] { 0x01, 0x02, 0x03, 0x04 });

            Given_Linker(false);

            var segs = linker.ComputeSegmentSizes();
            Assert.AreEqual(4, segs.Count);
        }

        [Test]
        public void Eol32_CreateSegmentHeaders()
        {
            Given_SegName(".text");
            Given_SegName(".data");
            Given_Section(".text", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_EXECINSTR, new byte[] { 0xc3 });
            Given_Section(".data", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_WRITE, new byte[] { 0x01, 0x02, 0x03, 0x04 });

            Given_Linker(false);

            var segs = linker.ComputeSegmentSizes();
            var segmentMap = linker.CreateSegments(Address.Ptr32(0x00800000), segs);
            Assert.AreEqual(3, segmentMap.Segments.Count);
            Assert.AreEqual("00800000", segmentMap.Segments.ElementAt(0).Value.MemoryArea.BaseAddress.ToString());
            Assert.AreEqual("00800001", segmentMap.Segments.ElementAt(0).Value.MemoryArea.EndAddress.ToString());
            Assert.AreEqual("00801000", segmentMap.Segments.ElementAt(1).Value.MemoryArea.BaseAddress.ToString());
            Assert.AreEqual("00801004", segmentMap.Segments.ElementAt(1).Value.MemoryArea.EndAddress.ToString());
            Assert.AreEqual(0x1, segmentMap.Segments.ElementAt(1).Value.MemoryArea.Bytes[0]);
        }

        [Test(Description = "SHN_COMMON symbols should be added to the rw segment")]
        public void Eol32_CommonSymbol()
        {
            Given_SegName(".text");
            Given_SegName(".data");
            Given_Section(".text", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_EXECINSTR, new byte[] { 0xc3 });
            Given_Section(".data", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_WRITE, new byte[] { 0x01, 0x02, 0x03, 0x04 });
            Given_Symbol(
                "shared_global", 8, 0x4000,
                ElfLoader32.ELF32_ST_INFO(0, ElfSymbolType.STT_OBJECT),
                0xFFF2);

            Given_Linker(false);

            linker.ComputeSegmentSizes();
            Assert.AreEqual(0x4000, linker.Segments[3].p_pmemsz);
        }

        [Test(Description = "Unresolved symbols of STT_NOTYPE should live in their own segment.")]
        public void Eol32_UnresolvedExternals_OwnSegment()
        {
            Given_SegName(".text");
            Given_SegName(".data");
            Given_Section(".text", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_EXECINSTR, new byte[] { 0xc3 });
            Given_Section(".data", SectionHeaderType.SHT_PROGBITS, ElfLoader.SHF_ALLOC | ElfLoader.SHF_WRITE, new byte[] { 0x01, 0x02, 0x03, 0x04 });
            Given_Symbol(
                "unresolved_global1", 0, 0,
                ElfLoader32.ELF32_ST_INFO(0, ElfSymbolType.STT_NOTYPE),
                0);
            Given_Symbol(
                "unresolved_global2", 0, 0,
                ElfLoader32.ELF32_ST_INFO(0, ElfSymbolType.STT_NOTYPE),
                0);

            Given_Linker(false);

            linker.ComputeSegmentSizes();
            Assert.AreEqual(0x0030, linker.Segments[0].p_pmemsz, "Each external symbol is simulated with 16 bytes and added to executable section");
        }
    }
}
