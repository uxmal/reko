#region License
/* 
 * Copyright (C) 2020-2023 Natalia Portillo <claunia@claunia.com>.
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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.ImageLoaders.MzExe
{
    /// <summary>
    /// Image loader for the Linear Executable (LE) binary file format.
    /// </summary>
    /// <remarks>
    /// http://faydoc.tripod.com/formats/exe-LE.htm
    /// vmm.inc
    /// https://www-user.tu-chemnitz.de/~heha/viewzip.cgi/hs/doslfn.zip/vmm.inc?auto=ASM
    /// 
    /// https://defacto2.net/f/b22f966
    /// </remarks>
    public class LeImageLoader : ProgramImageLoader
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(LeImageLoader), nameof(LeImageLoader));

        /// <summary>
        ///     Linear Executable signature, "LE"
        /// </summary>
        const ushort SIGNATURE16 = 0x454C;
        /// <summary>
        ///     Linear eXecutable signature, "LX"
        /// </summary>
        const ushort SIGNATURE = 0x584C;

        private readonly SortedList<Address, ImageSymbol> imageSymbols;
        private readonly Dictionary<uint, (Address, ImportReference)> importStubs;
        private readonly IEventListener listener;
        private readonly uint lfaNew;
        private IProcessorArchitecture arch;
        private LXHeader hdr;
        private List<string> moduleNames;

        public LeImageLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw, uint e_lfanew) 
            : base(services, imageLocation, imgRaw)
        {
            listener = Services.RequireService<IEventListener>();
            lfaNew = e_lfanew;
            importStubs = new Dictionary<uint, (Address, ImportReference)>();
            imageSymbols = new SortedList<Address, ImageSymbol>();
            PreferredBaseAddress = Address.Ptr32(0x0010_0000);  //$REVIEW: arbitrary address.
            this.arch = null!;
            this.moduleNames = null!;
        }
        
        /// <summary>
        ///     Executable module flags.
        /// </summary>
        [Flags]
        public enum ModuleFlags : uint
        {
            /// <summary>
            ///     Per-Process Library Initialization
            /// </summary>
            PerProcessLibrary = 0x04,
            /// <summary>
            ///     Internal fixups for the module have been applied
            /// </summary>
            InternalFixups = 0x10,
            /// <summary>
            ///     External fixups for the module have been applied
            /// </summary>
            ExternalFixups = 0x20,
            /// <summary>
            ///     Incompatible with Presentation Manager
            /// </summary>
            PMIncompatible = 0x100,
            /// <summary>
            ///     Compatible with Presentation Manager
            /// </summary>
            PMCompatible = 0x200,
            /// <summary>
            ///     Uses Presentation Manager
            /// </summary>
            UsesPM = 0x300,
            /// <summary>
            ///     Module is not loadable. Contains errors or is being linked
            /// </summary>
            NotLoadable = 0x2000,
            /// <summary>
            ///     Library module
            /// </summary>
            Library = 0x8000,
            /// <summary>
            ///     Protected Memory Library module
            /// </summary>
            ProtectedMemoryLibrary = 0x18000,
            /// <summary>
            ///     Physical Device Driver module
            /// </summary>
            PhysicalDeviceDriver = 0x20000,
            /// <summary>
            ///     Virtual Device Driver module
            /// </summary>
            VirtualDeviceDriver = 0x28000,
            /// <summary>
            ///     Per-process Library Termination
            /// </summary>
            PerProcessTermination = 0x40000000
        }

        public enum TargetCpu : ushort
        {
            Unknown = 0,
            i286 = 1,
            i386 = 2,
            i486 = 3,
            Pentium = 4,
            i860 = 0x20,
            N11 = 0x21,
            MIPS1 = 0x40,
            MIPS2 = 0x41,
            MIPS3 = 0x42
        }

        /// <summary>
        ///     Target operating system.
        /// </summary>
        public enum TargetOS : ushort
        {
            Unknown = 0,
            OS2 = 1,
            Windows = 2,
            DOS = 3,
            Win32 = 4,
            NT = 0x20,
            Posix = 0x21
        }

        [Flags]
        public enum ObjectFlags
        {
            Readable = 0x0001,
            Writable = 0x0002,
            Executable = 0x0004,
            Resource = 0x0008,
            Discardable = 0x0010,
            Shared = 0x0020,
            Preload = 0x0040,
            Invalid = 0x0080,
            Zeroed = 0x0100,
            Resident = 0x0200,
            Contiguous = 0x0300,
            LongLockable = 0x0400,
            Reserved = 0x0800,
            Alias1616Required = 0x1000,
            BigDefaultBitSetting = 0x2000,
            Conforming = 0x4000,
            Privilege = 0x8000
        }

        enum PageTableAttributes : ushort
        {
            /// <summary>
            ///     Offset from preload page section
            /// </summary>
            LegalPhysicalPage = 0,
            /// <summary>
            ///     Offset from iterated page section
            /// </summary>
            IteratedDataPage = 1,
            Invalid = 2,
            Zeroed = 3,
            RangeOfPages = 4
        }

        enum PageTableAttributes16 : byte
        {
            /// <summary>
            ///     Offset from preload page section
            /// </summary>
            LegalPhysicalPage = (byte) PageTableAttributes.LegalPhysicalPage,
            /// <summary>
            ///     Offset from iterated page section
            /// </summary>
            IteratedDataPage = (byte) PageTableAttributes.IteratedDataPage,
            Invalid = (byte) PageTableAttributes.Invalid,
            Zeroed = (byte) PageTableAttributes.Zeroed,
            RangeOfPages = (byte) PageTableAttributes.RangeOfPages
        }

        /// <summary>
        ///     Header for a Microsoft Linear Executable and IBM Linear eXecutable
        /// </summary>
        [StructLayout(LayoutKind.Sequential /*, Pack = 2*/)]
        public struct LXHeader
        {
            /// <summary>
            ///     Executable signature
            /// </summary>
            public ushort signature;
            /// <summary>
            ///     Byte ordering
            /// </summary>
            public byte byte_order;
            /// <summary>
            ///     Word ordering
            /// </summary>
            public byte word_order;
            /// <summary>
            ///     Format level, should be 0
            /// </summary>
            public ushort format_minor;
            public ushort format_major;
            /// <summary>
            ///     Type of CPU required by this executable to run
            /// </summary>
            public TargetCpu cpu_type;
            /// <summary>
            ///     Type of operating system requires by this executable to run
            /// </summary>
            public TargetOS os_type;
            /// <summary>
            ///     Executable version
            /// </summary>
            public ushort module_minor;
            public ushort module_major;
            /// <summary>
            ///     Executable flags
            /// </summary>
            public ModuleFlags module_flags;
            /// <summary>
            ///     Pages contained in this module
            /// </summary>
            public uint module_pages_no;
            /// <summary>
            ///     Object number to which the Entry Address is relative
            /// </summary>
            public uint eip_object;
            /// <summary>
            ///     Entry address of module, relative to the base address of 
            ///     the object whose index is in <see cref="eip_object" />.
            /// </summary>
            public uint eip;
            /// <summary>
            ///     Object number to which the ESP is relative
            /// </summary>
            public uint esp_object;
            /// <summary>
            ///     Starting stack address of module
            /// </summary>
            public uint esp;
            /// <summary>
            ///     Size of one page
            /// </summary>
            public uint page_size;
            /// <summary>
            ///     Shift left bits for page offsets
            ///     LE: Last page size in bytes
            /// </summary>
            public uint page_off_shift;
            /// <summary>
            ///     Total size of the fixup information
            /// </summary>
            public uint fixup_size;
            /// <summary>
            ///     Checksum for fixup information
            /// </summary>
            public uint fixup_checksum;
            /// <summary>
            ///     Size of memory resident tables
            /// </summary>
            public uint loader_size;
            /// <summary>
            ///     Checksum for loader section
            /// </summary>
            public uint loader_checksum;
            /// <summary>
            ///     Object table offset
            /// </summary>
            public uint obj_table_off;
            /// <summary>
            ///     Object table count
            /// </summary>
            public uint obj_no;
            /// <summary>
            ///     Object page table offset
            /// </summary>
            public uint obj_page_table_off;
            /// <summary>
            ///     Object iterated pages offset
            /// </summary>
            public uint obj_iter_pages_off;
            /// <summary>
            ///     Resource table offset
            /// </summary>
            public uint resource_table_off;
            /// <summary>
            ///     Entries in resource table
            /// </summary>
            public uint resource_entries;
            /// <summary>
            ///     Resident name table offset
            /// </summary>
            public ushort resident_names_off;
            /// <summary>
            ///     Entry table offset
            /// </summary>
            public uint entry_table_off;
            /// <summary>
            ///     Module format directives table offset
            /// </summary>
            public uint directives_off;
            /// <summary>
            ///     Entries in module format directives table
            /// </summary>
            public uint directives_no;
            /// <summary>
            ///     Fixup page table offset
            /// </summary>
            public uint fixup_page_table_off;
            /// <summary>
            ///     Fixup record table offset
            /// </summary>
            public uint fixup_record_table_off;
            /// <summary>
            ///     Import module name table offset
            /// </summary>
            public uint import_module_table_off;
            /// <summary>
            ///     Entries in the import module name table
            /// </summary>
            public uint import_module_entries;
            /// <summary>
            ///     Import procedure name table offset
            /// </summary>
            public uint import_proc_table_off;
            /// <summary>
            ///     Per-page checksum table offset
            /// </summary>
            public uint perpage_checksum_off;
            /// <summary>
            ///     Data pages offset
            /// </summary>
            public uint data_pages_off;
            /// <summary>
            ///     Number of preload pages for this module
            /// </summary>
            public uint preload_pages_no;
            /// <summary>
            ///     Non-resident name table offset
            /// </summary>
            public uint nonresident_name_table_off;
            /// <summary>
            ///     Number of bytes in the non-resident name table
            /// </summary>
            public uint nonresident_name_table_len;
            /// <summary>
            ///     Non-resident name table checksum
            /// </summary>
            public uint nonresident_name_table_checksum;
            /// <summary>
            ///     The auto data segment object number
            /// </summary>
            public uint auto_ds_obj_no;
            /// <summary>
            ///     Debug information offset
            /// </summary>
            public uint debug_info_off;
            /// <summary>
            ///     Debug information length
            /// </summary>
            public uint debug_info_len;
            /// <summary>
            ///     Instance pages in preload section
            /// </summary>
            public uint instance_preload_no;
            /// <summary>
            ///     Instance pages in demand section
            /// </summary>
            public uint instance_demand_no;
            /// <summary>
            ///     Heap size added to the auto ds object
            /// </summary>
            public uint heap_size;
            // Following is only defined for Windows VxDs
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] reserved;
            public uint win_res_off;
            public uint win_res_len;
            public ushort device_id;
            public byte ddk_minor;
            public byte ddk_major;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        struct ObjectTableEntry
        {
            public uint VirtualSize;
            public uint RelocationBaseAddress;
            public ObjectFlags ObjectFlags;
            public uint PageTableIndex;
            public uint PageTableEntries;
            /// <summary>
            ///     Only used in LE
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Name;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        struct ObjectPageTableEntry
        {
            public uint PageDataOffset;
            public ushort DataSize;
            public PageTableAttributes Flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        struct ObjectPageTableEntry16
        {
            public ushort High;
            public byte Low;
            public PageTableAttributes16 Flags;
        }

        public class Segment
        {
            public uint DataOffset;
            public uint DataLength;
            public ObjectFlags Flags;
            public Address? Address;
            public string? Name;
            public uint BaseAddress;
        }


        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            addrLoad ??= PreferredBaseAddress;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            this.arch = cfgSvc.GetArchitecture("x86-protected-32")!;
            var rdr = new LeImageReader(RawImage, this.lfaNew);

            var hdrReader = new StructureReader<LXHeader>(rdr);
            this.hdr = hdrReader.Read();
            DumpHeader(hdr);
            LoadModuleTable();
            var leSegs = LoadSegmentTable();

            var segments = MakeSegmentMap(addrLoad, leSegs);
            var platform = MakePlatform();
            var program  = new Program(segments, arch, platform);
            var eip_module = leSegs[hdr.eip_object - 1].BaseAddress;
            var addrEntry = Address.Ptr32(eip_module + hdr.eip);
            program.EntryPoints.Add(addrEntry, ImageSymbol.Procedure(arch, addrEntry, "_le_entry"));
            return program;
        }

        void LoadModuleTable()
        {
            var rdr = new LeImageReader(RawImage, lfaNew+ hdr.import_module_table_off);
            moduleNames = new List<string>();

            if (hdr.import_module_table_off == 0 || hdr.import_module_entries == 0)
                return;

            for (int i = 0; i < hdr.import_module_entries; i++)
            {
                int len = rdr.ReadByte();
                var abModuleName = rdr.ReadBytes(len);
                var moduleName = Encoding.ASCII.GetString(abModuleName);
                moduleNames.Add(moduleName);
            }
        }

        Segment[] LoadSegmentTable()
        {
            var objectTableEntries = new ObjectTableEntry[hdr.obj_no];
            var objectPageTableEntries = new ObjectPageTableEntry[hdr.module_pages_no];

            var rdr = new LeImageReader(RawImage, hdr.obj_table_off + lfaNew);
            var objTblEntryReader = new StructureReader<ObjectTableEntry>(rdr);

            for (int i = 0; i < hdr.obj_no; i++)
                objectTableEntries[i] = objTblEntryReader.Read();
            
            rdr = new LeImageReader(RawImage, hdr.obj_page_table_off + lfaNew);

            if (hdr.signature == SIGNATURE16)
            {
                var objPageTblEntry16Reader = new StructureReader<ObjectPageTableEntry16>(rdr);

                for (int i = 0; i < hdr.module_pages_no; i++)
                {
                    ObjectPageTableEntry16 page16 = objPageTblEntry16Reader.Read();

                    int pageNo = page16.High + page16.Low;

                    objectPageTableEntries[i] = new ObjectPageTableEntry
                    {
                        DataSize = (ushort) hdr.page_size,
                        Flags = (PageTableAttributes) page16.Flags,
                        PageDataOffset = (uint) ((pageNo - 1) * hdr.page_size)
                    };
                }
            }
            else
            {
                var objPageTblEntryReader = new StructureReader<ObjectPageTableEntry>(rdr);
                for (int i = 0; i < hdr.module_pages_no; i++)
                    objectPageTableEntries[i] = objPageTblEntryReader.Read();
            }

            int debugSections = 0;
            int winrsrcSections = 0;

            if (hdr.debug_info_len > 0) debugSections = 1;
            if (hdr.win_res_len > 0) winrsrcSections = 1;

            Segment[] sections = new Segment[objectTableEntries.Length + debugSections + winrsrcSections];
            int texts = 0;
            int rsrcs = 0;
            int rodatas = 0;
            int bsss = 0;
            int datas = 0;

            static string SegmentName(string prefix, ref int count)
            {
                string name = (count > 0)
                    ? $"{prefix}{count}"
                    : prefix;
                ++count;
                return name;
            }

            for (int i = 0; i < objectTableEntries.Length; i++)
            {
                sections[i] = new Segment { Flags = objectTableEntries[i].ObjectFlags };
                if (objectTableEntries[i].ObjectFlags.HasFlag(ObjectFlags.Resource)) sections[i].Name = SegmentName(".rsrc", ref rsrcs);
                else if (objectTableEntries[i].ObjectFlags.HasFlag(ObjectFlags.Executable)) sections[i].Name = SegmentName(".text", ref texts);
                else if (!objectTableEntries[i].ObjectFlags.HasFlag(ObjectFlags.Writable)) sections[i].Name = SegmentName(".rodata", ref rodatas);
                else if (new LeImageReader(objectTableEntries[i].Name).ReadCString(PrimitiveType.Char, Encoding.ASCII).ToString().ToLower() == "bss")
                    sections[i].Name = SegmentName(".bss", ref bsss);
                else if (!string.IsNullOrWhiteSpace(new LeImageReader(objectTableEntries[i].Name).ReadCString(PrimitiveType.Char, Encoding.ASCII).ToString().Trim()))
                    sections[i].Name = new LeImageReader(objectTableEntries[i].Name).ReadCString(PrimitiveType.Char, Encoding.ASCII).ToString().Trim();
                else sections[i].Name = SegmentName(".data", ref datas);

                if (objectTableEntries[i].PageTableEntries == 0 ||
                   objectTableEntries[i].PageTableIndex > objectPageTableEntries.Length)
                {
                    sections[i].DataLength = objectTableEntries[i].VirtualSize;
                    continue;
                }

                int shift = (int) (hdr.signature == SIGNATURE16 ? 0 : hdr.page_off_shift);

                if (objectPageTableEntries[objectTableEntries[i].PageTableIndex - 1]
                  .Flags.HasFlag(PageTableAttributes.IteratedDataPage))
                    sections[i].DataOffset =
                        (objectPageTableEntries[objectTableEntries[i].PageTableIndex - 1].PageDataOffset << shift) +
                        hdr.obj_iter_pages_off;
                else if (objectPageTableEntries[objectTableEntries[i].PageTableIndex - 1]
                       .Flags.HasFlag(PageTableAttributes.LegalPhysicalPage))
                    sections[i].DataOffset =
                        (objectPageTableEntries[objectTableEntries[i].PageTableIndex - 1].PageDataOffset << shift) +
                        hdr.data_pages_off;
                else sections[i].DataOffset = 0;

                sections[i].DataLength = 0;
                for (int j = 0; j < objectTableEntries[i].PageTableEntries; j++)
                    sections[i].DataLength += objectPageTableEntries[j + objectTableEntries[i].PageTableIndex - 1].DataSize;

                if (sections[i].DataOffset + sections[i].DataLength > RawImage.Length)
                    sections[i].DataLength = (uint)RawImage.Length - sections[i].DataOffset;

                sections[i].BaseAddress = objectTableEntries[i].RelocationBaseAddress;
            }

            if (winrsrcSections > 0)
                sections[sections.Length - debugSections - winrsrcSections] = new Segment
                {
                    Name = ".rsrc",
                    DataLength = hdr.win_res_len,
                    DataOffset = hdr.win_res_off
                };

            if (debugSections > 0)
                sections[sections.Length - debugSections] = new Segment
                {
                    Name = ".debug",
                    DataLength = hdr.debug_info_len,
                    DataOffset = hdr.debug_info_off
                };

            return sections;
        }

        private SegmentMap MakeSegmentMap(Address addrLoad, Segment[] leSegments)
        {
            var segments = leSegments.Select(s => MakeImageSegment(s, addrLoad)).ToArray();
            return new SegmentMap(addrLoad, segments);
        }

        private ImageSegment MakeImageSegment(Segment seg, Address addrLoad)
        {
            AccessMode access = 0;
            if ((seg.Flags & ObjectFlags.Readable) != 0)
                access |= AccessMode.Read;
            if ((seg.Flags & ObjectFlags.Writable) != 0)
                access |= AccessMode.Write;
            if ((seg.Flags & ObjectFlags.Executable) != 0)
                access |= AccessMode.Execute;

            //$REVIEW: the address calculation doesn't take into account zero-filled pages. 
            var mem = new ByteMemoryArea(Address.Ptr32(seg.BaseAddress), new byte[seg.DataLength]);
            Buffer.BlockCopy(
                RawImage, (int)seg.DataOffset,
                mem.Bytes, 0,
                mem.Bytes.Length);
            var imgSegment = new ImageSegment(seg.Name!, mem, access);
            return imgSegment;
        }

        private IPlatform MakePlatform()
        {
            string envName;
            if (this.hdr.signature == SIGNATURE16)
            {
                envName = "ms-dos-386";
            }
            else
            {
                switch (this.hdr.os_type)
                {
                case TargetOS.OS2:
                    envName = "os2-32";
                    break;
                case TargetOS.Win32:
                    if (hdr.module_flags.HasFlag(ModuleFlags.VirtualDeviceDriver))
                    {
                        envName = "win-vmm";
                    }
                    else
                    {
                        envName = "win32";
                    }
                    break;
                case TargetOS.DOS:
                    envName = "ms-dos-386";
                    break;
                default:
                    listener.Error($"Unsupported operating environment {this.hdr.os_type}.");
                    return new DefaultPlatform(this.Services, this.arch);
                }
            }
            var platform = Services.RequireService<IConfigurationService>()
                .GetEnvironment(envName)
                .Load(Services, arch);
            return platform;
        }


        [Conditional("DEBUG")]
        private void DumpHeader(in LXHeader hdr)
        {
            trace.Inform("== LE/LX image header ======");
        }
    }
}
