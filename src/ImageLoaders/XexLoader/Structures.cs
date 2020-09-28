#region License
/* 
 * Copyright (C) 2018-2020 Stefano Moioli <smxdev4@gmail.com>.
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
 * aUInt32 with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Reko.ImageLoaders.Xex.Enums;

namespace Reko.ImageLoaders.Xex
{
    public class Structures
    {
        public const UInt32 XEX1_MAGIC = 0x58455831; //XEX1
		public const UInt32 XEX2_MAGIC = 0x58455832; //XEX2

		public enum XexImportType : byte
		{
            Data = 0,
            Function = 1
		}

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexVersion
        {
            public UInt32 value;

            public UInt32 GetMajor() => value & 0xF0000000;
            public UInt32 GetMinor() => value & 0xF000000;
            public UInt32 GetBuild() => value & 0xFFFF00;
            public UInt32 GetQFE() => value & 0xFF;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexExecutionInfo
        {
            public UInt32 media_id;
            public XexVersion version;
            public XexVersion base_version;
            public UInt32 title_id;
            public byte platform;
            public byte executable_table;
            public byte disc_number;
            public byte disc_count;
            public UInt32 savegame_id;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexGameRating
        {
            public byte esrb;
            public byte pegi;
            public byte pegifi;
            public byte pegipt;

            public byte bbfc;
            public byte cero;
            public byte usk;
            public byte oflcau;

            public byte oflcnz;
            public byte kmrb;
            public byte brazil;
            public byte fpb;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexTlsInfo
        {
            public UInt32 slot_count;
            public UInt32 raw_data_address;
            public UInt32 data_size;
            public UInt32 raw_data_size;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexEncryptionHeader
        {
            public XEXEncryptionType encryption_type;
            public XEXCompressionType compression_type;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexFileBasicCompressionBlock
        {
            public UInt32 data_size;
            public UInt32 zero_size;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexFileNormalCompressionInfo
        {
            public UInt32 window_size;
            public UInt32 window_bits;
            public UInt32 block_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] block_hash;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexFileFormat
        {
            public UInt32 encryption_type;
            public UInt32 compression_type;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexHeader
        {
            public UInt32 magic;
            public UInt32 module_flags;
            public UInt32 header_size;
            public UInt32 discardable_headers_length;

            public UInt32 security_offset;
            public UInt32 header_count;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexOptionalHeader
        {
            public XEXHeaderKeys key;
            public UInt32 offset;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexResourceInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public byte[] name;
            public UInt32 address;
            public UInt32 size;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexImportLibraryBlockHeader
        {
            public UInt32 string_table_size;
            public UInt32 count;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexImportLibaryHeader
        {
            public UInt32 unknown;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] digest;
            public UInt32 import_id;
            public XexVersion version;
            public XexVersion min_version;
            public UInt16 name_index;
            public UInt16 record_count;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct XexSection
        {
            public UInt32 value;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] digest;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct Xex1LoaderInfo
		{
			public UInt32 header_size;
			public UInt32 image_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public byte[] rsa_signature;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
			public byte[] image_digest;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
			public byte[] import_table_digest;
			public UInt32 load_address;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] aes_key;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] xdg2_media_id;
			public UInt32 region;
			public UInt32 image_flags;
			public UInt32 export_table;
			public UInt32 allowed_media_types;
			public UInt32 page_descriptor_count;
		}

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct Xex2LoaderInfo
        {
            public UInt32 header_size;
            public UInt32 image_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] rsa_signature;
            public UInt32 unklength;
            public UInt32 image_flags;
            public UInt32 load_address;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] section_digest;
            public UInt32 import_table_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] import_table_digest;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] media_id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] aes_key;
            public UInt32 export_table;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] header_digest;
            public UInt32 game_regions;
            public UInt32 media_flags;
        }

        [Endian(Endianness.LittleEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct DOSHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public sbyte[] signature;
            public UInt16 lastsize;
            public UInt16 nblocks;
            public UInt16 nreloc;
            public UInt16 hdrsize;
            public UInt16 minalloc;
            public UInt16 maxalloc;
            public UInt16 ss;
            public UInt16 sp;
            public UInt16 checksum;
            public UInt16 ip;
            public UInt16 cs;
            public UInt16 relocpos;
            public UInt16 noverlay;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] reserved1;
            public UInt16 oem_id;
            public UInt16 oem_info;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public UInt16[] reserved2;
            public UInt32 e_lfanew;

            public void Validate()
            {
                if (signature[0] != 'M' && signature[1] != 'Z') {
                    throw new BadImageFormatException($"PE: Invalid file signature, should be MZ, found '{signature[0]}{signature[1]}'");
                }
            }
        }

        [Endian(Endianness.LittleEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct COFFHeader
        {
            public UInt16 Machine;
            public UInt16 NumberOfSections;
            public UInt32 TimeDateStamp;
            public UInt32 PointerToSymbolTable;
            public UInt32 NumberOfSymbols;
            public UInt16 SizeOfOptionalHeader;
            public UInt16 Characteristics;
        }

        [Endian(Endianness.LittleEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct COFFSection
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Name;
            public UInt32 VirtualSize;
            public UInt32 VirtualAddress;
            public UInt32 SizeOfRawData;
            public UInt32 PointerToRawData;
            public UInt32 PointerToRelocations;
            public UInt32 PointerToLinenumbers;
            public UInt16 NumberOfRelocations;
            public UInt16 NumberOfLinenumbers;
            public PESectionFlags Flags;
        }

        [Endian(Endianness.LittleEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct PEDataDirectory
        {
            public UInt32 VirtualAddress;
            public UInt32 Size;
        }

        [Endian(Endianness.LittleEndian)]
        [StructLayout(LayoutKind.Sequential)]
        public struct PEOptHeader
        {
            public UInt16 signature; //decimal number 267.
            public sbyte MajorLinkerVersion;
            public sbyte MinorLinkerVersion;
            public UInt32 SizeOfCode;
            public UInt32 SizeOfInitializedData;
            public UInt32 SizeOfUninitializedData;
            public UInt32 AddressOfEntryPoint;  //The RVA of the code entry point
            public UInt32 BaseOfCode;
            public UInt32 BaseOfData;
            public UInt32 ImageBase;
            public UInt32 SectionAlignment;
            public UInt32 FileAlignment;
            public UInt16 MajorOSVersion;
            public UInt16 MinorOSVersion;
            public UInt16 MajorImageVersion;
            public UInt16 MinorImageVersion;
            public UInt16 MajorSubsystemVersion;
            public UInt16 MinorSubsystemVersion;
            public UInt32 Reserved;
            public UInt32 SizeOfImage;
            public UInt32 SizeOfHeaders;
            public UInt32 Checksum;
            public UInt16 Subsystem;
            public UInt16 DLLCharacteristics;
            public UInt32 SizeOfStackReserve;
            public UInt32 SizeOfStackCommit;
            public UInt32 SizeOfHeapReserve;
            public UInt32 SizeOfHeapCommit;
            public UInt32 LoaderFlags;
            public UInt32 NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public PEDataDirectory[] DataDirectory;     //Can have any number of elements, matching the number in NumberOfRvaAndSizes.

            public void Validate()
            {
                if(signature != 267) {
                    throw new BadImageFormatException($"Invalid optional PE signature, should be 267, found {signature}");
                }
            }
        }
    }
}
