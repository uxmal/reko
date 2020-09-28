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
    public struct SpaceDictionaryRecord
    {
        public uint name; /* index to subspace name */
        public uint attributes;
            //unsigned int is_loadable : 1; /* space is loadable */
            //unsigned int is_defined : 1; /* space is defined within file */
            //unsigned int is_private : 1; /* space is not sharable */
            //unsigned int has_intermediate_code: 1; /* contain intermediate code */
            //unsigned int is_tspecific : 1; /* is thread specific */
            //unsigned int reserved : 11; /* reserved for future expansion */
            //unsigned int sort_key : 8; /* sort key for space */
            //unsigned int reserved2 : 8; /* reserved for future expansion */
        public int space_number; /* space index */
        public int subspace_index; /* index into subspace dictionary*/
        public uint subspace_quantity; /* number of subspaces in space */
        public int loader_fix_index; /* loader usage*/
        public uint loader_fix_quantity; /* loader usage*/
        public int init_pointer_index; /* index into data(initialization) pointer array */
        public int init_pointer_quantity; /* number of data (init) pointers*/
    }
}
