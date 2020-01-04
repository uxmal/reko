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
    public struct DlHeader
    {
        public int hdr_version; /* header version number */
        public int ltptr_value; /* data offset of LT pointer (R19) */
        public int shlib_list_loc; /* text offset of shlib list */
        public int shlib_list_count; /* count of items in shlib list */
        public uint import_list_loc; /* text offset of import list */
        public int import_list_count; /* count of items in import list */
        public int hash_table_loc; /* text offset of export hash table */
        public int hash_table_size; /* count of slots in export hash table */
        public int export_list_loc; /* text offset of export list */
        public int export_list_count; /* count of items in export list */
        public uint string_table_loc; /* text offset of string table */
        public int string_table_size; /* length in bytes of string table */
        public int dreloc_loc; /* text offset of dynamic reloc records */
        public int dreloc_count; /* number of dynamic relocation records */
        public uint dlt_loc; /* data offset of data linkage table */
        public uint plt_loc; /* data offset of procedure linkage table */
        public int dlt_count; /* number of dlt entries in linkage table*/
        public int plt_count; /* number of plt entries in linkage table*/
        public short highwater_mark; /* highest version number seen in lib or in shlib list*/
        public short flags; /* various flags */
        public int export_ext_loc; /* text offset of export extension tbl */
        public int module_loc; /* text offset of module table*/
        public int module_count; /* number of module entries */
        public int elaborator; /* import index of elaborator */
        public int initializer; /* import index of initializer */
        public int embedded_path; /* index into string table for search path */
                           /* index must be > 0 to be valid */
        public int initializer_count; /* count of items in initializer import list*/
        public int tdsize; /* size of the TSD area */
        public int fastbind_list_loc; /* text-relative offset of fastbind info */
    };
}