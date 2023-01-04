#region License
/*
 * Copyright (C) 2018-2023 Stefano Moioli <smxdev4@gmail.com>.
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it 
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented;
 *    you must not claim that you wrote the original software.
 *    If you use this software in a product, an acknowledgment
 *    in the product documentation would be appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such,
 *    and must not be misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
#endregion
using Reko.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Xex
{
    public class IMAGE_DOS_HEADER
    {
        public const ushort IMAGE_DOS_SIGNATURE = 0x5A4D;

        public ushort  e_magic;                     /* Magic number */
        public ushort  e_cblp;                      /* Bytes on last page of file */
        public ushort  e_cp;                        /* Pages in file */
        public ushort  e_crlc;                      /* Relocations */
        public ushort  e_cparhdr;                   /* Size of header in paragraphs */
        public ushort  e_minalloc;                  /* Minimum extra paragraphs needed */
        public ushort  e_maxalloc;                  /* Maximum extra paragraphs needed */
        public ushort  e_ss;                        /* Initial (relative) SS value */
        public ushort  e_sp;                        /* Initial SP value */
        public ushort  e_csum;                      /* Checksum */
        public ushort  e_ip;                        /* Initial IP value */
        public ushort  e_cs;                        /* Initial (relative) CS value */
        public ushort  e_lfarlc;                    /* File address of relocation table */
        public ushort  e_ovno;                      /* Overlay number */
        public ushort[]  e_res;                    /* Reserved words */
        public ushort  e_oemid;                     /* OEM identifier (for e_oeminfo) */
        public ushort  e_oeminfo;                   /* OEM information; e_oemid specific */
        public ushort[]  e_res2;                  /* Reserved words */
        public int e_lfanew;                    /* File address of new exe header */

        public readonly int SIZEOF;

        public IMAGE_DOS_HEADER(SpanStream r)
        {
            var pos = r.Position;
            e_magic = r.ReadUInt16();
            e_cblp = r.ReadUInt16();
            e_cp = r.ReadUInt16();
            e_crlc = r.ReadUInt16();
            e_cparhdr = r.ReadUInt16();
            e_minalloc = r.ReadUInt16();
            e_maxalloc = r.ReadUInt16();
            e_ss = r.ReadUInt16();
            e_sp = r.ReadUInt16();
            e_csum = r.ReadUInt16();
            e_ip = r.ReadUInt16();
            e_cs = r.ReadUInt16();
            e_lfarlc = r.ReadUInt16();
            e_ovno = r.ReadUInt16();
            e_res = Enumerable.Range(0, 4).Select(_ => r.ReadUInt16()).ToArray();
            e_oemid = r.ReadUInt16();
            e_oeminfo = r.ReadUInt16();
            e_res2 = Enumerable.Range(0, 10).Select(_ => r.ReadUInt16()).ToArray();
            e_lfanew = r.ReadInt32();
            SIZEOF = (int)(r.Position - pos);
        }

        public IMAGE_DOS_HEADER(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.LittleEndian))
        { }
    }

    public class IMAGE_FILE_HEADER
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;

        public readonly int SIZEOF = 20;

        public IMAGE_FILE_HEADER(SpanStream r)
        {
            Machine = r.ReadUInt16();
            NumberOfSections = r.ReadUInt16(); 
            TimeDateStamp = r.ReadUInt32();
            PointerToSymbolTable = r.ReadUInt32();
            NumberOfSymbols = r.ReadUInt32();
            SizeOfOptionalHeader = r.ReadUInt16();
            Characteristics = r.ReadUInt16();
        }

        public IMAGE_FILE_HEADER(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.LittleEndian))
        { }

        public const ushort IMAGE_FILE_MACHINE_POWERPCBE = 0x01F2;
        public const ushort IMAGE_FILE_32BIT_MACHINE = 0x0100;
        public const int IMAGE_SIZEOF_NT_OPTIONAL_HEADER = 224;
    }

    public class IMAGE_SECTION_HEADER
    {
        public string Name;
        public uint Misc;
        public uint PhysicalAddress => Misc;
        public uint VirtualSize => Misc;

        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public uint Characteristics;

        public const int IMAGE_SIZEOF_SHORT_NAME = 8;

        public const uint IMAGE_SCN_MEM_EXECUTE = 0x20000000;
        public const uint IMAGE_SCN_MEM_READ = 0x40000000;
        public const uint IMAGE_SCN_MEM_WRITE = 0x80000000;

        public bool IsReadable => (Characteristics & IMAGE_SCN_MEM_READ) == IMAGE_SCN_MEM_READ;
        public bool IsWritable => (Characteristics & IMAGE_SCN_MEM_WRITE) == IMAGE_SCN_MEM_WRITE;
        public bool IsExecutable => (Characteristics & IMAGE_SCN_MEM_EXECUTE) == IMAGE_SCN_MEM_EXECUTE;

        public IMAGE_SECTION_HEADER(SpanStream r)
        {
            Name = r.ReadString(IMAGE_SIZEOF_SHORT_NAME).TrimEnd((char)0);
            Misc = r.ReadUInt32();
            VirtualAddress = r.ReadUInt32();
            SizeOfRawData = r.ReadUInt32();
            PointerToRawData = r.ReadUInt32();
            PointerToRelocations = r.ReadUInt32();
            PointerToLinenumbers = r.ReadUInt32();
            NumberOfRelocations = r.ReadUInt16();
            NumberOfLinenumbers = r.ReadUInt16();
            Characteristics = r.ReadUInt32();
        }

        public IMAGE_SECTION_HEADER(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.LittleEndian))
        { }
    }

    public class IMAGE_DATA_DIRECTORY
    {
        public uint VirtualAddress;
        public uint Size;

        public readonly int SIZEOF = 8;

        public IMAGE_DATA_DIRECTORY(SpanStream r)
        {
            VirtualAddress = r.ReadUInt32();
            Size = r.ReadUInt32();
        }
        public IMAGE_DATA_DIRECTORY(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.LittleEndian))
        { }
    }

    public class IMAGE_OPTIONAL_HEADER
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;

        public uint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Reserved1;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public uint SizeOfStackReserve;
        public uint SizeOfStackCommit;
        public uint SizeOfHeapReserve;
        public uint SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;

        public int SIZEOF;

        public IMAGE_DATA_DIRECTORY[] DataDirectory;

        const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;

        public const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
        public const ushort IMAGE_SUBSYSTEM_XBOX = 14;

        public const int IMAGE_DIRECTORY_ENTRY_EXPORT         =  0;   // Export Directory
        public const int IMAGE_DIRECTORY_ENTRY_IMPORT         =  1;   // Import Directory
        public const int IMAGE_DIRECTORY_ENTRY_RESOURCE       =  2;   // Resource Directory
        public const int IMAGE_DIRECTORY_ENTRY_EXCEPTION      =  3;   // Exception Directory
        public const int IMAGE_DIRECTORY_ENTRY_SECURITY       =  4;   // Security Directory
        public const int IMAGE_DIRECTORY_ENTRY_BASERELOC      =  5;   // Base Relocation Table
        public const int IMAGE_DIRECTORY_ENTRY_DEBUG          =  6;   // Debug Directory
        public const int IMAGE_DIRECTORY_ENTRY_ARCHITECTURE   =  7;   // Architecture Specific Data
        public const int IMAGE_DIRECTORY_ENTRY_GLOBALPTR      =  8;   // RVA of GP
        public const int IMAGE_DIRECTORY_ENTRY_TLS            =  9;   // TLS Directory
        public const int IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG    = 10;   // Load Configuration Directory
        public const int IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT   = 11;   // Bound Import Directory in headers
        public const int IMAGE_DIRECTORY_ENTRY_IAT            = 12;   // Import Address Table
        public const int IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT   = 13;   // Delay Load Import Descriptors
        public const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;   // COM Runtime descriptor

        public IMAGE_OPTIONAL_HEADER(SpanStream r)
        {
            var pos = r.Position;
            Magic = r.ReadUInt16();
            MajorLinkerVersion = r.ReadByte();
            MinorLinkerVersion = r.ReadByte();
            SizeOfCode = r.ReadUInt32();
            SizeOfInitializedData = r.ReadUInt32();
            SizeOfUninitializedData = r.ReadUInt32();
            AddressOfEntryPoint = r.ReadUInt32();
            BaseOfCode = r.ReadUInt32();
            BaseOfData = r.ReadUInt32();
            ImageBase = r.ReadUInt32();
            SectionAlignment = r.ReadUInt32();
            FileAlignment = r.ReadUInt32();
            MajorOperatingSystemVersion = r.ReadUInt16();
            MinorOperatingSystemVersion = r.ReadUInt16();
            MajorImageVersion = r.ReadUInt16();
            MinorImageVersion = r.ReadUInt16();
            MajorSubsystemVersion = r.ReadUInt16();
            MinorSubsystemVersion = r.ReadUInt16();
            Reserved1 = r.ReadUInt32();
            SizeOfImage = r.ReadUInt32();
            SizeOfHeaders = r.ReadUInt32();
            CheckSum = r.ReadUInt32();
            Subsystem = r.ReadUInt16();
            DllCharacteristics = r.ReadUInt16();
            SizeOfStackReserve = r.ReadUInt32();
            SizeOfStackCommit = r.ReadUInt32();
            SizeOfHeapReserve = r.ReadUInt32();
            SizeOfHeapCommit = r.ReadUInt32();
            LoaderFlags = r.ReadUInt32();
            NumberOfRvaAndSizes = r.ReadUInt32();
            DataDirectory = Enumerable.Range(0, IMAGE_NUMBEROF_DIRECTORY_ENTRIES)
                .Select(_ => new IMAGE_DATA_DIRECTORY(r))
                .ToArray();

            SIZEOF = (int)(r.Position - pos);
        }

        public IMAGE_OPTIONAL_HEADER(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.LittleEndian))
        { }
    }

    public class IMAGE_IMPORT_DESCRIPTOR
    {
        private uint DUMMYUNIONNAME;

        public uint Characteristics => DUMMYUNIONNAME;
        public uint OriginalFirstThunk => DUMMYUNIONNAME;

        public uint TimeDateStamp;
        public uint ForwarderChain;
        public uint Name;
        public uint FirstThunk;

        public const int SIZEOF = 20;

        public IMAGE_IMPORT_DESCRIPTOR() { }

        public IMAGE_IMPORT_DESCRIPTOR(SpanStream r)
        {
            DUMMYUNIONNAME = r.ReadUInt32();
            TimeDateStamp = r.ReadUInt32();
            ForwarderChain = r.ReadUInt32();
            Name = r.ReadUInt32();
            FirstThunk = r.ReadUInt32();
        }

        public void Write(Memory<byte> bytes) => Write(new SpanStream(bytes, Endianness.LittleEndian));
        public void Write(SpanStream r)
        {
            r.WriteUInt32(DUMMYUNIONNAME);
            r.WriteUInt32(TimeDateStamp);
            r.WriteUInt32(ForwarderChain);
            r.WriteUInt32(Name);
            r.WriteUInt32(FirstThunk);
        }

        public IMAGE_IMPORT_DESCRIPTOR(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.LittleEndian))
        { }
    }

    public class IMAGE_THUNK_DATA32
    {
        public uint value;

        public const int SIZEOF = 4;

        /*public uint ForwarderString => value;
        public uint Function => value;
        public uint Ordinal => value;
        public uint AddressOfData => value;*/

        public bool IsOrdinal => ((value >> 31) & 1) == 1;

        public uint Ordinal
        {
            get
            {
                if (!IsOrdinal) throw new InvalidOperationException();
                return value & 0xFFFF;
            }
        }

        public uint HintRVA
        {
            get
            {
                if (IsOrdinal) throw new InvalidOperationException();
                return value >> 1;
            }
        }


        public IMAGE_THUNK_DATA32()
        {}

        public void Write(Memory<byte> bytes) => Write(new SpanStream(bytes, Endianness.LittleEndian));
        public void Write(SpanStream r)
        {
            r.WriteUInt32(value);
        }


        public IMAGE_THUNK_DATA32(SpanStream r)
        {
            value = r.ReadUInt32();
        }

        public IMAGE_THUNK_DATA32(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.LittleEndian))
        { }
    }

    public class IMAGE_NT_HEADERS
    {
        public const uint IMAGE_NT_SIGNATURE = 0x00004550;

        public uint Signature;
        public IMAGE_FILE_HEADER FileHeader;
        public IMAGE_OPTIONAL_HEADER OptionalHeader;

        /// <summary>
        /// Including optional headers
        /// </summary>
        public int SIZEOF;

        public IMAGE_NT_HEADERS(SpanStream r)
        {
            var pos = r.Position;
            Signature = r.ReadUInt32();
            FileHeader = new IMAGE_FILE_HEADER(r);
            OptionalHeader = new IMAGE_OPTIONAL_HEADER(r);

            SIZEOF = 4 + FileHeader.SIZEOF + FileHeader.SizeOfOptionalHeader;
        }

        public IMAGE_NT_HEADERS(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.LittleEndian))
        { }
    }
}
