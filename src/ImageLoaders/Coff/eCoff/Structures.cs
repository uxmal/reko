#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.IO;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Coff.eCoff
{
    using coff_addr = System.UInt32;        //$TODO generalize for 64-bit binaries
    using coff_off = System.UInt32;         //$TODO: generalize for 64-bit binaries
    using coff_word = System.UInt32;         //$TODO: generalize for 64-bit binaries
    using coff_ulong = System.UInt64;
    using coff_long = System.Int64;
    using coff_uint = System.UInt32;
    using coff_int = System.Int32;
    using coff_ushort = System.UInt16;
    using coff_short = System.Int16;
    using coff_ubyte = System.Byte;
    using coff_byte = System.SByte;


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Endian(Endianness.BigEndian)]
    public struct filehdr
    {
        public coff_ushort f_magic;
        public coff_ushort f_nscns;
        public coff_int f_timdat;
        public coff_off f_symptr;
        public coff_int f_nsyms;
        public coff_ushort f_opthdr;
        public coff_ushort f_flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Endian(Endianness.BigEndian)]
    public struct aouthdr
    {
        public coff_ushort magic;
        public coff_ushort vstamp;
        //public coff_ushort bldrev;
        //public coff_ushort padcell;
        public coff_uint tsize;         //$ 64-bit
        public coff_uint dsize;         //$ 64-bit
        public coff_uint bsize;         //$ 64-bit
        public coff_addr entry;
        public coff_addr text_start;
        public coff_addr data_start;
        public coff_addr bss_start;
        public coff_uint gprmask;
        public coff_word fprmask;
        public coff_uint gp_value;      //$ 64-bit
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Endian(Endianness.BigEndian)]
    public struct scnhdr
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] s_name;
        public coff_addr s_paddr;
        public coff_addr s_vaddr;
        public coff_uint s_size;        //$ 64-bit
        public coff_off s_scnptr;
        public coff_off s_relptr;
        public coff_uint s_lnnoptr;       //$ 64-bit
        public coff_ushort s_nreloc;
        public coff_ushort s_nlnno;
        public eCoffSectionFlags s_flags;
    }

    public enum eCoffSectionFlags : uint
    {
        STYP_REG = 0x00000000, // Regular section: allocated, relocated, loaded. User section flags have this setting.
        STYP_TEXT = 0x00000020, // Text only
        STYP_DATA = 0x00000040, // Data only
        STYP_BSS = 0x00000080, // Bss only
        STYP_RDATA = 0x00000100, // Read-only data only
        STYP_SDATA = 0x00000200, // Small data only
        STYP_SBSS = 0x00000400, // Small bss only
        STYP_UCODE = 0x00000800, // Obsolete
        STYP_GOT = 0x00001000, // Global offset table
        STYP_DYNAMIC = 0x00002000, // Dynamic linking information
        STYP_DYNSYM = 0x00004000, // Dynamic linking symbol table
        STYP_REL_DYN = 0x00008000, // Dynamic relocation information
        STYP_DYNSTR = 0x00010000, // Dynamic linking symbol table
        STYP_HASH = 0x00020000, // Dynamic symbol hash table
        STYP_DSOLIST = 0x00040000, // Shared library dependency list
        STYP_MSYM = 0x00080000, // Additional dynamic linking symbol table
        STYP_CONFLICT = 0x00100000, // Additional dynamic linking information
        STYP_FINI = 0x01000000, // Termination text only
        STYP_COMMENT = 0x02000000, // Comment section
        STYP_RCONST = 0x02200000, // Read-only constants
        STYP_XDATA = 0x02400000, // Exception scope table
        STYP_TLSDATA = 0x02500000, // Initialized TLS data
        STYP_TLSBSS = 0x02600000, // Uninitialized TLS data
        STYP_TLSINIT = 0x02700000, // Initialization for TLS data
        STYP_PDATA = 0x02800000, // Exception procedure table
        STYP_LITA = 0x04000000, // Address literals only
        STYP_LIT8 = 0x08000000, // 8-byte literals only
        STYP_EXTMASK = 0x0ff00000, // Identifies bits used for multiple bit flag values.
        STYP_LIT4 = 0x10000000, // 4-byte literals only
        S_NRELOC_OVFL = 0x20000000, // Indicates that section header field s_nreloc overflowed
        STYP_ECOFF_LIB = 0x40000000, 
        STYP_INIT = 0x80000000, // Initialization text only
    }


}