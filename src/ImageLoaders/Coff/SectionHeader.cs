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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Coff
{
    public class SectionHeader
    {
        public string s_name; // [8];  /* section name                     */
        public uint s_paddr;    /* physical address, aliased s_nlib */
        public uint s_vaddr;    /* virtual address                  */
        public uint s_size;     /* section size                     */
        public uint s_scnptr;   /* file ptr to raw data for section */
        public uint s_relptr;   /* file ptr to relocation           */
        public uint s_lnnoptr;  /* file ptr to line numbers         */
        public ushort s_nreloc;   /* number of relocation entries     */
        public ushort s_nlnno;    /* number of line number entries    */
        public uint s_flags;    /* flags                            */
    }
}
