#region License
/* 
 * Copyright (C) 2018-2023 Stefano Moioli <smxdev4@gmail.com>.
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
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Xbe
{
    public class Structures
    {
        public const UInt32 XBE_MAGIC = 0x48454258; //XBEH

        public const UInt32 XBE_XORKEY_ENTRYPOINT_DEBUG = 0x94859D4B;
        public const UInt32 XBE_XORKEY_ENTRYPOINT_RELEASE = 0xA8FC57AB;
        public const UInt32 XBE_XORKEY_ENTRYPOINT_CHIHIRO = 0x40B5C16E;

        public const UInt32 XBE_XORKEY_KERNELTHUNK_DEBUG = 0xEFB1F152;
        public const UInt32 XBE_XORKEY_KERNELTHUNK_RELEASE = 0x5B6D40B6;
        public const UInt32 XBE_XORKEY_KERNELTHUNK_CHIHIRO = 0x2290059D;

        [Flags]
        public enum InitializationFlags : UInt32
        {
            MountUnilityDrive = 1 << 0,
            FormatUtilityDrive = 1 << 1,
            Limit64Megabytes = 1 << 2,
            DontSetupHarddisk = 1 << 3
        }

        public struct XbeImageHeader
        {
            public UInt32 Magic;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Signature;
            public UInt32 BaseAddress;
            public UInt32 SizeOfHeaders;
            public UInt32 SizeOfImage;
            public UInt32 SizeOfImageHeader;
            public UInt32 TimeDate;
            public UInt32 CertificateAddress;
            public UInt32 NumberOfSections;
            public UInt32 SectionHeadersAddress;
            public InitializationFlags InitializationFlags;
            public UInt32 EntryPoint;
            public UInt32 TlsAddress;
            public UInt32 PeStackCommit;
            public UInt32 PeHeapReserve;
            public UInt32 PeHeapCommit;
            public UInt32 PeBaseAddress;
            public UInt32 PeSizeOfImage;
            public UInt32 PeChecksum;
            public UInt32 PeTimeDate;
            public UInt32 DebugPathNameAddress;
            public UInt32 DebugFileNameAddress;
            public UInt32 DebugUnicodeFileNameAddress;
            public UInt32 KernelImageThunkAddress;
            public UInt32 NonKernelImportDirectoryAddress;
            public UInt32 NumberOfLibraryVersions;
            public UInt32 LibraryVersionsAddress;
            public UInt32 KernelLibraryVersionAddress;
            public UInt32 XapiLibraryVersionAddress;
            public UInt32 LogoBitmapAddress;
            public UInt32 LogoBitmapSize;
        }

        [Flags]
        public enum XbeSectionFlags : UInt32
        {
            Writable = 1 << 0,
            Preload = 1 << 1,
            Executable = 1 << 2,
            InsertedFile = 1 << 3,
            HeadPageReadOnly = 1 << 4,
            TailPageReadOnly = 1 << 5
        }

        public struct XbeSectionHeader
        {
            public XbeSectionFlags Flags;
            public UInt32 VirtualAddress;
            public UInt32 VirtualSize;
            public UInt32 RawAddress;
            public UInt32 RawSize;
            public UInt32 SectionNameAddress;
            public UInt32 SectionNameReferenceCount;
            public UInt32 HeadSharedPageReferenceCountAddress;
            public UInt32 TailSharedPageReferenceCountAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] SectionDigest;
        }

        public struct XbeLibraryVersion
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] LibraryName;
            public UInt16 MajorVersion;
            public UInt16 MinorVersion;
            public UInt16 BuildVersion;
            public UInt16 LibraryFlags;
        }

        public struct XbeTls
        {
            public UInt32 DataStartAddress;
            public UInt32 DataEndAddress;
            public UInt32 TlsIndexAddress;
            public UInt32 TlsCallbackAddress;
            public UInt32 SizeOfZeroFill;
            public UInt32 Characteristics;
        }
    }
}
