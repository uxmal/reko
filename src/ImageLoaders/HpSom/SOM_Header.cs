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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.HpSom
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    [Endian(Endianness.BigEndian)]
    public struct sys_clock
    {
        public uint secs;
        public uint nanosecs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    [Endian(Endianness.BigEndian)]
    public struct SOM_Header
    {
        public ushort system_id;                // magic number - system
        public ushort a_magic;                  // magic number - file type
        public uint version_id;                 // version id; format=YYMMDDHH
        public sys_clock file_time;             // system clock- zero if unused
        public uint entry_space;                // index of space containing entry point
        public uint entry_subspace;             // index of subspace for entry point
        public uint entry_offset;               // offset of entry point
        public uint aux_header_location;        // auxiliary header location
        public uint aux_header_size;            // auxiliary header size
        public uint som_length;                 // length in bytes of entire som
        public uint presumed_dp;                // DP value assumed during compilation
        public uint space_location;             // location in file of space dictionary
        public uint space_total;                // number of space entries
        public uint subspace_location;          // location of subspace entries
        public uint subspace_total;             // number of subspace entries
        public uint loader_fixup_location;      // MPE/iX loader fixup
        public uint loader_fixup_total;         // number of loader fixup records
        public uint space_strings_location;     // file location of string area for space and subspace names
        public uint space_strings_size;         // size of string area for space and subspace names
        public uint init_array_location;        // reserved for use by system
        public uint init_array_total;           // reserved for use by system
        public uint compiler_location;          // location in file of module dictionary
        public uint compiler_total;             // number of modules
        public uint symbol_location;            // location in file of symbol dictionary
        public uint symbol_total;               // number of symbol records
        public uint fixup_request_location;     // location in file of fix_up requests
        public uint fixup_request_total;        // number of fixup requests
        public uint symbol_strings_location;    // file location of string area for module and symbol names
        public uint symbol_strings_size;        // size of string area for module and symbol names
        public uint unloadable_sp_location;     // byte offset of first byte of data for unloadable spaces
        public uint unloadable_sp_size;         // byte length of data for unloadable spaces
        public uint checksum;
    }
}
