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
        public uint subspace_length; /* number of bytes defined
by this subspace */
        public uint alignment;
        //public uint reserved2 :5;
        //public uint alignment :27; /* alignment required for the
        //subspace (largest alignment
        //requested for any item in
        //the subspace) */
        public uint name; /* index of subspace name */
        public int fixup_request_index; /* index into fixup array */
        public uint fixup_request_quantity; /* number of fixup requests */
    };
}
