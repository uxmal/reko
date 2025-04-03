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

// http://hitmen.c02.at/files/yapspd/psp_doc/chap26.html - PSP ELF

using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
            innerLoader.LoadFileHeader();

            var headers = innerLoader.LoadSegments();
            binaryImage.AddSections(innerLoader.LoadSectionHeaders());
            innerLoader.LoadSymbolsFromSections();
            innerLoader.LoadRelocations();
            innerLoader.LoadDynamicSegment();
            var dynRelocs = innerLoader.LoadDynamicRelocations();
            binaryImage.AddDynamicRelocations(dynRelocs);

            // At this point the image metadata has been loaded from disk.
            // The .text, .data and .bss - like segments are not yet 
            // loaded. That is what LoadProgram should do.

            var fmt = this.CreateBinaryFormatter(binaryImage);
            var sw = new StringWriter();
            fmt.FormatDynamicRelocations(sw);
            Debug.WriteLine(sw.ToString());


            //innerLoader.Dump();           // This spews a lot into the unit test output.
            Program program;
            Dictionary<ElfSymbol, Address> plt;
            var arch = innerLoader.CreateArchitecture(binaryImage.Header.Machine, innerLoader.Endianness);
            var platform = innerLoader.LoadPlatform(elfHeader.osAbi, arch);
            if (headers.Count > 0)
            {
                program = innerLoader.LoadImage(platform, RawImage);
                plt = new Dictionary<ElfSymbol, Address>();
            }
            else
            {
                if (addrLoad is not null)
                {
                    addrLoad = innerLoader.CreateAddress(addrLoad.Value.ToLinear());
                }
                innerLoader.Dump(addrLoad ?? innerLoader.CreateAddress(0), Console.Out);

                // The file we're loading is an object file, and needs to be 
                // linked before we can load it.
                var linker = innerLoader.CreateLinker(arch);
                program = linker.LinkObject(platform, addrLoad, RawImage);
                plt = linker.PltEntries;
            }
            innerLoader.Relocate(binaryImage.Header.Machine, program, addrLoad!.Value, plt);
            return program;
        }

        public override IBinaryFormatter CreateBinaryFormatter(IBinaryImage image)
        {
            return base.CreateBinaryFormatter(image);
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
            if (elf.Header.fileClass == ELFCLASS64)
            {
                return new ElfLoader64(this.Services, elf, RawImage);
            }
            else
            {
                return new ElfLoader32(this.Services, elf, RawImage);
            }
        }
    }
}
