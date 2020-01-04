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

// http://hitmen.c02.at/files/yapspd/psp_doc/chap26.html - PSP ELF

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    /// <summary>
    /// Loader for ELF images.
    /// </summary>
    public class ElfImageLoader : ImageLoader
    {
        #region Constants
        private const int ELF_MAGIC = 0x7F454C46;         // "\x7FELF"
        private const byte LITTLE_ENDIAN = 1;
        private const byte BIG_ENDIAN = 2;
        private const byte ELFCLASS32 = 1;              // 32-bit object file
        private const byte ELFCLASS64 = 2;              // 64-bit object file
        public const int HEADER_OFFSET = 0x0010;

        public const int ET_REL = 0x01;

        #endregion

        internal static TraceSwitch trace = new TraceSwitch(nameof(ElfImageLoader), "Traces the progress of the ELF image loader") { Level = TraceLevel.Verbose };

        private byte fileClass;
        private byte endianness;
        private byte fileVersion;
        private byte osAbi;
        private Address addrPreferred;

        protected ElfLoader innerLoader;

        public ElfImageLoader(IServiceProvider services, string filename, byte[] rawBytes)
            : base(services, filename, rawBytes)
        {
        }

        public override Address PreferredBaseAddress 
        {
            get { return addrPreferred; }
            set { addrPreferred = value; }
        }

        public override Program Load(Address addrLoad)
        {
            LoadElfIdentification();
            this.innerLoader = CreateLoader();
            this.innerLoader.LoadArchitectureFromHeader();
            addrLoad = addrLoad ?? innerLoader.DefaultAddress;
            var platform = innerLoader.LoadPlatform(osAbi, innerLoader.Architecture);
            int cHeaders = innerLoader.LoadSegments();
            innerLoader.LoadSectionHeaders();
            innerLoader.LoadSymbolsFromSections();
            //innerLoader.Dump();           // This spews a lot into the unit test output.
            if (cHeaders > 0)
            {
                return innerLoader.LoadImage(platform, RawImage);
            }
            else
            {
                // The file we're loading is an object file, and needs to be 
                // linked before we can load it.
                var linker = innerLoader.CreateLinker();
                return linker.LinkObject(platform, addrLoad, RawImage);
            }
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            var reloc = innerLoader.Relocate(program, addrLoad);
            return reloc;
        }

        public void LoadElfIdentification()
        {
            var rdr = new BeImageReader(base.RawImage, 0);
            var elfMagic = rdr.ReadBeInt32();
            if (elfMagic != ELF_MAGIC)
                throw new BadImageFormatException("File is not in ELF format.");
            this.fileClass = rdr.ReadByte();
            this.endianness = rdr.ReadByte();
            this.fileVersion = rdr.ReadByte();
            this.osAbi = rdr.ReadByte();
        }

        public EndianImageReader CreateReader(ulong fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageReader(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageReader(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public EndianImageReader CreateReader(uint fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageReader(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageReader(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public ImageWriter CreateWriter(uint fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageWriter(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageWriter(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public ElfLoader CreateLoader()
        {
            var rdr = CreateReader(HEADER_OFFSET);
            if (fileClass == ELFCLASS64)
            {
                var header64 = Elf64_EHdr.Load(rdr);
                return new ElfLoader64(this, header64, RawImage, osAbi, endianness);
            }
            else
            {
                var header32 = Elf32_EHdr.Load(rdr);
                return new ElfLoader32(this, header32, RawImage, endianness);
            }
        }
    }
}
