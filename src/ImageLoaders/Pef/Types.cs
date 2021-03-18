#region License
/* 
 * Copyright (C) 2018-2021 Stefano Moioli <smxdev4@gmail.com>.
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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public enum OSType : UInt32
    {
        /// <summary>
        /// pwpc
        /// </summary>
        kPowerPCCFragArch = 0x70777063,
        /// <summary>
        /// m68k
        /// </summary>
        kMotorola68KCFragArch = 0x6D36386B,
        /// <summary>
        /// ????
        /// </summary>
        kAnyCFragArch = 0x3F3F3F3F
    }

    /// <summary>
    /// Various types of sections that can appear in PEF containers 
    /// </summary>
    public enum PEFSectionType : byte
    {
        /// <summary>
        /// Contains read-only executable code in an uncompressed binary format
        /// </summary>
        Code = 0,
        /// <summary>
        /// Contains uncompressed, initialized, read/write data followed by zero-initialized read/write data.
        /// </summary>
        UnpackedData = 1,
        /// <summary>
        /// Contains read/write data initialized by a pattern specification contained in the section’s contents.
        /// The contents essentially contain a small program that tells the Code Fragment Manager how to initialize the raw data in memory.
        /// </summary>
        PatternInitializedData = 2,
        /// <summary>
        /// Contains uncompressed, initialized, read-only data.
        /// </summary>
        Constant = 3,
        /// <summary>
        /// Contains information about imports, exports, and entry points.
        /// </summary>
        Loader = 4,
        /// <summary>
        /// Reserved for future use. 
        /// </summary>
        Debug = 5,
        /// <summary>
        /// Contains information that is both executable and modifiable.
        /// For example, this section can store code that contains embedded data.
        /// </summary>
        ExecutableData = 6,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Exception = 7,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Traceback = 8
    }

    public enum PEFSectionShareKind : byte
    {
        /// <summary>
        /// Indicates that the section is shared within a process, but a fresh copy is created for different processes.
        /// </summary>
        ProcessShare = 1,
        /// <summary>
        /// Indicates that the section is shared between all processes in the system.
        /// </summary>
        GlobalShare = 4,
        /// <summary>
        /// Indicates that the section is shared between all processes, but is protected.
        /// Protected sections are read/write in privileged mode and read-only in user mode.
        /// </summary>
        ProtectedShare = 5
    }

    [Endian(Endianness.BigEndian)]
    public struct PEFLoaderInfoHeader
    {
        /// <summary>
        ///  the number of the section in this container that contains the main symbol.
        ///  If the fragment does not have a main symbol, this field is set to -1.
        /// </summary>
        public Int32 mainSection;
        /// <summary>
        /// the offset (in bytes) from the beginning of the section to the main symbol.
        /// </summary>
        public UInt32 mainOffset;
        /// <summary>
        ///  the number of the section containing the initialization function’s transition vector.
        ///  If no initialization function exists, this field is set to -1.
        /// </summary>
        public Int32 initSection;
        /// <summary>
        /// the offset (in bytes) from the beginning of the section to the initialization function’s transition vector. 
        /// </summary>
        public UInt32 initOffset;
        /// <summary>
        /// the number of the section containing the termination routine’s transition vector.
        /// If no termination routine exists, this field is set to -1.
        /// </summary>
        public Int32 termSection;
        /// <summary>
        /// the offset (in bytes) from the beginning of the section to the termination routine’s transition vector. 
        /// </summary>
        public UInt32 termOffset;
        /// <summary>
        /// the number of imported libraries. 
        /// </summary>
        public UInt32 importedLibraryCount;
        /// <summary>
        ///  the total number of imported symbols.
        /// </summary>
        public UInt32 totalImportedSymbolCount;
        /// <summary>
        /// the number of sections containing load-time relocations.
        /// </summary>
        public UInt32 relocSectionCount;
        /// <summary>
        /// the offset (in bytes) from the beginning of the loader section to the start of the relocations area. 
        /// </summary>
        public UInt32 relocInstrOffset;
        /// <summary>
        /// the offset (in bytes) from the beginning of the loader section to the start of the loader string table. 
        /// </summary>
        public UInt32 loaderStringsOffset;
        /// <summary>
        /// the offset (in bytes) from the beginning of the loader section to the start of the export hash table.
        /// The hash table should be 4-byte aligned with padding added if necessary. 
        /// </summary>
        public UInt32 exportHashOffset;
        /// <summary>
        /// the number of hash index values (that is, the number of entries in the hash table).
        /// The number of entries is specified as a power of two.
        /// </summary>
        public UInt32 exportHashTablePower;
        /// <summary>
        /// the number of symbols exported from this container. 
        /// </summary>
        public UInt32 exportedSymbolCount;

        private PEFLoaderInfoHeader(EndianByteImageReader rdr)
        {
            this = rdr.ReadStruct<PEFLoaderInfoHeader>();
        }

        public static PEFLoaderInfoHeader Load(EndianByteImageReader rdr) => new PEFLoaderInfoHeader(rdr);
    }

    [Endian(Endianness.BigEndian)]
    public struct PEFImportedLibrary
    {
        /// <summary>
        /// the offset (in bytes) from the beginning of the loader string table to the start of the null-terminated library name. 
        /// </summary>
        public UInt32 nameOffset;
        /// <summary>
        /// version information for checking the compatibility of the imported library.
        /// </summary>
        public UInt32 oldImpVersion;
        /// <summary>
        /// version information for checking the compatibility of the imported library.
        /// </summary>
        public UInt32 currentVersion;
        /// <summary>
        ///  the number of symbols imported from this library.
        /// </summary>
        public UInt32 importedSymbolCount;
        /// <summary>
        /// the (zero-based) index of the first entry in the imported symbol table for this library.
        /// </summary>
        public UInt32 firstImportedSymbol;
        /// <summary>
        ///  order that the import libraries are initialized + whether the import library is weak. 
        /// </summary>
        public byte options;
        /// <summary>
        /// reserved and must be set to 0
        /// </summary>
        public byte reservedA;
        /// <summary>
        /// reserved and must be set to 0
        /// </summary>
        public UInt16 reservedB;

        private PEFImportedLibrary(EndianByteImageReader rdr)
        {
            this = rdr.ReadStruct<PEFImportedLibrary>();

            if(reservedA != 0 || reservedB != 0)
            {
                throw new BadImageFormatException("Invalid PEFImportedLibrary");
            }
        }

        public static PEFImportedLibrary Load(EndianByteImageReader rdr) => new PEFImportedLibrary(rdr);
    }

    [Endian(Endianness.BigEndian)]
    public struct PEFLoaderRelocationHeader
    {
        /// <summary>
        /// the section number to which this relocation header refers.
        /// </summary>
        public UInt16 sectionIndex;
        /// <summary>
        /// reserved and must be set to 0.
        /// </summary>
        public UInt16 reservedA;
        /// <summary>
        /// the number of 16-bit relocation blocks for this section.
        /// </summary>
        public UInt32 relocCount;
        /// <summary>
        /// the byte offset from the start of the relocations area to the first relocation instruction for this section.
        /// </summary>
        public UInt32 firstRelocOffset;

        private PEFLoaderRelocationHeader(EndianByteImageReader rdr)
        {
            this = rdr.ReadStruct<PEFLoaderRelocationHeader>();

            if(reservedA != 0)
            {
                throw new BadImageFormatException("Invalid PEFLoaderRelocationHeader");
            }
        }

        public static PEFLoaderRelocationHeader Load(EndianByteImageReader rdr) => new PEFLoaderRelocationHeader(rdr);
    }

    public enum PEFSymbolClassType : byte
    {
        /// <summary>
        /// A code address
        /// </summary>
        kPEFCodeSymbol = 0,
        /// <summary>
        /// A data address
        /// </summary>
        kPEFDataSymbol = 1,
        /// <summary>
        /// A standard procedure pointer
        /// </summary>
        kPEFTVectSymbol = 2,
        /// <summary>
        /// A direct data area (Table of Contents ) symbol
        /// </summary>
        kPEFTOCSymbol = 3,
        /// <summary>
        /// A linker-inserted glue symbol
        /// </summary>
        kPEFGlueSymbol = 4

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Endian(Endianness.BigEndian)]
    public struct PEFExportedSymbol
    {
        public UInt32 classAndName;
        public UInt32 symbolValue;
        public Int16 sectionIndex;

        private PEFExportedSymbol(EndianByteImageReader rdr)
        {
            this = rdr.ReadStruct<PEFExportedSymbol>();
        }

        public static PEFExportedSymbol Load(EndianByteImageReader rdr) => new PEFExportedSymbol(rdr);
    }

    [Endian(Endianness.BigEndian)]
    public struct PEFSectionHeader
    {
        /// <summary>
        /// the offset from the start of the section
        /// name table to the location of the section name.
        /// </summary>
        public Int32 nameOffset;
        /// <summary>
        /// the preferred address (as
        /// designated by the linker) at which to place the section’s instance
        /// </summary>
        public UInt32 defaultAddress;
        /// <summary>
        /// the size, in bytes, required by the
        /// section’s contents at execution time
        /// </summary>
        public UInt32 totalSize;
        /// <summary>
        /// the size of the section’s contents that is explicitly
        /// initialized from the container.
        /// </summary>
        public UInt32 unpackedSize;
        /// <summary>
        /// the size, in bytes, of a section’s
        /// contents in the container
        /// </summary>
        public UInt32 packedSize;
        /// <summary>
        /// the offset from the beginning of
        /// the container to the start of the section’s contents
        /// </summary>
        public UInt32 containerOffset;
        /// <summary>
        /// he type of section as well as any special attributes
        /// </summary>
        public PEFSectionType sectionKind;
        /// <summary>
        /// how the section information is shared
        /// among processes
        /// </summary>
        public PEFSectionShareKind shareKind;
        /// <summary>
        /// the desired alignment for instantiated
        /// sections in memory as a power of 2.
        /// </summary>
        public byte alignment;

        public byte reservedA;

        private PEFSectionHeader(EndianByteImageReader rdr)
        {
            this = rdr.ReadStruct<PEFSectionHeader>();
        }

        public static PEFSectionHeader Load(EndianByteImageReader rdr) => new PEFSectionHeader(rdr);

        public bool IsCompressedSection()
        {
            return sectionKind == PEFSectionType.PatternInitializedData;
        }

        public AccessMode GetAccessMode()
        {
            switch (sectionKind)
            {
            case PEFSectionType.Code:
                return AccessMode.ReadExecute;
            case PEFSectionType.Constant:
            case PEFSectionType.PatternInitializedData:
            case PEFSectionType.Loader:
            // The following sections are "Reserved for future use", assume R/O
            case PEFSectionType.Debug:
            case PEFSectionType.Exception:
            case PEFSectionType.Traceback:
                return AccessMode.Read;
            case PEFSectionType.UnpackedData:
                return AccessMode.ReadWrite;
            case PEFSectionType.ExecutableData:
                return AccessMode.ReadWriteExecute;
            default:
                throw new BadImageFormatException($"Unknown section kind {sectionKind:X2}");
            }
        }

    }


    [Endian(Endianness.BigEndian)]
    public struct PEFContainerHeader
    {
        public UInt32 tag1;
        public UInt32 tag2;
        /// <summary>
        /// indicates the architecture type that the
        /// container was generated for
        /// </summary>
        public OSType architecture;
        /// <summary>
        /// indicates the version of PEF used in the container
        /// </summary>
        public UInt32 formatVersion;
        /// <summary>
        /// indicates when the PEF container was created.
        /// The stamp follows the Macintosh time-measurement scheme(that is,
        /// the number of seconds measured from January 1, 1904).
        /// </summary>
        public UInt32 dateTimeStamp;
        public UInt32 oldDefVersion;
        public UInt32 oldImpVersion;
        public UInt32 currentVersion;
        /// <summary>
        /// toatal number of sections
        /// contained in the container
        /// </summary>
        public UInt16 sectionCount;
        /// <summary>
        /// number of instantiated
        /// sections.Instantiated sections contain code or data that are required for
        /// execution.
        /// </summary>
        public UInt16 instSectionCount;
        public UInt32 reservedA;

        /// <summary>
        /// Joy!
        /// </summary>
        private const UInt32 TAG1_MAGIC = 0x4A6F7921;

        /// <summary>
        /// peff
        /// </summary>
        private const UInt32 TAG2_MAGIC = 0x70656666;

        private PEFContainerHeader(EndianByteImageReader rdr)
        {
            this = rdr.ReadStruct<PEFContainerHeader>();
            if(tag1 != TAG1_MAGIC)
            {
                throw new BadImageFormatException("Unexpected tag1 magic");
            }

            if(tag2 != TAG2_MAGIC)
            {
                throw new BadImageFormatException("Unexpected tag2 magic");
            }

            switch (architecture)
            {
            case OSType.kPowerPCCFragArch:
            case OSType.kMotorola68KCFragArch:
            case OSType.kAnyCFragArch:
                break;
            default:
                throw new BadImageFormatException("Unexpected/unsupported architecture");
            }

            if(formatVersion != 1)
            {
                throw new BadImageFormatException($"Unsupported format version {formatVersion}");
            }
        }

        public static PEFContainerHeader Load(EndianByteImageReader rdr) => new PEFContainerHeader(rdr);
    }
}
