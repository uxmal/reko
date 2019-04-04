#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System;


namespace Reko.ImageLoaders.Coff
{

    internal class ImageDataDirectory
    {
        internal uint VirtualAddress;              // Image relative address of table
        internal uint TableSize;                   // Size of table

        public const int Size = 8;

        public static ImageDataDirectory Load(LeImageReader rdr)
        {
            ImageDataDirectory idd = new ImageDataDirectory();

            idd.VirtualAddress= rdr.ReadLeUInt32();          
            idd.TableSize = rdr.ReadLeUInt32();

            return idd;
        }
    }


    public class OptionHeader32
    {
        // Value of Magic for optional header
        const ushort COFF_Magic_PE32 = 0x10B;
        const ushort COFF_Magic_PE64 = 0x20B;

        internal ushort Magic;                    // Magic number
        internal byte LinkerMajorVersion;
        internal byte LinkerMinorVersion;
        internal uint SizeOfCode;
        internal uint SizeOfInitializedData;
        internal uint SizeOfUninitializedData;
        internal uint AddressOfEntryPoint;      // Entry point relative to image base
        internal uint BaseOfCode;
        // Windows specific fields
        internal int ImageBase;                // Image base
        internal uint SectionAlignment;
        internal uint FileAlignment;
        internal ushort MajorOperatingSystemVersion;
        internal ushort MinorOperatingSystemVersion;
        internal ushort MajorImageVersion;
        internal ushort MinorImageVersion;
        internal ushort MajorSubsystemVersion;
        internal ushort MinorSubsystemVersion;
        internal uint Win32VersionValue;        // must be 0
        internal uint SizeOfImage;
        internal uint SizeOfHeaders;
        internal uint CheckSum;
        internal ushort Subsystem;
        internal ushort DllCharacteristics;
        internal uint SizeOfStackReserve;
        internal uint SizeOfStackCommit;
        internal uint SizeOfHeapReserve;
        internal uint SizeOfHeapCommit;
        internal uint LoaderFlags;
        internal uint NumberOfRvaAndSizes;
        // Data directories
        internal ImageDataDirectory ExportTable;
        internal ImageDataDirectory ImportTable;
        internal ImageDataDirectory ResourceTable;
        internal ImageDataDirectory ExceptionTable;
        internal ImageDataDirectory CertificateTable;
        internal ImageDataDirectory BaseRelocationTable;
        internal ImageDataDirectory Debug;
        internal ImageDataDirectory Architecture;
        internal ImageDataDirectory GlobalPtr;
        internal ImageDataDirectory TLSTable;
        internal ImageDataDirectory LoadConfigTable;
        internal ImageDataDirectory BoundImportTable;
        internal ImageDataDirectory ImportAddressTable;
        internal ImageDataDirectory DelayImportDescriptor;
        internal ImageDataDirectory CLRRuntimeHeader;
        internal ImageDataDirectory Reserved;

        public const int Size = 20;

        public static OptionHeader32 Load(LeImageReader rdr)
        {
            var header = new OptionHeader32();

            header.Magic = rdr.ReadLeUInt16();                   // Magic number
            header.LinkerMajorVersion = rdr.ReadByte();
            header.LinkerMinorVersion = rdr.ReadByte();
            header.SizeOfCode = rdr.ReadUInt32();
            header.SizeOfInitializedData = rdr.ReadUInt32();
            header.SizeOfUninitializedData = rdr.ReadUInt32();
            header.AddressOfEntryPoint = rdr.ReadUInt32();      // Entry point relative to image base
            header.BaseOfCode = rdr.ReadUInt32();
            // Windows specific fields
            header.ImageBase = rdr.ReadInt32();               // Image base
            header.SectionAlignment = rdr.ReadUInt32();
            header.FileAlignment = rdr.ReadUInt32();
            header.MajorOperatingSystemVersion = rdr.ReadLeUInt16();
            header.MinorOperatingSystemVersion = rdr.ReadLeUInt16();
            header.MajorImageVersion = rdr.ReadLeUInt16();
            header.MinorImageVersion = rdr.ReadLeUInt16();
            header.MajorSubsystemVersion = rdr.ReadLeUInt16();
            header.MinorSubsystemVersion = rdr.ReadLeUInt16();
            header.Win32VersionValue = rdr.ReadUInt32();        // must be 0
            header.SizeOfImage = rdr.ReadUInt32();
            header.SizeOfHeaders = rdr.ReadUInt32();
            header.CheckSum = rdr.ReadUInt32();
            header.Subsystem = rdr.ReadLeUInt16();
            header.DllCharacteristics = rdr.ReadLeUInt16();
            header.SizeOfStackReserve = rdr.ReadUInt32();
            header.SizeOfStackCommit = rdr.ReadUInt32();
            header.SizeOfHeapReserve = rdr.ReadUInt32();
            header.SizeOfHeapCommit = rdr.ReadUInt32();
            header.LoaderFlags = rdr.ReadUInt32();
            header.NumberOfRvaAndSizes = rdr.ReadUInt32();
            // Data directories
            header.ExportTable = ImageDataDirectory.Load(rdr);
            header.ImportTable = ImageDataDirectory.Load(rdr);
            header.ResourceTable = ImageDataDirectory.Load(rdr);
            header.ExceptionTable = ImageDataDirectory.Load(rdr);
            header.CertificateTable = ImageDataDirectory.Load(rdr);
            header.BaseRelocationTable = ImageDataDirectory.Load(rdr);
            header.Debug = ImageDataDirectory.Load(rdr);
            header.Architecture = ImageDataDirectory.Load(rdr);
            header.GlobalPtr = ImageDataDirectory.Load(rdr);
            header.TLSTable = ImageDataDirectory.Load(rdr);
            header.LoadConfigTable = ImageDataDirectory.Load(rdr);
            header.BoundImportTable = ImageDataDirectory.Load(rdr);
            header.ImportAddressTable = ImageDataDirectory.Load(rdr);
            header.DelayImportDescriptor = ImageDataDirectory.Load(rdr);
            header.CLRRuntimeHeader = ImageDataDirectory.Load(rdr);
            header.Reserved = ImageDataDirectory.Load(rdr);

            return header;
        }
    }


    public class OptionHeader64
    {
        internal ushort Magic;                    // Magic number
        internal byte LinkerMajorVersion;
        internal byte LinkerMinorVersion;
        internal uint SizeOfCode;
        internal uint SizeOfInitializedData;
        internal uint SizeOfUninitializedData;
        internal uint AddressOfEntryPoint;      // Entry point relative to image base
        internal uint BaseOfCode;
        // Windows specific fields
        internal Int64 ImageBase;                // Image base
        internal uint SectionAlignment;
        internal uint FileAlignment;
        internal ushort MajorOperatingSystemVersion;
        internal ushort MinorOperatingSystemVersion;
        internal ushort MajorImageVersion;
        internal ushort MinorImageVersion;
        internal ushort MajorSubsystemVersion;
        internal ushort MinorSubsystemVersion;
        internal uint Win32VersionValue;        // must be 0
        internal uint SizeOfImage;
        internal uint SizeOfHeaders;
        internal uint CheckSum;
        internal ushort Subsystem;
        internal ushort DllCharacteristics;
        internal UInt64 SizeOfStackReserve;
        internal UInt64 SizeOfStackCommit;
        internal UInt64 SizeOfHeapReserve;
        internal UInt64 SizeOfHeapCommit;
        internal uint LoaderFlags;
        internal uint NumberOfRvaAndSizes;
        // Data directories
        internal ImageDataDirectory ExportTable;
        internal ImageDataDirectory ImportTable;
        internal ImageDataDirectory ResourceTable;
        internal ImageDataDirectory ExceptionTable;
        internal ImageDataDirectory CertificateTable;
        internal ImageDataDirectory BaseRelocationTable;
        internal ImageDataDirectory Debug;
        internal ImageDataDirectory Architecture;
        internal ImageDataDirectory GlobalPtr;
        internal ImageDataDirectory TLSTable;
        internal ImageDataDirectory LoadConfigTable;
        internal ImageDataDirectory BoundImportTable;
        internal ImageDataDirectory ImportAddressTable;
        internal ImageDataDirectory DelayImportDescriptor;
        internal ImageDataDirectory CLRRuntimeHeader;
        internal ImageDataDirectory Reserved;

        public const int Size = 20;

        public static OptionHeader64 Load(LeImageReader rdr)
        {
            var header = new OptionHeader64();

            header.Magic = rdr.ReadLeUInt16();                   // Magic number
            header.LinkerMajorVersion = rdr.ReadByte();
            header.LinkerMinorVersion = rdr.ReadByte();
            header.SizeOfCode = rdr.ReadUInt32();
            header.SizeOfInitializedData = rdr.ReadUInt32();
            header.SizeOfUninitializedData = rdr.ReadUInt32();
            header.AddressOfEntryPoint = rdr.ReadUInt32();      // Entry point relative to image base
            header.BaseOfCode = rdr.ReadUInt32();
            // Windows specific fields
            header.ImageBase = rdr.ReadInt64();               // Image base
            header.SectionAlignment = rdr.ReadUInt32();
            header.FileAlignment = rdr.ReadUInt32();
            header.MajorOperatingSystemVersion = rdr.ReadLeUInt16();
            header.MinorOperatingSystemVersion = rdr.ReadLeUInt16();
            header.MajorImageVersion = rdr.ReadLeUInt16();
            header.MinorImageVersion = rdr.ReadLeUInt16();
            header.MajorSubsystemVersion = rdr.ReadLeUInt16();
            header.MinorSubsystemVersion = rdr.ReadLeUInt16();
            header.Win32VersionValue = rdr.ReadUInt32();        // must be 0
            header.SizeOfImage = rdr.ReadUInt32();
            header.SizeOfHeaders = rdr.ReadUInt32();
            header.CheckSum = rdr.ReadUInt32();
            header.Subsystem = rdr.ReadLeUInt16();
            header.DllCharacteristics = rdr.ReadLeUInt16();
            header.SizeOfStackReserve = rdr.ReadUInt64();
            header.SizeOfStackCommit = rdr.ReadUInt64();
            header.SizeOfHeapReserve = rdr.ReadUInt64();
            header.SizeOfHeapCommit = rdr.ReadUInt64();
            header.LoaderFlags = rdr.ReadUInt32();
            header.NumberOfRvaAndSizes = rdr.ReadUInt32();
            // Data directories
            header.ExportTable = ImageDataDirectory.Load(rdr);
            header.ImportTable = ImageDataDirectory.Load(rdr);
            header.ResourceTable = ImageDataDirectory.Load(rdr);
            header.ExceptionTable = ImageDataDirectory.Load(rdr);
            header.CertificateTable = ImageDataDirectory.Load(rdr);
            header.BaseRelocationTable = ImageDataDirectory.Load(rdr);
            header.Debug = ImageDataDirectory.Load(rdr);
            header.Architecture = ImageDataDirectory.Load(rdr);
            header.GlobalPtr = ImageDataDirectory.Load(rdr);
            header.TLSTable = ImageDataDirectory.Load(rdr);
            header.LoadConfigTable = ImageDataDirectory.Load(rdr);
            header.BoundImportTable = ImageDataDirectory.Load(rdr);
            header.ImportAddressTable = ImageDataDirectory.Load(rdr);
            header.DelayImportDescriptor = ImageDataDirectory.Load(rdr);
            header.CLRRuntimeHeader = ImageDataDirectory.Load(rdr);
            header.Reserved = ImageDataDirectory.Load(rdr);

            return header;
        }
    }
}
