#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
    public struct SubspaceDictionaryRecord
    {
        public int space_index;
        public uint attributes;
        //            public uint access_control_bits :7; /* access for PDIR entries */
        //            public uint memory_resident :1; /* lock in memory during
        //execution */
        //            public uint dup_common :1; /* data name clashes allowed */
        //            public uint is_common :1; /* subspace is a common
        //block*/
        //            public uint is_loadable :1;
        //            public uint quadrant :2; /* quadrant request */
        //            public uint initially_frozen :1; /* must be locked into memory
        //when OS is booted */
        //            public uint is_first :1; /* must be first subspace */
        //            public uint code_only :1; /* must contain only code */
        //            public uint sort_key :8; /* subspace sort key */
        //            public uint replicate_init :1; /* init values replicated to
        //fill subspace_length */
        //            public uint continuation :1; /* subspace is a continuation*/
        //            public uint is_tspecific :1; /* Is thread specific ?*/
        //            public uint is_comdat :1; /* Is for COMDAT subspaces?*/
        //            public uint reserved :4;
        public int file_loc_init_value; /* file location or initialization value */
        public uint initialization_length;
        public uint subspace_start; /* starting offset */
        public uint subspace_length; /* number of bytes defined by this subspace */
        public uint alignment;
        //public uint reserved2 :5;
        //public uint alignment :27; /* alignment required for the
        //subspace (largest alignment
        //requested for any item in
        //the subspace) */
        public uint name; /* index of subspace name */
        public int fixup_request_index; /* index into fixup array */
        public uint fixup_request_quantity; /* number of fixup requests */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    [Endian(Endianness.BigEndian)]
    public struct symbol_dictionary_record
    {
        public uint bits;
        //uint hidden : 1;
        //uint secondary_def : 1;
        //uint symbol_type : 6;
        //uint symbol_scope : 4;
        //uint check_level : 3;
        //uint must_qualify : 1;
        //uint initially_frozen : 1;
        //uint memory_resident : 1;
        //uint is_common : 1;
        //uint dup_common : 1;
        //uint xleast : 2;
        //uint arg_reloc :10;
        public uint name;
        public uint qualifier_name;
        public uint info;
        //unsigned int has_long_return :1;
        //unsigned int no_relocation :1;
        //unsigned int is_comdat :1;
        //unsigned int reserved :5;
        //unsigned int symbol_info :24;
        public int symbol_value;

        public SymbolType type => (SymbolType) ((bits >> 24) & 0x3F);
    }

    public enum SymbolType
    {
        NULL = 0,       // Invalid symbol record. 
        ABSOLUTE = 1,   // Absolute constant.
        DATA = 2,       // Normal initialized data. Initialized data symbols including Fortran and
                        // Cobol initialized common data blocks, as well as C initialized data. Data
                        // can be either imported or exported. For example C construct “EXTERN
                        // INT I” would be imported data. And the C construct “INT I = 1” would be
                        // exported data.
        CODE = 3,       // Executable code
        PRI_PROG = 4,   // Primary program entry point.
        SEC_PROG = 5,   // Secondary Program entry point.
        ENTRY = 6,      // Any code entry point. Includes both primary and secondary entry points.
                        // Code entry point symbols may be used as targets of inter-space calls.
        STORAGE = 7,    // The value of the symbol is not known, but the length of the area is given.
        STUB = 8,       // This symbol marks an import (outbound) external call stub (EXTERNAL
                        // scope) or a parameter relocation stub (LOCAL scope). The linker may create
                        // an import stub for any unsatisfied code symbols, and the loader would
                        // be responsible for satisfying the reference by filling in the XRT entry
                        // allocated for this stub.
        MODULE = 9,     // This symbol is a source module name.
        SYM_EXT = 10,   // This type is used to indicate that an entry in the SOM symbol dictionary is
                        // an extension record of the current entry (previous valid symbol entry in the
                        // list).
        ARG_EXT = 11,   // This type is used to indicate that an entry in the SOM symbol dictionary is
                        // an extension record of the current entry(previous valid symbol entry in the
                        // list.
        MILLICODE = 12, // This is the name of the millicode routine.
        PLABEL = 13,    // This symbol defines an export stub for a procedure for which a procedure
                        // label has been generated.The loader must build an XRT entry for the procedure
                        // at the offset allocated by the linker.
        OCT_DIS = 14,   // This type is used to indicate that the pointer to a translated code segment
                        // exists, but has been disabled. Used by the Object Code Translator only.
        MILLI_EXT = 15, // This symbol defines the address of an external millicode subroutine.It
                        // should be treated as an constant.
        TSTORAGE = 16,  // This symbol defines Thread Specific data storage.
        COMDAT = 17,    // This type is used to identify the secondary subspaces of a COMDAT
    }
}
