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
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Te
{
    /// <summary>
    /// Loads TE image files.
    /// </summary>
    public class TeImageLoader : AbstractPeLoader
    {
        private TeHeader hdr;
        private Address addrImageBase;

        /*
         doc-ref: https://uefi.org/sites/default/files/resources/PI_Spec_1_6.pdf
        seq:
          - id: te_hdr
            size: 0x28
            type: te_header
          - id: sections
            type: section
            repeat: expr
            repeat-expr: te_hdr.num_sections
        types:
          te_header:
            seq:
              - id: magic
                contents: "VZ"
              - id: machine
                type: u2
                enum: machine_type
              - id: num_sections
                type: u1
              - id: subsystem
                type: u1
                enum: subsystem_enum
              - id: stripped_size
                type: u2
              - id: entry_point_addr
                type: u4
              - id: base_of_code
                type: u4
              - id: image_base
                type: u8
              - id: data_dirs
                type: header_data_dirs*/
        public TeImageLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) : base(services, imageLocation, imgRaw)
        {
            var rdr = new LeImageReader(RawImage);
            if (!LoadTeHeader(rdr, out this.hdr!))
                throw new BadImageFormatException();
            this.addrImageBase = Address.Ptr64(hdr.image_base);
        }

        public override Address PreferredBaseAddress
        { 
            get => addrImageBase; 
            set => throw new NotImplementedException();
        }
        
        protected override SizeSpecificLoader Create32BitLoader(AbstractPeLoader outer)
        {
            return new Pe32Loader(this);
        }

        protected override SizeSpecificLoader Create64BitLoader(AbstractPeLoader outer)
        {
            return new Pe64Loader(this);
        }

        public override Program LoadProgram(Address? address)
        {
            var rdr = new LeImageReader(RawImage);
            if (!LoadTeHeader(rdr, out var hdr))
                throw new BadImageFormatException();
            if (!LoadSectionHeaders(rdr, hdr.num_sections, out var sections))
                throw new BadImageFormatException();
            var segments = LoadSectionData(sections);
            var arch = base.CreateArchitecture(hdr.machine);
            var platform = base.CreatePlatform(hdr.machine, Services, arch);
            return new Program(new ByteProgramMemory(segments), arch, platform);
        }

        private SegmentMap LoadSectionData(Section[] sections)
        {
            var segments = new ImageSegment[sections.Length];
            for (var iSection = 0; iSection < sections.Length; ++iSection)
            {
                var section = sections[iSection];
                var bytes = new byte[Math.Max(section.virtual_size, section.size_of_raw_data)];
                Array.Copy(
                    RawImage, section.pointer_to_raw_data,
                    bytes, 0,
                    Math.Min(section.size_of_raw_data, RawImage.Length - section.pointer_to_raw_data));
                var addr = this.addrImageBase + section.virtual_address;
                var mem = new ByteMemoryArea(addr, bytes);
                var access = AccessFromCharacteristics(section.characteristics);
                var segment = new ImageSegment(section.name ?? $".reko_{addr}", mem, access);
                segments[iSection] = segment;
            }
            return new SegmentMap(segments);
        }


        private class TeHeader
        {
            public ushort magic;
            public ushort machine;
            public byte num_sections;
            public byte subsystem;
            public ushort stripped_size;
            public uint entry_point_addr;
            public uint base_of_code;
            public ulong image_base;
        }

        private bool LoadTeHeader(LeImageReader rdr, [MaybeNullWhen(false)] out TeHeader te)
        {
            te = new TeHeader();
            if (!rdr.TryReadUInt16(out te.magic) ||
                te.magic != 0x5A56 || // 'VZ'
                !rdr.TryReadUInt16(out te.machine) ||
                !rdr.TryReadByte(out te.num_sections) ||
                !rdr.TryReadByte(out te.subsystem) ||
                !rdr.TryReadUInt16(out te.stripped_size) ||
                !rdr.TryReadUInt32(out te.entry_point_addr) ||
                !rdr.TryReadUInt32(out te.base_of_code) ||
                !rdr.TryReadUInt64(out te.image_base))
            {
                te = null;
                return false;
            }
            var data_dirs = LoadHeaderDataDirs(rdr);
            return true;
        }

        private object? LoadHeaderDataDirs(LeImageReader rdr)
        {
            if (!TryReadDataDir(rdr, out DataDirectory? base_relocation_table))
                return null;
            if (!TryReadDataDir(rdr, out DataDirectory? debug))
                return null;
            return null;
        }

        private bool TryReadDataDir(LeImageReader rdr, [MaybeNullWhen(false)] out DataDirectory dir)
        {
            if (!rdr.TryReadUInt32(out uint virtual_address) ||
                !rdr.TryReadUInt32(out uint size))
            {
                dir = null;
                return false;
            }
            dir = new DataDirectory
            {
                rvaAddress = virtual_address,
                size = size,
            };
            return true;
        }

        private class DataDirectory
        {
            public uint rvaAddress;
            public uint size;
        }

        private bool LoadSectionHeaders(LeImageReader rdr, int nSections, [MaybeNullWhen(false)] out Section[] sections)
        {
            sections = new Section[nSections];
            for (int iSection = 0; iSection < nSections; ++iSection)
            {
                var sec = new Section();
                sec.name = this.ReadSectionName(rdr, 0);
                if (!rdr.TryReadLeUInt32(out sec.virtual_size) ||
                    !rdr.TryReadLeUInt32(out sec.virtual_address) ||
                    !rdr.TryReadLeUInt32(out sec.size_of_raw_data) ||
                    !rdr.TryReadLeUInt32(out sec.pointer_to_raw_data) ||
                    !rdr.TryReadLeUInt32(out sec.pointer_to_relocations) ||
                    !rdr.TryReadLeUInt32(out sec.pointer_to_linenumbers) ||
                    !rdr.TryReadLeUInt16(out sec.num_relocations) ||
                    !rdr.TryReadLeUInt16(out sec.num_linenumbers) ||
                    !rdr.TryReadLeUInt32(out sec.characteristics))
                {
                    sections = null;
                    return false;
                }
                sections[iSection] = sec;
            }
            return true;
        }

        private class Section
        {
            public string? name;
            //section:
            //  seq:
            //    - id: name
            //      type: str
            //      encoding: UTF-8
            //      size: 8
            //      pad-right: 0
            public uint virtual_size;
            public uint virtual_address;
            public uint size_of_raw_data;
            public uint pointer_to_raw_data;
            public uint pointer_to_relocations;
            public uint pointer_to_linenumbers;
            public ushort num_relocations;
            public ushort num_linenumbers;
            public uint characteristics;
        }
    //instances:
    //  body:
    //    pos: pointer_to_raw_data - _root.te_hdr.stripped_size + _root.te_hdr._io.size
    //    size: size_of_raw_data
    }
}
