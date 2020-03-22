#region License
/* 
 * Copyright (C) 2020 Natalia Portillo <claunia@claunia.com>.
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
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;

namespace Reko.ImageLoaders.MzExe
{
    public class LeImageLoader : ImageLoader
    {
        private readonly SortedList<Address, ImageSymbol> imageSymbols;
        private readonly Dictionary<uint, Tuple<Address, ImportReference>> importStubs;
        private readonly IDiagnosticsService diags;
        private readonly uint lfaNew;
        private IProcessorArchitecture arch;
        private LXHeader hdr;

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
            ///     Entry address of module
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

        public LeImageLoader(IServiceProvider services, string filename, byte[] imgRaw, uint e_lfanew) : base(services, filename, imgRaw)
        {
            diags = Services.RequireService<IDiagnosticsService>();
            lfaNew = e_lfanew;
            importStubs = new Dictionary<uint, Tuple<Address, ImportReference>>();
            imageSymbols = new SortedList<Address, ImageSymbol>();
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            this.arch = cfgSvc.GetArchitecture("x86-protected-32");
            var rdr = new LeImageReader(RawImage, this.lfaNew);

            IntPtr hdrPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LXHeader)));
            Marshal.Copy(RawImage, (int)lfaNew, hdrPtr, Marshal.SizeOf(typeof(LXHeader)));
            this.hdr = (LXHeader) Marshal.PtrToStructure(hdrPtr, typeof(LXHeader));
            Marshal.FreeHGlobal(hdrPtr);

            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }
    }
}