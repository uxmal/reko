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
    public class FileHeader
    {
        public ushort f_magic;         // magic number
        public ushort f_nscns;         // number of sections
        public uint f_timdat;          // time & date stamp
        public uint f_symptr;          // file pointer to symtab
        public uint f_nsyms;           // number of symtab entries
        public ushort f_opthdr;        // sizeof(optional hdr)
        public ushort f_flags;         // flags
    }
}
