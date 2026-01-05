#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Coff;

public class OptionalHeader
{
    public short magic;        /* see magic.h				*/
    public short vstamp;       /* version stamp			*/
    public int tsize;     /* text size in bytes, padded to FW
				   bdry					*/
    public int dsize;     /* initialized data "  "		*/
    public int bsize;     /* uninitialized data "   "		*/
#if u3b
	long	dum1;
	long	dum2;		/* pad to entry point	*/
#endif
    public uint entry;     /* entry pt.				*/
    public uint text_start;    /* base of text used for this file	*/
    public uint data_start;    /* base of data used for this file	*/
}

public class Ns32kOptionalHeader
{
    public short magic;         /* see magic.h				*/
    public short vstamp;        /* version stamp			*/
    public int tsize;           /* text size in bytes, padded to FW
				   bdry					*/
    public int dsize;     /* initialized data "  "		*/
    public int bsize;     /* uninitialized data "   "		*/
    public int msize;     /* size of module table */
    public int mod_start; /* start address of module table */
    public uint entry;     /* entry pt.				*/
    public uint text_start;    /* base of text used for this file	*/
    public uint data_start;    /* base of data used for this file	*/
    public short entry_mod;    /* module number of entry point */
    public ushort flags;
}
