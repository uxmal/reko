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

    public enum PEFSectionType : byte
    {
        Code = 0,
        UnpackedData = 1,
        PatternInitializedData = 2,
        Constant = 3,
        Loader = 4,
        Debug = 5,
        ExecutableData = 6,
        Exception = 7,
        Traceback = 8
    }

    public enum PEFSectionShareKind : byte
    {
        ProcessShare = 1,
        GlobalShare = 4,
        ProtectedShare = 5
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

        private byte reservedA;

        public PEFSectionHeader(EndianByteImageReader rdr)
        {
            this = rdr.ReadStruct<PEFSectionHeader>();
        }

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
        private UInt32 tag1;
        private UInt32 tag2;
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
        private UInt32 reservedA;

        /// <summary>
        /// Joy!
        /// </summary>
        private const UInt32 TAG1_MAGIC = 0x4A6F7921;

        /// <summary>
        /// peff
        /// </summary>
        private const UInt32 TAG2_MAGIC = 0x70656666;

        public PEFContainerHeader(EndianByteImageReader rdr)
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
    }
}
