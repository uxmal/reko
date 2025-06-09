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
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.ImageLoaders.Elf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Elf
{
    public class ElfTests
    {
        protected Mock<IProcessorArchitecture> arch;
        protected Mock<IProcessorArchitecture> arch32be;

        private MemoryStream segnametab;
        protected MemoryStream symbolStringtab;
        protected List<Elf32_Sym> symbols;
        protected ServiceContainer sc;
        protected List<ProgramHeader> progHeaders;
        protected List<ObjectSection> objectSections;
        protected Mock<IConfigurationService> cfgSvc;
        protected byte[] rawBytes;

        public class ProgramHeader
        {
            public ProgramHeaderType Type;
            public uint Offset;
            public uint VirtualAddress;
            public byte[] Content;
            public uint AllocateSize;
            public uint Flags;
            public uint Alignment;
        }

        public class ObjectSection
        {
            public SectionHeaderType Type;
            public string Name;
            public byte[] Content;
            public uint Flags;
            public ushort Link;
            public uint Offset;
            public uint ElementSize;
        }

        public virtual void Setup()
        {
            this.sc = new ServiceContainer();

            this.cfgSvc = new Mock<IConfigurationService>();
            this.sc.AddService<IConfigurationService>(cfgSvc.Object);
            this.segnametab = new MemoryStream();
            this.segnametab.WriteByte(0);

            this.symbolStringtab = new MemoryStream();
            this.symbolStringtab.WriteByte(0);
            this.symbols = new List<Elf32_Sym> { new Elf32_Sym() };

            this.progHeaders = new List<ProgramHeader>();

            this.objectSections = new List<ObjectSection>
            {
                new ObjectSection { Name = "" },        // dummy section always present
                new ObjectSection { Name = ".shstrtab", Type = SectionHeaderType.SHT_STRTAB, Flags = 0 },
            };
        }

        protected void Given_BeArchitecture()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.arch.Setup(a => a.CreateImageWriter()).Returns(() => new BeImageWriter());
            cfgSvc.Setup(d => d.GetArchitecture("sparc32")).Returns(arch.Object);
        }

        protected int Given_SegName(string segname)
        {
            int i = (int)segnametab.Position;
            var bytes = Encoding.ASCII.GetBytes(segname);
            segnametab.Write(bytes, 0, bytes.Length);
            segnametab.WriteByte(0);
            return i;
        }

        protected void Given_Symbol(
            string name,
            uint st_value,
            uint st_size,
            byte st_info,
            ushort st_shndx)
        {
            var iName = (uint)symbolStringtab.Position;
            var bytes = Encoding.ASCII.GetBytes(name);
            symbolStringtab.Write(bytes, 0, bytes.Length);
            symbolStringtab.WriteByte(0);
            symbols.Add(new Elf32_Sym
            {
                st_name = iName,
                st_value = st_value,
                st_size = st_size,
                st_info = st_info,
                st_shndx = st_shndx
            });
        }

        protected void Given_Section(string name, SectionHeaderType type, uint flags, byte[] blob)
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

        protected void BuildObjectFile32(bool big_endian)
        {
            // Add symbol table
            if (symbols.Count > 0)
            {
                Given_Section(".symtab", SectionHeaderType.SHT_SYMTAB, ElfLoader.SHF_ALLOC, FlattenSymbolTable(big_endian));
                var os = objectSections[objectSections.Count - 1];
                os.ElementSize = Elf32_Sym.Size;
                os.Link = (ushort)objectSections.Count;
                Given_Section(".strtab", SectionHeaderType.SHT_STRTAB, ElfLoader.SHF_ALLOC, symbolStringtab.ToArray());
            }

            ImageWriter bin = big_endian ? new BeImageWriter() : (ImageWriter)new LeImageWriter();
            bin.WriteByte(0x7F);
            bin.WriteBytes(new byte[] { 0x45, 0x4C, 0x46 });
            bin.WriteByte(1);    // 32-bit
            bin.WriteByte(big_endian ? ElfLoader.ELFDATA2MSB : ElfLoader.ELFDATA2LSB);
            bin.WriteByte(1);    // ELF version
            bin.WriteByte(0);    // OS ABI
            bin.WriteByte(0);    // OS version
            bin.WriteBytes(0, 7); // pad

            // ELF header

            bin.WriteUInt16(1);   // relocatable
            bin.WriteUInt16((ushort)ElfMachine.EM_SPARC);
            bin.WriteUInt32(1);   // version
            bin.WriteUInt32(0);   // entry point (none in reloc file)
            bin.WriteUInt32(0);   // program segment table offset (none in reloc file)
            bin.WriteUInt32((uint)bin.Position + 20); // point to section table.

            bin.WriteUInt32(0);   // e_flags;
            bin.WriteUInt16(0);   // e_ehsize;
            bin.WriteUInt16((ushort)Elf32_PHdr.Size);   // e_phentsize;
            bin.WriteUInt16((ushort)progHeaders.Count);   // e_phnum;
            bin.WriteUInt16(0);   // e_shentsize;
            bin.WriteUInt16((ushort)objectSections.Count);   // e_shnum;
            bin.WriteUInt16(1);   // e_shstrndx;

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

            // Reserve space for program header table and 
            // section table.
            var iProgHdrTable = (uint)bin.Position;
            var iShTable = (uint)bin.Position + (uint)(progHeaders.Count * Elf32_PHdr.Size);
            var iStrTable = iShTable + (uint)(40 * objectSections.Count);

            // Write string table
            var aStrtable = strtab.ToArray();
            objectSections[1].Content = aStrtable;
            var iContent = iStrTable;

            // Place remaining sections.
            foreach (var section in objectSections.Skip(1))
            {
                section.Offset = iContent;
                iContent = Align((uint)(iContent + section.Content.Length));
            }

            // Write the program header table
            foreach (var ph in this.progHeaders)
            {
                bin.WriteUInt32((uint)ph.Type);
                bin.WriteUInt32(ph.Offset);
                bin.WriteUInt32(ph.VirtualAddress);
                bin.WriteUInt32(0);
                bin.WriteUInt32((uint)ph.Content.Length);
                bin.WriteUInt32(ph.AllocateSize);
                bin.WriteUInt32(ph.Flags);
                bin.WriteUInt32(ph.Alignment);
            }

            // Write the section table.
            foreach (var os in this.objectSections)
            {
                bin.WriteUInt32((uint)mpOsToiName[os]);
                bin.WriteUInt32((uint)os.Type);
                bin.WriteUInt32(os.Flags);
                bin.WriteUInt32(0);

                bin.WriteUInt32(os.Offset);
                bin.WriteUInt32(os.Content is not null ? (uint)os.Content.Length : 0u);
                bin.WriteUInt32(os.Link);
                bin.WriteUInt32(0);

                bin.WriteUInt32(0);
                bin.WriteUInt32(os.ElementSize);
            }

            // write the non-null sections.
            foreach (var section in objectSections.Skip(1))
            {
                bin.WriteBytes(section.Content);
                Align(bin);
            }
            this.rawBytes = bin.ToArray();
        }

        private byte[] FlattenSymbolTable(bool big_endian)
        {
            var syms = big_endian ? new BeImageWriter() : (ImageWriter) new LeImageWriter();
            foreach (var sym in symbols)
            {
                syms.WriteUInt32(sym.st_name);
                syms.WriteUInt32(sym.st_value);
                syms.WriteUInt32(sym.st_size);
                syms.WriteByte(sym.st_info);
                syms.WriteByte(sym.st_other);
                syms.WriteUInt16(sym.st_shndx);
            }
            return syms.ToArray();
        }

        protected static uint Align(uint n)
        {
            return 4 * (((n + 3) / 4));
        }

        protected static void Align(ImageWriter strtab)
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
    }
}
