#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

// http://hitmen.c02.at/files/yapspd/psp_doc/chap26.html - PSP ELF

using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.ImageLoaders.Elf
{
    /// <summary>
    /// Loader for ELF images.
    /// </summary>
    public class ElfImageLoader : ProgramImageLoader
    {
        #region Constants
        private const int ELF_MAGIC = 0x7F454C46;         // "\x7FELF"
        private const byte ELFDATA2LSB = 1;
        private const byte ELFDATA2MSB = 2;
        private const byte ELFCLASS32 = 1;              // 32-bit object file
        private const byte ELFCLASS64 = 2;              // 64-bit object file
        public const int HEADER_OFFSET = 0x0010;

        public const int ET_NONE = 0x01;
        public const int ET_REL = 0x01;
        public const int ET_EXEC = 0x02;
        public const int ET_DYN = 0x03;
        public const int ET_CORE = 0x04;

        #endregion

        internal static readonly TraceSwitch trace = new TraceSwitch(nameof(ElfImageLoader), "Traces the progress of the ELF image loader") { Level = TraceLevel.Warning };

        private Address addrPreferred;

        public ElfImageLoader(IServiceProvider services, ImageLocation imageLocation, byte[] rawBytes)
            : base(services, imageLocation, rawBytes)
        {
            this.addrPreferred = Address.Ptr32(0);
        }

        public override Address PreferredBaseAddress 
        {
            get { return addrPreferred; }
            set { addrPreferred = value; }
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            var rdr = new BeImageReader(this.RawImage, 0);
            var elfHeader = LoadElfIdentification(rdr);
            var binaryImage = new ElfBinaryImage(
                elfHeader,
                elfHeader.endianness == ELFDATA2MSB
                    ? EndianServices.Big
                    : EndianServices.Little);
            var innerLoader = CreateLoader(binaryImage);
            var arch = innerLoader.CreateArchitecture(innerLoader.Machine, innerLoader.Endianness);
            var platform = innerLoader.LoadPlatform(elfHeader.osAbi, arch);
            
            var headers = innerLoader.LoadSegments();
            binaryImage.AddSections(innerLoader.LoadSectionHeaders());
            innerLoader.LoadSymbolsFromSections();
            //innerLoader.Dump();           // This spews a lot into the unit test output.
            Program program;
            Dictionary<ElfSymbol, Address> plt;
            if (headers.Count > 0)
            {
                program = innerLoader.LoadImage(platform, RawImage);
                plt = new Dictionary<ElfSymbol, Address>();
            }
            else
            {
                if (addrLoad is not null)
                {
                    addrLoad = innerLoader.CreateAddress(addrLoad.ToLinear());
                }
                innerLoader.Dump(addrLoad ?? innerLoader.CreateAddress(0), Console.Out);

                // The file we're loading is an object file, and needs to be 
                // linked before we can load it.
                var linker = innerLoader.CreateLinker(arch);
                program = linker.LinkObject(platform, addrLoad, RawImage);
                plt = linker.PltEntries;
            }
            innerLoader.Relocate(innerLoader.Machine, program, addrLoad!, plt);
            return program;
        }

        public ElfHeader LoadElfIdentification()
        {
            var rdr = new BeImageReader(base.RawImage, 0);
            return LoadElfIdentification(rdr);
        }

        public ElfHeader LoadElfIdentification(EndianImageReader rdr)
        {
            var elfMagic = rdr.ReadBeInt32();
            if (elfMagic != ELF_MAGIC)
                throw new BadImageFormatException("File is not in ELF format.");
            var elfHeader = new ElfHeader();
            elfHeader.fileClass = rdr.ReadByte();
            elfHeader.endianness = rdr.ReadByte();
            elfHeader.fileVersion = rdr.ReadByte();
            elfHeader.osAbi = rdr.ReadByte();
            elfHeader.abiVersion = rdr.ReadByte();
            return elfHeader;
        }

        public EndianImageReader CreateReader(uint endianness, ulong fileOffset)
        {
            switch (endianness)
            {
            case ELFDATA2LSB: return new LeImageReader(RawImage, (long)fileOffset);
            case ELFDATA2MSB: return new BeImageReader(RawImage, (long)fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public EndianImageReader CreateReader(uint endianness, uint fileOffset)
        {
            switch (endianness)
            {
            case ELFDATA2LSB: return new LeImageReader(RawImage, fileOffset);
            case ELFDATA2MSB: return new BeImageReader(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public ImageWriter CreateWriter(uint endianness, uint fileOffset)
        {
            switch (endianness)
            {
            case ELFDATA2LSB: return new LeImageWriter(RawImage, fileOffset);
            case ELFDATA2MSB: return new BeImageWriter(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public ElfLoader CreateLoader(ElfBinaryImage elf)
        {
            var rdr = CreateReader(elf.Header.endianness, HEADER_OFFSET);
            var endianness = elf.Header.endianness == ElfLoader.ELFDATA2LSB
                ? EndianServices.Little
                : EndianServices.Big;
            if (elf.Header.fileClass == ELFCLASS64)
            {
                var header64 = Elf64_EHdr.Load(rdr);
                trace.Verbose("== ELF header =================");
                trace.Verbose("  e_entry: {0:X16}", header64.e_entry);
                trace.Verbose("  e_phoff: {0:X16}", header64.e_phoff);
                trace.Verbose("  e_shoff: {0:X16}", header64.e_shoff);
                trace.Verbose("  e_flags: {0:X8}", header64.e_flags);
                trace.Verbose("  e_ehsize: {0}", header64.e_ehsize);
                trace.Verbose("  e_phentsize: {0}", header64.e_phentsize);
                trace.Verbose("  e_phnum: {0}", header64.e_phnum);
                trace.Verbose("  e_shentsize: {0}", header64.e_shentsize);
                trace.Verbose("  e_shnum: {0}", header64.e_shnum);
                trace.Verbose("  e_shstrndx: {0}", header64.e_shstrndx);
                elf.Header.BinaryFileType = FileTypeOf(header64.e_type);
                elf.Header.StartAddress = Address.Ptr64(header64.e_entry);
                elf.Header.Machine = (ElfMachine) header64.e_machine;
                elf.Header.e_phoff = header64.e_phoff;
                elf.Header.e_shoff = header64.e_shoff;
                elf.Header.Flags = header64.e_flags;
                elf.Header.e_phnum = header64.e_phnum;
                elf.Header.e_shnum = header64.e_shnum;
                elf.Header.e_shstrndx = header64.e_shstrndx;
                elf.Header.PointerType = PrimitiveType.Ptr64;
                return new ElfLoader64(this.Services, elf, RawImage);
            }
            else
            {
                var header32 = Elf32_EHdr.Load(rdr);
                trace.Verbose("== ELF header =================");
                trace.Verbose("  e_entry: {0:X8}", header32.e_entry);
                trace.Verbose("  e_phoff: {0:X8}", header32.e_phoff);
                trace.Verbose("  e_shoff: {0:X8}", header32.e_shoff);
                trace.Verbose("  e_flags: {0:X8}", header32.e_flags);
                trace.Verbose("  e_ehsize: {0}", header32.e_ehsize);
                trace.Verbose("  e_phentsize: {0}", header32.e_phentsize);
                trace.Verbose("  e_phnum: {0}", header32.e_phnum);
                trace.Verbose("  e_shentsize: {0}", header32.e_shentsize);
                trace.Verbose("  e_shnum: {0}", header32.e_shnum);
                trace.Verbose("  e_shstrndx: {0}", header32.e_shstrndx);
                elf.Header.BinaryFileType = FileTypeOf(header32.e_type);
                elf.Header.StartAddress = Address.Ptr32(header32.e_entry);
                elf.Header.Machine = (ElfMachine) header32.e_machine;
                elf.Header.e_phoff = header32.e_phoff;
                elf.Header.e_shoff = header32.e_shoff;
                elf.Header.Flags = header32.e_flags;
                elf.Header.e_phnum = header32.e_phnum;
                elf.Header.e_shnum = header32.e_shnum;
                elf.Header.e_shstrndx = header32.e_shstrndx;
                elf.Header.PointerType = PrimitiveType.Ptr32;
                return new ElfLoader32(this.Services, elf, RawImage);
            }
        }

        private BinaryFileType FileTypeOf(ushort e_type)
        {
            return e_type switch
            {
                ET_REL => BinaryFileType.ObjectFile,
                ET_EXEC => BinaryFileType.Executable,
                ET_DYN => BinaryFileType.SharedLibrary,
                _ => BinaryFileType.Unknown,
            };
        }
    }
}
