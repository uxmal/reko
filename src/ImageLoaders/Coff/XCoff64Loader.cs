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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.IO;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Runtime.InteropServices;

namespace Reko.ImageLoaders.Coff
{
    public class XCoff64Loader : ProgramImageLoader
    {
        public XCoff64Loader(IServiceProvider services, ImageLocation imageUri, byte[]rawImage) :
            base(services, imageUri, rawImage)
        {
            this.PreferredBaseAddress = Address.Ptr64(0x0010_0000);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            BeImageReader rdr = new BeImageReader(this.RawImage, 0);
            FileHeader64? str = rdr.ReadStruct<FileHeader64>();
            if (!str.HasValue)
                throw new BadImageFormatException("Invalid XCoff64 header.");
            var mem = new ByteMemoryArea(addrLoad ?? PreferredBaseAddress, RawImage);
            var seg = new ImageSegment("foo", mem, AccessMode.ReadWriteExecute);
            var map = new SegmentMap(seg);
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("ppc-64-be")!;
            var platform = new DefaultPlatform(Services, arch);
            return new Program(map, arch, platform);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
	    [Endian(Endianness.BigEndian)]
        private struct FileHeader64
        {
            public ushort f_magic;
            public ushort f_nscns;
            public uint f_timdat;
            public ulong f_symptr;
            public uint f_nsyms;
            public ushort f_opthdr;
            public ushort f_flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.BigEndian)]
        public struct ExtFileHeader64
        {
            public ushort o_mflag; // Flags, offset 0
            public ushort o_vstamp; // Version, offset 2
            public uint o_debugger; // Reserved for debuggers., offset 4
            public ulong o_text_start; // Base address of text(virtual address), offset 8
            public ulong o_data_start; // Base address of data(virtual address), offset 16
            public ulong o_toc; // Address of TOC anchor, offset 24
            public ushort o_snentry; // Section number for entry point, offset 32
            public ushort o_sntext; // Section number for .text, offset 34
            public ushort o_sndata; // Section number for .data, offset 36
            public ushort o_sntoc; // Section number for TOC, offset 38
            public ushort o_snloader; // Section number for loader data, offset 40
            public ushort o_snbss; // Section number for .bss, offset 42
            public ushort o_algntext; // Maximum alignment for .text, offset 44
            public ushort o_algndata; // Maximum alignment for .data, offset 46
            public ushort o_modtype; // Module type field, offset 48
            public byte o_cpuflag; // Bit flags - cpu types of objects, offset 50
            public byte o_cputype; // Reserved for CPU type, offset 51
            public byte o_textpsize; // Requested text page size., offset 52
            public byte o_datapsize; // Requested data page size., offset 53
            public byte o_stackpsize; // Requested stack page size., offset 53
            public byte o_flags; // Flags and thread-local storage alignment, offset 55
            public ulong o_tsize; // Text size in bytes, offset 56
            public ulong o_dsize; // Initialized data size in bytes, offset 64
            public ulong o_bsize; // Uninitialized data size in bytes, offset 72
            public ulong o_entry; // Entry point descriptor(virtual address), offset 80
            public ulong o_maxstack; // Maximum stack size allowed(bytes), offset 88
            public ulong o_maxdata; // Maximum data size allowed(bytes), offset 96
            public ushort o_sntdata; // Section number for .tdata, offset 104
            public ushort o_sntbss; // Section number for .tbss, offset 106
            public ushort o_x64flags; // XCOFF64 flags, offset 108
        }
    }
}
