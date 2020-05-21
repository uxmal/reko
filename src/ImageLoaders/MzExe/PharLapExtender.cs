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

using Reko.Core;
using Reko.Core.Pascal;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.MzExe
{
    public class PharLapExtender : ImageLoader
    {

        public PharLapExtender(IServiceProvider services, string filename, byte[] rawImage, uint headerOffset) 
            : base(services, filename, rawImage)
        {
            this.FileHeaderOffset = headerOffset;
        }

        public uint FileHeaderOffset { get; }

        public override Address PreferredBaseAddress 
        {
            get { return Address.Ptr32(0x00100000); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            var rdr = new LeImageReader(RawImage, FileHeaderOffset);
            var fileHeader = rdr.ReadStruct<FileHeader>();
            var image = new MemoryArea(Address.Ptr32(fileHeader.base_load_offset), new byte[fileHeader.memory_requirements]);
            var w = new LeImageWriter(image.Bytes);
            if ((fileHeader.flags & 1) != 0)
            {
                rdr = new LeImageReader(RawImage, fileHeader.offset_load_image);
                while (rdr.TryReadUInt16(out ushort us))
                {
                    if ((us & 0x8000) == 0)
                    {
                        rdr.ReadBytes(w.Bytes, w.Position, us);
                        w.Position += us;
                    }
                    else
                    {
                        us &= 0x7FFF;
                        rdr.TryReadByte(out var b);
                        for (int i = 0; i < us; ++i)
                        {
                            w.WriteByte(b);
                        }
                    }
                }

            }



            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }

        [Endian(Endianness.LittleEndian)]
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct FileHeader
        {
            public ushort signature;    // "P2" for 286 .EXP executable, "P3" for 386 .EXP
            public ushort level;        // 01h flat-model file, 02h multisegmented file (02h)
            public ushort header_size;  // (04h)
            public uint file_size;      // in bytes (06h)
            public ushort checksum;     // (0Ah)
            public uint offset_runtime_parameters;  // offset of run-time parameters within file(see #01622) (0Ch)
            public uint size_runtime_parameters;    // in bytes (10h)
            public uint offset_relocation_table;    // offset of relocation table table within file (14h)
            public uint size_relocation_table;      // in bytes (18h)
            public uint offset_segment_information_table;   // within file (see #01621) (1Ch)
            public uint size_segment_information_table;     // in bytes (20h)
            public ushort size_segment_information_table_entry;   // entry size in bytes (24h)
            public uint offset_load_image;          // offset within file (26h)
            public uint size_load_image;            // size on disk (2Ah)
            public uint offset_symboltable;         // offset within file or 00000000h (2Eh)
            public uint size_symboltable;           // in bytes (32h)
            public uint offset_GDT;                 // offset of GDT within load image (36h)
            public uint size_GDT;                   // in bytes (3Ah)
            public uint offset_LDT;                 // within load image (3Eh)
            public uint size_LDT;                   // in bytes (42h)
            public uint offset_IDT;                 // within load image (46h)
            public uint size_IDT;                   // in bytes (4Ah)
            public uint offset_TSS;                 // within load image (4Eh)
            public uint size_TSS;                   // in bytes (52h)
            public uint min_alloc;                  //  minimum number of extra bytes to be allocated at end of program (56h)
                                                    // (level 1 executables only)
            public uint max_alloc;                  // maximum number of extra bytes to be allocated at end of program (5Ah)
                                                    // (level 1 executables only)
            public uint base_load_offset;           // (level 1 executables only) (5Eh)
            public uint initial_ESP;                // (62h)
            public ushort initial_SS;               // (66h)
            public uint initial_EIP;                // (68h)
            public ushort initial_CS;               // (6Ch)
            public ushort initial_LDT;              // (6Eh)
            public ushort initial_TSS;              // (70h)
            public ushort flags;                    // (72h)
                //bit 0: Load image is packed
                //bit 1: 32-bit checksum is present
                //bits 4-2: Type of relocation table
            public uint memory_requirements;        // for load image (74h)
            public uint checksum_32;                // 32-bit checksum(optional) (78h)
            public uint size_stacksegment;          // size of stack segment in bytes (7Ch)
            // public 256 BYTEs reserved(0) (80h)
        }
    }
}
